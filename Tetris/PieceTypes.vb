Public Class LinePiece
    Inherits Piece
    Public Sub New()
        Name = "Line"
        Index = 0
        StartPosX = 4
        StartPosY = 18
        Dimensions = 3
        States = {
    {
        {False, False, False, False},
        {True, True, True, True},
        {False, False, False, False},
        {False, False, False, False}},
        {
        {False, False, True, False},
        {False, False, True, False},
        {False, False, True, False},
        {False, False, True, False}},
        {
        {False, False, False, False},
        {False, False, False, False},
        {True, True, True, True},
        {False, False, False, False}},
        {
        {False, True, False, False},
        {False, True, False, False},
        {False, True, False, False},
        {False, True, False, False}}}

        ConstructPiece()
    End Sub
End Class

Public Class LPiece
    Inherits Piece
    Public Sub New()
        Name = "L"
        Index = 1
        StartPosX = 4
        StartPosY = 18
        Dimensions = 2
        States = {
    {
        {True, False, False},
        {True, True, True},
        {False, False, False}},
        {
        {False, True, True},
        {False, True, False},
        {False, True, False}},
        {
        {False, False, False},
        {True, True, True},
        {False, False, True}},
        {
        {False, True, False},
        {False, True, False},
        {True, True, False}}}

        ConstructPiece()
    End Sub
End Class

Public Class ReverseLPiece
    Inherits Piece
    Public Sub New()
        Name = "ReverseL"
        Index = 2
        StartPosX = 4
        StartPosY = 18
        Dimensions = 2
        States = {
    {
        {False, False, True},
        {True, True, True},
        {False, False, False}},
        {
        {False, True, False},
        {False, True, False},
        {False, True, True}},
        {
        {False, False, False},
        {True, True, True},
        {True, False, False}},
        {
        {True, True, False},
        {False, True, False},
        {False, True, False}}}

        ConstructPiece()
    End Sub
End Class

Public Class SquarePiece
    Inherits Piece
    Public Sub New()
        Name = "Square"
        Index = 3
        StartPosX = 5
        StartPosY = 19
        Dimensions = 1
        States = {
    {
        {True, True},
        {True, True}},
        {
        {True, True},
        {True, True}},
        {
        {True, True},
        {True, True}},
        {
        {True, True},
        {True, True}}}

        ConstructPiece()
    End Sub
End Class

Public Class SPiece
    Inherits Piece
    Public Sub New()
        Name = "S"
        Index = 4
        StartPosX = 4
        StartPosY = 18
        Dimensions = 2
        States = {
    {
        {False, True, True},
        {True, True, False},
        {False, False, False}},
        {
        {False, True, False},
        {False, True, True},
        {False, False, True}},
        {
        {False, False, False},
        {False, True, True},
        {True, True, False}},
        {
        {True, False, False},
        {True, True, False},
        {False, True, False}}}

        ConstructPiece()
    End Sub
End Class

Public Class ReverseSPiece
    Inherits Piece
    Public Sub New()
        Name = "ReverseS"
        Index = 5
        StartPosX = 4
        StartPosY = 18
        Dimensions = 2
        States = {
    {
        {True, True, False},
        {False, True, True},
        {False, False, False}},
        {
        {False, False, True},
        {False, True, True},
        {False, True, False}},
        {
        {False, False, False},
        {True, True, False},
        {False, True, True}},
        {
        {False, True, False},
        {True, True, False},
        {True, False, False}}}

        ConstructPiece()
    End Sub
End Class

Public Class TPiece
    Inherits Piece
    Public Sub New()
        Name = "T"
        Index = 6
        StartPosX = 4
        StartPosY = 18
        Dimensions = 2
        States = {
    {
        {False, True, False},
        {True, True, True},
        {False, False, False}},
        {
        {False, True, False},
        {False, True, True},
        {False, True, False}},
        {
        {False, False, False},
        {True, True, True},
        {False, True, False}},
        {
        {False, True, False},
        {True, True, False},
        {False, True, False}}}

        ConstructPiece()
    End Sub
End Class
