<%@ Page Title="" Language="C#" MasterPageFile="~/Taskpad.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="VSO_Taskpad._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:MultiView runat="server" ID="mvAuth">

        <asp:View runat="server" ID="vwNotAuth">

            <section class="mainLogin">


                <script type="text/javascript">
                    $(document).ready(function () {
                        $("#lnkLogin").click(function (e) {
                            $("#txtLogin").attr("disabled", "disabled");
                            $("#lnkLogin").attr("disabled", "disabled");
                            $("#txtPassword").attr("disabled", "disabled");
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
                                    $("#txtPassword").focus();
                                }
                            }).fail(function (a, b, c) {

                            }).always(function () {
                                $("#txtLogin").removeAttr("disabled");
                                $("#lnkLogin").removeAttr("disabled");
                                $("#txtPassword").removeAttr("disabled");
                            });

                        });

                        $("#lnkCreate").click(function (e) {
                            $("#txtCreateName").attr("disabled", "disabled");
                            $("#txtCreateLogin").attr("disabled", "disabled");
                            $("#txtCreatePassword").attr("disabled", "disabled");
                            $("#lnkCreate").attr("disabled", "disabled");

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

                                }
                            }).fail(function (a, b, c) {

                            }).always(function () {
                                $("#txtCreateName").removeAttr("disabled");
                                $("#txtCreateLogin").removeAttr("disabled");
                                $("#txtCreatePassword").removeAttr("disabled");
                                $("#lnkCreate").removeAttr("disabled");

                            });

                        });
                    });
                </script>

                <form class="login">
                    <h2>Login</h2>
                    <input type="text" id="txtLogin" placeholder="your email address" />
                    <input type="password" id="txtPassword" placeholder="your password" />
                    <button id="lnkLogin">Login</button>
                </form>

                <form class="createAccount">
                    <h2>Create an account</h2>
                    <input type="text" id="txtCreateName" placeholder="your name" />
                    <input type="text" id="txtCreateLogin" placeholder="your email address" />
                    <input type="password" id="txtCreatePassword" placeholder="your password" />
                    <button id="lnkCreate">Create account</button>
                </form>

            </section>
        </asp:View>

        <asp:View runat="server" ID="vwAuthNoVso">
            <a href="/oauth/vstudio/start/">VSO Authorize</a>
        </asp:View>

                <asp:View runat="server" ID="vwAuth">
                    Tout !
        </asp:View>

    </asp:MultiView>

</asp:Content>
