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
        Me.dgvCreatures = New System.Windows.Forms.DataGridView()
        Me.tabParams = New System.Windows.Forms.TabControl()
        Me.tabCreatures = New System.Windows.Forms.TabPage()
        Me.tabMapPieces = New System.Windows.Forms.TabPage()
        Me.dgvMapPieces = New System.Windows.Forms.DataGridView()
        Me.tabModels = New System.Windows.Forms.TabPage()
        Me.dgvModels = New System.Windows.Forms.DataGridView()
        Me.tabObjects = New System.Windows.Forms.TabPage()
        Me.dgvObjects = New System.Windows.Forms.DataGridView()
        Me.tabCollision0x5 = New System.Windows.Forms.TabPage()
        Me.dgvCollision0x5 = New System.Windows.Forms.DataGridView()
        Me.tabCollision0xB = New System.Windows.Forms.TabPage()
        Me.dgvCollision0xB = New System.Windows.Forms.DataGridView()
        Me.tabUnhandled = New System.Windows.Forms.TabPage()
        Me.dgvUnhandled = New System.Windows.Forms.DataGridView()
        CType(Me.dgvCreatures, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabParams.SuspendLayout()
        Me.tabCreatures.SuspendLayout()
        Me.tabMapPieces.SuspendLayout()
        CType(Me.dgvMapPieces, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabModels.SuspendLayout()
        CType(Me.dgvModels, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabObjects.SuspendLayout()
        CType(Me.dgvObjects, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabCollision0x5.SuspendLayout()
        CType(Me.dgvCollision0x5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabCollision0xB.SuspendLayout()
        CType(Me.dgvCollision0xB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabUnhandled.SuspendLayout()
        CType(Me.dgvUnhandled, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'dgvCreatures
        '
        Me.dgvCreatures.AllowUserToAddRows = False
        Me.dgvCreatures.AllowUserToDeleteRows = False
        Me.dgvCreatures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvCreatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCreatures.Location = New System.Drawing.Point(6, 6)
        Me.dgvCreatures.Name = "dgvCreatures"
        Me.dgvCreatures.Size = New System.Drawing.Size(1107, 626)
        Me.dgvCreatures.TabIndex = 37
        '
        'tabParams
        '
        Me.tabParams.Controls.Add(Me.tabCreatures)
        Me.tabParams.Controls.Add(Me.tabMapPieces)
        Me.tabParams.Controls.Add(Me.tabModels)
        Me.tabParams.Controls.Add(Me.tabObjects)
        Me.tabParams.Controls.Add(Me.tabCollision0x5)
        Me.tabParams.Controls.Add(Me.tabCollision0xB)
        Me.tabParams.Controls.Add(Me.tabUnhandled)
        Me.tabParams.Location = New System.Drawing.Point(12, 69)
        Me.tabParams.Name = "tabParams"
        Me.tabParams.SelectedIndex = 0
        Me.tabParams.Size = New System.Drawing.Size(1127, 664)
        Me.tabParams.TabIndex = 38
        '
        'tabCreatures
        '
        Me.tabCreatures.Controls.Add(Me.dgvCreatures)
        Me.tabCreatures.Location = New System.Drawing.Point(4, 22)
        Me.tabCreatures.Name = "tabCreatures"
        Me.tabCreatures.Padding = New System.Windows.Forms.Padding(3)
        Me.tabCreatures.Size = New System.Drawing.Size(1119, 638)
        Me.tabCreatures.TabIndex = 0
        Me.tabCreatures.Text = "Creatures"
        Me.tabCreatures.UseVisualStyleBackColor = True
        '
        'tabMapPieces
        '
        Me.tabMapPieces.Controls.Add(Me.dgvMapPieces)
        Me.tabMapPieces.Location = New System.Drawing.Point(4, 22)
        Me.tabMapPieces.Name = "tabMapPieces"
        Me.tabMapPieces.Size = New System.Drawing.Size(1119, 638)
        Me.tabMapPieces.TabIndex = 4
        Me.tabMapPieces.Text = "Map Pieces"
        Me.tabMapPieces.UseVisualStyleBackColor = True
        '
        'dgvMapPieces
        '
        Me.dgvMapPieces.AllowUserToAddRows = False
        Me.dgvMapPieces.AllowUserToDeleteRows = False
        Me.dgvMapPieces.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvMapPieces.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMapPieces.Location = New System.Drawing.Point(6, 6)
        Me.dgvMapPieces.Name = "dgvMapPieces"
        Me.dgvMapPieces.Size = New System.Drawing.Size(1107, 626)
        Me.dgvMapPieces.TabIndex = 38
        '
        'tabModels
        '
        Me.tabModels.Controls.Add(Me.dgvModels)
        Me.tabModels.Location = New System.Drawing.Point(4, 22)
        Me.tabModels.Name = "tabModels"
        Me.tabModels.Size = New System.Drawing.Size(1119, 638)
        Me.tabModels.TabIndex = 2
        Me.tabModels.Text = "Models"
        Me.tabModels.UseVisualStyleBackColor = True
        '
        'dgvModels
        '
        Me.dgvModels.AllowUserToAddRows = False
        Me.dgvModels.AllowUserToDeleteRows = False
        Me.dgvModels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvModels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvModels.Location = New System.Drawing.Point(6, 6)
        Me.dgvModels.Name = "dgvModels"
        Me.dgvModels.Size = New System.Drawing.Size(1107, 626)
        Me.dgvModels.TabIndex = 38
        '
        'tabObjects
        '
        Me.tabObjects.Controls.Add(Me.dgvObjects)
        Me.tabObjects.Location = New System.Drawing.Point(4, 22)
        Me.tabObjects.Name = "tabObjects"
        Me.tabObjects.Size = New System.Drawing.Size(1119, 638)
        Me.tabObjects.TabIndex = 1
        Me.tabObjects.Text = "Objects"
        Me.tabObjects.UseVisualStyleBackColor = True
        '
        'dgvObjects
        '
        Me.dgvObjects.AllowUserToAddRows = False
        Me.dgvObjects.AllowUserToDeleteRows = False
        Me.dgvObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvObjects.Location = New System.Drawing.Point(6, 6)
        Me.dgvObjects.Name = "dgvObjects"
        Me.dgvObjects.Size = New System.Drawing.Size(1107, 626)
        Me.dgvObjects.TabIndex = 38
        '
        'tabCollision0x5
        '
        Me.tabCollision0x5.Controls.Add(Me.dgvCollision0x5)
        Me.tabCollision0x5.Location = New System.Drawing.Point(4, 22)
        Me.tabCollision0x5.Name = "tabCollision0x5"
        Me.tabCollision0x5.Size = New System.Drawing.Size(1119, 638)
        Me.tabCollision0x5.TabIndex = 6
        Me.tabCollision0x5.Text = "Collision0x5"
        Me.tabCollision0x5.UseVisualStyleBackColor = True
        '
        'dgvCollision0x5
        '
        Me.dgvCollision0x5.AllowUserToAddRows = False
        Me.dgvCollision0x5.AllowUserToDeleteRows = False
        Me.dgvCollision0x5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvCollision0x5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCollision0x5.Location = New System.Drawing.Point(6, 6)
        Me.dgvCollision0x5.Name = "dgvCollision0x5"
        Me.dgvCollision0x5.Size = New System.Drawing.Size(1107, 626)
        Me.dgvCollision0x5.TabIndex = 39
        '
        'tabCollision0xB
        '
        Me.tabCollision0xB.Controls.Add(Me.dgvCollision0xB)
        Me.tabCollision0xB.Location = New System.Drawing.Point(4, 22)
        Me.tabCollision0xB.Name = "tabCollision0xB"
        Me.tabCollision0xB.Size = New System.Drawing.Size(1119, 638)
        Me.tabCollision0xB.TabIndex = 3
        Me.tabCollision0xB.Text = "Collision0xB"
        Me.tabCollision0xB.UseVisualStyleBackColor = True
        '
        'dgvCollision0xB
        '
        Me.dgvCollision0xB.AllowUserToAddRows = False
        Me.dgvCollision0xB.AllowUserToDeleteRows = False
        Me.dgvCollision0xB.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvCollision0xB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCollision0xB.Location = New System.Drawing.Point(6, 6)
        Me.dgvCollision0xB.Name = "dgvCollision0xB"
        Me.dgvCollision0xB.Size = New System.Drawing.Size(1107, 626)
        Me.dgvCollision0xB.TabIndex = 38
        '
        'tabUnhandled
        '
        Me.tabUnhandled.Controls.Add(Me.dgvUnhandled)
        Me.tabUnhandled.Location = New System.Drawing.Point(4, 22)
        Me.tabUnhandled.Name = "tabUnhandled"
        Me.tabUnhandled.Size = New System.Drawing.Size(1119, 638)
        Me.tabUnhandled.TabIndex = 5
        Me.tabUnhandled.Text = "Unhandled"
        Me.tabUnhandled.UseVisualStyleBackColor = True
        '
        'dgvUnhandled
        '
        Me.dgvUnhandled.AllowUserToAddRows = False
        Me.dgvUnhandled.AllowUserToDeleteRows = False
        Me.dgvUnhandled.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgvUnhandled.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvUnhandled.Location = New System.Drawing.Point(6, 6)
        Me.dgvUnhandled.Name = "dgvUnhandled"
        Me.dgvUnhandled.Size = New System.Drawing.Size(1107, 626)
        Me.dgvUnhandled.TabIndex = 38
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
        Me.Text = "Wulf's MSB Editor 2016-07-19"
        CType(Me.dgvCreatures, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabParams.ResumeLayout(False)
        Me.tabCreatures.ResumeLayout(False)
        Me.tabMapPieces.ResumeLayout(False)
        CType(Me.dgvMapPieces, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabModels.ResumeLayout(False)
        CType(Me.dgvModels, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabObjects.ResumeLayout(False)
        CType(Me.dgvObjects, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabCollision0x5.ResumeLayout(False)
        CType(Me.dgvCollision0x5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabCollision0xB.ResumeLayout(False)
        CType(Me.dgvCollision0xB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabUnhandled.ResumeLayout(False)
        CType(Me.dgvUnhandled, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents txtMSBfile As System.Windows.Forms.TextBox
    Friend WithEvents lblGAFile As System.Windows.Forms.Label
    Friend WithEvents dgvCreatures As System.Windows.Forms.DataGridView
    Friend WithEvents tabParams As System.Windows.Forms.TabControl
    Friend WithEvents tabCreatures As System.Windows.Forms.TabPage
    Friend WithEvents tabObjects As System.Windows.Forms.TabPage
    Friend WithEvents dgvObjects As System.Windows.Forms.DataGridView
    Friend WithEvents tabModels As System.Windows.Forms.TabPage
    Friend WithEvents dgvModels As System.Windows.Forms.DataGridView
    Friend WithEvents tabCollision0xB As System.Windows.Forms.TabPage
    Friend WithEvents dgvCollision0xB As System.Windows.Forms.DataGridView
    Friend WithEvents tabMapPieces As System.Windows.Forms.TabPage
    Friend WithEvents dgvMapPieces As System.Windows.Forms.DataGridView
    Friend WithEvents tabUnhandled As System.Windows.Forms.TabPage
    Friend WithEvents dgvUnhandled As System.Windows.Forms.DataGridView
    Friend WithEvents tabCollision0x5 As System.Windows.Forms.TabPage
    Friend WithEvents dgvCollision0x5 As System.Windows.Forms.DataGridView

End Class
