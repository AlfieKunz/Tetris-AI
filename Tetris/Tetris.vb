Imports System.Diagnostics.Metrics
Imports System.Reflection.Metadata.Ecma335
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button

Public Class Tetris

    Private GameRunning As Boolean
    Private Board As New List(Of BoardRow) '10x20 board (excluding buffers)

    Private PieceBag As New List(Of SByte)
    Private PieceTemplateArray(6) As Piece
    Private PieceImageArray(6) As Image
    Private PieceGUIImageArray(6) As Image

    Private GameClock As Timer
    Private TimerEnabled As Boolean
    Private PlayerScore As UInt64
    Private NoLinesPushedDown As Byte

    Private CurrentPiece As Piece
    Private CanHoldPiece As Boolean
    Private HeldIndex As SByte = -1
    Private TickConstant As Integer = 500


    Private CurrentPiecePictureBoxes(3) As PictureBox
    Private PredictedPiecePictureBoxes(3) As PictureBox
    Private BoardPiecePictureBoxes As New List(Of PictureBox)
    Private Const DisplayElementMultiplier As Decimal = 34

    Private LBluePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\LBlue.png")
    Private RedPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Red.png")
    Private GreenPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Green.png")
    Private YellowPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Yellow.png")
    Private PurplePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Purple.png")
    Private DBluePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\DBlue.png")
    Private OrangePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Orange.png")
    Private BlankPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\Blank.png")

    Private LinePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\LinePiece.png")
    Private LPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\LPiece.png")
    Private ReverseLPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\ReverseLPiece.png")
    Private SquarePieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\SquarePiece.png")
    Private SPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\SPiece.png")
    Private ReverseSPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\ReverseSPiece.png")
    Private TPieceImage As Image = Image.FromFile(Application.StartupPath & "\Assets\TPiece.png")

    Private AI As New TetrisAI
    Dim AITimer As New Stopwatch
    Private AIMode As Boolean
    Private AIMovesQueue As New Queue(Of Keys)
    Private AIMoveInverval As Integer = 50
    Private AIScoreToBeatHeldMove As Integer = 0




    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.OutputEncoding = System.Text.Encoding.UTF8
        Application.EnableVisualStyles()
        Me.Size = New Size(598, 770)
        DoubleBuffered = True
        GameBoard.SendToBack()
        GameClock = New Timer(New TimerCallback(AddressOf GameTick), Nothing, TickConstant, TickConstant)

        ConstructBoard()
        PopulateBag()
        ConstructPieces()
        CreateRndPiece()

        DisplayBoardConsole()

        CanHoldPiece = True
        TimerEnabled = True
        GameRunning = True
    End Sub

    Private Sub GameTick()
        If TimerEnabled Then
            Me.Invoke(Sub()
                          If AIMode AndAlso AIMovesQueue.Count > 0 Then
                              Dim NextMove As Keys = AIMovesQueue.Dequeue()
                              Tetris_KeyPress(Nothing, New KeyEventArgs(NextMove))
                          Else
                              If TestDown() Then
                                  CurrentPiece.Down()
                                  DisplayPieceDown()
                              Else
                                  FixDisplayPiece()
                                  FixPiece(True)
                                  CreateRndPiece()
                              End If
                          End If

                          DisplayBoardConsole()
                      End Sub)
        End If
    End Sub

    Private Sub CreateRndPiece()

        If PieceBag.Count <= 7 Then PopulateBag()
        CurrentPiece = PieceTemplateArray(PieceBag(0))
        CurrentPiece.Reset()

        For n = 0 To 3
            CurrentPiecePictureBoxes(n).Image = PieceImageArray(PieceBag(0))
        Next
        PieceBag.RemoveAt(0)
        RefreshPiece()
        ModifyNextPieces()

        If TestDown() Then
            PositionPredictedPiece()
            GameClock.Change(TickConstant, TickConstant)
            InitAIThread()
        Else
            For n = 0 To 3
                PredictedPiecePictureBoxes(n).Visible = False
            Next
            GameClock.Dispose()
            GameRunning = False
            GameOverLabel.Visible = True
        End If
    End Sub

    Private Sub InitAIThread()
        If AIMode Then
            Dim AIThread As Task = New Task(AddressOf Me.RunAI)
            AIThread.Start()
            While Not AIThread.IsCompleted
                Thread.Sleep(1) 'Allows more processing time to be spent on the AIs.
                Application.DoEvents() 'Allows the user to interact with the GUI (limited).
            End While
            Console.WriteLine($"AI Searched all Moves in {AITimer.Elapsed.TotalMilliseconds}ms.")
            GameClock.Change(0, AIMoveInverval)
        End If
    End Sub

    Private Sub RunAI()
        AITimer.Restart()
        AI.InitHelperFunction(Board, 11)
        'Instantly finds the best set of moves for the newly-spawned piece.
        Dim CurrentScore As Integer
        Dim CurrentMoves As List(Of Keys) = AI.GetBestPieceMoves(CurrentPiece, CurrentScore)
        AIMovesQueue.Clear()
        'Can we get a better score from the currently held piece?
        If HeldIndex = -1 Then
            AIMovesQueue.Enqueue(Keys.ShiftKey) 'Always holds the first piece.
        ElseIf CanHoldPiece Then
            Dim HoldScore As Integer
            Dim HoldMoves As List(Of Keys) = AI.GetBestPieceMoves(PieceTemplateArray(HeldIndex), HoldScore)
            'Uses the held piece if we get a better score from it.
            If HoldScore + AIScoreToBeatHeldMove < CurrentScore Then
                CurrentMoves = HoldMoves
                AIMovesQueue.Enqueue(Keys.ShiftKey)
            End If
        End If
        For Each MovementKey In CurrentMoves
            AIMovesQueue.Enqueue(MovementKey)
        Next
        AITimer.Stop()
    End Sub

    Private Sub RefreshPiece()
        Dim PieceState As Boolean(,) = CurrentPiece.GetCurrentState()
        Dim PiecePosition As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions()
        Dim PieceIndex As SByte = 0
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If PieceState(y, x) Then
                    CurrentPiecePictureBoxes(PieceIndex).Location = New Point((x + PiecePosition.X - 1) * DisplayElementMultiplier, (20 - ((PieceDimensions - y) + PiecePosition.Y)) * DisplayElementMultiplier + 1)
                    PieceIndex += 1
                    If PieceIndex = 4 Then Exit For
                End If
            Next
            If PieceIndex = 4 Then Exit For
        Next

        GameBoard.Refresh()
    End Sub

    Private Sub PositionPredictedPiece()
        For n = 0 To 3
            PredictedPiecePictureBoxes(n).Location = CurrentPiecePictureBoxes(n).Location
            If Not PredictedPiecePictureBoxes(n).Visible Then PredictedPiecePictureBoxes(n).Visible = True
        Next
        Dim MaxMoves As Byte = CalculateMaxNoOfDowns()
        For Each Box In PredictedPiecePictureBoxes
            Box.Top += MaxMoves * DisplayElementMultiplier
        Next
    End Sub

    Private Sub ModifyNextPieces()
        NextBox1.Image = PieceGUIImageArray(PieceBag(0))
        NextBox2.Image = PieceGUIImageArray(PieceBag(1))
        NextBox3.Image = PieceGUIImageArray(PieceBag(2))
    End Sub



    Private Sub ConstructBoard()
        Dim TempBoardRow As New BoardRow()
        TempBoardRow.Fill()
        Board.Add(TempBoardRow)
        For n = 1 To 20
            TempBoardRow = New BoardRow()
            Board.Add(TempBoardRow)
        Next
    End Sub

    Private Sub ConstructPieces()
        PieceTemplateArray(0) = New LinePiece()
        PieceTemplateArray(1) = New LPiece()
        PieceTemplateArray(2) = New ReverseLPiece()
        PieceTemplateArray(3) = New SquarePiece()
        PieceTemplateArray(4) = New SPiece()
        PieceTemplateArray(5) = New ReverseSPiece()
        PieceTemplateArray(6) = New TPiece()

        PieceImageArray(0) = LBluePieceImage
        PieceImageArray(1) = DBluePieceImage
        PieceImageArray(2) = OrangePieceImage
        PieceImageArray(3) = YellowPieceImage
        PieceImageArray(4) = GreenPieceImage
        PieceImageArray(5) = RedPieceImage
        PieceImageArray(6) = PurplePieceImage

        PieceGUIImageArray(0) = LinePieceImage
        PieceGUIImageArray(1) = LPieceImage
        PieceGUIImageArray(2) = ReverseLPieceImage
        PieceGUIImageArray(3) = SquarePieceImage
        PieceGUIImageArray(4) = SPieceImage
        PieceGUIImageArray(5) = ReverseSPieceImage
        PieceGUIImageArray(6) = TPieceImage

        For n = 0 To 3
            CurrentPiecePictureBoxes(n) = New PictureBox()
            GameBoard.Controls.Add(CurrentPiecePictureBoxes(n))
            CurrentPiecePictureBoxes(n).Size = New Size(DisplayElementMultiplier + 2, DisplayElementMultiplier + 2)
            CurrentPiecePictureBoxes(n).BringToFront()

            PredictedPiecePictureBoxes(n) = New PictureBox()
            GameBoard.Controls.Add(PredictedPiecePictureBoxes(n))
            PredictedPiecePictureBoxes(n).Size = New Size(DisplayElementMultiplier + 2, DisplayElementMultiplier + 2)
            PredictedPiecePictureBoxes(n).Image = BlankPieceImage
        Next
    End Sub

    Private Sub PopulateBag()
        Static RNDGen As New Random()
        Dim TempBag As New List(Of Integer)
        For n = 0 To 6
            TempBag.Add(n)
        Next

        'Shuffles the list using the Fisher-Yates Algorithm.
        Dim Index As SByte
        Dim TempItem As SByte
        For n = TempBag.Count - 1 To 1 Step -1
            Index = RNDGen.Next(n + 1)
            TempItem = TempBag(n)
            TempBag(n) = TempBag(Index)
            TempBag(Index) = TempItem
        Next

        'Adds the contents of this temporary bag to the main bag.
        For Each Element In TempBag
            PieceBag.Add(Element)
        Next
    End Sub




    Private Sub DisplayBoardConsole()
        Dim PieceState As Boolean(,) = CurrentPiece.GetCurrentState()
        Dim PiecePosition As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions() + 1
        Dim PieceMayBeOnCurrentRow As Boolean
        Console.WriteLine("Board Output:")
        Console.WriteLine(" __________ ")
        For y = 20 To 1 Step -1
            If y >= PiecePosition.Y AndAlso y < (PiecePosition.Y + PieceDimensions) Then PieceMayBeOnCurrentRow = True Else PieceMayBeOnCurrentRow = False
            Console.Write("|")
            For x = 1 To 10
                If Board(y).GetCell(x) Then
                    Console.Write(ChrW(&H25AB))
                ElseIf PieceMayBeOnCurrentRow AndAlso (x >= PiecePosition.X AndAlso x < (PiecePosition.X + PieceDimensions)) Then
                    'If x = PiecePosition.X AndAlso y = PiecePosition.Y Then
                    '    Console.Write("x")
                    If PieceState(PieceDimensions - 1 - (y - PiecePosition.Y), x - PiecePosition.X) Then
                        Console.Write(ChrW(&H25AB))
                    Else
                        Console.Write(ChrW(&HB7))
                    End If
                Else
                    Console.Write(ChrW(&HB7))
                End If
            Next
            Console.WriteLine("|")
        Next
        Console.WriteLine(" ---------- ")
        Console.CursorTop = Math.Max(0, Console.CursorTop - 23)
    End Sub




    Private Sub Tetris_KeyPress(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If GameRunning Then
            TimerEnabled = False
            Select Case e.KeyCode
                Case Keys.Down

                    If TestDown() Then
                        NoLinesPushedDown += 1
                        CurrentPiece.Down()
                        DisplayPieceDown()
                    Else
                        FixDisplayPiece()
                        FixPiece(True)
                        CreateRndPiece()
                    End If
                    If GameRunning AndAlso Not AIMode Then GameClock.Change(TickConstant, TickConstant)

                Case Keys.Left
                    If TestLeft() Then CurrentPiece.Left() : DisplayPieceLeft() : PositionPredictedPiece()
                Case Keys.Right
                    If TestRight() Then CurrentPiece.Right() : DisplayPieceRight() : PositionPredictedPiece()
                Case Keys.A
                    If TestLRotate() Then CurrentPiece.LRotate() : RefreshPiece() : PositionPredictedPiece()
                Case Keys.D, Keys.Up
                    If TestRRotate() Then CurrentPiece.RRotate() : RefreshPiece() : PositionPredictedPiece()
                Case Keys.Space
                    'Stops visual tearing for lots of instant down movements.
                    For n = 0 To 3
                        PredictedPiecePictureBoxes(n).Visible = False
                    Next
                    Dim NoOfMoves As Byte = CalculateMaxNoOfDowns()
                    For n = 1 To NoOfMoves
                        CurrentPiece.Down()
                        NoLinesPushedDown += 2
                    Next
                    For Each Box In CurrentPiecePictureBoxes
                        Box.Top += DisplayElementMultiplier * NoOfMoves
                    Next
                    FixDisplayPiece()
                    FixPiece(True)
                    CreateRndPiece()
                    If GameRunning AndAlso Not AIMode Then GameClock.Change(TickConstant, TickConstant)
                Case Keys.ShiftKey
                    HoldPiece(True)
            End Select
            TimerEnabled = True
            DisplayBoardConsole()
        End If
    End Sub


    Private Function TestDown() As Boolean
        Dim TempEdgeInfo() As SByte = CurrentPiece.GetDownEdgeInfo()
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(PieceLocation.Y + TempEdgeInfo(n)).GetCell(PieceLocation.X + n) Then Return False
            End If
        Next
        Return True
    End Function

    Private Function TestLeft() As Boolean
        Dim TempEdgeInfo() As SByte = CurrentPiece.GetLeftEdgeInfo()
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(PieceLocation.Y + CurrentPiece.GetDimensions() - n).GetCell(PieceLocation.X + TempEdgeInfo(n)) Then Return False
            End If
        Next
        Return True
    End Function

    Private Function TestRight() As Boolean
        Dim TempEdgeInfo() As SByte = CurrentPiece.GetRightEdgeInfo()
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions()
        For n = 0 To TempEdgeInfo.Length - 1
            If TempEdgeInfo(n) > -128 Then
                If Board(PieceLocation.Y + PieceDimensions - n).GetCell(PieceLocation.X + TempEdgeInfo(n)) Then Return False
            End If
        Next
        Return True
    End Function

    Private Function TestLRotate() As Boolean
        If CurrentPiece.GetName() = "Square" Then Return False
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions()
        If PieceLocation.Y <= 0 OrElse PieceLocation.Y + PieceDimensions > 20 Then Return False
        Dim TempState(,) As Boolean = CurrentPiece.GetLRotateState()
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If TempState(PieceDimensions - y, x) Then
                    Dim TempX As Integer = PieceLocation.X + x
                    If TempX < 0 OrElse TempX > 11 OrElse Board(PieceLocation.Y + y).GetCell(TempX) Then Return False
                End If
            Next
        Next
        Return True
    End Function

    Private Function TestRRotate() As Boolean
        If CurrentPiece.GetName() = "Square" Then Return False
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions()
        If PieceLocation.Y <= 0 OrElse PieceLocation.Y + PieceDimensions > 20 Then Return False
        Dim TempState(,) As Boolean = CurrentPiece.GetRRotateState()
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If TempState(PieceDimensions - y, x) Then
                    Dim TempX As Integer = PieceLocation.X + x
                    If TempX < 0 OrElse TempX > 11 OrElse Board(PieceLocation.Y + y).GetCell(TempX) Then Return False
                End If
            Next
        Next
        Return True
    End Function


    Private Function CalculateMaxNoOfDowns() As Byte
        Dim MinMoves As Byte = 255
        Dim TempMoves As Byte
        Dim TempEdgeInfo() As SByte = CurrentPiece.GetDownEdgeInfo()
        Dim PieceLocation As PiecePos = CurrentPiece.GetPosition()
        For x = 0 To CurrentPiece.GetDimensions()
            If TempEdgeInfo(x) > -128 Then
                TempMoves = 0
                For y = PieceLocation.Y + TempEdgeInfo(x) To 0 Step -1
                    If Board(y).GetCell(x + PieceLocation.X) Then Exit For Else TempMoves += 1
                Next
                MinMoves = Math.Min(MinMoves, TempMoves)
            End If
        Next
        Return MinMoves
    End Function


    Private Sub DisplayPieceDown()
        For Each Box In CurrentPiecePictureBoxes
            Box.Top += DisplayElementMultiplier
        Next
    End Sub
    Private Sub DisplayPieceLeft()
        For Each Box In CurrentPiecePictureBoxes
            Box.Left -= DisplayElementMultiplier
        Next
    End Sub
    Private Sub DisplayPieceRight()
        For Each Box In CurrentPiecePictureBoxes
            Box.Left += DisplayElementMultiplier
        Next
    End Sub


    Private Sub FixPiece(ByVal FixDisplayPiece As Boolean)
        Dim PieceState As Boolean(,) = CurrentPiece.GetCurrentState
        Dim PiecePosition As PiecePos = CurrentPiece.GetPosition()
        Dim PieceDimensions As SByte = CurrentPiece.GetDimensions()
        For y = 0 To PieceDimensions
            For x = 0 To PieceDimensions
                If PieceState(PieceDimensions - y, x) Then Board(PiecePosition.Y + y).SetCell(PiecePosition.X + x)
            Next
        Next

        Dim NoLinesCleared As Integer = 0
        For y = PiecePosition.Y + PieceDimensions To Math.Max(1, PiecePosition.Y) Step -1
            If Board(y).CheckIfFull Then
                NoLinesCleared += 1
                Board.RemoveAt(y)
                Board.Add(New BoardRow())

                If FixDisplayPiece Then
                    Dim LocationComparer As UInt16 = (20 - y) * DisplayElementMultiplier + 1

                    For n = BoardPiecePictureBoxes.Count - 1 To 0 Step -1
                        If BoardPiecePictureBoxes(n).Location.Y = LocationComparer Then
                            BoardPiecePictureBoxes(n).Dispose()
                            BoardPiecePictureBoxes.RemoveAt(n)
                        End If
                    Next
                    For Each Box In BoardPiecePictureBoxes
                        If Box.Location.Y < LocationComparer Then Box.Top += DisplayElementMultiplier
                    Next
                End If
            End If
        Next
        CurrentPiece.Reset()

        'Adds scores for clearing lines.
        Select Case NoLinesCleared
            Case 1 : PlayerScore += 40
            Case 2 : PlayerScore += 100
            Case 3 : PlayerScore += 300
            Case 4 : PlayerScore += 1200
        End Select
        PlayerScore += NoLinesPushedDown
        Label1.Text = "Score: " + PlayerScore.ToString("N0")
        NoLinesPushedDown = 0

        CanHoldPiece = True
    End Sub

    Private Sub FixDisplayPiece()
        Dim TempPicureBox As PictureBox
        For Each Box In CurrentPiecePictureBoxes
            TempPicureBox = New PictureBox()
            With TempPicureBox
                .Location = Box.Location
                .Size = Box.Size
                .Image = Box.Image
            End With
            GameBoard.Controls.Add(TempPicureBox)
            TempPicureBox.BringToFront()
            BoardPiecePictureBoxes.Add(TempPicureBox)
        Next
    End Sub


    Private Sub HoldPiece(ByVal UpdateGUI As Boolean)
        If CanHoldPiece Then
            Dim CurrentPieceIndex As SByte = CurrentPiece.GetIndex()
            CanHoldPiece = False

            If HeldIndex = -1 Then
                If PieceBag.Count <= 7 Then PopulateBag()
                HeldIndex = PieceBag(0)
                PieceBag.RemoveAt(0)
                ModifyNextPieces()
            End If

            CurrentPiece = PieceTemplateArray(HeldIndex)
            CurrentPiece.Reset()
            For n = 0 To 3
                CurrentPiecePictureBoxes(n).Image = PieceImageArray(HeldIndex)
            Next
            RefreshPiece()
            HeldIndex = CurrentPieceIndex
            PieceTemplateArray(HeldIndex).Reset()

            If UpdateGUI Then HeldBox.Image = PieceGUIImageArray(HeldIndex)
            PositionPredictedPiece()
            If AIMode Then
                If AIMovesQueue.Count = 0 Then InitAIThread()
            Else
                GameClock.Change(TickConstant, TickConstant)
            End If
        End If
    End Sub


    Private Sub AIBtn_CheckedChanged() Handles AIBtn.CheckedChanged
        If AIBtn.Checked Then
            AIMode = True
            Tetris_KeyPress(Nothing, If(HeldIndex = -1, New KeyEventArgs(Keys.ShiftKey), New KeyEventArgs(Keys.Space)))
        Else
            If GameRunning AndAlso Not AIMode Then GameClock.Change(TickConstant, TickConstant)
            AIMode = False
        End If
    End Sub
    Private Sub RadioButtons_Click(sender As Object, e As EventArgs) Handles AIBtn.Click, HumanBtn.Click
        ActiveControl = Nothing
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or &H2000000
            Return cp
        End Get
    End Property
End Class
