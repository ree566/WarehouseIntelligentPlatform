<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Calendar.aspx.cs" Inherits="Calendar_aspx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
 <form id="form1" runat="server">
     選取好日期點選兩下，視窗隨即關閉
    <asp:Calendar ID="Calendar1" runat="server" BackColor="#FFFFCC" 
        BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" 
        Font-Names="Verdana" Font-Size="8pt" ForeColor="#663399" Height="200px" 
        ShowGridLines="True" Width="220px">
        <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
        <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
        <OtherMonthDayStyle ForeColor="#CC9966" />
        <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
        <SelectorStyle BackColor="#FFCC66" />
        <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" 
            ForeColor="#FFFFCC" />
        <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
    </asp:Calendar>
    </form>
</body>
</html>
