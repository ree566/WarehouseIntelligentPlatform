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
using Microsoft.Reporting.WebForms;
using SAP.Middleware.Connector;


public partial class WH_STORAGE : System.Web.UI.Page
{
    string strLGORT = "0015,0008,0012,0055,0058", strWERKS = "TWM9";
    SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["m9PREPAREConnectionString"].ConnectionString);

    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
    protected void Page_Load(object sender, EventArgs e)
    {

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
            Cap_Vendor();
            Cap_OWNER();
            capData();
        }
       
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        capData();
    }
    private void Cap_Vendor()
    {

        ddl_Vendor.Items.Clear();
        string strwhere = "SELECT DISTINCT [VENDOR_CODE],[VENDOR_NAME] FROM [TWM8].[dbo].[WH_STORAGE] WHERE [STATUS]=0 ORDER BY [VENDOR_NAME] DESC";
        DataTable dtVENDOR = ATMCdb.reDt(strwhere);
        ddl_Vendor.DataSource = dtVENDOR;
        ddl_Vendor.DataTextField = "VENDOR_NAME";
        ddl_Vendor.DataValueField = "VENDOR_CODE";
        ddl_Vendor.DataBind();
        ddl_Vendor.Items.Insert(0, new ListItem("ALL", "ALL"));
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

    private void capData()
    {
        System.Text.StringBuilder sParam = new System.Text.StringBuilder();
        sParam.Append("SELECT A.[PK_NO],[PSTNG_DATE],[MATERIAL],[ED_DATE],(CASE WHEN [CACHE_NAME] is not null THEN [ENTRY_QNT] ELSE 0 END ) ENTRY_QNT,(CASE WHEN [CACHE_NAME] is null THEN [ENTRY_QNT] ELSE 0 END ) UNPULL_QNT,[STGE_LOC],[MOVEMENT_TYPE],[STORLOC_BIN],A.[EMP_NO],[VENDOR_CODE],[ENTRY_TIME],[VENDOR_NAME],[FIFO_FLAG],[CACHE_NAME],[CACHE_TIME]");
        sParam.Append(",(SELECT  COUNT([ENTRY_QNT]) FROM [TWM8].[dbo].[WH_STORAGE] AA WHERE AA.[MATERIAL]=A.[MATERIAL] AND [STATUS]=0 AND [DISPLAY_FALG]='Y'  AND [CACHE_NAME] <> '調整帳務'  ) COUNTNTRY_QNT ");
        sParam.Append(",(SELECT  SUM([ENTRY_QNT]) FROM [TWM8].[dbo].[WH_STORAGE] AA WHERE AA.[MATERIAL]=A.[MATERIAL] AND [STATUS]=0 AND [DISPLAY_FALG]='Y' AND [CACHE_NAME] <> '調整帳務') SUMENTRY_QNT ");
        sParam.Append(",(SELECT  COUNT([ENTRY_QNT]) FROM [TWM8].[dbo].[WH_STORAGE] AA WHERE AA.[MATERIAL]=A.[MATERIAL] AND [STATUS]=0 AND [DISPLAY_FALG]='Y' AND [CACHE_NAME] <> '調整帳務') COUNTNTRY_QNT ");
        sParam.Append(",CASE WHEN [DISPLAY_FALG]='N' THEN '已刪除' WHEN [STATUS]=1 THEN N'已上架' ELSE '' END TYPE ");
        sParam.Append("FROM [TWM8].[dbo].[WH_STORAGE] A    WHERE 1=1");

        if (!txt_MATNR.Text.ToString().Equals(""))
        {
            sParam.Append(" AND  [MATERIAL] LIKE'" + txt_MATNR.Text.ToString().Trim() + "%' ");
        }
        if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
        {
            sParam.Append("and A.[EMP_NO] LIKE '%" + cboOWNER.SelectedValue + "%' ");
        }

        switch (rdoCONFIRMTYPE.SelectedValue)
        {
            case "0":
                sParam.Append(" AND  [STATUS]=0 AND [DISPLAY_FALG]='Y' ");
                break;
            case "1":
                sParam.Append(" AND  [STATUS]=1  AND CONVERT(DATE,[ED_DATE]) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' " );
                break;
            case "2":
                sParam.Append("AND  [DISPLAY_FALG]='N'  AND CONVERT(DATE,[PSTNG_DATE]) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' ");
                break;
            case "-1":
                //sParam.Append(" ORDER BY  [PSTNG_DATE],[MAT_DOC],[EMP_NO],[MATERIAL] ");
                break;
        }
        //sParam.Append(" AND CONVERT(DATE,[ED_DATE]) BETWEEN '" + txtStartDate.Text + "' AND '" + txtEndDate.Text + "' ");


        switch (ddl_ORDERBY.SelectedValue)
        {
            case "0":
                sParam.Append(" ORDER BY  [VENDOR_CODE],[PSTNG_DATE],[MATERIAL] ");
                break;
            case "1":
                sParam.Append(" ORDER BY  [MATERIAL],[PSTNG_DATE] ");
                break;
            case "2":
                sParam.Append(" ORDER BY  [VENDOR_CODE],[MATERIAL],[PSTNG_DATE],[MAT_DOC] ");
                break;
            case "3":
                sParam.Append(" ORDER BY  [ED_DATE] ");
                break;
            case "-1":
                sParam.Append(" ORDER BY  [PSTNG_DATE],[MAT_DOC],[EMP_NO],[MATERIAL] ");
                break;

        }
        //DataSet set1 = new DataSet();
        //SqlDataAdapter sqlCommand = new SqlDataAdapter(sParam.ToString(), Conn);
        //sqlCommand.SelectCommand.CommandTimeout = 120;
        //sqlCommand.Fill(set1, "dtWH_STORAGE");
        //sqlCommand.Dispose();

        DataTable dtWH_STORAGE = ATMCdb.reDt(sParam.ToString());
        dtWH_STORAGE = SAPRFC.ZGBSN(dtWH_STORAGE, strWERKS, strLGORT, "MATERIAL", 1, SAPRFC.getSAPDB());
        ReportViewer1.LocalReport.EnableHyperlinks = true;
        ReportViewer1.LocalReport.DataSources.Clear();
        ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dtWH_STORAGE));
        ReportViewer1.LocalReport.ReportPath = Server.MapPath("./Report/V_WH_STORAGE.rdlc");
       // ReportParameter p1 = new ReportParameter("MAT_CAPTION_ORDER", cboOWNER.SelectedValue.ToString());
        //  ReportParameter p2 = new ReportParameter("UNIT_NO", rdoUNIT_NO.SelectedValue.ToString());
        // ReportParameter p2 = new ReportParameter("ShowTitle", CheckBox2.Checked.ToString());
     //   ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { p1 });
        ReportViewer1.LocalReport.Refresh();


    }

    //protected void ReportViewer1_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e)
    //{
    //    LocalReport lp = (LocalReport)e.Report;

    //}
    protected void Timer1_Tick(object sender, EventArgs e)
    {

    }

}