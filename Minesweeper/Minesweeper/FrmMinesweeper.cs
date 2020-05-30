using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//7.3
using Minesweeper.BaseClass;

namespace Minesweeper
{
    public partial class FrmMinesweeper : Form
    {
        public FrmMinesweeper()
        {
            InitializeComponent();
        }

        //7.2
        static int Difficulty = 1;//难度，初始为1--中级
        readonly int[] RowsColsNums = new int[] { 9, 9, 10, 16, 16, 40, 16, 30, 99 };//分别对应初级、中级、高级的行数、列数和雷的个数
        int RowsNum = 16, ColsNum = 16, MinesNum = 40;

        //7.2
        int FrmMainWidth, FrmMainHeight;
        Rectangle FaceAndDigitArea, MinesArea;
        Rectangle FaceRect;

        //7.2
        #region 窗体尺寸数据
        readonly int TitleHeight = 30;
        readonly int MenuBarHeight = 25;
        readonly int LineWidth_1 = 2;
        readonly int LineWidth_2 = 3;
        readonly int GrayWidth_1 = 5;
        readonly int GrayWidth_2 = 6;
        readonly int SmallShellHeight = 37;
        readonly int MineWidth = 20;


        readonly int FaceWidth = 24;

        readonly int DigitWidth = 13;
        readonly int DigitHeight = 23;
        #endregion

        //7.2
        #region 几个重要颜色值
        readonly Color GRAY = Color.FromArgb(192, 192, 192);
        readonly Color DARK_GRAY = Color.FromArgb(128, 128, 128);
        readonly Color WHITE = Color.FromArgb(255, 255, 255);
        #endregion

        //7.3
        int SpendSeconds = 0;
        Bitmap[] bmpMines = new Bitmap[16];
        Bitmap[] bmpFaces = new Bitmap[5];
        Bitmap[] bmpDigits = new Bitmap[10];

        //7.3
        #region 枚举变量
        enum MineGridState { Normal, Flag, Unknown3D, Blast, Error, Mine, UnknownFlat, Num8, Num7, Num6, Num5, Num4, Num3, Num2, Num1, Empty };
        enum FaceStyle { Down, Victory, Dead, Click, Normal };
        int FaceIndex = 4;
        enum GameState { Wait, Run, Dead, Victory };
        int gameState = 0;
        #endregion

        //7.3
        MineGrid[,] mineGrids = new MineGrid[16, 30];

        //7.5
        bool LDown = false, RDown = false, LRDown = false;
        int OldIndex = -1, NewIndex = -1;                 //记录鼠标移动时上一个和当前的方格的索引号
        bool bFacePressed = false;
        //7.2
        #region 自定义函数ResizeWindow()
        private void ResizeWindow()
        {
            FrmMainWidth = LineWidth_2 * 5 + GrayWidth_1 + GrayWidth_2 + RowsColsNums[3 * Difficulty + 1] * MineWidth; ;
            FrmMainHeight = TitleHeight + MenuBarHeight + LineWidth_1 * 2 + LineWidth_2 * 3 + GrayWidth_1 * 2 + GrayWidth_2 + SmallShellHeight + RowsColsNums[3 * Difficulty] * MineWidth;
            this.Width = FrmMainWidth;
            this.Height = FrmMainHeight;
            RowsNum = RowsColsNums[Difficulty * 3];
            ColsNum = RowsColsNums[Difficulty * 3 + 1];
            //7.3
            this.InitGame();
        }
        #endregion

