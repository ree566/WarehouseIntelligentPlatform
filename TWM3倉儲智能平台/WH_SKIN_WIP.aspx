<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WH_SKIN_WIP.aspx.cs" Inherits="WH_SKIN_WIP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
        <table border="1"   style="width: 429px; background-color: #E3FFDD;">
     <tr>
                <td>
                    <input type="submit" name="btn_MOVEMENT_TYPE" value="顯示類型" id="btn_MOVEMENT_TYPE" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                        <asp:DropDownList ID="cboMOVEMENT_TYPE" runat="server" Width="300"></asp:DropDownList>
                    </td>
            </tr>
         <tr>
                <td>
                    <input type="submit" name="btn_MATNR" value="搜尋料件" id="btn_MATNRWIP_NO" 
                        class="blue" style="width:90px;" />   
                </td>
                <td >
                    <asp:TextBox ID="txt_MATNR" runat="server" Width="150px" Height="25px" AutoPostBack="True"  autocomplete="off"  ></asp:TextBox>
                 <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                    </td>
            </tr>
        <tr>
                <td>
                    <input type="submit" name="btn_WIP_NO" value="搜尋工單" id="btn_WIP_NO"  
                        class="blue" style="width:90px;" />
                    
                </td>
                <td >
                    <asp:TextBox ID="txt_WIP_NO" runat="server" Width="150px" Height="25px" AutoPostBack="True" autocomplete="off"></asp:TextBox>
                 </td>
            </tr>
         <tr>
                <td>
                    <input type="submit" name="btn_EMPLR_ID" value="扣帳人員" id="btn_EMPLR_ID" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                    <asp:DropDownList ID="cboOWNER"  runat="server" Width="300"></asp:DropDownList>
                    </td>
            </tr>
        <tr>
                <td>
                    <input type="submit" name="btn_CONFIRMTYPE" value="顯示類型" id="btn_UNIT_NO" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                    <%--<asp:DropDownList ID="DropDownList1" runat="server" Width="300"></asp:DropDownList>--%>
                    <asp:RadioButtonList ID="rdoCONFIRMTYPE" runat="server" RepeatDirection="Horizontal">
                        <%--<asp:ListItem Selected="True" Value="ALL">ALL</asp:ListItem>--%>
                        <asp:ListItem  Value="A">ALL</asp:ListItem>
                        <asp:ListItem Selected="True" Value="F">已備料未扣帳</asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
            </tr>
        <tr>
                <td>
                    <input type="submit" name="btn_cr_date" value="日期" id="btn_cr_date" 
                        class="blue" style="width:90px;" /></td>
                <td >
                    <asp:textbox id="txtStartDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:textbox>
                    ~
                    <asp:textbox id="txtEndDate" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:textbox>
              
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

    <asp:GridView ID="gdv_WH_SKIN_WIP" runat="server" AllowSorting="True" AutoGenerateColumns="False" 
  style="font-size: small; margin-top: 0px;" 
            BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px" 
            CellPadding="3" ForeColor="Black" 
            Width="1000px"      OnRowDataBound="gdv_WH_SKIN_WIP_RowDataBound" OnRowCommand="gdv_WH_SKIN_WIP_RowCommand"   >
         <Columns>
            <asp:TemplateField HeaderText="已扣帳">
                    <ItemTemplate>
                   <asp:Button ID="btn_EditD" runat="server"  CommandName="edit_data" CausesValidation="False" 
                     Text="已扣帳"  
                       OnClientClick = "if (confirm('您確定要修改嗎?')==false) {return false;}"
                       Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="刪除">
                    <ItemTemplate>
                   <asp:Button ID="btn_delD" runat="server"  CommandName="delete_data" CausesValidation="False" 
                     Text="刪除"  
                       OnClientClick = "if (confirm('您確定要刪除嗎?')==false) {return false;}"
                       Width="50px" CssClass="blue" ForeColor="Red" BackColor="Yellow" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="left" Wrap="False"/>
                    <HeaderStyle HorizontalAlign="left" />
            </asp:TemplateField>              
        <asp:BoundField DataField="PK_NO" HeaderText="流水號"
            SortExpression="PK_NO" >
        </asp:BoundField>
        <asp:BoundField DataField="AUFNR" HeaderText="工單"
            SortExpression="AUFNR" >
             <FooterStyle Wrap="False" />
        </asp:BoundField>  
        <asp:BoundField DataField="MODEL" HeaderText="機種"
            SortExpression="MODEL" >
        </asp:BoundField>    
        <asp:BoundField DataField="MATNR" HeaderText="料號"
            SortExpression="MATNR" >
        </asp:BoundField>
        <asp:BoundField DataField="BDMNGS" HeaderText="工單需求數" 
            SortExpression="BDMNGS" >
        </asp:BoundField> 
        <asp:BoundField DataField="ENMNGS" HeaderText="已發料數" 
            SortExpression="ENMNGS" >
        </asp:BoundField> 
        <asp:BoundField DataField="PUB_QTY" HeaderText="扣帳數量" 
            SortExpression="PUB_QTY" >
        </asp:BoundField>                     
        <asp:BoundField DataField="NAME" HeaderText="單別" 
            SortExpression="NAME" >
        </asp:BoundField>
        <asp:BoundField DataField="REMOVE_STLOC" HeaderText="轉庫別" 
            SortExpression="REMOVE_STLOC" >
        </asp:BoundField>
        <asp:BoundField DataField="REMOVE_PLANT" HeaderText="轉廠別" 
            SortExpression="REMOVE_PLANT" >
        </asp:BoundField>
        <asp:BoundField DataField="EMP_NO" HeaderText="備料人員" 
            SortExpression="EMP_NO" >
        </asp:BoundField>
        <asp:BoundField DataField="MACHINE_NO" HeaderText="設備編號" 
            SortExpression="MACHINE_NO" >
        </asp:BoundField>
        <asp:BoundField DataField="ED_DATE" HeaderText="備料時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
            SortExpression="ED_DATE" >
        </asp:BoundField>
        <asp:BoundField DataField="EFLOW_MAS_PK_NO" HeaderText="超領流水號" 
            SortExpression="EFLOW_MAS_PK_NO" >
        </asp:BoundField>
        <asp:BoundField DataField="NO" HeaderText="超領表單號" 
            SortExpression="NO" >
        </asp:BoundField>
        <asp:BoundField DataField="TYPE" HeaderText="狀態" 
            SortExpression="TYPE" >
        </asp:BoundField>
        <asp:BoundField DataField="NOTE" HeaderText="備註" 
            SortExpression="NOTE" >
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
</asp:Content>



