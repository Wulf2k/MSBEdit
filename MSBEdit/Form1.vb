Imports System.IO
Imports System.Text


'Creature type 0x4 is sized wrong
'TODO:  Confirm the above

Public Class frmMSBEdit

    Public models As msbdata
    Public events0 As msbdata
    Public events1 As msbdata
    Public events2 As msbdata
    Public events3 As msbdata
    Public events4 As msbdata
    Public events5 As msbdata
    Public events6 As msbdata
    Public events7 As msbdata
    Public events8 As msbdata
    Public events9 As msbdata
    Public events10 As msbdata
    Public events11 As msbdata
    Public events12 As msbdata
    Public points0 As msbdata
    Public points2 As msbdata
    Public points3 As msbdata
    Public points5 As msbdata
    Public mapPieces0 As msbdata
    Public objects1 As msbdata
    Public creatures2 As msbdata
    Public creatures4 As msbdata
    Public collision5 As msbdata
    Public navimesh8 As msbdata
    Public objects9 As msbdata
    Public creatures10 As msbdata
    Public collision11 As msbdata

    Dim dgvs() As DataGridView = {}
    Dim layouts() As msbdata = {}

    Dim Shadows events() As msbdata = {}
    Dim eventsdgvs() As DataGridView = {}

    Dim points() As msbdata = {}
    Dim pointsdgvs() As DataGridView = {}

    Dim parts() As msbdata = {}
    Dim partsdgvs() As DataGridView = {}

    Public Shared bytes() As Byte
    Public Shared bigEndian As Boolean = True

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
            dgv.Columns(i).DefaultCellStyle.ForeColor = layout.retrieveForeColor(i)
            dgv.Columns(i).SortMode = DataGridViewColumnSortMode.NotSortable

            If layout.isKnown(i) = False Then
                dgv.Columns(i).Visible = chkShowUnknowns.Checked
            End If
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
        For i = 0 To dgv.Rows.Count - 1
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

        eventPtr = UIntFromFour((modelCnt * &H4) + &H8)
        eventCnt = UIntFromFour(eventPtr + &H8)

        pointPtr = UIntFromFour((eventCnt * &H4) + &H8 + eventPtr)
        pointCnt = UIntFromFour(pointPtr + &H8)

        partsPtr = UIntFromFour((pointCnt * &H4) + &H8 + pointPtr)
        partsCnt = UIntFromFour(partsPtr + &H8)

        initDGV(dgvModels, models)

        For i = 0 To events.Count - 1
            initDGV(eventsdgvs(i), events(i))
        Next

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
        Dim eventtype(13) As Integer
        eventtype = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

        For i = 0 To eventCnt - 2
            ptr = UIntFromFour(eventPtr + &HC + i * &H4)

            idx = Array.IndexOf(eventtype, SIntFromFour(ptr + &H8))
            readRow(eventsdgvs(idx), Events(idx), ptr)
        Next


        idx = 0
        Dim pointtype(4) As Integer
        pointtype = {0, 2, 3, 5}

        For i = 0 To pointCnt - 2
            ptr = UIntFromFour(pointPtr + &HC + i * &H4)

            idx = Array.IndexOf(pointtype, SIntFromFour(ptr + &HC))
            readRow(pointsdgvs(idx), points(idx), ptr)
        Next


        idx = 0
        Dim parttype(9) As Integer
        parttype = {0, 1, 2, 4, 5, 8, 9, &HA, &HB}

        For i = 0 To partsCnt - 2
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

        updateStatusBar()
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
        modelCnt = dgvModels.Rows.Count + 1
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

        eventCnt = 1
        For Each dgv In eventsdgvs
            eventCnt += dgv.Rows.Count
        Next
        curroffset = eventPtr + &HC + eventCnt * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(eventCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("EVENT_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        ' I haven't tested if the game wants events to be in order (like points) but it probably does.
        Dim rows = New List(Of Tuple(Of DataGridViewRow, msbdata))
        Dim keys = New List(Of Integer)
        For i = 0 To eventsdgvs.Length - 1
            For j = 0 To eventsdgvs(i).Rows.Count - 1
                Dim row = eventsdgvs(i).Rows(j)
                rows.Add(Tuple.Create(row, events(i)))
                Dim idx = CInt(row.Cells(3).Value)
                keys.Add(idx)
            Next
        Next
        Dim sortedRows As Array = rows.ToArray
        Array.Sort(keys.ToArray, rows.ToArray)

        Dim eventsidx = 0
        For i = 0 To sortedRows.Length - 1
            Dim t = CType(sortedRows(i), Tuple(Of DataGridViewRow, msbdata))
            saveRow(MSBStream, t.Item1, t.Item2, eventPtr, eventsidx)
        Next


        pointPtr = MSBStream.Length
        MSBStream.Position = eventPtr + &HC + (eventCnt - 1) * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(pointPtr))
        MSBStream.Position = pointPtr

        pointCnt = dgvPoints0.Rows.Count + dgvPoints2.Rows.Count + dgvPoints3.Rows.Count + dgvPoints5.Rows.Count + 1
        curroffset = pointPtr + &HC + pointCnt * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(pointCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("POINT_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        ' The game needs each point to be in order, so aggregate each point type, sorted by index
        rows = New List(Of Tuple(Of DataGridViewRow, msbdata))
        keys = New List(Of Integer)
        For i = 0 To pointsdgvs.Length - 1
            For j = 0 To pointsdgvs(i).Rows.Count - 1
                Dim row = pointsdgvs(i).Rows(j)
                rows.Add(Tuple.Create(row, points(i)))
                Dim idx = CInt(row.Cells(2).Value)
                keys.Add(idx)
            Next
        Next
        sortedRows = rows.ToArray
        Array.Sort(keys.ToArray, sortedRows)

        Dim partsidx = 0
        For i = 0 To sortedRows.Length - 1
            Dim t = CType(sortedRows(i), Tuple(Of DataGridViewRow, msbdata))
            saveRow(MSBStream, t.Item1, t.Item2, pointPtr, partsidx)
        Next


        partsPtr = MSBStream.Length
        MSBStream.Position = pointPtr + &HC + (pointCnt - 1) * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(partsPtr))
        MSBStream.Position = partsPtr

        partsCnt = dgvMapPieces0.Rows.Count + dgvObjects1.Rows.Count + dgvCreatures2.Rows.Count + dgvCreatures4.Rows.Count + dgvCollision5.Rows.Count + dgvNavimesh8.Rows.Count + dgvObjects9.Rows.Count + dgvCreatures10.Rows.Count + dgvCollision11.Rows.Count + 1
        curroffset = partsPtr + &HC + partsCnt * &H4
        WriteBytes(MSBStream, UInt32ToFourByte(0))
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(partsCnt))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("PARTS_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4

        partsidx = 0

        For i = 0 To partsdgvs.Length - 1
            For j = 0 To partsdgvs(i).Rows.Count - 1
                saveRow(MSBStream, partsdgvs(i).Rows(j), parts(i), partsPtr, partsidx)
            Next
        Next

        MSBStream.Close()
        MsgBox("Save Complete.")
    End Sub


    Private Sub frmMSBEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        models = msbdata.generate("models")
        events0 = msbdata.generate("events0")
        events1 = msbdata.generate("events1")
        events2 = msbdata.generate("events2")
        events3 = msbdata.generate("events3")
        events4 = msbdata.generate("events4")
        events5 = msbdata.generate("events5")
        events6 = msbdata.generate("events6")
        events7 = msbdata.generate("events7")
        events8 = msbdata.generate("events8")
        events9 = msbdata.generate("events9")
        events10 = msbdata.generate("events10")
        events11 = msbdata.generate("events11")
        events12 = msbdata.generate("events12")
        points0 = msbdata.generate("points0")
        points2 = msbdata.generate("points2")
        points3 = msbdata.generate("points3")
        points5 = msbdata.generate("points5")
        mapPieces0 = msbdata.generate("mapPieces0")
        objects1 = msbdata.generate("objects1")
        creatures2 = msbdata.generate("creatures2")
        creatures4 = msbdata.generate("creatures4")
        collision5 = msbdata.generate("collision5")
        navimesh8 = msbdata.generate("navimesh8")
        objects9 = msbdata.generate("objects9")
        creatures10 = msbdata.generate("creatures10")
        collision11 = msbdata.generate("collision11")

        events = {events0, events1, events2, events3, events4, events5, events6, events7, events8, events9, events10, events11, events12}
        eventsdgvs = {dgvEvents0, dgvEvents1, dgvEvents2, dgvEvents3, dgvEvents4, dgvEvents5, dgvEvents6, dgvEvents7, dgvEvents8, dgvEvents9, dgvEvents10, dgvEvents11, dgvEvents12}

        points = {points0, points2, points3, points5}
        pointsdgvs = {dgvPoints0, dgvPoints2, dgvPoints3, dgvPoints5}

        parts = {mapPieces0, objects1, creatures2, creatures4, collision5, navimesh8, objects9, creatures10, collision11}
        partsdgvs = {dgvMapPieces0, dgvObjects1, dgvCreatures2, dgvCreatures4, dgvCollision5, dgvNavimesh8, dgvObjects9, dgvCreatures10, dgvCollision11}

        layouts = {models}.Concat(events).Concat(points).Concat(parts).ToArray()
        dgvs = {dgvModels}.Concat(eventsdgvs).Concat(pointsdgvs).Concat(partsdgvs).ToArray()

        For Each dgv In dgvs
            AddHandler dgv.SelectionChanged, AddressOf Me.onDgvSelectionChanged
            AddHandler dgv.KeyDown, AddressOf Me.onDgvKeyDown
        Next

        AddHandler tabControlRoot.SelectedIndexChanged, AddressOf Me.onTabControlSelectedIndexChanged
        AddHandler tabControlEvents.SelectedIndexChanged, AddressOf Me.onTabControlSelectedIndexChanged
        AddHandler tabControlPoints.SelectedIndexChanged, AddressOf Me.onTabControlSelectedIndexChanged
        AddHandler tabControlParts.SelectedIndexChanged, AddressOf Me.onTabControlSelectedIndexChanged
    End Sub

    Private Function getCurrentRootTab()
        Return tabControlRoot.SelectedTab
    End Function

    Private Function getCurrentDgv() As DataGridView
        Dim rootTab As TabPage = getCurrentRootTab()
        If (rootTab Is tabModels) Then
            Return dgvModels
        ElseIf rootTab Is tabEvents Then
            Return eventsdgvs(tabControlEvents.SelectedIndex)
        ElseIf rootTab Is tabPoints Then
            Return pointsdgvs(tabControlPoints.SelectedIndex)
        ElseIf rootTab Is tabParts Then
            Return partsdgvs(tabControlParts.SelectedIndex)
        Else
            Throw New Exception()
        End If
    End Function

    Private Function getSectionIndex(sourceDgv As DataGridView, Optional ByVal currentSection() As DataGridView = Nothing)
        If currentSection Is Nothing Then
            Dim dgvModelsArray = {dgvModels}
            For Each section() As DataGridView In {dgvModelsArray, eventsdgvs, pointsdgvs, partsdgvs}
                If section.Contains(sourceDgv) Then
                    currentSection = section

                    Exit For
                End If
            Next
        End If

        Dim idx As Integer = 0
        For Each dgv In currentSection
            If dgv Is sourceDgv Then
                Exit For
            End If

            idx += dgv.Rows.Count
        Next

        Return idx
    End Function

    Private Sub updateStatusBar()
        Dim currentDgv = getCurrentDgv()

        labelRowCount.Text = "Row count: " & currentDgv.Rows.Count.ToString & " "

        If currentDgv.Rows.Count = 0 Or currentDgv.SelectedCells.Count = 0 Then
            Return
        End If

        Dim cell = currentDgv.SelectedCells(currentDgv.SelectedCells.Count - 1)

        Dim idx As Integer = getSectionIndex(currentDgv)

        idx += cell.RowIndex

        labelSectionIndex.Text = "Section index: " & idx.ToString
    End Sub

    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        Dim dgv = getCurrentDgv()

        If dgv.Rows.Count = 0 Then
            Return
        End If

        copyEntry(dgv, dgv.SelectedCells(0).RowIndex)
    End Sub

    Sub copyEntry(ByRef dgv As DataGridView, rowidx As Integer)
        Dim row(dgv.Columns.Count - 1)
        For i = 0 To row.Count - 1
            row(i) = dgv.Rows(dgv.SelectedCells(0).RowIndex).Cells(i).FormattedValue
        Next
        dgv.Rows.Add(row)

        If ChkUpdatePointerIndices.Checked Then
            If pointsdgvs.Contains(dgv) Then
                Dim totalPoints = getSectionIndex(pointsdgvs.Last, pointsdgvs) + pointsdgvs.Last.Rows.Count
                dgv.Rows(dgv.Rows.Count - 1).Cells(2).Value = totalPoints - 1
            Else
                UpdatePointerIndices(dgv, dgv.Rows.Count - 1, 1)
            End If
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim dgv = getCurrentDgv()

        If dgv.Rows.Count = 0 Then
            Return
        End If

        If dgv.SelectedRows.Count > 0 Then
            For each cell In dgv.SelectedRows
                deleteEntry(dgv, dgv.SelectedRows(0).Index)
            Next
        Else
            deleteEntry(dgv, dgv.SelectedCells(0).RowIndex)
        End If

        
    End Sub

    Sub deleteEntry(ByRef dgv As DataGridView, rowidx As Integer)
        If ChkUpdatePointerIndices.Checked Then
            UpdatePointerIndices(dgv, rowidx, -1)
        End If

        dgv.Rows.RemoveAt(rowidx)
    End Sub

    Private Sub btnMoveUp_Click(sender As Object, e As EventArgs) Handles btnMoveUp.Click
        Dim dgv = getCurrentDgv()

        If dgv.Rows.Count < 2 Then
            Return
        End If

        Dim rowIndex = dgv.SelectedCells(0).RowIndex

        If rowIndex = 0 Then
            Return
        End If

        If ChkUpdatePointerIndices.Checked Then
            UpdatePointerIndices(dgv, rowIndex, -1, True)
        End If

        Dim rowAbove As DataGridViewRow = dgv.Rows(rowIndex - 1)

        dgv.Rows.RemoveAt(rowIndex - 1)
        dgv.Rows.Insert(rowIndex, rowAbove)

        updateStatusBar()
    End Sub

    Private Sub btnMoveDown_Click(sender As Object, e As EventArgs) Handles btnMoveDown.Click
        Dim dgv = getCurrentDgv()

        If dgv.Rows.Count < 2 Then
            Return
        End If

        Dim rowIndex = dgv.SelectedCells(0).RowIndex

        If rowIndex = dgv.Rows.Count - 1 Then
            Return
        End If

        If ChkUpdatePointerIndices.Checked Then
            UpdatePointerIndices(dgv, rowIndex, 1, True)
        End If

        Dim rowBelow As DataGridViewRow = dgv.Rows(rowIndex + 1)

        dgv.Rows.RemoveAt(rowIndex + 1)
        dgv.Rows.Insert(rowIndex, rowBelow)

        updateStatusBar()
    End Sub

    ' When a row is added/deleted/moved in the points or parts section, update anything in the entire file that points to
    ' elements past that one.
    Sub UpdatePointerIndices(sourceDgv As DataGridView, sourceRowIdx As Integer, delta As Integer, Optional swap As Boolean = False)
        Dim currentRootTab = getCurrentRootTab()

        Dim isPoints As Boolean = currentRootTab Is tabPoints
        Dim isParts As Boolean = currentRootTab Is tabParts

        If isPoints = False And isParts = False Then
            Return
        End If

        ' Things that point to parts use their position in the parts section. For points, it's similar, but points are sorted
        ' by their index cell when the file is saved and they also need to have proper index values or the game doesn't like it.

        If isParts And swap = True Then
            Return
        End If

        Dim fromSectionIdx As Integer = 0
        If isPoints Then
            fromSectionIdx = CInt(sourceDgv.Rows(sourceRowIdx).Cells(2).Value)

            shiftPointIndices(sourceDgv, sourceRowIdx, delta)
        Else
            fromSectionIdx = getSectionIndex(sourceDgv) + sourceRowIdx
        End If

        For i = 0 To dgvs.Count - 1
            Dim dgv = dgvs(i)
            Dim layout = layouts(i)

            Dim indices As List(Of Integer) = Nothing
            If isPoints Then
                indices = layout.getPointIndices()
            ElseIf isParts Then
                indices = layout.getPartIndices()
            End If

            If indices.Count = 0 Then
                Continue For
            End If

            Dim otherSourceRowIdx = fromSectionIdx + delta

            For Each columnIdx In indices
                For rowIdx = 0 To dgv.Rows.Count - 1
                    Dim cell = dgv.Rows(rowIdx).Cells(columnIdx)
                    Dim sectionIdx = CInt(cell.Value)

                    If swap Then
                        If sectionIdx = fromSectionIdx Then
                            cell.Value = otherSourceRowIdx.ToString()
                        ElseIf sectionIdx = otherSourceRowIdx Then
                            cell.Value = fromSectionIdx.ToString()
                        End If
                    Else
                        If sectionIdx >= fromSectionIdx Then
                            cell.Value = (sectionIdx + delta).ToString()
                        End If
                    End If
                Next
            Next
        Next
    End Sub

    ' Shift the "index" of every point past a certain number
    ' (for the point type, naming things is hard)
    Private Sub shiftPointIndices(sourceDgv As DataGridView, rowIdx As Integer, delta As Integer)
        Dim fromPointIdx = CInt(sourceDgv.Rows(rowIdx).Cells(2).Value)

        For Each dgv In pointsdgvs
            For i = 0 To dgv.Rows.Count - 1
                Dim pointIdx = CInt(dgv.Rows(i).Cells(2).Value)
                If pointIdx >= fromPointIdx Then
                    dgv.Rows(i).Cells(2).Value = (pointIdx + delta).ToString
                End If
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

    Private Sub chkShowUnknownsChanged(sender As Object, e As EventArgs) Handles chkShowUnknowns.CheckedChanged
        For i = 0 To dgvs.Count - 1
            Dim dgv = dgvs(i)
            Dim layout = layouts(i)
            For j = 0 To layout.fieldCount - 1
                If layout.isKnown(j) = False Then
                    dgv.Columns(j).Visible = chkShowUnknowns.Checked
                End If
            Next
        Next
    End Sub

    Private Sub onDgvSelectionChanged(sender As Object, e As EventArgs)
        updateStatusBar()
    End Sub

    Private Sub onTabControlSelectedIndexChanged(sender As Object, e As EventArgs)
        updateStatusBar()
    End Sub

    Private Sub onDgvKeyDown(sender As Object, e As KeyEventArgs)
        If (e.Modifiers = Keys.Control AndAlso e.KeyCode = Keys.V) = False Then
            Return
        End If

        Dim o = CType(Clipboard.GetDataObject(), DataObject)
        If o.GetDataPresent(DataFormats.Text) = False Then
            Return
        End If
        Dim text = o.GetData(DataFormats.Text).ToString
        text = text.Replace(vbCr, "").TrimEnd(vbLf)
        Dim lines As String() = text.Split(vbLf)

        Dim sourceRows = New List(Of String())(lines.Length)
        Dim sourceMaxColumnCount = 0
        Dim sourceRowCount = lines.Length
        For i = 0 To lines.Length - 1
            Dim words = lines(i).Split(vbTab)
            sourceRows.Add(words)

            If words.Count > sourceMaxColumnCount Then
                sourceMaxColumnCount = words.Count
            End If
        Next

        Dim dgv = CType(sender, DataGridView)

        Dim cell As DataGridViewCell = dgv.SelectedCells(dgv.SelectedCells.Count - 1)
        Dim startColumn = cell.ColumnIndex
        Dim endColumn = startColumn + sourceMaxColumnCount - 1
        Dim startRow = cell.RowIndex
        Dim endRow = startRow + sourceRowCount - 1

        If endRow > dgv.RowCount - 1 Then
            endRow = dgv.RowCount - 1
        End If
        If endColumn > dgv.ColumnCount - 1 Then
            endColumn = dgv.ColumnCount - 1
        End If

        Dim destColumnCount = endColumn - startColumn + 1
        Dim destRowCount = endRow - startRow + 1

        For x = 0 To destColumnCount - 1
            For y = 0 To destRowCount - 1
                Dim newValue As String = ""
                If x < sourceRows(y).Count Then
                    newValue = sourceRows(y)(x)
                End If
                dgv.Rows(startRow + y).Cells(startColumn + x).Value = newValue
            Next
        Next
    End Sub
End Class
