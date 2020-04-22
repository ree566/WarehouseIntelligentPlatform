<%@ Page Title="PDA庫存查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="SAPSTOCKS_M.aspx.cs" Inherits="SAPSTOCKS_M" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
<!--
    //只可輸入整數,小數點
    function TextBoxNumCheck_number() {
        if ((event.keyCode < 48 || window.event.keyCode > 57) && window.event.keyCode != 46) event.returnValue = false;
    }
    // -->
    </script>
    <script type="text/javascript">
<!--
    //只可輸入整數
    function TextBoxNumCheck_Int() {
        if ((event.keyCode < 48 || window.event.keyCode > 57) && e.KeyChar != (char)('-')) event.returnValue = false;
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
        </div>
    </div>
    <br />
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="btn_search" runat="server" Text="查詢" Style="width: 110px;" OnClick="btn_search_Click" UseSubmitBehavior="true" />--%>
                 <asp:Button ID="btn_search" runat="server" Text="查詢" class="btn btn-primary" OnClick="btn_search_Click"　 />
            </td>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </tr>
    </table>

    <asp:Label ID="lblimplementrate" runat="server" Font-Size="Large" ForeColor="Red"></asp:Label>
    <asp:ListView ID="ListView1" runat="server" OnItemCommand="ListView1_ItemCommand" OnItemDataBound="ListView1_ItemDataBound">
        <AlternatingItemTemplate>
            <td runat="server" style="background-color: #000073;">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btn_EditD" runat="server" CommandName="edit_data" CausesValidation="False"
                        Text="修改"
                        OnClientClick="if (confirm('您確定要修改嗎?')==false) {return false;}"
                        Width="70px" CssClass="blue" ForeColor="Red" BackColor="Yellow" Height="40px" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   <asp:Button ID="btn_LOG" runat="server" CommandName="LOG_data"
                       Text="歷程" Width="70px" CssClass="blue" ForeColor="Red" BackColor="Yellow" Height="40px" />
                <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">儲位:</td>
                        <td align="left" style="font-size: 40px;">
                            <asp:TextBox ID="txtLGPBE" class="form-control"  Text='<%# DataBinder.Eval(Container.DataItem, "LGPBE")%>' runat="server" AutoPostBack="True" autocomplete="off" Font-Size="X-Large" ></asp:TextBox>
                                </td>
                    
                         
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">物料:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_MATNR" runat="server" Text='<%# Eval("MATNR") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">廠別:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_WERKS" runat="server" Text='<%# Eval("WERKS") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">庫別:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_LGORT" runat="server" Text='<%# Eval("LGORT") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">庫存:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_LABST" runat="server" Text='<%# Eval("LABST") %>' /></td>
                    </tr>

                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">今日已上架數:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_ENTRY_QNT_TODAY" runat="server" Text='<%# Eval("ENTRY_QNT_TODAY") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">未來需求:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_ReqQry" runat="server" Text='<%# Eval("ReqQry") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">未來進料:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_FeedQry" runat="server" Text='<%# Eval("FeedQry") %>' /></td>
                    </tr>

                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">(待上架):</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_ENTRY_QNT" runat="server" Text='<%# Eval("ENTRY_QNT") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">(未拉料):</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_UNPULL_QNT" runat="server" Text='<%# Eval("UNPULL_QNT") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">待驗:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lblINSME" runat="server" Text='<%# Eval("INSME") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">BLOCK:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_SPEME" runat="server" Text='<%# Eval("SPEME") %>' /></td>
                    </tr>
                    <tr>
                        <td style="width: 120px; font-size: 20px; text-align: left;">備品:</td>
                        <td align="left" style="font-size: 20px;">
                            <asp:Label ID="lbl_PRETOTAL_STOCK" runat="server" Text='<%# Eval("PRETOTAL_STOCK") %>' /></td>
                    </tr>
                </table>
            </td>
        </AlternatingItemTemplate>

        <GroupTemplate>
            <tr id="itemPlaceholderContainer" runat="server">
                <td id="itemPlaceholder" runat="server"></td>
            </tr>
        </GroupTemplate>
        <ItemTemplate>
            <td runat="server" style="background-color: #000073;">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btn_EditD" runat="server" CommandName="edit_data" CausesValidation="False"
                                            Text="修改"
                                            OnClientClick="if (confirm('您確定要修改嗎?')==false) {return false;}"
                                            Width="70px" CssClass="blue" ForeColor="Red" BackColor="Yellow" Height="40px" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                   <asp:Button ID="btn_LOG" runat="server" CommandName="LOG_data"
                       Text="歷程" Width="70px" CssClass="blue" ForeColor="Red" BackColor="Yellow" Height="40px" />
                <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                    <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">儲位:</td>
                        <td align="left" style="font-size: 40px;">
                            <asp:TextBox ID="txtLGPBE" class="form-control"  Text='<%# DataBinder.Eval(Container.DataItem, "LGPBE")%>' runat="server"  autocomplete="off" Font-Size="X-Large"  AutoPostBack="True"></asp:TextBox>
                                </td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">物料:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_MATNR" runat="server" Text='<%# Eval("MATNR") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">廠別:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_WERKS" runat="server" Text='<%# Eval("WERKS") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">庫別:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_LGORT" runat="server" Text='<%# Eval("LGORT") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">庫存:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_LABST" runat="server" Text='<%# Eval("LABST") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">今日已上架數:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_ENTRY_QNT_TODAY" runat="server" Text='<%# Eval("ENTRY_QNT_TODAY") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">未來需求:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_ReqQry" runat="server" Text='<%# Eval("ReqQry") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">未來進料:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_FeedQry" runat="server" Text='<%# Eval("FeedQry") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">(待上架):</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_ENTRY_QNT" runat="server" Text='<%# Eval("ENTRY_QNT") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">(未拉料):</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_UNPULL_QNT" runat="server" Text='<%# Eval("UNPULL_QNT") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">待驗:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lblINSME" runat="server" Text='<%# Eval("INSME") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">BLOCK:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_SPEME" runat="server" Text='<%# Eval("SPEME") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">備品:</td>
                            <td align="left" style="font-size: 20px;">
                                <asp:Label ID="lbl_PRETOTAL_STOCK" runat="server" Text='<%# Eval("PRETOTAL_STOCK") %>' /></td>
                        </tr>
                    </table>
            </td>
        </ItemTemplate>
        <LayoutTemplate>
            <table runat="server">
                <tr runat="server">
                    <td runat="server">
                        <table id="groupPlaceholderContainer" runat="server" border="1" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px; font-family: Verdana, Arial, Helvetica, sans-serif;">
                            <tr id="groupPlaceholder" runat="server">
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server">
                    <td runat="server" style="text-align: center; background-color: #CCCCCC; font-family: Verdana, Arial, Helvetica, sans-serif; color: #000000;"></td>
                </tr>
            </table>
        </LayoutTemplate>
        <SelectedItemTemplate>
        </SelectedItemTemplate>
        
    </asp:ListView>
<%= Session["name"] %>
</asp:Content>

