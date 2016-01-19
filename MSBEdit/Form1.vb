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

    Private Function Int16ToTwoByte(ByVal val As Integer) As Byte()
        If bigEndian Then
            Return ReverseTwoBytes(BitConverter.GetBytes(Convert.ToInt16(val)))
        Else
            Return BitConverter.GetBytes(Convert.ToInt16(val))
        End If
    End Function
    Private Function Int32ToFourByte(ByVal val As Integer) As Byte()
        If bigEndian Then
            Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToInt32(val)))
        Else
            Return BitConverter.GetBytes(Convert.ToInt32(val))
        End If
    End Function
    Private Function UInt16TotwoByte(ByVal val As UInteger) As Byte()
        If bigEndian Then
            Return ReverseTwoBytes(BitConverter.GetBytes(Convert.ToUInt16(val)))
        Else
            Return BitConverter.GetBytes(Convert.ToUInt16(val))
        End If
    End Function
    Private Function UInt32ToFourByte(ByVal val As UInteger) As Byte()
        If bigEndian Then
            Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToUInt32(val)))
        Else
            Return BitConverter.GetBytes(Convert.ToUInt32(val))
        End If
    End Function
    Private Function SingleToFourByte(ByVal val As String) As Byte()
        If IsNumeric(val) Then
            If bigEndian Then
                Return ReverseFourBytes(BitConverter.GetBytes(Convert.ToSingle(val)))
            Else
                Return BitConverter.GetBytes(Convert.ToSingle(val))
            End If
        Else
            Return {0, 0, 0, 0}
        End If
    End Function
    Private Function ReverseFourBytes(ByVal byt() As Byte)
        Return {byt(3), byt(2), byt(1), byt(0)}
    End Function
    Private Function ReverseTwoBytes(ByVal byt() As Byte)
        Return {byt(1), byt(0)}
    End Function
    Private Sub InsBytes(ByVal loc As UInteger, ByVal byt As Byte())
        For i = 0 To byt.Length - 1
            bytes(loc + i) = byt(i)
        Next
    End Sub

    Private Function SingleFromFour(ByVal loc As UInteger) As Single
        Dim bArray(3) As Byte

        For i = 0 To 3
            bArray(3 - i) = bytes(loc + i)
        Next
        If Not bigEndian Then bArray = ReverseFourBytes(bArray)
        Return BitConverter.ToSingle(bArray, 0)
    End Function
    Private Function SIntFromTwo(ByVal loc As UInteger) As Int16
        Dim tmpint As Integer = 0
        Dim bArray(1) As Byte

        For i = 0 To 1
            bArray(1 - i) = bytes(loc + i)
        Next
        If Not bigEndian Then bArray = ReverseTwoBytes(bArray)
        tmpint = BitConverter.ToInt16(bArray, 0)
        Return tmpint
    End Function
    Private Function SIntFromFour(ByVal loc As UInteger) As Integer
        Dim tmpint As Integer = 0
        Dim bArray(3) As Byte

        For i = 0 To 3
            bArray(3 - i) = bytes(loc + i)
        Next
        If Not bigEndian Then bArray = ReverseFourBytes(bArray)
        tmpint = BitConverter.ToInt32(bArray, 0)
        Return tmpint
    End Function
    Private Function UIntFromTwo(ByVal loc As UInteger) As UInteger
        Dim tmpUint As UInteger = 0

        If bigEndian Then
            For i = 0 To 1
                tmpUint += Convert.ToUInt16(bytes(loc + i)) * &H100 ^ (1 - i)
            Next
        Else
            For i = 0 To 1
                tmpUint += Convert.ToUInt16(bytes(loc + i)) * &H100 ^ (i)
            Next
        End If

        Return tmpUint
    End Function
    Private Function UIntFromFour(ByVal loc As UInteger) As UInteger
        Dim tmpUint As UInteger = 0

        If bigEndian Then
            For i = 0 To 3
                tmpUint += Convert.ToUInt32(bytes(loc + i)) * &H100 ^ (3 - i)
            Next
        Else
            For i = 0 To 3
                tmpUint += Convert.ToUInt32(bytes(loc + i)) * &H100 ^ (i)
            Next
        End If

        Return tmpUint
    End Function

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

        bigEndian = True
        If UIntFromFour(&H8) > &H10000 Then
            bigEndian = False
        End If


        modelPtr = UIntFromFour(&H4)
        modelCnt = UIntFromFour(&H8)


        eventPtr = UIntFromFour((modelCnt * &H4) + &H8)
        eventCnt = UIntFromFour(eventPtr + &H8)

        pointPtr = UIntFromFour((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromFour(pointPtr + &H8)

        partsPtr = UIntFromFour((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromFour(partsPtr + &H8)



        Dim ptr As UInteger
        Dim crtindex As UInteger
        Dim xpos As Single
        Dim ypos As Single
        Dim zpos As Single
        Dim facing As Single

        Dim model As String
        Dim sibpath As String

        Dim scriptid1 As Integer
        Dim scriptid2 As Integer
        Dim npcid1 As Integer
        Dim npcid2 As Integer

        Dim offset As UInteger

        Dim padding As UInteger

        Dim row(12) As String

        dgvCreature.Rows.Clear()
        dgvCreature.Columns.Clear()

        dgvCreature.Columns.Add("Index", "Index")

        dgvCreature.Columns.Add("X pos", "X pos")
        dgvCreature.Columns.Add("Y pos", "Y pos")
        dgvCreature.Columns.Add("Z pos", "Z pos")

        dgvCreature.Columns.Add("Facing", "Facing")
        dgvCreature.Columns.Add("Model", "Model")
        dgvCreature.Columns.Add("SibPath", "SibPath")

        dgvCreature.Columns.Add("Script ID #1", "Script ID #1")
        'dgvCreature.Columns.Add("Script ID #2", "Script ID #2")

        dgvCreature.Columns.Add("NPC ID #1", "NPC ID #1")
        'dgvCreature.Columns.Add("NPC ID #2", "NPC ID #2")

        dgvCreature.Columns.Add("Offset", "Offset")

        For i = 0 To partsCnt - 1
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)
            Select Case UIntFromFour(ptr + &H4)
                Case &H2

                    If bigEndian Then padding = &H4

                    crtindex = UIntFromFour(ptr + &H8)
                    xpos = SingleFromFour(ptr + &H14)
                    ypos = SingleFromFour(ptr + &H18)
                    zpos = SingleFromFour(ptr + &H1C)

                    facing = SingleFromFour(ptr + &H24)

                    model = StrFromBytes(ptr + &H64 + padding)
                    sibpath = StrFromBytes(ptr + &H64 + model.Length + 1 + padding)

                    If sibpath.Length = 0 Then padding += 4
                    If Not bigEndian Then padding += 4

                    If Not ((sibpath.Length + model.Length + 2) Mod 4) = 0 Then
                        padding += sibpath.Length + model.Length + 2
                        padding += (4 - (padding Mod 4))
                    Else
                        padding += sibpath.Length + model.Length + 2
                    End If

                    scriptid1 = SIntFromFour(ptr + &H64 + padding)
                    npcid1 = SIntFromFour(ptr + &H64 + padding + &H24)

                    offset = ptr

                    row(0) = crtindex
                    row(1) = xpos
                    row(2) = ypos
                    row(3) = zpos
                    row(4) = facing
                    row(5) = model
                    row(6) = sibpath
                    row(7) = scriptid1
                    row(8) = npcid1

                    row(9) = offset

                    dgvCreature.Rows.Add(row)
            End Select
        Next

        mapstudioPtr = UIntFromFour((partsCnt * &H4) + &H8 + partsPtr)
        mapstudioCnt = UIntFromFour(mapstudioPtr + &H8)

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
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

        bigEndian = True
        If UIntFromFour(&H8) > &H10000 Then
            bigEndian = False
        End If


        modelPtr = UIntFromFour(&H4)
        modelCnt = UIntFromFour(&H8)


        eventPtr = UIntFromFour((modelCnt * &H4) + &H8)
        eventCnt = UIntFromFour(eventPtr + &H8)

        pointPtr = UIntFromFour((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromFour(pointPtr + &H8)

        partsPtr = UIntFromFour((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromFour(partsPtr + &H8)



        Dim ptr As UInteger
        Dim crtindex As UInteger
        Dim xpos As Single
        Dim ypos As Single
        Dim zpos As Single
        Dim facing As Single

        Dim model As String
        Dim sibpath As String

        Dim scriptid1 As Integer
        Dim scriptid2 As Integer
        Dim npcid1 As Integer
        Dim npcid2 As Integer

        Dim offset As UInteger

        Dim padding As UInteger

        For i = 0 To dgvCreature.Rows.Count - 2
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)

            If bigEndian Then padding = &H4

            crtindex = dgvCreature.Rows(i).Cells(0).Value

            xpos = dgvCreature.Rows(i).Cells(1).Value
            ypos = dgvCreature.Rows(i).Cells(2).Value
            zpos = dgvCreature.Rows(i).Cells(3).Value

            facing = dgvCreature.Rows(i).Cells(4).Value

            model = dgvCreature.Rows(i).Cells(5).Value
            sibpath = dgvCreature.Rows(i).Cells(6).Value

            scriptid1 = dgvCreature.Rows(i).Cells(7).Value
            npcid1 = dgvCreature.Rows(i).Cells(8).Value
            ptr = dgvCreature.Rows(i).Cells(9).Value

            InsBytes(ptr + &H8, UInt32ToFourByte(crtindex))
            InsBytes(ptr + &H14, SingleToFourByte(xpos))
            InsBytes(ptr + &H18, SingleToFourByte(ypos))
            InsBytes(ptr + &H1C, SingleToFourByte(zpos))
            InsBytes(ptr + &H24, SingleToFourByte(facing))

            InsBytes(ptr + &H64 + padding, System.Text.Encoding.ASCII.GetBytes(model))
            InsBytes(ptr + &H64 + padding + model.Length + 1, System.Text.Encoding.ASCII.GetBytes(sibpath))

            If sibpath.Length = 0 Then padding += 4
            If Not bigEndian Then padding += 4

            If Not ((sibpath.Length + model.Length + 2) Mod 4) = 0 Then
                padding += sibpath.Length + model.Length + 2
                padding += (4 - (padding Mod 4))
            Else
                padding += sibpath.Length + model.Length + 2
            End If

            InsBytes(ptr + &H64 + padding, Int32ToFourByte(scriptid1))
            InsBytes(ptr + &H64 + padding + &H24, Int32ToFourByte(npcid1))
        Next



        File.WriteAllBytes(txtMSBfile.Text, bytes)

        MsgBox("Save Complete.")
    End Sub
End Class
