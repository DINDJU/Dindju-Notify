Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Drawing

Public Class Form1

    ' ===============================================
    '           Variables Globales
    ' ===============================================
    Private Const PORT As Integer = 3600
    Private serverListener As TcpListener
    Private cancellationTokenSource As CancellationTokenSource
    Private serverRunning As Boolean = False

    ' Référence unique au formulaire de verrouillage
    Private lockedFormInstance As LockedForm = Nothing

    ' ===============================================
    '           Initialisation du Formulaire
    ' ===============================================
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MAJs.Show()

        ' --- Configuration de la NotifyIcon ---
        NotifyIcon1.Visible = True
        NotifyIcon1.Text = "Enterprise Notifier"
        If NotifyIcon1.Icon Is Nothing Then
            NotifyIcon1.Icon = System.Drawing.SystemIcons.Information
        End If

        ' Configuration UI
        lblServerStatus.Text = "Initialisation..."
        txtServerIP.Text = "127.0.0.1" 
        rtbLog.ReadOnly = True
        txtRemoteAppToStart.Text = "C:\Windows\System32\"

        SetupNotifyIconContextMenu()

        ' --- DÉMARRAGE AUTOMATIQUE DU SERVEUR ---
        StartServerAuto()
    End Sub

    Private Sub StartServerAuto()
        Try
            serverListener = New TcpListener(IPAddress.Any, PORT)
            cancellationTokenSource = New CancellationTokenSource()
            serverListener.Start()
            serverRunning = True
            lblServerStatus.Text = "Serveur Actif (Port " & PORT & ")"
            lblServerStatus.ForeColor = Color.Green
            LogMessage("Serveur démarré automatiquement sur le port " & PORT)

            Task.Factory.StartNew(Sub() ListenForClients(cancellationTokenSource.Token), TaskCreationOptions.LongRunning)
        Catch ex As Exception
            lblServerStatus.Text = "Erreur Serveur"
            lblServerStatus.ForeColor = Color.Red
            LogMessage("Erreur démarrage serveur : " & ex.Message, Color.Red)
        End Try
    End Sub

    ' ===============================================
    '           Gestion des Commandes
    ' ===============================================
    Private Sub ListenForClients(token As CancellationToken)
        While serverRunning AndAlso Not token.IsCancellationRequested
            Try
                If serverListener.Pending() Then
                    Dim client As TcpClient = serverListener.AcceptTcpClient()
                    Task.Factory.StartNew(Sub() HandleClient(client), token)
                Else
                    Thread.Sleep(100)
                End If
            Catch ex As Exception
                Exit While
            End Try
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient)
        Try
            Using networkStream As NetworkStream = client.GetStream()
                Dim reader As New StreamReader(networkStream, Encoding.UTF8)
                Dim msg As String = reader.ReadLine()

                If Not String.IsNullOrEmpty(msg) Then
                    LogMessage("Commande reçue : " & msg, Color.Blue)
                    
                    ' Analyse de la commande
                    If msg.StartsWith("START_APP:", StringComparison.OrdinalIgnoreCase) Then
                        LaunchSoftware(msg.Substring(10).Trim())
                    ElseIf msg.Equals("COMMAND:TOGGLE_LOCKED_FORM", StringComparison.OrdinalIgnoreCase) Then
                        ToggleLockedForm()
                    ElseIf msg.StartsWith("NOTIFY_MB:", StringComparison.OrdinalIgnoreCase) Then
                        ShowNotificationOnClient(msg.Substring(10).Trim(), True)
                    ElseIf msg.StartsWith("NOTIFY_BUBBLE:", StringComparison.OrdinalIgnoreCase) Then
                        ShowNotificationOnClient(msg.Substring(14).Trim(), False)
                    Else
                        ShowNotificationOnClient(msg, False)
                    End If
                End If
            End Using
        Catch ex As Exception
            LogMessage("Erreur client : " & ex.Message, Color.Red)
        Finally
            client.Close()
        End Try
    End Sub

    ' --- Action de Verrouillage / Déverrouillage ---
    Private Sub ToggleLockedForm()
        If Me.InvokeRequired Then
            Me.Invoke(New Action(AddressOf ToggleLockedForm))
        Else
            ' Si le formulaire est déjà ouvert et visible
            If lockedFormInstance IsNot Nothing AndAlso lockedFormInstance.Visible Then
                lockedFormInstance.Close()
                lockedFormInstance.Dispose()
                lockedFormInstance = Nothing
                LogMessage("PC Déverrouillé (LockedForm fermé)", Color.DarkGreen)
            Else
                ' Sinon, on crée une nouvelle instance et on l'affiche
                lockedFormInstance = New LockedForm()
                lockedFormInstance.Show()
                LogMessage("PC Verrouillé (LockedForm ouvert)", Color.DarkRed)
            End If
        End If
    End Sub

    Private Sub LaunchSoftware(path As String)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String)(AddressOf LaunchSoftware), path)
        Else
            Try
                If File.Exists(path) Then
                    Process.Start(path)
                    LogMessage("Lancé : " & path)
                Else
                    LogMessage("Fichier introuvable : " & path, Color.Red)
                End If
            Catch ex As Exception
                LogMessage("Erreur : " & ex.Message, Color.Red)
            End Try
        End If
    End Sub

    ' ===============================================
    '           Logique d'Envoi (Client)
    ' ===============================================
    Private Sub btnLocked_Click(sender As Object, e As EventArgs) Handles btnLocked.Click
        Dim ip As String = txtServerIP.Text.Trim()
        If Not String.IsNullOrWhiteSpace(ip) Then
            Task.Factory.StartNew(Sub() SendMessageToServer(ip, "COMMAND:TOGGLE_LOCKED_FORM"))
        End If
    End Sub

    Private Sub btnSendNotification_Click(sender As Object, e As EventArgs) Handles btnSendNotification.Click
        Dim ip As String = txtServerIP.Text.Trim()
        Dim msg As String = txtMessageToSend.Text.Trim()
        If msg <> "" Then
            Dim prefix As String = If(CheckBox1.Checked, "NOTIFY_MB:", "NOTIFY_BUBBLE:")
            Task.Factory.StartNew(Sub() SendMessageToServer(ip, prefix & msg))
        End If
    End Sub

    Private Sub SendMessageToServer(ip As String, message As String)
        Try
            Using client As New TcpClient()
                client.Connect(ip, PORT)
                Using writer As New StreamWriter(client.GetStream(), Encoding.UTF8)
                    writer.WriteLine(message)
                    writer.Flush()
                End Using
            End Using
        Catch ex As Exception
            LogMessage("Erreur d'envoi : " & ex.Message, Color.Red)
        End Try
    End Sub

    ' ===============================================
    '           Utilitaires UI
    ' ===============================================
    Private Sub LogMessage(message As String, Optional color As Color = Nothing)
        If rtbLog.InvokeRequired Then
            rtbLog.Invoke(New Action(Of String, Color)(AddressOf LogMessage), message, color)
        Else
            rtbLog.SelectionStart = rtbLog.TextLength
            rtbLog.SelectionColor = If(color = Nothing, Color.Black, color)
            rtbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") & " - " & message & Environment.NewLine)
            rtbLog.ScrollToCaret()
        End If
    End Sub

    Private Sub ShowNotificationOnClient(text As String, useMessageBox As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String, Boolean)(AddressOf ShowNotificationOnClient), text, useMessageBox)
        Else
            If useMessageBox Then
                MessageBox.Show(text, TextBoxtitle.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                NotifyIcon1.ShowBalloonTip(5000, TextBoxtitle.Text, text, ToolTipIcon.Info)
            End If
        End If
    End Sub

    Private Sub SetupNotifyIconContextMenu()
        Dim contextMenu As New ContextMenuStrip()
        contextMenu.Items.Add("Afficher/Masquer", Nothing, AddressOf ShowHideForm)
        contextMenu.Items.Add("Quitter", Nothing, AddressOf ExitApplication)
        NotifyIcon1.ContextMenuStrip = contextMenu
    End Sub

    Private Sub ShowHideForm(sender As Object, e As EventArgs)
        If Me.Visible Then Me.Hide() Else Me.Show()
    End Sub

    Private Sub ExitApplication(sender As Object, e As EventArgs)
        CleanupServer()
        Application.Exit()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        CleanupServer()
    End Sub

    Private Sub CleanupServer()
        serverRunning = False
        cancellationTokenSource?.Cancel()
        serverListener?.Stop()
        NotifyIcon1.Visible = False
        If lockedFormInstance IsNot Nothing Then lockedFormInstance.Close()
    End Sub

    'Boutons raccourcis
    Private Sub btnStartRemoteApp_Click(sender As Object, e As EventArgs) Handles btnStartRemoteApp.Click
        SendMessageToServer(txtServerIP.Text.Trim(), "START_APP:" & txtRemoteAppToStart.Text.Trim())
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        txtRemoteAppToStart.Text = "C:\windows\system32\cmd.exe"
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        txtRemoteAppToStart.Text = "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        txtRemoteAppToStart.Text = "C:\windows\system32\notepad.exe"
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        txtRemoteAppToStart.Text = "C:\Program Files\Google\Chrome\Application\chrome.exe"
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Scanneur_d_ip.Show()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MAJs.Show()
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Me.Close()
    End Sub
End Class