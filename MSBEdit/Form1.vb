Imports System.IO


'Creature type 0x4 is sized wrong

Public Class frmMSBEdit

    Public models As msbdata = New msbdata
    Public creatures As msbdata = New msbdata
    Public mapPieces As msbdata = New msbdata
    Public objects As msbdata = New msbdata
    Public collision0x5 As msbdata = New msbdata
    Public collision0xB As msbdata = New msbdata
    Public unhandled As msbdata = New msbdata

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

        dgvModels.Rows.Clear()
        dgvModels.Columns.Clear()

        For i = 0 To models.fieldCount - 1
            dgvModels.Columns.Add(models.retrieveName(i), models.retrieveName(i))
            dgvModels.Columns(i).Width = models.retrieveWidth(i)
            dgvModels.Columns(i).DefaultCellStyle.BackColor = models.retrieveBackColor(i)
        Next


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

        For i = 0 To mapPieces.fieldCount - 1
            dgvMapPieces.Columns.Add(mapPieces.retrieveName(i), mapPieces.retrieveName(i))
            dgvMapPieces.Columns(i).Width = mapPieces.retrieveWidth(i)
            dgvMapPieces.Columns(i).DefaultCellStyle.BackColor = mapPieces.retrieveBackColor(i)
        Next




        dgvObjects.Rows.Clear()
        dgvObjects.Columns.Clear()

        For i = 0 To objects.fieldCount - 1
            dgvObjects.Columns.Add(objects.retrieveName(i), objects.retrieveName(i))
            dgvObjects.Columns(i).Width = objects.retrieveWidth(i)
            dgvObjects.Columns(i).DefaultCellStyle.BackColor = objects.retrieveBackColor(i)
        Next





        dgvCreatures.Rows.Clear()
        dgvCreatures.Columns.Clear()

        For i = 0 To creatures.fieldCount - 1
            dgvCreatures.Columns.Add(creatures.retrieveName(i), creatures.retrieveName(i))
            dgvCreatures.Columns(i).Width = creatures.retrieveWidth(i)
            dgvCreatures.Columns(i).DefaultCellStyle.BackColor = creatures.retrieveBackColor(i)
        Next




        dgvCollision0x5.Rows.Clear()
        dgvCollision0x5.Columns.Clear()

        For i = 0 To collision0x5.fieldCount - 1
            dgvCollision0x5.Columns.Add(collision0x5.retrieveName(i), collision0x5.retrieveName(i))
            dgvCollision0x5.Columns(i).Width = collision0x5.retrieveWidth(i)
            dgvCollision0x5.Columns(i).DefaultCellStyle.BackColor = collision0x5.retrieveBackColor(i)
        Next



        dgvCollision0xB.Rows.Clear()
        dgvCollision0xB.Columns.Clear()

        For i = 0 To collision0xB.fieldCount - 1
            dgvCollision0xB.Columns.Add(collision0xB.retrieveName(i), collision0xB.retrieveName(i))
            dgvCollision0xB.Columns(i).Width = collision0xB.retrieveWidth(i)
            dgvCollision0xB.Columns(i).DefaultCellStyle.BackColor = collision0xB.retrieveBackColor(i)
        Next




        dgvUnhandled.Rows.Clear()
        dgvUnhandled.Columns.Clear()

        For i = 0 To unhandled.fieldCount - 1
            dgvUnhandled.Columns.Add(unhandled.retrieveName(i), unhandled.retrieveName(i))
            dgvUnhandled.Columns(i).Width = unhandled.retrieveWidth(i)
            dgvUnhandled.Columns(i).DefaultCellStyle.BackColor = unhandled.retrieveBackColor(i)
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
        Next




        For i = 0 To partsCnt - 2
            padding = 0
            ptr = UIntFromFour(partsPtr + &HC + i * &H4)
            Select Case UIntFromFour(ptr + &H4)
                Case &H0, &H8
                    Dim currOffset As Integer = 0
                    Dim mapRow(mapPieces.fieldCount) As String
                    Dim mapName As String = ""
                    Dim mapSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)


                    mapRow(mapPieces.getNameIndex) = name
                    mapRow(mapPieces.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To mapPieces.fieldCount - 1
                        If j < mapPieces.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = mapPieces.getNameIndex Then currOffset = 0

                        Select Case mapPieces.retrieveType(j)
                            Case "i32"
                                mapRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                mapRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvMapPieces.Rows.Add(mapRow)



                Case &H1, &H9
                    Dim currOffset As Integer = 0
                    Dim objRow(objects.fieldCount) As String
                    Dim objName As String = ""
                    Dim objSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    objRow(objects.getNameIndex) = name
                    objRow(objects.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To objects.fieldCount - 1
                        If j < objects.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = objects.getNameIndex Then currOffset = 0

                        Select Case objects.retrieveType(j)
                            Case "i8"
                                objRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                objRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                objRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvObjects.Rows.Add(objRow)


                Case &H2, &H4, &HA
                    Dim currOffset As Integer = 0
                    Dim crtRow(creatures.fieldCount) As String
                    Dim crtName As String = ""
                    Dim crtSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    crtRow(creatures.getNameIndex) = name
                    crtRow(creatures.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To creatures.fieldCount - 1
                        If j < creatures.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = creatures.getNameIndex Then currOffset = 0

                        Select Case creatures.retrieveType(j)
                            Case "i32"
                                crtRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                crtRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCreatures.Rows.Add(crtRow)
                Case &H5
                    Dim currOffset As Integer = 0
                    Dim colRow(collision0x5.fieldCount) As String
                    Dim colName As String = ""
                    Dim colSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    colRow(collision0x5.getNameIndex) = name
                    colRow(collision0x5.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To collision0x5.fieldCount - 1
                        If j < collision0x5.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = collision0x5.getNameIndex Then currOffset = 0

                        Select Case collision0x5.retrieveType(j)
                            Case "i8"
                                colRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i16"
                                colRow(j) = SIntFromTwo(ptr + textboost + currOffset)
                                currOffset += 2
                            Case "i32"
                                colRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                colRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCollision0x5.Rows.Add(colRow)

                Case &HB
                    Dim currOffset As Integer = 0
                    Dim colRow(collision0xB.fieldCount) As String
                    Dim colName As String = ""
                    Dim colSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    colRow(collision0xB.getNameIndex) = name
                    colRow(collision0xB.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To collision0xB.fieldCount - 1
                        If j < collision0xB.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = collision0xB.getNameIndex Then currOffset = 0

                        Select Case collision0xB.retrieveType(j)
                            Case "i8"
                                colRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i16"
                                colRow(j) = SIntFromTwo(ptr + textboost + currOffset)
                                currOffset += 2
                            Case "i32"
                                colRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                colRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCollision0xB.Rows.Add(colRow)
                Case Else
                    Dim currOffset As Integer = 0
                    Dim unhRow(unhandled.fieldCount) As String
                    Dim unhName As String = ""
                    Dim unhSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    unhRow(unhandled.getNameIndex) = name
                    unhRow(unhandled.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To unhandled.fieldCount - 1
                        If j < unhandled.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = unhandled.getNameIndex Then currOffset = 0

                        Select Case unhandled.retrieveType(j)
                            Case "i8"
                                unhRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i16"
                                unhRow(j) = SIntFromTwo(ptr + textboost + currOffset)
                                currOffset += 2
                            Case "i32"
                                unhRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                unhRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvUnhandled.Rows.Add(unhRow)
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



        Dim curroffset As UInteger

        Dim nameoffset As UInteger

        Dim name As String
        Dim sibpath As String

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




        partsCnt = dgvMapPieces.Rows.Count + dgvCreatures.Rows.Count + dgvObjects.Rows.Count + dgvCollision0xB.Rows.Count - 5
        curroffset = partsPtr + &H10 + (partsCnt + 1) * &H4
        MSBStream.Position = partsPtr + &H4
        WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
        WriteBytes(MSBStream, UInt32ToFourByte(partsCnt + 2))

        MSBStream.Position = curroffset
        WriteBytes(MSBStream, Str2Bytes("PARTS_PARAM_ST"))
        MSBStream.Position = (MSBStream.Length And -&H4) + &H4


        'Map Pieces
        For i = 0 To dgvMapPieces.Rows.Count - 2
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvMapPieces.Rows(i).Cells(0).Value
            name = dgvMapPieces.Rows(i).Cells(mapPieces.getNameIndex).Value
            sibpath = dgvMapPieces.Rows(i).Cells(mapPieces.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding <= &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To mapPieces.fieldCount - 1
                If j = mapPieces.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = mapPieces.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + padding
                Select Case mapPieces.retrieveType(j)
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgvMapPieces.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgvMapPieces.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvMapPieces.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next

        'Objects
        For i = 0 To dgvObjects.Rows.Count - 2
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + dgvMapPieces.Rows.Count - 1) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvObjects.Rows(i).Cells(0).Value
            name = dgvObjects.Rows(i).Cells(objects.getNameIndex).Value
            sibpath = dgvObjects.Rows(i).Cells(objects.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding < &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To objects.fieldCount - 1
                If j = objects.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = objects.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + padding
                Select Case objects.retrieveType(j)
                    Case "i8"
                        WriteBytes(MSBStream, Int8ToOneByte(dgvObjects.Rows(i).Cells(j).Value))
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgvObjects.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgvObjects.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvObjects.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next

        'Creatures
        For i = 0 To dgvCreatures.Rows.Count - 2
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + dgvObjects.Rows.Count + dgvMapPieces.Rows.Count - 2) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvCreatures.Rows(i).Cells(0).Value
            name = dgvCreatures.Rows(i).Cells(creatures.getNameIndex).Value
            sibpath = dgvCreatures.Rows(i).Cells(creatures.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding <= &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To creatures.fieldCount - 1
                If j = creatures.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = creatures.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + padding
                Select Case creatures.retrieveType(j)
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgvCreatures.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgvCreatures.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvCreatures.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next

        'Collision x05
        For i = 0 To dgvCollision0x5.Rows.Count - 2
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + dgvObjects.Rows.Count + dgvMapPieces.Rows.Count + dgvCreatures.Rows.Count - 3) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvCollision0x5.Rows(i).Cells(0).Value
            name = dgvCollision0x5.Rows(i).Cells(collision0x5.getNameIndex).Value
            sibpath = dgvCollision0x5.Rows(i).Cells(collision0x5.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding <= &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To collision0x5.fieldCount - 1
                If j = collision0x5.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = collision0x5.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + padding
                Select Case collision0x5.retrieveType(j)
                    Case "i8"
                        WriteBytes(MSBStream, Int8ToOneByte(dgvCollision0x5.Rows(i).Cells(j).Value))
                    Case "i16"
                        WriteBytes(MSBStream, Int16ToTwoByte(dgvCollision0x5.Rows(i).Cells(j).Value))
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgvCollision0x5.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgvCollision0x5.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvCollision0x5.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next


        'Collision x0B
        For i = 0 To dgvCollision0xB.Rows.Count - 2
            curroffset = MSBStream.Position
            MSBStream.Position = partsPtr + &HC + (i + dgvObjects.Rows.Count + dgvMapPieces.Rows.Count + dgvCreatures.Rows.Count + dgvCollision0x5.ColumnCount - 4) * &H4
            WriteBytes(MSBStream, UInt32ToFourByte(curroffset))
            MSBStream.Position = curroffset

            nameoffset = dgvCollision0xB.Rows(i).Cells(0).Value
            name = dgvCollision0xB.Rows(i).Cells(collision0xB.getNameIndex).Value
            sibpath = dgvCollision0xB.Rows(i).Cells(collision0xB.getNameIndex + 1).Value

            padding = ((sibpath.Length + name.Length + 5) And -&H4)
            If padding <= &H10 Then
                padding = &H10
                If Not bigEndian Then padding += &H4
            End If

            For j = 0 To collision0xB.fieldCount - 1
                If j = collision0xB.getNameIndex Then MSBStream.Position = curroffset + nameoffset
                If j = collision0xB.getNameIndex + 2 Then MSBStream.Position = curroffset + nameoffset + padding
                Select Case collision0xB.retrieveType(j)
                    Case "i8"
                        WriteBytes(MSBStream, Int8ToOneByte(dgvCollision0xB.Rows(i).Cells(j).Value))
                    Case "i16"
                        WriteBytes(MSBStream, Int16ToTwoByte(dgvCollision0xB.Rows(i).Cells(j).Value))
                    Case "i32"
                        WriteBytes(MSBStream, Int32ToFourByte(dgvCollision0xB.Rows(i).Cells(j).Value))
                    Case "f32"
                        WriteBytes(MSBStream, SingleToFourByte(dgvCollision0xB.Rows(i).Cells(j).Value))
                    Case "string"
                        WriteBytes(MSBStream, Str2Bytes(dgvCollision0xB.Rows(i).Cells(j).Value & Chr(0)))
                End Select
            Next
        Next

        'File.WriteAllBytes(txtMSBfile.Text, bytes)
        MSBStream.Close()
        MsgBox("Save Complete.")
    End Sub

    Private Sub mapPrep()
        mapPieces.add("Name Offset", "i32", 40, Color.White)
        mapPieces.add("Type", "i32", 40, Color.White)
        mapPieces.add("Index", "i32", 40, Color.White)
        mapPieces.add("Model", "i32", 40, Color.White)
        mapPieces.add("x10", "i32", 40, Color.LightGray)
        mapPieces.add("X pos", "f32", 75, Color.White)
        mapPieces.add("Y pos", "f32", 75, Color.White)
        mapPieces.add("Z pos", "f32", 75, Color.White)
        mapPieces.add("Rotation 1", "f32", 60, Color.White)
        mapPieces.add("Rotation 2", "f32", 60, Color.White)
        mapPieces.add("Rotation 3", "f32", 60, Color.White)
        mapPieces.add("x2C", "f32", 40, Color.LightGray)
        mapPieces.add("x30", "f32", 40, Color.LightGray)
        mapPieces.add("x34", "f32", 40, Color.LightGray)
        mapPieces.add("x38", "i32", 75, Color.LightGray)
        mapPieces.add("x3C", "i32", 75, Color.LightGray)
        mapPieces.add("x40", "i32", 40, Color.LightGray)
        mapPieces.add("x44", "i32", 40, Color.LightGray)
        mapPieces.add("x48", "i32", 40, Color.LightGray)
        mapPieces.add("x4c", "i32", 40, Color.LightGray)
        mapPieces.add("x50", "i32", 40, Color.LightGray)
        mapPieces.add("x54", "i32", 40, Color.LightGray)
        mapPieces.add("x58", "i32", 40, Color.LightGray)
        mapPieces.add("x5C", "i32", 40, Color.LightGray)
        mapPieces.add("x60", "i32", 40, Color.LightGray)
        mapPieces.setNameIndex(mapPieces.fieldCount)
        mapPieces.add("Name", "string", 100, Color.White)
        mapPieces.add("Sibpath", "string", 100, Color.White)
        mapPieces.add("Script ID", "i32", 60, Color.White)
        mapPieces.add("p+x04", "i32", 75, Color.LightGray)
        mapPieces.add("p+x08", "i32", 75, Color.LightGray)
        mapPieces.add("p+x0C", "i32", 75, Color.LightGray)
        mapPieces.add("p+x10", "i32", 75, Color.LightGray)
        mapPieces.add("p+x14", "i32", 75, Color.LightGray)
        mapPieces.add("p+x18", "i32", 75, Color.LightGray)
        mapPieces.add("p+x1C", "i32", 75, Color.LightGray)
    End Sub
    Private Sub crtPrep()
        creatures.add("Name Offset", "i32", 40, Color.White)
        creatures.add("Type", "i32", 40, Color.White)
        creatures.add("Index", "i32", 40, Color.White)
        creatures.add("Model", "i32", 40, Color.White)
        creatures.add("x10", "i32", 40, Color.LightGray)
        creatures.add("X pos", "f32", 75, Color.White)
        creatures.add("Y pos", "f32", 75, Color.White)
        creatures.add("Z pos", "f32", 75, Color.White)
        creatures.add("Rotation 1", "f32", 60, Color.White)
        creatures.add("Rotation 2", "f32", 60, Color.White)
        creatures.add("Rotation 3", "f32", 60, Color.White)
        creatures.add("x2C", "f32", 40, Color.LightGray)
        creatures.add("x30", "f32", 40, Color.LightGray)
        creatures.add("x34", "f32", 40, Color.LightGray)
        creatures.add("x38", "i32", 40, Color.LightGray)
        creatures.add("x3C", "i32", 40, Color.LightGray)
        creatures.add("x40", "i32", 40, Color.LightGray)
        creatures.add("x44", "i32", 40, Color.LightGray)
        creatures.add("x48", "i32", 40, Color.LightGray)
        creatures.add("x4C", "i32", 40, Color.LightGray)
        creatures.add("x50", "i32", 40, Color.LightGray)
        creatures.add("x54", "i32", 40, Color.LightGray)
        creatures.add("x58", "i32", 40, Color.LightGray)
        creatures.add("x5C", "i32", 40, Color.LightGray)
        creatures.add("x60", "i32", 40, Color.LightGray)
        creatures.setNameIndex(creatures.fieldCount)
        creatures.add("Name", "string", 100, Color.White)
        creatures.add("Sibpath", "string", 100, Color.White)
        creatures.add("Script ID", "i32", 60, Color.White)
        creatures.add("p+x04", "i32", 75, Color.LightGray)
        creatures.add("p+x08", "i32", 60, Color.LightGray)
        creatures.add("p+x0C", "i32", 60, Color.LightGray)
        creatures.add("p+x10", "i32", 40, Color.LightGray)
        creatures.add("p+x14", "i32", 40, Color.LightGray)
        creatures.add("p+x18", "i32", 40, Color.LightGray)
        creatures.add("p+x1C", "i32", 40, Color.LightGray)
        creatures.add("p+x20", "i32", 70, Color.LightGray)
        creatures.add("p+x24", "i32", 60, Color.LightGray)
        creatures.add("NPC ID", "i32", 60, Color.White)
        creatures.add("p+x2C", "i32", 60, Color.LightGray)
        creatures.add("p+x30", "i32", 60, Color.LightGray)
        creatures.add("p+x34", "i32", 60, Color.LightGray)
        creatures.add("p+x38", "i32", 60, Color.LightGray)
        creatures.add("p+x3C", "i32", 75, Color.LightGray)
        creatures.add("p+x40", "i32", 75, Color.LightGray)
        creatures.add("p+x44", "i32", 75, Color.LightGray)
        creatures.add("p+x48", "i32", 75, Color.LightGray)
        creatures.add("p+x4C", "i32", 75, Color.LightGray)
        creatures.add("p+x50", "i32", 75, Color.LightGray)
        creatures.add("p+x54", "i32", 75, Color.LightGray)
    End Sub
    Private Sub mdlPrep()
        models.add("Name Offset", "i32", 40, Color.White)
        models.add("Type", "i32", 40, Color.White)
        models.add("Index", "i32", 40, Color.White)
        models.add("x0C", "i32", 40, Color.LightGray)
        models.add("x10", "i32", 40, Color.LightGray)
        models.add("x14", "i32", 40, Color.LightGray)
        models.add("x18", "i32", 40, Color.LightGray)
        models.add("x1C", "i32", 40, Color.LightGray)
        models.setNameIndex(models.fieldCount)
        models.add("Name", "string", 100, Color.White)
        models.add("Sibpath", "string", 400, Color.White)
    End Sub
    Private Sub objPrep()
        objects.add("Name Offset", "i32", 40, Color.White)
        objects.add("Type", "i32", 40, Color.White)
        objects.add("Index", "i32", 40, Color.White)
        objects.add("Model", "i32", 40, Color.White)
        objects.add("x10", "i32", 40, Color.LightGray)
        objects.add("X pos", "f32", 75, Color.White)
        objects.add("Y pos", "f32", 75, Color.White)
        objects.add("Z pos", "f32", 75, Color.White)
        objects.add("Rotation 1", "f32", 60, Color.White)
        objects.add("Rotation 2", "f32", 60, Color.White)
        objects.add("Rotation 3", "f32", 60, Color.White)
        objects.add("x2C", "f32", 40, Color.LightGray)
        objects.add("x30", "f32", 40, Color.LightGray)
        objects.add("x34", "f32", 40, Color.LightGray)
        objects.add("x38", "i32", 40, Color.LightGray)
        objects.add("x3C", "i32", 40, Color.LightGray)
        objects.add("x40", "i32", 40, Color.LightGray)
        objects.add("x44", "i32", 40, Color.LightGray)
        objects.add("x48", "i32", 40, Color.LightGray)
        objects.add("x4C", "i32", 40, Color.LightGray)
        objects.add("x50", "i32", 40, Color.LightGray)
        objects.add("x54", "i32", 40, Color.LightGray)
        objects.add("x58", "i32", 40, Color.LightGray)
        objects.add("x5C", "i32", 40, Color.LightGray)
        objects.add("x60", "i32", 40, Color.LightGray)
        objects.setNameIndex(objects.fieldCount)
        objects.add("Name", "string", 100, Color.White)
        objects.add("Sibpath", "string", 100, Color.White)
        objects.add("Script ID", "i32", 60, Color.White)
        objects.add("p+x04", "i8", 40, Color.LightGray)
        objects.add("p+x05", "i8", 40, Color.LightGray)
        objects.add("p+x06", "i8", 40, Color.LightGray)
        objects.add("p+x07", "i8", 40, Color.LightGray)
        objects.add("p+x08", "i32", 60, Color.LightGray)
        objects.add("p+x0C", "i8", 40, Color.LightGray)
        objects.add("p+x0D", "i8", 40, Color.LightGray)
        objects.add("p+x0E", "i8", 40, Color.LightGray)
        objects.add("p+x0F", "i8", 40, Color.LightGray)
        objects.add("p+x10", "i8", 40, Color.LightGray)
        objects.add("p+x11", "i8", 40, Color.LightGray)
        objects.add("p+x12", "i8", 40, Color.LightGray)
        objects.add("p+x13", "i8", 40, Color.LightGray)
        objects.add("p+x14", "i32", 40, Color.LightGray)
        objects.add("p+x18", "i32", 40, Color.LightGray)
        objects.add("p+x1C", "i32", 40, Color.LightGray)
        objects.add("p+x20", "i32", 70, Color.LightGray)
        objects.add("p+x24", "i32", 60, Color.LightGray)
        objects.add("p+x28", "i32", 60, Color.White)
        objects.add("p+x2C", "i32", 60, Color.LightGray)
    End Sub
    Private Sub col0x5Prep()
        collision0x5.add("Name Offset", "i32", 40, Color.White)
        collision0x5.add("Type", "i32", 40, Color.White)
        collision0x5.add("Index", "i32", 40, Color.White)
        collision0x5.add("Model", "i32", 40, Color.White)
        collision0x5.add("x10", "i32", 40, Color.LightGray)
        collision0x5.add("X pos", "f32", 75, Color.White)
        collision0x5.add("Y pos", "f32", 75, Color.White)
        collision0x5.add("Z pos", "f32", 75, Color.White)
        collision0x5.add("Rotation 1", "f32", 60, Color.White)
        collision0x5.add("Rotation 2", "f32", 60, Color.White)
        collision0x5.add("Rotation 3", "f32", 60, Color.White)
        collision0x5.add("x2C", "f32", 40, Color.LightGray)
        collision0x5.add("x30", "f32", 40, Color.LightGray)
        collision0x5.add("x34", "f32", 40, Color.LightGray)
        collision0x5.add("x38", "i32", 40, Color.LightGray)
        collision0x5.add("x3C", "i32", 40, Color.LightGray)
        collision0x5.add("x40", "i32", 40, Color.LightGray)
        collision0x5.add("x44", "i32", 40, Color.LightGray)
        collision0x5.add("x48", "i32", 40, Color.LightGray)
        collision0x5.add("x4C", "i32", 40, Color.LightGray)
        collision0x5.add("x50", "i32", 40, Color.LightGray)
        collision0x5.add("x54", "i32", 40, Color.LightGray)
        collision0x5.add("x58", "i32", 40, Color.LightGray)
        collision0x5.add("x5C", "i32", 40, Color.LightGray)
        collision0x5.add("x60", "i32", 40, Color.LightGray)
        collision0x5.setNameIndex(collision0x5.fieldCount)
        collision0x5.add("Name", "string", 100, Color.White)
        collision0x5.add("Sibpath", "string", 100, Color.White)
        collision0x5.add("Script ID", "i32", 60, Color.White)
        collision0x5.add("p+x04", "i8", 40, Color.LightGray)
        collision0x5.add("p+x05", "i8", 40, Color.LightGray)
        collision0x5.add("p+x06", "i8", 40, Color.LightGray)
        collision0x5.add("p+x07", "i8", 40, Color.LightGray)
        collision0x5.add("p+x08", "i8", 40, Color.LightGray)
        collision0x5.add("p+x09", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0A", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0B", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0C", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0D", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0E", "i8", 40, Color.LightGray)
        collision0x5.add("p+x0F", "i8", 40, Color.LightGray)
        collision0x5.add("p+x10", "i8", 40, Color.LightGray)
        collision0x5.add("p+x11", "i8", 40, Color.LightGray)
        collision0x5.add("p+x12", "i8", 40, Color.LightGray)
        collision0x5.add("p+x13", "i8", 40, Color.LightGray)
        collision0x5.add("p+x14", "i8", 40, Color.LightGray)
        collision0x5.add("p+x15", "i8", 40, Color.LightGray)
        collision0x5.add("p+x16", "i8", 40, Color.LightGray)
        collision0x5.add("p+x17", "i8", 40, Color.LightGray)
        collision0x5.add("p+x18", "i8", 40, Color.LightGray)
        collision0x5.add("p+x19", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1A", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1B", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1C", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1D", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1E", "i8", 40, Color.LightGray)
        collision0x5.add("p+x1F", "i8", 40, Color.LightGray)
        collision0x5.add("p+x20", "i32", 40, Color.LightGray)
        collision0x5.add("p+x24", "i32", 40, Color.LightGray)
        collision0x5.add("p+x28", "i32", 40, Color.LightGray)
        collision0x5.add("p+x2C", "i32", 60, Color.LightGray)
        collision0x5.add("p+x30", "i32", 60, Color.LightGray)
        collision0x5.add("p+x34", "i32", 60, Color.LightGray)
        collision0x5.add("p+x38", "i32", 60, Color.LightGray)
        collision0x5.add("p+x3C", "i16", 40, Color.LightGray)
        collision0x5.add("p+x3E", "i16", 40, Color.LightGray)
        collision0x5.add("p+x40", "i32", 60, Color.LightGray)
        collision0x5.add("p+x44", "i32", 40, Color.LightGray)
        collision0x5.add("p+x48", "i32", 40, Color.LightGray)
        collision0x5.add("p+x4C", "i32", 40, Color.LightGray)
        collision0x5.add("p+x50", "i32", 60, Color.LightGray)
        collision0x5.add("p+x54", "i16", 40, Color.LightGray)
        collision0x5.add("p+x56", "i16", 40, Color.LightGray)
        collision0x5.add("p+x58", "i32", 40, Color.LightGray)
        collision0x5.add("p+x5C", "i32", 40, Color.LightGray)
        collision0x5.add("p+x60", "i32", 40, Color.LightGray)
        collision0x5.add("p+x64", "i32", 40, Color.LightGray)
    End Sub
    Private Sub col0xBPrep()
        collision0xB.add("Name Offset", "i32", 40, Color.White)
        collision0xB.add("Type", "i32", 40, Color.White)
        collision0xB.add("Index", "i32", 40, Color.White)
        collision0xB.add("Model", "i32", 40, Color.White)
        collision0xB.add("x10", "i32", 40, Color.LightGray)
        collision0xB.add("X pos", "f32", 75, Color.White)
        collision0xB.add("Y pos", "f32", 75, Color.White)
        collision0xB.add("Z pos", "f32", 75, Color.White)
        collision0xB.add("Rotation 1", "f32", 60, Color.White)
        collision0xB.add("Rotation 2", "f32", 60, Color.White)
        collision0xB.add("Rotation 3", "f32", 60, Color.White)
        collision0xB.add("x2C", "f32", 40, Color.LightGray)
        collision0xB.add("x30", "f32", 40, Color.LightGray)
        collision0xB.add("x34", "f32", 40, Color.LightGray)
        collision0xB.add("x38", "i32", 40, Color.LightGray)
        collision0xB.add("x3C", "i32", 40, Color.LightGray)
        collision0xB.add("x40", "i32", 40, Color.LightGray)
        collision0xB.add("x44", "i32", 40, Color.LightGray)
        collision0xB.add("x48", "i32", 40, Color.LightGray)
        collision0xB.add("x4C", "i32", 40, Color.LightGray)
        collision0xB.add("x50", "i32", 40, Color.LightGray)
        collision0xB.add("x54", "i32", 40, Color.LightGray)
        collision0xB.add("x58", "i32", 40, Color.LightGray)
        collision0xB.add("x5C", "i32", 40, Color.LightGray)
        collision0xB.add("x60", "i32", 40, Color.LightGray)
        collision0xB.setNameIndex(collision0xB.fieldCount)
        collision0xB.add("Name", "string", 100, Color.White)
        collision0xB.add("Sibpath", "string", 100, Color.White)
        collision0xB.add("Script ID", "i32", 60, Color.White)
        collision0xB.add("p+x04", "i8", 40, Color.LightGray)
        collision0xB.add("p+x05", "i8", 40, Color.LightGray)
        collision0xB.add("p+x06", "i8", 40, Color.LightGray)
        collision0xB.add("p+x07", "i8", 40, Color.LightGray)
        collision0xB.add("p+x08", "i8", 40, Color.LightGray)
        collision0xB.add("p+x09", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0A", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0B", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0C", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0D", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0E", "i8", 40, Color.LightGray)
        collision0xB.add("p+x0F", "i8", 40, Color.LightGray)
        collision0xB.add("p+x10", "i8", 40, Color.LightGray)
        collision0xB.add("p+x11", "i8", 40, Color.LightGray)
        collision0xB.add("p+x12", "i8", 40, Color.LightGray)
        collision0xB.add("p+x13", "i8", 40, Color.LightGray)
        collision0xB.add("p+x14", "i8", 40, Color.LightGray)
        collision0xB.add("p+x15", "i8", 40, Color.LightGray)
        collision0xB.add("p+x16", "i8", 40, Color.LightGray)
        collision0xB.add("p+x17", "i8", 40, Color.LightGray)
        collision0xB.add("p+x18", "i8", 40, Color.LightGray)
        collision0xB.add("p+x19", "i8", 40, Color.LightGray)
        collision0xB.add("p+x1A", "i8", 40, Color.LightGray)
        collision0xB.add("p+x1B", "i8", 40, Color.LightGray)
        collision0xB.add("p+x1C", "i16", 40, Color.LightGray)
        collision0xB.add("p+x1E", "i16", 40, Color.LightGray)
        collision0xB.add("p+x20", "i32", 40, Color.LightGray)
        collision0xB.add("p+x24", "i32", 40, Color.LightGray)

    End Sub


    Private Sub unhPrep()
        unhandled.add("Name Offset", "i32", 40, Color.White)
        unhandled.add("Type", "i32", 40, Color.White)
        unhandled.add("Index", "i32", 40, Color.White)
        unhandled.add("Model", "i32", 40, Color.White)
        unhandled.setNameIndex(unhandled.fieldCount)
        unhandled.add("Name", "string", 100, Color.White)
        unhandled.add("Sibpath", "string", 100, Color.White)
    End Sub

    Private Sub frmMSBEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        mapPrep()

        crtPrep()

        mdlPrep()

        objPrep()

        col0x5Prep()

        col0xBPrep()

        unhPrep()
    End Sub

    Public Sub sizeChange() Handles MyBase.Resize
        tabParams.Width = MyBase.Width - 35
        tabParams.Height = MyBase.Height - 115

        dgvCreatures.Width = MyBase.Width - 55
        dgvCreatures.Height = MyBase.Height - 150

        dgvMapPieces.Width = MyBase.Width - 55
        dgvMapPieces.Height = MyBase.Height - 150

        dgvModels.Width = MyBase.Width - 55
        dgvModels.Height = MyBase.Height - 150

        dgvObjects.Width = MyBase.Width - 55
        dgvObjects.Height = MyBase.Height - 150

        dgvCollision0x5.Width = MyBase.Width - 55
        dgvCollision0x5.Height = MyBase.Height - 150

        dgvCollision0xB.Width = MyBase.Width - 55
        dgvCollision0xB.Height = MyBase.Height - 150

        dgvUnhandled.Width = MyBase.Width - 55
        dgvUnhandled.Height = MyBase.Height - 150
    End Sub


End Class

Public Class msbdata
    Private fieldNames As List(Of String) = New List(Of String)
    Private fieldtypes As List(Of String) = New List(Of String)
    Private fieldWidth As List(Of Integer) = New List(Of Integer)
    Private fieldBackColor As List(Of Color) = New List(Of Color)

    Private nameIdx As Integer

    Public Sub add(ByVal name As String, ByVal type As String, width As Integer, backColor As Color)
        fieldNames.Add(name)
        fieldtypes.Add(type)
        fieldWidth.Add(width)
        fieldBackColor.Add(backColor)
    End Sub
    Public Function retrieveName(ByVal i As Integer) As String
        Return fieldNames(i)
    End Function
    Public Function retrieveType(ByVal i As Integer) As String
        Return fieldtypes(i)
    End Function
    Public Function retrieveWidth(ByVal i As Integer) As Integer
        Return fieldWidth(i)
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