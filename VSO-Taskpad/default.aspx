<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="VSO_Taskpad._default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Repeater runat="server" ID="rptProjects">
        <ItemTemplate>
            <p><asp:Label runat="server" Text='<%# Eval("name") %>' /> : 
                <asp:HyperLink text="GO" runat="server" NavigateUrl='<%# Eval("url") %>' />
            </p>

        </ItemTemplate>

    </asp:Repeater>

</asp:Content>
