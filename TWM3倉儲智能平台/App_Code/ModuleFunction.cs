using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for ModuleFunction
/// </summary>
public class ModuleFunction
{
    ATMCdb ATMCdb = new ATMCdb();


    /// <summary>
    /// 修改儲位紀錄LOG存取
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="LGORT"></param>
    /// <param name="LGPBE_OLD"></param>
    /// <param name="LGPBE_NEW"></param>
    /// <param name="SAP_FLAG"></param>
    /// <param name="EMPLR_ID"></param>
    /// <param name="STOCK_QTY"></param>
    public void BKMODIFY_STORAGE_BINLOG(string MATNR, string WERKS, string LGORT, string LGPBE_OLD, string LGPBE_NEW, int SAP_FLAG, string EMPLR_ID ,string STOCK_QTY)
    {
            string strwhere = "INSERT INTO [TWM8].[M3WH].[MODIFY_STORAGE_BINLOG] ([MATNR],[WERKS],[LGORT],[LGPBE_OLD],[LGPBE_NEW],[SAP_FLAG],[EMPLR_ID],[CR_DATETIME],[STOCK_QTY]) VALUES(";
            strwhere += "'" + MATNR + "',";
            strwhere += "'" + WERKS + "',";
            strwhere += "'" + LGORT + "',";
            strwhere += "'" + LGPBE_OLD + "',";
            strwhere += "'" + LGPBE_NEW + "',";
            strwhere += "" + SAP_FLAG + ",";
            strwhere += "'" + EMPLR_ID + "',";
            strwhere += "GETDATE(),";
        strwhere += STOCK_QTY + ")";
        ATMCdb.Exsql(strwhere);
        if (SAP_FLAG == 1)
        {
            string sparam = "UPDATE [TWM8].[dbo].[WH_INVENTORY_STOCK] SET [BIN]='" + LGPBE_NEW + "' WHERE [MATNR]='" + MATNR + "' AND [LGORT]='" + LGORT + "' AND [PLANT]='" + WERKS + "'";
            ATMCdb.Exsql(sparam);
        }
    }

    /// <summary>
    /// 確認工號是否為智能檢料系統倉庫人員
    /// </summary>
    /// <param name="EMPLR_ID"></param>
    /// <returns></returns>
    public string CHECKWHEMPLR_ID(string EMPLR_ID)
    {
        string sparam = "SELECT 'Y' FROM [TWM8].[dbo].[WH_EMP_STD] WHERE [EMP_NO]='" + EMPLR_ID + "' AND [AUTHORITY] IN ('High','Normal')";
        return ATMCdb.scalDs(sparam);
    }

    /// <summary>
    /// 盤點功能
    /// </summary>
    /// <param name="EMPLR_ID"></param>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="LGORT"></param>
    /// <param name="LGPBE"></param>
    /// <param name="LABST"></param>
    /// <param name="LABSTLAST"></param>
    /// <param name="LABSTLASTNOTE"></param>
    /// <returns></returns>
    

}