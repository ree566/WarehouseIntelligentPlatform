using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Text;
using System.Data;
using Zen.Barcode;

public class CWork
{
    public string id { get; set; }
    public string size { get; set; }
    public string name { get; set; }
    public byte[] BarcodeImg { get; set; }
}
public partial class DN_FileNamePRINT : System.Web.UI.Page
{

    ATMCdb ATMCdb = new ATMCdb();

    string sAUFNR = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        capData();
    }

    private void capData()
    {
        if (Request["AUFNR"] != null) { sAUFNR = Request["AUFNR"].ToString(); }
        //StringBuilder sb = new StringBuilder();
        //sb.Append(" SELECT [VBELN],[DN_ShippingDate],[DN_Version],[CR_DATETIME],[MD_DATETIME],[DN_PackingNote],[DN_CFC],[WH_REMARK] FROM [ATMC].[M3WH].[DNPREPARE_Schedule] ");
        //sb.AppendFormat(" WHERE [VBELN]='{0}'", sAUFNR);
        //DataTable dt = ATMCdb.reDt(sb.ToString());

        StringBuilder sb1 = new StringBuilder();
        sb1.Append(" SELECT [VBELN],[DN_ShippingDate],[DN_Version],[CR_DATETIME],[MD_DATETIME],[DN_PackingNote],[DN_CFC],[WH_REMARK],[POSNR],[MATNR],[MAKTX],[LFIMG],[PUB_QTY],[STOCK_QTY],[ZPRDPLC],[ZTRADEMARK],[VGBEL],[VGPOS],[ZWERKS],[ZLGORT],[MAT_EMP_NO],[ShippingMark] FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] ");
        sb1.AppendFormat(" WHERE [VBELN]='{0}'", sAUFNR);
        DataTable dt1 = ATMCdb.reDt(sb1.ToString());


        ReportViewer1.LocalReport.EnableHyperlinks = true;
        ReportViewer1.LocalReport.DataSources.Clear();
        //ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", dt));
        ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", dt1));
        ReportViewer1.LocalReport.ReportPath = Server.MapPath("Report/RPT_DN_File_2.rdlc");
        //ReportViewer1.LocalReport.Refresh();

    }


    protected void ReportViewer1_Drillthrough(object sender, Microsoft.Reporting.WebForms.DrillthroughEventArgs e)
    {
        LocalReport lp = (LocalReport)e.Report;

    }
}