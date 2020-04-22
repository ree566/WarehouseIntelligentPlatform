using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MODIFY_STORAGE_BINLOG : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    string sMATNR = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            capData();
        }
    }
    private void capData()
    {
        if (Request["MATNR"].ToString() != "") { sMATNR = Request["MATNR"].ToString(); }
        System.Data.DataSet ds1 = new System.Data.DataSet();
        string sParam = "SELECT  [id],[MATNR] ,[WERKS],[LGORT] ,[STOCK_QTY],[LGPBE_OLD],[LGPBE_NEW],[SAP_FLAG],[CR_DATETIME],[EMPLR_ID]  FROM [TWM8].[M3WH].[MODIFY_STORAGE_BINLOG]  ";
        sParam += "WHERE [MATNR]='" + sMATNR + "'";
        sParam += "order by [CR_DATETIME] desc";
        DataTable dt = ATMCdb.reDt(sParam);
        if (dt.Rows.Count == 0)
        {
            Response.Write("<script   language=javascript> window.alert( '查無相關資訊') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            //儲存後離開
            ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
        }
        else
        {
            gdv_MODIFY_STORAGE_BINLOG.DataSource = dt;
            gdv_MODIFY_STORAGE_BINLOG.DataBind();
        }

       

    }
}