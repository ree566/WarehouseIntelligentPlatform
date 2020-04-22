<%@ Page Language="C#" AutoEventWireup="true" CodeFile="View_SKIN_BIN_CARD.aspx.cs" Inherits="View_SKIN_BIN_CARD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:GridView ID="gdv_SKIN_BIN_CARD" runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="100%"    AllowPaging="True" PageSize="100" EnableSortingAndPagingCallbacks="True">
         <Columns>
<%--                          <asp:TemplateField HeaderText="明細">
                    <ItemTemplate>
                   <asp:Button ID="btn_EditD" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"pk_no")%>' CommandName="edit_data" 
                     Text="明細"  Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
                </asp:TemplateField>  --%>
             <asp:BoundField DataField="MATNR" HeaderText="料號" 
                SortExpression="MATNR" >
        </asp:BoundField>
             <asp:BoundField DataField="LGORT" HeaderText="庫位" 
                SortExpression="LGORT" >
        </asp:BoundField>
                      <asp:BoundField DataField="STORLOC_BIN" HeaderText="儲位" 
                SortExpression="STORLOC_BIN" >
        </asp:BoundField>
                     <asp:BoundField DataField="DESCRIPTION" HeaderText="描述" 
                SortExpression="DESCRIPTION" >
        </asp:BoundField>
       
             <asp:BoundField DataField="IN_STOCK" HeaderText="入庫" 
                SortExpression="IN_STOCK" >
        </asp:BoundField>
              <asp:BoundField DataField="OUT_STOCK" HeaderText="出庫" 
                SortExpression="OUT_STOCK" >
        </asp:BoundField>
                           <asp:BoundField DataField="TOTAL_STOCK" HeaderText="結存" 
                SortExpression="TOTAL_STOCK" >
        </asp:BoundField>
                                        <asp:BoundField DataField="EMP_NAME" HeaderText="人員" 
                SortExpression="EMP_NAME" >
        </asp:BoundField>
             <asp:BoundField DataField="TYPE" HeaderText="類型" 
                SortExpression="TYPE" >
        </asp:BoundField>
             <asp:BoundField DataField="NOTE" HeaderText="備註" 
                SortExpression="NOTE" >
        </asp:BoundField>
             <asp:BoundField DataField="CR_DATE" HeaderText="建立時間" SortExpression="CR_DATE" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
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
