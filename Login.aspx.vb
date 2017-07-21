' ============================================================
' THIS IS A MOCKED LOGIN PAGE FOR NEW DEVELOPERS TO CRITIQUE.
' THIS SAMPLE IS NOT INTENDED TO COMPILE AND IS MADE UP OF 2 FILES.
' ============================================================
' Things we are looking for:
'   1) Bad practices
'   2) Potential problems
'   3) How it could be improved
   
Imports System.Web.Script.Serialization
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Drawing
Imports System.BinaryReader
Imports WRMData
Imports WRMData.Enumerations
Imports WSPData

Partial Class Login
    Inherits Page

    Private userDal = New User()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '****** Where DB is a globel variable holding database context
        If DB.Settings.GetFeatureSettings.ContainsKey(Tyoteho.Common.WRM.Feature.GoogleLogin) Then
            gpluslogin.Visible = True
            logintable.Width = 750 'put more width to the login table for google login to fit in.
            Button_Login.Attributes.Add("onclick", " remove_google_login();")

            'if we get access token from google, it means we should try to log in a user.
            If Not String.IsNullOrEmpty(Request.QueryString("accessToken")) Then
                'let's send an http-request to Google+ API using the token          
                Dim json As String = GetGoogleUserJSON2(Request.QueryString("accessToken"))
                'and Deserialize the JSON response
                Dim js As New JavaScriptSerializer()
                Dim oUser As GoogleEmail = js.Deserialize(Of GoogleEmail)(json)

                'if oUser is not object, we didn't get the information from google. This should happen with bad tokens.
                If TypeOf oUser Is Object Then
                    RevokeUser(Request.QueryString("accessToken"))
                    Dim iUsers As UserDataTable = userDal.GetUserByEmail(oUser.data.email, DeploymentID)
                    If iUsers.Rows.Count = 1 Then
                        Dim iUser As IUser = iUsers.First
                        If Not iUser Is Nothing Then
                            Dim WRMUser As UserRow
                            WRMUser = userDal.GetUserByADID(iUser.ADID, DeploymentID)
                            If WRMUser Is Nothing Then
                                RejectLogin()
                            End If
                            Session.Add("UserId", User.ID)
                            Session.Add("UserFullName", iUser.Name.ToString())
                            Session.Add(NeptonGeneralTools.SharedConstants.SessionKey_iUser, iUser)
                            Session("UserADID") = iUser.ADID
                            DB.User = User
                            SetCulture()
                            Me.Login()
                        Else
                            RejectLogin(Resources.Localisation.Login_InvalidCredentialMessage)
                        End If

                    Else
                        If iUsers.Rows.Count = 0 Then
                            Session("message") = Resources.Localisation.Login_GmailUserDoesNotExist
                        Else
                            Session("message") = Resources.Localisation.Login_GmailAccountIsLinkedWithMoreThanOneUser
                        End If
                    End If
                Else
                    Session("message") = Resources.Localisation.Login_GmailLoginAgainMessage
                End If
            End If
        End If

        If Not Session("message") Is Nothing Then
            LabelError.Text = Session("message")
            LabelError.Visible = True
            Session.Remove("message")
        Else
            LabelError.Visible = False
        End If

        If Request.Params("from") = "portal" Then
            ' if we come from portal
            Session("InsidePortal") = "yes"
            If String.IsNullOrEmpty(Request.Params.Item("ctl00$ContentPlaceHolder1$TextBox_UserName")) Then
                'no automatic login if username and pass is not set in req params
                Me.Label_FromPortalInfo.Text = Resources.Localisation.Login_ProtectionPasswordRequiredMessage
                Me.Label_FromPortalInfo.Visible = True
                Me.Label_FromPortalInfo.CssClass = "celectusRedInfoLabel"
                Me.TextBox_Password.Text = Me.TextBox_Password.Text
                Me.TextBox_UserName.Text = Me.TextBox_UserName.Text
            Else
                Me.Label_FromPortalInfo.Visible = False
                If IsPostBack = False Then
                    TryAutomaticLogin()
                    CheckLoginParams()
                End If
            End If
        Else
            ' if not from portal, automatic login is possible when no postback
            If IsPostBack = False Then
                TryAutomaticLogin()
                CheckLoginParams()
            End If
        End If
        Me.Form.DefaultFocus = Me.TextBox_UserName.ClientID
    End Sub

    Private Function TryAutomaticLogin()
        Dim UserName As String
        Dim TimeStamp As String
        Dim DeploymentID As String
        Dim PasswordHash As String
        Dim AutoLogin As String

        UserName = Request.QueryString("login_u")
        TimeStamp = Request.Params.Item("login_t")
        DeploymentID = Request.Params.Item("login_d")
        PasswordHash = Request.Params.Item("login_h")
        AutoLogin = Request.Params.Item("login_a")


        If String.IsNullOrEmpty(UserName) Or String.IsNullOrEmpty(TimeStamp) Or String.IsNullOrEmpty(DeploymentID) Or String.IsNullOrEmpty(PasswordHash) Or String.IsNullOrEmpty(AutoLogin) Then
            Exit Function
        End If
        If AutoLogin = "true" Then
            If NeptonGeneralTools.SharedLogin.ValidateAutomaticLogin(UserName, TimeStamp, CInt(DeploymentID), PasswordHash) Then
                Dim iUser As IUser
                iUser = userDal.GetUserByUserName(UserName, DeploymentID)

                If Not iUser Is Nothing Then
                    Dim User As UserRow
                    User = userDal.GetUserByADID(iUser.ADID, DeploymentID)
                    ' Response.Write(ADUser.FirstName & " " & ADUser.LastName & " " & ADUser.SamAccountName & " " & ADUser.FullName & " " & ADUser.ADID)
                    If User Is Nothing Then
                        '' AD User not found -> Create new user
                        'DB.Database.InsertUser(iUser.ADID, "", "", DeploymentID)
                        'User = DB.Database.GetUserByADID(iUser.ADID, DeploymentID)
                        RejectLogin() 'Failsafe, should never happen
                    End If

                    Session.Add("UserID", User.ID)
                    Session.Add("UserFullName", iUser.Name.ToString())
                    Session.Add(NeptonGeneralTools.SharedConstants.SessionKey_iUser, iUser)
                    Session("UserADID") = iUser.ADID
                    Me.Login()
                Else
                    RejectLogin()
                End If
            End If
        End If
    End Function

    Private Sub CheckLoginParams()
        ' this is for the automatic login, if not postback or from portal
        Dim UserName As String
        Dim Password As String
        Dim DeploymentID As Integer

        DeploymentID = Urls.Login.GetDeploymentID()
        UserName = Request.Params.Item("ctl00$ContentPlaceHolder1$TextBox_UserName")
        Password = Request.Params.Item("ctl00$ContentPlaceHolder1$TextBox_Password")

        If String.IsNullOrEmpty(UserName) Then
            Exit Sub
        End If
        If String.IsNullOrEmpty(Password) Then
            Throw New Exception("Error in login process. Password was empty")
        End If
        If DeploymentID > 0 Then
            Session("DeploymentID") = DeploymentID
            Cookies.DeploymentId.Value = DeploymentID
        Else
            Throw New Exception("Error in login process. DeploymentID was " & DeploymentID)
        End If

        Me.TryLogin(UserName, Password)
    End Sub


    Private Sub TryLogin(ByVal UserName As String, ByVal Password As String)
        Dim DeploymentID As Integer = Session("DeploymentID")
        Dim iUser As IUser

        If DB Is Nothing Then
            Throw New Exception("DB is nothing")
        End If
        If DB.Database Is Nothing Then
            Throw New Exception("DB database is nothing")
        End If

        iUser = userDal.GetUserByUserNameAndPassword(UserName, Password, DeploymentID)
        If iUser Is Nothing Then
            'if no user was found, check if user wants to log in with email instead.
            iUser = userDal.GetUserByEmailAndPassword(UserName, Password, DeploymentID)
        End If

        If Not iUser Is Nothing Then
            Dim User As UserRow
            User = userDal.GetUserByADID(iUser.ADID, DeploymentID)

            If User Is Nothing Then
                ''' AD User not found -> Create new user
                'DB.Database.InsertUser(iUser.ADID, "", "", DeploymentID)
                'User = DB.Database.GetUserByADID(iUser.ADID, DeploymentID)
                RejectLogin() 'Failsafe, should never happen
            End If

            'get the newly logged in user's db setting
            Session.Add("UserID", User.ID)
            Session.Add("UserFullName", User.Name.ToString())
            Session.Add(NeptonGeneralTools.SharedConstants.SessionKey_iUser, iUser)
            Session("UserADID") = iUser.ADID
            DB.User = User
            Me.Session("SessionKey_Data") = DB 'To store the logged user info.
            Me.Login()
        Else
            RejectLogin(Resources.Localisation.Login_InvalidCredentialMessage)
        End If
    End Sub

    Private Sub Login()
        Me.Response.Redirect(Urls.User_Default.Url)
    End Sub

    Public Sub RejectLogin(Optional ByVal ErrorMessage As String = "")
        If Not ErrorMessage = "" Then
            Session("message") = ErrorMessage
        End If
        Me.Response.Redirect(Urls.Login.Url)
    End Sub

    Protected Sub Button_Login_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button_Login.Click
        Me.TryLogin(Me.TextBox_UserName.Text, Me.TextBox_Password.Text)
    End Sub
    ''' <summary>
    ''' sends http-request to Google+ API and returns the response string
    ''' </summary>
    Private Function GetGoogleUserJSON2(ByVal access_token As String) As String
        Dim url As String = "someurl"

        Dim wc As New WebClient()
        wc.Headers.Add("Authorization", "OAuth " & access_token)
        Dim data As Stream

        Try
            data = wc.OpenRead(url)
        Catch ex As Exception
            Return ""
        End Try
        Dim reader As New StreamReader(data)
        Dim retirnedJson As String = reader.ReadToEnd()

        Return retirnedJson
    End Function

    ''' <summary>
    ''' sends revoke command to google api. Disconnects user fron the app.
    ''' </summary>
    Private Sub RevokeUser(ByVal access_token As String)
        Dim url As String = "someurl?token=" + access_token

        Dim wc As New WebClient()
        Dim data As Stream = wc.OpenRead(url)
        data.Close()
    End Sub

End Class

''' <summary>
''' Couple classes to sort out the email from google api response
''' </summary>
Public Class GoogleEmail
    Public Property data() As data
        Get
            Return m_data
        End Get
        Set(ByVal value As data)
            m_data = value
        End Set
    End Property
    Private m_data As data
