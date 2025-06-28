Imports System.Net.Sockets
Imports System.Threading.Tasks

Public Class Scanneur_d_ip
    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        ListBox1.Items.Clear()
        Dim baseIp As String = txtBaseIP.Text.Trim()
        If Not baseIp.EndsWith(".") Then baseIp &= "."

        ProgressBar1.Value = 0
        ProgressBar1.Maximum = 254

        Dim taches As New List(Of Task)

        For i As Integer = 1 To 254
            Dim ip As String = baseIp & i.ToString()
            taches.Add(Task.Run(Sub()
                                    If PortOuvert(ip, 3600) Then
                                        Me.Invoke(Sub() ListBox1.Items.Add(ip & " écoute sur le port 3600."))
                                    End If
                                    Me.Invoke(Sub()
                                                  If ProgressBar1.Value < ProgressBar1.Maximum Then
                                                      ProgressBar1.Value += 1
                                                  End If
                                              End Sub)
                                End Sub))
        Next

        Task.WhenAll(taches).ContinueWith(Sub()
                                              Me.Invoke(Sub() MessageBox.Show("Scan terminé !"))
                                          End Sub)
    End Sub

    Private Function PortOuvert(ip As String, port As Integer) As Boolean
        Try
            Using client As New TcpClient()
                Dim result = client.BeginConnect(ip, port, Nothing, Nothing)
                Dim success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(200))
                If success Then
                    client.EndConnect(result)
                    Return True
                End If
            End Using
        Catch
        End Try
        Return False
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
