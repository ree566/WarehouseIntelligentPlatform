using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.SetFocus("UserName");
    }
    protected void userLogin_Authenticate(object sender, AuthenticateEventArgs e)
    {
        string userID = null;
        int LOGINFLAG = 0;
        SqlDataReader dr = null;
        //Session.Abandon();
        SqlConnection Conn = ATMCdb.GetCon();//連接資料庫
        Conn.Open();
        string sql = "SELECT  [PK_NO],[EMP_NO],[EMP_NAME],[PASSWD],'Y' PREPARATION_FLAG,[AUTHORITY] FROM [ATMC].[dbo].[WH_EMP_STD] WHERE [EMP_NO]=@login_account AND [AUTHORITY] IN ('High','Normal')";
        SqlCommand cmd = new SqlCommand(sql, Conn);
        cmd.Parameters.AddWithValue("@login_account", userLogin.UserName);
        try
        {

                

            dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                userID = dr.GetValue(0).ToString();
                Session["login_account"] = dr.GetString(1).ToString();
                Session["name"] = dr.GetString(2).ToString();
                Session["PASSWD"] = dr.GetString(3).ToString();
                Session["PREPARATION_FLAG"] = dr.GetString(4).ToString();
                Session["AUTHORITY"] = dr.GetString(5).ToString();
                //Session["Login"] = "OK";  

            }

        }
        finally
        {
            cmd.Cancel();
            Conn.Close();
        }

        if (!string.IsNullOrEmpty(userID))
        {
            //If activeStatus = True Then
            //   System.Web.Security.FormsAuthentication.RedirectFromLoginPage(Session["login_account"].ToString(), false);

            if (userLogin.Password.ToString().Trim().Equals(Session["PASSWD"]))
            {
                LOGINFLAG = 1;
            }
            else
            {
                userLogin.FailureText = "密碼錯誤，請重新確認";
            }
        }
        else
        {
            e.Authenticated = false;
            //驗證失敗
            //If activeStatus = False Then
            //userLogin.FailureText = "此帳號被停用！"
            userLogin.FailureText = "此帳號無權限登入！";
        }

        if (LOGINFLAG == 1)
        {
            e.Authenticated = true;
            //驗證通過
            //Response.Redirect("Default.aspx");
            if (Session["URL"] != null)
            {
                Response.Redirect(Session["URL"].ToString());
            }
            else
            {
                Response.Redirect("Default.aspx");
                //--通過帳號與密碼的認證，就可以進入後端的管理區。
            }
        }
    }
   
}