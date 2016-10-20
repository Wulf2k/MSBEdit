Imports System.IO


'Creature type 0x4 is sized wrong
'TODO:  Confirm the above
'TODO:  Update to look at Sib Offset instead of using padding

Public Class frmMSBEdit

    Public models As msbdata = New msbdata
    Public mapPieces0 As msbdata = New msbdata
    Public objects1 As msbdata = New msbdata
    Public creatures2 As msbdata = New msbdata
    Public creatures4 As msbdata = New msbdata
    Public collision5 As msbdata = New msbdata
    Public navimesh8 As msbdata = New msbdata
    Public objects9 As msbdata = New msbdata
    Public creatures10 As msbdata = New msbdata
    Public collision11 As msbdata = New msbdata

    Dim parts() As msbdata = {}
    Dim partsdgvs() As DataGridView = {}

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


    Private Function Int8ToOneByte(ByVal val As Integer) As Byte()
        Return {CByte(val)}
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
    Private Function UInt8ToOneByte(ByVal val As UInteger) As Byte()
        Return BitConverter.GetBytes(val)
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

    Private Function SIntFromOne(ByVal loc As UInteger) As SByte
        Return CSByte(bytes(loc))
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
    Private Function SingleFromFour(ByVal loc As UInteger) As Single
        Dim bArray(3) As Byte

        For i = 0 To 3
            bArray(3 - i) = bytes(loc + i)
        Next
        If Not bigEndian Then bArray = ReverseFourBytes(bArray)
        Return BitConverter.ToSingle(bArray, 0)
    End Function


    Private Sub initDGV(ByRef dgv As DataGridView, layout As msbdata)
        dgv.Rows.Clear()
        dgv.Columns.Clear()

        For i = 0 To layout.fieldCount - 1
            dgv.Columns.Add(layout.retrieveName(i), layout.retrieveName(i))
            dgv.Columns(i).DefaultCellStyle.BackColor = layout.retrieveBackColor(i)
        Next
    End Sub
    Private Sub readParts(ByRef dgv As DataGridView, ByRef layout As msbdata, ptr As UInteger)

        Dim currOffset As Integer = 0
        Dim partRow(layout.fieldCount) As String
        Dim partName As String = ""
        Dim sibpath As String = ""
        Dim textboost As Integer

        Dim nameoffset = SIntFromFour(ptr)
        partName = StrFromBytes(ptr + nameoffset)
        sibpath = StrFromBytes(ptr + nameoffset + partName.Length + 1)

        partRow(layout.getNameIndex) = partName
        partRow(layout.getNameIndex + 1) = sibpath

        Dim Padding = ((sibpath.Length + partName.Length + 5) And -&H4)
        If Padding <= &H10 Then
            Padding = &H10
            If Not bigEndian Then Padding += &H4
        End If

        For j = 0 To layout.fieldCount - 1
            If j < layout.getNameIndex Then
                textboost = 0
            Else
                textboost = nameoffset + Padding
            End If

            If j = layout.getNameIndex Then currOffset = 0

            Select Case layout.retrieveType(j)
                Case "i8"
                    partRow(j) = SIntFromOne(ptr + textboost + currOffset)
                    currOffset += 1
                Case "i16"
                    partRow(j) = SIntFromTwo(ptr + textboost + currOffset)
                    currOffset += 2
                Case "i32"
                    partRow(j) = SIntFromFour(ptr + textboost + currOffset)
                    currOffset += 4
                Case "f32"
                    partRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                    currOffset += 4
            End Select
        Next

        dgv.Rows.Add(partRow)
    End Sub
    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        bytes = File.ReadAllBytes(txtMSBfile.Text)


        Dim ptr As UInteger

        Dim nameoffset As UInteger

        Dim name As String
        Dim sibpath As String


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

        initDGV(dgvModels, models)


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




        For i = 0 To parts.Count - 1
            initDGV(partsdgvs(i), parts(i))
        Next


        For i = 0 To modelCnt - 2
            Dim currOffset As Integer = 0
            Dim mdlRow(models.fieldCount) As String
            Dim mdlName As String = ""
            Dim mdlSibpath As String = ""

            ptr = UIntFromFour(modelPtr + &HC + i * &H4)

            nameoffset = UIntFromFour(ptr)
            name = StrFromBytes(ptr + nameoffset)
            sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

            mdlRow(models.getNameIndex) = name
            mdlRow(models.getNameIndex + 1) = sibpath

            For j = 0 To models.fieldCount - 1
                Select Case models.retrieveType(j)
                    Case "i32"
                        mdlRow(j) = SIntFromFour(ptr + currOffset)
                        currOffset += 4
                End Select
            Next

            dgvModels.Rows.Add(mdlRow)
            dgvModels.Rows(i).HeaderCell.Value = i.ToString
        Next


        Dim idx As Integer
        Dim parttype(9) As Integer
        parttype = {0, 1, 2, 4, 5, 8, 9, &HA, &HB}

        For i = 0 To partsCnt - 2
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)

            idx = Array.IndexOf(parttype, SIntFromFour(ptr + &H4))
            readParts(partsdgvs(idx), parts(idx), ptr)
        Next

        mapstudioPtr = UIntFromFour((partsCnt * &H4) + &H8 + partsPtr)
        mapstudioCnt = UIntFromFour(mapstudioPtr + &H8)

    End Sub
    Private Sub saveDGV(ByRef MSBStream As FileStream, ByRef dgv As DataGridView, ByRef data As msbdata, ByRef partsPtr As Integer, ByRef curroffset As Integer, ByRef partsidx As Integer)
        For i = 0 To dgv.Rows.Count - 2

            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + partsidx) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            Dim nameoffset = dgv.Rows(i).Cells(0).Value
            Name = dgv.Rows(i).Cells(data.getNameIndex).Value
            Dim sibpath = dgv.Rows(i).Cells(data.getNameIndex + 1).Value

            Dim Padding = ((sibpath.Length + Name.Length + 5) And -&H4)
            If Padding <= &H10 Then
                Padding = &H10
                If Not bigEndian Then Padding += &H4
            End If

            For j = 0 To data.fieldCount - 1
                If j = data.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = data.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + Padding
                Select Case data.retrieveType(j)
                    Case "i8"
                        WriteBytes(MSBStream, Int8ToOneByte(dgv.Rows(i).Cells(j).Value))
                    Case "i16"
                        WriteBytes(MSBStream, Int16ToTwoByte(dgv.Rows(i).Cells(j).Value))
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgv.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgv.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgv.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next
        partsidx += dgv.Rows.Count-1

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

        'Used in Demon's, not Dark
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



        Dim curroffset As UInteger
        Dim nameoffset As UInteger

        Dim name As String
        Dim sibpath As String

        Dim padding As UInteger

        If Not File.Exists(txtMSBfile.Text & ".bak") Then
            File.WriteAllBytes(txtMSBfile.Text & ".bak", bytes)
        End If

        If File.Exists(txtMSBfile.Text) Then File.Delete(txtMSBfile.Text)
        Dim MSBStream As New IO.FileStream(txtMSBfile.Text, IO.FileMode.CreateNew)

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

        'Models
        For i As UInteger = 0 To modelCnt
            curroffset = MSBStream.Position
            MSBStream.Position = modelPtr + &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvModels.Rows(i).Cells(0).Value
            name = dgvModels.Rows(i).Cells(models.getNameIndex).Value
            sibpath = dgvModels.Rows(i).Cells(models.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding <= &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To models.fieldCount - 1
                If j = models.getNameIndex Then MSBStream.Position = curroffset + nameoffset

                Select Case models.retrieveType(j)
                    Case "i32"
                        WriteBytes(MSBStream, UInt32ToFourByte(dgvModels.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvModels.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
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




        partsCnt = dgvMapPieces0.Rows.Count + dgvObjects1.Rows.Count + dgvCreatures2.Rows.Count + dgvCreatures4.Rows.Count + dgvCollision5.Rows.Count + dgvNavimesh8.Rows.Count + dgvObjects9.Rows.Count + dgvCreatures10.Rows.Count + dgvCollision11.Rows.Count - 9
        curroffset = partsPtr + &H10 + partsCnt * &H4
        MSBStream.Position = partsPtr + &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(partsCnt + 1))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("PARTS_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        Dim partsidx = 0

        'Map Pieces 0
        saveDGV(MSBStream, dgvMapPieces0, mapPieces0, partsPtr, curroffset, partsidx)

        'Objects 1
        saveDGV(MSBStream, dgvObjects1, objects1, partsPtr, curroffset, partsidx)

        'Creatures 2
        saveDGV(MSBStream, dgvCreatures2, creatures2, partsPtr, curroffset, partsidx)

        'Creatures 4
        saveDGV(MSBStream, dgvCreatures4, creatures4, partsPtr, curroffset, partsidx)

        'Collision 5
        saveDGV(MSBStream, dgvCollision5, collision5, partsPtr, curroffset, partsidx)

        'Navimesh 8
        saveDGV(MSBStream, dgvNavimesh8, navimesh8, partsPtr, curroffset, partsidx)

        'Objects 9
        saveDGV(MSBStream, dgvObjects9, objects9, partsPtr, curroffset, partsidx)

        'Creatures 10
        saveDGV(MSBStream, dgvCreatures10, creatures10, partsPtr, curroffset, partsidx)

        'Collision 11
        saveDGV(MSBStream, dgvCollision11, collision11, partsPtr, curroffset, partsidx)

        MSBStream.Close()
        MsgBox("Save Complete.")
    End Sub


    Private Sub mdlPrep()
        With models
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("x10", "i32", Color.LightGray)
            .add("x14", "i32", Color.LightGray)
            .add("x18", "i32", Color.LightGray)
            .add("x1C", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
        End With
    End Sub
    Private Sub map0Prep()
        With mapPieces0
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("DrawGroup1", "i32", Color.White)
            .add("DrawGroup2", "i32", Color.White)
            .add("DrawGroup3", "i32", Color.White)
            .add("DrawGroup4", "i32", Color.White)
            .add("x4c", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i8", Color.LightGray)
            .add("p+x09", "i8", Color.LightGray)
            .add("p+x0A", "i8", Color.LightGray)
            .add("p+x0B", "i8", Color.LightGray)
            .add("p+x0C", "i32", Color.LightGray)
            .add("p+x10", "i32", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub obj1Prep()
        With objects1
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i32", Color.LightGray)
            .add("p+x0C", "i8", Color.LightGray)
            .add("p+x0D", "i8", Color.LightGray)
            .add("p+x0E", "i8", Color.LightGray)
            .add("p+x0F", "i8", Color.LightGray)
            .add("p+x10", "i8", Color.LightGray)
            .add("p+x11", "i8", Color.LightGray)
            .add("p+x12", "i8", Color.LightGray)
            .add("p+x13", "i8", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
            .add("p+x28", "i32", Color.LightGray)
            .add("p+x2C", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub crt2Prep()
        With creatures2
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i32", Color.LightGray)
            .add("p+x0C", "i32", Color.LightGray)
            .add("p+x10", "i32", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("AI", "i32", Color.White)
            .add("NPCParam", "i32", Color.White)
            .add("TalkID", "i32", Color.White)
            .add("p+x2C", "i32", Color.LightGray)
            .add("ChrInitParam", "i32", Color.White)
            .add("p+x34", "i32", Color.LightGray)
            .add("p+x38", "i32", Color.LightGray)
            .add("p+x3C", "i32", Color.LightGray)
            .add("p+x40", "i32", Color.LightGray)
            .add("p+x44", "i32", Color.LightGray)
            .add("p+x48", "i32", Color.LightGray)
            .add("p+x4C", "i32", Color.LightGray)
            .add("AnimID", "i32", Color.White)
            .add("p+x54", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub crt4Prep()
        With creatures4
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i32", Color.LightGray)
            .add("p+x0C", "i32", Color.LightGray)
            .add("p+x10", "i32", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub coll5Prep()
        With collision5
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("p+x04", "i8", Color.LightGray)
            .add("p+x05", "i8", Color.LightGray)
            .add("p+x06", "i8", Color.LightGray)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i8", Color.LightGray)
            .add("p+x09", "i8", Color.LightGray)
            .add("p+x0A", "i8", Color.LightGray)
            .add("p+x0B", "i8", Color.LightGray)
            .add("p+x0C", "i8", Color.LightGray)
            .add("p+x0D", "i8", Color.LightGray)
            .add("p+x0E", "i8", Color.LightGray)
            .add("p+x0F", "i8", Color.LightGray)
            .add("p+x10", "i8", Color.LightGray)
            .add("p+x11", "i8", Color.LightGray)
            .add("p+x12", "i8", Color.LightGray)
            .add("p+x13", "i8", Color.LightGray)
            .add("p+x14", "i8", Color.LightGray)
            .add("p+x15", "i8", Color.LightGray)
            .add("p+x16", "i8", Color.LightGray)
            .add("p+x17", "i8", Color.LightGray)
            .add("p+x18", "i8", Color.LightGray)
            .add("p+x19", "i8", Color.LightGray)
            .add("p+x1A", "i8", Color.LightGray)
            .add("p+x1B", "i8", Color.LightGray)
            .add("p+x1C", "i8", Color.LightGray)
            .add("p+x1D", "i8", Color.LightGray)
            .add("p+x1E", "i8", Color.LightGray)
            .add("p+x1F", "i8", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
            .add("p+x28", "i32", Color.LightGray)
            .add("p+x2C", "i32", Color.LightGray)
            .add("p+x30", "i32", Color.LightGray)
            .add("p+x34", "i32", Color.LightGray)
            .add("p+x38", "i32", Color.LightGray)
            .add("p+x3C", "i16", Color.LightGray)
            .add("p+x3E", "i16", Color.LightGray)
            .add("p+x40", "i32", Color.LightGray)
            .add("p+x44", "i32", Color.LightGray)
            .add("p+x48", "i32", Color.LightGray)
            .add("p+x4C", "i32", Color.LightGray)
            .add("p+x50", "i32", Color.LightGray)
            .add("p+x54", "i16", Color.LightGray)
            .add("p+x56", "i16", Color.LightGray)
            .add("p+x58", "i32", Color.LightGray)
            .add("p+x5C", "i32", Color.LightGray)
            .add("p+x60", "i32", Color.LightGray)
            .add("p+x64", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub navi8Prep()
        With navimesh8
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("DrawGroup1", "i32", Color.White)
            .add("DrawGroup2", "i32", Color.White)
            .add("DrawGroup3", "i32", Color.White)
            .add("DrawGroup4", "i32", Color.White)
            .add("x4c", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i8", Color.LightGray)
            .add("p+x09", "i8", Color.LightGray)
            .add("p+x0A", "i8", Color.LightGray)
            .add("p+x0B", "i8", Color.LightGray)
            .add("p+x0C", "i32", Color.LightGray)
            .add("p+x10", "i32", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
            .add("p+x28", "i32", Color.LightGray)
            .add("p+x2C", "i32", Color.LightGray)
            .add("p+x30", "i32", Color.LightGray)
            .add("p+x34", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub obj9Prep()
        With objects9
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("p+x04", "i8", Color.LightGray)
            .add("p+x05", "i8", Color.LightGray)
            .add("p+x06", "i8", Color.LightGray)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i32", Color.LightGray)
            .add("p+x0C", "i8", Color.LightGray)
            .add("p+x0D", "i8", Color.LightGray)
            .add("p+x0E", "i8", Color.LightGray)
            .add("p+x0F", "i8", Color.LightGray)
            .add("p+x10", "i8", Color.LightGray)
            .add("p+x11", "i8", Color.LightGray)
            .add("p+x12", "i8", Color.LightGray)
            .add("p+x13", "i8", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
            .add("p+x28", "i32", Color.LightGray)
            .add("p+x2C", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub crt10Prep()
        With creatures10
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("LightId", "i8", Color.White)
            .add("FogId", "i8", Color.White)
            .add("ScatId", "i8", Color.White)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i32", Color.LightGray)
            .add("p+x0C", "i32", Color.LightGray)
            .add("p+x10", "i32", Color.LightGray)
            .add("p+x14", "i32", Color.LightGray)
            .add("p+x18", "i32", Color.LightGray)
            .add("p+x1C", "i32", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("NPC ID", "i32", Color.White)
            .add("p+x28", "i32", Color.LightGray)
            .add("p+x2C", "i32", Color.LightGray)
            .add("p+x30", "i32", Color.LightGray)
            .add("p+x34", "i32", Color.LightGray)
            .add("p+x38", "i32", Color.LightGray)
            .add("p+x3C", "i32", Color.LightGray)
            .add("p+x40", "i32", Color.LightGray)
            .add("p+x44", "i32", Color.LightGray)
            .add("p+x48", "i32", Color.LightGray)
            .add("p+x4C", "i32", Color.LightGray)
            .add("AnimID", "i32", Color.White)
            .add("p+x54", "i32", Color.LightGray)
        End With
    End Sub
    Private Sub coll11Prep()
        With collision11
            .add("Name Offset", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("Index", "i32", Color.White)
            .add("Model", "i32", Color.White)
            .add("Sib Offset", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("Scale X", "f32", Color.White)
            .add("Scale Y", "f32", Color.White)
            .add("Scale Z", "f32", Color.White)
            .add("x38", "i32", Color.LightGray)
            .add("x3C", "i32", Color.LightGray)
            .add("x40", "i32", Color.LightGray)
            .add("x44", "i32", Color.LightGray)
            .add("x48", "i32", Color.LightGray)
            .add("x4C", "i32", Color.LightGray)
            .add("x50", "i32", Color.LightGray)
            .add("x54", "i32", Color.LightGray)
            .add("x58", "i32", Color.LightGray)
            .add("x5C", "i32", Color.LightGray)
            .add("x60", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("Sibpath", "string", Color.White)
            .add("EventEntityID", "i32", Color.White)
            .add("p+x04", "i8", Color.LightGray)
            .add("p+x05", "i8", Color.LightGray)
            .add("p+x06", "i8", Color.LightGray)
            .add("p+x07", "i8", Color.LightGray)
            .add("p+x08", "i8", Color.LightGray)
            .add("p+x09", "i8", Color.LightGray)
            .add("p+x0A", "i8", Color.LightGray)
            .add("p+x0B", "i8", Color.LightGray)
            .add("p+x0C", "i8", Color.LightGray)
            .add("p+x0D", "i8", Color.LightGray)
            .add("p+x0E", "i8", Color.LightGray)
            .add("p+x0F", "i8", Color.LightGray)
            .add("p+x10", "i8", Color.LightGray)
            .add("p+x11", "i8", Color.LightGray)
            .add("p+x12", "i8", Color.LightGray)
            .add("p+x13", "i8", Color.LightGray)
            .add("p+x14", "i8", Color.LightGray)
            .add("p+x15", "i8", Color.LightGray)
            .add("p+x16", "i8", Color.LightGray)
            .add("p+x17", "i8", Color.LightGray)
            .add("p+x18", "i8", Color.LightGray)
            .add("p+x19", "i8", Color.LightGray)
            .add("p+x1A", "i8", Color.LightGray)
            .add("p+x1B", "i8", Color.LightGray)
            .add("p+x1C", "i16", Color.LightGray)
            .add("p+x1E", "i16", Color.LightGray)
            .add("p+x20", "i32", Color.LightGray)
            .add("p+x24", "i32", Color.LightGray)
        End With
    End Sub


    Private Sub frmMSBEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        mdlPrep()
        map0Prep()
        obj1Prep()
        crt2Prep()
        crt4Prep()
        coll5Prep()
        navi8Prep()
        obj9Prep()
        crt10Prep()
        coll11Prep()

        parts = {mapPieces0, objects1, creatures2, creatures4, collision5, navimesh8, objects9, creatures10, collision11}
        partsdgvs = {dgvMapPieces0, dgvObjects1, dgvCreatures2, dgvCreatures4, dgvCollision5, dgvNavimesh8, dgvObjects9, dgvCreatures10, dgvCollision11}

    End Sub

    Public Sub sizeChange() Handles MyBase.Resize
        tabParts.Width = MyBase.Width - 35
        tabParts.Height = MyBase.Height - 115

        dgvModels.Width = MyBase.Width - 55
        dgvModels.Height = MyBase.Height - 150

        dgvMapPieces0.Width = MyBase.Width - 55
        dgvMapPieces0.Height = MyBase.Height - 150

        dgvObjects1.Width = MyBase.Width - 55
        dgvObjects1.Height = MyBase.Height - 150

        dgvCreatures2.Width = MyBase.Width - 55
        dgvCreatures2.Height = MyBase.Height - 150

        dgvCreatures4.Width = MyBase.Width - 55
        dgvCreatures4.Height = MyBase.Height - 150

        dgvCollision5.Width = MyBase.Width - 55
        dgvCollision5.Height = MyBase.Height - 150

        dgvNavimesh8.Width = MyBase.Width - 55
        dgvNavimesh8.Height = MyBase.Height - 150

        dgvObjects9.Width = MyBase.Width - 55
        dgvObjects9.Height = MyBase.Height - 150

        dgvCreatures10.Width = MyBase.Width - 55
        dgvCreatures10.Height = MyBase.Height - 150

        dgvCollision11.Width = MyBase.Width - 55
        dgvCollision11.Height = MyBase.Height - 150

    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click

        Dim idx As Integer
        idx = tabParts.SelectedIndex

        Dim dgvs() as DataGridView

        dgvs = {dgvModels, dgvMapPieces0, dgvObjects1, dgvCreatures2, dgvCreatures4, dgvCollision5, dgvNavimesh8, dgvObjects9, dgvCreatures10, dgvCollision11}

        copyEntry(dgvs(idx), dgvs(idx).SelectedCells(0).RowIndex)
    End Sub

    Sub copyEntry(byref dgv As DataGridView, rowidx As Integer)
        Dim row(dgv.Columns.count-1)
        For i = 0 To row.Count-1
            row(i) = dgv.rows(dgv.SelectedCells(0).RowIndex).Cells(i).FormattedValue
        Next
        dgv.Rows.Add(row)
    End Sub
End Class

Public Class msbdata
    Private fieldNames As List(Of String) = New List(Of String)
    Private fieldtypes As List(Of String) = New List(Of String)
    Private fieldBackColor As List(Of Color) = New List(Of Color)

    Private nameIdx As Integer

    Public Sub add(ByVal name As String, ByVal type As String, backColor As Color)
        fieldNames.Add(name)
        fieldtypes.Add(type)
        fieldBackColor.Add(backColor)
    End Sub
    Public Function retrieveName(ByVal i As Integer) As String
        Return fieldNames(i)
    End Function
    Public Function retrieveType(ByVal i As Integer) As String
        Return fieldtypes(i)
    End Function
    Public Function retrieveBackColor(ByVal i As Integer) As Color
        Return fieldBackColor(i)
    End Function
    Public Function fieldCount() As Integer
        Return fieldNames.Count
    End Function
    Public Function getNameIndex() As Integer
        Return nameIdx
    End Function
    Public Sub setNameIndex(ByVal idx As Integer)
        nameIdx = idx
    End Sub
End Class