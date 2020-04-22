﻿<%@ Control Language="C#" CodeFile="GridViewPager.ascx.cs" Inherits="GridViewPager" %>

<div class="DDPager">
    <span class="DDFloatLeft">
        <asp:ImageButton AlternateText="第一頁" ToolTip="第一頁" ID="ImageButtonFirst" runat="server" ImageUrl="Images/PgFirst.gif" Width="8" Height="9" CommandName="Page" CommandArgument="First" />
        &nbsp;
        <asp:ImageButton AlternateText="上一頁" ToolTip="上一頁" ID="ImageButtonPrev" runat="server" ImageUrl="Images/PgPrev.gif" Width="5" Height="9" CommandName="Page" CommandArgument="Prev" />
        &nbsp;
        <asp:Label ID="LabelPage" runat="server" Text="頁 " AssociatedControlID="TextBoxPage" />
        <asp:TextBox ID="TextBoxPage" runat="server" Columns="5" AutoPostBack="true" ontextchanged="TextBoxPage_TextChanged" Width="20px" CssClass="DDControl" />
        /
        <asp:Label ID="LabelNumberOfPages" runat="server" />
        &nbsp;
        <asp:ImageButton AlternateText="下一頁" ToolTip="下一頁" ID="ImageButtonNext" runat="server" ImageUrl="Images/PgNext.gif" Width="5" Height="9" CommandName="Page" CommandArgument="Next" />
        &nbsp;
        <asp:ImageButton AlternateText="最後一頁" ToolTip="最後一頁" ID="ImageButtonLast" runat="server" ImageUrl="Images/PgLast.gif" Width="8" Height="9" CommandName="Page" CommandArgument="Last" />
    </span>
    <span class="DDFloatRight">
        <asp:Label ID="LabelRows" runat="server" Text="每頁結果數:" AssociatedControlID="DropDownListPageSize" />
        <asp:DropDownList ID="DropDownListPageSize" runat="server" AutoPostBack="true" CssClass="DDControl" onselectedindexchanged="DropDownListPageSize_SelectedIndexChanged">
            <asp:ListItem Value="5" />
            <asp:ListItem Value="10" />
            <asp:ListItem Value="15" />
            <asp:ListItem Value="20" />
        </asp:DropDownList>
    </span>
</div>

