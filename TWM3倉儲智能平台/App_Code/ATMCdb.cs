using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

/// <summary>
/// Summary description for ATMCdb
/// </summary>
public class ATMCdb
{

    #region 連接WMS資料庫抓取DN資訊
    public static SqlConnection GetConWMS()
    {
        return new SqlConnection(WebConfigurationManager.ConnectionStrings["WMSConnectionString"].ConnectionString);
    }

    public static DataTable reDtWMS(string cmdtxt)
    {
        SqlConnection con = GetConWMS();

        SqlDataAdapter da = new SqlDataAdapter(cmdtxt, con);
        da.SelectCommand.CommandTimeout = 360;
        //建立資料集ds
        DataSet ds = new DataSet();

        da.Fill(ds);
        return ds.Tables[0];
    }
    #endregion

    public static SqlConnection GetCon()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
        return new SqlConnection(WebConfigurationManager.ConnectionStrings["ATMCAConnectionString"].ConnectionString);
    }

    public static bool Exsql(string cmdtxt)
    {
        SqlConnection con = GetCon();//連接資料庫
        con.Open();
        SqlCommand cmd = new SqlCommand(cmdtxt, con);
        cmd.CommandTimeout = 360;
        try
        {
            cmd.ExecuteNonQuery();//執行SQL 語句並返回受影響的行數
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
        finally
        {
            con.Dispose();//釋放連接物件資源
            con.Close();
        }

    }
    /// <summary>
    /// 返回DataSet結果集
    /// </summary>
    /// <param name="P_Str_Condition">用來查詢的SQL語句</param>
    /// <returns>結果集</returns>
    public static DataSet reDs(string cmdtxt)
    {
        SqlConnection con = GetCon();
        SqlDataAdapter da = new SqlDataAdapter(cmdtxt, con);
        //建立資料集ds
        DataSet ds = new DataSet();
        da.Fill(ds);
        return ds;
    }

    public static DataTable reDt(string cmdtxt)
    {
        SqlConnection con = GetCon();

        SqlDataAdapter da = new SqlDataAdapter(cmdtxt, con);
        da.SelectCommand.CommandTimeout = 360;
        //建立資料集ds
        DataSet ds = new DataSet();

        da.Fill(ds);
        return ds.Tables[0];
    }
    public static string scalDs(string str_select)
    {
        //執行ExecuteScalar()，傳回單一字串,若遇NULL值,直接當空字串作
        //--------------------------------------------------------------------
        SqlConnection con = GetCon();
        SqlCommand com_select = new SqlCommand(str_select, con);
        com_select.CommandTimeout = 360;
        try
        {
            con.Open();
            str_select = Convert.ToString(com_select.ExecuteScalar());
        }
        catch (Exception ex)
        {
            con.Close();
            return Convert.ToString(ex);
        }
        finally
        {
            con.Close();
        }
        return str_select;
    }

    public bool scalstp(string cmdtxt)
    {

        SqlConnection con = GetCon();
        SqlCommand cm = new SqlCommand();
        cm.CommandType = CommandType.StoredProcedure;
        cm.Connection = con;
        cm.CommandText = cmdtxt;
        cm.CommandTimeout = 36000;
        try
        {

            //cn.Open();
            cm.ExecuteNonQuery();//執行SQL 語句並返回受影響的行數
            return true;
        }
        catch (Exception e)
        {
            // MessageBox.Show(e.ToString());
            return false;
        }
        finally
        {
            con.Dispose();//釋放連接物件資源
            con.Close();
        }
    }
    public void savemessage(string status, string sBK_NAME, string sBK_MESSAGE)
    {

        SqlConnection con = GetCon();
        try
        {
            con.Open();
            string str = "INSERT INTO [ATMC].[IE].[BKLISTRD] ([STATUS],[BK_NAME],[BK_MESSAGE])VALUES (@STATUS,@BK_NAME,@BK_MESSAGE)";
            SqlCommand cmd = new SqlCommand(str, con);

            cmd.Parameters.AddWithValue("@STATUS", status);
            cmd.Parameters.AddWithValue("@BK_NAME", sBK_NAME);
            cmd.Parameters.AddWithValue("@BK_MESSAGE", sBK_MESSAGE);
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            //MessageBox.Show(ex.ToString());
        }
        finally
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

    }
    public bool SqlBulkCopy(DataTable fromdt, string[] sColumnName, string UTablename)
    {
        SqlConnection con = GetCon();


        //宣告SqlBulkCopy
        using (SqlBulkCopy sqlBC = new SqlBulkCopy(con))
        {
            //設定一個批次量寫入多少筆資料
            sqlBC.BatchSize = 10000;
            //設定逾時的秒數
            sqlBC.BulkCopyTimeout = 60;
            //設定 NotifyAfter 屬性，以便在每複製 10000 個資料列至資料表後，呼叫事件處理常式。
            sqlBC.NotifyAfter = 10000;
            sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
            //設定要寫入的資料庫
            sqlBC.DestinationTableName = UTablename;
            for (int j = 0; j < sColumnName.Length; j++)
            {
                sqlBC.ColumnMappings.Add(sColumnName[j].ToString(), sColumnName[j].ToString());
                string dd = sColumnName[j].ToString();
            }

            #region 對應資料行
            //
            //sqlBC.ColumnMappings.Add("POWER_ID", "POWER_ID");
            //sqlBC.ColumnMappings.Add("POWER_DATE", "POWER_DATE");
            //sqlBC.ColumnMappings.Add("USER_NO", "USER_NO");
            //sqlBC.ColumnMappings.Add("USER_NAME", "USER_NAME");
            //sqlBC.ColumnMappings.Add("UNIT_NO", "UNIT_NO");
            //sqlBC.ColumnMappings.Add("CLASS_ID", "CLASS_ID");
            //sqlBC.ColumnMappings.Add("OVER_H", "OVER_H");
            //sqlBC.ColumnMappings.Add("PREPAR_REST_H", "PREPAR_REST_H");
            //sqlBC.ColumnMappings.Add("FACT_REST_H", "FACT_REST_H");
            //sqlBC.ColumnMappings.Add("LEAVE_H", "LEAVE_H");
            //sqlBC.ColumnMappings.Add("CREATE_DATE", "CREATE_DATE");
            //sqlBC.ColumnMappings.Add("UPDATE_DATE", "UPDATE_DATE");
            //sqlBC.ColumnMappings.Add("UNIT_NAME", "UNIT_NAME");
            //sqlBC.ColumnMappings.Add("CLASS_NAME", "CLASS_NAME");
            //sqlBC.ColumnMappings.Add("LINE_ID", "LINE_ID");
            //sqlBC.ColumnMappings.Add("STATION_ID", "STATION_ID");
            //sqlBC.ColumnMappings.Add("BREAK_TIME", "BREAK_TIME");
            //sqlBC.ColumnMappings.Add("WERKS", "WERKS");
            //sqlBC.ColumnMappings.Add("FACT_WORK_H", "FACT_WORK_H");
            //sqlBC.ColumnMappings.Add("CLASS_NO", "CLASS_NO");
            //sqlBC.ColumnMappings.Add("DEPT_NO", "DEPT_NO");
            #endregion
            try
            {
                con.Open();
                //開始寫入
                sqlBC.WriteToServer(fromdt);
                return true;
            }
            catch (Exception e)
            {
                //  MessageBox.Show();
                //  S.WriteTextFile2(Application.StartupPath, e.ToString(), "換線工時批量結轉");
                return false;
            }
            finally
            {
                fromdt.Dispose();
                sqlBC.Close();
                con.Dispose();//釋放連接物件資源
                con.Close();
            }
        }

    }
    void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
    {
        //   Response.Write("---<br/>");
    }

    #region 將DataTable經由過濾，排序得到新的資料表。
    // 提供兩個方法，一種是將DataTable轉成DataView，利用DataView的RowFilter及Sort方法得到所需資料，然後再將DataView轉成DataTable。
    //另一種方法是利用DataTable.Select().CopyToDataTable()方式得到資料表。
    //方式1
    //無法繫結至沒有名稱的 DataTable
    public DataTable DataTableFilterSort1(DataTable oTable, string filterExpression, string sortExpression)
    {
        DataView dv = new DataView();
        DataTable nTable = new DataTable();
        if (oTable.Rows.Count > 0)
        {
            dv.Table = oTable;
            dv.RowFilter = string.Format(filterExpression);
            dv.Sort = sortExpression;
            if (dv.Count > 0)
            {
                nTable = dv.ToTable();
            }
        }

        return nTable;
    }
    //方式2
    public DataTable DataTableFilterSort2(DataTable oTable, string filterExpression, string sortExpression)
    {
        DataTable nTable = new DataTable();
        if (oTable.Rows.Count > 0)
        {
            DataRow[] rows = oTable.Select(filterExpression, sortExpression);
            if (rows.Length > 0)
            {
                nTable = rows.CopyToDataTable();

            }
        }
        return nTable;
    }

    #endregion

    #region 判定是否為數字 若不是則回傳0
    public double checkisnumber(string sNUM)
    {
        Double n;
        if (Double.TryParse(sNUM, out n))
        {
            return double.Parse(sNUM);
        }
        else
        {
            return 0;
        }
    }
    #endregion

    /// <summary>
    /// 確認工號是否為智能檢料系統倉庫人員
    /// </summary>
    /// <param name="EMPLR_ID"></param>
    /// <returns></returns>
    public string CHECKWHEMPLR_ID(string EMPLR_ID)
    {
        return "Y";
        //string sparam = "SELECT 'Y' FROM [TWM8].[dbo].[WH_EMP_STD] WHERE [EMP_NO]='" + EMPLR_ID + "' AND [AUTHORITY] IN ('High','Normal')";
        //return ATMCdb.scalDs(sparam);
    }

    /// <summary>
    /// 判斷是否為日期
    /// </summary>
    /// <param name="strDate"></param>
    /// <returns></returns>
    public bool IsDate(string strDate)
    {
        try
        {
            DateTime.Parse(strDate);
            return true;
        }
        catch
        {
            return false;
        }
    }
}