Imports System.Net
Imports System.Text.RegularExpressions

Public Class MAJs
    Private Sub MAJs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim currentVersion As New Version("4.0.3")
        Dim latestVersion As Version = GetLatestGitHubReleaseVersion()

        If latestVersion IsNot Nothing AndAlso latestVersion > currentVersion Then
            MessageBox.Show("Une nouvelle version est disponible : " & latestVersion.ToString() & " | Version actuelle : " & currentVersion.ToString, "Mise à jour disponible", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        Me.Close()
    End Sub

    Private Function GetLatestGitHubReleaseVersion() As Version
        Try
            ' Forcer TLS 1.2 pour GitHub
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim client As New WebClient()
            client.Headers.Add("User-Agent", "Mozilla/5.0")
            Dim html As String = client.DownloadString("https://github.com/DINDJU/Dindju-Notify/releases")

            Dim match As Match = Regex.Match(html, "releases/tag/v?(\d+\.\d+\.\d+)", RegexOptions.IgnoreCase)
            If match.Success Then
                Return New Version(match.Groups(1).Value)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur lors de la vérification des mises à jour : " & ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning)

        End Try

        Return Nothing

    End Function

End Class
