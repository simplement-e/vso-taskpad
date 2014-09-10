<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="VSO_Taskpad._default" %>

<%@ Register Src="~/AccountHeaderControl.ascx" TagPrefix="uc1" TagName="AccountHeaderControl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:MultiView runat="server" ID="mvAuth">

        <asp:View runat="server" ID="vwNotAuth">

            <section class="mainLogin">


                <script type="text/javascript">
                    $(document).ready(function () {
                        $("#lnkLogin").click(function (e) {
                            $(".mainLogin a").attr("disabled", "disabled");
                            $(".mainLogin input").attr("disabled", "disabled");
                            $(".mainLogin button").attr("disabled", "disabled");
                            $.ajax({
                                url: "/api/auth/login", method: 'GET',
                                data: {
                                    login: $("#txtLogin").val(),
                                    pwd: $("#txtPassword").val()
                                }
                            }).done(function (result) {
                                if (result)
                                    document.location = "/";
                                else {
                                    toastr.error("Please check your credentials", "Unable to login", { "positionClass": "toast-top-full-width" });
                                    $(".mainLogin a").removeAttr("disabled");
                                    $(".mainLogin input").removeAttr("disabled");
                                    $(".mainLogin button").removeAttr("disabled");
                                    $("#txtPassword").focus();
                                }
                            }).fail(function (a, b, c) {
                                $(".mainLogin a").removeAttr("disabled");
                                $(".mainLogin input").removeAttr("disabled");
                                $(".mainLogin button").removeAttr("disabled");

                            }).always(function () {
                            });

                        });

                        $("#lnkCreate").click(function (e) {
                            $(".mainLogin a").attr("disabled", "disabled");
                            $(".mainLogin input").attr("disabled", "disabled");
                            $(".mainLogin button").attr("disabled", "disabled");

                            $.ajax({
                                url: "/api/auth/create", method: 'GET',
                                data: {
                                    name: $("#txtCreateName").val(),
                                    login: $("#txtCreateLogin").val(),
                                    pwd: $("#txtCreatePassword").val()
                                }
                            }).done(function (result) {
                                if (result)
                                    document.location = "/";
                                else {
                                    $(".mainLogin a").removeAttr("disabled");
                                    $(".mainLogin input").removeAttr("disabled");
                                    $(".mainLogin button").removeAttr("disabled");
                                }
                            }).fail(function (a, b, c) {
                                $(".mainLogin a").removeAttr("disabled");
                                $(".mainLogin input").removeAttr("disabled");
                                $(".mainLogin button").removeAttr("disabled");
                            }).always(function () {

                            });

                        });
                    });
                </script>

                <form class="login">
                    <h2>Login</h2>
                    <input type="email" id="txtLogin" placeholder="your email address" />
                    <input type="password" id="txtPassword" placeholder="your password" />
                    <button id="lnkLogin">Login</button>
                </form>

                <form class="createAccount" autocomplete="off">
                    <h2>Create an account</h2>
                    <input type="text" id="txtCreateName" autocomplete="off" placeholder="your name" />
                    <input type="email" id="txtCreateLogin" autocomplete="off" placeholder="your email address" />
                    <input style="display: none" />
                    <input type="password" id="txtCreatePassword" autocomplete="off" placeholder="your password" />
                    <button id="lnkCreate">Create account</button>
                </form>

            </section>
        </asp:View>

        <asp:View runat="server" ID="vwAuthNoVso">
            <a href="/oauth/vstudio/start/">VSO Authorize</a>
        </asp:View>

        <asp:View runat="server" ID="vwAuth">
            <uc1:AccountHeaderControl runat="server" id="AccountHeaderControl" />
        </asp:View>

    </asp:MultiView>

</asp:Content>
