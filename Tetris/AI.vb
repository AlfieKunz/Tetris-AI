Imports System.Collections.Generic
Imports System.Text.Json
Imports System.Windows.Forms.VisualStyles

Public Class TetrisAI
    ' Represents a single state in the simulation
    Private Class SearchNode
        Public X, Y, Rotation As SByte
        Public Move As Keys
        Public ParentNode As SearchNode 'Dynamic list approach towards parent, to form the path of moves
        Public Depth As Integer
    End Class

    Private PossibleActions As Keys() = {Keys.A, Keys.D, Keys.Left, Keys.Right, Keys.Down}
    Private SimPiece As Piece

    Private Board(,) As Boolean
    Private BaseHeightMap() As Integer
    Private BaseHoles As New List(Of PiecePos)
    Private BaseBlocksForLineClear() As Integer


    Public Sub InitHelperFunction(ByVal ListBoard As List(Of BoardRow), ByVal ColCount As Integer)
        ReDim Board(ListBoard.Count - 1, ColCount)
        ReDim BaseHeightMap(ColCount - 1)
        ReDim BaseBlocksForLineClear(ListBoard.Count - 1)
        BaseHoles.Clear()
        For y = ListBoard.Count - 1 To 0 Step -1
            For x = 0 To 11
                Board(y, x) = ListBoard(y).GetCell(x)
                If x = 0 OrElse x = 11 Then Continue For
                If Board(y, x) Then
                    If BaseHeightMap(x) = 0 Then BaseHeightMap(x) = y 'Record the highest block in the column
                Else
                    BaseBlocksForLineClear(y) += 1
                    If BaseHeightMap(x) > 0 Then BaseHoles.Add(New PiecePos(x, y)) 'There is a full cell above it - found a hole!
                End If
            Next
        Next
    End Sub

    Public Function GetBestPieceMoves(ByVal ActivePiece As Piece, ByRef BestScore As Integer) As (Moves As List(Of Keys), FinalPos As PiecePos)
        Dim MinScore As Integer = Integer.MaxValue
        Dim BestTerminalNode As SearchNode

        Dim Queue As New Queue(Of SearchNode)()
        Dim StartNode As New SearchNode 'Start from the initial starting position
        StartNode.X = ActivePiece.GetPosition().X
        StartNode.Y = ActivePiece.GetPosition().Y
        StartNode.Rotation = ActivePiece.GetRotation()
        Queue.Enqueue(StartNode)

        Select Case ActivePiece.GetIndex()
            Case 0 : SimPiece = New LinePiece()
            Case 1 : SimPiece = New LPiece()
            Case 2 : SimPiece = New ReverseLPiece()
            Case 3 : SimPiece = New SquarePiece()
            Case 4 : SimPiece = New SPiece()
            Case 5 : SimPiece = New ReverseSPiece()
            Case 6 : SimPiece = New TPiece()
            Case Else : SimPiece = New SquarePiece()
        End Select

        Dim VisitedPositions As New List(Of String) From {
            $"{StartNode.X},{StartNode.Y},{StartNode.Rotation}"
        }

        While Queue.Count > 0
            Dim CurrentNode As SearchNode = Queue.Dequeue()
            SimPiece.SetCoords(CurrentNode.X, CurrentNode.Y, CurrentNode.Rotation)
            If Not TestDown(CurrentNode) Then
                'Leaf node - piece has been placed, so we evaluate it! :)
                Dim Score As Integer = Evaluate(CurrentNode.Depth)
                'Stores the best score's move. 
                If Score < MinScore Then
                    MinScore = Score
                    BestTerminalNode = CurrentNode
                End If
            Else
                'Performs all possible moves on the piece, and adds them to the BFS tree.
                For Each MovementKey In PossibleActions
                    Dim NextNode As SearchNode = MakeMove(CurrentNode, MovementKey)
                    If NextNode IsNot Nothing Then
                        Dim NextNodeKey As String = $"{NextNode.X},{NextNode.Y},{NextNode.Rotation}"
                        'As we search moves via the BFS, the first time we reach a node is always by the
                        'shortest path. If we reach it again, it is from a longer path and therefore negligible.
                        If Not VisitedPositions.Contains(NextNodeKey) Then
                            VisitedPositions.Add(NextNodeKey)
                            Queue.Enqueue(NextNode)
                        End If
                    End If
                Next
            End If
        End While

        'Saves the final position (so we can check the GUI is placing things in the right place).
        Dim FinalPosition As PiecePos
        FinalPosition.X = StartNode.X
        FinalPosition.Y = StartNode.Y

        'Backtracks the tree of nodes to reconstruct the optimal path.
        Dim BestMoves As New List(Of Keys)()
        If BestTerminalNode IsNot Nothing Then
            FinalPosition.X = BestTerminalNode.X
            FinalPosition.Y = BestTerminalNode.Y

            ' Walk backwards up the tree until we hit the start node
            While BestTerminalNode.ParentNode IsNot Nothing
                BestMoves.Add(BestTerminalNode.Move)
                BestTerminalNode = BestTerminalNode.ParentNode
            End While
            BestMoves.Reverse()
            'Replaces any final string of continuous downs with a hard drop. As PossibleActions begins with rotations
            'and ends with drops, and BFS won't choose later paths unless they're strictly better (which typically means
            'fewer moves, hence more likely of drop chances), we are more likely to get a string of downs at the end.
            If BestMoves.Count > 0 AndAlso BestMoves(BestMoves.Count - 1) = Keys.Down Then
                While BestMoves.Count > 0 AndAlso BestMoves(BestMoves.Count - 1) = Keys.Down
                    BestMoves.RemoveAt(BestMoves.Count - 1)
                End While
                BestMoves.Add(Keys.Space)
            End If
        End If

        BestScore = MinScore
        Return (BestMoves, FinalPosition)
    End Function



    Private Function MakeMove(ByVal Node As SearchNode, ByVal MovementKey As Keys) As SearchNode
        Dim MoveIsValid As Boolean
        Select Case MovementKey
            'Skips moves if they are the reverse of the previous move.
            Case Keys.Left : MoveIsValid = Node.Move <> Keys.Right AndAlso TestLeft(SimPiece)
            Case Keys.Right : MoveIsValid = Node.Move <> Keys.Left AndAlso TestRight(SimPiece)
            Case Keys.Down : MoveIsValid = TestDown(SimPiece, Node.X, Node.Y)
            Case Keys.A
                'Also stops rotation if we are rotating the same direction 3x in a row (just rotate the opposite way)
                Dim ParentsExist As Boolean = Node.ParentNode IsNot Nothing AndAlso Node.ParentNode.ParentNode IsNot Nothing
                Dim Repetition As Boolean = Node.Move = Keys.D OrElse (ParentsExist AndAlso Node.ParentNode.Move = Keys.A AndAlso Node.ParentNode.ParentNode.Move = Keys.A)
                MoveIsValid = Not Repetition AndAlso TestLRotate(SimPiece)
            Case Keys.D
                Dim ParentsExist As Boolean = Node.ParentNode IsNot Nothing AndAlso Node.ParentNode.ParentNode IsNot Nothing
                Dim Repetition As Boolean = Node.Move = Keys.A OrElse (ParentsExist AndAlso Node.ParentNode.ParentNode IsNot Nothing AndAlso Node.ParentNode.Move = Keys.D AndAlso Node.ParentNode.ParentNode.Move = Keys.D)
                MoveIsValid = Not Repetition AndAlso TestRRotate(SimPiece)
        End Select
        If Not MoveIsValid Then Return Nothing

        Dim NextNode As New SearchNode With {
            .X = Node.X,
            .Y = Node.Y,
            .Rotation = Node.Rotation,
            .Move = MovementKey,
            .ParentNode = Node,
            .Depth = Node.Depth + 1
        }

        Select Case MovementKey
            Case Keys.Left : NextNode.X -= 1
            Case Keys.Right : NextNode.X += 1
            Case Keys.Down : NextNode.Y -= 1
            Case Keys.A : NextNode.Rotation = (NextNode.Rotation + 3) Mod 4
            Case Keys.D : NextNode.Rotation = (NextNode.Rotation + 1) Mod 4
        End Select
        Return NextNode
    End Function

    'Copied from Tetris.vb's TestDown, TestLeft, TestRight, etc etc.
    Private Function TestDown(ByVal SimPiece As Piece, ByVal XPos As SByte, ByVal YPos As SByte) As Boolean
        Dim TempEdgeInfo() As SByte = SimPiece.GetDownEdgeInfo()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(YPos + TempEdgeInfo(n), XPos + n) Then Return False
            End If
        Next
        Return True
    End Function
    Private Function TestDown(ByVal Node As SearchNode) As Boolean
        Return TestDown(SimPiece, Node.X, Node.Y)
    End Function

    Private Function TestLeft(ByVal SimPiece As Piece) As Boolean
        Dim TempEdgeInfo() As SByte = SimPiece.GetLeftEdgeInfo()
        Dim PieceLocation As PiecePos = SimPiece.GetPosition()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(PieceLocation.Y + SimPiece.GetDimensions() - n, PieceLocation.X + TempEdgeInfo(n)) Then Return False
            End If
        Next
        Return True
    End Function

    Private Function TestRight(ByVal SimPiece As Piece) As Boolean
        Dim TempEdgeInfo() As SByte = SimPiece.GetRightEdgeInfo()
        Dim PieceLocation As PiecePos = SimPiece.GetPosition()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(PieceLocation.Y + SimPiece.GetDimensions() - n, PieceLocation.X + TempEdgeInfo(n)) Then Return False
            End If
        Next
        Return True
    End Function

    Private Function TestLRotate(ByVal SimPiece As Piece) As Boolean
        If SimPiece.GetName() = "Square" Then Return False
        Dim PieceLocation As PiecePos = SimPiece.GetPosition()
        Dim PieceDimensions As SByte = SimPiece.GetDimensions()
        If PieceLocation.Y <= 0 OrElse PieceLocation.Y + PieceDimensions > 20 Then Return False
        Dim TempState(,) As Boolean = SimPiece.GetLRotateState()
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If TempState(PieceDimensions - y, x) Then
                    Dim TempX As Integer = PieceLocation.X + x
                    If TempX < 0 OrElse TempX > 11 OrElse Board(PieceLocation.Y + y, TempX) Then Return False
                End If
            Next
        Next
        Return True
    End Function

    Private Function TestRRotate(ByVal SimPiece As Piece) As Boolean
        If SimPiece.GetName() = "Square" Then Return False
        Dim PieceLocation As PiecePos = SimPiece.GetPosition()
        Dim PieceDimensions As SByte = SimPiece.GetDimensions()
        If PieceLocation.Y <= 0 OrElse PieceLocation.Y + PieceDimensions > 20 Then Return False
        Dim TempState(,) As Boolean = SimPiece.GetRRotateState()
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If TempState(PieceDimensions - y, x) Then
                    Dim TempX As Integer = PieceLocation.X + x
                    If TempX < 0 OrElse TempX > 11 OrElse Board(PieceLocation.Y + y, TempX) Then Return False
                End If
            Next
        Next
        Return True
    End Function




    'Evaluates the leaf nodes of the BFS, by adding the TempPiece to the board temporarily, and applying heuristics.
    'Set up so that a lower score is good! Also so that shorter paths are prioritised.
    Private Function Evaluate(ByVal NodeDepth As Integer) As Integer
        Dim PenaltyScore As Integer = 0
        Dim PieceState As Boolean(,) = SimPiece.GetCurrentState
        Dim PiecePosition As PiecePos = SimPiece.GetPosition()
        Dim PieceDimensions As SByte = SimPiece.GetDimensions()
        Dim PieceDownInfo As SByte() = SimPiece.GetDownEdgeInfo()

        'Makes a copy of the height map, and adjusts it via the new piece placement.
        Dim HeightMap(BaseHeightMap.Length - 1) As Integer
        Dim CorrectedValues(PieceDimensions) As Boolean
        Array.Copy(BaseHeightMap, HeightMap, BaseHeightMap.Length)
        For y = PieceDimensions To 0 Step -1
            For x = 0 To PieceDimensions
                If CorrectedValues(x) Then Continue For
                Dim BoardPosX As Integer = PiecePosition.X + x
                Dim BoardPosY As Integer = PiecePosition.Y + y
                If PieceState(PieceDimensions - y, x) Then
                    HeightMap(BoardPosX) = Math.Max(HeightMap(BoardPosX), BoardPosY)
                    'Don't need to check this x value again, as we are going in descending order of y.
                    CorrectedValues(x) = True
                End If
            Next
        Next
        Dim MaxHeightMap As Double = HeightMap.Max

        Dim HoleCount As Decimal = 0
        Dim HoleHeightScalingFactor As Decimal = 1.5
        Dim OpenHoleScalingFactor As Decimal = 0.5
        Dim BlockedHole As Boolean
        ReDim CorrectedValues(PieceDimensions)
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If CorrectedValues(x) Then Continue For
                Dim BoardPosX As Integer = PiecePosition.X + x
                Dim BoardPosY As Integer = PiecePosition.Y + y
                If PieceState(PieceDimensions - y, x) Then
                    If BoardPosY > 0 AndAlso Not Board(BoardPosY - 1, BoardPosX) Then
                        'We are creating a hole: calculate how many holes we create.
                        HoleCount += 1
                        'Holes are penalised more depending on how far they are from the placed piece.
                        For HoleYRay = BoardPosY - 2 To 0 Step -1
                            If Board(HoleYRay, BoardPosX) Then
                                Exit For
                            Else
                                'Calculates if the hole is closed or open. If it is open, apply less of a penalty.
                                Dim HoleBasePenalty As Decimal = HoleHeightScalingFactor * Math.Min((BoardPosY - HoleYRay) / 2, 4)
                                If Not (Board(HoleYRay, BoardPosX - 1) AndAlso Board(HoleYRay, BoardPosX + 1)) Then HoleBasePenalty *= OpenHoleScalingFactor
                                HoleCount += HoleBasePenalty
                            End If
                        Next
                        If BoardPosX = 10 Then BlockedHole = True
                    End If
                    'Don't need to check this x value again, as we are going in ascending order of y.
                    CorrectedValues(x) = True
                End If
            Next
        Next
        For Each Hole In BaseHoles
            If PiecePosition.X <= Hole.X AndAlso Hole.X <= PiecePosition.X + PieceDimensions Then
                ' Calculates if we are placing the piece directly over an existing hole. If so, apply a small penalty.
                If PieceDownInfo(Hole.X - PiecePosition.X) > -128 Then
                    HoleCount += OpenHoleScalingFactor * OpenHoleScalingFactor * HoleHeightScalingFactor * Math.Min(MaxHeightMap - Hole.Y, 4)
                End If
            End If
        Next

        'Calculates how many line clears the piece produces.
        Dim LinesCleared As Integer = 0
        Dim CanClearHole As Boolean = True
        For y = 0 To PieceDimensions
            Dim CheckY As Integer = PiecePosition.Y + y
            If CheckY >= 0 AndAlso CheckY < BaseBlocksForLineClear.Length Then
                If BaseBlocksForLineClear(CheckY) = SimPiece.GetBlockCountInfo(PieceDimensions - y) Then
                    LinesCleared += 1
                ElseIf BlockedHole AndAlso CanClearHole Then
                    'Checks if the line clearning stops the hole from being blocked.
                    If PieceState(PieceDimensions - y, PieceState.GetLength(1) - 1) AndAlso MaxHeightMap < 15 Then CanClearHole = False
                End If
            End If
        Next
        If CanClearHole Then BlockedHole = False

        'Calculates Bumpiness and Total Height.
        Dim Bumpiness As Integer = 0
        Dim ExtraPillerCount As Integer = 0
        For x = 1 To 9
            Dim LocalBump As Integer = HeightMap(x) - HeightMap(x + 1)
            If x < 9 Then
                Bumpiness += Math.Abs(LocalBump)
                'Incentivices only 1 large piller.
                If Math.Abs(LocalBump) > 4 Then ExtraPillerCount += 1
            ElseIf LocalBump >= 0 AndAlso MaxHeightMap <= 12 Then
                'Column 10 is level with or lower than column 9 - this is the intended "well" shape, so reward it (capped).
                PenaltyScore -= Math.Min(LocalBump, 5) * 15
            Else
                'Inhibits unwanted spikes in the well.
                Bumpiness += Math.Abs(LocalBump) * If(MaxHeightMap > 14, 2, 1)
                If Math.Abs(LocalBump) > 4 Then ExtraPillerCount += If(MaxHeightMap > 14, 2, 1)
            End If
        Next
        Dim HeightMapPenalty = If(MaxHeightMap <= 10, MaxHeightMap, 0.4 * MaxHeightMap * MaxHeightMap - 7 * MaxHeightMap + 40)
        PenaltyScore += (40 * HeightMapPenalty) + (8 * HeightMap.Sum) + (7 * PiecePosition.Y)
        PenaltyScore += CInt(100 * HoleCount) + (15 * Bumpiness) + (100 * ExtraPillerCount)
        If LinesCleared > 0 Then PenaltyScore -= If(MaxHeightMap <= 12, 45 * LinesCleared ^ 3, 120 * LinesCleared ^ 2)
        PenaltyScore += If(BlockedHole, 500, 0)

        'Prioritises the shortest path.
        Return PenaltyScore * 1000 + NodeDepth
    End Function

End Class