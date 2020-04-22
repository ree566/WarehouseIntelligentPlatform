<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WH_EFLOW_MASCONFIRM_M.aspx.cs" Inherits="WH_EFLOW_MASCONFIRM_M" %>


<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <table border="1"   style="width: 429px; background-color: #E3FFDD;">
     <tr>
                <td>
                    <input type="submit" name="btn_MOVEMENT_TYPE" value="顯示類型" id="btn_MOVEMENT_TYPE" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <asp:RadioButtonList ID="rdoMOVEMENT_TYPE" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Value="261">領料單</asp:ListItem>
                        <asp:ListItem Value="262">退料單</asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
            </tr>
        <tr>
                <td>
                    <input type="submit" name="btn_WIP_NO" value="搜尋單號" id="btn_WIP_NO" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <asp:TextBox ID="txt_EFLOW_NO" runat="server" Width="150px" Height="25px" AutoPostBack="True" ></asp:TextBox>
                    </td>
            </tr>
<tr>
                <td>
                    <input type="submit" name="btn_MATNR" value="搜尋料件" id="btn_MATNRWIP_NO" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <asp:TextBox ID="txt_MATNR" runat="server" Width="297px" Height="31px" AutoPostBack="True"  autocomplete="off" Font-Size="XX-Large"  ></asp:TextBox>
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
                        <asp:ListItem Selected="True" Value="A">未領取</asp:ListItem>
                        <asp:ListItem Value="F">已領取</asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
            </tr>
     </table>
    <br/>
             <table >
            <tr>
                <td>
                  <asp:Button ID="btn_search" runat="server" Text="查詢" style="width:110px; " /> </td>
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
            </tr>
        </table>
    <asp:ListView ID="ListView1" runat="server"  GroupItemCount="2" >
            <AlternatingItemTemplate>
               <td runat="server" style="background-color:#000073;">
                     <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">位置:</td>
                             <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_locname" runat="server" Text='<%# Eval("位置") %>' /></td>
                        </tr>                        
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">物料:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_MATNR" runat="server" Text='<%# Eval("物料") %>' /></td>
                        </tr>
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">工位:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_WORKSTATION" runat="server" Text='<%# Eval("工位") %>' /></td>
                        </tr>
                         <tr>
                            <td style="width: 120px; font-size: 20px;text-align: left;">應領:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_BDMNGS" runat="server" Text='<%# Eval("需求") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px;text-align: left;">已發:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_ENMNGS" runat="server" Text='<%# Eval("已扣") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px;text-align: left;">確效日期:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_WH_CFMDATE" runat="server" Text='<%# Eval("WH_CFMDATE", "{0:MM-dd HH:mm}")  %>' /></td>
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
                <td runat="server" style="background-color:#000073;">
                     <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                     <table style="text-align: center; color: White; font-family: Verdana; font-weight: bold;">
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">位置:</td>
                             <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_locname" runat="server" Text='<%# Eval("位置") %>' /></td>
                        </tr>                        
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">物料:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_MATNR" runat="server" Text='<%# Eval("物料") %>' /></td>
                        </tr>
                         <tr>
                            <td style="width: 120px; font-size: 20px; text-align: left;">工位:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_WORKSTATION" runat="server" Text='<%# Eval("工位") %>' /></td>
                        </tr>
                         <tr>
                            <td style="width: 120px; font-size: 20px;text-align: left;">應領:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_BDMNGS" runat="server" Text='<%# Eval("需求") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-Xsize: 20px;text-align: left;">已發:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_ENMNGS" runat="server" Text='<%# Eval("已扣") %>' /></td>
                        </tr>
                        <tr>
                            <td style="width: 120px; font-size: 20px;text-align: left;">確效日期:</td>
                            <td align="left" style="font-size: 20px;"><asp:Label ID="lbl_WH_CFMDATE" runat="server" Text='<%# Eval("WH_CFMDATE", "{0:MM-dd HH:mm}")  %>' /></td>
                        </tr>
                    </table>
                  </td>
            </ItemTemplate>
            <LayoutTemplate>
                <table runat="server">
                    <tr runat="server">
                        <td runat="server">
                            <table id="groupPlaceholderContainer" runat="server" border="1" style="background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;">
                                <tr id="groupPlaceholder" runat="server">
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server">
                        <td runat="server" style="text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;"></td>
                    </tr>
                </table>
            </LayoutTemplate>
            <SelectedItemTemplate>
            </SelectedItemTemplate>
         </asp:ListView>
</asp:Content>

