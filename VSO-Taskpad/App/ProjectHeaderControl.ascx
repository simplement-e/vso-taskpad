<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectHeaderControl.ascx.cs" Inherits="VSO_Taskpad.App.ProjectHeaderControl" %>

<div role="navigation">
    <nav role="main">
        <ul>
            <li class="homeProject">
                <asp:HyperLink runat="server" ID="lnkHomeProject" NavigateUrl="./"><span></span></asp:HyperLink>
            </li>
            <li class="bugsProject">
                <asp:HyperLink runat="server" ID="lnkBugsProject" NavigateUrl="./Bugs"><span></span></asp:HyperLink>
            </li>

        </ul>

    </nav>
</div>
