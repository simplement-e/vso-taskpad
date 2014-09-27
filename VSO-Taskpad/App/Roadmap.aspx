<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="Roadmap.aspx.cs" Inherits="VSO_Taskpad.App.Roadmap" %>

<%@ Register Src="~/App/ProjectHeaderControl.ascx" TagPrefix="uc1" TagName="ProjectHeaderControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ProjectHeaderControl runat="server" ID="ProjectHeaderControl" />
    <article>
        <h1>
            <asp:Label runat="server" ID="lblTitre" /></h1>
    </article>

</asp:Content>
