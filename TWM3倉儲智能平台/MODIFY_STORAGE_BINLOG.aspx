<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MODIFY_STORAGE_BINLOG.aspx.cs" Inherits="MODIFY_STORAGE_BINLOG" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:GridView ID="gdv_MODIFY_STORAGE_BINLOG" runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="100%"    DataKeyNames="id" AllowPaging="True" PageSize="100" EnableSortingAndPagingCallbacks="True">
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
             <asp:BoundField DataField="WERKS" HeaderText="廠別" 
                SortExpression="WERKS" >
        </asp:BoundField>
             <asp:BoundField DataField="LGORT" HeaderText="庫位" 
                SortExpression="LGORT" >
        </asp:BoundField>
             <asp:BoundField DataField="STOCK_QTY" HeaderText="當時庫存" 
                SortExpression="STOCK_QTY" >
        </asp:BoundField>
                      <asp:BoundField DataField="LGPBE_OLD" HeaderText="原儲位" 
                SortExpression="LGPBE_OLD" >
        </asp:BoundField>
                     <asp:BoundField DataField="LGPBE_NEW" HeaderText="修改後儲位" 
                SortExpression="LGPBE_NEW" >
        </asp:BoundField>
       
             <asp:BoundField DataField="SAP_FLAG" HeaderText="修改成功" 
                SortExpression="SAP_FLAG" >
        </asp:BoundField>
        <asp:BoundField DataField="EMPLR_ID" HeaderText="修改人員" 
                SortExpression="EMPLR_ID" >
        </asp:BoundField>
        <asp:BoundField DataField="CR_DATETIME" HeaderText="修改時間" SortExpression="CR_DATETIME" DataFormatString="{0:MM-dd HH:mm}" />
         
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
