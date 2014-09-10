<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="Backlog.aspx.cs" Inherits="VSO_Taskpad.App.Backlog" %>

<%@ Register Src="~/App/ProjectHeaderControl.ascx" TagPrefix="uc1" TagName="ProjectHeaderControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ProjectHeaderControl runat="server" id="ProjectHeaderControl" />
    
    <h2>Backlog</h2>
    <asp:Label runat="server" ID="lblProject" />
     <asp:Repeater runat="server" ID="rptProjects">
        <ItemTemplate>
            <p><asp:Label runat="server" Text='<%# Eval("label") %>' /> <%--: <asp:Label runat="server" Text='<%# Eval("id") %>' />--%>
                <%--<asp:HyperLink text="GO" runat="server" NavigateUrl='<%# Eval("url") %>' />--%>
            </p>

        </ItemTemplate>

    </asp:Repeater>
</asp:Content>
