using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.XSSF.UserModel;
//-- XSSF 用來產生Excel 2007檔案（.xlsx）

using NPOI.SS.UserModel;
//-- v.1.2.4起 新增的。
using NPOI.SS.Util;
using System.Web.Configuration;
using SAP.Middleware.Connector;

public partial class WIP_NOSAPCO03 : System.Web.UI.Page
{
    #region Add 庫存&待驗數量 by Apple at 20190215
    string strLGORT = "0015,0008,0012,0055,0058", strWERKS = "TWM9";
   
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    private void capData()
    {
        DataSet ds = ZGET_SAP_SODNWO_DATA(getSAPDB(), txt_WIP_NO.Text.ToString().Trim(), "", "", "");
        DataTable dt = ds.Tables[1];
        ViewState["BAUGR"] = "";
        if (ds.Tables[0].Rows.Count > 0)
        {
            ViewState["BAUGR"] = ds.Tables[0].Rows[0]["MATNR"].ToString();

        }
        dt = ZGBSN(dt, getSAPDB());


        gdv_WOSTATUS.DataSource = ds.Tables[0];
        gdv_WOSTATUS.DataBind();
        gdv_WIP_NOSAPCO03.DataSource = dt;
        gdv_WIP_NOSAPCO03.DataBind();
    }

    #region SAP連線 add by Apple at 20190215
    private RfcDestination getSAPDB()
    {
        IDestinationConfiguration ID = new SAP_FRC();
        try
        { RfcDestinationManager.UnregisterDestinationConfiguration(ID); RfcDestinationManager.RegisterDestinationConfiguration(ID); }
        catch
        { }
        RfcDestination dest = RfcDestinationManager.GetDestination("SAPMES");
        RfcRepository repository = dest.Repository;
        return dest;
    }
    public class SAP_FRC : IDestinationConfiguration
    {
        public RfcConfigParameters GetParameters(string destinationName)
        {
            if (destinationName.Equals("SAPMES"))
            {
                RfcConfigParameters rfcParams = new RfcConfigParameters();
                rfcParams.Add(RfcConfigParameters.Name, destinationName); //APPLE
                rfcParams.Add(RfcConfigParameters.AppServerHost, "172.20.1.176");   //SAP主機IP
                rfcParams.Add(RfcConfigParameters.SystemNumber, "05");              //SAP實例
                rfcParams.Add(RfcConfigParameters.Client, "168");                   // Client
                rfcParams.Add(RfcConfigParameters.User, "MES.ACL");                     //用戶名
                rfcParams.Add(RfcConfigParameters.Password, "MESMES");              //密碼
                rfcParams.Add(RfcConfigParameters.Language, "zf");                  //登陆語言
                //rfcParams.Add(RfcConfigParameters.PoolSize, "5");
                //rfcParams.Add(RfcConfigParameters.MaxPoolSize, "10");
                rfcParams.Add(RfcConfigParameters.ConnectionIdleTimeout, "1800");
                return rfcParams;

            }
            else
            {
                return null;
            }

        }

        public bool ChangeEventsSupported()
        {

            return false;

        }
        public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged
        {
            add
            {
                //configurationChanged = value;
            }
            remove
            {
                //do nothing
            }
        }
    }
    #endregion 

    #region RFC接口 料號庫存查詢 add by Apple at 20190215
    private DataTable ZGBSN(DataTable dt, RfcDestination dest)
    {
        DataSet oDs = new DataSet();
        DataTable distinctValues = new DataTable();
        try
        {

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
            IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
            string[] sArrayLGORT = strLGORT.Split(',');

            DataView view = new DataView(dt);
            distinctValues = view.ToTable(true, "MATNR");

            foreach (string SubstrLGORT in sArrayLGORT)
            {
                foreach (DataRow od in distinctValues.Rows)
                {
                    string strMaterial = od["MATNR"].ToString().TrimStart('0');
                    IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
                    MARD.SetValue("MATNR", strMaterial);
                    MARD.SetValue("WERKS", strWERKS);
                    MARD.SetValue("LGORT", SubstrLGORT);
                    ZMARD_INPUT.Insert(MARD);
                }

            }

            rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
            rfc.Invoke(dest);

            IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
            DataTable dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");
            oDs.Merge(dtZMARD_OUTPUT);

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }

        dt = InventoryIntegrationMissingincoming(dt, oDs);

        return dt;

    }
    #endregion

