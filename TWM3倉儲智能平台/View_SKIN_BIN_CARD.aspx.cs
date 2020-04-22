using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class View_SKIN_BIN_CARD : System.Web.UI.Page
{
    string sMATNR = "", sLGORT="9999";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            capData();
        }
    }

    private void capData()
    {
        if (Request["MATNR"].ToString() != "") { sMATNR = Request["MATNR"].ToString().TrimStart('0'); ; }
        if (Request["LGORT"].ToString() != "") { sLGORT = Request["LGORT"].ToString(); ; }

        using (SqlConnection Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ATMCAConnectionString"].ConnectionString))
        {

            System.Data.DataSet ds1 = new System.Data.DataSet();
            string sParam = "SELECT [PK_NO],[MATNR],[LGORT],[STORLOC_BIN],[DESCRIPTION],[IN_STOCK],[OUT_STOCK],[TOTAL_STOCK],[TYPE],[NOTE],[EMP_NAME],[CR_DATE]  FROM [TWM8].[dbo].[View_SKIN_BIN_CARD] ";
            sParam += "WHERE [MATNR]='" + sMATNR + "' AND [LGORT]='"+ sLGORT+"' ";
            sParam += "order by [PK_NO] desc";

            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter sqlCommand = new SqlDataAdapter(sParam, Conn);
            sqlCommand.SelectCommand.CommandTimeout = 120;
            sqlCommand.Fill(ds1, "SKIN_BIN_CARD");
            dt = ds1.Tables["SKIN_BIN_CARD"];
            sqlCommand.Dispose();
            gdv_SKIN_BIN_CARD.DataSource = dt;
            gdv_SKIN_BIN_CARD.DataBind();
        }
    }
}