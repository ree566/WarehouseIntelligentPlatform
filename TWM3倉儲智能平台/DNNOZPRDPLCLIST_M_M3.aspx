<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DNNOZPRDPLCLIST_M_M3.aspx.cs" Inherits="DNNOZPRDPLCLIST_M" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
              <%--Bootstrap also supported.--%>
    <script src="bootstrap/js/jquery-3.4.1.slim.min.js" ></script>
    <script src="bootstrap/js/popper.min.js"></script>
     <script src="bootstrap/js/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" media="screen" href="bootstrap/css/bootstrap.min.css" />
<%--    -----------------  --%>
</head>
<body>
    <form id="form1" runat="server">
<div class="card">
  <h5 class="card-header">Featured</h5>
  <div class="card-body">
<div class="form-group">
    <div class="input-group mb-3">
  <div class="input-group-prepend">
    <span class="input-group-text" id="lbl_MATNR">料號</span>
  </div>
<asp:TextBox ID="txtMATNR" class="form-control" aria-label="Sizing example input" placeholder="料號" aria-describedby="lbl_txtMATNR" runat="server" ReadOnly="true"></asp:TextBox>
</div>
    <div class="input-group mb-3">
  <div class="input-group-prepend">
    <span class="input-group-text" id="lbl_ZWERKS">廠別</span>
  </div>
<asp:TextBox ID="txtZWERKS" class="form-control" aria-label="Sizing example input" placeholder="廠別" aria-describedby="lbl_ZWERKS" runat="server" ReadOnly="true"></asp:TextBox>
</div>
    <div class="input-group mb-3">
  <div class="input-group-prepend">
    <span class="input-group-text" id="lbl_NAME1">供應商</span>
  </div>
<asp:TextBox ID="txtNAME1" class="form-control" aria-label="Sizing example input" placeholder="廠別" aria-describedby="lbl_NAME1" runat="server" ReadOnly="true"></asp:TextBox>
</div>
    <div class="input-group mb-3">
  <div class="input-group-prepend">
    <span class="input-group-text" id="lbl_MC_NAME">料件物管</span>
  </div>
<asp:TextBox ID="txtMC_NAME" class="form-control" aria-label="Sizing example input" placeholder="料件物管" aria-describedby="lbl_MC_NAME" runat="server" ReadOnly="true"></asp:TextBox>
</div>
    <div class="input-group mb-3">
  <div class="input-group-prepend">
    <span class="input-group-text" id="lbl_ZPRDPLC_SAP">料件產地</span>
  </div>
<asp:TextBox ID="txtZPRDPLC_SAP" class="form-control"  aria-label="Sizing example input" placeholder="產地只允許輸入英文" aria-describedby="lbl_ZPRDPLC_SAP" runat="server" onkeyup="this.value=this.value.replace(/[^\w_]/g,'');"></asp:TextBox>
</div>
</div>
 </div>
</div>
<asp:Button ID="btn_UPDATE" runat="server" Text="維護"  class="btn btn-primary" OnClick="btn_UPDATE_Click"  />
    </form>

</body>
</html>