End Class
Public Class data
    Public Property email() As String
        Get
            Return m_email
        End Get
        Set(ByVal value As String)
            m_email = value
        End Set
    End Property
    Private m_email As String

End Class


Public Class User

    'Extract from user Dal class

    Dim db As DBContext
    Public Sub New()
        DBContext = New DBContext()
    End Sub	
	
	
	
    Public Function GetUserByUserNameAndPassword(userName As String, passwordHash As String, DeploymentId As Integer) As UserRow
        Try
			userName = userName.Replace("'", "''")
			passwordHash = passwordHash.Replace("'", "''")
			Dim usr = db.GetUserByUserNameAndPassword("Select * From UserTable Where UserName='" & userName & "' And Password='" & passwordHash & "' And DId=" & DeploymentId)
			Return usr
		Catch ex As Exception
			'
		Finally
			Return Nothing
		End Try
    End Function
    Public Function GetUserByUserName(userName As String, DeploymentId As Integer) As UserRow
        userName = userName.Replace("'", "''")
        Dim usr = db.GetUserByUserName("Select * From UserTable Where UserName='" & userName & "' And DId=" & DeploymentId)
        Return usr
    End Function

    Public Function GetUserByEmail(email As String, DeploymentId As Integer) As UserRow
        Dim usr As Object = Nothing
		Try
			email = email.Replace("'", "''")
			usr = db.GetUserByEmail("Select * From UserTable Where Email='" & email & "' And DId=" & DeploymentId)
		Catch ex1 as Exception
			Throw ex1
		End Try
		Return usr
    End Function
	
	''' <summary>
	''' Couple classes to sort out the email from google api response
	''' </summary>
    Public Function GetUserByADID(ADID As String, DeploymentId As Integer) As UserRow
        ADID = ADID.Replace("'", "''")
        Dim usr = db.GetUserByADID("Select * From UserTable Where Email='" & email & "' And DId=" & DeploymentId)
        Return usr
    End Function

End Class
