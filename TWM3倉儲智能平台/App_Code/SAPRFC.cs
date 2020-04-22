using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAP.Middleware.Connector;
using System.Data;

/// <summary>
/// Summary description for SAPRFC
/// </summary>

public class SAPRFC
{
    #region SAP連線 add by Apple at 20190215
    public RfcDestination getSAPDB()
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
    public DataTable ZGBSN(DataTable dt, string strWERKS, string strLGORT, string Distinctname, int SUMTYPE, RfcDestination dest)
    {
        DataSet oDs = new DataSet();
        DataTable distinctValues = new DataTable();
        try
        {

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
            IRfcTable ZMARD_INPUT = rfc.GetTable("ZMARD_INPUT");
            string[] sArrayLGORT = strLGORT.Split(',');
            string[] sArrayWERKS = strWERKS.Split(',');
            DataView view = new DataView(dt);
            distinctValues = view.ToTable(true, Distinctname);
            foreach (string SubstrWERKS in sArrayWERKS)
            {
                foreach (string SubstrLGORT in sArrayLGORT)
                {
                    foreach (DataRow od in distinctValues.Rows)
                    {
                        string strMaterial = od[Distinctname].ToString();
                        IRfcStructure MARD = repository.GetStructureMetadata("MARD").CreateStructure();
                        MARD.SetValue("MATNR", strMaterial);
                        MARD.SetValue("WERKS", SubstrWERKS);
                        MARD.SetValue("LGORT", SubstrLGORT);
                        ZMARD_INPUT.Insert(MARD);
                    }

                }
            }
            rfc.SetValue("ZMARD_INPUT", ZMARD_INPUT);//寫入資料
            rfc.Invoke(dest);

            IRfcTable ZMARD_OUTPUT = rfc.GetTable("ZMARD_OUTPUT");
            //   DataTable dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "ZMARD_OUTPUT");

            oDs.Tables.Add(ConvertToTable(rfc.GetTable("ZMARD_OUTPUT"), "ZMARD_OUTPUT"));  //獲取相應的業務內表, 母表單

            //   oDs.Merge(dtZMARD_OUTPUT);

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }
        if (SUMTYPE != 0)
        {
            dt = InventoryIntegrationMissingincoming(dt, oDs.Tables[0], Distinctname);
        }
        else
        {
            dt = oDs.Tables[0];
        }
        return dt;

        // return oDs.Tables[0];
    }
    #endregion
    #region 整合庫存資料 add by Apple at 20190215
    public DataTable InventoryIntegrationMissingincoming(DataTable dt, DataTable dt1, string Distinctname)
    {
        try
        {


            var linqStament = from p in dt1.AsEnumerable()
                              group p by new
                              {
                                  MATNR = p.Field<string>("MATNR")
                              } into g
                              //let row = g.First()
                              select new
                              {
                                  MATNR = g.Key.MATNR
                                  ,
                                  //Total = g.Sum(x => Convert.ToInt32(x["LABST"]) + Convert.ToInt32(x["INSME"]))
                                  LABST = g.Sum(x => Convert.ToDouble(x["LABST"]))
                                  ,
                                  INSME = g.Sum(x => Convert.ToDouble(x["INSME"]))
                                  ,
                                  LGPBE = string.Join(",", g.Select(x => Convert.ToString(x["LGPBE"])))
                              };

            // dt.Columns.Add("Total");

            dt.Columns.Add("LABST_TOTAL", typeof(Double)); //庫存
            dt.Columns["LABST_TOTAL"].DefaultValue = 0;
            dt.Columns.Add("INSME", typeof(Double)); //待驗
            dt.Columns["INSME"].DefaultValue = 0;
            dt.Columns.Add("LGPBE", typeof(String)); //儲位
            foreach (var query in linqStament)
            {
                foreach (DataRow od in dt.Rows)
                {
                    if (od[Distinctname].ToString().TrimStart('0') == query.MATNR.ToString().TrimStart('0'))
                    {
                        // od["Total"] = query.Total;
                        od["LABST_TOTAL"] = query.LABST;
                        od["INSME"] = query.INSME;
                        od["LGPBE"] = query.LGPBE;
                    }
                }
            }
            dt.TableName = "Integration";
            return dt;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    //private DataTable ZCN_GET_BIN(DataTable dt, RfcDestination dest)
    //{

    //}

    #region 撈取SAP DN明細
    public DataTable ZGET_DN_INFO_2(string VBELN, RfcDestination dest)
    {
        DataTable dt = new DataTable();
        try
        {
            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZGET_DN_INFO_2");
            rfc.SetValue("VBELN", VBELN);
            rfc.Invoke(dest);
            dt = ConvertToTable(rfc.GetTable("ZDNINFO"), "ZDNINFO");
        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }
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
    #region MB51功能

    void ParameterRange(ref IRfcTable tb, string Low, string High)
    {
        tb.Append(1);
        tb[0].SetValue("SIGN", "I");
        tb[0].SetValue("OPTION", "BT");
        tb[0].SetValue("LOW", DateTime.Parse(Low));
        tb[0].SetValue("HIGH", DateTime.Parse(High));
    }

    void ParameterRange2(ref IRfcTable tb, string Low, string High)
    {
        tb.Append(1);
        tb[0].SetValue("SIGN", "I");
        tb[0].SetValue("OPTION", "BT");
        tb[0].SetValue("LOW", DateTime.Parse(Low));
        tb[0].SetValue("HIGH", DateTime.Parse(High));
        //tb[0].SetValue("LOW", DateTime.Parse("2019/08/01"));
        //tb[0].SetValue("HIGH", DateTime.Parse("2019/08/31"));
    }
    void ParameterRange(ref IRfcTable tb, string[] paramArray)
    {
        int len = paramArray.Length;
        if (len > 0)
        {
            tb.Append(len);
            for (int i = 0; i < len; i++)
            {
                if (tb.Metadata.LineType.Name == "BAPI2017_GM_MATERIAL_RA")
                {
                    int result;
                    if (int.TryParse(paramArray[i], out result))
                    {
                        paramArray[i] = paramArray[i].PadLeft(18, '0');
                    }
                }
                tb[i].SetValue("LOW", paramArray[i]);
            }
        }
    }
    /// <summary>
    /// 交易紀錄
    /// </summary>
    /// <param name="_MATERIAL_RA"></param>
    /// <param name="_PLANT_RA"></param>
    /// <param name="_STGE_LOC_RA"></param>
    /// <param name="_MOVE_TYPE_RA"></param>
    /// <param name="_TR_EV_TYPE_RA"></param>
    /// <param name="_PSTNG_DATE_RA"></param>
    /// <returns></returns>
    public DataSet sBAPI_GOODSMVT_GETITEMS(string[] _MATERIAL_RA, string[] _PLANT_RA, string[] _PSTNG_DATE_RA, RfcDestination dest)
    {
        try
        {
            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("BAPI_GOODSMVT_GETITEMS");
            IRfcTable MATERIAL_RA = rfc.GetTable("MATERIAL_RA");
            ParameterRange(ref MATERIAL_RA, _MATERIAL_RA);
            IRfcTable PLANT_RA = rfc.GetTable("PLANT_RA");
            ParameterRange(ref PLANT_RA, _PLANT_RA);
            //  IRfcTable STGE_LOC_RA = rfc.GetTable("STGE_LOC_RA");
            // ParameterRange(ref STGE_LOC_RA, _STGE_LOC_RA);
            //  IRfcTable MOVE_TYPE_RA = rfc.GetTable("MOVE_TYPE_RA");
            // ParameterRange(ref MOVE_TYPE_RA, _MOVE_TYPE_RA);
            // IRfcTable TR_EV_TYPE_RA = rfc.GetTable("TR_EV_TYPE_RA");
            //  ParameterRange(ref TR_EV_TYPE_RA, _TR_EV_TYPE_RA);
            IRfcTable PSTNG_DATE_RA = rfc.GetTable("PSTNG_DATE_RA");
            ParameterRange2(ref PSTNG_DATE_RA, _PSTNG_DATE_RA[0], _PSTNG_DATE_RA[1]);
            rfc.Invoke(dest);

            IRfcTable GOODSMVT_HEADER = rfc.GetTable("GOODSMVT_HEADER");
            IRfcTable GOODSMVT_ITEMS = rfc.GetTable("GOODSMVT_ITEMS");
            IRfcTable RETURN = rfc.GetTable("RETURN");

            DataSet ds = BAPI_GOODSMVT_GETITEMS_RESULT(GOODSMVT_HEADER, GOODSMVT_ITEMS, RETURN);
            ds.Tables[0].Columns.Add("MoveNAME");
            ds.Tables[0].Columns.Add("STATUS");
            ds.Tables[0].Columns["MoveNAME"].SetOrdinal(7);
            ds.Tables[0].Columns["STATUS"].SetOrdinal(16);
            ds.Tables[0].Columns.Add("MoveChange");
            DataTable dt = ATMCdb.reDt("SELECT  [MovementType],[NAME],[MoveChange] FROM [ATMC].[M9WH].[MOVEMENT_TYPE]");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string MovementType = dr["MOVE_TYPE"].ToString();
                string ORDERID = dr["ORDERID"].ToString();
                string STCK_TYPE = dr["STCK_TYPE"].ToString();
                dr["STATUS"] = ATMCdb.scalDs("SELECT [STATUS] FROM [ATMC].[dbo].[SKIN_WO_STATUS] WHERE [AUFNR]='" + ORDERID + "'");
                foreach (DataRow dr2 in dt.Rows)
                {

                    if (MovementType == dr2["MovementType"].ToString())
                    {
                        if (MovementType.Equals("101") && STCK_TYPE.Equals("X"))
                        {
                            dr["MoveNAME"] = "訂單待驗";
                        }
                        else
                        {
                            dr["MoveNAME"] = dr2["NAME"].ToString();
                        }

                        dr["MoveChange"] = dr2["MoveChange"].ToString();
                        break;
                    }
                }
            }


            return ds;

        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex1)
        {
            throw new Exception(ex1.Message);
        }

    }


    #region Fileds
    public const string sX_AUTO_CRE = "X_AUTO_CRE";
    public const string sX = "X";
    public const string sMAT_DOC = "MAT_DOC";
    public const string sMATDOC_ITM = "MATDOC_ITM";
    public const string sMATERIAL = "MATERIAL";
    public const string sPLANT = "PLANT";
    public const string sSTGE_LOC = "STGE_LOC";
    public const string sENTRY_QNT = "ENTRY_QNT";
    public const string sVENDOR = "VENDOR";
    public const string sUSERNAME = "USERNAME";
    public const string sMOVE_TYPE = "MOVE_TYPE";
    public const string sSTCK_TYPE = "STCK_TYPE";
    public const string sPO_NUMBER = "PO_NUMBER";
    public const string sPO_ITEM = "PO_ITEM";
    public const string sREF_DOC = "REF_DOC";
    public const string sORDERID = "ORDERID";
    public const string sHEADER_TXT = "HEADER_TXT";
    public const string sENTRY_DATE = "ENTRY_DATE";
    public const string sENTRY_TIME = "ENTRY_TIME";


    public const string Mvt101 = "101";
    public const string Mvt102 = "102";


    string[] GOODSMVT_ITEMS_FILEDS = {
                sMAT_DOC,
                sMATDOC_ITM,

                sMATERIAL,
                sPLANT,
                sSTGE_LOC,
                sMOVE_TYPE,
                sSTCK_TYPE,
                "MOVE_PLANT",
                "MOVE_STLOC",
                sENTRY_QNT,
                sVENDOR,
                sPO_NUMBER,
                sPO_ITEM,
                sREF_DOC,
                sORDERID,
            };
    string[] GOODSMVT_HEADER_FILEDS = {
                sMAT_DOC,
                //"TR_EV_TYPE",
                sENTRY_DATE,
                sENTRY_TIME,
                sUSERNAME,
                "REF_DOC_NO_LONG",
                //"REF_DOC_NO",
                sHEADER_TXT,
            };
    #endregion
    DataSet BAPI_GOODSMVT_GETITEMS_RESULT(IRfcTable GOODSMVT_HEADER, IRfcTable GOODSMVT_ITEMS, IRfcTable RETURN)
    {

        DataTable dt = new DataTable();

        #region 加入所有欄位名稱
        foreach (string item in GOODSMVT_ITEMS_FILEDS)
        {
            dt.Columns.Add(item);
        }
        foreach (string item in GOODSMVT_HEADER_FILEDS)
        {
            if (item == sMAT_DOC) continue;
            dt.Columns.Add(item);
        }
        #endregion

        foreach (IRfcStructure GOODSMVT_ITEMS_Structure in GOODSMVT_ITEMS)
        {

            #region 略過自動產生的帳務項目

            string X_auto = GOODSMVT_ITEMS_Structure.GetString(sX_AUTO_CRE).Trim();
            string X_STACK_TYPE = GOODSMVT_ITEMS_Structure.GetString("STCK_TYPE").Trim();
            //if (X_auto == sX || X_STACK_TYPE == sX)
            if (X_auto == sX)
            {
                continue;
            }

            #endregion

            DataRow row = dt.NewRow();

            ///取得列中每欄資料
            foreach (string item_filed in GOODSMVT_ITEMS_FILEDS)
            {

                string item = GOODSMVT_ITEMS_Structure.GetString(item_filed).Trim();

                #region 加入檔頭資訊

                if (item_filed == sMAT_DOC)
                {
                    row[item_filed] = item;

                    foreach (IRfcStructure GOODSMVT_HEADER_Structure in GOODSMVT_HEADER)
                    {
                        string header = GOODSMVT_HEADER_Structure.GetString(sMAT_DOC).Trim();
                        if (header != item)
                        {
                            continue;
                        }

                        foreach (string header_filed in GOODSMVT_HEADER_FILEDS)
                        {
                            if (header_filed == sMAT_DOC) continue;
                            header = GOODSMVT_HEADER_Structure.GetString(header_filed).Trim();
                            row[header_filed] = header;
                        }
                    }
                    continue;
                }

                #endregion


                #region 料號&PO去除前頭0
                if (item_filed == sMATERIAL ||
                    item_filed == sPO_NUMBER)
                {
                    item = item.TrimStart('0');
                }
                #endregion
                #region 數量去除小數點
                if (item_filed == sENTRY_QNT)
                {
                    item = item.Split('.')[0];
                }
                #endregion

                row[item_filed] = item;
            }
            dt.Rows.Add(row);

        }


        #region 處理102迴轉

        //bool check;

        //check = true;
        //while (check)
        //{
        //    check = false;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        if (row[sMOVE_TYPE].ToString() == Mvt102)
        //        {
        //            check = true;
        //            foreach (DataRow row_check in dt.Rows)
        //            {
        //                if (row_check[sREF_DOC].ToString() == row[sREF_DOC].ToString() && row_check[sMOVE_TYPE].ToString() == Mvt101)
        //                {
        //                    row_check.Delete();
        //                    break;
        //                }
        //            }
        //            row.Delete();
        //            break;
        //        }
        //    }
        //}

        #endregion

        DataTable dt1 = RfcTableToDataTable(RETURN);

        DataSet ds = new DataSet();
        ds.Tables.Add(dt);
        ds.Tables.Add(dt1);

        return ds;
    }

    public DataTable RfcTableToDataTable(IRfcTable rfcTable)
    {
        DataTable tb = new DataTable();
        try
        {

            ///取得標題列
            RfcTableToDataTableColumns(rfcTable, ref tb);

            foreach (IRfcStructure Structure in rfcTable)
            {
                DataRow row = tb.NewRow();

                ///取得列中每欄資料
                RfcStructureToDataRow(Structure, ref row);

                tb.Rows.Add(row);
            }
        }
        catch
        {
        }
        return tb;
    }

    public void RfcTableToDataTableColumns(IRfcTable rfcTable, ref DataTable tb)
    {
        try
        {
            for (int i = 0; i <= rfcTable.ElementCount - 1; i++)
            {
                RfcElementMetadata Metadata = rfcTable.GetElementMetadata(i);

                if (In<string>(Metadata.Name, GOODSMVT_ITEMS_FILEDS))
                {
                    continue;
                }
                tb.Columns.Add(Metadata.Name);
            }
        }
        catch
        {
        }

    }


    public void RfcStructureToDataRow(IRfcStructure Structure, ref DataRow row)
    {
        try
        {
            for (int i = 0; i <= Structure.ElementCount - 1; i++)
            {
                RfcElementMetadata Metadata = Structure.GetElementMetadata(i);
                if (In<string>(Metadata.Name, GOODSMVT_ITEMS_FILEDS))
                {
                    continue;
                }

                string value = Structure.GetString(Metadata.Name).Trim();
                if (Metadata.Name == "MATERIAL")
                {
                    value = value.TrimStart('0');
                }

                row[Metadata.Name] = value;
            }
        }
        catch
        {
        }
    }


    #endregion
    //public DataTable RfcTableToDataTable(IRfcTable rfcTable, string[] ITEMS_FILEDS)
    //{
    //    DataTable tb = new DataTable();
    //    try
    //    {

    //        ///取得標題列
    //        RfcTableToDataTableColumns(rfcTable, ITEMS_FILEDS, ref tb);

    //        foreach (IRfcStructure Structure in rfcTable)
    //        {
    //            DataRow row = tb.NewRow();

    //            ///取得列中每欄資料
    //            RfcStructureToDataRow(Structure, ITEMS_FILEDS, ref row);

    //            tb.Rows.Add(row);
    //        }
    //    }
    //    catch
    //    {
    //    }
    //    return tb;
    //}

    //public void RfcTableToDataTableColumns(IRfcTable rfcTable, string[] ITEMS_FILEDS, ref DataTable tb)
    //{
    //    try
    //    {
    //        for (int i = 0; i <= rfcTable.ElementCount - 1; i++)
    //        {
    //            RfcElementMetadata Metadata = rfcTable.GetElementMetadata(i);

    //            if (In<string>(Metadata.Name, ITEMS_FILEDS))
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


    //public void RfcStructureToDataRow(IRfcStructure Structure, string[] ITEMS_FILEDS, ref DataRow row)
    //{
    //    try
    //    {
    //        for (int i = 0; i <= Structure.ElementCount - 1; i++)
    //        {
    //            RfcElementMetadata Metadata = Structure.GetElementMetadata(i);
    //            if (In<string>(Metadata.Name, ITEMS_FILEDS))
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

    /// <summary>
    /// 找出物料供應商
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public string ZMM_SOURCE_BY_MATERIAL(string MATNR, string strWERKS, RfcDestination dest)
    {
        string VENDORNAME = "";

        try
        {

            long n = 0;
            if (long.TryParse(MATNR.Trim().ToUpper(), out n))
            {
                MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
            }
            else
            {
                MATNR = MATNR.Trim().ToUpper();
            }

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZMM_SOURCE_BY_MATERIAL");
            rfc.SetValue("I_WERKS", strWERKS);
            IRfcTable ZMARD_INPUT = rfc.GetTable("T_MATNR");
            // string strMaterial = MATNR.ToString().TrimStart('0');
            IRfcStructure MARD = repository.GetStructureMetadata("ZMATNR").CreateStructure();
            MARD.SetValue("MATNR", MATNR);
            ZMARD_INPUT.Insert(MARD);
            //}

            DataTable dtZMARD_OUTPUT = new DataTable();

            rfc.SetValue("T_MATNR", ZMARD_INPUT);//寫入資料

            rfc.Invoke(dest);

            IRfcTable ZMARD_OUTPUT = rfc.GetTable("T_MAT_SOURCE");
            dtZMARD_OUTPUT = ConvertToTable(ZMARD_OUTPUT, "T_MAT_SOURCE");
            //  oDs.Merge(dtZMARD_OUTPUT);
            if (dtZMARD_OUTPUT.Rows.Count > 0)
            {
                VENDORNAME = dtZMARD_OUTPUT.Rows[0]["NAME1"].ToString();

                // GridView1.DataSource = dtZMARD_OUTPUT;
                //GridView1.DataBind();
            }

        }
        catch (RfcAbapException ex)
        {
            // throw new Exception(ex.Message);
        }
        //catch (Exception ex1)
        //{
        //    throw new Exception(ex1.Message);
        //}

        return VENDORNAME;

    }

    /// <summary>
    /// 抓出物料MC管理人員
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public string ZMM_STOCK_REQUIREMENTS(string MATNR, string strWERKS, RfcDestination dest)
    {
        string MCNAME = "";
        try
        {

            long n = 0;
            if (long.TryParse(MATNR.Trim().ToUpper(), out n))
            {
                MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
            }
            else
            {
                MATNR = MATNR.Trim().ToUpper();
            }

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("ZMM_STOCK_REQUIREMENTS");
            rfc.SetValue("WERKS", strWERKS);
            rfc.SetValue("MATNR", MATNR);
            rfc.Invoke(dest);
            MCNAME = rfc.GetValue("E_DSNAM").ToString();


        }
        catch (RfcAbapException ex)
        {
            throw new Exception(ex.Message);
        }

        catch (RfcAbapMessageException ex)
        {
            // throw new Exception(ex.Message);
            return ex.ToString();
        }
        //catch (Exception ex1)
        //{
        //    throw new Exception(ex1.Message);
        //}

        return MCNAME;
    }


    /// <summary>
    /// 抓出物料單價
    /// </summary>
    /// <param name="MATNR"></param>
    /// <param name="strWERKS"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public double Z_SD_SEARCH_COST_PRICE(string MATNR, string strWERKS, RfcDestination dest)
    {
        DataTable dtLE_ZSD_COST = new DataTable();
        double STD_COST = 0;
        try
        {
            long n = 0;
            if (long.TryParse(MATNR.Trim().ToUpper(), out n))
            {
                MATNR = MATNR.Trim().ToUpper().PadLeft(18, '0');
            }
            else
            {
                MATNR = MATNR.Trim().ToUpper();
            }

            RfcRepository repository = dest.Repository;
            IRfcFunction rfc = repository.CreateFunction("Z_SD_SEARCH_COST_PRICE");
            rfc.SetValue("WERKS", strWERKS);
            rfc.SetValue("MATNR", MATNR);
            rfc.Invoke(dest);
            IRfcTable LE_ZSD_COST = rfc.GetTable("LE_ZSD_COST");
            dtLE_ZSD_COST = ConvertToTable(LE_ZSD_COST, "LE_ZSD_COST");
            if (dtLE_ZSD_COST.Rows[0]["PE_STPRS"] != DBNull.Value) STD_COST = double.Parse(dtLE_ZSD_COST.Rows[0]["PE_STPRS"].ToString());
        }
        catch (Exception)
        {

            throw;
        }
        return STD_COST;
    }

    private bool In<T>(T obj, T[] args)
    {
        return args.Contains(obj);
    }

    public string ZMM_MODIFY_STORAGE_BIN(string MATNR, string WERKS, string LGORT, string LGPBE, RfcDestination dest)
    {
        string nresult = "";
        try
        {
            RfcRepository Repo = dest.Repository;
            //  RfcRepository repository = dest.Repository;
            //  IRfcFunction rfc = repository.CreateFunction("ZCN_GET_BIN_STOCK_N");
            IRfcFunction rfc = Repo.CreateFunction("ZMM_MODIFY_STORAGE_BIN");
            rfc.SetValue("MATNR", MATNR);
            rfc.SetValue("WERKS", WERKS);
            rfc.SetValue("LGORT", LGORT);
            rfc.SetValue("LGPBE", LGPBE);
            rfc.Invoke(dest);
            DataTable dt = new DataTable();
            IRfcTable ZMARD_OUTPUT = rfc.GetTable("OUT_PUT");
            dt = ConvertToTable(ZMARD_OUTPUT, "OUT_PUT");

            if (dt.Rows[0]["FLAG"].ToString() == "Y")
            {
                nresult = "OK";
            }
            else
            {
                nresult = "NG";
            }
        }
        catch (Exception ex)
        {

            nresult = "SAP修改執行錯誤：" + ex.ToString();
        }
        return nresult;
    }

}