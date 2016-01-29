Imports System.IO

Public Class frmMSBEdit

    Public Shared bytes() As Byte
    Public Shared bigEndian As Boolean = True

    Public Shared eventParams() As Byte = {}
    Public Shared eventParamsOrigOffset As UInteger

    Public Shared pointParams() As Byte = {}
    Public Shared pointParamsOrigOffset As UInteger

    Private Sub txt_Drop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtMSBfile.DragDrop
        Dim file() As String = e.Data.GetData(DataFormats.FileDrop)
        sender.Text = file(0)
    End Sub
    Private Sub txt_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles txtMSBfile.DragEnter
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub WriteBytes(ByRef fs As FileStream, ByVal byt() As Byte)
        For i = 0 To byt.Length - 1
            fs.WriteByte(byt(i))
        Next
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

    Private Function Str2Bytes(ByVal str As String) As Byte()
        Dim BArr() As Byte
        BArr = System.Text.Encoding.ASCII.GetBytes(str)
        Return BArr
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


        Dim ptr As UInteger

        Dim nameoffset As UInteger
        Dim type As UInteger
        Dim index As UInteger

        Dim xpos As Single
        Dim ypos As Single
        Dim zpos As Single
        Dim facing As Single

        Dim model As UInteger
        Dim name As String
        Dim sibpath As String

        Dim scriptid1 As Integer
        Dim scriptid2 As Integer
        Dim npcid1 As Integer
        Dim npcid2 As Integer

        Dim offset As UInteger
        Dim padding As UInteger
        Dim row(40) As String




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


        modelPtr = 0
        modelCnt = UIntFromFour(&H8)

        dgvModels.Rows.Clear()
        dgvModels.Columns.Clear()

        dgvModels.Columns.Add("Type", "Type")
        dgvModels.Columns(0).Width = 40
        dgvModels.Columns.Add("Index", "Index")
        dgvModels.Columns(1).Width = 40
        dgvModels.Columns.Add("Model #", "Model #")
        dgvModels.Columns(2).Width = 60
        dgvModels.Columns(2).DefaultCellStyle.BackColor = Color.LightGray
        dgvModels.Columns.Add("Model", "Model")
        dgvModels.Columns.Add("Sibpath", "Sibpath")
        dgvModels.Columns.Add("Unknown &H0C", "Unknown &H0C")
        dgvModels.Columns(5).Width = 60
        dgvModels.Columns.Add("Unknown &H10", "Unknown &H10")
        dgvModels.Columns(6).Width = 60
        dgvModels.Columns.Add("Offset", "Offset")
        dgvModels.Columns(7).DefaultCellStyle.BackColor = Color.LightGray
        dgvModels.Columns.Add("Name Offset", "Name Offset")
        dgvModels.Columns(8).Width = 60


        eventPtr = UIntFromFour((modelCnt * &H4) + &H8)
        eventCnt = UIntFromFour(eventPtr + &H8)

        pointPtr = UIntFromFour((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromFour(pointPtr + &H8)

        partsPtr = UIntFromFour((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromFour(partsPtr + &H8)

        eventParamsOrigOffset = eventPtr
        ReDim eventParams(pointPtr - eventPtr - 1)
        Array.Copy(bytes, eventPtr, eventParams, 0, eventParams.Length)

        pointParamsOrigOffset = pointPtr
        ReDim pointParams(partsPtr - pointPtr - 1)
        Array.Copy(bytes, pointPtr, pointParams, 0, pointParams.Length)



        dgvMapPieces.Rows.Clear()
        dgvMapPieces.Columns.Clear()

        dgvMapPieces.Columns.Add("Type", "Type")
        dgvMapPieces.Columns(0).Width = 40
        dgvMapPieces.Columns.Add("Index", "Index")
        dgvMapPieces.Columns(1).Width = 40
        dgvMapPieces.Columns.Add("Model #", "Model #")
        dgvMapPieces.Columns(2).Width = 60
        dgvMapPieces.Columns.Add("Name", "Name")
        dgvMapPieces.Columns.Add("Sibpath", "Sibpath")
        dgvMapPieces.Columns.Add("Script ID", "Script ID")
        dgvMapPieces.Columns(5).Width = 60

        dgvMapPieces.Columns.Add("Name Offset", "Name Offset")
        dgvMapPieces.Columns.Add("Unknown &H10", "Unknown &H10")
        dgvMapPieces.Columns.Add("Unknown &H20", "Unknown &H20")
        dgvMapPieces.Columns.Add("Unknown &H24", "Unknown &H24")
        dgvMapPieces.Columns.Add("Unknown &H28", "Unknown &H28")
        dgvMapPieces.Columns.Add("Unknown &H2C", "Unknown &H2C")
        dgvMapPieces.Columns.Add("Unknown &H30", "Unknown &H30")
        dgvMapPieces.Columns.Add("Unknown &H34", "Unknown &H34")
        dgvMapPieces.Columns.Add("Unknown &H38", "Unknown &H38")
        dgvMapPieces.Columns.Add("Unknown &H3C", "Unknown &H3C")
        dgvMapPieces.Columns.Add("Unknown &H40", "Unknown &H40")
        dgvMapPieces.Columns.Add("Unknown &H48", "Unknown &H48")
        dgvMapPieces.Columns.Add("Unknown &H58", "Unknown &H58")
        dgvMapPieces.Columns.Add("Unknown &H5C", "Unknown &H5C")
        dgvMapPieces.Columns.Add("Unknown p+&H04", "Unknown p+&H04")
        dgvMapPieces.Columns.Add("Unknown p+&H08", "Unknown p+&H08")
        dgvMapPieces.Columns.Add("Unknown p+&H0C", "Unknown p+&H0C")
        dgvMapPieces.Columns.Add("Unknown p+&H10", "Unknown p+&H10")
        dgvMapPieces.Columns.Add("Offset", "Offset")




        dgvObjects.Rows.Clear()
        dgvObjects.Columns.Clear()

        dgvObjects.Columns.Add("Type", "Type")
        dgvObjects.Columns(0).Width = 40
        dgvObjects.Columns.Add("Index", "Index")
        dgvObjects.Columns(1).Width = 40
        dgvObjects.Columns.Add("X pos", "X pos")
        dgvObjects.Columns(2).Width = 60
        dgvObjects.Columns.Add("Y pos", "Y pos")
        dgvObjects.Columns(3).Width = 60
        dgvObjects.Columns.Add("Z pos", "Z pos")
        dgvObjects.Columns(4).Width = 60
        dgvObjects.Columns.Add("Facing", "Facing")
        dgvObjects.Columns(5).Width = 60
        dgvObjects.Columns.Add("Model #", "Model #")
        dgvObjects.Columns(6).Width = 40
        dgvObjects.Columns.Add("Name", "Name")
        dgvObjects.Columns.Add("Sibpath", "Sibpath")
        dgvObjects.Columns.Add("Offset", "Offset")
        dgvObjects.Columns(9).DefaultCellStyle.BackColor = Color.LightGray
        dgvObjects.Columns.Add("Name Offset", "Name Offset")
        dgvObjects.Columns.Add("Script ID", "Script ID")
        dgvObjects.Columns.Add("Unknown &H10", "Unknown &H10")
        dgvObjects.Columns.Add("Unknown &H2C", "Unknown &H2C")
        dgvObjects.Columns.Add("Unknown &H30", "Unknown &H30")
        dgvObjects.Columns.Add("Unknown &H34", "Unknown &H34")
        dgvObjects.Columns.Add("Unknown &H38", "Unknown &H38")
        dgvObjects.Columns.Add("Unknown &H3C", "Unknown &H3C")
        dgvObjects.Columns.Add("Unknown &H40", "Unknown &H40")
        dgvObjects.Columns.Add("Unknown &H44", "Unknown &H44")
        dgvObjects.Columns.Add("Unknown &H58", "Unknown &H58")
        dgvObjects.Columns.Add("Unknown &H5C", "Unknown &H5C")
        dgvObjects.Columns.Add("Unknown p+&H04", "Unknown p+&H04")
        dgvObjects.Columns.Add("Unknown p+&H08", "Unknown p+&H08")
        dgvObjects.Columns.Add("Unknown p+&H0C", "Unknown p+&H0C")
        dgvObjects.Columns.Add("Unknown p+&H10", "Unknown p+&H10")
        dgvObjects.Columns.Add("Unknown p+&H14", "Unknown p+&H14")
        dgvObjects.Columns.Add("Unknown p+&H18", "Unknown p+&H18")
        dgvObjects.Columns.Add("Unknown p+&H1C", "Unknown p+&H1C")
        dgvObjects.Columns.Add("Unknown p+&H20", "Unknown p+&H20")
        dgvObjects.Columns.Add("Unknown p+&H24", "Unknown p+&H24")
        dgvObjects.Columns.Add("Unknown p+&H28", "Unknown p+&H28")
        dgvObjects.Columns.Add("Unknown p+&H2C", "Unknown p+&H2C")


        dgvCreatures.Rows.Clear()
        dgvCreatures.Columns.Clear()
        dgvCreatures.Columns.Add("Type", "Type")
        dgvCreatures.Columns(0).Width = 40
        dgvCreatures.Columns.Add("Index", "Index")
        dgvCreatures.Columns(1).Width = 40
        dgvCreatures.Columns.Add("X pos", "X pos")
        dgvCreatures.Columns(2).Width = 60
        dgvCreatures.Columns.Add("Y pos", "Y pos")
        dgvCreatures.Columns(3).Width = 60
        dgvCreatures.Columns.Add("Z pos", "Z pos")
        dgvCreatures.Columns(4).Width = 60
        dgvCreatures.Columns.Add("Facing", "Facing")
        dgvCreatures.Columns(5).Width = 60
        dgvCreatures.Columns.Add("Model #", "Model #")
        dgvCreatures.Columns(6).Width = 40
        dgvCreatures.Columns.Add("Name", "Name")
        dgvCreatures.Columns.Add("SibPath", "SibPath")
        dgvCreatures.Columns.Add("Script ID #1", "Script ID #1")
        'dgvCreatures.Columns.Add("Script ID #2", "Script ID #2")
        dgvCreatures.Columns.Add("NPC ID #1", "NPC ID #1")
        'dgvCreatures.Columns.Add("NPC ID #2", "NPC ID #2")
        dgvCreatures.Columns.Add("Offset", "Offset")
        dgvCreatures.Columns(11).DefaultCellStyle.BackColor = Color.LightGray
        dgvCreatures.Columns.Add("Name Offset", "Name Offset")

        dgvUnhandled.Rows.Clear()
        dgvUnhandled.Columns.Clear()
        dgvUnhandled.Columns.Add("Type", "Type")
        dgvUnhandled.Columns.Add("Index", "Index")
        dgvUnhandled.Columns.Add("Name", "Name")
        dgvUnhandled.Columns.Add("Offset", "Offset")
        dgvUnhandled.Columns.Add("Name Offset", "Name Offset")


        For i = 0 To modelCnt - 2
            ptr = UIntFromFour(modelPtr + &HC + i * &H4)

            nameoffset = UIntFromFour(ptr)
            type = UIntFromFour(ptr + &H4)
            index = UIntFromFour(ptr + &H8)
            name = StrFromBytes(ptr + nameoffset)
            sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

            offset = ptr

            row(0) = type
            row(1) = index
            row(2) = i
            row(3) = name
            row(4) = sibpath

            row(5) = UIntFromFour(ptr + &HC)
            row(6) = UIntFromFour(ptr + &H10)

            row(7) = offset
            row(8) = nameoffset

            dgvModels.Rows.Add(row)
        Next

        For i = 0 To partsCnt - 2
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)
            Select Case UIntFromFour(ptr + &H4)
                Case &H0
                    nameoffset = UIntFromFour(ptr)
                    type = UIntFromFour(ptr + &H4)
                    index = UIntFromFour(ptr + &H8)
                    model = UIntFromFour(ptr + &HC)

                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If sibpath.Length = 0 Then padding += &H4

                    scriptid1 = SIntFromFour(ptr + nameoffset + padding)
                    offset = ptr

                    row(0) = type
                    row(1) = index
                    row(2) = model
                    row(3) = name
                    row(4) = sibpath
                    row(5) = scriptid1
                    row(6) = nameoffset
                    row(7) = UIntFromFour(ptr + &H10)
                    row(8) = SIntFromFour(ptr + &H20)
                    row(9) = SIntFromFour(ptr + &H24)
                    row(10) = SIntFromFour(ptr + &H28)
                    row(11) = SingleFromFour(ptr + &H2C)
                    row(12) = SingleFromFour(ptr + &H30)
                    row(13) = SingleFromFour(ptr + &H34)
                    row(14) = SIntFromFour(ptr + &H38)
                    row(15) = SIntFromFour(ptr + &H3C)
                    row(16) = SIntFromFour(ptr + &H40)

                    row(17) = SIntFromFour(ptr + &H48)
                    row(18) = SIntFromFour(ptr + &H58)
                    row(19) = SIntFromFour(ptr + &H5C)
                    row(20) = UIntFromFour(ptr + nameoffset + padding + &H4)
                    row(21) = UIntFromFour(ptr + nameoffset + padding + &H8)
                    row(22) = UIntFromFour(ptr + nameoffset + padding + &HC)
                    row(23) = UIntFromFour(ptr + nameoffset + padding + &H10)
                    row(24) = ptr

                    dgvMapPieces.Rows.Add(row)


                Case &H1, &H9
                    nameoffset = UIntFromFour(ptr)
                    type = UIntFromFour(ptr + &H4)
                    index = UIntFromFour(ptr + &H8)
                    model = UIntFromFour(ptr + &HC)
                    xpos = SingleFromFour(ptr + &H14)
                    ypos = SingleFromFour(ptr + &H18)
                    zpos = SingleFromFour(ptr + &H1C)

                    facing = SingleFromFour(ptr + &H24)

                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding < &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If


                    scriptid1 = SIntFromFour(ptr + nameoffset + padding)

                    offset = ptr

                    row(0) = type
                    row(1) = index
                    row(2) = xpos
                    row(3) = ypos
                    row(4) = zpos
                    row(5) = facing
                    row(6) = model
                    row(7) = name
                    row(8) = sibpath
                    row(9) = offset
                    row(10) = nameoffset
                    row(11) = scriptid1
                    row(12) = SIntFromFour(ptr + &H10)
                    row(13) = SingleFromFour(ptr + &H2C)
                    row(14) = SingleFromFour(ptr + &H30)
                    row(15) = SingleFromFour(ptr + &H34)
                    row(16) = SIntFromFour(ptr + &H38)
                    row(17) = SIntFromFour(ptr + &H3C)
                    row(18) = SIntFromFour(ptr + &H40)
                    row(19) = SIntFromFour(ptr + &H44)
                    row(20) = SIntFromFour(ptr + &H58)
                    row(21) = SIntFromFour(ptr + &H5C)
                    row(22) = SIntFromFour(ptr + nameoffset + padding + &H4)
                    row(23) = SIntFromFour(ptr + nameoffset + padding + &H8)
                    row(24) = SIntFromFour(ptr + nameoffset + padding + &HC)
                    row(25) = SIntFromFour(ptr + nameoffset + padding + &H10)
                    row(26) = SIntFromFour(ptr + nameoffset + padding + &H14)
                    row(27) = SIntFromFour(ptr + nameoffset + padding + &H18)
                    row(28) = SIntFromFour(ptr + nameoffset + padding + &H1C)
                    row(29) = SIntFromFour(ptr + nameoffset + padding + &H20)
                    row(30) = SIntFromFour(ptr + nameoffset + padding + &H24)
                    row(31) = SIntFromFour(ptr + nameoffset + padding + &H28)
                    row(32) = SIntFromFour(ptr + nameoffset + padding + &H2C)

                    dgvObjects.Rows.Add(row)

                Case &H2, &H4, &HA
                    nameoffset = UIntFromFour(ptr)
                    type = UIntFromFour(ptr + &H4)
                    index = UIntFromFour(ptr + &H8)
                    model = UIntFromFour(ptr + &HC)
                    xpos = SingleFromFour(ptr + &H14)
                    ypos = SingleFromFour(ptr + &H18)
                    zpos = SingleFromFour(ptr + &H1C)

                    facing = SingleFromFour(ptr + &H24)

                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding < &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If


                    scriptid1 = SIntFromFour(ptr + nameoffset + padding)
                    npcid1 = SIntFromFour(ptr + nameoffset + padding + &H24)

                    offset = ptr

                    row(0) = type
                    row(1) = index
                    row(2) = xpos
                    row(3) = ypos
                    row(4) = zpos
                    row(5) = facing
                    row(6) = model
                    row(7) = name
                    row(8) = sibpath
                    row(9) = scriptid1
                    row(10) = npcid1

                    row(11) = offset
                    row(12) = nameoffset

                    dgvCreatures.Rows.Add(row)

                Case Else
                    nameoffset = UIntFromTwo(ptr)
                    type = UIntFromFour(ptr + &H4)
                    name = StrFromBytes(ptr + nameoffset)
                    offset = ptr

                    row(0) = type
                    row(1) = UIntFromFour(ptr + &H8)
                    row(2) = name
                    row(3) = offset
                    row(4) = nameoffset

                    dgvUnhandled.Rows.Add(row)
            End Select
        Next

        mapstudioPtr = UIntFromFour((partsCnt * &H4) + &H8 + partsPtr)
        mapstudioCnt = UIntFromFour(mapstudioPtr + &H8)

    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        bytes = File.ReadAllBytes(txtMSBfile.Text)

        Dim tmpbytes() As Byte

        Dim msbIndex As Byte() = {}
        Dim msbData As Byte() = {}

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
        Dim curroffset As UInteger

        Dim nameoffset As UInteger
        Dim type As UInteger
        Dim model As UInteger

        Dim index As UInteger

        Dim xpos As Single
        Dim ypos As Single
        Dim zpos As Single
        Dim facing As Single

        Dim name As String
        Dim sibpath As String

        Dim scriptid1 As Integer
        Dim scriptid2 As Integer
        Dim npcid1 As Integer
        Dim npcid2 As Integer

        Dim offset As UInteger
        Dim padding As UInteger

        If File.Exists(txtMSBfile.Text & ".tmp") Then File.Delete(txtMSBfile.Text & ".tmp")
        Dim MSBStream As New IO.FileStream(txtMSBfile.Text & ".tmp", IO.FileMode.CreateNew)

        WriteBytes(MSBStream, UInt32ToFourByte(0))


        modelPtr = 0
        modelCnt = dgvModels.Rows.Count - 2
        curroffset = modelPtr + &H10 + (modelCnt + 1) * &H4

        MSBStream.Position = &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(modelCnt + 2))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("MODEL_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        For i As UInteger = 0 To modelCnt
            type = dgvModels.Rows(i).Cells(0).Value
            index = dgvModels.Rows(i).Cells(1).Value
            name = dgvModels.Rows(i).Cells(3).Value
            sibpath = dgvModels.Rows(i).Cells(4).Value
            nameoffset = dgvModels.Rows(i).Cells(8).Value
            padding = ((sibpath.Length + name.Length + 5) And -&H4)

            'Update Index
            curroffset = MSBStream.Position
            MSBStream.Position = modelPtr + &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            WriteBytes(MSBStream, UInt32ToFourByte(nameoffset))
            WriteBytes(MSBStream, UInt32ToFourByte(type))
            WriteBytes(MSBStream, UInt32ToFourByte(index))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvModels.Rows(i).Cells(5).Value))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvModels.Rows(i).Cells(6).Value))

            MSBStream.Position = curroffset + nameoffset
            WriteBytes(MSBStream, Str2Bytes(name))
            MSBStream.Position += 1
            WriteBytes(MSBStream, Str2Bytes(sibpath))
            MSBStream.Position = curroffset + nameoffset + padding
        Next
        eventPtr = (MSBStream.Length And -&H4) + &H4
        MSBStream.Position = modelPtr + &H10 + (modelCnt) * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(eventPtr))
        MSBStream.Position = eventPtr




        bytes = eventParams
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(UIntFromFour(&H4) - eventParamsOrigOffset + eventPtr))
        eventCnt = UIntFromFour(&H8)
        WriteBytes(MSBStream, UInt32ToFourByte(eventCnt))

        For i As UInteger = 0 To eventCnt - 1
            curroffset = &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(UIntFromFour(curroffset) - eventParamsOrigOffset + eventPtr))
        Next

        ReDim tmpbytes(bytes.Length - curroffset - &H4 - 1)
        Array.Copy(bytes, curroffset + &H4, tmpbytes, 0, tmpbytes.Length)
        WriteBytes(MSBStream, tmpbytes)


        pointPtr = MSBStream.Length
        bytes = pointParams
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(UIntFromFour(&H4) - pointParamsOrigOffset + pointPtr))
        pointCnt = UIntFromFour(&H8)
        WriteBytes(MSBStream, UInt32ToFourByte(pointCnt))

        For i As UInteger = 0 To pointCnt - 1
            curroffset = &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(UIntFromFour(curroffset) - pointParamsOrigOffset + pointPtr))
        Next

        ReDim tmpbytes(bytes.Length - curroffset - &H4 - 1)
        Array.Copy(bytes, curroffset + &H4, tmpbytes, 0, tmpbytes.Length)
        WriteBytes(MSBStream, tmpbytes)
        partsPtr = MSBStream.Length




        partsCnt = dgvMapPieces.Rows.Count + dgvCreatures.Rows.Count + dgvObjects.Rows.Count + dgvUnhandled.Rows.Count - 5
        curroffset = partsPtr + &H10 + (partsCnt + 1) * &H4
        MSBStream.Position = partsPtr + &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(partsCnt + 2))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("PARTS_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        For i = 0 To dgvMapPieces.Rows.Count - 2
            type = dgvMapPieces.Rows(i).Cells(0).Value
            index = dgvMapPieces.Rows(i).Cells(1).Value
            model = dgvMapPieces.Rows(i).Cells(2).Value
            name = dgvMapPieces.Rows(i).Cells(3).Value
            sibpath = dgvMapPieces.Rows(i).Cells(4).Value
            scriptid1 = dgvMapPieces.Rows(i).Cells(5).Value
            nameoffset = dgvMapPieces.Rows(i).Cells(6).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding < &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            'Update Index
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            WriteBytes(MSBStream, UInt32ToFourByte(nameoffset))
            WriteBytes(MSBStream, UInt32ToFourByte(type))
            WriteBytes(MSBStream, UInt32ToFourByte(index))
            WriteBytes(MSBStream, UInt32ToFourByte(model))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(7).Value))

            MSBStream.Position = curroffset + &H20
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(8).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(9).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(10).Value))
            WriteBytes(MSBStream, SingleToFourByte(dgvMapPieces.Rows(i).Cells(11).Value))
            WriteBytes(MSBStream, SingleToFourByte(dgvMapPieces.Rows(i).Cells(12).Value))
            WriteBytes(MSBStream, SingleToFourByte(dgvMapPieces.Rows(i).Cells(13).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(14).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(15).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(16).Value))

            MSBStream.Position = curroffset + &H48
            WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(17).Value))

            MSBStream.Position = curroffset + &H58
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(18).Value))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(19).Value))

            MSBStream.Position = curroffset + nameoffset
            WriteBytes(MSBStream, Str2Bytes(name))

            MSBStream.Position += 1
            WriteBytes(MSBStream, Str2Bytes(sibpath))

            MSBStream.Position = curroffset + nameoffset + padding
            WriteBytes(MSBStream, Int32ToFourByte(scriptid1))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(20).Value))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(21).Value))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(22).Value))
            WriteBytes(MSBStream, UInt32ToFourByte(dgvMapPieces.Rows(i).Cells(23).Value))

            WriteBytes(MSBStream, UInt32ToFourByte(0))
            WriteBytes(MSBStream, UInt32ToFourByte(0))
            WriteBytes(MSBStream, UInt32ToFourByte(0))
        Next




        For i = 0 To dgvObjects.Rows.Count - 2
            type = dgvObjects.Rows(i).Cells(0).Value
            index = dgvObjects.Rows(i).Cells(1).Value
            xpos = dgvObjects.Rows(i).Cells(2).Value
            ypos = dgvObjects.Rows(i).Cells(3).Value
            zpos = dgvObjects.Rows(i).Cells(4).Value
            facing = dgvObjects.Rows(i).Cells(5).Value
            model = dgvObjects.Rows(i).Cells(6).Value
            name = dgvObjects.Rows(i).Cells(7).Value
            sibpath = dgvObjects.Rows(i).Cells(8).Value
            nameoffset = dgvObjects.Rows(i).Cells(10).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding < &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            scriptid1 = dgvObjects.Rows(i).Cells(11).Value

            'Update Index
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + dgvMapPieces.Rows.Count - 1) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            '&H0
            WriteBytes(MSBStream, Int32ToFourByte(nameoffset))
            WriteBytes(MSBStream, Int32ToFourByte(type))
            WriteBytes(MSBStream, Int32ToFourByte(index))
            WriteBytes(MSBStream, Int32ToFourByte(model))
            '&H10
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(12).Value))
            WriteBytes(MSBStream, SingleToFourByte(xpos))
            WriteBytes(MSBStream, SingleToFourByte(ypos))
            WriteBytes(MSBStream, SingleToFourByte(zpos))
            '&H20

            MSBStream.Position = curroffset + &H24
            WriteBytes(MSBStream, SingleToFourByte(facing))
            MSBStream.Position = curroffset + &H2C
            WriteBytes(MSBStream, SingleToFourByte(dgvObjects.Rows(i).Cells(13).Value))
            '&H30
            WriteBytes(MSBStream, SingleToFourByte(dgvObjects.Rows(i).Cells(14).Value))
            WriteBytes(MSBStream, SingleToFourByte(dgvObjects.Rows(i).Cells(15).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(16).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(17).Value))
            '&H40
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(18).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(19).Value))

            MSBStream.Position = curroffset + &H58
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(20).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(21).Value))


            MSBStream.Position = curroffset + nameoffset
            WriteBytes(MSBStream, Str2Bytes(name))
            MSBStream.Position += 1
            WriteBytes(MSBStream, Str2Bytes(sibpath))

            'p+&H0
            MSBStream.Position = curroffset + nameoffset + padding
            WriteBytes(MSBStream, Int32ToFourByte(scriptid1))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(22).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(23).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(24).Value))
            'p+&H10
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(25).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(26).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(27).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(28).Value))
            'p+&H20
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(29).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(30).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(31).Value))
            WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(32).Value))
        Next
        For i = 0 To dgvCreatures.Rows.Count - 2
            type = dgvCreatures.Rows(i).Cells(0).Value
            index = dgvCreatures.Rows(i).Cells(1).Value

            xpos = dgvCreatures.Rows(i).Cells(2).Value
            ypos = dgvCreatures.Rows(i).Cells(3).Value
            zpos = dgvCreatures.Rows(i).Cells(4).Value

            facing = dgvCreatures.Rows(i).Cells(5).Value

            model = dgvCreatures.Rows(i).Cells(6).Value
            name = dgvCreatures.Rows(i).Cells(7).Value
            sibpath = dgvCreatures.Rows(i).Cells(8).Value

            scriptid1 = dgvCreatures.Rows(i).Cells(9).Value
            npcid1 = dgvCreatures.Rows(i).Cells(10).Value
            ptr = dgvCreatures.Rows(i).Cells(11).Value
            nameoffset = dgvCreatures.Rows(i).Cells(12).Value

            'InsBytes(ptr, UInt32ToFourByte(nameoffset))
            'InsBytes(ptr + &H4, UInt32ToFourByte(type))
            'InsBytes(ptr + &H8, UInt32ToFourByte(index))
            'InsBytes(ptr + &HC, UInt32ToFourByte(model))
            'InsBytes(ptr + &H14, SingleToFourByte(xpos))
            'InsBytes(ptr + &H18, SingleToFourByte(ypos))
            'InsBytes(ptr + &H1C, SingleToFourByte(zpos))
            'InsBytes(ptr + &H24, SingleToFourByte(facing))

            'InsBytes(ptr + nameoffset, System.Text.Encoding.ASCII.GetBytes(name))
            'InsBytes(ptr + nameoffset + name.Length + 1, System.Text.Encoding.ASCII.GetBytes(sibpath))


            'InsBytes(ptr + nameoffset + padding, Int32ToFourByte(scriptid1))
            'InsBytes(ptr + nameoffset + padding + &H24, Int32ToFourByte(npcid1))
        Next


        'File.WriteAllBytes(txtMSBfile.Text, bytes)
        MSBStream.Close()
        MsgBox("Save Complete.")
    End Sub
End Class
