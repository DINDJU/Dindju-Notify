Imports System.Security.Principal
Imports System.Diagnostics

Public Class Administrator
    Private Sub Administrator_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Vérifie si l'utilisateur est administrateur
        Dim identity = WindowsIdentity.GetCurrent()
        Dim principal = New WindowsPrincipal(identity)

        If Not principal.IsInRole(WindowsBuiltInRole.Administrator) Then
            ' Relance avec élévation
            Dim psi As New ProcessStartInfo(Application.ExecutablePath)
            psi.Verb = "runas"
            psi.UseShellExecute = True

            Try
                Process.Start(psi)
            Catch ex As Exception
                MessageBox.Show("Lancement en administrateur annulé ou échoué.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            ' Ferme l'instance actuelle
            Application.Exit()
        Else
            ' Si déjà en admin, ouvrir l'app
            Form1.Show()
            Me.Close()
        End If
    End Sub
End Class
