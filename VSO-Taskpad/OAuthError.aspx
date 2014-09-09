<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="OAuthError.aspx.cs" Inherits="VSO_Taskpad.OAuthError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:MultiView runat="server" ID="mvError">
        <asp:View runat="server" ID="vwGeneric">
            <h1>Something went very wrong</h1>
            <p>And I would really be happy if I'd known what it was and how it failed...</p>
        </asp:View>
        <asp:View runat="server" ID="vwNoGrant">
            <h1>Access not granted</h1>
        </asp:View>
    </asp:MultiView>

</asp:Content>
