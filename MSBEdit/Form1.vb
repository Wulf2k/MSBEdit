Imports System.IO
Imports System.Text


'Creature type 0x4 is sized wrong
'TODO:  Confirm the above

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
    Public points0 As msbdata = New msbdata
    Public points2 As msbdata = New msbdata
    Public points3 As msbdata = New msbdata
    Public points5 As msbdata = New msbdata

    Dim dgvs() As DataGridView = {}
    Dim layouts() As msbdata = {}

    Dim parts() As msbdata = {}
    Dim partsdgvs() As DataGridView = {}

    Dim points() As msbdata = {}
    Dim pointsdgvs() As DataGridView = {}

    Public Shared bytes() As Byte
    Public Shared bigEndian As Boolean = True

    Public Shared eventParams() As Byte = {}
    Public Shared eventParamsOrigOffset As UInteger

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

    Private Function RawStrFromBytes(ByVal loc As UInteger) As Byte()
        Dim cont As Boolean = True
        Dim len As Integer = 0

        While cont
            If bytes(loc + len) > 0 Then
                len += 1
            Else
                cont = False
            End If
        End While

        If len = 0 Then
            Return {}
        End If

        Dim strBytesJIS(len - 1) As Byte
        Array.Copy(bytes, loc, strBytesJIS, 0, len)

        Return strBytesJIS
    End Function
    Private Function Str2Bytes(ByVal str As String) As Byte()
        Dim BArr() As Byte
        BArr = Encoding.GetEncoding("shift_jis").GetBytes(str)
        Return BArr
    End Function
    Private Function RawStrToStr(ByVal rawStr As Byte()) As String
        Dim enc1 = Encoding.GetEncoding("shift_jis")
        Dim enc2 = Encoding.Unicode
        Dim strBytesUnicode As Byte() = Encoding.Convert(enc1, enc2, rawStr)
        Dim str As String = Encoding.Unicode.GetString(strBytesUnicode)
        Return str
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
    Private Sub readRow(ByRef dgv As DataGridView, ByRef layout As msbdata, ptr As UInteger)

        Dim currOffset As Integer = 0
        Dim partRow(layout.fieldCount) As String
        Dim partName As Byte() = {}
        Dim sibpath As Byte() = {}
        Dim textboost As Integer
        Dim hasSib As Boolean
        Dim Padding As Integer

        Dim nameoffset = SIntFromFour(ptr)
        partName = RawStrFromBytes(ptr + nameoffset)
        partRow(layout.getNameIndex) = RawStrToStr(partName)
        Padding = partName.Length + 1

        hasSib = layout.retrieveName(layout.getNameIndex + 1) = "Sibpath"
        If hasSib Then
            Dim siboffset = SIntFromFour(ptr + &H10)
            sibpath = RawStrFromBytes(ptr + siboffset)
            partRow(layout.getNameIndex + 1) = RawStrToStr(sibpath)

            Padding += sibpath.Length + 1
            If sibpath.Length = 0 Then
                Padding += 5
            End If
        End If

        Padding = (Padding + 3) And -&H4

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
                    partRow(j) = SingleFromFour(ptr + textboost + currOffset)
                    currOffset += 4
            End Select
        Next

        dgv.Rows.Add(partRow)
    End Sub
    Private Sub labelRows(ByVal dgv As DataGridView)
        'Label row headers with name
        For i = 0 To dgv.Rows.Count - 2
            dgv.Rows(i).HeaderCell.Value = dgv.Rows(i).Cells(25).Value
        Next
        dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
    End Sub
    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        bytes = File.ReadAllBytes(txtMSBfile.Text)


        Dim ptr As UInteger

        Dim nameoffset As UInteger

        Dim name As Byte()
        Dim sibpath As Byte()


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


        For i = 0 To points.Count - 1
            initDGV(pointsdgvs(i), points(i))
        Next

        For i = 0 To parts.Count - 1
            initDGV(partsdgvs(i), parts(i))
        Next


        For i = 0 To modelCnt - 2
            Dim currOffset As Integer = 0
            Dim mdlRow(models.fieldCount) As String

            ptr = UIntFromFour(modelPtr + &HC + i * &H4)

            nameoffset = UIntFromFour(ptr)
            name = RawStrFromBytes(ptr + nameoffset)
            sibpath = RawStrFromBytes(ptr + nameoffset + name.Length + 1)

            mdlRow(models.getNameIndex) = RawStrToStr(name)
            mdlRow(models.getNameIndex + 1) = RawStrToStr(sibpath)

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
        Dim pointtype(4) As Integer
        pointtype = {0, 2, 3, 5}

        For i = 0 To pointCnt - 2
            padding = 0
            ptr = UIntFromFour(pointPtr + &HC + i * &H4)

            idx = Array.IndexOf(pointtype, SIntFromFour(ptr + &HC))
            readRow(pointsdgvs(idx), points(idx), ptr)
        Next


        idx = 0
        Dim parttype(9) As Integer
        parttype = {0, 1, 2, 4, 5, 8, 9, &HA, &HB}

        For i = 0 To partsCnt - 2
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)

            idx = Array.IndexOf(parttype, SIntFromFour(ptr + &H4))
            readRow(partsdgvs(idx), parts(idx), ptr)
        Next


        mapstudioPtr = UIntFromFour((partsCnt * &H4) + &H8 + partsPtr)
        mapstudioCnt = UIntFromFour(mapstudioPtr + &H8)


        labelRows(dgvMapPieces0)
        labelRows(dgvObjects1)
        labelRows(dgvCreatures2)
        labelRows(dgvCreatures4)
        labelRows(dgvCollision5)
        labelRows(dgvNavimesh8)
        labelRows(dgvObjects9)
        labelRows(dgvCreatures10)
        labelRows(dgvCollision11)

    End Sub

    Private Sub saveRow(ByRef MSBStream As FileStream, ByRef row As DataGridViewRow, ByRef data As msbdata, ByRef ptr As Integer, ByRef partsidx As Integer)
        Dim curroffset = MSBStream.Position
        MSBStream.Position = ptr + &HC + partsidx * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        MSBStream.Position = curroffset

        Dim nameoffset = row.Cells(0).Value
        Dim Name As Byte() = Str2Bytes(row.Cells(data.getNameIndex).Value)

        Dim Padding = Name.Length + 1

        Dim hasSib As Boolean = data.retrieveName(data.getNameIndex + 1) = "Sibpath"
        If hasSib Then
            Dim sibpath As Byte() = Str2Bytes(row.Cells(data.getNameIndex + 1).Value)
            Padding += sibpath.Length + 1
            If sibpath.Length = 0 Then
                Padding += 5
            End If
        End If

        Padding = (Padding + 3) And -&H4

        For j = 0 To data.fieldCount - 1
            If j = data.getNameIndex Then MSBStream.Position = curroffset + nameoffset

            If hasSib Then
                If j = data.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + Padding
            Else
                If j = data.getNameIndex + 1 Then MSBStream.Position = curroffset + nameoffset + Padding
            End If
            Select Case data.retrieveType(j)
                Case "i8"
                    WriteBytes(MSBStream, Int8ToOneByte(row.Cells(j).Value))
                Case "i16"
                    WriteBytes(MSBStream, Int16ToTwoByte(row.Cells(j).Value))
                Case "i32"
                    WriteBytes(MSBStream, Int32ToFourByte(row.Cells(j).Value))
                Case "f32"
                    WriteBytes(MSBStream, SingleToFourByte(row.Cells(j).Value))
                Case "string"
                    WriteBytes(MSBStream, Str2Bytes(row.Cells(j).Value))
                    WriteBytes(MSBStream, Int8ToOneByte(0))
            End Select
        Next
        partsidx += 1
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

        Dim name As Byte()
        Dim sibpath As Byte()

        Dim padding As UInteger

        If Not File.Exists(txtMSBfile.Text & ".bak") Then
            File.WriteAllBytes(txtMSBfile.Text & ".bak", bytes)
        End If

        If File.Exists(txtMSBfile.Text) Then File.Delete(txtMSBfile.Text)
        Dim MSBStream As New IO.FileStream(txtMSBfile.Text, IO.FileMode.CreateNew)

        WriteBytes(MSBStream, UInt32ToFourByte(0))


        modelPtr = 0
        modelCnt = dgvModels.Rows.Count
        curroffset = modelPtr + &HC + (modelCnt) * &H4

        MSBStream.Position = &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(modelCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("MODEL_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        'Models
        For i As UInteger = 0 To modelCnt - 2
            curroffset = MSBStream.Position
            MSBStream.Position = modelPtr + &HC + i * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvModels.Rows(i).Cells(0).Value
            name = Str2Bytes(dgvModels.Rows(i).Cells(models.getNameIndex).Value)
            sibpath = Str2Bytes(dgvModels.Rows(i).Cells(models.getNameIndex + 1).Value)

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
                        WriteBytes(MSBStream, Str2Bytes(dgvModels.Rows(i).Cells(j).Value))
                        WriteBytes(MSBStream, Int8ToOneByte(0))
                End Select
            Next
            MSBStream.Position = curroffset + nameoffset + padding
        Next


        eventPtr = (MSBStream.Length And -&H4) + &H4
        MSBStream.Position = modelPtr + &HC + (modelCnt - 1) * &H4
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
        MSBStream.Position = eventPtr + &HC + (eventCnt - 1) * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(pointPtr))
        MSBStream.Position = pointPtr

        pointCnt = dgvPoints0.Rows.Count + dgvPoints2.Rows.Count + dgvPoints3.Rows.Count + dgvPoints5.Rows.Count - 4 + 1
        curroffset = pointPtr + &HC + pointCnt * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(pointCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("POINT_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        Dim partsidx = 0
        Dim rowCount = 0
        ' The game needs each point to be in order, so aggregate each point type, sorted by index
        Dim rows = New List(Of Tuple(Of DataGridViewRow, msbdata))
        Dim keys = New List(Of Integer)
        For i = 0 To pointsdgvs.Length - 1
            For j = 0 To pointsdgvs(i).Rows.Count - 2
                Dim row = pointsdgvs(i).Rows(j)
                rows.Add(Tuple.Create(row, points(i)))
                Dim idx = CInt(row.Cells(2).Value)
                keys.Add(idx)
            Next
        Next
        Dim sortedRows As Array = rows.ToArray
        Array.Sort(keys.ToArray, sortedRows)

        For i = 0 To sortedRows.Length - 1
            Dim t = CType(sortedRows(i), Tuple(Of DataGridViewRow, msbdata))
            saveRow(MSBStream, t.Item1, t.Item2, pointPtr, partsidx)
        Next


        partsPtr = MSBStream.Length
        MSBStream.Position = pointPtr + &HC + (pointCnt - 1) * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(partsPtr))
        MSBStream.Position = partsPtr

        partsCnt = dgvMapPieces0.Rows.Count + dgvObjects1.Rows.Count + dgvCreatures2.Rows.Count + dgvCreatures4.Rows.Count + dgvCollision5.Rows.Count + dgvNavimesh8.Rows.Count + dgvObjects9.Rows.Count + dgvCreatures10.Rows.Count + dgvCollision11.Rows.Count - 9 + 1
        curroffset = partsPtr + &HC + partsCnt * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(partsCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("PARTS_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        partsidx = 0

        For i = 0 To partsdgvs.Length - 1
            For j = 0 To partsdgvs(i).Rows.Count - 2
                saveRow(MSBStream, partsdgvs(i).Rows(j), parts(i), partsPtr, partsidx)
            Next
        Next

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
    Private Sub pointPrep0()
        With points0
            .add("Name Offset", "i32", Color.White)
            .add("x04", "i32", Color.LightGray)
            .add("index", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("x28", "i32", Color.LightGray)
            .add("x2c", "i32", Color.LightGray)
            .add("x30", "i32", Color.LightGray)
            .add("x34", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("p+0x00", "i32", Color.LightGray)
            .add("p+0x04", "i32", Color.LightGray)
            .add("EventEntityID", "i32", Color.White)
        End With
    End Sub
    Private Sub pointPrep2()
        With points2
            .add("Name Offset", "i32", Color.White)
            .add("x04", "i32", Color.LightGray)
            .add("index", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("x28", "i32", Color.LightGray)
            .add("x2c", "i32", Color.LightGray)
            .add("x30", "i32", Color.LightGray)
            .add("x34", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("p+0x00", "i32", Color.LightGray)
            .add("p+0x04", "i32", Color.LightGray)
            .add("p+0x08", "f32", Color.LightGray)
            .add("EventEntityID", "i32", Color.White)
        End With
    End Sub
    Private Sub pointPrep3()
        With points3
            .add("Name Offset", "i32", Color.White)
            .add("x04", "i32", Color.LightGray)
            .add("index", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("x28", "i32", Color.LightGray)
            .add("x2c", "i32", Color.LightGray)
            .add("x30", "i32", Color.LightGray)
            .add("x34", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("p+0x00", "i32", Color.LightGray)
            .add("p+0x04", "i32", Color.LightGray)
            .add("p+0x08", "f32", Color.LightGray)
            .add("p+0x0C", "f32", Color.LightGray)
            .add("EventEntityID", "i32", Color.White)
        End With
    End Sub
    Private Sub pointPrep5()
        With points5
            .add("Name Offset", "i32", Color.White)
            .add("x04", "i32", Color.LightGray)
            .add("index", "i32", Color.White)
            .add("Type", "i32", Color.White)
            .add("X pos", "f32", Color.White)
            .add("Y pos", "f32", Color.White)
            .add("Z pos", "f32", Color.White)
            .add("Rot X", "f32", Color.White)
            .add("Rot Y", "f32", Color.White)
            .add("Rot Z", "f32", Color.White)
            .add("x28", "i32", Color.LightGray)
            .add("x2c", "i32", Color.LightGray)
            .add("x30", "i32", Color.LightGray)
            .add("x34", "i32", Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", Color.White)
            .add("p+0x00", "i32", Color.LightGray)
            .add("p+0x04", "i32", Color.LightGray)
            .add("p+0x08", "f32", Color.LightGray)
            .add("p+0x0C", "f32", Color.LightGray)
            .add("p+0x10", "f32", Color.LightGray)
            .add("EventEntityID", "i32", Color.White)
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
            .add("PhysIndex", "i32", Color.White)
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
            .add("PhysIndex", "i32", Color.White)
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
            .add("PhysIndex", "i32", Color.White)
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
            .add("PhysIndex", "i32", Color.White)
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
        pointPrep0()
        pointPrep2()
        pointPrep3()
        pointPrep5()
        map0Prep()
        obj1Prep()
        crt2Prep()
        crt4Prep()
        coll5Prep()
        navi8Prep()
        obj9Prep()
        crt10Prep()
        coll11Prep()

        points = {points0, points2, points3, points5}
        pointsdgvs = {dgvPoints0, dgvPoints2, dgvPoints3, dgvPoints5}

        parts = {mapPieces0, objects1, creatures2, creatures4, collision5, navimesh8, objects9, creatures10, collision11}
        partsdgvs = {dgvMapPieces0, dgvObjects1, dgvCreatures2, dgvCreatures4, dgvCollision5, dgvNavimesh8, dgvObjects9, dgvCreatures10, dgvCollision11}

        layouts = {models}.Concat(points).Concat(parts).ToArray()
        dgvs = {dgvModels}.Concat(pointsdgvs).Concat(partsdgvs).ToArray()
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        Dim idx As Integer
        idx = tabParts.SelectedIndex

        copyEntry(dgvs(idx), dgvs(idx).SelectedCells(0).RowIndex)
    End Sub

    Sub copyEntry(ByRef dgv As DataGridView, rowidx As Integer)
        Dim row(dgv.Columns.Count - 1)
        For i = 0 To row.Count - 1
            row(i) = dgv.Rows(dgv.SelectedCells(0).RowIndex).Cells(i).FormattedValue
        Next
        dgv.Rows.Add(row)

        If ChkUpdatePhysIndices.Checked Then
            UpdatePhysIndices(tabParts.SelectedIndex, 1)
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim idx As Integer
        idx = tabParts.SelectedIndex

        deleteEntry(dgvs(idx), dgvs(idx).SelectedCells(0).RowIndex)
    End Sub

    Sub deleteEntry(ByRef dgv As DataGridView, rowidx As Integer)
        dgv.Rows.RemoveAt(rowidx)

        If ChkUpdatePhysIndices.Checked Then
            UpdatePhysIndices(tabParts.SelectedIndex, -1)
        End If
    End Sub

    ' Won't work properly if the user edits map parts or collision5 parts, can be fixed later if there's ever a need.
    Sub UpdatePhysIndices(dgvSourceIdx As Integer, delta As Integer)
        Dim colIdx = Array.IndexOf(layouts, collision5)
        Dim mapIdx = Array.IndexOf(layouts, mapPieces0)

        If dgvSourceIdx > colidx Then
            Return
        End If

        Dim firstPhysIdx As Integer = -1
        For i = mapIdx To colIdx - 1
            firstPhysIdx += dgvs(i).Rows.Count - 1
        Next

        For i = 0 To dgvs.Length - 1
            Dim idx = layouts(i).getFieldIndex("PhysIndex")
            If idx = -1 Then
                Continue For
            End If

            For j = 0 To dgvs(i).Rows.Count - 2
                Dim row = dgvs(i).Rows(j)

                Dim oldValue = CInt(row.Cells(idx).Value)
                If oldValue < firstPhysIdx Then
                    ' I don't know why some things use map part indices and most others use collision indices.
                    Continue For
                End If

                Dim newValue = oldValue + delta
                row.Cells(idx).Value = newValue
            Next
        Next
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim openDlg As New OpenFileDialog()

        openDlg.Filter = "MSB File|*.MSB"
        openDlg.Title = "Open your MSB file"

        If openDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtMSBfile.Text = openDlg.FileName
        End If
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
    Public Function getFieldIndex(ByVal name As String) As Integer
        Return fieldNames.IndexOf(name)
    End Function
End Class