using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Zen.Barcode;

public partial class WH_DNPREPARE_Schedule : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    SAPRFC SAPRFC = new SAPRFC();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["URL"] = System.IO.Path.GetFileName(Request.PhysicalPath) + Request.Url.Query;
        if (!Page.IsPostBack)
        {
            Cap_EMP_NO();
           
            capdata();
        }
    }
    private void Cap_EMP_NO()
    {

        rdoEMP_NO.Items.Clear();
        string sparam = "SELECT  A.[EMP_NO] USER_NO,A.EMP_NO+A.[EMP_NAME] OWNER FROM [TWM8].[dbo].[EMP_STD] A WHERE [PREPARATION_FLAG]='Y' ";
        DataTable dt = ATMCdb.reDt(sparam);

        rdoEMP_NO.DataSource = dt;
        rdoEMP_NO.DataTextField = "OWNER";
        rdoEMP_NO.DataValueField = "USER_NO";
        rdoEMP_NO.DataBind();
        rdoEMP_NO.Items.Insert(0, new ListItem("ALL", "ALL"));
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        capdata();
    }
    private void capDatatrasfertime()
    {
        string sparam = "SELECT TOP 1 convert(varchar, [BK_DATE], 120)  + '  '+[STATUS] RESULT  FROM [ATMC].[IE].[BKLISTRD] WHERE [BK_NAME]=N'TWM3DN結轉'  ORDER BY [BK_DATE] DESC";
        lblMessage.Text = "DN結轉時間：" + ATMCdb.scalDs(sparam);
    }
    private void capdata()
    {
        string startDate = "", endDate = "";
        string sEMP_NO = rdoEMP_NO.SelectedValue.ToString().Trim();
        string AUFNR = txtAUFNR.Text.ToString().Trim().ToUpper();
        string MDDATEFLAG = rdoMDDATE.SelectedValue;
        string DONEPREPARE = rdoDONEPREPARE.SelectedValue;
        //INPUT標籤 一定要加name="XXXX" 否則Request會為空值
        if (Request.Form["startDate"] != null) { startDate = Request.Form["startDate"].ToString(); }
        if (Request.Form["endDate"] != null) { endDate = Request.Form["endDate"].ToString(); }
        StringBuilder sb = new StringBuilder();
        sb.Append("SELECT AA.*,ISNULL(CC.SKIN_WIPQTY,0) SKIN_WIPQTY  FROM ( ");
       sb.Append("SELECT  A.[PK_ID],A.[VBELN],A.[MODATE],A.[CFMShipping_DATE],A.[CFMShipping_USER],A.CFMPacking_DATE,A.CFMPacking_USER,A.[DN_Version],A.[DN_ShippingDate],A.[DN_PackingNote],A.[DN_FileName],A.[CR_DATETIME],A.[MD_DATETIME],A.[DN_CFC],A.[WH_REMARK],CASE WHEN A.[PRINTFLAG]='1' THEN N'已列印' ELSE '' END PRINTTYPE ");
        sb.Append(",COUNT(*) ITEMQTY,SUM(CASE WHEN [PUB_QTY]+[PUB_QTY_NPT]>0 THEN 1 ELSE 0 END) ENMNGSITEMQTY ");
        sb.Append(",SUM(CASE WHEN [PUB_QTY_NPT]>0 OR [LFIMG] < [PUB_QTY] THEN 1 ELSE 0 END) ENMNGSITEMQTY_NPT");
        sb.Append(",SUM(CASE WHEN [LFIMG]> [PUB_QTY]+[PUB_QTY_NPT] THEN 1 ELSE 0 END) OPENITEMQTY");
        sb.Append(",SUM(CASE WHEN [STOCK_QTY] < [LFIMG]-[PUB_QTY]+[PUB_QTY_NPT] THEN 1 ELSE 0 END) OUTSTOCK");
        sb.Append(" FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A");
        sb.Append(" WHERE LOADLISTFLAG=1 ");

        if (sEMP_NO != "ALL")
        {
            sb.AppendFormat("  AND [MAT_EMP_NO] LIKE '%{0}%'", sEMP_NO);
        }

        if (startDate.Equals(""))
        {
            startDate = DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");
        }
        if (endDate.Equals(""))
        {
            endDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
        }

        if (!AUFNR.Equals(""))
        {
            sb.AppendFormat(" AND [VBELN]='{0}'", AUFNR);
        }
        else
        {
            switch (MDDATEFLAG)
            {
                case "N":
                    sb.Append(" AND [CFMShipping_DATE] IS NULL");
                    sb.AppendFormat(" AND [DN_ShippingDate]>='2020-03-19' AND CONVERT(DATE,[DN_ShippingDate]) BETWEEN '{0}' AND '{1}' ", startDate, endDate);

                    break;
                case "E":
                    sb.Append(" AND [CFMShipping_DATE] IS NOT NULL");
                    sb.AppendFormat(" AND CONVERT(DATE,[CFMShipping_DATE]) BETWEEN '{0}' AND '{1}' ", startDate, endDate);
                    break;
                default:
                    sb.AppendFormat(" AND (CONVERT(DATE,[CFMShipping_DATE]) BETWEEN '{0}' AND '{1}'  OR CONVERT(DATE,[DN_ShippingDate]) BETWEEN '{0}' AND '{1}' )", startDate, endDate);
                    break;
            }

        }

        

        sb.Append(" GROUP BY [PK_ID],[VBELN],[MODATE],[CFMShipping_DATE],[CFMShipping_USER],[CFMPacking_DATE],[CFMPacking_USER],[DN_Version],[DN_ShippingDate],[DN_PackingNote],[DN_FileName],[CR_DATETIME],[MD_DATETIME],[DN_CFC],[WH_REMARK],[PRINTFLAG] ) AA");
        sb.Append(" LEFT JOIN  (SELECT [AUFNR],COUNT(*) SKIN_WIPQTY  FROM [TWM8].[dbo].[SKIN_WIP] B LEFT JOIN ");
        sb.Append(" [ATMC].[M3WH].[DNPREPARE_DETAIL] C ON B.[AUFNR]=C.[VBELN] AND B.[POSNR]=C.[POSNR] ");
        sb.Append("  WHERE [HAIR_MATERIAL]=1 AND C.[POSNR] IS NULL AND CONVERT(FLOAT,B.[PUB_QTY]) <> 0  GROUP BY B.[AUFNR]) CC ");
        sb.Append(" ON AA.VBELN=CC.AUFNR ");
        switch (DONEPREPARE)
        {
            case "N":
                sb.AppendFormat(" WHERE OPENITEMQTY > 0 OR ENMNGSITEMQTY_NPT > 0 OR OUTSTOCK>0");

                break;
            case "E":
                sb.AppendFormat(" WHERE OPENITEMQTY = 0 OR ENMNGSITEMQTY_NPT > 0 OR OUTSTOCK>0");
                break;
            default:

                break;
        }
        sb.Append(" ORDER BY [MODATE],[DN_CFC],[DN_ShippingDate],[VBELN],[DN_PackingNote]");


       DataTable dt = ATMCdb.reDt(sb.ToString());
        gdv_DNPREPARE_Schedule.DataSource = dt;
        gdv_DNPREPARE_Schedule.DataBind();
        capDatatrasfertime();
    }


    protected void gdv_DNPREPARE_Schedule_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //未補數
            string OPENITEMQTY = e.Row.Cells[7].Text;
            if (OPENITEMQTY != "0") { e.Row.Cells[7].BackColor = System.Drawing.Color.Red; }
            if (OPENITEMQTY == "0") { e.Row.Cells[1].BackColor = System.Drawing.Color.Yellow; }
            string PRINTTYPE = e.Row.Cells[16].Text;
            if (PRINTTYPE == "已列印") { e.Row.Cells[16].BackColor = System.Drawing.Color.Yellow; }
            if (PRINTTYPE == "已列印") { e.Row.Cells[0].Enabled = false; }
            //缺料數
            string OUTSTOCK = e.Row.Cells[8].Text;
            if (OUTSTOCK != "0") { e.Row.Cells[8].BackColor = System.Drawing.Color.Red; }
            //未退帳
            HyperLink HLK = (HyperLink)e.Row.FindControl("HyperLink2");
            string SKIN_WIPQTY = HLK.Text;
            if (SKIN_WIPQTY != "0") { e.Row.Cells[9].BackColor = System.Drawing.Color.Red; }
            //異常數
            string ENMNGSITEMQTY_NPT = e.Row.Cells[6].Text;
            if (ENMNGSITEMQTY_NPT != "0") { e.Row.Cells[6].BackColor = System.Drawing.Color.Red; }
            #region 整列變色
            //if (DateTime.Compare(DateTime.Parse(MODATE), DateTime.Parse(NowDate))<=0) { e.Row.BackColor = System.Drawing.Color.Yellow; }
            #endregion
            //備料日
            string MODATE = e.Row.Cells[2].Text;
            string NowDate = DateTime.Now.AddDays(0).ToString("MM-dd");
            if (DateTime.Compare(DateTime.Parse(MODATE), DateTime.Parse(NowDate)) <= 0) { e.Row.Cells[2].BackColor = System.Drawing.Color.Yellow; }

        }
    }
    protected void gdv_DNPREPARE_Schedule_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string PK_NO = "", sparam = "";
        Button BTN = (Button)e.CommandSource;// 先抓到這個按鈕（我們設定了CommandName）
        GridViewRow myRow = (GridViewRow)BTN.NamingContainer; // 從你按下 Button按鈕的時候，NamingContainer知道你按下的按鈕在GridView「哪一列」！
        PK_NO = gdv_DNPREPARE_Schedule.DataKeys[myRow.DataItemIndex].Value.ToString();//抓取ID
        if (e.CommandName == "PRINT_data")
        {
            //sparam = "DELETE FROM  [ATMC].[dbo].[SKIN_WIP]   WHERE [PK_NO]=" + PK_NO;
            //2019/11/20 刪除功能改為 [POSTING]=1 並備註之
            sparam = "UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET [PRINTFLAG]=1 WHERE [PK_ID]=" + PK_NO + ";";
            if (ATMCdb.Exsql(sparam))
            {
                // Response.Write(" <script   language=javascript> window.alert( '停用成功') </script> ");
            }
            else
            {
                Response.Write("<script   language=javascript> window.alert( '更新失敗，請洽詢管理者') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            }
            btn_search_Click(sender, e);
        }
    }
    #region 出貨確效
    protected void btn_CFMDN_Click(object sender, EventArgs e)
    {
        string CFMEMP_NO = txtCFMEMP_NO.Text.ToString().Trim().ToUpper();
        string CFMAUFNR = txtCFMAUFNR.Text.ToString().Trim().ToUpper();
        string CFMREMARK = txt_CFMREMARK.Text.ToString().ToUpper();
        if(ATMCdb.CHECKWHEMPLR_ID(CFMEMP_NO) != "Y")
        {
            Response.Write("<script   language=javascript> window.alert( '你沒有權限確效DN出貨') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        if (CFMAUFNR.Equals(""))
        {
         //   Response.Write("<script   language=javascript> window.alert( '你未填入任何出貨DN單號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        string nresult = CFM_AUFNR(CFMEMP_NO, CFMAUFNR, CFMREMARK);
        if (!nresult.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '" + nresult + "') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
        }
        else
        {
            txt_CFMREMARK.Text = "";
            txtCFMAUFNR.Text = "";
            Response.Write("<script   language=javascript> window.alert( 'DN：" + CFMAUFNR + "確效完成') </script><embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            capdata();

        }
        
        txtCFMAUFNR.Attributes.Add("onfocus", "this.select();");
        txtCFMAUFNR.Focus();
    }

    protected void txtCFMAUFNR_TextChanged(object sender, EventArgs e)
    {
        btn_CFMDN_Click(null, null);
    }
    private string CFM_AUFNR(string CFMEMP_NO, string CFMAUFNR,string CFMREMARK)
    {
        string nresult = "";
        string sparam = "SELECT [VBELN],[LOADLISTFLAG],[CFMShipping_DATE] FROM [ATMC].[M3WH].[DNPREPARE_Schedule] WHERE [VBELN]='" + CFMAUFNR + "'";
        DataTable dt = ATMCdb.reDt(sparam);
        if (dt.Rows.Count == 0)
        {
            //Response.Write("<script   language=javascript> window.alert( '查無相關DN訊息') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            nresult = "查無相關DN訊息";
        }
        else
        {
            string CFMShipping_DATE = dt.Rows[0]["CFMShipping_DATE"].ToString();
            string LOADLISTFLAG = dt.Rows[0]["LOADLISTFLAG"].ToString();

            string sparam1 = "SELECT COUNT(*) SKIN_WIPQTY  FROM [TWM8].[dbo].[SKIN_WIP] B LEFT JOIN ";
            sparam1 += " [ATMC].[M3WH].[DNPREPARE_DETAIL] C ON B.[AUFNR]=C.[VBELN] AND B.[POSNR]=C.[POSNR] ";
            sparam1 += " WHERE [HAIR_MATERIAL]=1 AND C.[POSNR] IS NULL AND CONVERT(FLOAT,B.[PUB_QTY]) <> 0 AND [AUFNR]='"+ CFMAUFNR+ "'";

            if (ATMCdb.scalDs(sparam1) !="0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有料件已扣帳不出貨，尚未完成退帳，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有料件已扣帳不出貨，尚未完成退帳，請重新確認";
                return nresult;
            }

            string sparam2 = "SELECT SUM(CASE WHEN [PUB_QTY_NPT]>0 OR [LFIMG] < [PUB_QTY]  THEN 1 ELSE 0 END) ENMNGSITEMQTY_NPT ";
            sparam2 += "FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A WHERE [VBELN]='"+ CFMAUFNR + "'" ;
            if (ATMCdb.scalDs(sparam2)!= "0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有異常筆數，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有異常筆數，請重新確認";
                return nresult;
            }

            string sparam3 = "SELECT SUM(CASE WHEN [LFIMG] > [PUB_QTY]  THEN 1 ELSE 0 END) OPENITEMQTY ";
            sparam3 += "FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A WHERE [VBELN]='" + CFMAUFNR + "'";
            if (ATMCdb.scalDs(sparam3) != "0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有異常筆數，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有未備齊數，請重新確認";
                return nresult;
            }

            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET ");
                sb.AppendFormat("[CFMShipping_DATE]=getdate()");
                sb.AppendFormat(",[DN_PackingNote]=N'{0}'", CFMREMARK);
                sb.AppendFormat(",[CFMShipping_USER]='{0}'", CFMEMP_NO);
                sb.AppendFormat(" WHERE [VBELN]='{0}'", CFMAUFNR);
                if (!ATMCdb.Exsql(sb.ToString()))
                {
                    nresult = "確效失敗";
                }
                else
                {

                }
            }
        }
        

        return nresult;
    }
    #endregion

    #region 包裝確效
    protected void btn_CFMPackDN_Click(object sender, EventArgs e)
    {
        string CFMPackEMP_NO = txtCFMPackEMP_NO.Text.ToString().Trim().ToUpper();
        string CFMPackAUFNR = txtCFMPackAUFNR.Text.ToString().Trim().ToUpper();
        string CFMPackREMARK = txt_CFMPackREMARK.Text.ToString().ToUpper();
        if (ATMCdb.CHECKWHEMPLR_ID(CFMPackEMP_NO) != "Y")
        {
            Response.Write("<script   language=javascript> window.alert( '你沒有權限確效DN包裝') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        if (CFMPackAUFNR.Equals(""))
        {
          //  Response.Write("<script   language=javascript> window.alert( '你未填入任何DN單號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        string nresult = CFMPack_AUFNR(CFMPackEMP_NO, CFMPackAUFNR, CFMPackREMARK);
        if (!nresult.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '" + nresult + "') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
        }
        else
        {
            txt_CFMPackREMARK.Text = "";
            txtCFMPackAUFNR.Text = "";
            Response.Write("<script   language=javascript> window.alert( 'DN：" + CFMPackAUFNR + "包裝效完成') </script><embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");

            capdata();

        }

        txtCFMPackAUFNR.Attributes.Add("onfocus", "this.select();");
        txtCFMPackAUFNR.Focus();
    }

    protected void txtCFMPackAUFNR_TextChanged(object sender, EventArgs e)
    {
        btn_CFMPackDN_Click(null, null);
    }
    private string CFMPack_AUFNR(string CFMPackEMP_NO, string CFMPackAUFNR, string CFMPackREMARK)
    {
        string nresult = "";
        string sparam = "SELECT [VBELN],[LOADLISTFLAG],[CFMPacking_DATE] FROM [ATMC].[M3WH].[DNPREPARE_Schedule] WHERE [VBELN]='" + CFMPackAUFNR + "'";
        DataTable dt = ATMCdb.reDt(sparam);
        if (dt.Rows.Count == 0)
        {
            //Response.Write("<script   language=javascript> window.alert( '查無相關DN訊息') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            nresult = "查無相關DN訊息";
        }
        else
        {
            string CFMPacking_DATE = dt.Rows[0]["CFMPacking_DATE"].ToString();
            string LOADLISTFLAG = dt.Rows[0]["LOADLISTFLAG"].ToString();

            string sparam1 = "SELECT COUNT(*) SKIN_WIPQTY  FROM [TWM8].[dbo].[SKIN_WIP] B LEFT JOIN ";
            sparam1 += " [ATMC].[M3WH].[DNPREPARE_DETAIL] C ON B.[AUFNR]=C.[VBELN] AND B.[POSNR]=C.[POSNR] ";
            sparam1 += " WHERE [HAIR_MATERIAL]=1 AND C.[POSNR] IS NULL AND CONVERT(FLOAT,B.[PUB_QTY]) <> 0 AND [AUFNR]='" + CFMPackAUFNR + "'";

            if (ATMCdb.scalDs(sparam1) != "0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有料件已扣帳不出貨，尚未完成退帳，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有料件已扣帳不出貨，尚未完成退帳，請重新確認";
                return nresult;
            }

            string sparam2 = "SELECT SUM(CASE WHEN [PUB_QTY_NPT]>0 OR [LFIMG] < [PUB_QTY]  THEN 1 ELSE 0 END) ENMNGSITEMQTY_NPT ";
            sparam2 += "FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A WHERE [VBELN]='" + CFMPackAUFNR + "'";
            if (ATMCdb.scalDs(sparam2) != "0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有異常筆數，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有異常筆數，請重新確認";
                return nresult;
            }

            string sparam3 = "SELECT SUM(CASE WHEN [LFIMG] > [PUB_QTY]  THEN 1 ELSE 0 END) OPENITEMQTY ";
            sparam3 += "FROM [ATMC].[M3WH].[VW_DNPREPARE_DETAIL] A WHERE [VBELN]='" + CFMPackAUFNR + "'";
            if (ATMCdb.scalDs(sparam3) != "0")
            {
                //Response.Write("<script   language=javascript> window.alert( '此DN有異常筆數，請重新確認') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                nresult = "此DN有未備齊數，請重新確認";
                return nresult;
            }

            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET ");
                sb.AppendFormat("[CFMPacking_DATE]=getdate()");
                sb.AppendFormat(",[WH_REMARK]=N'{0}'", CFMPackREMARK);
                sb.AppendFormat(",[CFMPacking_USER]='{0}'", CFMPackEMP_NO);
                sb.AppendFormat(" WHERE [VBELN]='{0}'", CFMPackAUFNR);
                if (!ATMCdb.Exsql(sb.ToString()))
                {
                    nresult = "包裝確效失敗";
                }
                else
                {

                }
            }
        }


        return nresult;
    }
    #endregion



    #region DN單備註修改
    protected void btn_WH_REMARK_Click(object sender, EventArgs e)
    {
        string REMARKAUFNR = txtREMARKAUFNR.Text.ToString().Trim().ToUpper();
        string WH_REMARK = txtWH_REMARK.Text.ToString();

        if (REMARKAUFNR.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '你未填入任何DN單號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        string nresult = CFM_REMARKAUFNR(REMARKAUFNR, WH_REMARK);
        if (!nresult.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '" + nresult + "') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
        }
        else
        {
            txtREMARKAUFNR.Text = "";
            txtWH_REMARK.Text = "";
            Response.Write("<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
            btn_search_Click(sender, e);
        }
    }
    private string CFM_REMARKAUFNR(string REMARKAUFNR, string WH_REMARK)
    {
        string nresult = "";
        string sparam = "SELECT [VBELN],[LOADLISTFLAG],[CFMShipping_DATE] FROM [ATMC].[M3WH].[DNPREPARE_Schedule] WHERE [VBELN]='" + REMARKAUFNR + "'";
        DataTable dt = ATMCdb.reDt(sparam);
        if (dt.Rows.Count == 0)
        {
            Response.Write("<script   language=javascript> window.alert( '查無相關DN訊息') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            nresult = "查無相關DN訊息";
        }
        else
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET ");
            sb.AppendFormat("[WH_REMARK]=ISNULL([WH_REMARK],'')+N'{0}'", WH_REMARK);
            sb.AppendFormat(" WHERE [VBELN]='{0}'", REMARKAUFNR);
            if (!ATMCdb.Exsql(sb.ToString()))
            {
                nresult = "備註加入失敗";
            }
        }
        return nresult;
    }
    #endregion


    #region DN單新增
    protected void btn_ADDAUFNR_Click(object sender, EventArgs e)
    {
        string ADDAUFNR = txtADDAUFNR.Text.ToString().Trim().ToUpper();
        if (ADDAUFNR.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '你未填入任何DN單號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        string nresult = capADDAUFNR(ADDAUFNR);
            Response.Write("<script   language=javascript> window.alert( '" + nresult + "') </script>");
            btn_search_Click(sender, e);

    }
    private string capADDAUFNR(string ADDAUFNR)
    {
        string nresult = "";
        string WMSsparam = "SELECT * FROM ( ";
        WMSsparam += " SELECT [DN_Number],[DN_Version],[DN_ShippingDate],[DN_PackingNote],[DN_FileName],[DN_CreateTime],[DN_CFC] ";
        WMSsparam += " ,ROW_NUMBER() OVER (PARTITION BY [DN_Number] ORDER BY [DN_Version] DESC) ROW_ID ";
        WMSsparam += " FROM [dbo].[ViewPrintedDeliveryNote] ";
        WMSsparam += " where [DN_Number]='" + ADDAUFNR + "' AND [DN_ShippingDate] IS NOT NULL) A ";
        WMSsparam += " WHERE   ROW_ID=1 ";
        DataTable dt = ATMCdb.reDtWMS(WMSsparam);
        if (dt.Rows.Count == 0)
        {
            Response.Write("<script   language=javascript> window.alert( '查無相關DN訊息') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            nresult = "查無相關DN訊息";
        }
        else
        {
            string DN_Version = dt.Rows[0]["DN_Version"].ToString();
            string DN_ShippingDate = dt.Rows[0]["DN_ShippingDate"].ToString();
            string MODATE = ATMCdb.scalDs("SELECT convert(varchar, [DATE], 111) FROM (SELECT ROW_NUMBER() OVER (ORDER BY [DATE] DESC) ROW_id,[DATE] FROM [ATMC].[IE].[WORKDAYCALENDAR] WHERE [WORKDAYFLAG]  = 1 AND [DATE]< '" + DN_ShippingDate + "') C WHERE  ROW_id = 3");

            string DN_PackingNote = dt.Rows[0]["DN_PackingNote"].ToString();
            string DN_FileName = dt.Rows[0]["DN_FileName"].ToString();
            // string DN_CreateTime = dt.Rows[0]["DN_CreateTime"].ToString();
            string DN_CFC = dt.Rows[0]["DN_CFC"].ToString();
            DataTable dt1 = ATMCdb.reDt("SELECT [VBELN],[DN_Version],[LOADLISTFLAG] FROM [ATMC].[M3WH].[DNPREPARE_Schedule] WHERE [VBELN]='" + ADDAUFNR + "'");
            StringBuilder sb = new StringBuilder();

            if (dt1.Rows.Count > 0)
            {
                string sDN_Version = dt1.Rows[0]["DN_Version"].ToString();
                string LOADLISTFLAG = dt1.Rows[0]["LOADLISTFLAG"].ToString();
                if ((int.Parse(DN_Version) > int.Parse(sDN_Version)) || !LOADLISTFLAG.Equals("1"))
                {
                    sb.Append("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET ");
                    sb.AppendFormat("[DN_Version]='{0}'", DN_Version);
                    if (!DN_ShippingDate.Equals(""))
                    {
                        sb.AppendFormat(",[DN_ShippingDate]='{0}'", DN_ShippingDate);
                    }
                    sb.AppendFormat(",[DN_PackingNote]=N'{0}'", DN_PackingNote);
                    sb.AppendFormat(",[DN_FileName]='{0}'", DN_FileName);
                    sb.AppendFormat(",[MODATE]='{0}'", MODATE);
                    sb.AppendFormat(",[MD_DATETIME]=getdate()");
                    sb.AppendFormat(",[UPDATEFLAG]=1");
                    sb.AppendFormat(",[DN_CFC]='{0}'", DN_CFC);
                    sb.AppendFormat(" WHERE [VBELN]='{0}'", ADDAUFNR);
                    if (ATMCdb.Exsql(sb.ToString()))
                    {

                        nresult += DOWNLOADDNDETAIL(ADDAUFNR, "U");
                    }
                    else
                    {
                        nresult += "\n" + "DN：" + ADDAUFNR + "出貨資訊版本更新失敗";
                    }
                }
                else
                {
                    if (CheckBox1.Checked)
                    {
                        nresult += DOWNLOADDNDETAIL(ADDAUFNR, "U");
                    }
                    else
                    {
                        nresult += "\n" + "DN：" + ADDAUFNR + "出貨資訊版本並無更新 不須異動";
                    }
                    
                }

            }
            else
            {
                sb.Append("INSERT INTO [M3WH].[DNPREPARE_Schedule]([VBELN],[MODATE],[DN_Version],[DN_ShippingDate],[DN_PackingNote],[DN_FileName],[LOADLISTFLAG],[DN_CFC]) VALUES( ");
                sb.AppendFormat("'{0}'", ADDAUFNR);
                sb.AppendFormat(",'{0}'", MODATE);
                sb.AppendFormat(",'{0}'", DN_Version);
                sb.AppendFormat(",'{0}'", DN_ShippingDate);
                sb.AppendFormat(",N'{0}'", DN_PackingNote);
                sb.AppendFormat(",'{0}'", DN_FileName);
                sb.AppendFormat(",0");
                sb.AppendFormat(",'{0}')", DN_CFC);
                if (ATMCdb.Exsql(sb.ToString()))
                {
                    
                    nresult += "\n" + "DN：" + ADDAUFNR + "DNPREPARE_Schedule 新增完成";
                    nresult += DOWNLOADDNDETAIL(ADDAUFNR, "A");
                }
            }
            CHECKDNNOZPRDPLCLIST_CFM(ADDAUFNR);
        }
        return nresult;
    }
    #region 下載DN明細 Filter "TWM3"的料件存至ATMC資料庫
    public string DOWNLOADDNDETAIL(string VBELN, string STYPE)
    {
        string nresult = "", STYPEstr = "";
        if (STYPE == "A")
        {
            STYPEstr = "新增";
        }
        else
        {
            STYPEstr = "更新";
        }
        try
        {

            DataTable dt = SAPRFC.ZGET_DN_INFO_2(VBELN, SAPRFC.getSAPDB());
            if (dt.Rows.Count > 0)
            {
                dt = ATMCdb.DataTableFilterSort2(dt, "ZWERKS='TWM3' AND LFIMG <> '0'", "");
            }


            if (dt.Rows.Count > 0)
            {
                ATMCdb.Exsql("DELETE FROM [ATMC].[M3WH].[DNPREPARE_DETAIL] WHERE [VBELN] = '" + VBELN + "'");
                string[] sColumnName = { "VBELN", "POSNR", "MATNR", "WERKS","LGORT"
                            ,"LFIMG","MAKTX","VGBEL","VGPOS","LIFNR","WEMNG","OPENQTY","NAME1"
                            ,"IMPORT","ZWERKS","ZPRDPLC","ZEMATN","ZTRADEMARK","ZLGORT"};
                if (ATMCdb.SqlBulkCopy(dt, sColumnName, "M3WH.DNPREPARE_DETAIL"))
                {
                    nresult += "\n" + "DN：" + VBELN + "DN明細" + STYPEstr + "成功";

                    ATMCdb.Exsql("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET [LOADLISTFLAG]=1,[UPDATEFLAG]=0 WHERE [VBELN]='" + VBELN + "'");
                    //去掉料號開頭0贅字 ATMC資料庫一致性
                    ATMCdb.Exsql("UPDATE [ATMC].[M3WH].[DNPREPARE_DETAIL]  SET [MATNR]=SUBSTRING(MATNR, PATINDEX('%[^0]%', MATNR), 100) WHERE [VBELN]='" + VBELN + "'");
                    
                }
                else
                {
                    nresult += "\n" + "DN：" + VBELN + "DN明細" + STYPEstr + "失敗";
                }
            }
            else
            {
                ATMCdb.Exsql("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET [LOADLISTFLAG]=-1,[UPDATEFLAG]=0 WHERE [VBELN]='" + VBELN + "'");
                nresult += "\n" + "DN：" + VBELN + "DN明細" + STYPEstr + "無TWM3料件";
            }
        }
        catch (Exception ex)
        {

            nresult += ex.Message;
        }
        return nresult;
    }
    #endregion
    #region 批量更新DNPREPARE_DETAIL產地資訊
    public void CHECKDNNOZPRDPLCLIST_CFM(string VBELN)
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE A SET ");
            sb.Append("A.[ZPRDPLC]=B.[ZPRDPLC_SAP]");
            sb.Append("FROM [ATMC].[M3WH].[DNPREPARE_DETAIL] A LEFT JOIN [ATMC].[M3WH].[DNNOZPRDPLCLIST_CFM] ");
            sb.Append("ON A.[VBELN]=B.[VBELN] AND A.[POSNR]=B.[POSNR] WHERE B.[CFM_FLAG]=1 AND ISNULL([ZPRDPLC],'')='' ");
            sb.AppendFormat(" AND A.[VBELN]='{0}'", VBELN);
            if (ATMCdb.Exsql(sb.ToString()))
            {
                
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    #endregion

    #endregion
    /// <summary>
    /// 取消出貨/包裝確效
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btn_CancelAUFNR_Click(object sender, EventArgs e)
    {
        string CancelAUFNR = txtCancelAUFNR.Text.ToString().Trim().ToUpper();
        if (CancelAUFNR.Equals(""))
        {
            Response.Write("<script   language=javascript> window.alert( '你未填入任何DN單號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }

        string CancelAUFNRTYPE = rdoCancelAUFNRTYPE.SelectedValue.ToString();
        if (CancelAUFNRTYPE == "-1")
        {
            Response.Write("<script   language=javascript> window.alert( '你未選擇取消類型) </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
            return;
        }
        string nresult = capCancelAUFNR(CancelAUFNR, CancelAUFNRTYPE);
        string title = "取消結果";
        if (nresult == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + title + "', '" + "修改完成" + "');", true);
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "ShowPopup('" + title + "', '" + nresult + "');", true);
        }
        btn_search_Click(sender, e);
    }
    private string capCancelAUFNR(string CancelUFNR, string CancelAUFNRTYPE)
    {
        StringBuilder sb = new StringBuilder();
        string nresult = "";
        switch (CancelAUFNRTYPE)
        {
            case "1":
                sb.AppendFormat("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET [CFMPacking_DATE]=Null,[CFMPacking_USER]=Null WHERE [VBELN] ='{0}'", CancelUFNR);
                break;
            case "2":
                sb.AppendFormat("UPDATE [ATMC].[M3WH].[DNPREPARE_Schedule] SET [CFMShipping_DATE]=Null,[CFMShipping_USER]=Null WHERE [VBELN] ='{0}'", CancelUFNR);
                break;
        }
        try
        {
            if (!ATMCdb.Exsql(sb.ToString()))
            {
                nresult = "取消失敗";
            }
        }
        catch (Exception e)
        {

            nresult = e.ToString();
        }

        return nresult;

    }




}