Module Program
    Public Sub Main(ByVal args() As String)
        Application.EnableVisualStyles()
        Dim mainForm As New frmMSBEdit
        If args.Length > 0 Then
            frmMSBEdit.AutoOpenMsbFile = args(0)
        End If
        Application.Run(mainForm)
    End Sub
End Module