    #region SAP TABLE拆解 add by Apple at 20190215
    public DataTable ConvertToTable(IRfcTable rfcTable, string tableName)
    {
        DataTable dt = new DataTable(tableName);
        //CreateTable
        for (int i = 0; i < rfcTable.ElementCount; i++)
        {
            RfcElementMetadata rfcElementMetadata = rfcTable.GetElementMetadata(i);
            dt.Columns.Add(rfcElementMetadata.Name);
        }

        foreach (IRfcStructure rfcStructure in rfcTable)
        {
            DataRow dr = dt.NewRow();
            for (int z = 0; z < rfcTable.ElementCount; z++)
            {
                RfcElementMetadata metadata = rfcTable.GetElementMetadata(z);
                switch (metadata.DataType)
                {
                    case RfcDataType.BCD:
                        try
                        {
                            dr[z] = rfcStructure.GetInt(metadata.Name);
                        }
                        catch
                        {
                            //CANNOT CONVERT BCD[8:2] INTO INT32 解决:转DOUBLE add by dick 20170421
                            dr[z] = rfcStructure.GetDouble(metadata.Name);
                        }
                        break;

                    default:
                        dr[z] = rfcStructure.GetString(metadata.Name);
                        break;
                }
            }
            dt.Rows.Add(dr);
            dt.AcceptChanges();
        }

        return dt;
    }
    #endregion

    #region 整合庫存資料 add by Apple at 20190215
    private DataTable InventoryIntegrationMissingincoming(DataTable dt, DataSet oDs)
    {
        var linqStament = from p in oDs.Tables["ZMARD_OUTPUT"].AsEnumerable()
                          group p by new
                          {
                              MATNR = p.Field<string>("MATNR")
                          } into g
                          //let row = g.First()
                          select new
                          {
                              MATNR = g.Key.MATNR
                              ,
                              //Total = g.Sum(x => Convert.double(x["LABST"]) + Convert.ToInt32(x["INSME"]))
                              LABST = g.Sum(x => Convert.ToDouble(x["LABST"]))
                              ,
                              INSME = g.Sum(x => Convert.ToDouble(x["INSME"]))
                              ,
                              LGPBE = string.Join(",", g.Select(x => Convert.ToString(x["LGPBE"])))
                          };

        // dt.Columns.Add("Total");
     //   dt.Columns.Add("BAUGR"); 
       // dt.Columns.Add("BAUGR", typeof(string), ViewState["BAUGR"].ToString());//需求溯源
        DataColumn column1 = new DataColumn("BAUGR");
        column1.DataType = System.Type.GetType("System.String");
      //  column1.AllowDBNull = false;
        column1.DefaultValue = ViewState["BAUGR"].ToString();
        dt.Columns.Add(column1);
        dt.Columns.Add("LABST"); //庫存
        dt.Columns.Add("INSME"); //待驗
        dt.Columns.Add("LGPBE"); //儲位
        dt.Columns.Add("MEMO"); //備註
        foreach (var query in linqStament)
        {
            foreach (DataRow od in dt.Rows)
            {
                if (od["MATNR"].ToString().TrimStart('0') == query.MATNR.ToString().TrimStart('0'))
                {
                    // od["Total"] = query.Total;
                    od["LABST"] = query.LABST;
                    od["INSME"] = query.INSME;
                    od["LGPBE"] = query.LGPBE;
                    if (od["BWART"].ToString()=="531")
                    {
                        od["BDMNG"] = float.Parse(od["BDMNG"].ToString())*-1;
                        od["OPEN_QTY"] = float.Parse(od["OPEN_QTY"].ToString()) * -1;
                    }

                    if (float.Parse(od["OPEN_QTY"].ToString()) >0 )
                    {
                        if (float.Parse(od["LABST"].ToString())- float.Parse(od["OPEN_QTY"].ToString()) < 0)
                        {
                            od["MEMO"] = "庫存不足";
                        }
                        else
                        {
                            od["MEMO"] = "請發料";
                        }
                    }
                }
            }
        }
        dt.TableName = "Integration";
        return dt;
    }

    #endregion


