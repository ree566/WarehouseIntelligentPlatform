<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DN_FileNamePRINT_M3.aspx.cs" Inherits="DN_FileNamePRINT" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <script type="text/javascript">
            function doPrint() {
                var prtContent = document.getElementById('<%= ReportViewer1.ClientID %>');
        prtContent.border = 0; //set no border here
        var WinPrint = window.open('', '', 'left=150,top=100,width=1000,height=1000,toolbar=0,scrollbars=1,status=0,resizable=1');
        WinPrint.document.write(prtContent.outerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        WinPrint.close();
    }
   </script>   
 <asp:Button ID="PrintButton" runat="server" Text="Print" OnClientClick="doPrint();" ToolTip="Print Report"/>
        <div  style="height:auto; vertical-align:middle; text-align: center;">
            <asp:Image ID="Image1" runat="server" />
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names= "Verdana"  ConsumeConteinerWhitespace = "True"
            Font-Size= "8pt" style="height:auto;width:auto" SizeToReportContent="true" InteractivityPostBackMode="AlwaysSynchronous" OnDrillthrough="ReportViewer1_Drillthrough" AsyncRendering="False"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
