Imports System.IO

Public Class frmMSBEdit

    Public Shared bytes() As Byte
    Public Shared bigEndian As Boolean = True

    Private Sub txt_Drop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtMSBfile.DragDrop
        Dim file() As String = e.Data.GetData(DataFormats.FileDrop)
        sender.Text = file(0)
    End Sub
    Private Sub txt_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtMSBfile.DragEnter
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Function StrFromBytes(ByVal loc As UInteger) As String
        Dim Str As String = ""
        Dim cont As Boolean = True

        While cont
            If bytes(loc) > 0 Then
                Str = Str + Convert.ToChar(bytes(loc))
                loc += 1
            Else
                cont = False
            End If
        End While

        Return Str
    End Function
    Private Function UIntFromBytes(ByVal loc As UInteger) As UInteger
        Dim tmpUint As UInteger = 0

        If bigEndian Then
            For i = 0 To 3
                tmpUint += Convert.ToUInt32(bytes(loc + i)) * &H100 ^ (3 - i)
            Next
        Else
            For i = 0 To 3
                tmpUint += Convert.ToUInt32(bytes(loc + 3 - i)) * &H100 ^ (3 - i)
            Next
        End If

        Return tmpUint
    End Function

    Private Sub InsBytes(ByVal bytes2() As Byte, ByVal loc As UInteger)
        Array.Copy(bytes2, 0, bytes, loc, bytes2.Length)
    End Sub
    Private Sub StrToBytes(ByVal str As String, ByVal loc As UInteger)
        Dim BArr() As Byte
        BArr = System.Text.Encoding.ASCII.GetBytes(str)

        Array.Copy(BArr, 0, bytes, loc, BArr.Length)
    End Sub
    Private Sub UINTToBytes(ByVal val As UInteger, loc As UInteger)
        Dim BArr(3) As Byte

        If bigEndian Then
            For i = 0 To 3
                BArr(i) = Math.Floor(val / (&H100 ^ (3 - i))) Mod &H100
            Next
        Else
            For i = 0 To 3
                BArr(3 - i) = Math.Floor(val / (&H100 ^ (3 - i))) Mod &H100
            Next
        End If
        Array.Copy(BArr, 0, bytes, loc, 4)
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        bytes = File.ReadAllBytes(txtMSBfile.Text)

        Dim modelPtr As UInteger
        Dim modelCnt As UInteger

        Dim eventPtr As UInteger
        Dim eventCnt As UInteger

        Dim pointPtr As UInteger
        Dim pointCnt As UInteger

        Dim partsPtr As UInteger
        Dim partsCnt As UInteger

        If UIntFromBytes(&H8) > &H10000 Then bigEndian = False

        modelPtr = UIntFromBytes(&H4)
        modelCnt = UIntFromBytes(&H8)


        eventPtr = UIntFromBytes((modelCnt * &H4) + &H8)
        eventCnt = UIntFromBytes(eventPtr + &H8)

        pointPtr = UIntFromBytes((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromBytes(pointPtr + &H8)

        partsPtr = UIntFromBytes((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromBytes(partsPtr + &H8)

        MsgBox(Hex(partsPtr))





    End Sub
End Class
