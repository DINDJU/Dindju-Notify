Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class Form1

    Dim listener As TcpListener
    Dim serverThread As Thread
    Dim connectedClients As New List(Of TcpClient)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NotifyIcon1.Visible = True

        serverThread = New Thread(AddressOf StartServer)
        serverThread.IsBackground = True
        serverThread.Start()
    End Sub

    ' ============================
    '        SERVEUR
    ' ============================
    Private Sub StartServer()
        listener = New TcpListener(IPAddress.Any, 2025)
        listener.Start()

        While True
            Dim client As TcpClient = listener.AcceptTcpClient()

            SyncLock connectedClients
                connectedClients.Add(client)
            End SyncLock

            Dim thread As New Thread(AddressOf HandleClient)
            thread.IsBackground = True
            thread.Start(client)
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient)
        Try
            Dim stream As NetworkStream = client.GetStream()
            Dim buffer(2048) As Byte
            Dim bytesRead As Integer = stream.Read(buffer, 0, buffer.Length)

            If bytesRead <= 0 Then Exit Try

            Dim raw As String = Encoding.UTF8.GetString(buffer, 0, bytesRead)

            ' Format attendu : TITRE|MESSAGE
            Dim parts() As String = raw.Split("|"c)

            Dim title As String = If(parts.Length > 1, parts(0), "Notification")
            Dim message As String = If(parts.Length > 1, parts(1), raw)

            Invoke(Sub() ListBox1.Items.Add($"Reçu : {title} → {message}"))

            Invoke(Sub()
                       If CheckBox2.Checked Then
                           MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information)
                       Else
                           NotifyIcon1.BalloonTipTitle = title
                           NotifyIcon1.BalloonTipText = message
                           NotifyIcon1.ShowBalloonTip(3000)
                       End If
                   End Sub)

        Catch
            ' Ignorer
        Finally
            SyncLock connectedClients
                connectedClients.Remove(client)
            End SyncLock
            client.Close()
        End Try
    End Sub

    ' ============================
    '        ENVOI
    ' ============================
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim title As String = TextBoxTitle.Text.Trim()
        If title = "" Then title = "Notification"

        Dim message As String = TextBox1.Text.Trim()
        If message = "" Then Exit Sub

        Dim payload As String = title & "|" & message
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(payload)

        If CheckBox1.Checked Then
            ' Broadcast
            SyncLock connectedClients
                For Each client In connectedClients.ToList()
                    Try
                        Dim stream As NetworkStream = client.GetStream()
                        stream.Write(bytes, 0, bytes.Length)
                    Catch
                        ' Ignorer
                    End Try
                Next
            End SyncLock

        Else
            ' Envoi à une IP spécifique
            Try
                Using client As New TcpClient(TextBox2.Text, 2025)
                    Dim stream As NetworkStream = client.GetStream()
                    stream.Write(bytes, 0, bytes.Length)
                End Using
            Catch
                MessageBox.Show("Impossible de se connecter à l'IP spécifiée.")
            End Try
        End If

    End Sub

End Class