        //7.2
        #region 自定义函数:DrawFrame(Graphics graphic)
        private void DrawFrame(Graphics graphic)
        {
            //画阴影线
            Pen penDarkShadow_Narrow = new Pen(DARK_GRAY, LineWidth_1);
            Pen penDarkShadow_Wide = new Pen(DARK_GRAY, LineWidth_2);
            Pen penLightShadow_Narrow = new Pen(WHITE, LineWidth_1);
            Pen penLightShadow_Wide = new Pen(WHITE, LineWidth_2);
            Point p_LeftTop = new Point(LineWidth_2 + GrayWidth_2, MenuBarHeight + LineWidth_2 + GrayWidth_2);
            Point p_RightBottom = new Point(this.Width - LineWidth_2 * 2 - GrayWidth_1, p_LeftTop.Y + SmallShellHeight);

            graphic.DrawLine(penLightShadow_Narrow, 1, 26, this.ClientRectangle.Width, 26);
            graphic.DrawLine(penLightShadow_Wide, 1, 26, 1, this.ClientRectangle.Height);

            graphic.DrawLine(penDarkShadow_Narrow, p_LeftTop.X, p_LeftTop.Y, p_RightBottom.X, p_LeftTop.Y);
            graphic.DrawLine(penLightShadow_Narrow, p_RightBottom.X, p_LeftTop.Y, p_RightBottom.X, p_RightBottom.Y);
            graphic.DrawLine(penLightShadow_Narrow, p_RightBottom.X, p_RightBottom.Y, p_LeftTop.X, p_RightBottom.Y);
            graphic.DrawLine(penDarkShadow_Narrow, p_LeftTop.X, p_RightBottom.Y, p_LeftTop.X, p_LeftTop.Y);
            FaceAndDigitArea = new Rectangle(p_LeftTop.X, p_LeftTop.Y, p_RightBottom.X - p_LeftTop.X, p_RightBottom.Y - p_LeftTop.Y);

            p_LeftTop.Y += SmallShellHeight + LineWidth_1 + GrayWidth_2;
            p_RightBottom.Y = p_LeftTop.Y + RowsColsNums[Difficulty * 3] * MineWidth + LineWidth_2;
            graphic.DrawLine(penDarkShadow_Wide, p_LeftTop.X, p_LeftTop.Y, p_RightBottom.X, p_LeftTop.Y);
            graphic.DrawLine(penLightShadow_Wide, p_RightBottom.X, p_LeftTop.Y, p_RightBottom.X, p_RightBottom.Y);
            graphic.DrawLine(penLightShadow_Wide, p_RightBottom.X, p_RightBottom.Y, p_LeftTop.X, p_RightBottom.Y);
            graphic.DrawLine(penDarkShadow_Wide, p_LeftTop.X, p_RightBottom.Y, p_LeftTop.X, p_LeftTop.Y);
            MinesArea = new Rectangle(p_LeftTop.X + 3, p_LeftTop.Y + 2, p_RightBottom.X - p_LeftTop.X - 7, p_RightBottom.Y - p_LeftTop.Y - 5);
        }
        #endregion

        //7.2
        #region 自定义函数DrawGame(Graphics graphic)
        private void DrawGame(Graphics graphic)
        {
            this.DrawFrame(graphic);
            //7.3
            this.DrawFace(graphic);
            this.DrawDigits(graphic);
            this.DrawMineGrids(graphic);
        }
        #endregion

        //7.2 
        #region OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bufferBmp = new Bitmap(this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            Graphics g = Graphics.FromImage(bufferBmp);
            this.DrawGame(g);
            e.Graphics.DrawImage(bufferBmp, 0, 0);
            g.Dispose();
            base.OnPaint(e);
        }
        #endregion

