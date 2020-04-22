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
using System.Text;

public partial class SAPSTOCKS : System.Web.UI.Page
{
    ATMCdb ATMCdb = new ATMCdb();
    ModuleFunction ModuleFunction = new ModuleFunction();
    double ReqQry = 0;
    double FeedQry = 0;
    string strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158", strWERKS = "TWM3";
    int modifyflag = 0;
    DataTable dtSAPSTOCKS = new DataTable();
    SAPRFC SAPRFC = new SAPRFC();

    string sAUTHORITY = "";
    string sPREPARATION_FLAG = "";
    int MD04DAYS = 60;
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["URL"] = System.IO.Path.GetFileName(Request.PhysicalPath) + Request.Url.Query;
        if (Session["PREPARATION_FLAG"] != null)
        {
            sPREPARATION_FLAG = Session["PREPARATION_FLAG"].ToString();
            sAUTHORITY = Session["AUTHORITY"].ToString();
        }
        //     txt_MATNR.Attributes.Add("onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) { __doPostBack('MainContent$btn_search', ''); return false; }} else {return true}; ");
        if (!Page.IsPostBack)
        {

            string sScript = null;
            string sScriptl = null;
            string surl = null;
            string sur2 = null;
            //   日期輸入的頁面，將 TextBox 以 TextBoxId 網址參數傳給日期頁面
            surl = "calendar.aspx?TextBoxID=" + txtStartDate.ClientID;
            sScript = "window.open('" + surl + "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')";
            txtStartDate.Attributes["onclick"] = sScript;
            sur2 = "calendar.aspx?TextBoxID=" + txtEndDate.ClientID;
            sScriptl = "window.open('" + sur2 + "','','height=250,width=250,status=no,toolbar=no,menubar=no,location=no','')";
            txtEndDate.Attributes["onclick"] = sScriptl;
            txtEndDate.Text = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
            //txtStartDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            txtStartDate.Text = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd");
        }
    }

    #region 呼叫SAP RFC 已移植至SAPRFC.cs
    #region SAP連線 add by Apple at 20190215
    //private RfcDestination getSAPDB()
    //{
    //    IDestinationConfiguration ID = new SAP_FRC();
    //    try
    //    { RfcDestinationManager.UnregisterDestinationConfiguration(ID); RfcDestinationManager.RegisterDestinationConfiguration(ID); }
    //    catch
    //    { }
    //    RfcDestination dest = RfcDestinationManager.GetDestination("SAPMES");
    //    RfcRepository repository = dest.Repository;
    //    return dest;
    //}
    //public class SAP_FRC : IDestinationConfiguration
    //{
    //    public RfcConfigParameters GetParameters(string destinationName)
    //    {
    //        if (destinationName.Equals("SAPMES"))
    //        {
    //            RfcConfigParameters rfcParams = new RfcConfigParameters();
    //            rfcParams.Add(RfcConfigParameters.Name, destinationName); //APPLE
    //            rfcParams.Add(RfcConfigParameters.AppServerHost, "172.20.1.176");   //SAP主機IP
    //            rfcParams.Add(RfcConfigParameters.SystemNumber, "05");              //SAP實例
    //            rfcParams.Add(RfcConfigParameters.Client, "168");                   // Client
    //            rfcParams.Add(RfcConfigParameters.User, "MES.ACL");                     //用戶名
    //            rfcParams.Add(RfcConfigParameters.Password, "MESMES");              //密碼
    //            rfcParams.Add(RfcConfigParameters.Language, "zf");                  //登陆語言
    //            //rfcParams.Add(RfcConfigParameters.PoolSize, "5");
    //            //rfcParams.Add(RfcConfigParameters.MaxPoolSize, "10");
    //            rfcParams.Add(RfcConfigParameters.ConnectionIdleTimeout, "1800");
    //            return rfcParams;

    //        }
    //        else
    //        {
    //            return null;
    //        }

    //    }

    //    public bool ChangeEventsSupported()
    //    {

    //        return false;

    //    }
    //    public event RfcDestinationManager.ConfigurationChangeHandler ConfigurationChanged
    //    {
    //        add
    //        {
    //            //configurationChanged = value;
    //        }
    //        remove
    //        {
    //            //do nothing
    //        }
    //    }
    //}
    #endregion
    #region SAP TABLE拆解 add by Apple at 20190215
    //public DataTable ConvertToTable(IRfcTable rfcTable, string tableName)
    //{
    //    DataTable dt = new DataTable(tableName);
    //    //CreateTable
    //    for (int i = 0; i < rfcTable.ElementCount; i++)
    //    {
    //        RfcElementMetadata rfcElementMetadata = rfcTable.GetElementMetadata(i);
    //        dt.Columns.Add(rfcElementMetadata.Name);
    //    }

    //    foreach (IRfcStructure rfcStructure in rfcTable)
    //    {
    //        DataRow dr = dt.NewRow();
    //        for (int z = 0; z < rfcTable.ElementCount; z++)
    //        {
    //            RfcElementMetadata metadata = rfcTable.GetElementMetadata(z);
    //            switch (metadata.DataType)
    //            {
    //                case RfcDataType.BCD:
    //                    try
    //                    {
    //                        dr[z] = rfcStructure.GetInt(metadata.Name);
    //                    }
    //                    catch
    //                    {
    //                        //CANNOT CONVERT BCD[8:2] INTO INT32 解决:转DOUBLE add by dick 20170421
    //                        dr[z] = rfcStructure.GetDouble(metadata.Name);
    //                    }
    //                    break;

    //                default:
    //                    dr[z] = rfcStructure.GetString(metadata.Name);
    //                    break;
    //            }
    //        }
    //        dt.Rows.Add(dr);
    //        dt.AcceptChanges();
    //    }

    //    return dt;
    //}
    #endregion
    //private DataTable ZGBSN(string MATNR, string strWERKS, RfcDestination dest)
    //{
    //    gdv_GOODSMVT_GETITEMS.DataSource = "";
    //    gdv_SAPSTOCKS_MD04.DataSource = "";
    //    string ED_DATE = "", CONFIRM_DATE = "";
    //    double PRETOTAL_STOCK = 0;
    //    double[] ENTRY_QNT = new double[2] { 0, 0 };
    //    double ENTRY_QNT_TODAY = 0;
    //    string VENDORNAME = "";
    //    string MCNAME = "";
    //    double STD_COST = 0;
    //    //   DataSet oDs = new DataSet();
    //    DataTable dtZMARD_OUTPUT = new DataTable();
    //    //  DataTable distinctValues = new DataTable();
    //    try
    //    {

    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
    //        IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
    //        string[] sArrayLGORT = strLGORT.Split(',');

    //        //  DataView view = new DataView(dt);
    //        //  distinctValues = view.ToTable(true, "MATNR");

    //        foreach (string SubstrLGORT in sArrayLGORT)
    //        {
    //            //foreach (DataRow od in distinctValues.Rows)
    //            //{
    //            string strMaterial = MATNR.ToString().TrimStart('0');
    //            IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
    //            MARD.SetValue("MATNR", strMaterial);
    //            MARD.SetValue("WERKS", strWERKS);
    //            MARD.SetValue("LGORT", SubstrLGORT);
    //            ZMARD_INPUT.Insert(MARD);
    //            //}

    //        }

    //        rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
    //        rfc.Invoke(dest);

    //        IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
    //        dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");

    //        if (dtZMARD_OUTPUT.Rows.Count > 0)
    //        {
    //            if (!txtMD04DAYS.Text.ToString().Equals(""))
    //            {
    //                MD04DAYS = int.Parse(txtMD04DAYS.Text.ToString());
    //            }
    //            ReturnMD04(MD04DAYS, MATNR, strWERKS, dest);
    //            ED_DATE = WH_INVENTORY(MATNR, strWERKS);
    //            CONFIRM_DATE = WH_BININVENTORY(MATNR, strWERKS);
    //            ENTRY_QNT = ZGWH_STORAGE(MATNR, strWERKS);
    //            ENTRY_QNT_TODAY = ZGWH_STORAGE_TODAY(MATNR, strWERKS);
    //            VENDORNAME = SAPRFC.ZMM_SOURCE_BY_MATERIAL(MATNR, strWERKS, dest);
    //            MCNAME = SAPRFC.ZMM_STOCK_REQUIREMENTS(MATNR, strWERKS, dest);
    //            PRETOTAL_STOCK = ZGSKIN_BIN_CARD(MATNR, strWERKS);
    //            STD_COST = SAPRFC.Z_SD_SEARCH_COST_PRICE(MATNR, strWERKS, dest);
    //            long n = 0;
    //            string sMATNR = MATNR;
    //            if (long.TryParse(MATNR, out n))
    //            {
    //                sMATNR = MATNR.PadLeft(18, '0');
    //            }

    //            string[] material_ra = { sMATNR };
    //            string[] plant_ra = { "TWM9" };
    //            string StartDate = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd");
    //            string txtEndDate = DateTime.Now.ToString("yyyy-MM-dd");
    //            string[] PSTNG_DATE_RA = { StartDate , txtEndDate };
    //            //string[] sloc_ra = { "" };
    //            // string[] move_type_ra = txt_MOVEMENT_TYPE.Text.Split(',');
    //            // string[] tr_ev_type_ra = { "WE", "WQ" };
    //            string[] pstng_date_ra = { StartDate, txtEndDate };
    //            DataSet Ds = SAPRFC.sBAPI_GOODSMVT_GETITEMS(material_ra, plant_ra, pstng_date_ra, getSAPDB());
    //            //  Ds.Relations.Add(new DataRelation("MAT_DOC", Ds.Tables["GOODSMVT_HEADER"].Columns["MAT_DOC"], Ds.Tables["GOODSMVT_ITEMS"].Columns["MAT_DOC"]));

    //            Ds.Tables[0].DefaultView.Sort = "ENTRY_DATE DESC,ENTRY_TIME DESC";

    //            gdv_GOODSMVT_GETITEMS.DataSource = Ds.Tables[0];
    //            gdv_GOODSMVT_GETITEMS.DataBind();
    //        }
    //        else
    //        {
    //            Response.Write("<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //        }
    //        //  oDs.Merge(dtZMARD_OUTPUT);

    //    }
    //    catch (RfcAbapException ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //    //catch (Exception ex1)
    //    //{
    //    //    throw new Exception(ex1.Message);
    //    //}


    //    DataColumn column1 = new DataColumn("ENTRY_QNT");
    //    column1.DataType = System.Type.GetType("System.Decimal");
    //    column1.DefaultValue = ENTRY_QNT[0];
    //    dtZMARD_OUTPUT.Columns.Add(column1);

    //    DataColumn column4 = new DataColumn("UNPULL_QNT");
    //    column4.DataType = System.Type.GetType("System.Decimal");
    //    column4.DefaultValue = ENTRY_QNT[1];
    //    dtZMARD_OUTPUT.Columns.Add(column4);

    //    DataColumn column2 = new DataColumn("ReqQry");
    //    column2.DataType = System.Type.GetType("System.Decimal");
    //    column2.DefaultValue = ReqQry;
    //    dtZMARD_OUTPUT.Columns.Add(column2);

    //    DataColumn column3 = new DataColumn("FeedQry");
    //    column3.DataType = System.Type.GetType("System.Decimal");
    //    column3.DefaultValue = FeedQry;
    //    dtZMARD_OUTPUT.Columns.Add(column3);

    //    DataColumn column5 = new DataColumn("ED_DATE");
    //    column5.DataType = System.Type.GetType("System.String");
    //    column5.DefaultValue = ED_DATE;
    //    dtZMARD_OUTPUT.Columns.Add(column5);

    //    DataColumn column6 = new DataColumn("VENDOR");
    //    column6.DataType = System.Type.GetType("System.String");
    //    column6.DefaultValue = VENDORNAME;
    //    dtZMARD_OUTPUT.Columns.Add(column6);

    //    DataColumn column7 = new DataColumn("MCNAME");
    //    column7.DataType = System.Type.GetType("System.String");
    //    column7.DefaultValue = MCNAME;
    //    dtZMARD_OUTPUT.Columns.Add(column7);

    //    DataColumn column8 = new DataColumn("CONFIRM_DATE");
    //    column8.DataType = System.Type.GetType("System.String");
    //    column8.DefaultValue = CONFIRM_DATE;
    //    dtZMARD_OUTPUT.Columns.Add(column8);
    //    DataColumn column9 = new DataColumn("PRETOTAL_STOCK");
    //    column9.DataType = System.Type.GetType("System.Decimal");
    //    column9.DefaultValue = PRETOTAL_STOCK;
    //    dtZMARD_OUTPUT.Columns.Add(column9);


    //    DataColumn column10 = new DataColumn("ENTRY_QNT_TODAY");
    //    column10.DataType = System.Type.GetType("System.Decimal");
    //    column10.DefaultValue = ENTRY_QNT_TODAY;
    //    dtZMARD_OUTPUT.Columns.Add(column10);

    //    DataColumn column11 = new DataColumn("STD_COST");
    //    column11.DataType = System.Type.GetType("System.Decimal");
    //    column11.DefaultValue = STD_COST;
    //    dtZMARD_OUTPUT.Columns.Add(column11);

    //    return dtZMARD_OUTPUT;



    //}

    #region 抓出物料單價
    //private double Z_SD_SEARCH_COST_PRICE(string MATNR, string strWERKS, RfcDestination dest)
    //{
    //    DataTable dtLE_ZSD_COST = new DataTable();
    //    double STD_COST = 0;
    //    try
    //    {
    //        long n = 0;
    //        if (long.TryParse(MATNR.Trim().ToUpper(), out n))
    //        {
    //            MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
    //        }
    //        else
    //        {
    //            MATNR = MATNR.Trim().ToUpper();
    //        }

    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("Z_SD_SEARCH_COST_PRICE");
    //        rfc.SetValue("WERKS", strWERKS);
    //        rfc.SetValue("MATNR", MATNR);
    //        rfc.Invoke(dest);
    //        IRfcTable LE_ZSD_COST = rfc.GetTable("LE_ZSD_COST");
    //        dtLE_ZSD_COST = ConvertToTable(LE_ZSD_COST, "LE_ZSD_COST");
    //        if (dtLE_ZSD_COST.Rows[0]["PE_STPRS"] != DBNull.Value) STD_COST = double.Parse(dtLE_ZSD_COST.Rows[0]["PE_STPRS"].ToString());
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //    return STD_COST;




    //}
    #endregion
    #region 抓出物料MC管理人員
    //private string ZMM_STOCK_REQUIREMENTS(string MATNR, string strWERKS, RfcDestination dest)
    //{
    //    string MCNAME = "";
    //    try
    //    {

    //        long n = 0;
    //        if (long.TryParse(MATNR.Trim().ToUpper(), out n))
    //        {
    //            MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
    //        }
    //        else
    //        {
    //            MATNR = MATNR.Trim().ToUpper();
    //        }

    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("ZMM_STOCK_REQUIREMENTS");
    //        rfc.SetValue("WERKS", strWERKS);
    //        rfc.SetValue("MATNR", MATNR);
    //        rfc.Invoke(dest);
    //        MCNAME = rfc.GetValue("E_DSNAM").ToString();


    //    }
    //    catch (RfcAbapException ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }

    //    catch (RfcAbapMessageException ex)
    //    {
    //        // throw new Exception(ex.Message);
    //        Response.Write(" <script   language=javascript> window.alert( '" + ex.Message + "') </script> ");
    //    }
    //    //catch (Exception ex1)
    //    //{
    //    //    throw new Exception(ex1.Message);
    //    //}

    //    return MCNAME;
    //}
    #endregion
    //private string ZMM_SOURCE_BY_MATERIAL(string MATNR, string strWERKS, RfcDestination dest)
    //{
    //    string VENDORNAME = "";

    //    try
    //    {

    //        long n = 0;
    //        if (long.TryParse(MATNR.Trim().ToUpper(), out n))
    //        {
    //            MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
    //        }
    //        else
    //        {
    //            MATNR = MATNR.Trim().ToUpper();
    //        }

    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("ZMM_SOURCE_BY_MATERIAL");
    //        rfc.SetValue("I_WERKS", strWERKS);
    //        IRfcTable ZMARD_INPUT = rfc.GetTable("T_MATNR");
    //        // string strMaterial = MATNR.ToString().TrimStart('0');
    //        IRfcStructure MARD = repository.GetStructureMetadata("ZMATNR").CreateStructure();
    //        MARD.SetValue("MATNR", MATNR);
    //        ZMARD_INPUT.Insert(MARD);
    //        //}

    //        DataTable dtZMARD_OUTPUT = new DataTable();

    //        rfc.SetValue("T_MATNR", ZMARD_INPUT);//寫入資料

    //        rfc.Invoke(dest);

    //        IRfcTable ZMARD_OUTPUT = rfc.GetTable("T_MAT_SOURCE");
    //        dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "T_MAT_SOURCE");
    //        //  oDs.Merge(dtZMARD_OUTPUT);
    //        if (dtZMARD_OUTPUT.Rows.Count > 0)
    //        {
    //            VENDORNAME = dtZMARD_OUTPUT.Rows[0]["NAME1"].ToString();

    //            // GridView1.DataSource = dtZMARD_OUTPUT;
    //            //GridView1.DataBind();
    //        }

    //    }
    //    catch (RfcAbapException ex)
    //    {
    //        // throw new Exception(ex.Message);
    //    }
    //    //catch (Exception ex1)
    //    //{
    //    //    throw new Exception(ex1.Message);
    //    //}

    //    return VENDORNAME;

    //}

    #region MB51功能
    protected void gdv_GOODSMVT_GETITEMS_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string QA = e.Row.Cells[22].Text.ToString().Trim();
            // if (QA == "201" || QA == "261" || QA == "303" || QA == "306" || QA == "309" || QA == "311" || QA == "321" || QA == "351" || QA == "344" || QA == "311") { e.Row.Cells[7].BackColor = System.Drawing.Color.Red; }
            if (QA.Equals("-"))
            {
                e.Row.Cells[10].BackColor = System.Drawing.Color.Red;
            }

        }
    }
    //void ParameterRange(ref IRfcTable tb, string Low, string High)
    //{
    //    tb.Append(1);
    //    tb[0].SetValue("SIGN", "I");
    //    tb[0].SetValue("OPTION", "BT");
    //    tb[0].SetValue("LOW", DateTime.Parse(Low));
    //    tb[0].SetValue("HIGH", DateTime.Parse(High));
    //}
    //void ParameterRange(ref IRfcTable tb, string[] paramArray)
    //{
    //    int len = paramArray.Length;
    //    if (len > 0)
    //    {
    //        tb.Append(len);
    //        for (int i = 0; i < len; i++)
    //        {
    //            if (tb.Metadata.LineType.Name == "BAPI2017_GM_MATERIAL_RA")
    //            {
    //                int result;
    //                if (int.TryParse(paramArray[i], out result))
    //                {
    //                    paramArray[i] = paramArray[i].PadLeft(18, '0');
    //                }
    //            }
    //            tb[i].SetValue("LOW", paramArray[i]);
    //        }
    //    }
    //}
    ///// <summary>
    ///// 交易紀錄
    ///// </summary>
    ///// <param name="_MATERIAL_RA"></param>
    ///// <param name="_PLANT_RA"></param>
    ///// <param name="_STGE_LOC_RA"></param>
    ///// <param name="_MOVE_TYPE_RA"></param>
    ///// <param name="_TR_EV_TYPE_RA"></param>
    ///// <param name="_PSTNG_DATE_RA"></param>
    ///// <returns></returns>
    //public DataSet sBAPI_GOODSMVT_GETITEMS(string[] _MATERIAL_RA, string[] _PLANT_RA, RfcDestination dest)
    //{
    //    try
    //    {
    //        RfcRepository repository = dest.Repository;
    //        IRfcFunction rfc = repository.CreateFunction("BAPI_GOODSMVT_GETITEMS");
    //        IRfcTable MATERIAL_RA = rfc.GetTable("MATERIAL_RA");
    //        ParameterRange(ref MATERIAL_RA, _MATERIAL_RA);
    //        IRfcTable PLANT_RA = rfc.GetTable("PLANT_RA");
    //        ParameterRange(ref PLANT_RA, _PLANT_RA);
    //        //  IRfcTable STGE_LOC_RA = rfc.GetTable("STGE_LOC_RA");
    //        // ParameterRange(ref STGE_LOC_RA, _STGE_LOC_RA);
    //        //  IRfcTable MOVE_TYPE_RA = rfc.GetTable("MOVE_TYPE_RA");
    //        // ParameterRange(ref MOVE_TYPE_RA, _MOVE_TYPE_RA);
    //        // IRfcTable TR_EV_TYPE_RA = rfc.GetTable("TR_EV_TYPE_RA");
    //        //  ParameterRange(ref TR_EV_TYPE_RA, _TR_EV_TYPE_RA);
    //        IRfcTable PSTNG_DATE_RA = rfc.GetTable("PSTNG_DATE_RA");
    //        ParameterRange(ref PSTNG_DATE_RA, txtStartDate.Text, txtEndDate.Text);
    //        rfc.Invoke(dest);

    //        IRfcTable GOODSMVT_HEADER = rfc.GetTable("GOODSMVT_HEADER");
    //        IRfcTable GOODSMVT_ITEMS = rfc.GetTable("GOODSMVT_ITEMS");
    //        IRfcTable RETURN = rfc.GetTable("RETURN");

    //        DataSet ds = BAPI_GOODSMVT_GETITEMS_RESULT(GOODSMVT_HEADER, GOODSMVT_ITEMS, RETURN);
    //        ds.Tables[0].Columns.Add("MoveNAME");
    //        ds.Tables[0].Columns.Add("STATUS");
    //        ds.Tables[0].Columns["MoveNAME"].SetOrdinal(7);
    //        ds.Tables[0].Columns["STATUS"].SetOrdinal(16);
    //        ds.Tables[0].Columns.Add("MoveChange");
    //        DataTable dt = ATMCdb.reDt("SELECT  [MovementType],[NAME],[MoveChange] FROM [ATMC].[M9WH].[MOVEMENT_TYPE]");
    //        foreach (DataRow dr in ds.Tables[0].Rows)
    //        {
    //            string MovementType = dr["MOVE_TYPE"].ToString();
    //            string ORDERID = dr["ORDERID"].ToString();
    //            string STCK_TYPE = dr["STCK_TYPE"].ToString();
    //            dr["STATUS"] = ATMCdb.scalDs("SELECT [STATUS] FROM [ATMC].[dbo].[SKIN_WO_STATUS] WHERE [AUFNR]='" + ORDERID + "'");
    //            foreach (DataRow dr2 in dt.Rows)
    //            {

    //                if (MovementType == dr2["MovementType"].ToString())
    //                {
    //                    if (MovementType.Equals("101") && STCK_TYPE.Equals("X"))
    //                    {
    //                        dr["MoveNAME"] = "訂單待驗";
    //                    }
    //                    else
    //                    {
    //                        dr["MoveNAME"] = dr2["NAME"].ToString();
    //                    }

    //                    dr["MoveChange"] = dr2["MoveChange"].ToString();
    //                    break;
    //                }
    //            }
    //        }


    //        return ds;

    //    }
    //    catch (RfcAbapException ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //    catch (Exception ex1)
    //    {
    //        throw new Exception(ex1.Message);
    //    }

    //}


    //#region Fileds
    //public const string sX_AUTO_CRE = "X_AUTO_CRE";
    //public const string sX = "X";
    //public const string sMAT_DOC = "MAT_DOC";
    //public const string sMATDOC_ITM = "MATDOC_ITM";
    //public const string sMATERIAL = "MATERIAL";
    //public const string sPLANT = "PLANT";
    //public const string sSTGE_LOC = "STGE_LOC";
    //public const string sENTRY_QNT = "ENTRY_QNT";
    //public const string sVENDOR = "VENDOR";
    //public const string sUSERNAME = "USERNAME";
    //public const string sMOVE_TYPE = "MOVE_TYPE";
    //public const string sSTCK_TYPE = "STCK_TYPE";
    //public const string sPO_NUMBER = "PO_NUMBER";
    //public const string sPO_ITEM = "PO_ITEM";
    //public const string sREF_DOC = "REF_DOC";
    //public const string sORDERID = "ORDERID";
    //public const string sHEADER_TXT = "HEADER_TXT";
    //public const string sENTRY_DATE = "ENTRY_DATE";
    //public const string sENTRY_TIME = "ENTRY_TIME";


    //public const string Mvt101 = "101";
    //public const string Mvt102 = "102";


    //string[] GOODSMVT_ITEMS_FILEDS = {
    //            sMAT_DOC,
    //            sMATDOC_ITM,

    //            sMATERIAL,
    //            sPLANT,
    //            sSTGE_LOC,
    //            sMOVE_TYPE,
    //            sSTCK_TYPE,
    //            "MOVE_PLANT",
    //            "MOVE_STLOC",
    //            sENTRY_QNT,
    //            sVENDOR,
    //            sPO_NUMBER,
    //            sPO_ITEM,
    //            sREF_DOC,
    //            sORDERID,
    //        };
    //string[] GOODSMVT_HEADER_FILEDS = {
    //            sMAT_DOC,
    //            //"TR_EV_TYPE",
    //            sENTRY_DATE,
    //            sENTRY_TIME,
    //            sUSERNAME,
    //            "REF_DOC_NO_LONG",
    //            //"REF_DOC_NO",
    //            sHEADER_TXT,
    //        };
    //#endregion
    //DataSet BAPI_GOODSMVT_GETITEMS_RESULT(IRfcTable GOODSMVT_HEADER, IRfcTable GOODSMVT_ITEMS, IRfcTable RETURN)
    //{

    //    DataTable dt = new DataTable();

    //    #region 加入所有欄位名稱
    //    foreach (string item in GOODSMVT_ITEMS_FILEDS)
    //    {
    //        dt.Columns.Add(item);
    //    }
    //    foreach (string item in GOODSMVT_HEADER_FILEDS)
    //    {
    //        if (item == sMAT_DOC) continue;
    //        dt.Columns.Add(item);
    //    }
    //    #endregion

    //    foreach (IRfcStructure GOODSMVT_ITEMS_Structure in GOODSMVT_ITEMS)
    //    {

    //        #region 略過自動產生的帳務項目

    //        string X_auto = GOODSMVT_ITEMS_Structure.GetString(sX_AUTO_CRE).Trim();
    //        string X_STACK_TYPE = GOODSMVT_ITEMS_Structure.GetString("STCK_TYPE").Trim();
    //        //if (X_auto == sX || X_STACK_TYPE == sX)
    //        if (X_auto == sX)
    //        {
    //            continue;
    //        }

    //        #endregion

    //        DataRow row = dt.NewRow();

    //        ///取得列中每欄資料
    //        foreach (string item_filed in GOODSMVT_ITEMS_FILEDS)
    //        {

    //            string item = GOODSMVT_ITEMS_Structure.GetString(item_filed).Trim();

    //            #region 加入檔頭資訊

    //            if (item_filed == sMAT_DOC)
    //            {
    //                row[item_filed] = item;

    //                foreach (IRfcStructure GOODSMVT_HEADER_Structure in GOODSMVT_HEADER)
    //                {
    //                    string header = GOODSMVT_HEADER_Structure.GetString(sMAT_DOC).Trim();
    //                    if (header != item)
    //                    {
    //                        continue;
    //                    }

    //                    foreach (string header_filed in GOODSMVT_HEADER_FILEDS)
    //                    {
    //                        if (header_filed == sMAT_DOC) continue;
    //                        header = GOODSMVT_HEADER_Structure.GetString(header_filed).Trim();
    //                        row[header_filed] = header;
    //                    }
    //                }
    //                continue;
    //            }

    //            #endregion


    //            #region 料號&PO去除前頭0
    //            if (item_filed == sMATERIAL ||
    //                item_filed == sPO_NUMBER)
    //            {
    //                item = item.TrimStart('0');
    //            }
    //            #endregion
    //            #region 數量去除小數點
    //            if (item_filed == sENTRY_QNT)
    //            {
    //                item = item.Split('.')[0];
    //            }
    //            #endregion

    //            row[item_filed] = item;
    //        }
    //        dt.Rows.Add(row);

    //    }


    //    #region 處理102迴轉

    //    //bool check;

    //    //check = true;
    //    //while (check)
    //    //{
    //    //    check = false;
    //    //    foreach (DataRow row in dt.Rows)
    //    //    {
    //    //        if (row[sMOVE_TYPE].ToString() == Mvt102)
    //    //        {
    //    //            check = true;
    //    //            foreach (DataRow row_check in dt.Rows)
    //    //            {
    //    //                if (row_check[sREF_DOC].ToString() == row[sREF_DOC].ToString() && row_check[sMOVE_TYPE].ToString() == Mvt101)
    //    //                {
    //    //                    row_check.Delete();
    //    //                    break;
    //    //                }
    //    //            }
    //    //            row.Delete();
    //    //            break;
    //    //        }
    //    //    }
    //    //}

    //    #endregion

    //    DataTable dt1 = RfcTableToDataTable(RETURN);

    //    DataSet ds = new DataSet();
    //    ds.Tables.Add(dt);
    //    ds.Tables.Add(dt1);

    //    return ds;
    //}

    //public DataTable RfcTableToDataTable(IRfcTable rfcTable)
    //{
    //    DataTable tb = new DataTable();
    //    try
    //    {

    //        ///取得標題列
    //        RfcTableToDataTableColumns(rfcTable, ref tb);

    //        foreach (IRfcStructure Structure in rfcTable)
    //        {
    //            DataRow row = tb.NewRow();

    //            ///取得列中每欄資料
    //            RfcStructureToDataRow(Structure, ref row);

    //            tb.Rows.Add(row);
    //        }
    //    }
    //    catch
    //    {
    //    }
    //    return tb;
    //}

    //public void RfcTableToDataTableColumns(IRfcTable rfcTable, ref DataTable tb)
    //{
    //    try
    //    {
    //        for (int i = 0; i <= rfcTable.ElementCount - 1; i++)
    //        {
    //            RfcElementMetadata Metadata = rfcTable.GetElementMetadata(i);

    //            if (In<string>(Metadata.Name, GOODSMVT_ITEMS_FILEDS))
    //            {
    //                continue;
    //            }
    //            tb.Columns.Add(Metadata.Name);
    //        }
    //    }
    //    catch
    //    {
    //    }

    //}


    //public void RfcStructureToDataRow(IRfcStructure Structure, ref DataRow row)
    //{
    //    try
    //    {
    //        for (int i = 0; i <= Structure.ElementCount - 1; i++)
    //        {
    //            RfcElementMetadata Metadata = Structure.GetElementMetadata(i);
    //            if (In<string>(Metadata.Name, GOODSMVT_ITEMS_FILEDS))
    //            {
    //                continue;
    //            }

    //            string value = Structure.GetString(Metadata.Name).Trim();
    //            if (Metadata.Name == "MATERIAL")
    //            {
    //                value = value.TrimStart('0');
    //            }

    //            row[Metadata.Name] = value;
    //        }
    //    }
    //    catch
    //    {
    //    }
    //}

    //private bool In<T>(T obj, T[] args)
    //{
    //    return args.Contains(obj);
    //}

    #endregion

    #region SAP修改儲位
    //public string ZMM_MODIFY_STORAGE_BIN(string MATNR, string WERKS, string LGORT, string LGPBE, RfcDestination dest)
    //{
    //    string nresult = "";
    //    RfcRepository Repo = dest.Repository;
    //    //  RfcRepository repository = dest.Repository;
    //    //  IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
    //    IRfcFunction rfc = Repo.CreateFunction("ZMM_MODIFY_STORAGE_BIN");
    //    rfc.SetValue("MATNR", MATNR);
    //    rfc.SetValue("WERKS", WERKS);
    //    rfc.SetValue("LGORT", LGORT);
    //    rfc.SetValue("LGPBE", LGPBE);
    //    rfc.Invoke(dest);
    //    DataTable dt = new DataTable();
    //    IRfcTable ZMARD_OUTPUT = rfc.GetTable("OUT_PUT");
    //    dt = ConvertToTable(ZMARD_OUTPUT, "OUT_PUT");

    //    if (dt.Rows[0]["FLAG"].ToString() == "Y")
    //    {
    //        nresult = "OK";
    //        Response.Write("<embed src = 'right.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //    }
    //    else
    //    {
    //        nresult = "NG";

    //        Response.Write("<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
    //    }

    //    return nresult;
    //}
    #endregion
    #endregion


    /// <summary>
    /// CAP 待上架數量
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private double[] ZGWH_STORAGE(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        //double ENTRY_QNT = 0;
        double[] ENTRY_QNT = new double[2] { 0, 0 };
            string sParam = "SELECT ISNULL(SUM(CASE WHEN [CACHE_NAME] is not null AND [CACHE_NAME] <> '調整帳務' THEN [ENTRY_QNT] ELSE 0 END ),0),ISNULL(SUM(CASE WHEN [CACHE_NAME] is null THEN [ENTRY_QNT] ELSE 0 END ),0) FROM [TWM8].[dbo].[WH_STORAGE] WHERE  [STATUS]=0 AND [DISPLAY_FALG]='Y' AND [MATERIAL]='" + strMaterial + "' AND [PLANT]='"+strWERKS+"' ";
            DataTable dt = ATMCdb.reDt(sParam);

            if (dt.Rows.Count > 0)
            {
                ENTRY_QNT[0] = double.Parse(dt.Rows[0][0].ToString());
                ENTRY_QNT[1] = double.Parse(dt.Rows[0][1].ToString());
            }
        return ENTRY_QNT;
    }
    /// <summary>
    /// CAP 今日上架數量
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private double ZGWH_STORAGE_TODAY(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        //double ENTRY_QNT = 0;
        double ENTRY_QNT_TODAY = 0;
            string sParam = "SELECT ISNULL(SUM([ENTRY_QNT]),0) FROM [TWM8].[dbo].[WH_STORAGE] WHERE  [STATUS]=1 AND [DISPLAY_FALG]='Y' AND CONVERT(DATE,[ED_DATE])=CONVERT(DATE,GETDATE()) AND [MATERIAL]='" + strMaterial + "' AND [PLANT]='" + strWERKS + "' ";
            DataTable dt = ATMCdb.reDt(sParam);
            if (dt.Rows.Count > 0)
            {
                ENTRY_QNT_TODAY = double.Parse(dt.Rows[0][0].ToString());

            }
        return ENTRY_QNT_TODAY;
    }
    /// <summary>
    /// CAP 備品庫數量
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private double ZGSKIN_BIN_CARD(string MATNR, string strWERKS)
    {
        string strMaterial = MATNR.ToString().TrimStart('0');
        double TOTAL_STOCK = 0;
            string sParam = "SELECT ISNULL(SUM([IN_STOCK])-SUM([OUT_STOCK]),0) FROM [TWM8].[dbo].[SKIN_BIN_CARD] WHERE  [LGORT]='9999' AND [MATNR]='" + strMaterial + "' ";
            TOTAL_STOCK = double.Parse(ATMCdb.scalDs(sParam));
        return TOTAL_STOCK;
    }
    protected void btn_search_Click(object sender, EventArgs e)
    {
        if (txt_MATNR.Text.ToString().Trim().Equals("") )
        {
            Response.Write(" <script   language=javascript> window.alert( '請輸入料號再進行搜尋') </script> ");
        }
        else
        {
            capdata();
        }
    }
    /// <summary>
    /// 料號庫存查詢
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <returns></returns>
    private DataTable capZGBSN(string MATNR, string strWERKS)
    {
        gdv_GOODSMVT_GETITEMS.DataSource = "";
        gdv_SAPSTOCKS_MD04.DataSource = "";
        double PRETOTAL_STOCK = 0;
        double[] ENTRY_QNT = new double[2] { 0, 0 };
        double ENTRY_QNT_TODAY = 0;
        string VENDORNAME = "";
        string MCNAME = "";
        double STD_COST = 0;
        //   DataSet oDs = new DataSet();
        DataTable dtZMARD_OUTPUT = new DataTable();
        DataColumn mycolumn = new DataColumn("MATNR");
        mycolumn.DataType = System.Type.GetType("System.String");
        dtZMARD_OUTPUT.Columns.Add(mycolumn);
        dtZMARD_OUTPUT.Rows.Add(dtZMARD_OUTPUT.NewRow());
        dtZMARD_OUTPUT.Rows[0][0] = MATNR;
        try
        {
            dtZMARD_OUTPUT = SAPRFC.ZGBSN(dtZMARD_OUTPUT, strWERKS, strLGORT, "MATNR",0, SAPRFC.getSAPDB());
            if (dtZMARD_OUTPUT.Rows.Count > 0)
            {
                if (!txtMD04DAYS.Text.ToString().Equals(""))
                {
                    MD04DAYS = int.Parse(txtMD04DAYS.Text.ToString());
                }
                ReturnMD04(MD04DAYS, MATNR, strWERKS, SAPRFC.getSAPDB());
                ENTRY_QNT = ZGWH_STORAGE(MATNR, strWERKS);
                ENTRY_QNT_TODAY = ZGWH_STORAGE_TODAY(MATNR, strWERKS);
                VENDORNAME = SAPRFC.ZMM_SOURCE_BY_MATERIAL(MATNR, strWERKS, SAPRFC.getSAPDB());
                MCNAME = SAPRFC.ZMM_STOCK_REQUIREMENTS(MATNR, strWERKS, SAPRFC.getSAPDB());
                PRETOTAL_STOCK = ZGSKIN_BIN_CARD(MATNR, strWERKS);
                STD_COST = SAPRFC.Z_SD_SEARCH_COST_PRICE(MATNR, strWERKS, SAPRFC.getSAPDB());
                long n = 0;
                string sMATNR = MATNR;
                if (long.TryParse(MATNR, out n))
                {
                    sMATNR = MATNR.PadLeft(18, '0');
                }

                string[] material_ra = { sMATNR };
                string[] plant_ra = { strWERKS };
                string StartDate = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd");
                string txtEndDate = DateTime.Now.ToString("yyyy-MM-dd");
                string[] PSTNG_DATE_RA = { StartDate, txtEndDate };
                //string[] sloc_ra = { "" };
                // string[] move_type_ra = txt_MOVEMENT_TYPE.Text.Split(',');
                // string[] tr_ev_type_ra = { "WE", "WQ" };
                string[] pstng_date_ra = { StartDate, txtEndDate };
                DataSet Ds = SAPRFC.sBAPI_GOODSMVT_GETITEMS(material_ra, plant_ra, pstng_date_ra, SAPRFC.getSAPDB());
                //  Ds.Relations.Add(new DataRelation("MAT_DOC", Ds.Tables["GOODSMVT_HEADER"].Columns["MAT_DOC"], Ds.Tables["GOODSMVT_ITEMS"].Columns["MAT_DOC"]));

                Ds.Tables[0].DefaultView.Sort = "ENTRY_DATE DESC,ENTRY_TIME DESC";

                gdv_GOODSMVT_GETITEMS.DataSource = Ds.Tables[0];
                gdv_GOODSMVT_GETITEMS.DataBind();
            }
            else
            {
                Response.Write("<embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
            }
            //  oDs.Merge(dtZMARD_OUTPUT);

            DataColumn column1 = new DataColumn("ENTRY_QNT");
            column1.DataType = System.Type.GetType("System.Decimal");
            column1.DefaultValue = ENTRY_QNT[0];
            dtZMARD_OUTPUT.Columns.Add(column1);

            DataColumn column4 = new DataColumn("UNPULL_QNT");
            column4.DataType = System.Type.GetType("System.Decimal");
            column4.DefaultValue = ENTRY_QNT[1];
            dtZMARD_OUTPUT.Columns.Add(column4);

            DataColumn column2 = new DataColumn("ReqQry");
            column2.DataType = System.Type.GetType("System.Decimal");
            column2.DefaultValue = ReqQry;
            dtZMARD_OUTPUT.Columns.Add(column2);

            DataColumn column3 = new DataColumn("FeedQry");
            column3.DataType = System.Type.GetType("System.Decimal");
            column3.DefaultValue = FeedQry;
            dtZMARD_OUTPUT.Columns.Add(column3);


            DataColumn column6 = new DataColumn("VENDOR");
            column6.DataType = System.Type.GetType("System.String");
            column6.DefaultValue = VENDORNAME;
            dtZMARD_OUTPUT.Columns.Add(column6);

            DataColumn column7 = new DataColumn("MCNAME");
            column7.DataType = System.Type.GetType("System.String");
            column7.DefaultValue = MCNAME;
            dtZMARD_OUTPUT.Columns.Add(column7);

            DataColumn column9 = new DataColumn("PRETOTAL_STOCK");
            column9.DataType = System.Type.GetType("System.Decimal");
            column9.DefaultValue = PRETOTAL_STOCK;
            dtZMARD_OUTPUT.Columns.Add(column9);
            DataColumn column10 = new DataColumn("ENTRY_QNT_TODAY");
            column10.DataType = System.Type.GetType("System.Decimal");
            column10.DefaultValue = ENTRY_QNT_TODAY;
            dtZMARD_OUTPUT.Columns.Add(column10);

            DataColumn column11 = new DataColumn("STD_COST");
            column11.DataType = System.Type.GetType("System.Decimal");
            column11.DefaultValue = STD_COST;
            dtZMARD_OUTPUT.Columns.Add(column11);

            return dtZMARD_OUTPUT;

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }

    }
    protected void capdata()
    {
        int LGORTTYPE = int.Parse(rdoLGORTTYPE.SelectedValue);
        switch (LGORTTYPE)
        {
            case 0://ALL庫別
                strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158,0100,0110,0120,0121,0128,0129,0130";
                break;
            case 1://良品庫
                strLGORT = "0005,0008,0011,0012,0015,0016,0018,0030,0055,0056,0058,0071,0158";
                break;
            case 2://非良品庫
                strLGORT = "0100,0110,0120,0121,0128,0129,0130";
                break;
        }
        long n = 0;
        string MATNR = txt_MATNR.Text.ToString().Trim().ToUpper();
        if (long.TryParse(MATNR, out n))
        {
            MATNR = MATNR.PadLeft(18, '0');
        }

        if (rdoWERKS.SelectedValue.Equals("ALL"))
        {
            strWERKS = "TWM2,TWM3,TWM6";
        }
        else
        {
            strWERKS = rdoWERKS.SelectedValue;
        }

        ViewState["dtSAPSTOCKS"] = capZGBSN(MATNR, strWERKS);
        gdv_SAPSTOCKS.DataSource = ViewState["dtSAPSTOCKS"];
        gdv_SAPSTOCKS.DataBind();
        int icount = gdv_SAPSTOCKS.Rows.Count;
        //if (gdv_SAPSTOCKS.Rows.Count > 0)
        //{
        //    txt_MATNR.Text = "";
        //}
        if (icount > 0)
        {
            Response.Write("<embed src = 'seachfinish.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed>");
        }
        txt_MATNR.Attributes.Add("onfocus", "this.select();");
        txt_MATNR.Focus();
        modifyflag = 0;

    }
    protected void gdv_SAPSTOCKS_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //  string str_pk = e.CommandArgument.ToString();

        string PLANT = "", MATNR = "", WERKS = "", LGORT = "", LGPBE = "", LABSTLAST = "", EMPLR_ID = "", LABST = "", LABSTLASTNOTE = "";
        Button BTN = (Button)e.CommandSource;// 先抓到這個按鈕（我們設定了CommandName）
        GridViewRow myRow = (GridViewRow)BTN.NamingContainer; // 從你按下 Button按鈕的時候，NamingContainer知道你按下的按鈕在GridView「哪一列」！
        EMPLR_ID = txt_EMPNO.Text.ToString().Trim();
        MATNR = gdv_SAPSTOCKS.Rows[myRow.DataItemIndex].Cells[3].Text;
        WERKS = gdv_SAPSTOCKS.Rows[myRow.DataItemIndex].Cells[4].Text;
        LGORT = gdv_SAPSTOCKS.Rows[myRow.DataItemIndex].Cells[5].Text.Trim().ToUpper();
        LABST = gdv_SAPSTOCKS.Rows[myRow.DataItemIndex].Cells[6].Text;
        TextBox txt = (TextBox)gdv_SAPSTOCKS.Rows[myRow.DataItemIndex].FindControl("txtLGPBE");
        LGPBE = txt.Text.ToString().Trim().ToUpper().Replace(":", ";");
        dtSAPSTOCKS = (DataTable)ViewState["dtSAPSTOCKS"];
        string OldLGPBE = dtSAPSTOCKS.Rows[myRow.DataItemIndex]["LGPBE"].ToString().Trim().ToUpper();

        //修改儲位
        if (e.CommandName == "edit_data") 
        {
            // int pk_key = Convert.ToInt32(e.CommandArgument);
            //抓到RowCommand選取時的控制項
            if (EMPLR_ID.Equals(""))
            {
                Response.Write(" <script   language=javascript> window.alert( '修改儲位前請先輸入工號') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                txt_EMPNO.Attributes.Add("onfocus", "this.select();");
                txt_EMPNO.Focus();
                return;
            }
            else
            {
                if (ModuleFunction.CHECKWHEMPLR_ID(EMPLR_ID) != "Y")
                {
                    Response.Write("<script   language=javascript> window.alert( '你沒有權限修改儲位') </script><embed src = 'error2.wav' width = '0' height = '0' id = 'music' autostart = 'true'></embed> ");
                }
                else
                {
                    MODIFY_STORAGE_BIN(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, EMPLR_ID, LABST);
                    btn_search_Click(sender, e);
                }
            }
        }
        //查詢儲位修改LOG
        if (e.CommandName == "LOG_data")
        {
            string htm = "MODIFY_STORAGE_BINLOG.aspx?MATNR=" + MATNR;
            string javascript = "window.open('" + htm + "','','width=800px,height=500px,scrollbars=yes,resizable=yes, status=yes');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "LOG", javascript, true);

        }
    }
    /// <summary>
    /// 修改儲位
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="LGORT"></param>
    /// <param name="OldLGPBE"></param>
    /// <param name="LGPBE"></param>
    /// <param name="EMPLR_ID"></param>
    /// <param name="LABST"></param>
    void MODIFY_STORAGE_BIN(string MATNR, string WERKS, string LGORT, string OldLGPBE, string LGPBE, string EMPLR_ID,string LABST)
    {
        if (SAPRFC.ZMM_MODIFY_STORAGE_BIN(MATNR, WERKS, LGORT, LGPBE, SAPRFC.getSAPDB()) == "OK")
        {
            ModuleFunction.BKMODIFY_STORAGE_BINLOG(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, 1, EMPLR_ID, LABST);
            Response.Write(" <script   language=javascript> window.alert( '料號：【" + MATNR + "】 儲位已修改為【" + LGPBE + "】') </script> ");
        }
        else
        {
            ModuleFunction.BKMODIFY_STORAGE_BINLOG(MATNR, WERKS, LGORT, OldLGPBE, LGPBE, 0, EMPLR_ID, LABST);
            var str = "< span style = 'color: red;' > 修改【失敗】，請重新確認 </ span >";
            Response.Write(" <script   language=javascript> window.alert( '" + str + "') </script> ");
        }
        //this.btn_search_Click(null, null);
        // capdata();
    }
    protected void txt_MATNR_TextChanged(object sender, EventArgs e)
    {
        if (!txt_MATNR.Text.ToString().Equals(""))
        {
            capdata();
        }
    }

    /// <summary>
    /// 回傳MD04資訊
    /// </summary>
    /// <param name="AddDays"></param>
    /// <param name="MATNR"></param>
    /// <param name="WERKS"></param>
    /// <param name="dest"></param>
    public void ReturnMD04(int AddDays, string MATNR, string WERKS, RfcDestination dest)
    {

        //MATNR = MATNR.ToString().TrimStart('0');
        //string ed = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");
        string ed = DateTime.Now.AddDays(AddDays).ToString("yyyy-MM-dd");

        RfcRepository repository = dest.Repository;
        IRfcFunction rfc = repository.CreateFunction("BAPI_MATERIAL_STOCK_REQ_LIST");
        // string WONO = ""; string SDATE = DateTime.Now.AddDays(-7).ToString("yyyyMMdd"); string EDATE = DateTime.Now.AddDays(7).ToString("yyyyMMdd"); string PLANT = "TWM9"; 
        string SPFLG = "";
        rfc.SetValue("MATERIAL", MATNR);
        rfc.SetValue("PLANT", WERKS);
        rfc.SetValue("GET_ITEM_DETAILS", "X");
        rfc.Invoke(dest);
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        IRfcTable ZMARD_OUTPUT = rfc.GetTable("MRP_ITEMS");
        dt = SAPRFC.ConvertToTable(ZMARD_OUTPUT, "MRP_ITEMS");
        if (dt.Rows.Count > 0)
        {


            //dt.DefaultView.RowFilter = "AVAIL_DATE <='"+ ed+"'";
            DataRow[] filteredRows = dt.Select("AVAIL_DATE <='" + ed + "'");
            dt1 = filteredRows.CopyToDataTable();
            //dt.Rows.RemoveAt(0);

            int COUNT = dt.Rows.Count;
            //  dt.Columns[2].DataType=
            // DateTime sd = Convert.ToDateTime("2018/08/07");
            // var linqStament = from p in dt.AsEnumerable() where (DateTime)p.Field<DateTime>("AVAIL_DATE") <= DateTime.Today.AddDays(30)
            var linqStament = from p in dt1.AsEnumerable()

                              group p by new
                              {
                                  PLUS_MINUS = p.Field<string>("PLUS_MINUS")

                              } into g
                              //let row = g.First()
                              select new
                              {
                                  // MATNR = g.Key.MATNR
                                  //,
                                  //Total = g.Sum(x => Convert.ToInt32(x["LABST"]) + Convert.ToInt32(x["INSME"]))
                                  PLUS_MINUS = g.Key.PLUS_MINUS,
                                  REC_REQD_QTY = g.Sum(p => Convert.ToDouble(p["REC_REQD_QTY"]))
                                  ,
                                  SHORTAGE_QTY = g.Sum(p => Convert.ToDouble(p["SHORTAGE_QTY"]))
                              };

            foreach (var query in linqStament)
            {
                if (query.PLUS_MINUS.Equals("-"))
                {
                    ReqQry = query.REC_REQD_QTY;
                    // MD04List.Add(new myList { PartName = "ReqQry", REC_REQD_QTY = query.REC_REQD_QTY, SHORTAGE_QTY = query.SHORTAGE_QTY });
                }
                if (query.PLUS_MINUS.Equals("+"))
                {
                    FeedQry = query.REC_REQD_QTY;
                    //  MD04List.Add(new myList { PartName = "FeedQry", REC_REQD_QTY = query.REC_REQD_QTY, SHORTAGE_QTY = query.SHORTAGE_QTY });
                }
            }

        }
        gdv_SAPSTOCKS_MD04.DataSource = dt;
        gdv_SAPSTOCKS_MD04.DataBind();
    }
    protected void gdv_SAPSTOCKS_MD04_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string QA = e.Row.Cells[1].Text;
            if (QA == "-") { e.Row.Cells[2].BackColor = System.Drawing.Color.Red; }

        }
    }
}