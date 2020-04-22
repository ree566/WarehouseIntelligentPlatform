using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using SAP.Middleware.Connector;
using System.Net;
using NPOI.XSSF.UserModel;
//-- XSSF 用來產生Excel 2007檔案（.xlsx）

using NPOI.SS.UserModel;
//-- v.1.2.4起 新增的。
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System.IO;

public partial class WH_DNPREPARE_DETAIL : System.Web.UI.Page
{
    string sAUTHORITY = "";
    string sPREPARATION_FLAG = "";
    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
    #region Add 庫存&待驗數量 by Apple at 20190215
    string strLGORT = "0015,0008,0012,0055,0058", strWERKS = "TWM3";
    #endregion
    string sAUFNR = "", MAT_CAPTION_ORDER = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PREPARATION_FLAG"] != null)
        {
            sPREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
            sAUTHORITY = Session["AUTHORITY"].ToString();
        }
        if (!Page.IsPostBack)
        {

            capData();
        }
    }
    private void capData()
    {
        if (Request["AUFNR"] != null) { sAUFNR = Request["AUFNR"].ToString(); }
        if (Request["MAT_CAPTION_ORDER"] != null) { MAT_CAPTION_ORDER = Request["MAT_CAPTION_ORDER"].ToString(); }

        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT  A.[ZWERKS],A.[Detail_ID],A.[VBELN],A.[POSNR],A.[MATNR],A.[MAKTX],A.[ZLGORT],A.[LFIMG],A.[PUB_QTY],A.[PUB_QTY_NPT],(A.[LFIMG]-(A.[PUB_QTY]+A.[PUB_QTY_NPT])) OPEN_QTY");
        sb.Append(",C.ENTRY_QNT,C.UNPULL_QNT,C.ENTRY_QNT_TODAY ");
        sb.Append(",A.[ZPRDPLC],A.[ZTRADEMARK],A.[MAT_EMP_NO],A.[Detail_REMARK],CASE WHEN A.[ZPRDPLC] <> 'TAIWAN'AND A.[ZPRDPLC] <> ''  THEN 'X' ELSE '' END IMPORT FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A");

        sb.Append(" LEFT JOIN (SELECT [MATERIAL],ISNULL(SUM(CASE WHEN [CACHE_NAME] is not null AND [CACHE_NAME] <> '調整帳務' AND [STATUS]=0 AND [DISPLAY_FALG]='Y' THEN [ENTRY_QNT] ELSE 0 END ),0) ENTRY_QNT ");
        sb.Append(",ISNULL(SUM(CASE WHEN [CACHE_NAME] is null  AND [STATUS]=0 AND [DISPLAY_FALG]='Y' THEN [ENTRY_QNT] ELSE 0 END ),0) UNPULL_QNT ");
        sb.Append(",ISNULL(SUM(CASE WHEN CONVERT(DATE,[ED_DATE])=CONVERT(DATE,GETDATE())  AND [STATUS]=1 AND [DISPLAY_FALG]='Y' THEN [ENTRY_QNT] ELSE 0 END ),0) ENTRY_QNT_TODAY ");
        sb.Append(" FROM [TWM8].[dbo].[WH_STORAGE] GROUP BY [MATERIAL] ) C ON C.[MATERIAL]=A.[MATNR]");

        sb.AppendFormat("WHERE [VBELN]='{0}'", sAUFNR);
        if (!MAT_CAPTION_ORDER.Equals("ALL"))
        {
            sb.AppendFormat(" AND [MAT_EMP_NO] LIKE '%{0}%'", MAT_CAPTION_ORDER);
        }
        sb.Append(" ORDER BY A.[POSNR]  ");

        DataTable dt = ATMCdb.reDt(sb.ToString());
        #region Add 庫存&待驗數量 by Apple at 20190215
        dt = ZGBSN(dt, SAPRFC.getSAPDB());
        #endregion
        gdv_WH_DNPREPARE_DETAIL.DataSource = dt;
        ViewState["GridViewData"] = sb.ToString();
        gdv_WH_DNPREPARE_DETAIL.DataBind();
    }


    #region RFC接口 料號庫存查詢 add by Apple at 20190215
    private DataTable ZGBSN(DataTable dt, RfcDestination dest)
    {
        DataSet oDs = new DataSet();
        DataTable distinctValues = new DataTable();
        try
        {

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
            IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
            string[] sArrayLGORT = strLGORT.Split(',');

            DataView view = new DataView(dt);
            distinctValues = view.ToTable(true, "MATNR");

            foreach (string SubstrLGORT in sArrayLGORT)
            {
                foreach (DataRow od in distinctValues.Rows)
                {
                    string strMaterial = od["MATNR"].ToString();
                    IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
                    MARD.SetValue("MATNR", strMaterial);
                    MARD.SetValue("WERKS", strWERKS);
                    MARD.SetValue("LGORT", SubstrLGORT);
                    ZMARD_INPUT.Insert(MARD);
                }

            }

            rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
            rfc.Invoke(dest);

            IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
            DataTable dtZMARD_OUTPUT = SAPRFC.ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");
            oDs.Merge(dtZMARD_OUTPUT);

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }

        dt = InventoryIntegrationMissingincoming(dt, oDs);

        return dt;

    }
    #endregion
    #region 整合庫存資料 add by Apple at 20190215
    private DataTable InventoryIntegrationMissingincoming(DataTable dt, DataSet oDs)
    {
        var linqStament = from p in oDs.Tables["ZMARD_OUTPUT"].AsEnumerable()
                          group p by new
                          {
                              MATNR = p.Field<string>("MATNR")
                          } into g
                          //let row = g.First()
                          select new
                          {
                              MATNR = g.Key.MATNR
                              ,
                              //Total = g.Sum(x => Convert.ToInt32(x["LABST"]) + Convert.ToInt32(x["INSME"]))
                              LABST = g.Sum(x => Convert.ToDouble(x["LABST"]))
                              ,
                              INSME = g.Sum(x => Convert.ToDouble(x["INSME"]))
                              ,
                              LGPBE = string.Join(",", g.Select(x => Convert.ToString(x["LGPBE"])))
                          };

        // dt.Columns.Add("Total");

        dt.Columns.Add("LABST"); //庫存
        dt.Columns.Add("INSME"); //待驗
        dt.Columns.Add("LGPBE"); //儲位
        foreach (var query in linqStament)
        {
            foreach (DataRow od in dt.Rows)
            {
                if (od["MATNR"].ToString() == query.MATNR.ToString().TrimStart('0'))
                {
                    // od["Total"] = query.Total;
                    od["LABST"] = query.LABST;
                    od["INSME"] = query.INSME;
                    od["LGPBE"] = query.LGPBE;
                }
            }
        }
        dt.TableName = "Integration";
        return dt;
    }

    #endregion

    protected void gdv_WH_DNPREPARE_DETAIL_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string ZLGORT = e.Row.Cells[13].Text;
            if (ZLGORT != "0000" & ZLGORT != "0005") { e.Row.Cells[13].BackColor = System.Drawing.Color.Yellow; }
            string INSME = e.Row.Cells[12].Text;
            if (INSME != "0") { e.Row.Cells[12].BackColor = System.Drawing.Color.Yellow; }
            string ENMNGSITEMQTY_NPT = e.Row.Cells[16].Text;
            if (ENMNGSITEMQTY_NPT != "0") { e.Row.Cells[16].BackColor = System.Drawing.Color.Red; }
            string OPENITEMQTY = e.Row.Cells[17].Text;
            if (OPENITEMQTY != "0") { e.Row.Cells[17].BackColor = System.Drawing.Color.Red; }

        }
    }



    protected void btn_ADDPUB_QTY_Click(object sender, EventArgs e)
    {
        //string sparam = Server.HtmlEncode("< div class='alert alert-warning alert-dismissible fade show' role='alert'>");
        //sparam += Server.HtmlEncode("<button type='button' class='close' data-dismiss='alert' aria-label='Close'>");
        //sparam += Server.HtmlEncode("<span aria-hidden='true'>&times;</span></button>");
        //sparam += Server.HtmlEncode("<strong>Holy guacamole!</strong> You should check in on some of those fields below.</div>");
        string EMP_NO = txt_EMP_NO.Text.Trim();


        //if (ATMCdb.CHECKWHEMPLR_ID(EMP_NO)!="Y")
        //{
        //    string title = "非倉庫人員";
        //    string body = "非倉庫人員無法執行此作業";
        //    ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + title + "', '" + body + "');", true);
        //    return;
        //}
        ADDSKINWIPITEM(EMP_NO);
    }
    public void ADDSKINWIPITEM(string EMP_NO)
    {
        string nresult = "";
        if (Request["AUFNR"] != null) { sAUFNR = Request["AUFNR"].ToString(); }
        for (int i = 0; i < gdv_WH_DNPREPARE_DETAIL.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)gdv_WH_DNPREPARE_DETAIL.Rows[i].FindControl("CheckBox");
            if (cb.Checked)
            {
                string MATNR = "", LFIMG = "", WEMNG = "", ZPRDPLC = "", ZLGORT = "", POSNR = "";
                if (Request["AUFNR"] != null) { sAUFNR = Request["AUFNR"].ToString(); }
                POSNR = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[4].Text.ToString();
                MATNR = ((HyperLink)gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[5].FindControl("HyperLink1")).Text.ToString();
                // LIFNR = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[13].ToString();
                LFIMG = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[14].Text.ToString();
                WEMNG = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[15].Text.ToString();
                ZLGORT = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[13].Text.ToString();
                ZPRDPLC = gdv_WH_DNPREPARE_DETAIL.Rows[i].Cells[19].Text.ToString();
                if (ZLGORT == "0005" | ZLGORT == "0000")
                {
                    ZLGORT = "0015";
                }
                double PUB_QTY = double.Parse(LFIMG) - double.Parse(WEMNG);
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO [TWM8].[dbo].[SKIN_WIP] ");
                sb.Append("([AUFNR],[MODEL],[MATNR],[LGORT],[BDMNGS],[ENMNGS],[POSNR],[PUB_QTY],[MOVEMENT_TYPE]");
                sb.Append(",[HAIR_MATERIAL],[LACK_MATERIAL],[LACK_QTY],[POSTING],[SAPDEDUCT_NO]");
                sb.Append(",[DEDUCT_15],[DEDUCT_08],[DEDUCT_12],[DEDUCT_16],[FLAG_CLOSE]");
                sb.Append(",[EMP_NO],[MACHINE_NO],[PRINT_FLAG],[SKIN_EMPTY_FLAG],[SuperDeduction_QTY],[CR_DATE]");
                sb.Append(",[ED_DATE],[Calculation_Time],[Calculation_Efficiency_FLAG],[PrepareSchedule_FLAG]");
                sb.Append(",[Retry_Sap_Deduction_Counts],[FIFO_FLAG],[NOTE]) VALUES(");
                sb.AppendFormat("'{0}'", sAUFNR);//[AUFNR]
                sb.AppendFormat(",''");//[MODEL]
                sb.AppendFormat(",'{0}'", MATNR);//[MATNR]
                sb.AppendFormat(",'{0}'", ZLGORT);//[LGORT]
                sb.AppendFormat(",{0}", double.Parse(LFIMG));//[BDMNGS]
                sb.AppendFormat(",{0}", double.Parse(WEMNG));//[ENMNGS]
                sb.AppendFormat(",'{0}'", POSNR);//[POSNR]
                sb.AppendFormat(",'{0}'", PUB_QTY);//[PUB_QTY]
                sb.AppendFormat(",'313'");//[MOVEMENT_TYPE]
                sb.AppendFormat(",'1'");//[HAIR_MATERIAL]
                sb.AppendFormat(",'0'");//[LACK_MATERIAL]
                sb.AppendFormat(",'0'");//[LACK_QTY]
                sb.AppendFormat(",'1'");//[POSTING]
                sb.AppendFormat(",'{0}'", "SAP-000000000000");//[SAPDEDUCT_NO]
                sb.AppendFormat(",'0'");//[DEDUCT_15]
                sb.AppendFormat(",'0'");//[DEDUCT_08]
                sb.AppendFormat(",'0'");//[DEDUCT_12]
                sb.AppendFormat(",'0'");//[DEDUCT_16]
                sb.AppendFormat(",'0'");//[FLAG_CLOSE]
                sb.AppendFormat(",'{0}'", EMP_NO);//[EMP_NO]
                sb.AppendFormat(",'{0}'", "M9-WH00");//[MACHINE_NO]
                sb.AppendFormat(",'{0}'", "0");//[PRINT_FLAG]
                sb.AppendFormat(",'{0}'", "0");//[SKIN_EMPTY_FLAG]
                sb.AppendFormat(",'{0}'", "0");//[SuperDeduction_QTY]
                sb.AppendFormat(",GETDATE()");//[CR_DATE]
                sb.AppendFormat(",GETDATE()");//[ED_DATE]
                sb.AppendFormat(",'0'");//[Calculation_Time]
                sb.AppendFormat(",'0'");//[Calculation_Efficiency_FLAG]
                sb.AppendFormat(",'0'");//[PrepareSchedule_FLAG]
                sb.AppendFormat(",'0'");//[Retry_Sap_Deduction_Counts]
                sb.AppendFormat(",'0'");//[FIFO_FLAG]
                                        //sb.AppendFormat(",'0'");//[CROSSHAIR_FLAG]
                sb.AppendFormat(",N'{0}'", "人工過帳登載扣帳紀錄");//[NOTE]
                                                         //  sb.AppendFormat(",'{0}'", ZPRDPLC);//[ORIGIN]
                sb.AppendFormat(")");
                if (PUB_QTY < 0)
                {
                    nresult += POSNR + " " + MATNR + " " + "未備數量小於0,無法修改" + "</br>";
                }
                else
                {
                    if (!ATMCdb.Exsql(sb.ToString()))
                    {
                        nresult += POSNR + " " + MATNR + " " + "修改失敗" + "</br>";
                    }

                }
            }

        }
        string title = "修改結果";
        if (nresult == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + title + "', '" + "修改完成" + "');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + title + "', '" + nresult + "');", true);
        }
        capData();
        txt_EMP_NO.Text = "";
        Response.Write("<script language='javascript'>window.opener.location.reload();</script>");
    }

    protected void EXPORT_EXCEL_Click(object sender, EventArgs e)
    {
        if (Request["AUFNR"] != null) { sAUFNR = Request["AUFNR"].ToString(); }
        if (Request["MAT_CAPTION_ORDER"] != null) { MAT_CAPTION_ORDER = Request["MAT_CAPTION_ORDER"].ToString(); }
        XSSFWorkbook workbook = new XSSFWorkbook();
        MemoryStream ms = new MemoryStream();
        XSSFSheet u_sheet = (XSSFSheet)workbook.CreateSheet("My Sheet");
        XSSFCellStyle CellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
        CellStyle.Alignment = HorizontalAlignment.Left;
        CellStyle.VerticalAlignment = VerticalAlignment.Center;
        CellStyle.WrapText = true;
        CellStyle.Indention = 0;

        string[] excelHeadertitle = { "廠別","流水號", "DN單號", "項次", "料號", "描述",
                                      "庫別", "應備數", "已備數", "異常數", "未備數",
                                      "已上架", "待拉料", "今日上架數", "ZPRDPLC",
                                      "ZTRADEMARK", "負責人", "備註", "洋貨"};

        for (int j = 0; j < excelHeadertitle.Length; j++)
        {
            if (j == 0)
            {
                u_sheet.CreateRow(0).CreateCell(j).SetCellValue(excelHeadertitle[j]);
            }
            else
            {
                u_sheet.GetRow(0).CreateCell(j).SetCellValue(excelHeadertitle[j]);
            }
        }

        string strwhere = ViewState["GridViewData"].ToString();
        DataTable dtWORK_RECORDList = ATMCdb.reDt(strwhere);

        for (int i = 0; i < dtWORK_RECORDList.Rows.Count; i++)
        {
            IRow u_Row = u_sheet.CreateRow(i + 1);
            for (int j = 0; j < dtWORK_RECORDList.Columns.Count; j++)
            {
                string ValueType = dtWORK_RECORDList.Rows[i][j].GetType().ToString();
                string Value = dtWORK_RECORDList.Rows[i][j].ToString();

                switch (ValueType)
                {
                    case "System.String":
                        //字符串类型
                        //u_Row.CreateCell(j - 1).SetCellValue(Value)
                        //Exit Select
                        u_Row.CreateCell(j).SetCellValue(Value.Replace("&nbsp;", ""));
                        break; // TODO: might not be correct. Was : Exit Select

                    case "System.DateTime":
                        //日期类型
                        System.DateTime datetimeV = default(System.DateTime);
                        System.DateTime.TryParse(Value, out datetimeV);
                        if (datetimeV.ToString("HH:mm").Equals("00:00"))
                        {
                            u_Row.CreateCell(j).SetCellValue(datetimeV.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            u_Row.CreateCell(j).SetCellValue(datetimeV.ToString("yyyy-MM-dd HH:mm"));
                        }
                        break;

                    case "System.Boolean":
                        //布尔型
                        bool boolV = false;
                        bool.TryParse(Value, out boolV);
                        u_Row.CreateCell(j).SetCellValue(boolV);
                        break; // TODO: might not be correct. Was : Exit Select

                    //整型
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(Value, out intV);
                        u_Row.CreateCell(j).SetCellValue(intV);
                        break; // TODO: might not be correct. Was : Exit Select

                    //浮点型
                    case "System.Decimal":
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(Value, out doubV);
                        u_Row.CreateCell(j).SetCellValue(doubV);
                        break; // TODO: might not be correct. Was : Exit Select

                    case "System.DBNull":
                        //空值处理
                        u_Row.CreateCell(j).SetCellValue("");
                        break; // TODO: might not be correct. Was : Exit Select
                    default:
                        u_Row.CreateCell(j).SetCellValue("");
                        break; // TODO: might not be correct. Was : Exit Select
                }
                
            }

        }
        string Now = System.DateTime.Now.ToString("yyyyMMddHHmm");
        workbook.Write(ms);
        //== Excel檔名，請寫在最後面 filename的地方
        Response.AddHeader("Content-Disposition", "attachment;filename=\"" + HttpUtility.UrlEncode(sAUFNR + "DN單明細_"+ MAT_CAPTION_ORDER, System.Text.Encoding.UTF8) + Now + ".xlsx\"");
        Response.BinaryWrite(ms.ToArray());

        //== 釋放資源
        workbook = null;   //== C#為 null
        ms.Close();
        ms.Dispose();
        //缺乏這兩段程式，當您開啟EXCEL時會出現警告訊息，說檔案內容毀損需要修復
        Response.Flush();
        Response.End();
    }



}