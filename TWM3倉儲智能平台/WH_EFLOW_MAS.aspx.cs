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

public partial class WH_EFLOW_MAS : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
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

            string sScript = null;
            string sScriptl = null;
            string surl = null;
            string sur2 = null;
            //   日期輸入的頁面，將 TextBox 以 TextBoxId 網址參數傳給日期頁面
            surl = "calendar.aspx?TextBoxID=" + txtStartDate.ClientID;
            sScript = "window.open('" + surl + "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')";
            txtStartDate.Attributes["onclick"] = sScript;
            sur2 = "calendar.aspx?TextBoxID=" + txtEndDate.ClientID;
            sScriptl = "window.open('" + sur2 + "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')";
            txtEndDate.Attributes["onclick"] = sScriptl;
            txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //txtStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            Cap_OWNER();
            btn_search_Click(sender, e);

        }
    }
    private void Cap_OWNER()
    {

        cboOWNER.Items.Clear();
        // string strwhere = "SELECT DISTINCT [USER_NO],[USER_NO]+[USER_NAME_CH] OWNER FROM [MES_QryUserInfo001] WHERE [DEPT_NO]='MC10' and [LINE_DESC]='WAREHOUSE' AND [cr_datetime]>=CONVERT(DATE,GETDATE()-1) ";
        string strwhere = "SELECT  A.[EMP_NO] USER_NO,A.EMP_NO+A.[EMP_NAME] OWNER FROM [TWM8].[dbo].[EMP_STD] A WHERE [PREPARATION_FLAG]='Y'  ";
        DataTable dt = ATMCdb.reDt(strwhere);
        cboOWNER.DataSource = dt;
        cboOWNER.DataTextField = "OWNER";
        cboOWNER.DataValueField = "USER_NO";
        cboOWNER.DataBind();
        cboOWNER.Items.Insert(0, new ListItem("ALL", "ALL"));
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        capData();

    }
    protected void Timer1_Tick(object sender, EventArgs e)
    {
        btn_search_Click(sender, e);
    }
    protected void txt_EFLOW_NO_TextChanged(object sender, EventArgs e)
    {
        if (!txt_EFLOW_NO.Text.ToString().Equals(""))
        {
            btn_search_Click(sender, e);
        }
    }

    private void capData()
    {
        string sparam = "SELECT *,'" + cboOWNER.SelectedValue + "' OWNER FROM (SELECT A.[PLANT],[MOVEMENT_TYPE],[NO],COUNT(*) BDMBGSQTY,SUM(CASE WHEN [STATUS]=0 THEN 1 ELSE 0 END) OPENQTY,SUM(CASE WHEN [CONFIRM_TIME] IS  NULL THEN 1 ELSE 0 END ) NOCONFIRMQTY ";
        sparam += ",MIN([INSERT_TIME]) C_DATE FROM [TWM8].[dbo].[WH_EFLOW_MAS]  A ";
        sparam += "LEFT JOIN  (SELECT DISTINCT [MATNR],[PLANT],[EMP_NO] FROM [TWM8].[dbo].[WH_INVENTORY_STOCK]) B ON A.MATNR=B.MATNR AND A.[PLANT]=B.[PLANT] ";
        sparam += "";
        sparam += "WHERE A.[MOVEMENT_TYPE]=" + rdoMOVEMENT_TYPE.SelectedValue;

        if (!txt_MATNR.Text.ToString().Trim().Equals(""))
        {
            sparam += " AND A.[MATNR]='" + txt_MATNR.Text.ToString().Trim() + "' ";
        }

        if (!cbo_PLANT.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND [PLANT]='" + cbo_PLANT.SelectedValue + "' ";
        }

        if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND B.[EMP_NO] LIKE '%" + cboOWNER.SelectedValue + "%' ";
        }

        //if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
        //{
        //    sparam += " AND B.[EMP_NO]='" + cboOWNER.SelectedValue + "' ";
        //}

        sparam += "GROUP BY A.[PLANT],[MOVEMENT_TYPE],[NO] ) T1 ";

        if (txt_EFLOW_NO.Text.ToString().Trim().Equals(""))
        {
            sparam += "WHERE [OPENQTY] > 0";
        }
        else
        {
            sparam += "WHERE [NO] =" + txt_EFLOW_NO.Text.ToString().Trim();
        }

        sparam += " AND CONVERT(DATE,C_DATE) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' ";


        sparam += " ORDER BY C_DATE ";
        gdv_WH_EFLOW_MAS.DataSource = ATMCdb.reDt(sparam);
        gdv_WH_EFLOW_MAS.DataBind();
        if (rdoMOVEMENT_TYPE.SelectedValue == "261")
        {
            lbl_MOVEMENT_TYPE.Text = "領料單明細";
        }
        else
        {
            lbl_MOVEMENT_TYPE.Text = "退料單明細";
        }
        capData2();
    }
    private void capData2()
    {
        string sparam = "SELECT *,'" + cboOWNER.SelectedValue + "' OWNER FROM (SELECT B.[EMP_NO],A.[PLANT],[MOVEMENT_TYPE],[INSERT_TIME],[PK_NO],[NO],[AUFNR],A.[MATNR],[BDMNGS],[ENMNGS],[REASON],[POSTING],[APPLYINFO],[STATUS],(CASE WHEN [STATUS]=1 AND [POSTING]= 1  THEN N'已完成' WHEN [STATUS]=1 AND [POSTING]= 0 AND [ENMNGS]>0 THEN N'過帳異常' WHEN [STATUS]=1 AND [POSTING]= 0 AND [ENMNGS]=0 THEN N'已關單'  ELSE N'處理中' END ) STATUSNAME,[CONFIRM_TIME] FROM [TWM8].[dbo].[WH_EFLOW_MAS] A ";
        sparam += "LEFT JOIN  (SELECT DISTINCT [MATNR],[PLANT],[EMP_NO] FROM [TWM8].[dbo].[WH_INVENTORY_STOCK]) B ON A.MATNR=B.MATNR AND A.[PLANT]=B.[PLANT] ";
        sparam += " ) T1";
        sparam += " WHERE [MOVEMENT_TYPE]=" + rdoMOVEMENT_TYPE.SelectedValue;
        if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND [EMP_NO] LIKE '%" + cboOWNER.SelectedValue + "%' ";
        }
        if (!cbo_PLANT.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND A.[PLANT]='" + cbo_PLANT.SelectedValue + "' ";
        }
        if (!txt_MATNR.Text.ToString().Trim().Equals(""))
        {
            sparam += " AND A.[MATNR]='" + txt_MATNR.Text.ToString().Trim() + "' ";
        }

        if (!txt_EFLOW_NO.Text.ToString().Trim().Equals(""))
        {
            sparam += " AND [NO] =" + txt_EFLOW_NO.Text.ToString().Trim();
        }
        else
        {
            if (rdoCONFIRMTYPE.SelectedValue.ToString().Trim().Equals("A"))
            {
                sparam += " AND ([CONFIRM_TIME] IS NULL OR [STATUSNAME] IN (N'過帳異常',N'處理中'))";
            }
            else
            {
                sparam += " AND [CONFIRM_TIME] IS NOT NULL";
            }
        }
        sparam += " AND CONVERT(DATE,[INSERT_TIME]) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' ";

        sparam += " ORDER BY [INSERT_TIME],[NO],[AUFNR],[PK_NO] ";
        DataTable dt = ATMCdb.reDt(sparam);
        #region Add 庫存&待驗數量 by Apple at 20190215
        dt = ZGBSN(dt, SAPRFC.getSAPDB());
        #endregion
        gdv_WH_EFLOW_MASDATA.DataSource = dt;
        gdv_WH_EFLOW_MASDATA.DataBind();
        txt_PK_NO.Attributes.Add("onfocus", "this.select();");
        txt_PK_NO.Focus();
    }
    protected void gdv_WH_EFLOW_MAS_RowDataBound(object sender, GridViewRowEventArgs e)
    {

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
            string QA1 = e.Row.Cells[14].Text;
            string POSTING = ((DropDownList)e.Row.Cells[12].FindControl("ddl_POSTING")).SelectedValue;
            string STATUS = ((DropDownList)e.Row.Cells[13].FindControl("ddl_STATUS")).SelectedValue;
            if (QA1 == "處理中" || QA1 == "過帳異常") { e.Row.Cells[14].BackColor = System.Drawing.Color.Red; }
            if (QA1 == "已關單") { e.Row.Cells[14].BackColor = System.Drawing.Color.Yellow; }
            //if (QA == "0" && QA1 == "已完成") { e.Row.Cells[13].BackColor = System.Drawing.Color.Red; }
            if (POSTING == "0") { e.Row.Cells[12].BackColor = System.Drawing.Color.Red; }
            if (STATUS == "0") { e.Row.Cells[13].BackColor = System.Drawing.Color.Red; }
            if (PREPARATION_FLAG != "Y")
            {
                e.Row.Cells[17].Enabled = false;
                e.Row.Cells[18].Enabled = false;

            }
        }
    }

    protected void txt_PK_NO_TextChanged(object sender, EventArgs e)
    {
        if (txt_PK_NO.Text.ToString().ToUpper().Trim().Equals(""))

        {
            Response.Write(" <script   language=javascript> window.alert( '你沒有輸入流水號喔!!') </script> ");
            return;
        }

        if (txt_EMPNO.Text.ToString().ToUpper().Trim().Equals(""))
        {

            Response.Write(" <script   language=javascript> window.alert( '確效時，工號不可為空') </script> ");
            return;
        }

        CHECKPK_NO();

    }

    private void CHECKPK_NO()
    {
        string message = "", CONFIRM_TIME = "";
        int PK_NO = int.Parse(txt_PK_NO.Text.ToString());
        string REASON = txt_REASON.Text.ToString();
        string EMPNO = txt_EMPNO.Text.ToString().ToUpper().Trim();
        DataTable dt = ATMCdb.reDt("SELECT  [PK_NO],[CONFIRM_TIME],[CONFIRM_USER] FROM [TWM8].[dbo].[WH_EFLOW_MAS] WHERE [PK_NO]=" + PK_NO);
        if (dt.Rows.Count > 0)
        {
            CONFIRM_TIME = dt.Rows[0]["CONFIRM_TIME"].ToString();
            if (!CONFIRM_TIME.Equals(""))
            {
                message = "<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>";
                message += " <script   language=javascript> window.alert( '該流水號已確校過') </script> ";
                Response.Write(message);

            }
            else
            {
                string sparam = "UPDATE [TWM8].[dbo].[WH_EFLOW_MAS] SET [CONFIRM_TIME]=GETDATE(),[REASON]=[REASON]+'" + REASON + "' ,[CONFIRM_USER]='" + EMPNO + "' WHERE [PK_NO]=" + PK_NO;
                ATMCdb.Exsql(sparam);
                Response.Write("<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
                capData2();
            }
        }
        else
        {
            message = "<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>";
            message += " <script   language=javascript> window.alert( '不存在此流水號，請確認之') </script> ";
            Response.Write(message);

        }
    }

    protected void gdv_WH_EFLOW_MASDATA_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string PK_NO = "", REASON = "", ENMNGS = "", POSTING = "", STATUS = "";

        Button BTN = (Button)e.CommandSource;// 先抓到這個按鈕（我們設定了CommandName）
        GridViewRow myRow = (GridViewRow)BTN.NamingContainer; // 從你按下 Button按鈕的時候，NamingContainer知道你按下的按鈕在GridView「哪一列」！
        PK_NO = gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].Cells[4].Text;
        TextBox txt = (TextBox)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("txtREASON");
        REASON = txt.Text.ToString();
        TextBox txt2 = (TextBox)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("txtENMNGS");
        ENMNGS = txt2.Text.ToString();
        DropDownList ddl = (DropDownList)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("ddl_POSTING");
        POSTING = ddl.SelectedValue.ToString();
        DropDownList ddl2 = (DropDownList)gdv_WH_EFLOW_MASDATA.Rows[myRow.DataItemIndex].FindControl("ddl_STATUS");
        STATUS = ddl2.SelectedValue.ToString();
        if (e.CommandName == "edit_data")
        {
            UPDATEREASON(PK_NO, REASON, POSTING, STATUS);
            btn_search_Click(sender, e);

        }
        if (e.CommandName == "Posting_data")
        {
            string sparam = "UPDATE A  SET [POSTING]=1,[STATUS]=1,[REASON]=N'" + REASON + "【人工調整過帳】' FROM [TWM8].[dbo].[WH_EFLOW_MAS] A WHERE [PK_NO]=" + PK_NO;
            string sparam2 = "UPDATE A  SET [POSTING]=1,[ED_DATE]=getdate(),[NOTE]=N'人工調整過帳' FROM [TWM8].[dbo].[SKIN_WIP] A WHERE [EFLOW_MAS_PK_NO]=" + PK_NO;
            ATMCdb.Exsql(sparam);
            ATMCdb.Exsql(sparam2);
            btn_search_Click(sender, e);

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