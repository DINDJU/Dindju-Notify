Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO
Imports System.Windows.Forms
Imports System.Diagnostics ' Nécessaire pour Process.Start
Imports System.Drawing

Public Class Form1

    ' ===============================================
    '           Variables Globales
    ' ===============================================
    Private Const PORT As Integer = 3600
    Private serverListener As TcpListener
    Private cancellationTokenSource As CancellationTokenSource ' Pour arrêter proprement le serveur
    Private serverRunning As Boolean = False

    ' Référence au formulaire de verrouillage sur le PC serveur
    Private lockedFormInstance As LockedForm = Nothing

    ' ===============================================
    '           Initialisation du Formulaire
    ' ===============================================
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MAJs.Show()
        Me.Text = "Dindju notify 3.0.2"

        ' --- Configuration de la NotifyIcon ---
        NotifyIcon1.Visible = True
        NotifyIcon1.Text = "Enterprise Notifier"
        If NotifyIcon1.Icon Is Nothing Then
            Try
                ' Utilisez une icône par défaut si aucune n'est définie
                NotifyIcon1.Icon = System.Drawing.SystemIcons.Information
            Catch ex As Exception
                LogMessage("AVERTISSEMENT: Impossible de charger une icône par défaut pour NotifyIcon. Erreur: " & ex.Message, Color.OrangeRed)
            End Try
        End If
        ' --- Fin de la configuration NotifyIcon ---

        ' Configuration initiale de l'UI du Serveur
        btnStopServer.Enabled = False
        lblServerStatus.Text = "Serveur Inactif"
        txtServerIP.Text = "127.0.0.1" ' IP locale par défaut pour les tests
        rtbLog.ReadOnly = True ' Rendre la RichTextBox non modifiable

        ' Initialisation des nouveaux contrôles
        txtSoftwarePath.Text = "" ' Laisse vide ou met un chemin par défaut pour le logiciel local
        txtRemoteAppToStart.Text = "C:\Windows\System32\notepad.exe" ' Exemple pour tester l'appli distante

        ' Ajouter un menu contextuel pour la NotifyIcon
        SetupNotifyIconContextMenu()
    End Sub

    ' ===============================================
    '           Logique du Serveur (Client de notifications / Récepteur de commandes)
    ' ===============================================
    Private Sub btnStartServer_Click(sender As Object, e As EventArgs) Handles btnStartServer.Click
        If serverRunning Then Return

        Try
            serverListener = New TcpListener(IPAddress.Any, PORT)
            cancellationTokenSource = New CancellationTokenSource()

            serverListener.Start()
            serverRunning = True
            lblServerStatus.Text = "Serveur Démarré sur le port " & PORT & "..."
            btnStartServer.Enabled = False
            btnStopServer.Enabled = True
            LogMessage("Serveur démarré et en écoute sur " & IPAddress.Any.ToString() & ":" & PORT)

            Task.Factory.StartNew(Sub() ListenForClients(cancellationTokenSource.Token), TaskCreationOptions.LongRunning)

        Catch ex As Exception
            MessageBox.Show("Erreur au démarrage du serveur : " & ex.Message, "Erreur Serveur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LogMessage("Erreur au démarrage du serveur : " & ex.Message, Color.Red)
            CleanupServer()
        End Try
    End Sub

    Private Sub ListenForClients(token As CancellationToken)
        While serverRunning AndAlso Not token.IsCancellationRequested
            Try
                If serverListener.Pending() Then
                    Dim client As TcpClient = serverListener.AcceptTcpClient()
                    LogMessage("Client connecté depuis : " & DirectCast(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString())
                    Task.Factory.StartNew(Sub() HandleClient(client, token), token)
                Else
                    Thread.Sleep(100)
                End If
            Catch ex As ObjectDisposedException
                LogMessage("Écoute du serveur arrêtée (ObjectDisposedException).", Color.Orange)
                Exit While
            Catch ex As SocketException
                If ex.SocketErrorCode = SocketError.Interrupted Then
                    LogMessage("Serveur interrompu (arrêt en cours).", Color.Orange)
                Else
                    LogMessage("Erreur de socket lors de l'écoute des clients : " & ex.Message, Color.Red)
                End If
                Exit While
            Catch ex As Exception
                LogMessage("Erreur inattendue lors de l'écoute des clients : " & ex.Message, Color.Red)
            End Try
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient, token As CancellationToken)
        Dim reader As StreamReader = Nothing
        Try
            Dim networkStream As NetworkStream = client.GetStream()
            reader = New StreamReader(networkStream, Encoding.UTF8)

            Dim receivedMessage As String = reader.ReadLine()

            If Not String.IsNullOrEmpty(receivedMessage) Then
                LogMessage("Message reçu de " & DirectCast(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString() & ": " & receivedMessage, Color.Blue)

                ' *** LOGIQUE MISE À JOUR POUR GÉRER LE TYPE DE NOTIFICATION ET COMMANDES ***
                If receivedMessage.StartsWith("START_APP:", StringComparison.OrdinalIgnoreCase) Then
                    Dim softwarePathToLaunch As String = receivedMessage.Substring("START_APP:".Length).Trim()
                    LaunchSoftware(softwarePathToLaunch)
                ElseIf receivedMessage.Equals("COMMAND:TOGGLE_LOCKED_FORM", StringComparison.OrdinalIgnoreCase) Then
                    ToggleLockedForm()
                ElseIf receivedMessage.StartsWith("NOTIFY_MB:", StringComparison.OrdinalIgnoreCase) Then ' Nouveau préfixe pour MessageBox
                    Dim actualMessage As String = receivedMessage.Substring("NOTIFY_MB:".Length).Trim()
                    ShowNotificationOnClient(actualMessage, True) ' True indique d'utiliser MessageBox
                ElseIf receivedMessage.StartsWith("NOTIFY_BUBBLE:", StringComparison.OrdinalIgnoreCase) Then ' Nouveau préfixe pour Bulle
                    Dim actualMessage As String = receivedMessage.Substring("NOTIFY_BUBBLE:".Length).Trim()
                    ShowNotificationOnClient(actualMessage, False) ' False indique d'utiliser Bulle
                Else
                    ' Fallback pour les messages sans préfixe (afficher en bulle par défaut)
                    ShowNotificationOnClient(receivedMessage, False)
                End If

            End If

        Catch ex As IOException
            LogMessage("Erreur réseau avec le client " & DirectCast(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString() & ": " & ex.Message, Color.Orange)
        Catch ex As Exception
            LogMessage("Erreur lors de la gestion du client : " & ex.Message, Color.Red)
        Finally
            If Not IsNothing(reader) Then reader.Dispose()
            If Not IsNothing(client) Then client.Close()
            LogMessage("Client déconnecté.", Color.Gray)
        End Try
    End Sub

    Private Sub LaunchSoftware(softwarePath As String)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String)(AddressOf LaunchSoftware), softwarePath)
        Else
            If String.IsNullOrWhiteSpace(softwarePath) Then
                LogMessage("Erreur: Le chemin du logiciel à lancer est vide.", Color.Red)
                ShowBalloonNotification("Erreur de Lancement", "Le chemin du logiciel à lancer est vide.", ToolTipIcon.Error)
                Return
            End If

            If Not File.Exists(softwarePath) Then
                LogMessage("Erreur: Le fichier logiciel '" & softwarePath & "' n'existe pas sur ce PC.", Color.Red)
                ShowBalloonNotification("Erreur de Lancement", "Le logiciel spécifié (" & softwarePath & ") n'a pas été trouvé.", ToolTipIcon.Error)
                Return
            End If

            Try
                Process.Start(softwarePath)
                LogMessage("Logiciel démarré avec succès : " & softwarePath, Color.Purple)
                ShowBalloonNotification("Logiciel Démarré", "L'application '" & Path.GetFileName(softwarePath) & "' a été lancée.", ToolTipIcon.Info)
            Catch ex As Exception
                LogMessage("Erreur lors du démarrage du logiciel '" & softwarePath & "': " & ex.Message, Color.Red)
                ShowBalloonNotification("Erreur de Lancement", "Impossible de démarrer l'application '" & Path.GetFileName(softwarePath) & "'. Détails: " & ex.Message, ToolTipIcon.Error)
            End Try
        End If
    End Sub

    Private Sub ToggleLockedForm()
        If Me.InvokeRequired Then
            Me.Invoke(New Action(AddressOf ToggleLockedForm))
        Else
            If lockedFormInstance Is Nothing OrElse lockedFormInstance.IsDisposed Then
                ' Le formulaire n'est pas ouvert ou a été fermé
                lockedFormInstance = New LockedForm()
                lockedFormInstance.Show()
                LogMessage("Formulaire de verrouillage affiché.", Color.DeepPink)
                ShowBalloonNotification("PC Verrouillé", "L'écran distant est maintenant verrouillé.", ToolTipIcon.Warning)
            Else
                ' Le formulaire est déjà ouvert, le fermer
                lockedFormInstance.Close()
                lockedFormInstance.Dispose() ' Libérer les ressources
                lockedFormInstance = Nothing
                LogMessage("Formulaire de verrouillage fermé.", Color.DeepPink)
                ShowBalloonNotification("PC Déverrouillé", "L'écran distant est maintenant déverrouillé.", ToolTipIcon.Info)
            End If
        End If
    End Sub

    Private Sub btnStopServer_Click(sender As Object, e As EventArgs) Handles btnStopServer.Click
        If Not serverRunning Then Return

        cancellationTokenSource?.Cancel()

        ' Ferme le formulaire de verrouillage si ouvert avant d'arrêter le serveur
        If Not IsNothing(lockedFormInstance) And Not lockedFormInstance.IsDisposed Then
            lockedFormInstance.Close()
            lockedFormInstance.Dispose()
            lockedFormInstance = Nothing
            LogMessage("Formulaire de verrouillage fermé avant l'arrêt du serveur.", Color.DeepPink)
        End If

        CleanupServer()
        LogMessage("Serveur arrêté.")
        MessageBox.Show("Serveur arrêté.", "Serveur", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub CleanupServer()
        If Not IsNothing(serverListener) Then
            If serverRunning Then
                serverListener.Stop()
            End If
            serverListener = Nothing
        End If

        serverRunning = False
        lblServerStatus.Text = "Serveur Inactif"
        btnStartServer.Enabled = True
        btnStopServer.Enabled = False

        If Not IsNothing(cancellationTokenSource) Then
            cancellationTokenSource.Dispose()
            cancellationTokenSource = Nothing
        End If
    End Sub

    ' ===============================================
    '           Logique du Client (Émetteur de notifications/commandes)
    ' ===============================================
    Private Sub btnSendNotification_Click(sender As Object, e As EventArgs) Handles btnSendNotification.Click
        Dim serverIpAddress As String = txtServerIP.Text.Trim()
        Dim messageToSend As String = txtMessageToSend.Text.Trim()

        If String.IsNullOrWhiteSpace(serverIpAddress) Then
            MessageBox.Show("Veuillez entrer l'adresse IP du serveur.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(messageToSend) Then
            MessageBox.Show("Veuillez saisir un message à envoyer.", "Message Vide", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' *** LOGIQUE MISE À JOUR : Détermine le préfixe en fonction de CheckBox1 ***
        Dim prefixedMessage As String
        If CheckBox1.Checked Then ' Si la CheckBox est cochée sur le PC émetteur
            prefixedMessage = "NOTIFY_MB:" & messageToSend
        Else ' Si la CheckBox n'est pas cochée
            prefixedMessage = "NOTIFY_BUBBLE:" & messageToSend
        End If

        Task.Factory.StartNew(Sub() SendMessageToServer(serverIpAddress, prefixedMessage))
    End Sub

    Private Sub btnStartRemoteApp_Click(sender As Object, e As EventArgs) Handles btnStartRemoteApp.Click
        Dim serverIpAddress As String = txtServerIP.Text.Trim()
        Dim appPath As String = txtRemoteAppToStart.Text.Trim()

        If String.IsNullOrWhiteSpace(serverIpAddress) Then
            MessageBox.Show("Veuillez entrer l'adresse IP du serveur.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(appPath) Then
            MessageBox.Show("Veuillez saisir le chemin du logiciel à démarrer sur le PC distant.", "Chemin Vide", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim commandToSend As String = "START_APP:" & appPath
        Task.Factory.StartNew(Sub() SendMessageToServer(serverIpAddress, commandToSend))
    End Sub

    Private Sub btnLocked_Click(sender As Object, e As EventArgs) Handles btnLocked.Click
        Dim serverIpAddress As String = txtServerIP.Text.Trim()

        If String.IsNullOrWhiteSpace(serverIpAddress) Then
            MessageBox.Show("Veuillez entrer l'adresse IP du serveur.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim commandToSend As String = "COMMAND:TOGGLE_LOCKED_FORM"
        Task.Factory.StartNew(Sub() SendMessageToServer(serverIpAddress, commandToSend))
    End Sub

    Private Sub SendMessageToServer(serverIpAddress As String, message As String)
        Dim client As TcpClient = Nothing
        Dim writer As StreamWriter = Nothing
        Try
            LogMessage("Tentative de connexion au serveur " & serverIpAddress & ":" & PORT & "...", Color.DarkOrange)
            client = New TcpClient()
            client.Connect(serverIpAddress, PORT)

            LogMessage("Connecté au serveur : " & serverIpAddress, Color.Green)
            Dim networkStream As NetworkStream = client.GetStream()
            writer = New StreamWriter(networkStream, Encoding.UTF8)

            writer.WriteLine(message)
            writer.Flush()

            LogMessage("Commande/Message envoyé avec succès : " & message, Color.Green)
            ' Ne pas effacer txtMessageToSend si c'est une commande spécifique
            If Not message.StartsWith("START_APP:", StringComparison.OrdinalIgnoreCase) AndAlso
               Not message.Equals("COMMAND:TOGGLE_LOCKED_FORM", StringComparison.OrdinalIgnoreCase) AndAlso
               Not message.StartsWith("NOTIFY_MB:", StringComparison.OrdinalIgnoreCase) AndAlso
               Not message.StartsWith("NOTIFY_BUBBLE:", StringComparison.OrdinalIgnoreCase) Then
                If txtMessageToSend.InvokeRequired Then
                    txtMessageToSend.Invoke(New Action(Sub() txtMessageToSend.Clear()))
                Else
                    txtMessageToSend.Clear()
                End If
            End If

        Catch ex As SocketException
            LogMessage("Erreur de connexion : " & ex.Message & " (Code: " & ex.ErrorCode & ")", Color.Red)
            If Me.InvokeRequired Then
                Me.Invoke(New Action(Sub() MessageBox.Show("Impossible de se connecter au serveur (" & serverIpAddress & ":" & PORT & "). Vérifiez l'IP et que le serveur est démarré.", "Erreur de Connexion", MessageBoxButtons.OK, MessageBoxIcon.Error)))
            Else
                MessageBox.Show("Impossible de se connecter au serveur (" & serverIpAddress & ":" & PORT & "). Vérifiez l'IP et que le serveur est démarré.", "Erreur de Connexion", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            LogMessage("Erreur lors de l'envoi du message : " & ex.Message, Color.Red)
            If Me.InvokeRequired Then
                Me.Invoke(New Action(Sub() MessageBox.Show("Erreur lors de l'envoi du message : " & ex.Message, "Erreur Client", MessageBoxButtons.OK, MessageBoxIcon.Error)))
            Else
                MessageBox.Show("Erreur lors de l'envoi du message : " & ex.Message, "Erreur Client", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Finally
            If Not IsNothing(writer) Then writer.Dispose()
            If Not IsNothing(client) Then client.Close()
        End Try
    End Sub

    ' ===============================================
    '           Fonctions Utilitaire
    ' ===============================================
    Private Sub LogMessage(message As String, Optional color As Color = Nothing)
        If rtbLog.InvokeRequired Then
            rtbLog.Invoke(New Action(Of String, Color)(AddressOf LogMessage), message, color)
        Else
            rtbLog.SelectionStart = rtbLog.TextLength
            rtbLog.SelectionLength = 0

            If color = Nothing Then color = Color.Black
            rtbLog.SelectionColor = color

            rtbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") & " - " & message & Environment.NewLine)
            rtbLog.SelectionColor = rtbLog.ForeColor
            rtbLog.ScrollToCaret()
        End If
    End Sub

    ' Gère l'affichage de la notification sur le PC qui REÇOIT la commande (le serveur)
    ' bool useMessageBox indique si la notification doit être une MessageBox ou une bulle
    Private Sub ShowNotificationOnClient(text As String, useMessageBox As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String, Boolean)(AddressOf ShowNotificationOnClient), text, useMessageBox)
        Else
            Try
                Dim title As String = TextBoxtitle.Text ' Titre par défaut
                Dim icon As ToolTipIcon = ToolTipIcon.Info ' Icône par défaut

                If useMessageBox Then
                    Dim msgBoxIcon As MessageBoxIcon
                    Select Case icon
                        Case ToolTipIcon.Error
                            msgBoxIcon = MessageBoxIcon.Error
                        Case ToolTipIcon.Info
                            msgBoxIcon = MessageBoxIcon.Information
                        Case ToolTipIcon.Warning
                            msgBoxIcon = MessageBoxIcon.Warning
                        Case Else
                            msgBoxIcon = MessageBoxIcon.None
                    End Select
                    MessageBox.Show(text, title, MessageBoxButtons.OK, msgBoxIcon)
                    LogMessage("Notification affichée en MessageBox (PC Distant) : " & title & " - " & text, Color.Orange)
                Else
                    If NotifyIcon1.Icon Is Nothing Then
                        LogMessage("AVERTISSEMENT: NotifyIcon n'a pas d'icône. La bulle de notification ne peut pas être affichée. Retour au MessageBox.", Color.Red)
                        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If

                    NotifyIcon1.ShowBalloonTip(5000, title, text, icon)
                    LogMessage("Notification bulle envoyée (PC Distant) : " & title & " - " & text, Color.DarkGreen)
                End If

            Catch ex As Exception
                LogMessage("Erreur lors de l'affichage de la notification (fallback au MessageBox) : " & ex.Message, Color.Red)
                MessageBox.Show(text, " (Erreur de notification)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' Cette fonction est pour les notifications générées LOCALEMENT par le serveur (ex: erreur de lancement de logiciel)
    Private Sub ShowBalloonNotification(title As String, text As String, icon As ToolTipIcon)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String, String, ToolTipIcon)(AddressOf ShowBalloonNotification), title, text, icon)
        Else
            Try
                If CheckBox1.Checked Then ' La CheckBox1 sur le PC serveur affecte seulement les messages de LOG locaux
                    Dim msgBoxIcon As MessageBoxIcon
                    Select Case icon
                        Case ToolTipIcon.Error
                            msgBoxIcon = MessageBoxIcon.Error
                        Case ToolTipIcon.Info
                            msgBoxIcon = MessageBoxIcon.Information
                        Case ToolTipIcon.Warning
                            msgBoxIcon = MessageBoxIcon.Warning
                        Case Else
                            msgBoxIcon = MessageBoxIcon.None
                    End Select
                    MessageBox.Show(text, title, MessageBoxButtons.OK, msgBoxIcon)
                    LogMessage("Notification LOCALE affichée en MessageBox : " & title & " - " & text, Color.Orange)
                Else
                    If NotifyIcon1.Icon Is Nothing Then
                        LogMessage("AVERTISSEMENT: NotifyIcon n'a pas d'icône. La bulle de notification LOCALE ne peut pas être affichée. Retour au MessageBox.", Color.Red)
                        MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Return
                    End If

                    NotifyIcon1.ShowBalloonTip(5000, title, text, icon)
                    LogMessage("Notification bulle LOCALE envoyée : " & title & " - " & text, Color.DarkGreen)
                End If

            Catch ex As Exception
                LogMessage("Erreur lors de l'affichage de la notification LOCALE (fallback au MessageBox) : " & ex.Message, Color.Red)
                MessageBox.Show(text, title & " (Erreur de notification LOCALE)", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub


    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        If Me.Visible Then
            Me.Hide()
            Me.ShowInTaskbar = False
        Else
            Me.Show()
            Me.BringToFront()
            Me.ShowInTaskbar = True
        End If
    End Sub

    Private Sub SetupNotifyIconContextMenu()
        Dim contextMenu As New ContextMenuStrip()
        Dim showHideMenuItem As New ToolStripMenuItem("Afficher/Masquer la Fenêtre")
        Dim exitMenuItem As New ToolStripMenuItem("Quitter")

        AddHandler showHideMenuItem.Click, AddressOf ShowHideForm
        AddHandler exitMenuItem.Click, AddressOf ExitApplication

        contextMenu.Items.Add(showHideMenuItem)
        contextMenu.Items.Add(exitMenuItem)

        NotifyIcon1.ContextMenuStrip = contextMenu
    End Sub

    Private Sub ShowHideForm(sender As Object, e As EventArgs)
        If Me.Visible Then
            Me.Hide()
            Me.ShowInTaskbar = False
        Else
            Me.Show()
            Me.BringToFront()
            Me.ShowInTaskbar = True
        End If
    End Sub

    Private Sub ExitApplication(sender As Object, e As EventArgs)
        ' Assurez-vous de fermer le LockedForm si ouvert avant de quitter l'application
        If Not IsNothing(lockedFormInstance) AndAlso Not lockedFormInstance.IsDisposed Then
            lockedFormInstance.Close()
            lockedFormInstance.Dispose()
            lockedFormInstance = Nothing
        End If

        If serverRunning Then
            CleanupServer()
        End If
        NotifyIcon1.Visible = False
        NotifyIcon1.Dispose()
        Application.Exit()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim result As DialogResult = MessageBox.Show("Êtes-vous sûr de vouloir fermer ?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)

        If result = DialogResult.Cancel Then
            e.Cancel = True ' Annule la fermeture
            Return
        End If

        If lockedFormInstance IsNot Nothing AndAlso Not lockedFormInstance.IsDisposed Then
            lockedFormInstance.Close()
            lockedFormInstance.Dispose()
            lockedFormInstance = Nothing
        End If

        If serverRunning Then CleanupServer()
        NotifyIcon1.Visible = False
        NotifyIcon1.Dispose()
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Scanneur_d_ip.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MAJs.Show()
    End Sub
End Class