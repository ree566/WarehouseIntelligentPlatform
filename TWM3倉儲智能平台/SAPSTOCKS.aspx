<%@ Page Title="SAP料號庫存查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SAPSTOCKS.aspx.cs" Inherits="SAPSTOCKS" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
<!--
    //只可輸入整數,小數點
    function TextBoxNumCheck_number() {
        if ((event.keyCode < 48 || window.event.keyCode > 57) && window.event.keyCode != 46) event.returnValue = false;
    }
    // -->
    </script>
    <div class="card">
        <h5 class="card-header">查詢</h5>
        <div class="card-body">
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_EMPNO">確效工號</span>
                </div>
                <asp:TextBox ID="txt_EMPNO" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_EMPNO" runat="server"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_MATNR">搜尋料件</span>
                </div>
                <asp:TextBox ID="txt_MATNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_MATNR" runat="server" AutoPostBack="True" OnTextChanged="txt_MATNR_TextChanged" autocomplete="off"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_WERKS">廠　　別</span>
                </div>
                <%--<asp:TextBox ID="txt_WERKS" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_WERKS" runat="server" autocomplete="off" Text="TWM9"></asp:TextBox>--%>
                <div class="form-check form-check-inline">
                    <asp:RadioButtonList ID="rdoWERKS" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="TWM2">TWM2</asp:ListItem>
                        <asp:ListItem Selected="True" Value="TWM3">TWM3</asp:ListItem>
                        <asp:ListItem Value="TWM6">TWM6</asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="form-check form-check-inline">
                <asp:RadioButtonList ID="rdoLGORTTYPE" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="0">ALL</asp:ListItem>
                    <asp:ListItem Selected="True" Value="1">良品庫</asp:ListItem>
                    <asp:ListItem Value="2">非良品庫</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_MD04DAYS">需求天數</span>
                </div>
                <asp:TextBox ID="txtMD04DAYS" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_MD04DAYS" runat="server" autocomplete="off" Text="60"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <div class="col-md-4 mb-3">
                    <div class="input-group-prepend">
                        <span class="input-group-text" id="lbl_POSTDATE">過帳日期</span>
                    </div>
                    <asp:TextBox ID="txtStartDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>
                    ~
                    <asp:TextBox ID="txtEndDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <br>
    <table>
        <tr>
            <td>
                <asp:Button ID="btn_search" runat="server" Text="查詢" class="btn btn-primary" OnClick="btn_search_Click" />
            </td>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </tr>
    </table>
    <asp:GridView ID="gdv_SAPSTOCKS" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        Style="font-size: small; margin-top: 0px;"
        BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px"
        CellPadding="3" ForeColor="Black"
        Width="100%" AllowPaging="True" PageSize="100" OnRowCommand="gdv_SAPSTOCKS_RowCommand">
        <Columns>

            <asp:TemplateField HeaderText="修改">
                <ItemTemplate>
                    <asp:Button ID="btn_EditD" runat="server" CommandName="edit_data" CausesValidation="False"
                        Text="修改"
                        OnClientClick="if (confirm('您確定要修改嗎?')==false) {return false;}"
                        Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="left" Wrap="False" />
                <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="歷程">
                <ItemTemplate>
                    <asp:Button ID="btn_LOG" runat="server" CommandName="LOG_data"
                        Text="歷程" Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="left" Wrap="False" />
                <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="儲位" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:TextBox ID="txtLGPBE" Width="200px" Text='<%# DataBinder.Eval(Container.DataItem, "LGPBE")%>' runat="server" autocomplete="off" AutoPostBack="True"></asp:TextBox>
                    <%-- <script>
                        function textboxEnterKey(txt){
                         try{
                             if(event.keyCode == 13) return false;
                            }
                          catch(e){ }
                        }
                        </script>--%>
                </ItemTemplate>
                <HeaderStyle Font-Size="12px"></HeaderStyle>
                <ItemStyle HorizontalAlign="left"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="MATNR" HeaderText="物料"
                SortExpression="MATNR"></asp:BoundField>
            <asp:BoundField DataField="WERKS" HeaderText="廠別"
                SortExpression="WERKS"></asp:BoundField>
            <asp:BoundField DataField="LGORT" HeaderText="庫別"
                SortExpression="LGORT"></asp:BoundField>
            <asp:BoundField DataField="LABST" HeaderText="庫存"
                SortExpression="LABST"></asp:BoundField>
            <asp:BoundField DataField="ENTRY_QNT_TODAY" HeaderText="今日已上架數"
                SortExpression="ENTRY_QNT_TODAY"></asp:BoundField>
            <asp:BoundField DataField="ENTRY_QNT" HeaderText="(待上架)"
                SortExpression="ENTRY_QNT"></asp:BoundField>
            <asp:BoundField DataField="UNPULL_QNT" HeaderText="(未拉料)"
                SortExpression="UNPULL_QNT"></asp:BoundField>
            <asp:BoundField DataField="INSME" HeaderText="待驗"
                SortExpression="INSME"></asp:BoundField>
            <asp:BoundField DataField="SPEME" HeaderText="BLOCK"
                SortExpression="SPEME"></asp:BoundField>
            <asp:BoundField DataField="ReqQry" HeaderText="未來需求"
                SortExpression="ReqQry"></asp:BoundField>
            <asp:BoundField DataField="FeedQry" HeaderText="未來進料"
                SortExpression="FeedQry"></asp:BoundField>
            <asp:BoundField DataField="VENDOR" HeaderText="供應商"
                SortExpression="VENDOR"></asp:BoundField>
            <asp:BoundField DataField="MCNAME" HeaderText="MC OWNER"
                SortExpression="MCNAME"></asp:BoundField>
            <asp:HyperLinkField DataNavigateUrlFields="MATNR" HeaderText="備品"
                Target="_blank" DataTextField="PRETOTAL_STOCK" DataNavigateUrlFormatString="~/View_SKIN_BIN_CARD.aspx?MATNR={0}&LGORT=9999"
                SortExpression="PRETOTAL_STOCK"></asp:HyperLinkField>
            <asp:BoundField DataField="STD_COST" HeaderText="物料單價"
                SortExpression="STD_COST"></asp:BoundField>
        </Columns>
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BorderWidth="1px" BackColor="#6B696B" Font-Bold="True"
            ForeColor="White" />
        <PagerStyle BorderColor="#9999FF"
            BorderWidth="2px" BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Left"
            BorderStyle="Solid" />
        <RowStyle Wrap="False" BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
    </asp:GridView>

    <hr />

    <asp:GridView ID="gdv_SAPSTOCKS_MD04" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        Style="font-size: small; margin-top: 0px;"
        BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px"
        CellPadding="3" ForeColor="Black"
        Width="1000" AllowPaging="True" PageSize="100" OnRowDataBound="gdv_SAPSTOCKS_MD04_RowDataBound">
        <Columns>
            <asp:BoundField DataField="AVAIL_DATE" HeaderText="日期"
                SortExpression="AVAIL_DATE"></asp:BoundField>
            <asp:BoundField DataField="PLUS_MINUS" HeaderText="出入庫"
                SortExpression="PLUS_MINUS"></asp:BoundField>
            <asp:BoundField DataField="REC_REQD_QTY" HeaderText="數量"
                SortExpression="LABST"></asp:BoundField>

            <%--             <asp:BoundField DataField="SHORTAGE_QTY" HeaderText="剩餘庫存" 
                SortExpression="SHORTAGE_QTY" >
        </asp:BoundField>--%>
            <asp:BoundField DataField="MRP_NO12" HeaderText="工單"
                SortExpression="MRP_N0O12"></asp:BoundField>
            <asp:BoundField DataField="PEGGEDRQMT" HeaderText="機種"
                SortExpression="PEGGEDRQMT"></asp:BoundField>
            <asp:BoundField DataField="MRP_NO" HeaderText="MRP_NO"
                SortExpression="MRP_NO"></asp:BoundField>
            <asp:BoundField DataField="PROD_PLANT" HeaderText="PROD_PLANT"
                SortExpression="PROD_PLANT"></asp:BoundField>
        </Columns>
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BorderWidth="1px" BackColor="#6B696B" Font-Bold="True"
            ForeColor="White" />
        <PagerStyle BorderColor="#9999FF"
            BorderWidth="2px" BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Left"
            BorderStyle="Solid" />
        <RowStyle Wrap="False" BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
    </asp:GridView>
    <hr />

    <asp:GridView ID="gdv_GOODSMVT_GETITEMS" runat="server" AllowSorting="True"
        Style="font-size: small; margin-top: 0px;"
        BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px"
        CellPadding="3" ForeColor="Black"
        Width="100%" EnableSortingAndPagingCallbacks="True" OnRowDataBound="gdv_GOODSMVT_GETITEMS_RowDataBound">
        <FooterStyle BackColor="#CCCC99" />
        <HeaderStyle BorderWidth="1px" BackColor="#6B696B" Font-Bold="True"
            ForeColor="White" />
        <PagerStyle BorderColor="#9999FF"
            BorderWidth="2px" BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Left"
            BorderStyle="Solid" />
        <RowStyle Wrap="False" BackColor="#F7F7DE" />
        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        <SortedAscendingCellStyle BackColor="#FBFBF2" />
        <SortedAscendingHeaderStyle BackColor="#848384" />
        <SortedDescendingCellStyle BackColor="#EAEAD3" />
        <SortedDescendingHeaderStyle BackColor="#575357" />
    </asp:GridView>


</asp:Content>

