Imports System.Runtime.InteropServices ' Nécessaire pour DllImport

Public Class LockedForm

    ' Déclare la fonction API Windows pour bloquer/débloquer les entrées
    ' fBlockIt = True pour bloquer, False pour débloquer
    Private Declare Auto Function BlockInput Lib "user32.dll" (ByVal fBlockIt As Boolean) As Boolean

    Private Sub LockedForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Positionne le formulaire sur tout l'écran principal
        Me.Location = New Point(0, 0)
        Me.Size = Screen.PrimaryScreen.Bounds.Size
        Me.TopMost = True ' Garde le formulaire toujours au-dessus des autres fenêtres

        ' Cache le curseur de la souris (pour une meilleure illusion de verrouillage)
        Cursor.Hide()

        ' Tente de bloquer toutes les entrées clavier et souris au niveau du système
        Try
            If Not BlockInput(True) Then
                ' Si BlockInput échoue (par exemple, permissions insuffisantes), avertir l'utilisateur
                MessageBox.Show("Attention : Impossible de bloquer l'entrée utilisateur. Le formulaire ne pourra pas empêcher l'interaction avec le système. Vérifiez les privilèges de l'application.", "Avertissement de Verrouillage", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur inattendue lors du blocage de l'entrée : " & ex.Message, "Erreur Critique", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Centrer le message de verrouillage (Assurez-vous d'avoir un Label nommé 'lblLockMessage' sur votre LockedForm)
        If Not IsNothing(Me.Controls("lblLockMessage")) Then
            Dim lbl As Label = CType(Me.Controls("lblLockMessage"), Label)
            lbl.Left = (Me.ClientSize.Width - lbl.Width) / 2
            lbl.Top = (Me.ClientSize.Height - lbl.Height) / 2
        End If
    End Sub

    Private Sub LockedForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ' Empêche le formulaire d'être fermé par l'utilisateur (Alt+F4, bouton X, Gestionnaire de tâches)
        ' Le formulaire ne doit être fermé que par la commande explicite de Form1.
        If e.CloseReason = CloseReason.UserClosing OrElse e.CloseReason = CloseReason.TaskManagerClosing Then
            e.Cancel = True ' Annule l'événement de fermeture
            ' Optionnel : vous pouvez afficher un bref message ici, mais sur un écran verrouillé, c'est souvent indésirable.
        End If
    End Sub

    Private Sub LockedForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ' Cette partie est CRUCIALE : débloque les entrées lorsque le formulaire est réellement fermé.
        Try
            If Not BlockInput(False) Then
                ' Si le déblocage échoue, c'est une situation critique
                MessageBox.Show("Erreur critique : Impossible de débloquer l'entrée utilisateur. Votre clavier et/ou souris pourrait être inopérant(e). Veuillez redémarrer votre PC manuellement si les entrées ne répondent pas.", "Erreur de Déblocage", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show("Erreur inattendue lors du déblocage de l'entrée : " & ex.Message, "Erreur Critique", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Rétablit la visibilité du curseur de la souris
        Cursor.Show()
    End Sub

    ' Pour le débogage ou une "porte dérobée" temporaire :
    ' Permet de fermer le formulaire avec une touche spécifique (ex: ESC).
    ' À NE PAS UTILISER TEL QUEL EN PRODUCTION SANS SÉCURITÉ SUPPLÉMENTAIRE (ex: mot de passe).
    Private Sub LockedForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then ' Vous pouvez choisir une autre touche ou une combinaison
            ' Débloque l'entrée immédiatement avant de tenter de fermer, au cas où
            ' FormClosing annule la fermeture pour une raison quelconque.
            Try
                BlockInput(False)
            Catch ex As Exception
                ' Gérer l'erreur si le déblocage échoue (déjà géré dans FormClosed, mais sécurité supplémentaire)
            End Try

            Cursor.Show() ' Rétablit le curseur
            e.SuppressKeyPress = True ' Empêche la touche ESC d'être traitée ailleurs
            Me.Close() ' Permet la fermeture du formulaire
        End If
    End Sub

End Class