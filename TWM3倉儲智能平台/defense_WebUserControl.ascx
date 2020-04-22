<%@ Control Language="C#" AutoEventWireup="true" CodeFile="defense_WebUserControl.ascx.cs" Inherits="defense_WebUserControl" %>

<%
//**** 請勿 單獨 執行本範例！！****
//**** 請勿 單獨 執行本範例！！****
//**** 請勿 單獨 執行本範例！！**** 
        
Response.Write("<br /><br /><font color=blue>");
Response.Write("<h3>此為廠區內部使用！</h3></font>");
    
            if (Session["Login"] == null)   //***C#不加上這一段會報錯。
            {
                       Response.Redirect("Login.aspx");
                       Response.Write("<h3><font color=red><b>嚴重警告！</b></font>您的帳號、密碼錯誤！是非法使用者～</h3>");
                       Response.End();     //--註解：程式立刻終止！
            }
        
           //============================================
           //== Session如果是 null，一使用就會報錯。所以要用上面的判別式來預防。
           //============================================
            if (Session["Login"].ToString() == "OK")
            {
               //Response.Write("<h3>恭喜您，您成功登入，才會看見這一頁！ Session_Loging_End.aspx</h3><hr />");
               //Response.Write("<br />您的個人資料是----<br>");
               //Response.Write("<br />    帳號 =>  " + Session["u_name"].ToString());
               // Response.Write("<br />    姓名 =>  " + Session["u_realname"].ToString());
               // Response.Write("<br />    密碼 =>  " + Session["u_passwd"].ToString());
             //   Response.Write("<script language=javascript>history.go(-1);</script>");
            }
                
            else
            {
               Response.Write("<h3><font color=red><b>嚴重警告！</b></font>您的帳號、密碼錯誤！是非法使用者～</h3>");
               Response.End();     //--註解：程式立刻終止！
            }

    //*********************************************
    // 上面兩段程式可以改寫成 if ((String)Session["Login"] == "OK")
    // 取代上面兩段 if 判別式if (Session["Login"] == null) 與 if (Session["Login"].ToString() == "OK")

    // 因為.ToString()中，一定要有內容或是『 "" 』，
    // 如果為 null就會報錯(並未將物件參考設定為物件的執行個體。)，

    // 以上參考自MSDN中的Object.ToString()方法中的說明，
    // 『Your ToString override should not return String.Empty or a null string.』
    // http://msdn.microsoft.com/zh-tw/library/system.object.tostring.aspx            
%>