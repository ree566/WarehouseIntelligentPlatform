<%@ Page Title="倉庫上架看板" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="WH_STORAGE.aspx.cs" Inherits="WH_STORAGE" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
     <table border="1"   style="width: 429px; background-color: #E3FFDD;">
     <tr>
                <td>
                    <input type="submit" name="btn_WIP_NO" value="搜尋料件" id="btn_WIP_NO" 
                        class="blue" style="width:90px;" />
                    
                </td>
                <td >
                    <asp:TextBox ID="txt_MATNR" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>
                 <%--   <asp:DropDownList ID="ddl_WIP_NO" runat="server" Width="300"></asp:DropDownList>--%>
                    可進行模糊比對
                    </td>
            </tr>
         <tr>
                <td>
                    <input type="submit" name="btn_EMPLR_ID" value="備料人員" id="btn_EMPLR_ID" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                    <asp:DropDownList ID="cboOWNER" runat="server" Width="300"></asp:DropDownList>
                    </td>
            </tr>
          <tr>
                <td>
                    <input type="submit" name="btn_Vendor" value="供應商" id="btn_Vendor" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                    <asp:DropDownList ID="ddl_Vendor" runat="server" Width="300"></asp:DropDownList>
                   <%-- <asp:RadioButtonList ID="rdoTYPE" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Selected="True" Value="1">已備料</asp:ListItem>
                        <asp:ListItem Value="2">未發料</asp:ListItem>
                    </asp:RadioButtonList>--%>
                    </td>
            </tr>
                  <tr>
                <td>
                    <input type="submit" name="btn_ORDERBY" value="排序方式" id="btn_ORDERBY" 
                        class="blue" style="width:90px;" />
                </td>
                <td >
                    <%--<asp:TextBox ID="txt_EMPLR_ID" runat="server" Width="150px" Height="25px" AutoPostBack="True"></asp:TextBox>--%>
                    <asp:DropDownList ID="ddl_ORDERBY" runat="server" Width="300">
                        <asp:ListItem Value="-1">進貨日期,人員,料號</asp:ListItem>
                        <asp:ListItem Value="0">供應商,進貨日期,料號</asp:ListItem>
                        <asp:ListItem Value="2">供應商,料號,進貨日期</asp:ListItem>
                        <asp:ListItem Value="3">上架時間</asp:ListItem>
                        <asp:ListItem Value="1">料號,進貨日期</asp:ListItem>
                    </asp:DropDownList>
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
                        <asp:ListItem  Value="-1">ALL</asp:ListItem>
                        <asp:ListItem Selected="True" Value="0">待上架</asp:ListItem>
                        <asp:ListItem  Value="1">已上架</asp:ListItem>
                        <asp:ListItem  Value="2">已刪除</asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
            </tr>
        <tr>
                <td>
                    <input type="submit" name="btn_cr_date" value="上架日期" id="btn_cr_date" 
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
        <div  style="height:auto; vertical-align:middle; text-align: center;">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names= "Verdana"  
            Font-Size= "8pt" style="height:auto;width:auto" SizeToReportContent="true" InteractivityPostBackMode="AlwaysSynchronous" ></rsweb:ReportViewer>
    </div>
         <asp:Timer ID="Timer1" runat="server" Interval="120000" OnTick="Timer1_Tick">
    </asp:Timer>
</asp:Content>

