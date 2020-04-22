<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WH_SKIN_WIP_M_M3.aspx.cs" Inherits="WH_SKIN_WIP_M" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TWM3智能檢料扣帳修改</title>
          <%--Bootstrap also supported.--%>
    <script src="bootstrap/js/jquery-3.4.1.slim.min.js" ></script>
    <script src="bootstrap/js/popper.min.js"></script>
     <script src="bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" media="screen" href="bootstrap/css/bootstrap.min.css" />
<%--    -----------------  --%>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script>	
            function CheckAllItem(Check)
            {
                elm = document.forms[0];  //取得form表單

                for (i = 0; i < elm.length; i++) 
                {
                  if (elm[i].type == "checkbox" && elm[i].id != Check.id) //若為checkbox，並且ID與表頭CheckBox不同。表示為明細的CheckBox
                  {
                    if (elm.elements[i].checked != Check.checked)  //若明細的CheckBox的checked狀態與表頭CheckBox不同
                    {
                    elm.elements[i].click();  //明細的CheckBox執行click
                    }
                  }
                }
            }
</script>
    <script type="text/javascript">
<!--
    //只可輸入整數,小數點
    function TextBoxNumCheck_number() {
        if ((event.keyCode < 48 || window.event.keyCode > 57 ) && window.event.keyCode != 46) event.returnValue = false;
    }
// -->
</script>
        <div class="card">
  <h5 class="card-header">智能檢料扣帳明細</h5>
  <div class="card-body">
<asp:GridView ID="gdv_WH_SKIN_WIP" runat="server" 
               CellPadding="3" AutoGenerateColumns="False"  OnRowDataBound="gdv_WH_SKIN_WIP_RowDataBound"
                CssClass="table table-striped">
               <Columns>
    <asp:TemplateField HeaderText="*" ItemStyle-BackColor="#f5f5f5" HeaderStyle-BackColor="#E2C2DE" ItemStyle-HorizontalAlign="Center">
    <HeaderStyle  Wrap="False" />
    <ItemStyle  Wrap="False" />
    <HeaderTemplate>
        <asp:CheckBox ID="CheckAllItem" runat="server" onclick="javascript: CheckAllItem(this);"  />
    </HeaderTemplate>
    <ItemTemplate>
        <asp:CheckBox ID="CheckBox" AutoCallBack="true" runat="server"></asp:CheckBox>
    </ItemTemplate>
</asp:TemplateField>
        <asp:BoundField DataField="PK_NO" HeaderText="ID">
        </asp:BoundField>
        <asp:BoundField DataField="AUFNR" HeaderText="單號">
        </asp:BoundField>
        <asp:BoundField DataField="MODEL" HeaderText="機種">
        </asp:BoundField>
         <asp:BoundField DataField="POSNR" HeaderText="POSNR">
        </asp:BoundField>     
         <asp:BoundField DataField="MATNR" HeaderText="料號">
        </asp:BoundField>                      
         <asp:BoundField DataField="BDMNGS" HeaderText="應發數量">
        </asp:BoundField>
         <asp:BoundField DataField="ENMNGS" HeaderText="已發數量">
        </asp:BoundField>
 <asp:TemplateField HeaderText="扣帳數量" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                         <asp:TextBox ID="txtPUB_QTY" Width="50px" Text='<%# DataBinder.Eval(Container.DataItem, "PUB_QTY")%>' runat="server" onkeypress="TextBoxNumCheck_number();" autocomplete="off" ></asp:TextBox>
                </ItemTemplate>
                    <HeaderStyle Font-Size="12px"></HeaderStyle>
<ItemStyle HorizontalAlign="left"></ItemStyle>
                </asp:TemplateField> 
        <asp:BoundField DataField="TYPE" HeaderText="狀態" 
            SortExpression="TYPE" >
        </asp:BoundField>                       
         <asp:BoundField DataField="NOTE" HeaderText="備註">
        </asp:BoundField>
        <asp:BoundField DataField="ED_DATE" HeaderText="備料時間" DataFormatString="{0:yyyy-MM-dd HH:mm}"
            SortExpression="ED_DATE" >
        </asp:BoundField>
        </Columns>
        </asp:GridView>
      <asp:Button ID="btn_UPDATE" runat="server" Text="確認修改發料數" class="btn btn-primary" OnClick="btn_UPDATE_Click"  />
  </div>
</div>
    </div>
    </form>
</body>
</html>