    #region RFC接口 料號庫存查詢 add by Apple at 20190215
    private DataTable ZGBdSN(DataTable dt, RfcDestination dest)
    {
        DataSet oDs = new DataSet();
        DataTable distinctValues = new DataTable();
        try
        {

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
            IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
            string[] sArrayLGORT = strLGORT.Split(',');

            DataView view = new DataView(dt);
            distinctValues = view.ToTable(true, "物料");

            foreach (string SubstrLGORT in sArrayLGORT)
            {
                foreach (DataRow od in distinctValues.Rows)
                {
                    string strMaterial = od["物料"].ToString();
                    IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
                    MARD.SetValue("MATNR", strMaterial);
                    MARD.SetValue("WERKS", strWERKS);
                    MARD.SetValue("LGORT", SubstrLGORT);
                    ZMARD_INPUT.Insert(MARD);
                }

            }

            rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
            rfc.Invoke(dest);

            IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
            DataTable dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");
            oDs.Merge(dtZMARD_OUTPUT);

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }

        dt = InventoryIntegrationMissingincoming(dt, oDs);

        return dt;

    }
    #endregion

    public DataSet ZGET_SAP_SODNWO_DATA_CK(RfcDestination dest, string WONO, string SDATE, string EDATE, string PLANT)
    {

        RfcRepository repository = dest.Repository;
        IRfcFunction rfc = repository.CreateFunction("ZGET_SAP_SODNWO_DATA_CK");
        // string WONO = ""; string SDATE = DateTime.Now.AddDays(-7).ToString("yyyyMMdd"); string EDATE = DateTime.Now.AddDays(7).ToString("yyyyMMdd"); string PLANT = "TWM9"; 
        string SPFLG = "";
        rfc.SetValue("WONO", WONO);
        rfc.SetValue("SDATE", SDATE);
        rfc.SetValue("EDATE", EDATE);
        rfc.SetValue("SPFLG", SPFLG);
        rfc.SetValue("PLANT", PLANT);
        rfc.Invoke(dest);
        DataSet ds = new DataSet();
        ds.Tables.Add(CreateDataTable(rfc.GetTable("ZWOMASTER")));  //獲取相應的業務內表, 母表單
        ds.Tables.Add(CreateDataTable(rfc.GetTable("ZWODETAIL")));  //獲取相應的業務內表, 母表單
        return ds;
    }

    public DataSet ZGET_SAP_SODNWO_DATA(RfcDestination dest, string WONO, string SONO, string DNNO, string PONO)
    {

        RfcRepository repository = dest.Repository;
        IRfcFunction rfc = repository.CreateFunction("ZGET_SAP_SODNWO_DATA");
        // string WONO = ""; string SDATE = DateTime.Now.AddDays(-7).ToString("yyyyMMdd"); string EDATE = DateTime.Now.AddDays(7).ToString("yyyyMMdd"); string PLANT = "TWM9"; 
        string SPFLG = "";
        rfc.SetValue("WONO", WONO);
        rfc.SetValue("SONO", SONO);
        rfc.SetValue("DNNO", DNNO);
        rfc.SetValue("PONO", PONO);
        // rfc.SetValue("PLANT", PLANT);
        rfc.Invoke(dest);
        DataSet ds = new DataSet();
        ds.Tables.Add(CreateDataTable(rfc.GetTable("ZWOMASTER")));  //獲取相應的業務內表, 母表單
        ds.Tables.Add(CreateDataTable(rfc.GetTable("ZWODETAIL")));  //獲取相應的業務內表, 母表單
        return ds;
    }
    public static DataTable CreateDataTable(IRfcTable rfcTable)
    {
        var dataTable = new DataTable();

        for (int element = 0; element < rfcTable.ElementCount; element++)
        {
            RfcElementMetadata metadata = rfcTable.GetElementMetadata(element);
            dataTable.Columns.Add(metadata.Name);
        }

        foreach (IRfcStructure row in rfcTable)
        {
            DataRow newRow = dataTable.NewRow();
            for (int element = 0; element < rfcTable.ElementCount; element++)
            {
                RfcElementMetadata metadata = rfcTable.GetElementMetadata(element);
                newRow[metadata.Name] = row.GetString(metadata.Name);
            }
            dataTable.Rows.Add(newRow);
        }
        return dataTable;
    }

    protected void btn_search_Click(object sender, EventArgs e)
    {
        if (!txt_WIP_NO.Text.ToString().Trim().Equals(""))
        {

            capData();
        }
        
    }
}