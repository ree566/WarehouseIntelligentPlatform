<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
     <h2><%: Title %>.<asp:Login ID="userLogin" runat="server" BackColor="#E3EAEB" BorderColor="#E6E2D8" BorderPadding="4" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" OnAuthenticate="userLogin_Authenticate" TextLayout="TextOnTop">
         <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
         <LoginButtonStyle BackColor="White" BorderColor="#C5BBAF" BorderStyle="Solid" BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#1C5E55" />
         <TextBoxStyle Font-Size="0.8em" />
         <TitleTextStyle BackColor="#1C5E55" Font-Bold="True" Font-Size="0.9em" ForeColor="White" />
         </asp:Login>
     </h2>
    </asp:Content>

