<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WH_EFLOW_MAS_DETAIL.aspx.cs" Inherits="WH_EFLOW_MAS_DETAIL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView ID="gdv_WH_EFLOW_MASDATA" runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="1000"     AllowPaging="True" PageSize="100" OnRowDataBound="gdv_WH_EFLOW_MASDATA_RowDataBound" OnRowCommand="gdv_WH_EFLOW_MASDATA_RowCommand"   >
         <Columns>
            <asp:TemplateField HeaderText="已扣帳">
                    <ItemTemplate>
                   <asp:Button ID="btn_PostingD" runat="server"  CommandName="Posting_data" CausesValidation="False" 
                     Text="已扣帳"  
                       OnClientClick = "if (confirm('您確定要修改嗎?')==false) {return false;}"
                       Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
                          <asp:TemplateField HeaderText="備註修改">
                    <ItemTemplate>
                   <asp:Button ID="btn_EditD" runat="server"  CommandName="edit_data" CausesValidation="False" 
                     Text="修改"  
                       OnClientClick = "if (confirm('您確定要修改嗎?')==false) {return false;}"
                       Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
                </asp:TemplateField> 
        <asp:BoundField DataField="INSERT_TIME" HeaderText="開單時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
            SortExpression="INSERT_TIME" >
        </asp:BoundField>              
        <asp:BoundField DataField="MOVEMENT_TYPE" HeaderText="單別" 
            SortExpression="MOVEMENT_TYPE" >
        </asp:BoundField>
        <asp:BoundField DataField="NO" HeaderText="單號"
            SortExpression="NO" >
        </asp:BoundField>
      <asp:BoundField DataField="PLANT" HeaderText="廠別" 
            SortExpression="PLANT" >
        </asp:BoundField>  
        <asp:BoundField DataField="PK_NO" HeaderText="流水號"
            SortExpression="PK_NO" >
        </asp:BoundField>
        <asp:BoundField DataField="AUFNR" HeaderText="工單"
            SortExpression="AUFNR" >
        </asp:BoundField>
        <asp:BoundField DataField="MATNR" HeaderText="料號"
            SortExpression="MATNR" >
        </asp:BoundField>
        <asp:BoundField DataField="LABST" HeaderText="庫存" 
                SortExpression="LABST" >
        </asp:BoundField>
        <asp:BoundField DataField="LGPBE" HeaderText="生產儲位" 
           SortExpression="LGPBE" >
        </asp:BoundField>
        <asp:BoundField DataField="BDMNGS" HeaderText="需求數量" 
            SortExpression="BDMNGS" >
        </asp:BoundField>
<%--        <asp:BoundField DataField="ENMNGS" HeaderText="已處理數量" 
            SortExpression="ENMNGS" >
        </asp:BoundField>--%>
<asp:TemplateField HeaderText="已處理數量" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                         <asp:TextBox ID="txtENMNGS" Width="30px" Text='<%# DataBinder.Eval(Container.DataItem, "ENMNGS")%>' runat="server"  ></asp:TextBox>
                         </ItemTemplate>
                    <HeaderStyle Font-Size="12px"></HeaderStyle>
<ItemStyle HorizontalAlign="left"></ItemStyle>
                </asp:TemplateField>  
<%--        <asp:BoundField DataField="REASON" HeaderText="備註" 
            SortExpression="REASON" >
        </asp:BoundField>--%>
                   <asp:TemplateField HeaderText="備註" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                         <asp:TextBox ID="txtREASON" Width="300px" Text='<%# DataBinder.Eval(Container.DataItem, "REASON")%>' runat="server"  ></asp:TextBox>
                         </ItemTemplate>
                    <HeaderStyle Font-Size="12px"></HeaderStyle>
<ItemStyle HorizontalAlign="left"></ItemStyle>
                </asp:TemplateField>       
        <asp:BoundField DataField="APPLYINFO" HeaderText="開單人" 
            SortExpression="APPLYINFO" >
        </asp:BoundField>
<%--             <asp:BoundField DataField="POSTING" HeaderText="SAP過帳" 
            SortExpression="POSTING" >
        </asp:BoundField>--%>
 <asp:TemplateField HeaderText="SAP過帳" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
<asp:DropDownList ID="ddl_POSTING" runat="server" SelectedValue='<%# Bind("POSTING") %>' >
                <asp:ListItem>1</asp:ListItem>
                <asp:ListItem>0</asp:ListItem>
            </asp:DropDownList>
                         </ItemTemplate>
                    <HeaderStyle Font-Size="12px"></HeaderStyle>
<ItemStyle HorizontalAlign="left"></ItemStyle>
                </asp:TemplateField> 
 <asp:TemplateField HeaderText="STATUS" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
<asp:DropDownList ID="ddl_STATUS" runat="server" SelectedValue='<%# Bind("STATUS") %>'>
                <asp:ListItem>1</asp:ListItem>
                <asp:ListItem>0</asp:ListItem>
            </asp:DropDownList>
                         </ItemTemplate>
                    <HeaderStyle Font-Size="12px"></HeaderStyle>
<ItemStyle HorizontalAlign="left"></ItemStyle>
                </asp:TemplateField> 
        <asp:BoundField DataField="STATUSNAME" HeaderText="狀態" 
        SortExpression="STATUSNAME" >
        </asp:BoundField> 
        <asp:BoundField DataField="CONFIRM_TIME" HeaderText="領取時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
            SortExpression="CONFIRM_TIME" >
        </asp:BoundField>                           
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
    </div>
    </form>
</body>
</html>
