using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.SetFocus("UserName");
    }
    protected void userLogin_Authenticate(object sender, AuthenticateEventArgs e)
    {
        SqlConnection Conn = ATMCdb.GetCon();//連接資料庫
        Conn.Open();
        string sql = "SELECT  [PK_NO],[EMP_NO],[EMP_NAME],[PASSWD],'Y' PREPARATION_FLAG,[AUTHORITY] FROM [ATMC].[dbo].[WH_EMP_STD] WHERE [EMP_NO]=@login_account AND [AUTHORITY] IN ('High','Normal')";
        SqlCommand cmd = new SqlCommand(sql, Conn);
        cmd.Parameters.AddWithValue("@login_account", userLogin.UserName);
        try
        {
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                string password = dr.GetString(3).ToString();

                if (userLogin.Password.ToString().Trim().Equals(password))
                {
                    string userID = dr.GetValue(0).ToString();
                    Session["login_account"] = dr.GetString(1).ToString();
                    Session["name"] = dr.GetString(2).ToString();
                    Session["PASSWD"] = dr.GetString(3).ToString();
                    Session["PREPARATION_FLAG"] = dr.GetString(4).ToString();
                    Session["AUTHORITY"] = dr.GetString(5).ToString();
                    e.Authenticated = true;
                }
                else
                {
                    e.Authenticated = false;
                    userLogin.FailureText = "帳號或密碼錯誤，請重新確認";
                }

            }
            else
            {
                e.Authenticated = false;
                userLogin.FailureText = "帳號或密碼錯誤，請重新確認";
            }

        }
        finally
        {
            cmd.Cancel();
            Conn.Close();
        }

        if (e.Authenticated)
        {
            
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