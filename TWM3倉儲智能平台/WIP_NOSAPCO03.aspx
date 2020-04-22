<%@ Page Title="SAP工單查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WIP_NOSAPCO03.aspx.cs" Inherits="WIP_NOSAPCO03" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
        <div>
     <table border="1"   style="width: 429px; background-color: #E3FFDD;">
     <tr>
                <td>
                    <input type="submit" name="btn_WIP_NO" value="搜尋工單" id="btn_WIP_NO" 
                        class="blue" style="width:90px;" />
                    
                </td>
                <td >
                    <asp:TextBox ID="txt_WIP_NO" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>
                 <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                    </td>
            </tr>

     </table>
      <br>
             <table >
            <tr>
                <td>
                  <asp:Button ID="btn_search" runat="server" Text="查詢" style="width:110px; " OnClick="btn_search_Click"/> </td>
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            </tr>
        </table>
            <asp:GridView ID="gdv_WOSTATUS"  runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="100%"      EnableSortingAndPagingCallbacks="True">
            <Columns>
             <asp:BoundField DataField="AUFNR" HeaderText="訂單" 
                SortExpression="AUFNR" >
        </asp:BoundField>
             <asp:BoundField DataField="AFART" HeaderText="AFART" 
                SortExpression="AFART" >
        </asp:BoundField>
        <asp:BoundField DataField="WERKS" HeaderText="廠別" 
                SortExpression="WERKS" >
        </asp:BoundField>
             <asp:BoundField DataField="MATNR" HeaderText="需求溯源" 
                SortExpression="MATNR" >
        </asp:BoundField>
             <asp:BoundField DataField="GSMNG" HeaderText="工單數" 
                SortExpression="GSMNG" >
        </asp:BoundField>
             <asp:BoundField DataField="WEMNG" HeaderText="已入庫數" 
                SortExpression="WEMNG" >
        </asp:BoundField>
             <asp:BoundField DataField="OPENQTY" HeaderText="未結數" 
                SortExpression="OPENQTY" >
        </asp:BoundField>
             <asp:BoundField DataField="GSTRP" HeaderText="STARTDATE" 
                SortExpression="GSTRP" >
        </asp:BoundField>
             <asp:BoundField DataField="GLTRP" HeaderText="FINISHDATE" 
                SortExpression="GLTRP" >
        </asp:BoundField>
             <asp:BoundField DataField="MAKTX" HeaderText="機種描述" 
                SortExpression="MAKTX" >
        </asp:BoundField>
             <asp:BoundField DataField="STATUS" HeaderText="STATUS" 
                SortExpression="STATUS" >
        </asp:BoundField>
             <asp:BoundField DataField="LBL_MADESC" HeaderText="工單狀態" 
                SortExpression="LBL_MADESC" >
        </asp:BoundField>
             <asp:BoundField DataField="MODEL_NM" HeaderText="MODEL_NM" 
                SortExpression="MODEL_NM" >
        </asp:BoundField>
             <asp:BoundField DataField="ZLONG_TEXT" HeaderText="ZLONG_TEXT" 
                SortExpression="ZLONG_TEXT" >
        </asp:BoundField>
             <asp:BoundField DataField="ERNAM" HeaderText="生管負責人" 
                SortExpression="ERNAM" >
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
    <asp:GridView ID="gdv_WIP_NOSAPCO03" runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="100%"     AllowPaging="True" PageSize="100" EnableSortingAndPagingCallbacks="True">
         <Columns>
<%--                          <asp:TemplateField HeaderText="明細">
                    <ItemTemplate>
                   <asp:Button ID="btn_EditD" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"pk_no")%>' CommandName="edit_data" 
                     Text="明細"  Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
                </asp:TemplateField>  --%>
             <asp:BoundField DataField="AUFNR" HeaderText="訂單" 
                SortExpression="AUFNR" >
        </asp:BoundField>
             <asp:BoundField DataField="BAUGR" HeaderText="需求溯源" 
                SortExpression="BAUGR" >
        </asp:BoundField>
             <asp:BoundField DataField="MATNR" HeaderText="物料" 
                SortExpression="MATNR" >
        </asp:BoundField>
                          <asp:BoundField DataField="MAKTX" HeaderText="物料描述" 
                SortExpression="MAKTX" >
        </asp:BoundField>
                      <asp:BoundField DataField="WERKS" HeaderText="廠別" 
                SortExpression="WERKS" >
        </asp:BoundField>
                  <asp:BoundField DataField="LGORT" HeaderText="庫別" 
                SortExpression="LGORT" >
        </asp:BoundField>    
             <asp:BoundField DataField="POSNR" HeaderText="POSNR" 
                SortExpression="POSNR" >
        </asp:BoundField>
                     <asp:BoundField DataField="LABST" HeaderText="庫存" 
                SortExpression="LABST" >
        </asp:BoundField>
<%--             <asp:BoundField DataField="ENTRY_QNT" HeaderText="(待上架)" 
                SortExpression="ENTRY_QNT" >
        </asp:BoundField>--%>
                     <asp:BoundField DataField="INSME" HeaderText="待驗" 
                SortExpression="INSME" >
        </asp:BoundField>
        <asp:BoundField DataField="BDMNG" HeaderText="需求數量" 
                SortExpression="BDMNG" >
        </asp:BoundField>
<%--           <asp:BoundField DataField="ENMNG" HeaderText="領料數量" 
                SortExpression="ENMNG" >
        </asp:BoundField>--%>
          <asp:BoundField DataField="OPEN_QTY" HeaderText="未發數量" SortExpression="OPEN_QTY" >
        </asp:BoundField>
             <asp:BoundField DataField="GSTRP" HeaderText="StartDate" SortExpression="SAP_MdDate" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
             <asp:BoundField DataField="LGPBE" HeaderText="儲位" SortExpression="LGPBE" />
             <asp:BoundField DataField="MEMO" HeaderText="MEMO" SortExpression="MEMO" />
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
</asp:Content>

