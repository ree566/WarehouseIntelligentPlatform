using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DLL_LINENOTIFY;

public partial class DNNOZPRDPLCLIST_M : System.Web.UI.Page
{
    string PK_NO = "";
    ATMCdb ATMCdb = new ATMCdb();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            capData();
        }
    }

    private void capData()
    {
        if (Request["PK_NO"] != null) { PK_NO = Request["PK_NO"].ToString(); }
         //PK_NO = "3";
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT [PK_NO],[MATNR],[ZWERKS],[NAME1],[ZPRDPLC_SAP],[MC_NAME] FROM [ATMC].[M3WH].[DNNOZPRDPLCLIST] ");
        sb.AppendFormat("WHERE [PK_NO]={0}", PK_NO);
        sb.AppendFormat(" AND [ZWERKS]='{0}'", "TWM3");
        DataTable dt = ATMCdb.reDt(sb.ToString());

        if (dt.Rows.Count > 0)
        {
            txtMATNR.Text = dt.Rows[0]["MATNR"].ToString();
            txtZWERKS.Text = dt.Rows[0]["ZWERKS"].ToString();
            txtNAME1.Text = dt.Rows[0]["NAME1"].ToString();
            txtMC_NAME.Text = dt.Rows[0]["MC_NAME"].ToString();
            txtZPRDPLC_SAP.Text = dt.Rows[0]["ZPRDPLC_SAP"].ToString();
            txtZPRDPLC_SAP.Attributes.Add("onfocus", "this.select();");
            txtZPRDPLC_SAP.Focus();
        }
        else
        {
            Response.Write("<script   language=javascript> window.alert( '查無相關資訊') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            //儲存後離開
            ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
        }
    }

    protected void btn_UPDATE_Click(object sender, EventArgs e)
    {
        Updatedata();
    }
    private void Updatedata()
    {
        if (Request["PK_NO"] != null) { PK_NO = Request["PK_NO"].ToString(); }
        // PK_NO = "3";
        StringBuilder sb = new StringBuilder();
        string ZPRDPLC_SAP = txtZPRDPLC_SAP.Text.ToString().Trim().ToUpper();
        if (!ZPRDPLC_SAP.Equals(""))
        {
            sb.Append("UPDATE [ATMC].[M3WH].[DNNOZPRDPLCLIST] SET ");
            sb.AppendFormat("[ZPRDPLC_SAP]=N'{0}' ", ZPRDPLC_SAP);
            sb.AppendFormat("WHERE [PK_NO]={0}", PK_NO);
            if (ATMCdb.Exsql(sb.ToString()))
            {
                //SENDLINENOTIFY(PK_NO);
                Response.Write("<script   language=javascript> window.alert( '產地資訊更新成功') </script><embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                //儲存後離開
                ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
            }
        }
    }
    
}