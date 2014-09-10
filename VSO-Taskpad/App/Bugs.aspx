<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="Bugs.aspx.cs" Inherits="VSO_Taskpad.App.Bugs" %>

<%@ Register Src="~/App/ProjectHeaderControl.ascx" TagPrefix="uc1" TagName="ProjectHeaderControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ProjectHeaderControl runat="server" id="ProjectHeaderControl" />
    <h2>Bugs</h2>
    <asp:Label runat="server" ID="lblProject" />
</asp:Content>
