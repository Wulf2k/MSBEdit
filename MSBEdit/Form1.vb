Imports System.IO


'Creature type 0x4 is sized wrong

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





        dgvMapPieces0.Rows.Clear()
        dgvMapPieces0.Columns.Clear()

        For i = 0 To mapPieces0.fieldCount - 1
            dgvMapPieces0.Columns.Add(mapPieces0.retrieveName(i), mapPieces0.retrieveName(i))
            dgvMapPieces0.Columns(i).Width = mapPieces0.retrieveWidth(i)
            dgvMapPieces0.Columns(i).DefaultCellStyle.BackColor = mapPieces0.retrieveBackColor(i)
        Next

        dgvObjects1.Rows.Clear()
        dgvObjects1.Columns.Clear()

        For i = 0 To objects1.fieldCount - 1
            dgvObjects1.Columns.Add(objects1.retrieveName(i), objects1.retrieveName(i))
            dgvObjects1.Columns(i).Width = objects1.retrieveWidth(i)
            dgvObjects1.Columns(i).DefaultCellStyle.BackColor = objects1.retrieveBackColor(i)
        Next

        dgvCreatures2.Rows.Clear()
        dgvCreatures2.Columns.Clear()

        For i = 0 To creatures2.fieldCount - 1
            dgvCreatures2.Columns.Add(creatures2.retrieveName(i), creatures2.retrieveName(i))
            dgvCreatures2.Columns(i).Width = creatures2.retrieveWidth(i)
            dgvCreatures2.Columns(i).DefaultCellStyle.BackColor = creatures2.retrieveBackColor(i)
        Next

        dgvCreatures4.Rows.Clear()
        dgvCreatures4.Columns.Clear()

        For i = 0 To creatures4.fieldCount - 1
            dgvCreatures4.Columns.Add(creatures4.retrieveName(i), creatures4.retrieveName(i))
            dgvCreatures4.Columns(i).Width = creatures4.retrieveWidth(i)
            dgvCreatures4.Columns(i).DefaultCellStyle.BackColor = creatures4.retrieveBackColor(i)
        Next

        dgvCollision5.Rows.Clear()
        dgvCollision5.Columns.Clear()

        For i = 0 To collision5.fieldCount - 1
            dgvCollision5.Columns.Add(collision5.retrieveName(i), collision5.retrieveName(i))
            dgvCollision5.Columns(i).Width = collision5.retrieveWidth(i)
            dgvCollision5.Columns(i).DefaultCellStyle.BackColor = collision5.retrieveBackColor(i)
        Next

        dgvNavimesh8.Rows.Clear()
        dgvNavimesh8.Columns.Clear()

        For i = 0 To navimesh8.fieldCount - 1
            dgvNavimesh8.Columns.Add(collision5.retrieveName(i), navimesh8.retrieveName(i))
            dgvNavimesh8.Columns(i).Width = navimesh8.retrieveWidth(i)
            dgvNavimesh8.Columns(i).DefaultCellStyle.BackColor = navimesh8.retrieveBackColor(i)
        Next

        dgvObjects9.Rows.Clear()
        dgvObjects9.Columns.Clear()

        For i = 0 To objects1.fieldCount - 1
            dgvObjects9.Columns.Add(objects9.retrieveName(i), objects9.retrieveName(i))
            dgvObjects9.Columns(i).Width = objects9.retrieveWidth(i)
            dgvObjects9.Columns(i).DefaultCellStyle.BackColor = objects9.retrieveBackColor(i)
        Next

        dgvCreatures10.Rows.Clear()
        dgvCreatures10.Columns.Clear()

        For i = 0 To creatures10.fieldCount - 1
            dgvCreatures10.Columns.Add(creatures10.retrieveName(i), creatures10.retrieveName(i))
            dgvCreatures10.Columns(i).Width = creatures10.retrieveWidth(i)
            dgvCreatures10.Columns(i).DefaultCellStyle.BackColor = creatures10.retrieveBackColor(i)
        Next

        dgvCollision11.Rows.Clear()
        dgvCollision11.Columns.Clear()

        For i = 0 To collision11.fieldCount - 1
            dgvCollision11.Columns.Add(collision11.retrieveName(i), collision11.retrieveName(i))
            dgvCollision11.Columns(i).Width = collision11.retrieveWidth(i)
            dgvCollision11.Columns(i).DefaultCellStyle.BackColor = collision11.retrieveBackColor(i)
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
                Case &H0
                    Dim currOffset As Integer = 0
                    Dim mapRow(mapPieces0.fieldCount) As String
                    Dim mapName As String = ""
                    Dim mapSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)


                    mapRow(mapPieces0.getNameIndex) = name
                    mapRow(mapPieces0.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To mapPieces0.fieldCount - 1
                        If j < mapPieces0.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = mapPieces0.getNameIndex Then currOffset = 0

                        Select Case mapPieces0.retrieveType(j)
                            Case "i8"
                                mapRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                mapRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                mapRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvMapPieces0.Rows.Add(mapRow)



                Case &H1
                    Dim currOffset As Integer = 0
                    Dim objRow(objects1.fieldCount) As String
                    Dim objName As String = ""
                    Dim objSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    objRow(objects1.getNameIndex) = name
                    objRow(objects1.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To objects1.fieldCount - 1
                        If j < objects1.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = objects1.getNameIndex Then currOffset = 0

                        Select Case objects1.retrieveType(j)
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

                    dgvObjects1.Rows.Add(objRow)


                Case &H2
                    Dim currOffset As Integer = 0
                    Dim crtRow(creatures2.fieldCount) As String
                    Dim crtName As String = ""
                    Dim crtSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    crtRow(creatures2.getNameIndex) = name
                    crtRow(creatures2.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To creatures2.fieldCount - 1
                        If j < creatures2.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = creatures2.getNameIndex Then currOffset = 0

                        Select Case creatures2.retrieveType(j)
                            Case "i8"
                                crtRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                crtRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                crtRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCreatures2.Rows.Add(crtRow)


                Case &H4
                    Dim currOffset As Integer = 0
                    Dim crtRow(creatures4.fieldCount) As String
                    Dim crtName As String = ""
                    Dim crtSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    crtRow(creatures4.getNameIndex) = name
                    crtRow(creatures4.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To creatures4.fieldCount - 1
                        If j < creatures4.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = creatures4.getNameIndex Then currOffset = 0

                        Select Case creatures4.retrieveType(j)
                            Case "i8"
                                crtRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                crtRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                crtRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCreatures4.Rows.Add(crtRow)

                Case &H5
                    Dim currOffset As Integer = 0
                    Dim colRow(collision5.fieldCount) As String
                    Dim colName As String = ""
                    Dim colSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    colRow(collision5.getNameIndex) = name
                    colRow(collision5.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To collision5.fieldCount - 1
                        If j < collision5.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = collision5.getNameIndex Then currOffset = 0

                        Select Case collision5.retrieveType(j)
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

                    dgvCollision5.Rows.Add(colRow)


                Case &H8
                    Dim currOffset As Integer = 0
                    Dim naviRow(navimesh8.fieldCount) As String
                    Dim mnaviName As String = ""
                    Dim naviSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)


                    naviRow(navimesh8.getNameIndex) = name
                    naviRow(navimesh8.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To navimesh8.fieldCount - 1
                        If j < navimesh8.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = navimesh8.getNameIndex Then currOffset = 0

                        Select Case navimesh8.retrieveType(j)
                            Case "i8"
                                naviRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                naviRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                naviRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvNavimesh8.Rows.Add(naviRow)



                Case &H9
                    Dim currOffset As Integer = 0
                    Dim objRow(objects9.fieldCount) As String
                    Dim objName As String = ""
                    Dim objSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    objRow(objects9.getNameIndex) = name
                    objRow(objects9.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To objects9.fieldCount - 1
                        If j < objects9.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = objects9.getNameIndex Then currOffset = 0

                        Select Case objects9.retrieveType(j)
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

                    dgvObjects9.Rows.Add(objRow)


                Case &HA
                    Dim currOffset As Integer = 0
                    Dim crtRow(creatures10.fieldCount) As String
                    Dim crtName As String = ""
                    Dim crtSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    crtRow(creatures10.getNameIndex) = name
                    crtRow(creatures10.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To creatures10.fieldCount - 1
                        If j < creatures10.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = creatures10.getNameIndex Then currOffset = 0

                        Select Case creatures10.retrieveType(j)
                            Case "i8"
                                crtRow(j) = SIntFromOne(ptr + textboost + currOffset)
                                currOffset += 1
                            Case "i32"
                                crtRow(j) = SIntFromFour(ptr + textboost + currOffset)
                                currOffset += 4
                            Case "f32"
                                crtRow(j) = Math.Round(SingleFromFour(ptr + textboost + currOffset), 2)
                                currOffset += 4
                        End Select
                    Next

                    dgvCreatures10.Rows.Add(crtRow)





                Case &HB
                    Dim currOffset As Integer = 0
                    Dim colRow(collision11.fieldCount) As String
                    Dim colName As String = ""
                    Dim colSibpath As String = ""
                    Dim textboost As Integer

                    nameoffset = SIntFromFour(ptr)
                    name = StrFromBytes(ptr + nameoffset)
                    sibpath = StrFromBytes(ptr + nameoffset + name.Length + 1)

                    colRow(collision11.getNameIndex) = name
                    colRow(collision11.getNameIndex + 1) = sibpath

                    padding = ((sibpath.Length + name.Length + 5) And -&H4)
                    If padding <= &H10 Then
                        padding = &H10
                        If Not bigEndian Then padding += &H4
                    End If

                    For j = 0 To collision11.fieldCount - 1
                        If j < collision11.getNameIndex Then
                            textboost = 0
                        Else
                            textboost = nameoffset + padding
                        End If

                        If j = collision11.getNameIndex Then currOffset = 0

                        Select Case collision11.retrieveType(j)
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

                    dgvCollision11.Rows.Add(colRow)
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
    Private Sub saveDGV(ByRef MSBStream As FileStream, ByRef dgv As DataGridView, ByRef data As msbdata, ByRef partsPtr As Integer, ByRef curroffset As Integer, ByRef partsidx As Integer)
        For i = 0 To dgv.Rows.Count - 1

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
        partsidx += dgv.Rows.Count

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

        If Not File.Exists(txtMSBfile.Text & ".bak") Then
            File.WriteAllBytes(txtMSBfile.Text & ".bak", bytes)
        End If

        If File.Exists(txtMSBfile.Text) Then File.Delete(txtMSBfile.Text)
        Dim MSBStream As New IO.FileStream(txtMSBfile.Text, IO.FileMode.CreateNew)

        WriteBytes(MSBStream, UInt32ToFourByte(0))


        modelPtr = 0
        modelCnt = dgvModels.Rows.Count - 1
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




        partsCnt = dgvMapPieces0.Rows.Count + dgvObjects1.Rows.Count + dgvCreatures2.Rows.Count + dgvCreatures4.Rows.Count + dgvCollision5.Rows.Count + dgvNavimesh8.Rows.Count + dgvObjects9.Rows.Count + dgvCreatures10.Rows.Count + dgvCollision11.Rows.Count
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

        'File.WriteAllBytes(txtMSBfile.Text, bytes)
        MSBStream.Close()
        MsgBox("Save Complete.")
    End Sub


    Private Sub mdlPrep()
        With models
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("x0C", "i32", 40, Color.LightGray)
            .add("x10", "i32", 40, Color.LightGray)
            .add("x14", "i32", 40, Color.LightGray)
            .add("x18", "i32", 40, Color.LightGray)
            .add("x1C", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 400, Color.White)
        End With
    End Sub
    Private Sub map0Prep()
        With mapPieces0
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 75, Color.LightGray)
            .add("DrawGroup1", "i32", 75, Color.White)
            .add("DrawGroup2", "i32", 75, Color.White)
            .add("DrawGroup3", "i32", 75, Color.White)
            .add("DrawGroup4", "i32", 75, Color.White)
            .add("x4c", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 75, Color.LightGray)
            .add("p+x08", "i8", 75, Color.LightGray)
            .add("p+x09", "i8", 75, Color.LightGray)
            .add("p+x0A", "i8", 75, Color.LightGray)
            .add("p+x0B", "i8", 75, Color.LightGray)
            .add("p+x0C", "i32", 75, Color.LightGray)
            .add("p+x10", "i32", 75, Color.LightGray)
            .add("p+x14", "i32", 75, Color.LightGray)
            .add("p+x18", "i32", 75, Color.LightGray)
            .add("p+x1C", "i32", 75, Color.LightGray)
        End With
    End Sub
    Private Sub obj1Prep()
        With objects1
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 40, Color.LightGray)
            .add("p+x08", "i32", 60, Color.LightGray)
            .add("p+x0C", "i8", 40, Color.LightGray)
            .add("p+x0D", "i8", 40, Color.LightGray)
            .add("p+x0E", "i8", 40, Color.LightGray)
            .add("p+x0F", "i8", 40, Color.LightGray)
            .add("p+x10", "i8", 40, Color.LightGray)
            .add("p+x11", "i8", 40, Color.LightGray)
            .add("p+x12", "i8", 40, Color.LightGray)
            .add("p+x13", "i8", 40, Color.LightGray)
            .add("p+x14", "i32", 40, Color.LightGray)
            .add("p+x18", "i32", 40, Color.LightGray)
            .add("p+x1C", "i32", 40, Color.LightGray)
            .add("p+x20", "i32", 70, Color.LightGray)
            .add("p+x24", "i32", 60, Color.LightGray)
            .add("p+x28", "i32", 60, Color.White)
            .add("p+x2C", "i32", 60, Color.LightGray)
        End With
    End Sub
    Private Sub crt2Prep()
        With creatures2
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 75, Color.LightGray)
            .add("p+x08", "i32", 60, Color.LightGray)
            .add("p+x0C", "i32", 60, Color.LightGray)
            .add("p+x10", "i32", 40, Color.LightGray)
            .add("p+x14", "i32", 40, Color.LightGray)
            .add("p+x18", "i32", 40, Color.LightGray)
            .add("p+x1C", "i32", 40, Color.LightGray)
            .add("p+x20", "i32", 70, Color.LightGray)
            .add("p+x24", "i32", 60, Color.LightGray)
            .add("NPC ID", "i32", 60, Color.White)
            .add("p+x2C", "i32", 60, Color.LightGray)
            .add("p+x30", "i32", 60, Color.LightGray)
            .add("p+x34", "i32", 60, Color.LightGray)
            .add("p+x38", "i32", 60, Color.LightGray)
            .add("p+x3C", "i32", 75, Color.LightGray)
            .add("p+x40", "i32", 75, Color.LightGray)
            .add("p+x44", "i32", 75, Color.LightGray)
            .add("p+x48", "i32", 75, Color.LightGray)
            .add("p+x4C", "i32", 75, Color.LightGray)
            .add("AnimID", "i32", 75, Color.White)
            .add("p+x54", "i32", 75, Color.LightGray)
        End With
    End Sub
    Private Sub crt4Prep()
        With creatures4
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 75, Color.LightGray)
            .add("p+x08", "i32", 60, Color.LightGray)
            .add("p+x0C", "i32", 60, Color.LightGray)
            .add("p+x10", "i32", 40, Color.LightGray)
            .add("p+x14", "i32", 40, Color.LightGray)
            .add("p+x18", "i32", 40, Color.LightGray)
            .add("p+x1C", "i32", 40, Color.LightGray)
            .add("p+x20", "i32", 70, Color.LightGray)
            .add("p+x24", "i32", 60, Color.LightGray)
            '.add("NPC ID", "i32", 60, Color.White)
            '.add("p+x2C", "i32", 60, Color.LightGray)
        End With
    End Sub
    Private Sub coll5Prep()
        With collision5
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("p+x04", "i8", 40, Color.LightGray)
            .add("p+x05", "i8", 40, Color.LightGray)
            .add("p+x06", "i8", 40, Color.LightGray)
            .add("p+x07", "i8", 40, Color.LightGray)
            .add("p+x08", "i8", 40, Color.LightGray)
            .add("p+x09", "i8", 40, Color.LightGray)
            .add("p+x0A", "i8", 40, Color.LightGray)
            .add("p+x0B", "i8", 40, Color.LightGray)
            .add("p+x0C", "i8", 40, Color.LightGray)
            .add("p+x0D", "i8", 40, Color.LightGray)
            .add("p+x0E", "i8", 40, Color.LightGray)
            .add("p+x0F", "i8", 40, Color.LightGray)
            .add("p+x10", "i8", 40, Color.LightGray)
            .add("p+x11", "i8", 40, Color.LightGray)
            .add("p+x12", "i8", 40, Color.LightGray)
            .add("p+x13", "i8", 40, Color.LightGray)
            .add("p+x14", "i8", 40, Color.LightGray)
            .add("p+x15", "i8", 40, Color.LightGray)
            .add("p+x16", "i8", 40, Color.LightGray)
            .add("p+x17", "i8", 40, Color.LightGray)
            .add("p+x18", "i8", 40, Color.LightGray)
            .add("p+x19", "i8", 40, Color.LightGray)
            .add("p+x1A", "i8", 40, Color.LightGray)
            .add("p+x1B", "i8", 40, Color.LightGray)
            .add("p+x1C", "i8", 40, Color.LightGray)
            .add("p+x1D", "i8", 40, Color.LightGray)
            .add("p+x1E", "i8", 40, Color.LightGray)
            .add("p+x1F", "i8", 40, Color.LightGray)
            .add("p+x20", "i32", 40, Color.LightGray)
            .add("p+x24", "i32", 40, Color.LightGray)
            .add("p+x28", "i32", 40, Color.LightGray)
            .add("p+x2C", "i32", 60, Color.LightGray)
            .add("p+x30", "i32", 60, Color.LightGray)
            .add("p+x34", "i32", 60, Color.LightGray)
            .add("p+x38", "i32", 60, Color.LightGray)
            .add("p+x3C", "i16", 40, Color.LightGray)
            .add("p+x3E", "i16", 40, Color.LightGray)
            .add("p+x40", "i32", 60, Color.LightGray)
            .add("p+x44", "i32", 40, Color.LightGray)
            .add("p+x48", "i32", 40, Color.LightGray)
            .add("p+x4C", "i32", 40, Color.LightGray)
            .add("p+x50", "i32", 60, Color.LightGray)
            .add("p+x54", "i16", 40, Color.LightGray)
            .add("p+x56", "i16", 40, Color.LightGray)
            .add("p+x58", "i32", 40, Color.LightGray)
            .add("p+x5C", "i32", 40, Color.LightGray)
            .add("p+x60", "i32", 40, Color.LightGray)
            .add("p+x64", "i32", 40, Color.LightGray)
        End With
    End Sub
    Private Sub navi8Prep()
        With navimesh8
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 75, Color.LightGray)
            .add("DrawGroup1", "i32", 75, Color.White)
            .add("DrawGroup2", "i32", 75, Color.White)
            .add("DrawGroup3", "i32", 75, Color.White)
            .add("DrawGroup4", "i32", 75, Color.White)
            .add("x4c", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 75, Color.LightGray)
            .add("p+x08", "i8", 75, Color.LightGray)
            .add("p+x09", "i8", 75, Color.LightGray)
            .add("p+x0A", "i8", 75, Color.LightGray)
            .add("p+x0B", "i8", 75, Color.LightGray)
            .add("p+x0C", "i32", 75, Color.LightGray)
            .add("p+x10", "i32", 75, Color.LightGray)
            .add("p+x14", "i32", 75, Color.LightGray)
            .add("p+x18", "i32", 75, Color.LightGray)
            .add("p+x1C", "i32", 75, Color.LightGray)
            .add("p+x20", "i32", 75, Color.LightGray)
            .add("p+x24", "i32", 75, Color.LightGray)
            .add("p+x28", "i32", 75, Color.LightGray)
            .add("p+x2C", "i32", 75, Color.LightGray)
            .add("p+x30", "i32", 75, Color.LightGray)
            .add("p+x34", "i32", 75, Color.LightGray)
        End With
    End Sub
    Private Sub obj9Prep()
        With objects9
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("p+x04", "i8", 40, Color.LightGray)
            .add("p+x05", "i8", 40, Color.LightGray)
            .add("p+x06", "i8", 40, Color.LightGray)
            .add("p+x07", "i8", 40, Color.LightGray)
            .add("p+x08", "i32", 60, Color.LightGray)
            .add("p+x0C", "i8", 40, Color.LightGray)
            .add("p+x0D", "i8", 40, Color.LightGray)
            .add("p+x0E", "i8", 40, Color.LightGray)
            .add("p+x0F", "i8", 40, Color.LightGray)
            .add("p+x10", "i8", 40, Color.LightGray)
            .add("p+x11", "i8", 40, Color.LightGray)
            .add("p+x12", "i8", 40, Color.LightGray)
            .add("p+x13", "i8", 40, Color.LightGray)
            .add("p+x14", "i32", 40, Color.LightGray)
            .add("p+x18", "i32", 40, Color.LightGray)
            .add("p+x1C", "i32", 40, Color.LightGray)
            .add("p+x20", "i32", 70, Color.LightGray)
            .add("p+x24", "i32", 60, Color.LightGray)
            .add("p+x28", "i32", 60, Color.White)
            .add("p+x2C", "i32", 60, Color.LightGray)
        End With
    End Sub
    Private Sub crt10Prep()
        With creatures10
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("LightId", "i8", 75, Color.White)
            .add("FogId", "i8", 75, Color.White)
            .add("ScatId", "i8", 75, Color.White)
            .add("p+x07", "i8", 75, Color.LightGray)
            .add("p+x08", "i32", 60, Color.LightGray)
            .add("p+x0C", "i32", 60, Color.LightGray)
            .add("p+x10", "i32", 40, Color.LightGray)
            .add("p+x14", "i32", 40, Color.LightGray)
            .add("p+x18", "i32", 40, Color.LightGray)
            .add("p+x1C", "i32", 40, Color.LightGray)
            .add("p+x20", "i32", 70, Color.LightGray)
            .add("p+x24", "i32", 60, Color.LightGray)
            .add("NPC ID", "i32", 60, Color.White)
            .add("p+x2C", "i32", 60, Color.LightGray)
            .add("p+x30", "i32", 60, Color.LightGray)
            .add("p+x34", "i32", 60, Color.LightGray)
            .add("p+x38", "i32", 60, Color.LightGray)
            .add("p+x3C", "i32", 75, Color.LightGray)
            .add("p+x40", "i32", 75, Color.LightGray)
            .add("p+x44", "i32", 75, Color.LightGray)
            .add("p+x48", "i32", 75, Color.LightGray)
            .add("p+x4C", "i32", 75, Color.LightGray)
            .add("AnimID", "i32", 75, Color.White)
            .add("p+x54", "i32", 75, Color.LightGray)
        End With
    End Sub
    Private Sub coll11Prep()
        With collision11
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .add("x10", "i32", 40, Color.LightGray)
            .add("X pos", "f32", 75, Color.White)
            .add("Y pos", "f32", 75, Color.White)
            .add("Z pos", "f32", 75, Color.White)
            .add("Rotation 1", "f32", 60, Color.White)
            .add("Rotation 2", "f32", 60, Color.White)
            .add("Rotation 3", "f32", 60, Color.White)
            .add("x2C", "f32", 40, Color.LightGray)
            .add("x30", "f32", 40, Color.LightGray)
            .add("x34", "f32", 40, Color.LightGray)
            .add("x38", "i32", 40, Color.LightGray)
            .add("x3C", "i32", 40, Color.LightGray)
            .add("x40", "i32", 40, Color.LightGray)
            .add("x44", "i32", 40, Color.LightGray)
            .add("x48", "i32", 40, Color.LightGray)
            .add("x4C", "i32", 40, Color.LightGray)
            .add("x50", "i32", 40, Color.LightGray)
            .add("x54", "i32", 40, Color.LightGray)
            .add("x58", "i32", 40, Color.LightGray)
            .add("x5C", "i32", 40, Color.LightGray)
            .add("x60", "i32", 40, Color.LightGray)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
            .add("EventEntityID", "i32", 60, Color.White)
            .add("p+x04", "i8", 40, Color.LightGray)
            .add("p+x05", "i8", 40, Color.LightGray)
            .add("p+x06", "i8", 40, Color.LightGray)
            .add("p+x07", "i8", 40, Color.LightGray)
            .add("p+x08", "i8", 40, Color.LightGray)
            .add("p+x09", "i8", 40, Color.LightGray)
            .add("p+x0A", "i8", 40, Color.LightGray)
            .add("p+x0B", "i8", 40, Color.LightGray)
            .add("p+x0C", "i8", 40, Color.LightGray)
            .add("p+x0D", "i8", 40, Color.LightGray)
            .add("p+x0E", "i8", 40, Color.LightGray)
            .add("p+x0F", "i8", 40, Color.LightGray)
            .add("p+x10", "i8", 40, Color.LightGray)
            .add("p+x11", "i8", 40, Color.LightGray)
            .add("p+x12", "i8", 40, Color.LightGray)
            .add("p+x13", "i8", 40, Color.LightGray)
            .add("p+x14", "i8", 40, Color.LightGray)
            .add("p+x15", "i8", 40, Color.LightGray)
            .add("p+x16", "i8", 40, Color.LightGray)
            .add("p+x17", "i8", 40, Color.LightGray)
            .add("p+x18", "i8", 40, Color.LightGray)
            .add("p+x19", "i8", 40, Color.LightGray)
            .add("p+x1A", "i8", 40, Color.LightGray)
            .add("p+x1B", "i8", 40, Color.LightGray)
            .add("p+x1C", "i16", 40, Color.LightGray)
            .add("p+x1E", "i16", 40, Color.LightGray)
            .add("p+x20", "i32", 40, Color.LightGray)
            .add("p+x24", "i32", 40, Color.LightGray)
        End With
    End Sub
    Private Sub unhPrep()
        With unhandled
            .add("Name Offset", "i32", 40, Color.White)
            .add("Type", "i32", 40, Color.White)
            .add("Index", "i32", 40, Color.White)
            .add("Model", "i32", 40, Color.White)
            .setNameIndex(.fieldCount)
            .add("Name", "string", 100, Color.White)
            .add("Sibpath", "string", 100, Color.White)
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

        unhPrep()
    End Sub

    Public Sub sizeChange() Handles MyBase.Resize
        tabParams.Width = MyBase.Width - 35
        tabParams.Height = MyBase.Height - 115

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