        //7.2
        #region 菜单响应函数
        private void 初级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.初级ToolStripMenuItem.Checked = true;
            this.中级ToolStripMenuItem.Checked = false;
            this.高级ToolStripMenuItem.Checked = false;
            Difficulty = 0;
            this.ResizeWindow();
            this.Invalidate();
        }

        private void 中级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.初级ToolStripMenuItem.Checked = false;
            this.中级ToolStripMenuItem.Checked = true;
            this.高级ToolStripMenuItem.Checked = false;
            Difficulty = 1;
            this.ResizeWindow();
            this.Invalidate();
        }
        
        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.初级ToolStripMenuItem.Checked = false;
            this.中级ToolStripMenuItem.Checked = false;
            this.高级ToolStripMenuItem.Checked = true;
            Difficulty = 2;
            this.ResizeWindow();
            this.Invalidate();
        }
        #endregion

        //7.2
        #region FrmMinesweeper_Load(object sender, EventArgs e)
        private void FrmMinesweeper_Load(object sender, EventArgs e)
        {
            //7.3
            this.LoadAllBitmaps();
            //7.2
            this.ResizeWindow();
        }
        #endregion

        //7.3
        #region LoadAllBitmaps()
        private void LoadAllBitmaps()
        {
            Bitmap faces = LoadBitmap.LoadBmp("Faces");
            for (int i = 0; i < 5; i++)
            {
                bmpFaces[i] = faces.Clone(new Rectangle(0, FaceWidth * i, FaceWidth, FaceWidth), faces.PixelFormat);
                bmpFaces[i] = new Bitmap(bmpFaces[i], new Size(FaceWidth, FaceWidth));
            }
            Bitmap mines = LoadBitmap.LoadBmp("Mines");
            for (int i = 0; i < 16; i++)
            {
                bmpMines[i] = mines.Clone(new Rectangle(0, MineWidth * i, MineWidth, MineWidth), mines.PixelFormat);
                bmpMines[i] = new Bitmap(bmpMines[i], new Size(MineWidth, MineWidth));
            }
            Bitmap digits = LoadBitmap.LoadBmp("Digits");
            for (int i = 0; i < 10; i++)
            {
                bmpDigits[i] = digits.Clone(new Rectangle(0, (11 - i) * DigitHeight, DigitWidth, DigitHeight), digits.PixelFormat);
                bmpDigits[i] = new Bitmap(bmpDigits[i], new Size(DigitWidth, DigitHeight));
            }
        }
        #endregion

        //7.3
        #region InitGame()
        private void InitGame()
        {
            SpendSeconds = 0;
            gameState = (int)GameState.Wait;
            FaceIndex = (int)FaceStyle.Normal;
            MinesNum = RowsColsNums[Difficulty * 3 + 2];
            for (int i = 0; i < RowsNum; i++)
                for (int j = 0; j < ColsNum; j++)
                {
                    mineGrids[i, j] = new MineGrid();
                    mineGrids[i, j].Row = i;
                    mineGrids[i, j].Col = j;
                    mineGrids[i, j].State = (int)(MineGridState.Normal);
                    mineGrids[i, j].OldState = (int)(MineGridState.Normal);
                    mineGrids[i, j].IsMine = false;
                }
        }
        #endregion

        //7.3
        #region DrawFace
        private void DrawFace(Graphics graphic)
        {
            FaceRect = new Rectangle(FaceAndDigitArea.Left + (FaceAndDigitArea.Width - FaceWidth) / 2, FaceAndDigitArea.Top + (FaceAndDigitArea.Height - FaceWidth) / 2, FaceWidth, FaceWidth);
            graphic.DrawImage(bmpFaces[FaceIndex], FaceRect.X, FaceRect.Y);
        }
        #endregion

        //7.3
        #region DrawDigits
        private void DrawDigits(Graphics graphic)
        {
            //画剩余的雷的个数
            graphic.DrawImage(bmpDigits[MinesNum / 10], FaceAndDigitArea.X + 8, FaceAndDigitArea.Y + 7);
            graphic.DrawImage(bmpDigits[MinesNum % 10], FaceAndDigitArea.X + 8 + DigitWidth, FaceAndDigitArea.Y + 7);
            //画已用时间
            graphic.DrawImage(bmpDigits[SpendSeconds / 100], FaceAndDigitArea.Right - 8 - DigitWidth * 3, FaceAndDigitArea.Y + 7);
            graphic.DrawImage(bmpDigits[SpendSeconds % 100 / 10], FaceAndDigitArea.Right - 8 - DigitWidth * 2, FaceAndDigitArea.Y + 7);
            graphic.DrawImage(bmpDigits[SpendSeconds % 10], FaceAndDigitArea.Right - 8 - DigitWidth, FaceAndDigitArea.Y + 7);
        }
        #endregion

        //7.3 
        #region DrawMineGrids
        private void DrawMineGrids(Graphics graphic)
        {
            for (int i = 0; i < RowsNum; i++)
                for (int j = 0; j < ColsNum; j++)
                    graphic.DrawImage(bmpMines[mineGrids[i, j].State], MinesArea.Left + j * MineWidth, MinesArea.Top + i * MineWidth);
        }
        #endregion

        //7.5
        #region FrmMinesweeper_MouseDown
        private void FrmMinesweeper_MouseDown(object sender, MouseEventArgs e)
        {
            //7.9
            if (gameState > 1 && !PtInRect(e.Location, FaceRect))
                return;//7.9
            if (e.Button == MouseButtons.Left)
                LDown = true;
            if (e.Button == MouseButtons.Right)
                RDown = true;
            LRDown = LDown && RDown;
            MouseButtonDown(e);
            this.Invalidate();
        }
        #endregion

        //7.5
        #region FrmMinesweeper_MouseUp
        private void FrmMinesweeper_MouseUp(object sender, MouseEventArgs e)
        {
            MouseButtonUp(e);
            if (e.Button == MouseButtons.Left)
                LDown = false;
            if (e.Button == MouseButtons.Right)
                RDown = false;
            LRDown = false;
            this.Invalidate();
        }
        #endregion

        //7.5
        #region FrmMinesweeper_MouseMove
        private void FrmMinesweeper_MouseMove(object sender, MouseEventArgs e)
        {
            if (PtInRect(e.Location, MinesArea))
                NewIndex = (e.Y - MinesArea.Y) / MineWidth * ColsNum + (e.X - MinesArea.X) / MineWidth;
            else
                NewIndex = -1;
            if (NewIndex == -1)
            {
                for (int row = 0; row < RowsNum; row++)
                    for (int col = 0; col < ColsNum; col++)
                        mineGrids[row, col].State = mineGrids[row, col].OldState;
            }
            if (NewIndex > -1 && OldIndex == -1)
            {
                int row = NewIndex / ColsNum;
                int col = NewIndex % ColsNum;
                int minRow = row == 0 ? 0 : row - 1;
                int maxRow = row == RowsNum - 1 ? row : row + 1;
                int minCol = col == 0 ? 0 : col - 1;
                int maxCol = col == ColsNum - 1 ? col : col + 1;
                if (LRDown)
                    for (int i = minRow; i <= maxRow; i++)
                        for (int j = minCol; j <= maxCol; j++)
                            if (mineGrids[i, j].OldState == (int)MineGridState.Normal)
                                mineGrids[i, j].State = (int)MineGridState.Empty;
                if (LDown && !RDown)
                    if (mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State == (int)MineGridState.Normal)
                        mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Empty;
            }
            if (NewIndex > -1 && OldIndex > -1 && NewIndex != OldIndex)
            {
                if (LRDown)
                {
                    int row = OldIndex / ColsNum;
                    int col = OldIndex % ColsNum;
                    int minRow = row == 0 ? 0 : row - 1;
                    int maxRow = row == RowsNum - 1 ? row : row + 1;
                    int minCol = col == 0 ? 0 : col - 1;
                    int maxCol = col == ColsNum - 1 ? col : col + 1;
                    for (int i = minRow; i <= maxRow; i++)
                        for (int j = minCol; j <= maxCol; j++)
                            if (mineGrids[i, j].OldState == (int)MineGridState.Normal)
                                mineGrids[i, j].State = (int)MineGridState.Normal;
                    row = NewIndex / ColsNum;
                    col = NewIndex % ColsNum;
                    minRow = row == 0 ? 0 : row - 1;
                    maxRow = row == RowsNum - 1 ? row : row + 1;
                    minCol = col == 0 ? 0 : col - 1;
                    maxCol = col == ColsNum - 1 ? col : col + 1;
                    for (int i = minRow; i <= maxRow; i++)
                        for (int j = minCol; j <= maxCol; j++)
                            if (mineGrids[i, j].OldState == (int)MineGridState.Normal)
                                mineGrids[i, j].State = (int)MineGridState.Empty;
                }
                if (LDown && !RDown)
                {
                    if (mineGrids[OldIndex / ColsNum, OldIndex % ColsNum].OldState == (int)MineGridState.Normal)
                        mineGrids[OldIndex / ColsNum, OldIndex % ColsNum].State = (int)MineGridState.Normal;
                    if (mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State == (int)MineGridState.Normal)
                        mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Empty;
                }
            }
            this.Invalidate();
            OldIndex = NewIndex;
        }
        #endregion

        //7.5
        #region 自定义函数 MouseButtonDown(MouseEventArgs e)
        private void MouseButtonDown(MouseEventArgs e)
        {
            if (PtInRect(e.Location, FaceRect))
            {
                bFacePressed = true;
                FaceIndex = (int)FaceStyle.Down;
            }
            //仅左键被按下
            if (LDown && !RDown)
            {
                //7.7
                if (NewIndex > -1)
                {
                    if (gameState == (int)GameState.Wait || gameState == (int)GameState.Run &&mineGrids[NewIndex/ColsNum,NewIndex%ColsNum].State == (int)MineGridState.Normal)
                    {
                        mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Empty;
                        FaceIndex = (int)FaceStyle.Click;
                    }
                }
            }
            //仅右键被按下
            if (!LDown && RDown)
            {
                //7.7
                if (gameState <2&& NewIndex > -1)
                {
                    switch ((int)mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State)
                    {
                        case (int)MineGridState.Normal:
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Flag;
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = (int)MineGridState.Flag;
                            MinesNum--;
                            FaceIndex = (int)FaceStyle.Click;
                            break;
                        case (int)MineGridState.Flag:
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Unknown3D;
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = (int)MineGridState.Unknown3D;
                            MinesNum++;
                            break;
                        case (int)MineGridState.Unknown3D:
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Normal;
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = (int)MineGridState.Normal;
                            break;
                    }
                    
                }//7.7
            }
            //左右键都被按下
            if (LRDown)
            {
                //7.7
                if (NewIndex < 0)
                    return;
                int row = NewIndex / ColsNum;
                int col = NewIndex % ColsNum;
                int minRow = row == 0 ? 0 : row - 1;
                int maxRow = row == RowsNum - 1 ? row : row + 1;
                int minCol = col == 0 ? 0 : col - 1;
                int maxCol = col == ColsNum - 1 ? col : col + 1;

                for (int i = minRow; i <= maxRow; i++)
                {
                    for (int j = minCol; j <= maxCol; j++)
                    {
                        if (i == row && j == col) continue;
                        if (mineGrids[i, j].State == (int)MineGridState.Normal)
                            mineGrids[i, j].State = (int)MineGridState.Empty;
                        if (mineGrids[i, j].State == (int)MineGridState.Unknown3D)
                            mineGrids[i, j].State = (int)MineGridState.UnknownFlat;
                    }
                }//7.7
                FaceIndex = (int)FaceStyle.Click;
            }
        }
        #endregion

        //7.5
        #region 自定义函数MouseButtonUp(MouseEventArgs e)
        private void MouseButtonUp(MouseEventArgs e)
        {
            if (bFacePressed)
            {
                FaceIndex = (int)FaceStyle.Normal;
                bFacePressed = !bFacePressed;
                this.InitGame();
            }
            //仅鼠标左键按下后的释放
            if (LDown && !RDown)
            {
                //7.7
                if (NewIndex > -1)
                {
                    //7.8
                    if (gameState == (int)GameState.Run)
                    {
                        if (mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State == (int)MineGridState.Empty && mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState == (int)MineGridState.Normal)
                        {
                            int aroundNums = GetAroundNums(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);
                            if (aroundNums == 0)
                            {
                                Expand(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);

                            }
                            else
                            {

                                mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = 15 - aroundNums;
                                mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = 15 - aroundNums;
                            }
                        }
                        if (mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].IsMine)
                        {

                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = (int)MineGridState.Blast;
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = (int)MineGridState.Blast;
                            this.Dead();
                            return;
                        }
                    }//7.8
                    if (gameState == (int)GameState.Wait)
                    {
                        gameState = (int)GameState.Run;
                        LayMines();
                        int aroundNums = GetAroundNums(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);
                        if (aroundNums == 0)
                            Expand(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);
                        else
                        {
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = 15 - aroundNums;
                            mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = 15 - aroundNums;
                        }
                        //7.9
                        this.GameTimer.Start();
                    }
                }
            }
            //鼠标左右键都被按下后的释放
            //7.8
            if (LRDown && NewIndex > -1 && gameState<2)
            {
                //7.9最后添加
                if (gameState == (int)GameState.Wait)
                {
                    gameState = (int)GameState.Run;
                    LayMines();
                    int aroundNums = GetAroundNums(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);
                    if (aroundNums == 0)
                        Expand(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]);
                    else
                    {
                        mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State = 15 - aroundNums;
                        mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState = 15 - aroundNums;
                    }
                    //7.9
                    this.GameTimer.Start();
                }//7.9最后添加
                if (mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].OldState == (int)MineGridState.Normal && mineGrids[NewIndex / ColsNum, NewIndex % ColsNum].State == (int)MineGridState.Empty)
                    for (int i = 0; i < RowsNum; i++)
                        for (int j = 0; j < ColsNum; j++)
                        {
                            mineGrids[i, j].State = mineGrids[i, j].OldState;
                        }
                int row = NewIndex / ColsNum;
                int col = NewIndex % ColsNum;
                int minRow = row == 0 ? 0 : row - 1;
                int maxRow = row == RowsNum - 1 ? row : row + 1;
                int minCol = col == 0 ? 0 : col - 1;
                int maxCol = col == ColsNum - 1 ? col : col + 1;
                int AroundFlages = 0;
                for (int i = minRow; i <= maxRow; i++)
                {
                    for (int j = minCol; j <= maxCol; j++)
                    {
                        if (i == row && j == col) continue;
                        if (!mineGrids[i, j].IsMine && mineGrids[i, j].State == (int)MineGridState.Flag)
                        {
                            mineGrids[i, j].State = (int)MineGridState.Error;
                            mineGrids[i, j].OldState = (int)MineGridState.Error;
                            this.Dead();
                            return;
                        }
                        if (mineGrids[i, j].IsMine && mineGrids[i, j].State == (int)MineGridState.Flag)
                        {
                            AroundFlages++;
                        }
                    }
                }
                if (GetAroundNums(mineGrids[NewIndex / ColsNum, NewIndex % ColsNum]) != AroundFlages)
                {
                    for (int i = minRow; i <= maxRow; i++)
                    {
                        for (int j = minCol; j <= maxCol; j++)
                        {
                            if (i == row && j == col) continue;


                            if (mineGrids[i, j].State == (int)MineGridState.Empty && mineGrids[i, j].OldState == (int)MineGridState.Normal)
                            {
                                mineGrids[i, j].State = (int)MineGridState.Normal;
                                mineGrids[i, j].OldState = (int)MineGridState.Normal;

                            }
                            if (mineGrids[i, j].State == (int)MineGridState.UnknownFlat)
                            {
                                mineGrids[i, j].State = (int)MineGridState.Unknown3D;
                                mineGrids[i, j].OldState = (int)MineGridState.Unknown3D;

                            }
                        }
                    }


                }
                else
                {
                    for (int i = minRow; i <= maxRow; i++)
                    {
                        for (int j = minCol; j <= maxCol; j++)
                        {
                            if (i == row && j == col) continue;

                            if (mineGrids[i, j].State != (int)MineGridState.Flag)
                            {
                                int aroundnums = GetAroundNums(mineGrids[i, j]);
                                mineGrids[i, j].State = 15 - aroundnums;
                                mineGrids[i, j].OldState = 15 - aroundnums;
                                if (aroundnums == 0)
                                {
                                    Expand(mineGrids[i, j]);

                                }
                            }

                        }
                    }


                }
            }
            //FaceIndex = (int)FaceStyle.Normal;//7.8
            //7.9
            if (gameState != (int)GameState.Dead)
                FaceIndex = (int)FaceStyle.Normal;
            if (this.Victory())
            {
                gameState = (int)GameState.Victory;
                FaceIndex = (int)FaceStyle.Victory;
                //7.9最后添加
                this.GameTimer.Stop();
            }//7.9
        }
        #endregion

        //7.5
        #region PtInRect
        private bool PtInRect(Point pt, Rectangle rect)
        {
            return pt.X >= rect.Left && pt.X <= rect.Right && pt.Y >= rect.Top && pt.Y <= rect.Bottom;
        }
        #endregion

        //7.6
        #region 自定义函数LayMines()
        private void LayMines()
        {
            for (int LayedNums = 0; LayedNums < MinesNum; )
            {
                int row = GetRandom.GetRandomInt(RowsNum);
                int col = GetRandom.GetRandomInt(ColsNum);
                if (row == NewIndex/ColsNum && col ==NewIndex%ColsNum) continue;
                if (!mineGrids[row, col].IsMine)
                {
                    mineGrids[row, col].IsMine = true;
                    LayedNums++;
                }
            }
        }
        #endregion

        //7.6
        #region 自定义函数GetAroundNums(MineGrid mineGrid)
        private int GetAroundNums(MineGrid mineGrid)
        {
            int row = mineGrid.Row;
            int col = mineGrid.Col;
            int aroundNums = 0;
            int minRow = row == 0 ? 0 : row - 1;
            int maxRow = row == RowsNum - 1 ? row : row + 1;
            int minCol = col == 0 ? 0 : col - 1;
            int maxCol = col == ColsNum - 1 ? col : col + 1;
            for (int i = minRow; i <= maxRow; i++)
            {
                for (int j = minCol; j <= maxCol; j++)
                {
                    if (!(i == row && j == col) && mineGrids[i, j].IsMine)
                        aroundNums++;
                }
            }
            return aroundNums;
        }
        #endregion

        //7.6
        #region 自定义的递归函数Expand(MineGrid mineGrid),是扫雷游戏的核心算法
        private void Expand(MineGrid mineGrid)
        {
            int row = mineGrid.Row;
            int col = mineGrid.Col;
            int minRow = row == 0 ? 0 : row - 1;
            int maxRow = row == RowsNum - 1 ? row : row + 1;
            int minCol = col == 0 ? 0 : col - 1;
            int maxCol = col == ColsNum - 1 ? col : col + 1;
            int aroundNums = GetAroundNums(mineGrid);
            mineGrid.State = 15 - aroundNums;
            mineGrid.OldState = 15 - aroundNums;

            if (aroundNums == 0)
            {
                for (int i = minRow; i <= maxRow; i++)
                {
                    for (int j = minCol; j <= maxCol; j++)
                    {
                        if (!(i == row && j == col) &&
                            mineGrids[i, j].State == (int)MineGridState.Normal
                            && (!mineGrids[i, j].IsMine))
                        {
                            Expand(mineGrids[i, j]);

                        }
                    }
                }
            }
        }
        #endregion

        //7.6
        #region 自定义函数，当游戏失败时，显示所有未被确定的雷的位置
        private void Dead()
        {
            gameState = (int)GameState.Dead;
            int flagsNum = 0;
            for (int row = 0; row < RowsNum; row++)
                for (int col = 0; col < ColsNum; col++)
                {
                    if (row == NewIndex / ColsNum && col == NewIndex % ColsNum) continue;
                    if (mineGrids[row, col].State == (int)MineGridState.Empty && mineGrids[row, col].OldState == (int)MineGridState.Normal)
                    {
                        mineGrids[row, col].State = (int)MineGridState.Normal;
                    }
                    if (mineGrids[row, col].IsMine && mineGrids[row, col].State == (int)MineGridState.Normal)
                    {
                        mineGrids[row, col].State = (int)MineGridState.Mine;
                        mineGrids[row, col].OldState = (int)MineGridState.Mine;
                    }
                    if (mineGrids[row, col].State == (int)MineGridState.Flag)
                        flagsNum++;
                }
            MinesNum = RowsColsNums[Difficulty * 3 + 2] - flagsNum;
            FaceIndex = (int)FaceStyle.Dead;
            //7.9
            this.GameTimer.Stop();//7.9最后添加
        }
        #endregion

        //7.6
        #region bool Victory()
        private bool Victory()
        {
            for (int row = 0; row < RowsNum; row++)
                for (int col = 0; col < ColsNum; col++)
                    if (mineGrids[row, col].State == (int)MineGridState.Normal || mineGrids[row, col].State == (int)MineGridState.Error || mineGrids[row, col].State == (int)MineGridState.Blast)
                        return false;
            return true;
        }
        #endregion

        //7.9
        #region GameTimer_Tick(object sender, EventArgs e)
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (SpendSeconds < 999)
            {
                SpendSeconds++;
                this.Invalidate();
            }
        }
        #endregion


    }
}
