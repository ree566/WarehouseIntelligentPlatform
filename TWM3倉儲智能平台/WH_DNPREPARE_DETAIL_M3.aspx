<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WH_DNPREPARE_DETAIL_M3.aspx.cs" Inherits="WH_DNPREPARE_DETAIL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TWM3DN出貨備料明細</title>
    <%--Bootstrap also supported.--%>
    <script src="bootstrap/js/jquery-3.4.1.slim.min.js"></script>
    <script src="bootstrap/js/popper.min.js"></script>
    <script src="bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" media="screen" href="bootstrap/css/bootstrap.min.css" />
    <%--    -----------------  --%>
</head>
<body>
    <!-- Modal Popup -->
    <div id="MyPopup" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        &times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body">
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">
                        Close</button>
                </div>
            </div>
        </div>
    </div>
    <!-- Modal Popup -->
    <form id="form1" runat="server">
        <div>
            <script>

                function TextBoxNumCheck_number() {
                    if ((event.keyCode < 48 || window.event.keyCode > 57) && window.event.keyCode != 46) event.returnValue = false;
                }

                function ShowPopup(title, body) {
                    $("#MyPopup .modal-title").html(title);
                    $("#MyPopup .modal-body").html(body);
                    $("#MyPopup").modal("show");
                }
                function CheckAllItem(Check) {
                    elm = document.forms[0];  //取得form表單

                    for (i = 0; i < elm.length; i++) {
                        if (elm[i].type == "checkbox" && elm[i].id != Check.id) //若為checkbox，並且ID與表頭CheckBox不同。表示為明細的CheckBox
                        {
                            if (elm.elements[i].checked != Check.checked)  //若明細的CheckBox的checked狀態與表頭CheckBox不同
                            {
                                elm.elements[i].click();  //明細的CheckBox執行click
                            }
                        }
                    }
                }

            </script>
            <div class="accordion" id="accordionExample">
                <div class="card">
                    <div class="card-header" id="headingOne">
                        <h2 class="mb-0">
                            <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                                新增發料資料(僅針對SAP人工發退料手動新增紀錄)
                            </button>
                        </h2>
                    </div>

                    <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordionExample">
                        <div class="card-body">
                            <div class="input-group mb-3">
                                <div class="input-group-prepend">
                                    <span class="input-group-text" id="lbl_EMP_NO">工號</span>
                                </div>
                                <asp:TextBox ID="txt_EMP_NO" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_EMP_NO" runat="server" autocomplete="off"></asp:TextBox>
                            </div>
                            <asp:Button ID="btn_ADDPUB_QTY" runat="server" Text="新增" class="btn btn-primary" OnClick="btn_ADDPUB_QTY_Click" />
                            勾選後，便可進行新增
                        </div>
                    </div>
                </div>
            </div>
            <asp:Button id="EXPORT_EXCEL" class="btn btn-outline-primary" runat="server" Text="匯出EXCEL"   OnClick="EXPORT_EXCEL_Click" />
            <div class="card">
                <h5 class="card-header">看板明細</h5>
                <div class="card-body">
                    <asp:GridView ID="gdv_WH_DNPREPARE_DETAIL" runat="server"
                        CellPadding="3" AutoGenerateColumns="False" OnRowDataBound="gdv_WH_DNPREPARE_DETAIL_RowDataBound"
                        CssClass="table table-striped">
                        <Columns>
                            <asp:TemplateField HeaderText="*" ItemStyle-BackColor="#f5f5f5" HeaderStyle-BackColor="#E2C2DE" ItemStyle-HorizontalAlign="Center">
                                <HeaderStyle Wrap="False" />
                                <ItemStyle Wrap="False" />
                                <HeaderTemplate>
                                    <asp:CheckBox ID="CheckAllItem" runat="server" onclick="javascript: CheckAllItem(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox" AutoCallBack="true" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ZWERKS" HeaderText="廠別"></asp:BoundField>
                            <asp:BoundField DataField="Detail_ID" HeaderText="ID"></asp:BoundField>
                            <asp:BoundField DataField="VBELN" HeaderText="DN單號"></asp:BoundField>
                            <asp:BoundField DataField="POSNR" HeaderText="POSNR"></asp:BoundField>
                            <%--         <asp:BoundField DataField="MATNR" HeaderText="料號">
        </asp:BoundField> --%>
                            <asp:TemplateField HeaderText="料號">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" Text='<%# DataBinder.Eval(Container.DataItem, "MATNR")%>'
                                        NavigateUrl='<%# string.Format("WH_SKIN_WIP_M_M3.aspx?AUFNR={0}&POSNR={1}&TYPE=-1", Eval("VBELN"), Eval("POSNR")) %>'> </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="MAKTX" HeaderText="描述"></asp:BoundField>
                            <asp:BoundField DataField="LGPBE" HeaderText="儲位"></asp:BoundField>
                            <asp:BoundField DataField="LABST" HeaderText="庫存"></asp:BoundField>
                            <asp:BoundField DataField="ENTRY_QNT_TODAY" HeaderText="今日上架數"></asp:BoundField>
                            <asp:BoundField DataField="ENTRY_QNT" HeaderText="已上架"></asp:BoundField>
                            <asp:BoundField DataField="UNPULL_QNT" HeaderText="待拉料"></asp:BoundField>
                            <asp:BoundField DataField="INSME" HeaderText="待驗"></asp:BoundField>
                            <asp:BoundField DataField="ZLGORT" HeaderText="庫位"></asp:BoundField>
                            <asp:BoundField DataField="LFIMG" HeaderText="應備數" DataFormatString="{0:N0}"></asp:BoundField>
                            <asp:BoundField DataField="PUB_QTY" HeaderText="已備數"></asp:BoundField>
                            <asp:BoundField DataField="PUB_QTY_NPT" HeaderText="異常數"></asp:BoundField>
                            <asp:BoundField DataField="OPEN_QTY" HeaderText="未備數"></asp:BoundField>
                            <asp:BoundField DataField="IMPORT" HeaderText="洋貨"></asp:BoundField>
                            <asp:BoundField DataField="ZPRDPLC" HeaderText="ZPRDPLC"></asp:BoundField>
                            <asp:BoundField DataField="ZTRADEMARK" HeaderText="ZTRADEMARK"></asp:BoundField>
                            <asp:BoundField DataField="MAT_EMP_NO" HeaderText="料階人員"></asp:BoundField>
                            <asp:BoundField DataField="Detail_REMARK" HeaderText="倉庫備註"></asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
                <asp:Label ID="Label2" runat="server" style="font-weight: 700; color: #FF0000"></asp:Label>
            </div>
        </div>
        
    </form>
</body>
</html>
