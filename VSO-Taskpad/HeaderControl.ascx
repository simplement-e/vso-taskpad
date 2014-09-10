<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderControl.ascx.cs" Inherits="VSO_Taskpad.HeaderControl" %>

<header role="main">
    <div class="login">
        <asp:Label runat="server" ID="lnkUserName" /></div>
    <ul>
        <li class="home">
            <asp:HyperLink runat="server" ID="lnkHome" NavigateUrl="~/"><span>Home</span></asp:HyperLink>
        </li>
        <asp:Repeater runat="server" ID="rptProjects">
            <ItemTemplate>
                <li class="project">
                    <asp:HyperLink runat="server"
                        CssClass='<%# Eval("css")%>'
                        NavigateUrl='<%# Eval("name", "~/app/{0}/")%>'><asp:Label runat="server" Text='<%# Eval("name")%>'></asp:Label></asp:HyperLink>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>

</header>

