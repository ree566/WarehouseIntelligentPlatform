using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Calendar_aspx : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sScript = null;
        string sTextBoxID = null;

        //取得要輸入日期的 TextBox
        sTextBoxID = Request.QueryString["TextBoxId"];
        if (Calendar1.SelectedDate.ToString("yyyy/MM/dd") != "0001/01/01")
        {
            //將日期設給 TextBox，並將視窗關閉
            sScript = "opener.window.document.getElementById('" + sTextBoxID + "').value='" + Calendar1.SelectedDate.ToString("yyyy/MM/dd") + "';";
            sScript = sScript + "window.close();";
            this.ClientScript.RegisterStartupScript(this.GetType(), "_Calendar", sScript, true);
        }
    }
    
}

//使用方式
  //              Dim sScript As String
  //              Dim sScriptl As String
  //              Dim surl As String
  //              Dim sur2 As String
  //              ' 日期輸入的頁面，將 TextBox 以 TextBoxId 網址參數傳給日期頁面
  //              surl = "calendar.aspx?TextBoxID=" & TextBox7.ClientID
  //              sScript = "window.open('" & surl & "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')"
  //              TextBox7.Attributes("onclick") = sScript
  //              sur2 = "calendar.aspx?TextBoxID=" & TextBox8.ClientID
  //              sScriptl = "window.open('" & sur2 & "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')"
  //              TextBox8.Attributes("onclick") = sScriptl