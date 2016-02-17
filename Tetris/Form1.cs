﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Tetris
{
    public interface IMainForm
    {
        Bitmap Image { get; set; }

        Bitmap RImage { get; set; }
 
    }
    public partial class Form1 : Form,IMainForm
    {
        Block block;
        Block Nextblock;
        public Form1()
        {
            InitializeComponent();

            BlockSize = 35;//1 cm

            Draft = new Bitmap(Sheet.Width, Sheet.Height);
            DraftForNextBlock = new Bitmap(RandomBSheet.Width, RandomBSheet.Height);
            painter = new Painter(Draft, BlockSize);
            PainterForNextBlock = new Painter(DraftForNextBlock, BlockSize);


            board = new Board(4,Sheet.Height / BlockSize, Sheet.Width / BlockSize);
            BoardForNextBlock = new Board(RandomBSheet.Height / BlockSize, RandomBSheet.Width / BlockSize);


            Block.ArrivedAtBottom += Board_ArrivedAtBottom;
            Board.FullLine += Board_FullLine;
            Block.GameOver += Block_GameOver;

            StartPoint = new Point(3, 0);
            RandomBlock = rand.Next(0, 7);

            blocks = Enumerable.Range(0, 7).ToArray();
            Shuffle();
            IndexOfNextBlock = RandomBlock;

            block = new Block(blocks[RandomBlock], StartPoint,board);
            Nextblock = new Block(blocks[IndexOfNextBlock], PositionOfNextBlock, board);

            IndexOfNextBlock++;
            ShowNextBlock();

            Level = 1;
            Speed = speed;
            timer1.Enabled = true;
            

        }
      
        private void Block_GameOver()
        {
            Speed = 0;
           // painter.Clear();
            board.Clear();

            painter.PrintGameOver();

            Image = Draft;
        }

        private void Board_FullLine(int index)
        {
            FullLinesCounter = FullLinesCounter + 1;
            if (FullLinesCounter==10)
            {
                Level = 2;
                speed = 200;
                accelerate = 5;
                incrementScore = new Point(25, 35);
            }
            if(FullLinesCounter == 20)
            {
                Level = 3;
                speed = 100;
                accelerate = 1;
                incrementScore = new Point(35, 100);
            }
            Speed = speed;
  
            board.MoveValues(index);
            
            Score = Score + incrementScore.Y;
            board.DrawBlocks(Draft,BlockSize);
            Image = Draft;
        }
        #region variables

        static int speed=400;
        static int accelerate=10;
        static Point incrementScore = new Point(15, 25);
        int RandomBlock;
        static Point PositionOfNextBlock = new Point(0, 0);

        Bitmap Draft;
        Bitmap DraftForNextBlock;

        Painter painter;
        Painter PainterForNextBlock;
        Board board;
        Board BoardForNextBlock;

        static int BlockSize;

        Random rand = new Random();
        Point StartPoint;

        static int[] blocks;
        #endregion

        int indexOfNextBlock;

        #region methods

        void ShowNextBlock()
        {
            Nextblock = new Block(blocks[IndexOfNextBlock], PositionOfNextBlock, board);
            BoardForNextBlock.Clear();
            BoardForNextBlock.SetValue(Nextblock.d, Nextblock.skeleton, Nextblock.color);
            PainterForNextBlock.Clear();
            BoardForNextBlock.DrawBlocks(DraftForNextBlock, BlockSize);
            PainterForNextBlock.DrawArea();

            RImage = DraftForNextBlock;
           // timer1.Start();
        }


        public static void Shuffle()
        {
            Random rng = new Random();
            int n = blocks.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = blocks[k];
                blocks[k] = blocks[n];
                blocks[n] = value;
            }
        }

        public Bitmap Image
        {
            get
            {
                return (Bitmap)Sheet.Image;
            }
            set
            {
                Sheet.Image = value;
            }
        }

        public Bitmap RImage
        {
            get
            {
                return (Bitmap)RandomBSheet.Image;
            }
            set
            {
                RandomBSheet.Image = value;
            }
        }

        int IndexOfNextBlock
        {
            get
            {
                return indexOfNextBlock;
            }
            set
            {
                if ((value >= 0) && (value < 7))
                {
                    indexOfNextBlock = value;
                }
                else
                {
                    indexOfNextBlock = 0;
                    Shuffle();
                }
            }
        }

        int Speed
        {
            get
            {
                if(timer1.Enabled==false)
                {
                    return 0;
                }
                return timer1.Interval;
            }
            set
            {
                if (value == 0)
                {
                    if (timer1.Enabled != false)
                    timer1.Enabled = false;
                }
                else
                {
                    if (timer1.Enabled != false)
                        timer1.Enabled = false;
                    timer1.Interval = value;
                    timer1.Enabled = true;
                }
            }
        }

        int Score
        {
            get
            {
                return Convert.ToInt32(Scorelabel.Text);
            }
            set
            {
                Scorelabel.Text = value.ToString();
            }
        }

        int FullLinesCounter
        {
            get
            {
                return Int32.Parse(Lineslabel.Text);
            }
            set
            {
                Lineslabel.Text = value.ToString();
            }
        }
        int Level
        {
            get
            {
                return Convert.ToInt32(Levellabel.Text);
            }
            set
            {
                Levellabel.Text = value.ToString();
            }
        }

        #endregion

        #region events

        private void timer1_Tick(object sender, EventArgs e)
        {
            painter.Clear();

            block.MoveToDown();

            board.DrawBlocks(Draft, BlockSize);

            painter.DrawArea();
            Image = Draft;

        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (Speed != 0)
            {
               // MessageBox.Show(e.KeyCode.ToString());
                switch (e.KeyCode.ToString())
                {
                    case "Left":
                        painter.Clear();

                        block.MoveToLeft();

                        board.DrawBlocks(Draft, BlockSize);

                        painter.DrawArea();
                        Image = Draft;
                        break;

                    case "Right":
                        painter.Clear();

                        block.MoveToRight();
                        board.DrawBlocks(Draft, BlockSize);

                        painter.DrawArea();
                        Image = Draft;
                        break;
                    case "Up":
                        painter.Clear();

                        try
                        {
                            block.Rotate();
                        }
                        catch
                        {
                            ;//
                        }

                        board.DrawBlocks(Draft, BlockSize);

                        painter.DrawArea();
                        Image = Draft;
                        break;
                    case "Down":

                        Speed = speed;
                        break;
                    
                }
            }
            switch(e.KeyCode.ToString())
            {
                case "R"://Restart

                    board.Clear();
                    Score = 0;
                    FullLinesCounter = 0;
                    Level = 1;
                    speed = 400;
                    accelerate = 10;
                    incrementScore = new Point(15, 25);

                    blocks = Enumerable.Range(0, 7).ToArray();
                    Shuffle();
                    IndexOfNextBlock = RandomBlock;

                    block = new Block(blocks[RandomBlock], StartPoint, board);
                    Nextblock = new Block(blocks[IndexOfNextBlock], PositionOfNextBlock, board);

                    IndexOfNextBlock++;
                    ShowNextBlock();

                    Speed = speed;
                    timer1.Enabled = true;
                    break;

                case "P":
                    if(Speed!=0)
                    {
                        Speed = 0;
                    }
                    else
                    {
                        Speed = speed;
                    }
                    break;
            }
        }

        private void Board_ArrivedAtBottom()
        {
           // Speed = 0;

            Score = Score + incrementScore.X;
            board.CheckFullLines();

            block = new Block(blocks[IndexOfNextBlock], StartPoint, board);
            IndexOfNextBlock++;
            ShowNextBlock();


            // timer1.Start();
           // Speed = 400;
            //timer1.Enabled = true;
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode.ToString() == "Down")&&(Speed!=0))
            {
                Speed = accelerate;
            }
        }

        #endregion

    }
}
