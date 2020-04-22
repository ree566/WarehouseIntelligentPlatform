using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WH_DNNOZPRDPLCLIST_CFM : System.Web.UI.Page
{
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
        string AUFNR = txtAUFNR.Text.ToString().Trim().ToUpper();
        string MATNR = txtMATNR.Text.ToString().Trim().ToUpper();
        string CFMTYPE = rdoCFM.SelectedValue;
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT A.[VBELN],A.[POSNR],A.[MATNR],A.[ZWERKS],A.[ZPRDPLC],A.[ZPRDPLC_SAP],B.[ZPRDPLC_SAP] ZPRDPLC_SAP_CFM ");
        sb.Append("FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A ");
        sb.Append("LEFT JOIN [ATMC].[M3WH].[DNNOZPRDPLCLIST_CFM] B ON A.VBELN=B.[VBELN] AND A.POSNR=B.POSNR ");
        sb.Append("where (ISNULL([ZPRDPLC],'')='' OR B.[CFM_FLAG] IS NULL) and A.[POSNR] is not null AND [ZWERKS]='TWM3' ");

        switch (CFMTYPE)
        {
            case "A":
                break;
            case "E":
                sb.Append(" AND  LEN(A.[ZPRDPLC_SAP]) > 0 ");
                break;
            case "N":
                sb.Append(" AND  LEN(A.[ZPRDPLC_SAP]) = 0 ");
                break;
        }

        if (!AUFNR.Equals(""))
        {
            sb.AppendFormat(" AND A.[VBELN]='{0}' ", AUFNR);
        }
        if (!MATNR.Equals(""))
        {
            sb.AppendFormat(" AND A.[MATNR]='{0}' ", MATNR);
        }




        sb.Append("order by A.[VBELN],A.[POSNR] ");

        try
        {
            DataTable dt = ATMCdb.reDt(sb.ToString());
            gdv_WH_DNNOZPRDPLCLIST_CFM.DataSource = dt;
            gdv_WH_DNNOZPRDPLCLIST_CFM.DataBind();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        capData();
    }

    protected void btn_UPDATE_Click(object sender, EventArgs e)
    {
        int icount = 0;
        int iPass = 0;
        int iFail = 0;
        string nresult = "";
        try
        {
            for (int i = 0; i < gdv_WH_DNNOZPRDPLCLIST_CFM.Rows.Count; i++)
            {
                icount += 1;
                CheckBox cb = (CheckBox)gdv_WH_DNNOZPRDPLCLIST_CFM.Rows[i].FindControl("CheckBox");
                TextBox txtZPRDPLC_SAP_CFM = (TextBox)gdv_WH_DNNOZPRDPLCLIST_CFM.Rows[i].FindControl("txtZPRDPLC_SAP_CFM");
                if (cb.Checked == true)
                {
                    string AUFNR = gdv_WH_DNNOZPRDPLCLIST_CFM.Rows[i].Cells[1].Text.ToString();
                    string POSNR = gdv_WH_DNNOZPRDPLCLIST_CFM.Rows[i].Cells[2].Text.ToString();
                    string MATNR = gdv_WH_DNNOZPRDPLCLIST_CFM.Rows[i].Cells[3].Text.ToString();
                    string ZPRDPLC_SAP_CFM = txtZPRDPLC_SAP_CFM.Text.ToString().Trim().ToUpper();
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb1 = new StringBuilder();
                    sb.Append("UPDATE [ATMC].[M3WH].[DNPREPARE_DETAIL] SET ");
                    sb.AppendFormat("[ZPRDPLC]='{0}', [REMARK]=N'人工更新產地' WHERE [VBELN]='{1}' AND [POSNR]='{2}';", ZPRDPLC_SAP_CFM, AUFNR, POSNR );
                    if (ATMCdb.Exsql(sb.ToString()))
                    {
                        iPass += 1;
                        if (ATMCdb.scalDs("SELECT [PK_ID] FROM [ATMC].[M3WH].[DNNOZPRDPLCLIST_CFM] WHERE [VBELN]='"+ AUFNR+"' AND [POSNR]='"+ POSNR+"'").Equals(""))
                        {
                            sb1.Append("INSERT INTO [ATMC].[M3WH].[DNNOZPRDPLCLIST_CFM]([VBELN],[POSNR],[MATNR],[ZPRDPLC_SAP],[CFM_FLAG])VALUES (");
                            sb1.AppendFormat("'{0}'", AUFNR);
                            sb1.AppendFormat(",'{0}'", POSNR);
                            sb1.AppendFormat(",'{0}'", MATNR);
                            sb1.AppendFormat(",N'{0}'", ZPRDPLC_SAP_CFM);
                            sb1.AppendFormat(",1)");
                        }
                        else
                        {
                            sb1.Append("UPDATE [ATMC].[M3WH].[DNNOZPRDPLCLIST_CFM] SET ");
                            sb1.AppendFormat("[ZPRDPLC]='{0}',AND [CFM_FLAG]=1 WHERE [VBELN]='{1}' AND [POSNR]='{2}';", ZPRDPLC_SAP_CFM, AUFNR, POSNR);
                        }
                        ATMCdb.Exsql(sb1.ToString());
                    }
                    else
                    {
                        iFail += 1;
                    }
                   
                }
            }
            nresult = "成功筆數：【" + iPass + "】失敗筆數：【" + iFail + "】";
        }
        catch (Exception ex)
        {
            nresult = ex.Message;
            throw new Exception(ex.Message);
        }
      
        Response.Write("<script   language=javascript> window.alert( '更新產地資訊"+ nresult+"') </script><embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
        btn_search_Click(sender, e);
    }

    


}