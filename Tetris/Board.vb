Public Class BoardRow

    Private Row(11) As Boolean
    'Private Colours(9) As Color

    Public Sub New()
        Row(0) = True
        Row(11) = True
    End Sub

    Public Sub Fill()
        For n = 1 To 10
            Row(n) = True
        Next
    End Sub
    Public Sub Clear()
        For n = 1 To 10
            Row(n) = False
        Next
    End Sub

    Public Sub SetCell(ByVal Index As SByte)
        Row(Index) = True
    End Sub
    Public Sub ClearCell(ByVal Index As SByte)
        Row(Index) = false
    End Sub

    Public Function GetCell(ByVal Index As SByte) As Boolean
        Return Row(Index)
    End Function


    'Public Sub SetCell(ByVal Index As SByte, ByVal Colour As Color)
    '    Row(Index) = True
    '    Colours(Index) = Colour
    'End Sub
    'Public Function GetCellColour(ByVal Index As SByte) As Color
    '    Return Colours(Index)
    'End Function


    Public Function CheckIfFull()
        Return Row(1) AndAlso Row(2) AndAlso Row(3) AndAlso Row(4) AndAlso Row(5) AndAlso Row(6) AndAlso Row(7) AndAlso Row(8) AndAlso Row(9) AndAlso Row(10)
    End Function

End Class
