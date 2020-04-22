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

public partial class WH_SKIN_WIP : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    string sAUTHORITY = "";
    string sPREPARATION_FLAG = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["URL"] = System.IO.Path.GetFileName(Request.PhysicalPath) + Request.Url.Query;
        if (Session["PREPARATION_FLAG"] != null)
        {
            sPREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
            sAUTHORITY = Session["AUTHORITY"].ToString();
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
            txtStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            Cap_OWNER();
            Cap_MOVEMENT_TYPE();
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
    private void Cap_MOVEMENT_TYPE()
    {
        cboMOVEMENT_TYPE.Items.Clear();
        // string strwhere = "SELECT DISTINCT [USER_NO],[USER_NO]+[USER_NAME_CH] OWNER FROM [MES_QryUserInfo001] WHERE [DEPT_NO]='MC10' and [LINE_DESC]='WAREHOUSE' AND [cr_datetime]>=CONVERT(DATE,GETDATE()-1) ";
        string sparam = "SELECT [CODE] ,[CODE]+[NAME] CODENAME FROM [TWM8].[dbo].[SKIN_MOVEMENT_TYPE] WHERE [PRIMARY_FLAG]='Y' ";
        DataTable dt = ATMCdb.reDt(sparam);
        cboMOVEMENT_TYPE.DataSource = dt;
        cboMOVEMENT_TYPE.DataTextField = "CODE";
        cboMOVEMENT_TYPE.DataValueField = "CODENAME";
        cboMOVEMENT_TYPE.DataBind();
        cboMOVEMENT_TYPE.Items.Insert(0, new ListItem("ALL", "ALL"));
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        capData();

    }

    private void capData()
    {
        string sparam = "SELECT  A.[PK_NO],A.[AUFNR],[MODEL],A.[MATNR],[LGORT],A.[BDMNGS],A.[ENMNGS],[SAP_BIN],[POSNR],[PUB_QTY] ";
        sparam += ",A.[MOVEMENT_TYPE],C.[NAME],[REMOVE_STLOC],[REMOVE_PLANT],[HAIR_MATERIAL],A.[POSTING],[EMP_NO],[MACHINE_NO]";
        sparam += ",[PRINT_FLAG],[CR_DATE],[ED_DATE],[EFLOW_MAS_PK_NO],B.NO,CASE WHEN  A.[POSTING]=0 AND A.[HAIR_MATERIAL]=1 THEN N'過帳異常' ELSE N'正常' END   TYPE ";
        sparam += ",A.[NOTE] FROM [TWM8].[dbo].[SKIN_WIP] A LEFT JOIN [TWM8].[dbo].[WH_EFLOW_MAS] B ON A.EFLOW_MAS_PK_NO=B.PK_NO";
        sparam += " LEFT JOIN [TWM8].[dbo].[SKIN_MOVEMENT_TYPE] C ON A.MOVEMENT_TYPE=C.CODE";
        sparam += " WHERE  CONVERT(DATE,[ED_DATE]) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' ";
        if (!txt_MATNR.Text.ToString().Trim().Equals(""))
        {
            sparam += " AND A.[MATNR]='" + txt_MATNR.Text.ToString().Trim() + "' ";
        }
        if (!txt_WIP_NO.Text.ToString().Trim().Equals(""))
        {
            sparam += " AND A.[AUFNR]='" + txt_WIP_NO.Text.ToString().Trim() + "' ";
        }
        if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND A.[EMP_NO]='" + cboOWNER.SelectedValue + "' ";
        }
        if (!cboMOVEMENT_TYPE.SelectedValue.Trim().Equals("ALL"))
        {
            sparam += " AND A.[MOVEMENT_TYPE]='" + cboMOVEMENT_TYPE.SelectedValue + "' ";
        }
        if (!rdoCONFIRMTYPE.SelectedValue.ToString().Trim().Equals("A"))
        {
            sparam += " AND A.[POSTING]=0 AND A.[HAIR_MATERIAL]=1";
        }
        sparam += " order by [AUFNR],[EMP_NO],[ED_DATE] ";
        gdv_WH_SKIN_WIP.DataSource = ATMCdb.reDt(sparam);
        gdv_WH_SKIN_WIP.DataBind();
    }

    protected void gdv_WH_SKIN_WIP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //string QA = e.Row.Cells[13].Text;
            string QA1 = e.Row.Cells[17].Text;
            if (QA1 == "過帳異常") { e.Row.Cells[17].BackColor = System.Drawing.Color.Red; }
            if (sPREPARATION_FLAG != "Y")
            {
                e.Row.Cells[0].Enabled = false;
                e.Row.Cells[1].Enabled = false;
            }
        }
    }
    protected void gdv_WH_SKIN_WIP_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string PK_NO = "",sparam="";

        Button BTN = (Button)e.CommandSource;// 先抓到這個按鈕（我們設定了CommandName）
        GridViewRow myRow = (GridViewRow)BTN.NamingContainer; // 從你按下 Button按鈕的時候，NamingContainer知道你按下的按鈕在GridView「哪一列」！
        PK_NO = gdv_WH_SKIN_WIP.Rows[myRow.DataItemIndex].Cells[2].Text;
        
        if (e.CommandName == "edit_data")
        {
            sparam = "UPDATE [TWM8].[dbo].[SKIN_WIP] SET [POSTING]=1,[NOTE]=N'人工調整過帳' WHERE [PK_NO]=" + PK_NO;
            ATMCdb.Exsql(sparam);
            btn_search_Click(sender, e);

        }
        if (e.CommandName == "delete_data")
        {
            //sparam = "DELETE FROM  [ATMC].[dbo].[SKIN_WIP]   WHERE [PK_NO]=" + PK_NO;
            //2019/11/20 刪除功能改為 [POSTING]=1 並備註之
            sparam = "UPDATE [TWM8].[dbo].[SKIN_WIP] SET [POSTING]=1,[NOTE]=N'人工刪除該筆過帳紀錄' WHERE [PK_NO]=" + PK_NO + ";";
            sparam += "UPDATE [TWM8].[dbo].[SKIN_BIN_CARD] SET [TOTAL_STOCK]=[TOTAL_STOCK]+[OUT_STOCK]-[IN_STOCK],[IN_STOCK]=0,[OUT_STOCK]=0,[NOTE]=ISNULL([NOTE],'')+N'人工刪除該筆過帳紀錄' WHERE [NOTE] LIKE '%" + PK_NO + "%'";
            ATMCdb.Exsql(sparam);
            btn_search_Click(sender, e);
        }
    }
}