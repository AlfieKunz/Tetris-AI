<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Tetris
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Tetris))
        GameBoard = New PictureBox()
        PieceInfoPanel = New Panel()
        AIBtn = New RadioButton()
        HumanBtn = New RadioButton()
        NextBox3 = New PictureBox()
        NextBox2 = New PictureBox()
        NextBox1 = New PictureBox()
        OutlineBox2 = New PictureBox()
        NextLabel = New Label()
        HeldBox = New PictureBox()
        OutlineBox1 = New PictureBox()
        HeldLabel = New Label()
        GameOverLabel = New Label()
        Label1 = New Label()
        Label2 = New Label()
        CType(GameBoard, ComponentModel.ISupportInitialize).BeginInit()
        PieceInfoPanel.SuspendLayout()
        CType(NextBox3, ComponentModel.ISupportInitialize).BeginInit()
        CType(NextBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(NextBox1, ComponentModel.ISupportInitialize).BeginInit()
        CType(OutlineBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(HeldBox, ComponentModel.ISupportInitialize).BeginInit()
        CType(OutlineBox1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' GameBoard
        ' 
        GameBoard.BackgroundImage = CType(resources.GetObject("GameBoard.BackgroundImage"), Image)
        GameBoard.Location = New Point(12, 39)
        GameBoard.Name = "GameBoard"
        GameBoard.Size = New Size(342, 684)
        GameBoard.TabIndex = 0
        GameBoard.TabStop = False
        ' 
        ' PieceInfoPanel
        ' 
        PieceInfoPanel.BackColor = Color.Silver
        PieceInfoPanel.Controls.Add(AIBtn)
        PieceInfoPanel.Controls.Add(HumanBtn)
        PieceInfoPanel.Controls.Add(NextBox3)
        PieceInfoPanel.Controls.Add(NextBox2)
        PieceInfoPanel.Controls.Add(NextBox1)
        PieceInfoPanel.Controls.Add(OutlineBox2)
        PieceInfoPanel.Controls.Add(NextLabel)
        PieceInfoPanel.Controls.Add(HeldBox)
        PieceInfoPanel.Controls.Add(OutlineBox1)
        PieceInfoPanel.Controls.Add(HeldLabel)
        PieceInfoPanel.Location = New Point(390, 28)
        PieceInfoPanel.Name = "PieceInfoPanel"
        PieceInfoPanel.Size = New Size(160, 694)
        PieceInfoPanel.TabIndex = 1
        ' 
        ' AIBtn
        ' 
        AIBtn.AutoSize = True
        AIBtn.Font = New Font("Elephant", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point)
        AIBtn.Location = New Point(21, 50)
        AIBtn.Margin = New Padding(3, 2, 3, 2)
        AIBtn.Name = "AIBtn"
        AIBtn.Size = New Size(87, 22)
        AIBtn.TabIndex = 9
        AIBtn.Text = "AI Mode"
        AIBtn.UseVisualStyleBackColor = True
        ' 
        ' HumanBtn
        ' 
        HumanBtn.AutoSize = True
        HumanBtn.Checked = True
        HumanBtn.Font = New Font("Elephant", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point)
        HumanBtn.Location = New Point(21, 24)
        HumanBtn.Margin = New Padding(3, 2, 3, 2)
        HumanBtn.Name = "HumanBtn"
        HumanBtn.Size = New Size(122, 22)
        HumanBtn.TabIndex = 9
        HumanBtn.TabStop = True
        HumanBtn.Text = "Human Mode"
        HumanBtn.UseVisualStyleBackColor = True
        ' 
        ' NextBox3
        ' 
        NextBox3.BackColor = Color.White
        NextBox3.Location = New Point(21, 565)
        NextBox3.Name = "NextBox3"
        NextBox3.Size = New Size(120, 120)
        NextBox3.TabIndex = 8
        NextBox3.TabStop = False
        ' 
        ' NextBox2
        ' 
        NextBox2.BackColor = Color.White
        NextBox2.Location = New Point(21, 439)
        NextBox2.Name = "NextBox2"
        NextBox2.Size = New Size(120, 120)
        NextBox2.TabIndex = 7
        NextBox2.TabStop = False
        ' 
        ' NextBox1
        ' 
        NextBox1.BackColor = Color.White
        NextBox1.Location = New Point(21, 313)
        NextBox1.Name = "NextBox1"
        NextBox1.Size = New Size(120, 120)
        NextBox1.TabIndex = 6
        NextBox1.TabStop = False
        ' 
        ' OutlineBox2
        ' 
        OutlineBox2.BackColor = Color.Black
        OutlineBox2.Location = New Point(17, 309)
        OutlineBox2.Name = "OutlineBox2"
        OutlineBox2.Size = New Size(128, 380)
        OutlineBox2.TabIndex = 5
        OutlineBox2.TabStop = False
        ' 
        ' NextLabel
        ' 
        NextLabel.AutoSize = True
        NextLabel.Font = New Font("Elephant", 20.2499962F, FontStyle.Regular, GraphicsUnit.Point)
        NextLabel.Location = New Point(24, 269)
        NextLabel.Name = "NextLabel"
        NextLabel.Size = New Size(105, 35)
        NextLabel.TabIndex = 2
        NextLabel.Text = "NEXT"
        ' 
        ' HeldBox
        ' 
        HeldBox.BackColor = Color.White
        HeldBox.Location = New Point(21, 128)
        HeldBox.Name = "HeldBox"
        HeldBox.Size = New Size(120, 120)
        HeldBox.TabIndex = 1
        HeldBox.TabStop = False
        ' 
        ' OutlineBox1
        ' 
        OutlineBox1.BackColor = Color.Black
        OutlineBox1.Location = New Point(17, 123)
        OutlineBox1.Name = "OutlineBox1"
        OutlineBox1.Size = New Size(128, 128)
        OutlineBox1.TabIndex = 4
        OutlineBox1.TabStop = False
        ' 
        ' HeldLabel
        ' 
        HeldLabel.AutoSize = True
        HeldLabel.Font = New Font("Elephant", 20.2499962F, FontStyle.Regular, GraphicsUnit.Point)
        HeldLabel.Location = New Point(20, 86)
        HeldLabel.Name = "HeldLabel"
        HeldLabel.Size = New Size(112, 35)
        HeldLabel.TabIndex = 0
        HeldLabel.Text = "HELD"
        ' 
        ' GameOverLabel
        ' 
        GameOverLabel.AutoSize = True
        GameOverLabel.BackColor = Color.WhiteSmoke
        GameOverLabel.Font = New Font("Elephant", 35.9999962F, FontStyle.Regular, GraphicsUnit.Point)
        GameOverLabel.Location = New Point(27, 346)
        GameOverLabel.Name = "GameOverLabel"
        GameOverLabel.Size = New Size(314, 62)
        GameOverLabel.TabIndex = 2
        GameOverLabel.Text = "Game Over!"
        GameOverLabel.TextAlign = ContentAlignment.MiddleCenter
        GameOverLabel.Visible = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Elephant", 11.999999F, FontStyle.Regular, GraphicsUnit.Point)
        Label1.Location = New Point(14, 10)
        Label1.Name = "Label1"
        Label1.Size = New Size(76, 21)
        Label1.TabIndex = 3
        Label1.Text = "Score: 0"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Elephant", 11.999999F, FontStyle.Regular, GraphicsUnit.Point)
        Label2.Location = New Point(193, 10)
        Label2.Name = "Label2"
        Label2.Size = New Size(164, 21)
        Label2.TabIndex = 3
        Label2.Text = "Made by Alfie Kunz"
        ' 
        ' Tetris
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.White
        ClientSize = New Size(582, 634)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(GameOverLabel)
        Controls.Add(PieceInfoPanel)
        Controls.Add(GameBoard)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Tetris"
        Text = "Tetris"
        CType(GameBoard, ComponentModel.ISupportInitialize).EndInit()
        PieceInfoPanel.ResumeLayout(False)
        PieceInfoPanel.PerformLayout()
        CType(NextBox3, ComponentModel.ISupportInitialize).EndInit()
        CType(NextBox2, ComponentModel.ISupportInitialize).EndInit()
        CType(NextBox1, ComponentModel.ISupportInitialize).EndInit()
        CType(OutlineBox2, ComponentModel.ISupportInitialize).EndInit()
        CType(HeldBox, ComponentModel.ISupportInitialize).EndInit()
        CType(OutlineBox1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents GameBoard As PictureBox
    Friend WithEvents PieceInfoPanel As Panel
    Friend WithEvents HeldLabel As Label
    Friend WithEvents HeldBox As PictureBox
    Friend WithEvents NextLabel As Label
    Friend WithEvents OutlineBox2 As PictureBox
    Friend WithEvents OutlineBox1 As PictureBox
    Friend WithEvents NextBox3 As PictureBox
    Friend WithEvents NextBox2 As PictureBox
    Friend WithEvents NextBox1 As PictureBox
    Friend WithEvents GameOverLabel As Label
    Friend WithEvents HumanBtn As RadioButton
    Friend WithEvents AIBtn As RadioButton
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
End Class
