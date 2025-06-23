
Imports System.Net.NetworkInformation
    Imports System.Threading.Tasks

Public Class Scanneur_d_ip
    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        ListBox1.Items.Clear()
        Dim baseIp As String = txtBaseIP.Text
        ProgressBar1.Value = 0
        ProgressBar1.Maximum = 254

        Dim taches As New List(Of Task)

        For i As Integer = 1 To 254
            Dim ip As String = baseIp & i.ToString()
            Dim index As Integer = i
            taches.Add(Task.Run(Sub()
                                    If PingAdresse(ip) Then
                                        Me.Invoke(Sub() ListBox1.Items.Add(ip & " est active."))
                                    End If
                                    Me.Invoke(Sub() ProgressBar1.Value += 1)
                                End Sub))
        Next

        Task.WhenAll(taches).ContinueWith(Sub()
                                              MessageBox.Show("Scan terminé !")
                                          End Sub)
    End Sub


    Private Function PingAdresse(ip As String) As Boolean
        Try
            Dim p As New Ping()
            Dim reponse = p.Send(ip, 100)
            Return reponse.Status = IPStatus.Success
        Catch
            Return False
        End Try
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class
