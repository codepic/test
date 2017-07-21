<!--
 ============================================================
 THIS IS A MOCKED LOGIN PAGE FOR NEW DEVELOPERS TO CRITIQUE.
 THIS SAMPLE IS NOT INTENDED TO COMPILE AND IS MADE UP OF 2 FILES.
 ============================================================
 Things we are looking for:
   1) Bad practices
   2) Potential problems
   3) How it could be improved
-->


<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="Application.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <div class="no-tab-menu-padding-container">
        
        <asp:Literal ID="Literal_Announcement" runat="server"></asp:Literal>
        <asp:Panel ID="PanelLogin" runat="server" DefaultButton="Button_Login">

            <asp:Label ID="Label_FromPortalInfo" runat="server" Visible="False"></asp:Label>

            <table class="basicTableWithNoWidth" runat="server" id="logintable" width="350px">
                <tr>
                    <th><%=Resources.Localisation.Login_Title %></th>
                </tr>
                <tr>
                    <td>
                        <div class="main_login_box">
                            <br />
                            <asp:Label ID="LabelError" runat="server" ForeColor="Red" Text="Invalid Credentials!" 
                                Visible="False" EnableViewState="False"></asp:Label>
                            <table class="logintable" cellpadding="3">
                                <tr>
                                    <td align="right">
                                        <%=Resources.Localisation.Login_UserNameLabel %>:
                                    </td>
                                    <td style="padding-top: 10px;">
                                        <asp:TextBox ID="TextBox_UserName" runat="server" Width="170px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <%=Resources.Localisation.Login_PasswordLabel %>:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TextBox_Password" runat="server" TextMode="SingleLine" Width="170px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:Button ID="Button_Login" runat="server" Font-Bold="True" Text="<%$Resources:Localisation, Login_Signin %>" Width="100px" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:Panel ID="gpluslogin" runat="server" Visible="false">

                            <div class="secondary_login_box">
                                <div class="blue_ball_container">
                                    <div class="blue_ball">
                                        tai
                                    </div>
                                </div>
                                <div class="gmail_login_box">
                                    <h2><%=Resources.Localisation.Login_LoginwithGoogleTitle %></h2>
                                    <p>
                                        <%=Resources.Localisation.Login_LoginwithGoogleText %>
                                    </p>
                                    <div id="gConnect" runat="server">
                                        <button id="Button_Login2" runat="server" class="g-signin" data-scope="" data-requestvisibleactions="" data-clientid=""
                                            data-accesstype="offline" data-callback="onSignInCallback" data-theme="dark"
                                            data-cookiepolicy="single_host_origin">
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        <div style="clear: both;"></div>
                    </td>
                </tr>
            </table>

        </asp:Panel>

        <br />
        <br />
        <asp:LinkButton ID="LinkButtonUnohditkoSalasanan" runat="server" Visible="False"><%=Resources.Localisation.Login_ForgotpasswordTitle %></asp:LinkButton>
        <br />
        <br />
        <asp:Label
            ID="LabelUnohtunutSalasanaLahetetty" runat="server" ForeColor="Green" Text="<%$Resources:Localisation, Login_PasswordSentMessage %>"
            Visible="False" EnableViewState="False"></asp:Label>
        <br />
        <br />
        <asp:Label ID="LabelVaaratNimet"
            runat="server" ForeColor="Red" Text="<%$Resources:Localisation, Login_UserNameOrEmailNotFound %>"
            Visible="False" EnableViewState="False"></asp:Label><br />
        <asp:Panel ID="PanelUnohditkoSalasanan" runat="server" Visible="False">
            <br />
            <%=Resources.Localisation.Login_ForgotpasswordFormTitle %>
            <br />
            <br />
            <table class="basicTable">
                <tr>
                    <td class="header" colspan="2" style="height: 21px"><%=Resources.Localisation.Login_ForgottenPassword %></td>
                </tr>
                <tr>
                    <td><%=Resources.Localisation.General_YourEmail %>:</td>
                    <td>
                        <asp:TextBox ID="TextBoxUnohtunutSahkoposti" runat="server" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td><%=Resources.Localisation.General_FirstName %>:</td>
                    <td>
                        <asp:TextBox ID="TextBoxEtunimet" runat="server" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td><%=Resources.Localisation.General_LastName %>:</td>
                    <td>
                        <asp:TextBox ID="TextBoxSukunimet" runat="server" Width="200px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="ButtonLahetaUnohtunutSahkoposti" runat="server" Text="<%$Resources:Localisation, Login_SentPasswordToMyEmail %>" />
                    </td>
                </tr>
            </table>
            <br />
        </asp:Panel>

    </div>
    </form>
</body>
</html>
