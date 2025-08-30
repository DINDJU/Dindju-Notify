Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class Form1
    Dim listener As TcpListener
    Dim serverThread As Thread
    Dim connectedClients As New List(Of TcpClient)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Icône obligatoire pour les notifications
        NotifyIcon1.Visible = True

        ' Lancer le serveur en arrière-plan
        serverThread = New Thread(AddressOf StartServer)
        serverThread.IsBackground = True
        serverThread.Start()
    End Sub

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
            Dim buffer(1024) As Byte
            Dim bytesRead As Integer = stream.Read(buffer, 0, buffer.Length)
            Dim message As String = Encoding.UTF8.GetString(buffer, 0, bytesRead)

            ' Afficher dans la ListBox
            Invoke(Sub() ListBox1.Items.Add("Reçu : " & message))

            ' Affichage selon la CheckBox2
            Invoke(Sub()
                       If CheckBox2.Checked Then
                           ' Afficher en MessageBox
                           MessageBox.Show(message, "Nouveau message", MessageBoxButtons.OK, MessageBoxIcon.Information)
                       Else
                           ' Afficher en notification Windows
                           NotifyIcon1.BalloonTipTitle = "Nouveau message"
                           NotifyIcon1.BalloonTipText = message
                           NotifyIcon1.ShowBalloonTip(3000)
                       End If
                   End Sub)
        Catch ex As Exception
            ' Ignorer les erreurs
        Finally
            SyncLock connectedClients
                connectedClients.Remove(client)
            End SyncLock
            client.Close()
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim messageBytes As Byte() = Encoding.UTF8.GetBytes(TextBox1.Text)

        If CheckBox1.Checked Then
            ' Envoyer à tous les clients connectés
            SyncLock connectedClients
                For Each client In connectedClients
                    Try
                        Dim stream As NetworkStream = client.GetStream()
                        stream.Write(messageBytes, 0, messageBytes.Length)
                    Catch ex As Exception
                        ' Ignorer les erreurs
                    End Try
                Next
            End SyncLock
        Else
            ' Envoyer à une IP spécifique
            Try
                Dim client As New TcpClient(TextBox2.Text, 2025)
                Dim stream As NetworkStream = client.GetStream()
                stream.Write(messageBytes, 0, messageBytes.Length)
                client.Close()
            Catch ex As Exception
                MessageBox.Show("Impossible de se connecter à l'IP spécifiée.")
            End Try
        End If
    End Sub
End Class
