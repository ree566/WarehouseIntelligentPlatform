
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using SAP.Middleware.Connector;
using System.Text;

public partial class WH_EFLOW_MAS_DETAIL : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
    string NO = "";
    #region Add 庫存&待驗數量 by Apple at 20190215
    string strLGORT = "0015,0008,0012,0055,0058", strWERKS = "TWM3";
    string PREPARATION_FLAG = "";
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["URL"] = System.IO.Path.GetFileName(Request.PhysicalPath) + Request.Url.Query;
        if (Session["PREPARATION_FLAG"] != null)
        {
            PREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
        }
        if (!Page.IsPostBack)
        {
            if (Request["NO"].ToString() != "") { ViewState["NO"] = Request["NO"].ToString(); }
            if (Request["OWNER"].ToString() != "") { ViewState["OWNER"] = Request["OWNER"].ToString(); }
            capData();
        }
    }
    private void capData()
    {
        string sparam = "SELECT A.[PLANT],A.[MOVEMENT_TYPE],A.[INSERT_TIME],A.[PK_NO],A.[NO],A.[AUFNR],A.[MATNR],A.[BDMNGS],A.[ENMNGS],[REASON],[POSTING],[APPLYINFO],[STATUS],(CASE WHEN [STATUS]=1 AND [POSTING]= 1  THEN N'已完成' WHEN [STATUS]=1 AND [POSTING]= 0 AND [ENMNGS]>0 THEN N'過帳異常' WHEN [STATUS]=1 AND [POSTING]= 0 AND [ENMNGS]=0 THEN N'已關單'  ELSE N'處理中' END ) STATUSNAME,[CONFIRM_TIME],B.[EMP_NO] FROM [TWM8].[dbo].[WH_EFLOW_MAS] A ";
        sparam += "LEFT JOIN  (SELECT DISTINCT [MATNR],[PLANT],[EMP_NO] FROM [TWM8].[dbo].[WH_INVENTORY_STOCK]) B ON A.MATNR=B.MATNR AND A.[PLANT]=B.[PLANT] ";
         sparam += " WHERE A.[NO]=" + ViewState["NO"];
        if (ViewState["OWNER"].ToString() != "ALL")
        {
            sparam += " AND B.[EMP_NO] LIKE'%" + ViewState["OWNER"].ToString() + "%' ";
        }
        sparam += " ORDER BY [INSERT_TIME],[NO],[AUFNR],[PK_NO] ";
        DataTable dt = ATMCdb.reDt(sparam);
        #region Add 庫存&待驗數量 by Apple at 20190215
        dt = ZGBSN(dt, SAPRFC.getSAPDB());
        #endregion
        gdv_WH_EFLOW_MASDATA.DataSource = dt;
        gdv_WH_EFLOW_MASDATA.DataBind();
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
            distinctValues = view.ToTable(true, "MATNR", "PLANT");

            foreach (string SubstrLGORT in sArrayLGORT)
            {
                foreach (DataRow od in distinctValues.Rows)
                {
                    string strMaterial = od["MATNR"].ToString();
                    string strPLANT = od["PLANT"].ToString();
                    IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
                    MARD.SetValue("MATNR", strMaterial);
                    MARD.SetValue("WERKS", strPLANT);
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
                              ,
                              WERKS = p.Field<string>("WERKS")
                          } into g
                          //let row = g.First()
                          select new
                          {
                              MATNR = g.Key.MATNR,
                              WERKS = g.Key.WERKS
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
                if (od["MATNR"].ToString() == query.MATNR.ToString().TrimStart('0') && od["PLANT"].ToString() == query.WERKS.ToString())
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
    protected void gdv_WH_EFLOW_MASDATA_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //string QA = e.Row.Cells[13].Text;
            string QA1 = e.Row.Cells[16].Text;
            string POSTING = ((DropDownList)e.Row.Cells[14].FindControl("ddl_POSTING")).SelectedValue;
            string STATUS = ((DropDownList)e.Row.Cells[15].FindControl("ddl_STATUS")).SelectedValue;
            if (QA1 == "處理中" || QA1 == "過帳異常") { e.Row.Cells[16].BackColor = System.Drawing.Color.Red; }
            if (QA1 == "已關單") { e.Row.Cells[16].BackColor = System.Drawing.Color.Yellow; }
            //if (QA == "0" && QA1 == "已完成") { e.Row.Cells[13].BackColor = System.Drawing.Color.Red; }
            if (POSTING == "0") { e.Row.Cells[14].BackColor = System.Drawing.Color.Red; }
            if (STATUS == "0") { e.Row.Cells[15].BackColor = System.Drawing.Color.Red; }
            if (PREPARATION_FLAG != "Y")
            {
                e.Row.Cells[0].Enabled = false;
                e.Row.Cells[1].Enabled = false;
            }
        }
    }
    protected void gdv_WH_EFLOW_MASDATA_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string PK_NO = "", REASON = "", ENMNGS = "", POSTING = "", STATUS = "";

        Button BTN = (Button)e.CommandSource;// 先抓到這個按鈕（我們設定了CommandName）
        GridViewRow myRow = (GridViewRow)BTN.NamingContainer; // 從你按下 Button按鈕的時候，NamingContainer知道你按下的按鈕在GridView「哪一列」！
        PK_NO = gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].Cells[5].Text;
        TextBox txt = (TextBox)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("txtREASON");
        REASON = txt.Text.ToString();
        TextBox txt2 = (TextBox)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("txtENMNGS");
        ENMNGS = txt.Text.ToString();
        DropDownList ddl = (DropDownList)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("ddl_POSTING");
        POSTING = ddl.SelectedValue.ToString();
        DropDownList ddl2 = (DropDownList)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("ddl_STATUS");
        STATUS = ddl2.SelectedValue.ToString();
        if (e.CommandName == "edit_data")
        {
            UPDATEREASON(PK_NO, REASON, POSTING, STATUS);
            capData();

        }
        if (e.CommandName == "Posting_data")
        {
            string sparam = "UPDATE A  SET [POSTING]=1,[STATUS]=1,[REASON]=N'" + REASON + "【人工調整過帳】' FROM [TWM8].[dbo].[WH_EFLOW_MAS] A WHERE [PK_NO]=" + PK_NO;
            string sparam2 = "UPDATE A  SET [POSTING]=1,[ED_DATE]=getdate(),[NOTE]=N'人工調整過帳' FROM [TWM8].[dbo].[SKIN_WIP] A WHERE [EFLOW_MAS_PK_NO]=" + PK_NO;
            ATMCdb.Exsql(sparam);
            ATMCdb.Exsql(sparam2);
            capData();

        }
    }
    private void UPDATEREASON(string PK_NO, string REASON, string POSTING, string STATUS)
    {
        string message = "";
        string sparam = "UPDATE A  SET [REASON]=N'" + REASON + "',[POSTING]=" + POSTING + ",[STATUS]=" + STATUS + " FROM [TWM8].[dbo].[WH_EFLOW_MAS] A WHERE [PK_NO]=" + PK_NO;
        string sparam2 = "UPDATE A  SET [POSTING]=" + POSTING + ",[ED_DATE]=getdate(),[NOTE]=N'人工修改狀態' FROM [TWM8].[dbo].[SKIN_WIP] A WHERE [EFLOW_MAS_PK_NO]=" + PK_NO;
        if (ATMCdb.Exsql(sparam + ";" + sparam2))
        {
            message = "<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>";
            message += " <script   language=javascript> window.alert( '備註修改完成') </script> ";
            Response.Write(message);
        }
        else
        {
            message = "<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>";
            message += " <script   language=javascript> window.alert( '備註修改失敗') </script> ";
            Response.Write(message);
        }

    }
}