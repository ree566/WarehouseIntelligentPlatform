using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

public partial class WH_SKIN_WIP_M : System.Web.UI.Page
{
    string sAUTHORITY = "";
    string sPREPARATION_FLAG = "";
    ATMCdb ATMCdb = new ATMCdb();
    string AUFNR = "", POSNR = "";
    int TYPE = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["PREPARATION_FLAG"] != null)
        {
            sPREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
            sAUTHORITY = Session["AUTHORITY"].ToString();
        }
        if (!Page.IsPostBack)
        {
            if (sPREPARATION_FLAG != "Y")
            {
                 btn_UPDATE.Enabled = false;

            }
            capData();
        }
    }
    private void capData()
    {
        if (Request["AUFNR"] != null) { AUFNR = Request["AUFNR"].ToString(); }
        if (Request["POSNR"] != null) { POSNR = Request["POSNR"].ToString(); }
        if (Request["TYPE"] != null) { TYPE = int.Parse(Request["TYPE"].ToString()); }

        StringBuilder sb = new StringBuilder();


        switch (TYPE)
        {
            case -1: //已過帳扣帳明細
                sb.Append("SELECT  [PK_NO],[AUFNR],[MODEL],[MATNR],[BDMNGS],[ENMNGS],[POSNR],[PUB_QTY],[ED_DATE],[NOTE],CASE WHEN [POSTING]=0 AND [HAIR_MATERIAL]=1 THEN N'過帳異常' ELSE N'正常' END   TYPE  FROM [TWM8].[dbo].[SKIN_WIP]");
                sb.AppendFormat(" WHERE [AUFNR]='{0}'", AUFNR);
                sb.AppendFormat(" AND [POSNR]='{0}'", POSNR);
                sb.Append(" AND [HAIR_MATERIAL]=1 ");
                sb.Append(" ORDER BY [ED_DATE] DESC");
                break;
            case 0: //DN已移除料件，已過帳扣帳尚未退回
                sb.Append("SELECT  B.[PK_NO],B.[AUFNR],B.[MODEL],B.[MATNR],B.[BDMNGS],B.[ENMNGS],B.[POSNR],B.[PUB_QTY],B.[ED_DATE],B.[NOTE],CASE WHEN B.[POSTING]=0 AND B.[HAIR_MATERIAL]=1 THEN N'過帳異常' ELSE N'正常' END   TYPE FROM [TWM8].[dbo].[SKIN_WIP] B");
                sb.Append(" LEFT JOIN  [ATMC].[M3WH].[DNPREPARE_DETAIL] C ON B.[AUFNR]=C.[VBELN] AND B.[POSNR]=C.[POSNR] ");
                sb.AppendFormat(" WHERE B.[AUFNR]='{0}'", AUFNR);
                sb.AppendFormat(" AND C.[POSNR] IS NULL AND CONVERT(FLOAT,B.[PUB_QTY])>0 ");
                sb.Append(" AND [HAIR_MATERIAL]=1 ");
                sb.Append(" ORDER BY [ED_DATE] DESC");

                break;
            default:
                break;
        }



        DataTable dt = ATMCdb.reDt(sb.ToString());
        ViewState["dtSKIN_WIP_M"] = dt;
        if (dt.Rows.Count == 0)
        {
            Response.Write("<script   language=javascript> window.alert( '查無相關智能扣帳資訊') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            //儲存後離開
            ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
        }
        gdv_WH_SKIN_WIP.DataSource = dt;
        gdv_WH_SKIN_WIP.DataBind();
    }

    protected void btn_UPDATE_Click(object sender, EventArgs e)
    {
        int icount = 0;
        int iPass = 0;
        int iFail = 0;
        for (int i = 0; i < gdv_WH_SKIN_WIP.Rows.Count; i++)
        {
            CheckBox cb = (CheckBox)gdv_WH_SKIN_WIP.Rows[i].FindControl("CheckBox");
            TextBox txtPUB_QTY = (TextBox)gdv_WH_SKIN_WIP.Rows[i].FindControl("txtPUB_QTY");
            DataTable dt = (DataTable)ViewState["dtSKIN_WIP_M"];
            if (cb.Checked == true)
            {
                int id = int.Parse(gdv_WH_SKIN_WIP.Rows[i].Cells[1].Text.ToString());
               
                double PUB_QTY = double.Parse(txtPUB_QTY.Text.ToString());
                string OldPUB_QTY = dt.Rows[i]["PUB_QTY"].ToString();
                string sparam = "UPDATE  [TWM8].[dbo].[SKIN_WIP] SET [PUB_QTY]=" + PUB_QTY + ",[NOTE]=ISNULL([NOTE],'')+N'扣帳數量【" + OldPUB_QTY + "】人工修改為【"+ PUB_QTY +"】' WHERE [PK_NO]=" + id;
                string sparam2 = "UPDATE [TWM8].[dbo].[SKIN_BIN_CARD] SET [TOTAL_STOCK]=[TOTAL_STOCK]-" + PUB_QTY + "+ " + OldPUB_QTY + ",[IN_STOCK]=0,[OUT_STOCK]=" + PUB_QTY + ",[NOTE]=ISNULL([NOTE],'')+N'原數量【" + OldPUB_QTY + "】人工修改為【" + PUB_QTY + "】' WHERE [NOTE] LIKE '%PK_NO-" + id + "%'";
                if (ATMCdb.Exsql(sparam))
                {

                    // 2020/02/11 已加到智能檢料流程中
                    //string AUFNR = gdv_WH_SKIN_WIP.Rows[i].Cells[2].Text.ToString();
                    //string POSNR = gdv_WH_SKIN_WIP.Rows[i].Cells[4].Text.ToString();
                    //DataTable dt1 = ATMCdb.reDt("SELECT [LIFNR],CASE WHEN [LIFNR]=[PUB_QTY] THE 1 ELSE 0 END LACK_MATERIAL FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] WHERE [VBELN]='" + AUFNR + "' AND [POSNR]='" + POSNR + "'");
                    //double LIFNR = double.Parse(dt1.Rows[0]["LIFNR"].ToString());
                    //string LACK_MATERIAL = dt1.Rows[0]["LACK_MATERIAL"].ToString();
                    //ATMCdb.Exsql("UPDATE  [ATMC].[dbo].[SKIN_WIP] SET [BDMNGS]="+ LIFNR+ ",[LACK_MATERIAL]="+ LACK_MATERIAL+ " WHERE [PK_NO]=" + id);

                    iPass += 1;
                }
                else
                {
                    iFail += 1;
                }

            }
        }
        capData();
        ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.Page), "工單發料數更新完畢", "alert('工單備料明細更新完畢!!');", true);
        Response.Write("<script type=\"javascript\">window.location.href=window.location.href;</script>");

    }

    protected void gdv_WH_SKIN_WIP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //string QA = e.Row.Cells[13].Text;
            string QA1 = e.Row.Cells[9].Text;
            if (QA1 == "過帳異常") { e.Row.Cells[9].BackColor = System.Drawing.Color.Red; }
        }
    }
}