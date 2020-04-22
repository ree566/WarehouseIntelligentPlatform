<%@ Page Title="" Language="C#" MasterPageFile="Site.master" AutoEventWireup="true" CodeFile="WH_DNNOZPRDPLCLIST_CFM_M3.aspx.cs" Inherits="WH_DNNOZPRDPLCLIST_CFM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script>
        function CheckAllItem(Check) {
            elm = document.forms[0];  //取得form表單

            for (i = 0; i < elm.length; i++) {
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="card">
        <h5 class="card-header">查詢</h5>
        <div class="card-body">

            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_AUFNR">DN單</span>
                </div>
                <asp:TextBox ID="txtAUFNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtAUFNR" runat="server"></asp:TextBox>
            </div>
            <div class="input-group mb-3">
                <div class="input-group-prepend">
                    <span class="input-group-text" id="lbl_MATNR">料號</span>
                </div>
                <asp:TextBox ID="txtMATNR" class="form-control" aria-label="Sizing example input" aria-describedby="lbl_txtMATNR" runat="server"></asp:TextBox>
            </div>
            <div class="form-check form-check-inline">
                <asp:RadioButtonList ID="rdoCFM" class="form-check-input" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="A">ALL</asp:ListItem>
                    <asp:ListItem Value="E" Selected="True">物管已維護</asp:ListItem>
                    <asp:ListItem Value="N">物管未維護</asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <asp:Button ID="btn_search" runat="server" Text="查詢" class="btn btn-primary" OnClick="btn_search_Click" />
        </div>
    </div>
    <div class="card">
        <h5 class="card-header">查詢結果</h5>
        <div class="card-body">
            <asp:GridView ID="gdv_WH_DNNOZPRDPLCLIST_CFM" runat="server"
                CellPadding="3" AutoGenerateColumns="False"
                CssClass="table table-striped">
                <Columns>
                    <asp:TemplateField HeaderText="更新" ItemStyle-BackColor="#f5f5f5" HeaderStyle-BackColor="#E2C2DE" ItemStyle-HorizontalAlign="Center">
                        <HeaderStyle Wrap="False" />
                        <ItemStyle Wrap="False" />
                        <HeaderTemplate>
                            <asp:CheckBox ID="CheckAllItem" runat="server" onclick="javascript: CheckAllItem(this);" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckBox" AutoCallBack="true" runat="server"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="VBELN" HeaderText="DN單"></asp:BoundField>
                    <asp:BoundField DataField="POSNR" HeaderText="POSNR"></asp:BoundField>
                    <asp:BoundField DataField="MATNR" HeaderText="料號"></asp:BoundField>
                    <asp:BoundField DataField="ZWERKS" HeaderText="廠別"></asp:BoundField>
                    <asp:BoundField DataField="ZPRDPLC" HeaderText="DN單產地"></asp:BoundField>
                    <asp:BoundField DataField="ZPRDPLC_SAP" HeaderText="物管維護"></asp:BoundField>
                    <asp:TemplateField HeaderText="更新產地" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="txtZPRDPLC_SAP_CFM" Width="100px" Text='<%# DataBinder.Eval(Container.DataItem, "ZPRDPLC_SAP")%>' runat="server" autocomplete="off"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Font-Size="12px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="left"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="btn_UPDATE" runat="server" Text="更新產地資訊" class="btn btn-primary" OnClick="btn_UPDATE_Click" />
        </div>
    </div>
</asp:Content>

