<%@ Page Title="TWM3DN出貨備料看板" Language="C#" MasterPageFile="Site.master" AutoEventWireup="true" CodeFile="WH_DNPREPARE_Schedule_M3.aspx.cs" Inherits="WH_DNPREPARE_Schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="accordion" id="accordionExample">
        <div class="card">
            <div class="card-header" id="headingOne">
                <h2 class="mb-0">
                    <button class="btn btn-link" type="button" data-toggle="collapse" data-target="#collapseOne" aria-expanded="false" aria-controls="collapseOne">
                        查詢功能
                    </button>
                </h2>
            </div>

            <div id="collapseOne" class="collapse" aria-labelledby="headingOne" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_EMP_NO">料階人員</span>
                        </div>
                        <asp:DropDownList ID="rdoEMP_NO" runat="server" class="custom-select">
                        </asp:DropDownList>
                    </div>
                    <div class="form-check form-check-inline">
                        <asp:RadioButtonList ID="rdoMDDATE" class="form-check-input" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="A">ALL</asp:ListItem>
                            <asp:ListItem Value="N" Selected="True">待出貨</asp:ListItem>
                            <asp:ListItem Value="E">已出貨</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <br />
                    <div class="form-check form-check-inline">
                        <asp:RadioButtonList ID="rdoDONEPREPARE" class="form-check-input" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="A" Selected="True">ALL</asp:ListItem>
                            <asp:ListItem Value="N">待備料</asp:ListItem>
                            <asp:ListItem Value="E">已備料</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_AUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtAUFNR" runat="server"></asp:TextBox>
                        輸入DN不受日期、類別影響
                    </div>
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <span class="input-group-text">出貨確效日期</span>
                        </div>
                        <%-- <div class="container">--%>

                        <input id="startDate" name="startDate" type="text" aria-label="startDate" class="form-control" onkeypress="if (event.keyCode == 13) {return false;}" />
                        <input id="endDate" name="endDate" type="text" aria-label="endDate" class="form-control" onkeypress="if (event.keyCode == 13) {return false;}" />
                        <%--    </div>--%>
                        <script>
                            $('#startDate').datepicker({
                                uiLibrary: 'bootstrap4',
                                //iconsLibrary: 'fontawesome',
                                // value: today,
                                format: 'yyyy-mm-dd',

                                //    value:today.addDays(-7),
                                //minDate: today,
                                maxDate: function () {
                                    return $('#endDate').val();
                                }
                            });
                            $('#endDate').datepicker({
                                uiLibrary: 'bootstrap4',
                                //iconsLibrary: 'fontawesome',
                                //value: today,
                                format: 'yyyy-mm-dd',

                                minDate: function () {
                                    return $('#startDate').val();
                                }
                            });
                        </script>

                    </div>
                    <asp:Button ID="btn_search" runat="server" Text="查詢" class="btn btn-primary" OnClick="btn_search_Click" />
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header" id="headingTwo">
                <h2 class="mb-0">
                    <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseTwo" aria-expanded="True" aria-controls="collapseTwo">
                        出貨確效
                    </button>
                </h2>
            </div>
            <div id="collapseTwo" class="collapse show" aria-labelledby="headingTwo" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMREMARK">出貨備註</span>
                        </div>
                        <asp:TextBox ID="txt_CFMREMARK" class="form-control" aria-label="Sizing example input" placeholder="原備註將會被覆蓋" aria-describedby="lbl_CFMREMARK" runat="server"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMEMP_NO">出貨人員</span>
                        </div>
                        <asp:TextBox ID="txtCFMEMP_NO" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_CFMEMP_NO" runat="server"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMAUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtCFMAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtCFMAUFNR" runat="server" autocomplete="off" OnTextChanged="txtCFMAUFNR_TextChanged" ></asp:TextBox>
                    </div>

                    <asp:Button ID="btn_CFMDN" runat="server" Text="出貨確效" class="btn btn-primary" OnClick="btn_CFMDN_Click" OnClientClick="return confirm('確定要確效?');" />
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header" id="headingFive">
                <h2 class="mb-0">
                    <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFive" aria-expanded="false" aria-controls="collapseFive">
                        包裝確效
                    </button>
                </h2>
            </div>
            <div id="collapseFive" class="collapse" aria-labelledby="headingFive" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMPackREMARK">倉庫備註</span>
                        </div>
                        <asp:TextBox ID="txt_CFMPackREMARK" class="form-control" aria-label="Sizing example input" placeholder="原備註將會被覆蓋" aria-describedby="lbl_CFMPackREMARK" runat="server"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMPackEMP_NO">包裝人員</span>
                        </div>
                        <asp:TextBox ID="txtCFMPackEMP_NO" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_CFMPackEMP_NO" runat="server"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CFMPackAUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtCFMPackAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtCFMPackAUFNR" OnTextChanged="txtCFMPackAUFNR_TextChanged"  runat="server" autocomplete="off"></asp:TextBox>
                    </div>

                    <asp:Button ID="btn_CFMPackDN" runat="server" Text="包裝確校" class="btn btn-primary" OnClick="btn_CFMPackDN_Click" OnClientClick="return confirm('確定要包裝確效?');" />
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header" id="headingThree">
                <h2 class="mb-0">
                    <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                        DN單備註修改</button>
                </h2>
            </div>
            <div id="collapseThree" class="collapse" aria-labelledby="headingThree" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_REMARKAUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtREMARKAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtREMARKAUFNR" runat="server"></asp:TextBox>
                    </div>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_WH_REMARK">倉庫備註</span>
                        </div>
                        <asp:TextBox ID="txtWH_REMARK" class="form-control" aria-label="Sizing example input" placeholder="原備註將會進行堆疊" aria-describedby="lbl_WH_REMARK" runat="server"></asp:TextBox>
                    </div>
                    <asp:Button ID="btn_WH_REMARK" runat="server" Text="備註修改" class="btn btn-primary" OnClick="btn_WH_REMARK_Click" />
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header" id="headingFour">
                <h2 class="mb-0">
                    <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseFour" aria-expanded="false" aria-controls="collapseFour">
                        加入DN單
                    </button>
                </h2>
            </div>
            <div id="collapseFour" class="collapse" aria-labelledby="headingFour" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_ADDAUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtADDAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtADDAUFNR" runat="server"></asp:TextBox>
                    </div>
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="強制更新" Checked="false" />
                    <asp:Button ID="btn_ADDAUFNR" runat="server" Text="加入DN單" class="btn btn-primary" OnClick="btn_ADDAUFNR_Click" />
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header" id="headingSIX">
                <h2 class="mb-0">
                    <button class="btn btn-link collapsed" type="button" data-toggle="collapse" data-target="#collapseSIX" aria-expanded="false" aria-controls="collapseSIX">
                        取消出貨/包裝確效
                    </button>
                </h2>
            </div>
            <div id="collapseSIX" class="collapse" aria-labelledby="headingSIX" data-parent="#accordionExample">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text" id="lbl_CancelAUFNR">DN單</span>
                        </div>
                        <asp:TextBox ID="txtCancelAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtCancelAUFNR" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-check form-check-inline">
                            <asp:RadioButtonList ID="rdoCancelAUFNRTYPE" class="form-check-input" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="-1" Selected="True">請選擇</asp:ListItem>
                                <asp:ListItem Value="1">取消包裝確效</asp:ListItem>
                                <asp:ListItem Value="2">取消出貨確效</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <asp:Button ID="btn_CancelAUFNR" runat="server" Text="取消確效" class="btn btn-primary" OnClick="btn_CancelAUFNR_Click" />
                    </div>
                </div>
        </div>
    </div>
<asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
    <div class="card">
        <h5 class="card-header">看板明細</h5>
        <div class="card-body">
            <asp:GridView ID="gdv_DNPREPARE_Schedule" runat="server"
                CellPadding="3" AutoGenerateColumns="False" OnRowDataBound="gdv_DNPREPARE_Schedule_RowDataBound" OnRowCommand="gdv_DNPREPARE_Schedule_RowCommand" DataKeyNames="PK_ID"
                CssClass="table table-striped">
                <Columns>
                    <asp:TemplateField HeaderText="已列印">
                        <ItemTemplate>
                            <asp:Button ID="btn_PRINT" runat="server" CommandName="PRINT_data" CausesValidation="False"
                                Text="已列印"
                                class="btn btn-outline-danger" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="left" Wrap="False" />
                        <HeaderStyle HorizontalAlign="left" />
                    </asp:TemplateField>
                    <%--<asp:BoundField DataField="PK_ID" HeaderText="ID"></asp:BoundField>--%>
                    <asp:BoundField DataField="VBELN" HeaderText="DN單號"></asp:BoundField>
                    <%--<asp:BoundField DataField="DN_Version" HeaderText="版本號"></asp:BoundField>--%>
                    <asp:BoundField DataField="MODATE" HeaderText="備料日" DataFormatString="{0:MM-dd}">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DN_ShippingDate" HeaderText="出貨日" DataFormatString="{0:MM-dd}">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <%--<asp:BoundField DataField="ITEMQTY" HeaderText="備料數"></asp:BoundField>--%>
                    <asp:TemplateField HeaderText="應備數">
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank"
                                NavigateUrl='<%# string.Format("WH_DNPREPARE_DETAIL_M3.aspx?AUFNR={0}&MAT_CAPTION_ORDER={1}", Eval("VBELN"), rdoEMP_NO.SelectedValue) %>'
                                Text='<%# DataBinder.Eval(Container.DataItem, "ITEMQTY")%>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ENMNGSITEMQTY" HeaderText="已備數"></asp:BoundField>
                    <asp:BoundField DataField="ENMNGSITEMQTY_NPT" HeaderText="異常數"></asp:BoundField>
                    <asp:BoundField DataField="OPENITEMQTY" HeaderText="未補數"></asp:BoundField>
                    <asp:BoundField DataField="OUTSTOCK" HeaderText="缺料數"></asp:BoundField>
                    <asp:TemplateField HeaderText="未退帳">
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink2" runat="server" Target="_blank"
                                NavigateUrl='<%# string.Format("WH_SKIN_WIP_M_M3.aspx?AUFNR={0}&POSNR=&TYPE=0", Eval("VBELN")) %>'
                                Text='<%# DataBinder.Eval(Container.DataItem, "SKIN_WIPQTY")%>'>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CFMShipping_DATE" HeaderText="出貨確效" DataFormatString="{0:MM-dd}"></asp:BoundField>
                    <asp:BoundField DataField="CFMShipping_USER" HeaderText="出貨人"></asp:BoundField>
                    <asp:BoundField DataField="CFMPacking_DATE" HeaderText="包裝確效" DataFormatString="{0:MM-dd}"></asp:BoundField>
                    <asp:BoundField DataField="CFMPacking_USER" HeaderText="包裝人"></asp:BoundField>
                    <asp:BoundField DataField="DN_PackingNote" HeaderText="出貨備註"></asp:BoundField>
                    <asp:BoundField DataField="WH_REMARK" HeaderText="倉庫備註"></asp:BoundField>
                    <asp:BoundField DataField="PRINTTYPE" HeaderText="列印"></asp:BoundField>
                    <%--        <asp:HyperLinkField DataNavigateUrlFields="DN_FileName" HeaderText="PDF檔" 
        Target="_blank" DataTextField="DN_FileName"  DataNavigateUrlFormatString="http://iwms.advantech.com.tw:999/DNFile/{0}">
        </asp:HyperLinkField>--%>
                    <asp:HyperLinkField DataNavigateUrlFields="VBELN" HeaderText="PDF檔"
                        Target="_blank" DataTextField="VBELN" DataNavigateUrlFormatString="DN_FileNamePRINT_M3.aspx?AUFNR={0}"></asp:HyperLinkField>
                    <asp:BoundField DataField="DN_CFC" HeaderText="開單人"></asp:BoundField>
                    <asp:BoundField DataField="CR_DATETIME" HeaderText="建立時間" DataFormatString="{0:MM-dd HH:mm}">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="MD_DATETIME" HeaderText="修改時間" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>

