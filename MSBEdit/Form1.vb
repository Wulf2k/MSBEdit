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

        Dim mapstudioPtr As UInteger
        Dim mapstudioCnt As UInteger


        If UIntFromBytes(&H8) > &H10000 Then bigEndian = False

        modelPtr = UIntFromBytes(&H4)
        modelCnt = UIntFromBytes(&H8)


        eventPtr = UIntFromBytes((modelCnt * &H4) + &H8)
        eventCnt = UIntFromBytes(eventPtr + &H8)

        pointPtr = UIntFromBytes((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromBytes(pointPtr + &H8)

        partsPtr = UIntFromBytes((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromBytes(partsPtr + &H8)





        dgvParts.Rows.Clear()
        dgvParts.Columns.Clear()

        dgvParts.Columns.Add("Unknown 1", "Unknown 1")
        dgvParts.Columns.Add("Object type", "Object type")
        dgvParts.Columns.Add("Type index", "Type index")
        dgvParts.Columns.Add("Unknown 2", "Unknown 2")

        dgvParts.Columns.Add("Unknown 3", "Unknown 3")
        dgvParts.Columns.Add("X pos", "X pos")
        dgvParts.Columns.Add("Y pos", "Y pos")
        dgvParts.Columns.Add("Z pos", "Z pos")

        dgvParts.Columns.Add("Unknown 4", "Unknown 4")
        dgvParts.Columns.Add("Facing", "Facing")
        dgvParts.Columns.Add("Unknown 5", "Unknown 5")
        dgvParts.Columns.Add("Unknown 6", "Unknown 6")  '1.0

        dgvParts.Columns.Add("Unknown 7", "Unknown 7")  '1.0
        dgvParts.Columns.Add("Unknown 8", "Unknown 8")  '1.0
        dgvParts.Columns.Add("Unknown 9", "Unknown 9")  '1 for h
        dgvParts.Columns.Add("Unknown 10", "Unknown 10")    '0

        dgvParts.Columns.Add("Unknown 11", "Unknown 11")    '0
        dgvParts.Columns.Add("Unknown 12", "Unknown 12")    '0
        dgvParts.Columns.Add("Unknown 13", "Unknown 13")    '1 for h
        dgvParts.Columns.Add("Unknown 14", "Unknown 14")    '0s

        dgvParts.Columns.Add("Unknown 15", "Unknown 15")    '0
        dgvParts.Columns.Add("Unknown 16", "Unknown 16")    '0
        dgvParts.Columns.Add("Unknown 17", "Unknown 17")    'something
        dgvParts.Columns.Add("Unknown 18", "Unknown 18")    'something

        dgvParts.Columns.Add("Unknown 19", "Unknown 19")
        dgvParts.Columns.Add("Unknown 20", "Unknown 20")

        dgvParts.Columns.Add("Object name", "Object name")
        dgvParts.Columns.Add("SIB path", "SIB path")

        dgvParts.Columns.Add("ID in scripts", "ID in scripts")
        dgvParts.Columns.Add("Unknown 23", "Unknown 23")    '0
        dgvParts.Columns.Add("Unknown 24", "Unknown 24")    '0

        dgvParts.Columns.Add("Unknown 25", "Unknown 25")    '00010100

        dgvParts.Columns.Add("Unknown 26", "Unknown 26")    '0
        dgvParts.Columns.Add("Unknown 27", "Unknown 27")    '0

        dgvParts.Columns.Add("Unknown 28", "Unknown 28")   '08000000 for h

        dgvParts.Columns.Add("Unknown 29", "Unknown 29")    '0
        dgvParts.Columns.Add("Unknown 30", "Unknown 30")    '00000001 for h

        dgvParts.Columns.Add("NPC ID", "NPC ID")    '0

        dgvParts.Columns.Add("ID in scripts#2", "ID in scripts#2")    '0
        dgvParts.Columns.Add("Unknown 33", "Unknown 33")    '0
        dgvParts.Columns.Add("Unknown 34", "Unknown 34")    '0008ffff for h
        dgvParts.Columns.Add("Unknown 35", "Unknown 35")

        dgvParts.Columns.Add("Unknown 36", "Unknown 36")
        dgvParts.Columns.Add("Unknown 37", "Unknown 37")
        dgvParts.Columns.Add("Unknown 38", "Unknown 38")
        dgvParts.Columns.Add("Unknown 39", "Unknown 39")

        dgvParts.Columns.Add("Unknown 40", "Unknown 40")
        dgvParts.Columns.Add("Unknown 41", "Unknown 41")    '-1s for h
        dgvParts.Columns.Add("Unknown 42", "Unknown 42")
        dgvParts.Columns.Add("Unknown 43", "Unknown 43")

        dgvParts.Columns.Add("Unknown 44", "Unknown 44")
        dgvParts.Columns.Add("Unknown 45", "Unknown 45")
        dgvParts.Columns.Add("Unknown 46", "Unknown 46")    '0s for h





        mapstudioPtr = UIntFromBytes((partsCnt * &H4) + &H8 + partsPtr)
        mapstudioCnt = UIntFromBytes(mapstudioPtr + &H8)






    End Sub
End Class
