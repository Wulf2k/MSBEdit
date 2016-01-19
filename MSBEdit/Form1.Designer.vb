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
        Me.dgvModel = New System.Windows.Forms.DataGridView()
        Me.tabParams = New System.Windows.Forms.TabControl()
        Me.tabModel = New System.Windows.Forms.TabPage()
        Me.tabEvent = New System.Windows.Forms.TabPage()
        Me.tabPoint = New System.Windows.Forms.TabPage()
        Me.tabParts = New System.Windows.Forms.TabPage()
        Me.tabMapstudio = New System.Windows.Forms.TabPage()
        Me.dgvEvent = New System.Windows.Forms.DataGridView()
        Me.dgvPoint = New System.Windows.Forms.DataGridView()
        Me.dgvParts = New System.Windows.Forms.DataGridView()
        Me.dgvMapstudio = New System.Windows.Forms.DataGridView()
        CType(Me.dgvModel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabParams.SuspendLayout()
        Me.tabModel.SuspendLayout()
        Me.tabEvent.SuspendLayout()
        Me.tabPoint.SuspendLayout()
        Me.tabParts.SuspendLayout()
        Me.tabMapstudio.SuspendLayout()
        CType(Me.dgvEvent, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvPoint, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvParts, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvMapstudio, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'dgvModel
        '
        Me.dgvModel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvModel.Location = New System.Drawing.Point(6, 6)
        Me.dgvModel.Name = "dgvModel"
        Me.dgvModel.Size = New System.Drawing.Size(665, 626)
        Me.dgvModel.TabIndex = 37
        '
        'tabParams
        '
        Me.tabParams.Controls.Add(Me.tabModel)
        Me.tabParams.Controls.Add(Me.tabEvent)
        Me.tabParams.Controls.Add(Me.tabPoint)
        Me.tabParams.Controls.Add(Me.tabParts)
        Me.tabParams.Controls.Add(Me.tabMapstudio)
        Me.tabParams.Location = New System.Drawing.Point(12, 70)
        Me.tabParams.Name = "tabParams"
        Me.tabParams.SelectedIndex = 0
        Me.tabParams.Size = New System.Drawing.Size(685, 664)
        Me.tabParams.TabIndex = 38
        '
        'tabModel
        '
        Me.tabModel.Controls.Add(Me.dgvModel)
        Me.tabModel.Location = New System.Drawing.Point(4, 22)
        Me.tabModel.Name = "tabModel"
        Me.tabModel.Padding = New System.Windows.Forms.Padding(3)
        Me.tabModel.Size = New System.Drawing.Size(677, 638)
        Me.tabModel.TabIndex = 0
        Me.tabModel.Text = "MODEL_PARAM_ST"
        Me.tabModel.UseVisualStyleBackColor = True
        '
        'tabEvent
        '
        Me.tabEvent.Controls.Add(Me.dgvEvent)
        Me.tabEvent.Location = New System.Drawing.Point(4, 22)
        Me.tabEvent.Name = "tabEvent"
        Me.tabEvent.Padding = New System.Windows.Forms.Padding(3)
        Me.tabEvent.Size = New System.Drawing.Size(677, 638)
        Me.tabEvent.TabIndex = 1
        Me.tabEvent.Text = "EVENT_PARAM_ST"
        Me.tabEvent.UseVisualStyleBackColor = True
        '
        'tabPoint
        '
        Me.tabPoint.Controls.Add(Me.dgvPoint)
        Me.tabPoint.Location = New System.Drawing.Point(4, 22)
        Me.tabPoint.Name = "tabPoint"
        Me.tabPoint.Size = New System.Drawing.Size(677, 638)
        Me.tabPoint.TabIndex = 2
        Me.tabPoint.Text = "POINT_PARAM_ST"
        Me.tabPoint.UseVisualStyleBackColor = True
        '
        'tabParts
        '
        Me.tabParts.Controls.Add(Me.dgvParts)
        Me.tabParts.Location = New System.Drawing.Point(4, 22)
        Me.tabParts.Name = "tabParts"
        Me.tabParts.Size = New System.Drawing.Size(677, 638)
        Me.tabParts.TabIndex = 3
        Me.tabParts.Text = "PARTS_PARAM_ST"
        Me.tabParts.UseVisualStyleBackColor = True
        '
        'tabMapstudio
        '
        Me.tabMapstudio.Controls.Add(Me.dgvMapstudio)
        Me.tabMapstudio.Location = New System.Drawing.Point(4, 22)
        Me.tabMapstudio.Name = "tabMapstudio"
        Me.tabMapstudio.Size = New System.Drawing.Size(677, 638)
        Me.tabMapstudio.TabIndex = 4
        Me.tabMapstudio.Text = "MAPSTUDIO_TREE_ST"
        Me.tabMapstudio.UseVisualStyleBackColor = True
        '
        'dgvEvent
        '
        Me.dgvEvent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEvent.Location = New System.Drawing.Point(6, 6)
        Me.dgvEvent.Name = "dgvEvent"
        Me.dgvEvent.Size = New System.Drawing.Size(665, 626)
        Me.dgvEvent.TabIndex = 39
        '
        'dgvPoint
        '
        Me.dgvPoint.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPoint.Location = New System.Drawing.Point(6, 6)
        Me.dgvPoint.Name = "dgvPoint"
        Me.dgvPoint.Size = New System.Drawing.Size(665, 626)
        Me.dgvPoint.TabIndex = 38
        '
        'dgvParts
        '
        Me.dgvParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvParts.Location = New System.Drawing.Point(6, 6)
        Me.dgvParts.Name = "dgvParts"
        Me.dgvParts.Size = New System.Drawing.Size(665, 626)
        Me.dgvParts.TabIndex = 38
        '
        'dgvMapstudio
        '
        Me.dgvMapstudio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMapstudio.Location = New System.Drawing.Point(6, 6)
        Me.dgvMapstudio.Name = "dgvMapstudio"
        Me.dgvMapstudio.Size = New System.Drawing.Size(665, 626)
        Me.dgvMapstudio.TabIndex = 38
        '
        'frmMSBEdit
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(706, 746)
        Me.Controls.Add(Me.tabParams)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnOpen)
        Me.Controls.Add(Me.txtMSBfile)
        Me.Controls.Add(Me.lblGAFile)
        Me.Name = "frmMSBEdit"
        Me.Text = "Wulf's MSB Editor"
        CType(Me.dgvModel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabParams.ResumeLayout(False)
        Me.tabModel.ResumeLayout(False)
        Me.tabEvent.ResumeLayout(False)
        Me.tabPoint.ResumeLayout(False)
        Me.tabParts.ResumeLayout(False)
        Me.tabMapstudio.ResumeLayout(False)
        CType(Me.dgvEvent, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvPoint, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvParts, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvMapstudio, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents txtMSBfile As System.Windows.Forms.TextBox
    Friend WithEvents lblGAFile As System.Windows.Forms.Label
    Friend WithEvents dgvModel As System.Windows.Forms.DataGridView
    Friend WithEvents tabParams As System.Windows.Forms.TabControl
    Friend WithEvents tabModel As System.Windows.Forms.TabPage
    Friend WithEvents tabEvent As System.Windows.Forms.TabPage
    Friend WithEvents tabPoint As System.Windows.Forms.TabPage
    Friend WithEvents tabParts As System.Windows.Forms.TabPage
    Friend WithEvents tabMapstudio As System.Windows.Forms.TabPage
    Friend WithEvents dgvEvent As System.Windows.Forms.DataGridView
    Friend WithEvents dgvPoint As System.Windows.Forms.DataGridView
    Friend WithEvents dgvParts As System.Windows.Forms.DataGridView
    Friend WithEvents dgvMapstudio As System.Windows.Forms.DataGridView

End Class
