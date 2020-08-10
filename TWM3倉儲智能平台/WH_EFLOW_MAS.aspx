<%@ Page Title="e-Flow領退料看板" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WH_EFLOW_MAS.aspx.cs" Inherits="WH_EFLOW_MAS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <table>
        <tr>
            <td>開立時間</td>
            <td>領取時間</td>
        </tr>
        <tr>
            <td>當天10點前</td>
            <td>下午1點</td>
        </tr>
        <tr>
            <td>當天10點後14點前</td>
            <td>當天17:30</td>
        </tr>
        <tr>
            <td>當天14點後17點前</td>
            <td>隔日10點</td>
        </tr>
        <tr>
            <td>當天17點後</td>
            <td>下午1點</td>
        </tr>

    </table>
    <h1>點選單號可單獨查詢明細</h1>
    <table border="1" style="width: 429px; background-color: #E3FFDD;">
        <tr>
            <td>
                <input type="submit" name="btn_MOVEMENT_TYPE" value="顯示類型" id="btn_MOVEMENT_TYPE"
                    class="blue" style="width: 90px;" />
            </td>
            <td>
                <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                <%--<asp:DropDownList ID="DropDownList1" runat="server" Width="300"></asp:DropDownList>--%>
                <asp:RadioButtonList ID="rdoMOVEMENT_TYPE" runat="server" RepeatDirection="Horizontal">
                    <%--<asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>--%>
                    <asp:ListItem Selected="True" Value="261">領料單</asp:ListItem>
                    <%--<asp:ListItem Value="262">退料單</asp:ListItem>--%>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_MATNR" value="搜尋料件" id="btn_MATNRWIP_NO"
                    class="blue" style="width: 90px;" />

            </td>
            <td>
                <asp:TextBox ID="txt_MATNR" runat="server" Width="150px" Height="25px" AutoPostBack="True" autocomplete="off"></asp:TextBox>
                <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_WIP_NO" value="搜尋單號" id="btn_WIP_NO"
                    class="blue" style="width: 90px;" />

            </td>
            <td>
                <asp:TextBox ID="txt_EFLOW_NO" runat="server" Width="150px" Height="25px" AutoPostBack="True" OnTextChanged="txt_EFLOW_NO_TextChanged"></asp:TextBox>
                <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                    可針對已關單進行搜尋</td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_EMPLR_ID" value="備料人員" id="btn_EMPLR_ID"
                    class="blue" style="width: 90px;" />
            </td>
            <td>

                <asp:DropDownList ID="cboOWNER" runat="server" Width="300"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_PLANT" value="廠別" id="btn_PLANT"
                    class="blue" style="width: 90px;" />
            </td>
            <td>
                <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                <asp:DropDownList ID="cbo_PLANT" runat="server" Width="300">
                    <asp:ListItem Selected="True">ALL</asp:ListItem>
                    <asp:ListItem>TWM2</asp:ListItem>
                    <asp:ListItem>TWM3</asp:ListItem>
                    <asp:ListItem>TWM6</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_CONFIRMTYPE" value="顯示類型" id="btn_UNIT_NO"
                    class="blue" style="width: 90px;" />
            </td>
            <td>
                <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                <%--<asp:DropDownList ID="DropDownList1" runat="server" Width="300"></asp:DropDownList>--%>
                <asp:RadioButtonList ID="rdoCONFIRMTYPE" runat="server" RepeatDirection="Horizontal">
                    <%--<asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>--%>
                    <asp:ListItem Selected="True" Value="A">未領取</asp:ListItem>
                    <asp:ListItem Value="F">已領取</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <input type="submit" name="btn_cr_date" value="日期" id="btn_cr_date"
                    class="blue" style="width: 90px;" /></td>
            <td>
                <asp:TextBox ID="txtStartDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>
                ~
                    <asp:TextBox ID="txtEndDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>

            </td>
        </tr>
    </table>
    <br>
    <table>
        <tr>
            <td>
                <asp:Button ID="btn_search" runat="server" Text="查詢" Style="width: 110px;" OnClick="btn_search_Click" />
            </td>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
        </tr>
    </table>
    <div style="height: auto; vertical-align: middle; text-align: center;">
        <h2>
            <asp:Label ID="lbl_MOVEMENT_TYPE" runat="server" ForeColor="Red"></asp:Label></h2>
        <asp:GridView ID="gdv_WH_EFLOW_MAS" runat="server" AllowSorting="True" AutoGenerateColumns="False"
            Style="font-size: small; margin-top: 0px;"
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px"
            CellPadding="3" ForeColor="Black"
            Width="1000" OnRowDataBound="gdv_WH_EFLOW_MAS_RowDataBound">
            <Columns>
                <asp:BoundField DataField="C_DATE" HeaderText="開單時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                    SortExpression="C_DATE"></asp:BoundField>
                <asp:BoundField DataField="PLANT" HeaderText="廠別"
                    SortExpression="PLANT"></asp:BoundField>
                <asp:BoundField DataField="MOVEMENT_TYPE" HeaderText="單別"
                    SortExpression="MOVEMENT_TYPE"></asp:BoundField>
                <asp:HyperLinkField DataNavigateUrlFields="NO,OWNER" HeaderText="單號"
                    Target="_blank" DataTextField="NO" DataNavigateUrlFormatString="~/WH_EFLOW_MAS_DETAIL.aspx?NO={0}&OWNER={1}"
                    SortExpression="NO"></asp:HyperLinkField>
                <%--        <asp:BoundField DataField="NO" HeaderText="單號"
            SortExpression="NO" >
        </asp:BoundField>--%>
                <asp:BoundField DataField="BDMBGSQTY" HeaderText="總筆數"
                    SortExpression="BDMBGSQTY"></asp:BoundField>
                <asp:BoundField DataField="OPENQTY" HeaderText="未結筆數"
                    SortExpression="OPENQTY"></asp:BoundField>
                <asp:BoundField DataField="NOCONFIRMQTY" HeaderText="未領筆數"
                    SortExpression="NOCONFIRMQTY"></asp:BoundField>
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
        <table border="1" style="width: 429px; background-color: #E3FFDD;">
            <tr>
                <td>
                    <input type="submit" name="btn_EMPNO" value="確效工號" id="btn_EMPNO"
                        class="blue" style="width: 90px;" />

                </td>
                <td>
                    <asp:TextBox ID="txt_EMPNO" runat="server" Width="150px" Height="25px" AutoPostBack="True" autocomplete="off"></asp:TextBox>
                    <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                </td>
            </tr>
            <tr>
                <td>
                    <input type="submit" name="btn_REASON" value="確效備註" id="btn_REASON"
                        class="blue" style="width: 90px;" />

                </td>
                <td>
                    <asp:TextBox ID="txt_REASON" runat="server" Width="150px" Height="25px" AutoPostBack="True" autocomplete="off" ViewState="False"></asp:TextBox>
                    <%--<asp:TextBox ID="TextBox1" runat="server" Width="150px" Height="25px" AutoPostBack="True" OnTextChanged="txt_MATNR_TextChanged" autocomplete="off"  ></asp:TextBox>--%>
                    <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                   
                </td>
            </tr>
            <tr>
                <td>
                    <input type="submit" name="btn_PK_NO" value="確效流水號" id="btn_PK_NO"
                        class="blue" style="width: 90px;" />

                </td>
                <td>
                    <asp:TextBox ID="txt_PK_NO" runat="server" Width="150px" Height="25px" AutoPostBack="True" OnTextChanged="txt_PK_NO_TextChanged" autocomplete="off" ViewState="False"></asp:TextBox>
                    <%--<asp:TextBox ID="TextBox1" runat="server" Width="150px" Height="25px" AutoPostBack="True" OnTextChanged="txt_MATNR_TextChanged" autocomplete="off"  ></asp:TextBox>--%>
                    <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                </td>
            </tr>


        </table>
        <h1>製造確認明細</h1>
    </div>
    <asp:GridView ID="gdv_WH_EFLOW_MASDATA" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        Style="font-size: small; margin-top: 0px;"
        BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px"
        CellPadding="3" ForeColor="Black"
        Width="1000px" OnRowDataBound="gdv_WH_EFLOW_MASDATA_RowDataBound" OnRowCommand="gdv_WH_EFLOW_MASDATA_RowCommand">
        <Columns>
            <%--        <asp:BoundField DataField="NO" HeaderText="單號"
            SortExpression="NO" >
        </asp:BoundField>--%>
            <%--        <asp:BoundField DataField="ENMNGS" HeaderText="已處理數量" 
            SortExpression="ENMNGS" >
        </asp:BoundField>--%>
            <%--        <asp:BoundField DataField="REASON" HeaderText="備註" 
            SortExpression="REASON" >
        </asp:BoundField>--%>
            <%--             <asp:BoundField DataField="POSTING" HeaderText="SAP過帳" 
            SortExpression="POSTING" >
        </asp:BoundField>--%>
            <asp:BoundField DataField="INSERT_TIME" HeaderText="開單時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                SortExpression="INSERT_TIME"></asp:BoundField>
            <asp:BoundField DataField="MOVEMENT_TYPE" HeaderText="單別"
                SortExpression="MOVEMENT_TYPE"></asp:BoundField>
            <asp:HyperLinkField DataNavigateUrlFields="NO,OWNER" HeaderText="單號"
                Target="_blank" DataTextField="NO" DataNavigateUrlFormatString="~/WH_EFLOW_MAS_DETAIL.aspx?NO={0}&OWNER={1}"
                SortExpression="NO"></asp:HyperLinkField>
            <asp:BoundField DataField="PLANT" HeaderText="廠別"
                SortExpression="PLANT"></asp:BoundField>
            <asp:BoundField DataField="PK_NO" HeaderText="流水號"
                SortExpression="PK_NO"></asp:BoundField>
            <asp:BoundField DataField="AUFNR" HeaderText="工單"
                SortExpression="AUFNR"></asp:BoundField>
            <asp:BoundField DataField="MATNR" HeaderText="料號"
                SortExpression="MATNR"></asp:BoundField>
            <asp:BoundField DataField="LABST" HeaderText="庫存"
                SortExpression="LABST"></asp:BoundField>
            <asp:BoundField DataField="LGPBE" HeaderText="生產儲位"
                SortExpression="LGPBE"></asp:BoundField>
            <asp:BoundField DataField="BDMNGS" HeaderText="需求數量"
                SortExpression="BDMNGS"></asp:BoundField>
            <asp:TemplateField HeaderText="已處理數量" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:TextBox ID="txtENMNGS" Width="30px" Text='<%# DataBinder.Eval(Container.DataItem, "ENMNGS")%>' runat="server"></asp:TextBox>
                </ItemTemplate>
                <HeaderStyle Font-Size="12px"></HeaderStyle>
                <ItemStyle HorizontalAlign="left"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="備註" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:TextBox ID="txtREASON" Width="300px" Text='<%# DataBinder.Eval(Container.DataItem, "REASON")%>' runat="server"></asp:TextBox>
                </ItemTemplate>
                <HeaderStyle Font-Size="12px"></HeaderStyle>
                <ItemStyle HorizontalAlign="left"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="APPLYINFO" HeaderText="開單人"
                SortExpression="APPLYINFO"></asp:BoundField>
            <asp:TemplateField HeaderText="SAP過帳" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:DropDownList ID="ddl_POSTING" runat="server" SelectedValue='<%# Bind("POSTING") %>'>
                        <asp:ListItem Value="1">OK</asp:ListItem>
                        <asp:ListItem Value="0">NA</asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
                <HeaderStyle Font-Size="12px"></HeaderStyle>
                <ItemStyle HorizontalAlign="left"></ItemStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="STATUS" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:DropDownList ID="ddl_STATUS" runat="server" SelectedValue='<%# Bind("STATUS") %>'>
                        <asp:ListItem Value="1">OK</asp:ListItem>
                        <asp:ListItem Value="0">NA</asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
                <HeaderStyle Font-Size="12px"></HeaderStyle>
                <ItemStyle HorizontalAlign="left"></ItemStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="STATUSNAME" HeaderText="狀態"
                SortExpression="STATUSNAME"></asp:BoundField>
            <asp:BoundField DataField="CONFIRM_TIME" HeaderText="領取時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
                SortExpression="CONFIRM_TIME"></asp:BoundField>
            <asp:TemplateField HeaderText="備註修改">
                <ItemTemplate>
                    <asp:Button ID="btn_EditD" runat="server" CommandName="edit_data" CausesValidation="False"
                        Text="修改"
                        OnClientClick="if (confirm('您確定要修改嗎?')==false) {return false;}"
                        Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="left" Wrap="False" />
                <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="補扣帳">
                <ItemTemplate>
                    <asp:Button ID="btn_PostingD" runat="server" CommandName="Posting_data" CausesValidation="False"
                        Text="補扣帳"
                        OnClientClick="if (confirm('您確定要修改嗎?')==false) {return false;}"
                        Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="left" Wrap="False" />
                <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
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
    <asp:Timer ID="Timer1" runat="server" Interval="300000" OnTick="Timer1_Tick">
    </asp:Timer>
</asp:Content>

