Imports System.Drawing.Imaging
Imports System.Windows.Forms.AxHost

Public Structure PiecePos
    Dim X As SByte
    Dim Y As SByte

    Public Sub New(ByVal x As Integer, ByVal y As Integer)
        Me.X = x
        Me.Y = y
    End Sub
End Structure

Public Class Piece
    Protected Name As String
    Protected Index As SByte
    Protected Position As PiecePos
    Protected StartPosX As SByte
    Protected StartPosY As SByte

    Protected Dimensions As SByte
    Protected States(,,) As Boolean
    Protected NoBlocksPerRow(,) As SByte
    Protected CurrentState As SByte
    Protected DownEdgeInfo(,) As SByte
    Protected LeftEdgeInfo(,) As SByte
    Protected RightEdgeInfo(,) As SByte

    Protected TempState(,) As Boolean
    Protected TempEdgeInfo() As SByte
    Protected TempBlockCountInfo() As SByte


    Public Sub ConstructPiece()
        CalculateEdgeInfo()
        CurrentState = 0

        ReDim TempState(Dimensions, Dimensions)
        ReDim TempEdgeInfo(Dimensions)
        ReDim NoBlocksPerRow(3, Dimensions)
        ReDim TempBlockCountInfo(Dimensions)

        Dim LocalBlockCount As Integer
        For n = 0 To 3
            For y = 0 To Dimensions
                LocalBlockCount = 0
                For x = 0 To Dimensions
                    If States(n, y, x) Then LocalBlockCount += 1
                Next
                NoBlocksPerRow(n, y) = LocalBlockCount
            Next
        Next

        Position.X = StartPosX
        Position.Y = StartPosY
    End Sub

    Private Sub CalculateEdgeInfo()
        Dim IndexFinished(Dimensions) As Boolean

        'Calculates DownEdgeInfo.
        ReDim DownEdgeInfo(3, Dimensions)
        While CurrentState < 4
            For y = Dimensions To 0 Step -1
                For x = 0 To Dimensions
                    If Not IndexFinished(x) Then
                        If States(CurrentState, y, x) Then
                            IndexFinished(x) = True
                        Else
                            DownEdgeInfo(CurrentState, x) += 1
                        End If
                    End If
                Next
            Next
            For n = 0 To Dimensions
                If DownEdgeInfo(CurrentState, n) > Dimensions Then DownEdgeInfo(CurrentState, n) = -128 Else DownEdgeInfo(CurrentState, n) -= 1
            Next
            CurrentState += 1
            For n = 0 To Dimensions
                IndexFinished(n) = False
            Next
        End While
        CurrentState = 0

        'Calculates LeftEdgeInfo.
        ReDim LeftEdgeInfo(3, Dimensions)
        While CurrentState < 4
            For x = 0 To Dimensions
                For y = 0 To Dimensions
                    If Not IndexFinished(y) Then
                        If States(CurrentState, y, x) Then
                            IndexFinished(y) = True
                        Else
                            LeftEdgeInfo(CurrentState, y) += 1
                        End If
                    End If
                Next
            Next
            For n = 0 To Dimensions
                If LeftEdgeInfo(CurrentState, n) > Dimensions Then LeftEdgeInfo(CurrentState, n) = -128 Else LeftEdgeInfo(CurrentState, n) -= 1
            Next
            CurrentState += 1
            For n = 0 To Dimensions
                IndexFinished(n) = False
            Next
        End While
        CurrentState = 0

        'Calculates RightEdgeInfo.
        ReDim RightEdgeInfo(3, Dimensions)
        While CurrentState < 4
            For x = Dimensions To 0 Step -1
                For y = 0 To Dimensions
                    If Not IndexFinished(y) Then
                        If States(CurrentState, y, x) Then
                            IndexFinished(y) = True
                        Else
                            RightEdgeInfo(CurrentState, y) += 1
                        End If
                    End If
                Next
            Next
            For n = 0 To Dimensions
                If RightEdgeInfo(CurrentState, n) > Dimensions Then RightEdgeInfo(CurrentState, n) = -128 Else RightEdgeInfo(CurrentState, n) = Dimensions - RightEdgeInfo(CurrentState, n) + 1
            Next
            CurrentState += 1
            For n = 0 To Dimensions
                IndexFinished(n) = False
            Next
        End While
    End Sub

    Public Sub Reset()
        CurrentState = 0
        Position.X = StartPosX
        Position.Y = StartPosY
    End Sub


    Public Function GetName() As String
        Return Name
    End Function
    Public Function GetIndex() As SByte
        Return Index
    End Function

    Public Function GetDimensions() As SByte
        Return Dimensions
    End Function
    Public Function GetPosition() As PiecePos
        Return Position
    End Function
    Public Function GetRotation()
        Return CurrentState
    End Function

    Public Function GetCurrentState() As Boolean(,)
        For y = 0 To Dimensions
            For x = 0 To Dimensions
                TempState(y, x) = States(CurrentState, y, x)
            Next
        Next
        Return TempState
    End Function
    Public Function GetLRotateState() As Boolean(,)
        LRotate()
        GetCurrentState()
        RRotate()
        Return TempState
    End Function
    Public Function GetRRotateState() As Boolean(,)
        RRotate()
        GetCurrentState()
        LRotate()
        Return TempState
    End Function


    Public Function GetDownEdgeInfo() As SByte()
        For n = 0 To Dimensions
            TempEdgeInfo(n) = DownEdgeInfo(CurrentState, n)
        Next
        Return TempEdgeInfo
    End Function
    Public Function GetLeftEdgeInfo() As SByte()
        For n = 0 To Dimensions
            TempEdgeInfo(n) = LeftEdgeInfo(CurrentState, n)
        Next
        Return TempEdgeInfo
    End Function
    Public Function GetRightEdgeInfo() As SByte()
        For n = 0 To Dimensions
            TempEdgeInfo(n) = RightEdgeInfo(CurrentState, n)
        Next
        Return TempEdgeInfo
    End Function

    Public Function GetBlockCountInfo() As SByte()
        For n = 0 To Dimensions
            TempBlockCountInfo(n) = NoBlocksPerRow(CurrentState, n)
        Next
        Return TempBlockCountInfo
    End Function


    Public Sub Down()
        Position.Y -= 1
    End Sub
    Public Sub Left()
        Position.X -= 1
    End Sub
    Public Sub Right()
        Position.X += 1
    End Sub
    Public Sub LRotate()
        If CurrentState = 0 Then CurrentState = 3 Else CurrentState -= 1
    End Sub
    Public Sub RRotate()
        If CurrentState = 3 Then CurrentState = 0 Else CurrentState += 1
    End Sub

    Public Sub SetCoords(ByVal XPos As SByte, YPos As SByte, Rotation As SByte)
        Position.X = XPos
        Position.Y = YPos
        CurrentState = Rotation
    End Sub

End Class