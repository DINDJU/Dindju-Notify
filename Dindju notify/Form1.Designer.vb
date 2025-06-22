<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.btnSendNotification = New System.Windows.Forms.Button()
        Me.btnStartServer = New System.Windows.Forms.Button()
        Me.btnStopServer = New System.Windows.Forms.Button()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.txtMessageToSend = New System.Windows.Forms.TextBox()
        Me.txtServerIP = New System.Windows.Forms.TextBox()
        Me.lblServerStatus = New System.Windows.Forms.Label()
        Me.rtbLog = New System.Windows.Forms.RichTextBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.txtRemoteAppToStart = New System.Windows.Forms.TextBox()
        Me.btnStartRemoteApp = New System.Windows.Forms.Button()
        Me.txtSoftwarePath = New System.Windows.Forms.TextBox()
        Me.btnLocked = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSendNotification
        '
        Me.btnSendNotification.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSendNotification.Location = New System.Drawing.Point(7, 134)
        Me.btnSendNotification.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnSendNotification.Name = "btnSendNotification"
        Me.btnSendNotification.Size = New System.Drawing.Size(132, 32)
        Me.btnSendNotification.TabIndex = 0
        Me.btnSendNotification.Text = "Executer"
        Me.btnSendNotification.UseVisualStyleBackColor = True
        '
        'btnStartServer
        '
        Me.btnStartServer.BackColor = System.Drawing.Color.Lime
        Me.btnStartServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStartServer.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.btnStartServer.Location = New System.Drawing.Point(132, 24)
        Me.btnStartServer.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnStartServer.Name = "btnStartServer"
        Me.btnStartServer.Size = New System.Drawing.Size(119, 32)
        Me.btnStartServer.TabIndex = 1
        Me.btnStartServer.Text = "▶️ démmarage"
        Me.btnStartServer.UseVisualStyleBackColor = False
        '
        'btnStopServer
        '
        Me.btnStopServer.BackColor = System.Drawing.Color.Red
        Me.btnStopServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStopServer.ForeColor = System.Drawing.Color.Maroon
        Me.btnStopServer.Location = New System.Drawing.Point(9, 24)
        Me.btnStopServer.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnStopServer.Name = "btnStopServer"
        Me.btnStopServer.Size = New System.Drawing.Size(115, 32)
        Me.btnStopServer.TabIndex = 2
        Me.btnStopServer.Text = "🛑 Arrêt"
        Me.btnStopServer.UseVisualStyleBackColor = False
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Text = "NotifyIcon1"
        Me.NotifyIcon1.Visible = True
        '
        'txtMessageToSend
        '
        Me.txtMessageToSend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtMessageToSend.Location = New System.Drawing.Point(7, 100)
        Me.txtMessageToSend.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtMessageToSend.Name = "txtMessageToSend"
        Me.txtMessageToSend.Size = New System.Drawing.Size(244, 26)
        Me.txtMessageToSend.TabIndex = 3
        '
        'txtServerIP
        '
        Me.txtServerIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtServerIP.Location = New System.Drawing.Point(7, 44)
        Me.txtServerIP.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtServerIP.Name = "txtServerIP"
        Me.txtServerIP.Size = New System.Drawing.Size(244, 26)
        Me.txtServerIP.TabIndex = 4
        Me.txtServerIP.Text = "127.0.0.1"
        '
        'lblServerStatus
        '
        Me.lblServerStatus.AutoSize = True
        Me.lblServerStatus.Location = New System.Drawing.Point(7, 60)
        Me.lblServerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblServerStatus.Name = "lblServerStatus"
        Me.lblServerStatus.Size = New System.Drawing.Size(98, 18)
        Me.lblServerStatus.TabIndex = 5
        Me.lblServerStatus.Text = "Teatus serveur"
        '
        'rtbLog
        '
        Me.rtbLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.rtbLog.Location = New System.Drawing.Point(0, 25)
        Me.rtbLog.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.rtbLog.Name = "rtbLog"
        Me.rtbLog.Size = New System.Drawing.Size(415, 432)
        Me.rtbLog.TabIndex = 6
        Me.rtbLog.Text = ""
        '
        'CheckBox1
        '
        Me.CheckBox1.Appearance = System.Windows.Forms.Appearance.Button
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CheckBox1.Location = New System.Drawing.Point(146, 138)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(105, 28)
        Me.CheckBox1.TabIndex = 7
        Me.CheckBox1.Text = "Mode fenaitré"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'txtRemoteAppToStart
        '
        Me.txtRemoteAppToStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtRemoteAppToStart.Location = New System.Drawing.Point(7, 44)
        Me.txtRemoteAppToStart.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtRemoteAppToStart.Name = "txtRemoteAppToStart"
        Me.txtRemoteAppToStart.Size = New System.Drawing.Size(244, 26)
        Me.txtRemoteAppToStart.TabIndex = 8
        '
        'btnStartRemoteApp
        '
        Me.btnStartRemoteApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStartRemoteApp.Location = New System.Drawing.Point(7, 78)
        Me.btnStartRemoteApp.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnStartRemoteApp.Name = "btnStartRemoteApp"
        Me.btnStartRemoteApp.Size = New System.Drawing.Size(132, 32)
        Me.btnStartRemoteApp.TabIndex = 9
        Me.btnStartRemoteApp.Text = "Executer"
        Me.btnStartRemoteApp.UseVisualStyleBackColor = True
        '
        'txtSoftwarePath
        '
        Me.txtSoftwarePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtSoftwarePath.Location = New System.Drawing.Point(862, 80)
        Me.txtSoftwarePath.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.txtSoftwarePath.Name = "txtSoftwarePath"
        Me.txtSoftwarePath.Size = New System.Drawing.Size(132, 26)
        Me.txtSoftwarePath.TabIndex = 10
        '
        'btnLocked
        '
        Me.btnLocked.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLocked.Location = New System.Drawing.Point(422, 301)
        Me.btnLocked.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.btnLocked.Name = "btnLocked"
        Me.btnLocked.Size = New System.Drawing.Size(265, 32)
        Me.btnLocked.TabIndex = 11
        Me.btnLocked.Text = "Bloquer le pc a distance"
        Me.btnLocked.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 4)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 18)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Logs"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(159, 18)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "Adresse IP Du pc distant"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 78)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 18)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Message"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.btnSendNotification)
        Me.GroupBox1.Controls.Add(Me.txtMessageToSend)
        Me.GroupBox1.Controls.Add(Me.txtServerIP)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(422, 1)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(265, 187)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Notification"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnStopServer)
        Me.GroupBox2.Controls.Add(Me.btnStartServer)
        Me.GroupBox2.Controls.Add(Me.lblServerStatus)
        Me.GroupBox2.Location = New System.Drawing.Point(422, 194)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(265, 100)
        Me.GroupBox2.TabIndex = 16
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Serveur"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label4)
        Me.GroupBox3.Controls.Add(Me.txtRemoteAppToStart)
        Me.GroupBox3.Controls.Add(Me.btnStartRemoteApp)
        Me.GroupBox3.Location = New System.Drawing.Point(422, 337)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(265, 119)
        Me.GroupBox3.TabIndex = 17
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Applications"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 22)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(155, 18)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Chemain de l'application"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 18.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(692, 460)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnLocked)
        Me.Controls.Add(Me.txtSoftwarePath)
        Me.Controls.Add(Me.rtbLog)
        Me.Font = New System.Drawing.Font("Comic Sans MS", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSendNotification As Button
    Friend WithEvents btnStartServer As Button
    Friend WithEvents btnStopServer As Button
    Friend WithEvents NotifyIcon1 As NotifyIcon
    Friend WithEvents txtMessageToSend As TextBox
    Friend WithEvents txtServerIP As TextBox
    Friend WithEvents lblServerStatus As Label
    Friend WithEvents rtbLog As RichTextBox
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents txtRemoteAppToStart As TextBox
    Friend WithEvents btnStartRemoteApp As Button
    Friend WithEvents txtSoftwarePath As TextBox
    Friend WithEvents btnLocked As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents Label4 As Label
End Class
