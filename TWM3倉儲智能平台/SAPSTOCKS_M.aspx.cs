using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.XSSF.UserModel;
//-- XSSF 用來產生Excel 2007檔案（.xlsx）
using NPOI.SS.UserModel;
//-- v.1.2.4起 新增的。
using NPOI.SS.Util;
using System.Web.Configuration;
using SAP.Middleware.Connector;
using System.Text;

public partial class SAPSTOCKS_M : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
    ModuleFunction ModuleFunction = new ModuleFunction();

    double ReqQry = 0;
    double FeedQry = 0;
    string strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158", strWERKS = "TWM3";
    string sAUTHORITY = "";
    string sPREPARATION_FLAG = "";
    int modifyflag = 0;
    int MD04DAYS = 60;
    DataTable dtSAPSTOCKS = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["URL"] = System.IO.Path.GetFileName(Request.PhysicalPath) + Request.Url.Query;
        if (Session["PREPARATION_FLAG"] != null)
        {
            sPREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
            sAUTHORITY = Session["AUTHORITY"].ToString();
        }
        //     txt_MATNR.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) { __doPostBack('MainContent$btn_search', ''); return false; }} else {return true}; ");
    }
    #region 呼叫SAP RFC 已移植至SAPRFC.cs
    #region SAP連線 add by Apple at 20190215
    //private RfcDestination getSAPDB()
    //{
    //    IDestinationConfiguration ID = new SAP_FRC();
    //    try
    //    { RfcDestinationManager.UnregisterDestinationConfiguration(ID); RfcDestinationManager.RegisterDestinationConfiguration(ID); }
    //    catch
    //    { }
    //    RfcDestination dest = RfcDestinationManager.GetDestination("SAPMES");
    //    RfcRepository repository = dest.Repository;
    //    return dest;
    //}
    //public class SAP_FRC : IDestinationConfiguration
    //{
    //    public RfcConfigParameters GetParameters(string destinationName)
    //    {
    //        if (destinationName.Equals("SAPMES"))
    //        {
    //            RfcConfigParameters rfcParams = new RfcConfigParameters();
    //            rfcParams.Add(RfcConfigParameters.Name, destinationName); //APPLE
    //            rfcParams.Add(RfcConfigParameters.AppServerHost, "172.20.1.176");   //SAP主機IP
    //            rfcParams.Add(RfcConfigParameters.SystemNumber, "05");              //SAP實例
    //            rfcParams.Add(RfcConfigParameters.Client, "168");                   // Client
    //            rfcParams.Add(RfcConfigParameters.User, "MES.ACL");                     //用戶名
    //            rfcParams.Add(RfcConfigParameters.Password, "MESMES");              //密碼
    //            rfcParams.Add(RfcConfigParameters.Language, "zf");                  //登陆語言
    //            //rfcParams.Add(RfcConfigParameters.PoolSize, "5");
    //            //rfcParams.Add(RfcConfigParameters.MaxPoolSize, "10");
    //            rfcParams.Add(RfcConfigParameters.ConnectionIdleTimeout, "1800");
    //            return rfcParams;

    //        }
    //        else
    //        {
    //            return null;
    //        }

    //    }

    //    public bool ChangeEventsSupported()
    //    {

    //        return false;

    //    }
    //    public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged
    //    {
    //        add
    //        {
    //            //configurationChanged = value;
    //        }
    //        remove
    //        {
    //            //do nothing
    //        }
    //    }
    //}
    #endregion
    #region SAP TABLE拆解 add by Apple at 20190215
    //public DataTable ConvertToTable(IRfcTable rfcTable, string tableName)
    //{
    //    DataTable dt = new DataTable(tableName);
    //    //CreateTable
    //    for (int i = 0; i < rfcTable.ElementCount; i++)
    //    {
    //        RfcElementMetadata rfcElementMetadata = rfcTable.GetElementMetadata(i);
    //        dt.Columns.Add(rfcElementMetadata.Name);
    //    }

    //    foreach (IRfcStructure rfcStructure in rfcTable)
    //    {
    //        DataRow dr = dt.NewRow();
    //        for (int z = 0; z < rfcTable.ElementCount; z++)
    //        {
    //            RfcElementMetadata metadata = rfcTable.GetElementMetadata(z);
    //            switch (metadata.DataType)
    //            {
    //                case RfcDataType.BCD:
    //                    try
    //                    {
    //                        dr[z] = rfcStructure.GetInt(metadata.Name);
    //                    }
    //                    catch
    //                    {
    //                        //CANNOT CONVERT BCD[8:2] INTO INT32 解决:转DOUBLE add by dick 20170421
    //                        dr[z] = rfcStructure.GetDouble(metadata.Name);
    //                    }
    //                    break;

    //                default:
    //                    dr[z] = rfcStructure.GetString(metadata.Name);
    //                    break;
    //            }
    //        }
    //        dt.Rows.Add(dr);
    //        dt.AcceptChanges();
    //    }

    //    return dt;
    //}
    #endregion
    #region RFC接口 料號庫存查詢 add by Apple at 20190215
    //private DataTable ZGBSN(string MATNR, string strWERKS, RfcDestination dest)
    //{
    //    string ED_DATE = "", CONFIRM_DATE = "";
    //    double PRETOTAL_STOCK = 0;
    //    double[] ENTRY_QNT = new double[2] { 0, 0 };
    //    double ENTRY_QNT_TODAY = 0;
    //    DataTable dtZMARD_OUTPUT = new DataTable();
    //    try
    //    {
    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
    //        IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
    //        string[] sArrayLGORT = strLGORT.Split(',');
    //        foreach (string SubstrLGORT in sArrayLGORT)
    //        {
    //            string strMaterial = MATNR.ToString().TrimStart('0');
    //            IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
    //            MARD.SetValue("MATNR", strMaterial);
    //            MARD.SetValue("WERKS", strWERKS);
    //            MARD.SetValue("LGORT", SubstrLGORT);
    //            ZMARD_INPUT.Insert(MARD);
    //        }
    //        rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
    //        rfc.Invoke(dest);
    //        IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
    //        dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");

    //        if (dtZMARD_OUTPUT.Rows.Count > 0)
    //        {
    //            if (!txtMD04DAYS.Text.ToString().Equals(""))
    //            {
    //                MD04DAYS = int.Parse(txtMD04DAYS.Text.ToString());
    //            }
    //            ReturnMD04(MD04DAYS, MATNR, strWERKS, dest);
    //            ENTRY_QNT = ZGWH_STORAGE(MATNR, strWERKS);
    //            ENTRY_QNT_TODAY = ZGWH_STORAGE_TODAY(MATNR, strWERKS);
    //            ED_DATE = WH_INVENTORY(MATNR, strWERKS);
    //            PRETOTAL_STOCK = ZGSKIN_BIN_CARD(MATNR, strWERKS);
    //            //Response.Write("<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //        }
    //        else
    //        {
    //            Response.Write("<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //        }
    //    }
    //    catch (RfcAbapException ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //    DataColumn column1 = new DataColumn("ENTRY_QNT");
    //    column1.DataType = System.Type.GetType("System.Decimal");
    //    column1.DefaultValue = ENTRY_QNT[0];
    //    dtZMARD_OUTPUT.Columns.Add(column1);

    //    DataColumn column4 = new DataColumn("UNPULL_QNT");
    //    column4.DataType = System.Type.GetType("System.Decimal");
    //    column4.DefaultValue = ENTRY_QNT[1];
    //    dtZMARD_OUTPUT.Columns.Add(column4);

    //    DataColumn column5 = new DataColumn("ENTRY_QNT_TODAY");
    //    column5.DataType = System.Type.GetType("System.Decimal");
    //    column5.DefaultValue = ENTRY_QNT_TODAY;
    //    dtZMARD_OUTPUT.Columns.Add(column5);

    //    DataColumn column2 = new DataColumn("ReqQry");
    //    column2.DataType = System.Type.GetType("System.Decimal");
    //    column2.DefaultValue = ReqQry;
    //    dtZMARD_OUTPUT.Columns.Add(column2);

    //    DataColumn column3 = new DataColumn("FeedQry");
    //    column3.DataType = System.Type.GetType("System.Decimal");
    //    column3.DefaultValue = FeedQry;
    //    dtZMARD_OUTPUT.Columns.Add(column3);

    //    DataColumn column6 = new DataColumn("ED_DATE");
    //    column6.DataType = System.Type.GetType("System.String");
    //    column6.DefaultValue = ED_DATE;
    //    dtZMARD_OUTPUT.Columns.Add(column6);
    //    DataColumn column9 = new DataColumn("PRETOTAL_STOCK");
    //    column9.DataType = System.Type.GetType("System.Decimal");
    //    column9.DefaultValue = PRETOTAL_STOCK;
    //    dtZMARD_OUTPUT.Columns.Add(column9);


    //    return dtZMARD_OUTPUT;
    //}
    #endregion
    /// <summary>
    /// 盤點功能
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    //public bool Finish_Inventory(string EMPLR_ID, string MATNR, string WERKS, string LGORT, string LGPBE, string LABST, string LABSTLAST, string LABSTLASTNOTE)
    //{
    //    string mas_no = "";
    //    bool nresult = false;
    //    // string ed = DateTime.Now.ToString("yyyy-MM-dd");
    //    StringBuilder sb = new StringBuilder();
    //    sb.Append(" select TOP (1) PK_NO from WH_INVENTORY_MAS ");
    //    sb.AppendFormat("  WHERE  (START_DATE < GETDATE() AND [END_DATE]>GETDATE()+1) order by PK_NO desc");
    //    DataSet ds = ATMCdb.reDs(sb.ToString());
    //    foreach (DataRow dr in ds.Tables[0].Rows)
    //    {
    //        mas_no = dr["PK_NO"].ToString();
    //    }


    //    InventoryFunction.WH_INVENTORY item = new InventoryFunction.WH_INVENTORY();
    //    item.MATNR = MATNR;
    //    item.LGORT = LGORT;
    //    item.BIN = LGPBE;
    //    item.STOCK_QTY = LABST;
    //    item.STOCK_QTY_LATEST = LABSTLAST;
    //    item.PLANT = WERKS;
    //    item.EMP_NO = EMPLR_ID;
    //    item.MAS_NO = mas_no;
    //    item.NOTE = LABSTLASTNOTE;
    //    string Result = SAPRFC.Z_IWMS2SAP_TRANSFER_DATA(WERKS, item, SAPRFC.getSAPDB());
    //    if (Result == "OK")
    //    {
    //        nresult = InventoryFunction.Finish_Inventory(item);

    //    }
    //    else
    //    {
    //        Response.Write(" <script   language=javascript> window.alert( 'SAP上傳失敗：" + Result + "') </script> ");
    //        nresult = InventoryFunction.Finish_Inventory(item);
    //    }
    //    return nresult;
    //}

    //public string ZMM_MODIFY_STORAGE_BIN(string MATNR, string WERKS, string LGORT, string LGPBE, RfcDestination dest)
    //{
    //    string nresult = "";
    //    RfcRepository Repo = dest.Repository;
    //    //  RfcRepository repository = dest.Repository;
    //    //  IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
    //    IRfcFunction rfc = Repo.CreateFunction("ZMM_MODIFY_STORAGE_BIN");
    //    rfc.SetValue("MATNR", MATNR);
    //    rfc.SetValue("WERKS", WERKS);
    //    rfc.SetValue("LGORT", LGORT);
    //    rfc.SetValue("LGPBE", LGPBE);
    //    rfc.Invoke(dest);
    //    DataTable dt = new DataTable();
    //    IRfcTable ZMARD_OUTPUT = rfc.GetTable("OUT_PUT");
    //    dt = ConvertToTable(ZMARD_OUTPUT, "OUT_PUT");

    //    if (dt.Rows[0]["FLAG"].ToString() == "Y")
    //    {
    //        nresult = "OK";
    //        Response.Write("<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //    }
    //    else
    //    {
    //        nresult = "NG";
    //    }

    //    return nresult;
    //}
    #endregion
    /// <summary>
    /// CAP 待上架數量
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private double[] ZGWH_STORAGE(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        //double ENTRY_QNT = 0;
        double[] ENTRY_QNT = new double[2] { 0, 0 };
            string sParam = "SELECT ISNULL(SUM(CASE WHEN [CACHE_NAME] is not null AND [CACHE_NAME] <> '調整帳務' THEN [ENTRY_QNT] ELSE 0 END ),0),ISNULL(SUM(CASE WHEN [CACHE_NAME] is null THEN [ENTRY_QNT] ELSE 0 END ),0) FROM [TWM8].[dbo].[WH_STORAGE] WHERE  [STATUS]=0 AND [DISPLAY_FALG]='Y' AND [MATERIAL]='" + strMaterial + "' AND [PLANT]='" + strWERKS + "' ";
            DataTable dt = ATMCdb.reDt(sParam);
            if (dt.Rows.Count > 0)
            {

                ENTRY_QNT[0] = double.Parse(dt.Rows[0][0].ToString());
                ENTRY_QNT[1] = double.Parse(dt.Rows[0][1].ToString());

            }
        return ENTRY_QNT;
    }
    /// <summary>
    /// CAP 今日上架數量
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private double ZGWH_STORAGE_TODAY(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        //double ENTRY_QNT = 0;
        double ENTRY_QNT_TODAY = 0;
            string sParam = "SELECT ISNULL(SUM([ENTRY_QNT]),0) FROM [TWM8].[dbo].[WH_STORAGE] WHERE  [STATUS]=1 AND [DISPLAY_FALG]='Y' AND CONVERT(DATE,[ED_DATE])=CONVERT(DATE,GETDATE()) AND [MATERIAL]='" + strMaterial + "' AND [PLANT]='" + strWERKS + "' ";
            DataTable dt = ATMCdb.reDt(sParam);
            if (dt.Rows.Count > 0)
            {

                ENTRY_QNT_TODAY = double.Parse(dt.Rows[0][0].ToString());
            }
        return ENTRY_QNT_TODAY;
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        if (txt_MATNR.Text.ToString().Trim().Equals("") )
        {
            Response.Write(" <script   language=javascript> window.alert( '請輸入料號廠別再進行搜尋') </script> ");
        }
        else
        {
            capdata();
        }

    }
    /// <summary>
    /// 料號庫存查詢
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private DataTable capZGBSN(string MATNR, string strWERKS)
    {
        string ED_DATE = "";
        double PRETOTAL_STOCK = 0;
        double[] ENTRY_QNT = new double[2] { 0, 0 };
        double ENTRY_QNT_TODAY = 0;
        DataTable dtZMARD_OUTPUT = new DataTable();
        DataColumn mycolumn = new DataColumn("MATNR");
        mycolumn.DataType = System.Type.GetType("System.String");
        dtZMARD_OUTPUT.Columns.Add(mycolumn);
        dtZMARD_OUTPUT.Rows.Add(dtZMARD_OUTPUT.NewRow());
        dtZMARD_OUTPUT.Rows[0][0] = MATNR;
        try
        {
            dtZMARD_OUTPUT = SAPRFC.ZGBSN(dtZMARD_OUTPUT, strWERKS, strLGORT, "MATNR",0, SAPRFC.getSAPDB());
            if (dtZMARD_OUTPUT.Rows.Count > 0)
            {
                if (!txtMD04DAYS.Text.ToString().Equals(""))
                {
                    MD04DAYS = int.Parse(txtMD04DAYS.Text.ToString());
                }
                ReturnMD04(MD04DAYS, MATNR, strWERKS, SAPRFC.getSAPDB());
                ENTRY_QNT = ZGWH_STORAGE(MATNR, strWERKS);
                ENTRY_QNT_TODAY = ZGWH_STORAGE_TODAY(MATNR, strWERKS);
                PRETOTAL_STOCK = ZGSKIN_BIN_CARD(MATNR, strWERKS);

                DataColumn column1 = new DataColumn("ENTRY_QNT");
                column1.DataType = System.Type.GetType("System.Decimal");
                column1.DefaultValue = ENTRY_QNT[0];
                dtZMARD_OUTPUT.Columns.Add(column1);

                DataColumn column4 = new DataColumn("UNPULL_QNT");
                column4.DataType = System.Type.GetType("System.Decimal");
                column4.DefaultValue = ENTRY_QNT[1];
                dtZMARD_OUTPUT.Columns.Add(column4);

                DataColumn column5 = new DataColumn("ENTRY_QNT_TODAY");
                column5.DataType = System.Type.GetType("System.Decimal");
                column5.DefaultValue = ENTRY_QNT_TODAY;
                dtZMARD_OUTPUT.Columns.Add(column5);

                DataColumn column2 = new DataColumn("ReqQry");
                column2.DataType = System.Type.GetType("System.Decimal");
                column2.DefaultValue = ReqQry;
                dtZMARD_OUTPUT.Columns.Add(column2);

                DataColumn column3 = new DataColumn("FeedQry");
                column3.DataType = System.Type.GetType("System.Decimal");
                column3.DefaultValue = FeedQry;
                dtZMARD_OUTPUT.Columns.Add(column3);

                DataColumn column9 = new DataColumn("PRETOTAL_STOCK");
                column9.DataType = System.Type.GetType("System.Decimal");
                column9.DefaultValue = PRETOTAL_STOCK;
                dtZMARD_OUTPUT.Columns.Add(column9);
            }
        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        return dtZMARD_OUTPUT;

    }
    protected void capdata()
    {
        int LGORTTYPE = int.Parse(rdoLGORTTYPE.SelectedValue);
        switch (LGORTTYPE)
        {
            case 0://ALL庫別
                strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158,0100,0110,0120,0121,0128,0129,0130";
                break;
            case 1://良品庫
                strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158";
                break;
            case 2://非良品庫
                strLGORT = "0100,0110,0120,0121,0128,0129,0130";
                break;
        }
        ListView1.SelectedIndex = -1;
        ListView1.Items.Clear();
        long n = 0;
        string MATNR = txt_MATNR.Text.ToString().Trim().ToUpper();
        if (long.TryParse(MATNR, out n))
        {
            MATNR = MATNR.PadLeft(18, '0');
        }

        strWERKS = rdoWERKS.SelectedValue;

        DataTable dt = capZGBSN(MATNR, strWERKS);
        dt = ATMCdb.DataTableFilterSort2(dt, "LABST>0 OR LGPBE<>''", "LGORT");
        int icount = dt.Rows.Count;
        if (dt.Rows.Count>0)
        {
            string str = "";
            for (int i = 0; i < icount; i++)
            {
                str += "【" + dt.Rows[i]["LGORT"].ToString() + " " + dt.Rows[i]["LGPBE"].ToString() + "庫存:" + dt.Rows[i]["LABST"].ToString() + "】<br>";
            }
            lblimplementrate.Text = str.Substring(0).ToString();
            ViewState["dtSAPSTOCKS"] = dt;
            //DataTable dt = ((DataTable)ViewState["dtSAPSTOCKS"]);

            ListView1.DataSource = dt;
            ListView1.DataBind();
        }
        
        txt_MATNR.Attributes.Add("onfocus", "this.select();");
        txt_MATNR.Focus();
       
        //lblimplementrate.Text = "總庫別有：<br>";
        
       
        //if (gdv_SAPSTOCKS.Rows.Count > 0)
        //{
        //    txt_MATNR.Text = "";
        //}
        if (icount > 0)
        {
            Response.Write("<embed src = 'seachfinish.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
        }
        else
        {
            Response.Write("<script   language=javascript> window.alert( '庫存為0或未設定儲位') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
        }
    }
    protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        string MATNR = ""; string WERKS = ""; string LGORT = ""; string LGPBE = "", LABSTLAST = "", EMPLR_ID = "", LABST = "", LABSTLASTNOTE = "";
        ListView1.SelectedIndex = ((ListViewDataItem)e.Item).DisplayIndex;
        EMPLR_ID = txt_EMPNO.Text.ToString().Trim();
        MATNR = ((Label)ListView1.Items[ListView1.SelectedIndex].FindControl("lbl_MATNR")).Text.ToString();
        WERKS = ((Label)ListView1.Items[ListView1.SelectedIndex].FindControl("lbl_WERKS")).Text.ToString();
        LGORT = ((Label)ListView1.Items[ListView1.SelectedIndex].FindControl("lbl_LGORT")).Text.ToString().Trim().ToUpper();
        LABST = ((Label)ListView1.Items[ListView1.SelectedIndex].FindControl("lbl_LABST")).Text.ToString().Trim().ToUpper();
        TextBox txt = (TextBox)ListView1.Items[ListView1.SelectedIndex].FindControl("txtLGPBE");
        LGPBE = txt.Text.ToString().Trim().ToUpper().Replace(":", ";");
        dtSAPSTOCKS = (DataTable)ViewState["dtSAPSTOCKS"];
        string OldLGPBE = dtSAPSTOCKS.Rows[ListView1.SelectedIndex]["LGPBE"].ToString().Trim().ToUpper();
        if (e.CommandName == "edit_data")  //對應上面的CommandName
        {
            if (EMPLR_ID.Equals(""))
            {
                Response.Write(" <script   language=javascript> window.alert( '修改儲位前請先輸入工號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                txt_EMPNO.Attributes.Add("onfocus", "this.select();");
                txt_EMPNO.Focus();
                return;
            }
            else
            {
                if (ATMCdb.CHECKWHEMPLR_ID(EMPLR_ID) != "Y")
                {
                    Response.Write("<script   language=javascript> window.alert( '你沒有權限修改儲位') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                }
                else
                {
                    MODIFY_STORAGE_BIN(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, EMPLR_ID, LABST);

                }
            }



            //capdata();
            //txt_MATNR.Attributes.Add("onfocus", "this.select();");
            //txt_MATNR.Focus();
            ListView1.SelectedIndex = -1;
            //-1代表離開「選取」按鈕的狀態！
            btn_search_Click(sender, e);

        }
        if (e.CommandName == "LOG_data")
        {
            string htm = "MODIFY_STORAGE_BINLOG.aspx?MATNR=" + MATNR;
            string javascript = "window.open('" + htm + "','','width=800px,height=500px,scrollbars=yes,resizable=yes, status=yes');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "LOG", javascript, true);
            ListView1.SelectedIndex = -1;
            //-1代表離開「選取」按鈕的狀態！
            btn_search_Click(sender, e);
        }

    }


    /// <summary>
    /// 修改儲位
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="LGORT"></param>
    /// <param name="OldLGPBE"></param>
    /// <param name="LGPBE"></param>
    /// <param name="EMPLR_ID"></param>
    /// <param name="LABST"></param>
    void MODIFY_STORAGE_BIN(string MATNR, string WERKS, string LGORT, string OldLGPBE, string LGPBE, string EMPLR_ID,string LABST)
    {
        if (SAPRFC.ZMM_MODIFY_STORAGE_BIN(MATNR, WERKS, LGORT, LGPBE, SAPRFC.getSAPDB()) == "OK")
        {
            ModuleFunction.BKMODIFY_STORAGE_BINLOG(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, 1, EMPLR_ID, LABST);
            Response.Write(" <script   language=javascript> window.alert( '料號：【" + MATNR + "】 儲位已修改為【" + LGPBE + "】') </script> ");
        }
        else
        {
            ModuleFunction.BKMODIFY_STORAGE_BINLOG(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, 0, EMPLR_ID, LABST);
            var str = "< span style = 'color: red;' > 修改【失敗】，請重新確認 </ span >";
            Response.Write(" <script   language=javascript> window.alert( '" + str + "') </script> ");
        }
        //this.btn_search_Click(null, null);
        capdata();
    }
    protected void txt_MATNR_TextChanged(object sender, EventArgs e)
    {
        if (!txt_MATNR.Text.ToString().Equals(""))
        {
            //btn_search_Click(sender, e);
            capdata();
        }
    }
    /// <summary>
    /// 回傳MD04資訊
    /// </summary>
    /// <param name="AddDays"></param>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="dest"></param>
    public void ReturnMD04(int AddDays, string MATNR, string WERKS, RfcDestination dest)
    {

        //MATNR = MATNR.ToString().TrimStart('0');
        //string ed = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        string ed = DateTime.Now.AddDays(AddDays).ToString("yyyy-MM-dd");
        RfcRepository repository = dest.Repository;
        IRfcFunction rfc = repository.CreateFunction("BAPI_MATERIAL_STOCK_REQ_LIST");
        // string WONO = ""; string SDATE = DateTime.Now.AddDays(-7).ToString("yyyyMMdd"); string EDATE = DateTime.Now.AddDays(7).ToString("yyyyMMdd"); string PLANT = "TWM9"; 
        string SPFLG = "";
        rfc.SetValue("MATERIAL", MATNR);
        rfc.SetValue("PLANT", WERKS);
        rfc.SetValue("GET_ITEM_DETAILS", "X");
        rfc.Invoke(dest);
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        IRfcTable ZMARD_OUTPUT = rfc.GetTable("MRP_ITEMS");
        dt = SAPRFC.ConvertToTable(ZMARD_OUTPUT, "MRP_ITEMS");
        if (dt.Rows.Count > 0)
        {


            //dt.DefaultView.RowFilter = "AVAIL_DATE <='"+ ed+"'";
            DataRow[] filteredRows = dt.Select("AVAIL_DATE <='" + ed + "'");
            dt1 = filteredRows.CopyToDataTable();
            //dt.Rows.RemoveAt(0);

            int COUNT = dt.Rows.Count;
            //  dt.Columns[2].DataType=
            // DateTime sd = Convert.ToDateTime("2018/08/07");
            // var linqStament = from p in dt.AsEnumerable() where (DateTime)p.Field<DateTime>("AVAIL_DATE") <= DateTime.Today.AddDays(30)
            var linqStament = from p in dt1.AsEnumerable()

                              group p by new
                              {
                                  PLUS_MINUS = p.Field<string>("PLUS_MINUS")

                              } into g
                              //let row = g.First()
                              select new
                              {
                                  // MATNR = g.Key.MATNR
                                  //,
                                  //Total = g.Sum(x => Convert.ToInt32(x["LABST"]) + Convert.ToInt32(x["INSME"]))
                                  PLUS_MINUS = g.Key.PLUS_MINUS,
                                  REC_REQD_QTY = g.Sum(p => Convert.ToDouble(p["REC_REQD_QTY"]))
                                  ,
                                  SHORTAGE_QTY = g.Sum(p => Convert.ToDouble(p["SHORTAGE_QTY"]))
                              };

            foreach (var query in linqStament)
            {
                if (query.PLUS_MINUS.Equals("-"))
                {
                    ReqQry = query.REC_REQD_QTY;
                    // MD04List.Add(new myList { PartName = "ReqQry", REC_REQD_QTY = query.REC_REQD_QTY, SHORTAGE_QTY = query.SHORTAGE_QTY });
                }
                if (query.PLUS_MINUS.Equals("+"))
                {
                    FeedQry = query.REC_REQD_QTY;
                    //  MD04List.Add(new myList { PartName = "FeedQry", REC_REQD_QTY = query.REC_REQD_QTY, SHORTAGE_QTY = query.SHORTAGE_QTY });
                }
            }



        }
    }
    protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            if (!sPREPARATION_FLAG.Equals("Y"))
            {
                //Button BTN = (Button)e.Item.FindControl("btn_EditD");
                //BTN.Enabled = false;
                //Button BTN2 = (Button)e.Item.FindControl("btn_INVENTORY");
                //BTN2.Enabled = false;
            }

        }
    }
    #region 備品庫資訊
    private double ZGSKIN_BIN_CARD(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        //double ENTRY_QNT = 0;
        double TOTAL_STOCK = 0;
        if (strWERKS == "TWM9")
        {
            string sParam = "SELECT ISNULL(SUM([IN_STOCK])-SUM([OUT_STOCK]),0) FROM [TWM9].[dbo].[SKIN_BIN_CARD] WHERE  [LGORT]='9999' AND [MATNR]='" + strMaterial + "' ";
            TOTAL_STOCK = double.Parse(ATMCdb.scalDs(sParam));
        }
        return TOTAL_STOCK;
    }
    #endregion
}