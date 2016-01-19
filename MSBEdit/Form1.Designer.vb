<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSBEdit
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.txtMSBfile = New System.Windows.Forms.TextBox()
        Me.lblGAFile = New System.Windows.Forms.Label()
        Me.dgvCreature = New System.Windows.Forms.DataGridView()
        Me.tabParams = New System.Windows.Forms.TabControl()
        Me.tabCreatures = New System.Windows.Forms.TabPage()
        CType(Me.dgvCreature, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabParams.SuspendLayout()
        Me.tabCreatures.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(618, 40)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 35
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnOpen
        '
        Me.btnOpen.Location = New System.Drawing.Point(540, 40)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(75, 23)
        Me.btnOpen.TabIndex = 34
        Me.btnOpen.Text = "Open"
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'txtMSBfile
        '
        Me.txtMSBfile.AllowDrop = True
        Me.txtMSBfile.Location = New System.Drawing.Point(46, 14)
        Me.txtMSBfile.Name = "txtMSBfile"
        Me.txtMSBfile.Size = New System.Drawing.Size(647, 20)
        Me.txtMSBfile.TabIndex = 31
        '
        'lblGAFile
        '
        Me.lblGAFile.AutoSize = True
        Me.lblGAFile.Location = New System.Drawing.Point(14, 17)
        Me.lblGAFile.Name = "lblGAFile"
        Me.lblGAFile.Size = New System.Drawing.Size(26, 13)
        Me.lblGAFile.TabIndex = 33
        Me.lblGAFile.Text = "File:"
        '
        'dgvCreature
        '
        Me.dgvCreature.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCreature.Location = New System.Drawing.Point(6, 6)
        Me.dgvCreature.Name = "dgvCreature"
        Me.dgvCreature.Size = New System.Drawing.Size(1107, 626)
        Me.dgvCreature.TabIndex = 37
        '
        'tabParams
        '
        Me.tabParams.Controls.Add(Me.tabCreatures)
        Me.tabParams.Location = New System.Drawing.Point(12, 70)
        Me.tabParams.Name = "tabParams"
        Me.tabParams.SelectedIndex = 0
        Me.tabParams.Size = New System.Drawing.Size(1127, 664)
        Me.tabParams.TabIndex = 38
        '
        'tabCreatures
        '
        Me.tabCreatures.Controls.Add(Me.dgvCreature)
        Me.tabCreatures.Location = New System.Drawing.Point(4, 22)
        Me.tabCreatures.Name = "tabCreatures"
        Me.tabCreatures.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCreatures.Size = New System.Drawing.Size(1119, 638)
        Me.tabCreatures.TabIndex = 0
        Me.tabCreatures.Text = "Creatures"
        Me.tabCreatures.UseVisualStyleBackColor = True
        '
        'frmMSBEdit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1151, 747)
        Me.Controls.Add(Me.tabParams)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnOpen)
        Me.Controls.Add(Me.txtMSBfile)
        Me.Controls.Add(Me.lblGAFile)
        Me.Name = "frmMSBEdit"
        Me.Text = "Wulf's MSB Editor v0.100"
        CType(Me.dgvCreature, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabParams.ResumeLayout(False)
        Me.tabCreatures.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents txtMSBfile As System.Windows.Forms.TextBox
    Friend WithEvents lblGAFile As System.Windows.Forms.Label
    Friend WithEvents dgvCreature As System.Windows.Forms.DataGridView
    Friend WithEvents tabParams As System.Windows.Forms.TabControl
    Friend WithEvents tabCreatures As System.Windows.Forms.TabPage

End Class
