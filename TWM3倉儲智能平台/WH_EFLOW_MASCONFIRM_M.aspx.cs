
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

public partial class WH_EFLOW_MASCONFIRM_M : System.Web.UI.Page
{
    //ATMCdb ATMCdb = new ATMCdb();
    //SAPRFC SAPRFC = new SAPRFC();
    //#region Add 庫存&待驗數量 by Apple at 20190215
    //string strLGORT = "0015,0008,0012,0055,0058", strWERKS = "TWM9";
    //#endregion
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!Page.IsPostBack)
    //    {
    //        Cap_OWNER();
    //        btn_search_Click(sender, e);
    //    }
    //}
    //protected void btn_search_Click(object sender, EventArgs e)
    //{
    //    capData();

    //}
    //protected void txt_EFLOW_NO_TextChanged(object sender, EventArgs e)
    //{
    //    if (!txt_EFLOW_NO.Text.ToString().Equals(""))
    //    {
    //        btn_search_Click(sender, e);
    //    }
    //}
    //private void capData()
    //{
    //    string sparam = "SELECT [MOVEMENT_TYPE],[INSERT_TIME],[PK_NO],[NO],[AUFNR],[MATNR],[BDMNGS],[ENMNGS],[REASON],[POSTING],[APPLYINFO],(CASE WHEN [STATUS]=1 THEN N'已完成' ELSE N'處理中' END ) STATUS,[CONFIRM_TIME],B.[EMP_NO] FROM [ATMC].[dbo].[WH_EFLOW_MAS] A ";
    //    sparam += "  LEFT JOIN [ATMC].[M9WH].[MAT_CAPTION_ORDER] B ON left(A.[MATNR],3)=B.[MAT_CAPTION]";
    //    sparam += " OR left(A.[MATNR] ,4)=B.[MAT_CAPTION] OR left(A.[MATNR] ,2)=B.[MAT_CAPTION] OR left(A.[MATNR] ,1)=B.[MAT_CAPTION]";
    //    sparam += " WHERE A.[MOVEMENT_TYPE]=" + rdoMOVEMENT_TYPE.SelectedValue;
    //    if (!cboOWNER.SelectedValue.Trim().Equals("ALL"))
    //    {
    //        sparam += " AND B.[EMP_NO]='" + cboOWNER.SelectedValue + "' ";
    //    }


    //    if (rdoCONFIRMTYPE.SelectedValue.ToString().Trim().Equals("A"))
    //    {
    //        sparam += " AND [CONFIRM_TIME] IS NULL";
    //    }
    //    else
    //    {
    //        sparam += " AND [CONFIRM_TIME] IS NOT NULL";
    //    }

    //    if (!txt_EFLOW_NO.Text.ToString().Trim().Equals(""))
    //    {
    //        sparam += " AND [NO] =" + txt_EFLOW_NO.Text.ToString().Trim();
    //    }
    //    sparam += " ORDER BY [INSERT_TIME],[NO],[AUFNR],[PK_NO] ";
    //    DataTable dt = ATMCdb.reDt(sparam);
    //    #region Add 庫存&待驗數量 by Apple at 20190215
    //    dt = ZGBSN(dt, SAPRFC.getSAPDB());
    //    #endregion
    //    gdv_WH_EFLOW_MASDATA.DataSource = dt;
    //    gdv_WH_EFLOW_MASDATA.DataBind();
    //    txt_PK_NO.Attributes.Add("onfocus", "this.select();");
    //    txt_PK_NO.Focus();
    //}
}