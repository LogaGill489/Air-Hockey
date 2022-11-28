using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Air_Hockey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Rectangle player1 = new Rectangle(167, 50, 30, 30);
        Rectangle player2 = new Rectangle(167, 330, 30, 30);
        Rectangle goal1Rectangle = new Rectangle(210, 0, 105, 30);
        Rectangle goal2Rectangle = new Rectangle(210, 578, 105, 30);
        Rectangle goal1 = new Rectangle(185, 0, 155, 5);
        Rectangle goal2 = new Rectangle(185, 605, 155, 5);
        Rectangle puck = new Rectangle(30, 30, 15, 15);

        Brush blackBrush = new SolidBrush(Color.Black);
        Brush blueBrush = new SolidBrush(Color.SteelBlue);
        Brush redBrush = new SolidBrush(Color.Red);

        Pen linePen = new Pen(Color.Red, 3);
        Pen blueLinePen = new Pen(Color.SteelBlue, 3);
        Pen blackPen = new Pen(Color.Black, 5);

        bool wDown = false;
        bool sDown = false;
        bool aDown = false;
        bool dDown = false;
        bool upArrowDown = false;
        bool downArrowDown = false;
        bool leftArrowDown = false;
        bool rightArrowDown = false;

        int player1Speed = 4;
        int player2Speed = 4;
        int ballXSpeed = 6;
        int ballYSpeed = 6;

        //integers for player 1 slide mechanic
        int temp1X = 0;
        int temp1Y = 0;
        int temptemp1X = 0;
        int temptemp1Y = 0;

        //integers for player 2 slide mechanic
        int temp2X = 0;
        int temp2Y = 0;
        int temptemp2X = 0;
        int temptemp2Y = 0;

        int p1SpeedCounter = 0;
        int p2SpeedCounter = 0;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.A:
                    aDown = true;
                    break;
                case Keys.D:
                    dDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.A:
                    aDown = false;
                    break;
                case Keys.D:
                    dDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //center line and circle
            e.Graphics.DrawEllipse(blueLinePen, this.Width / 2 - 45, this.Height / 2 - 45, 90, 90);
            e.Graphics.DrawLine(linePen, 0, this.Height / 2, this.Width, this.Height / 2);

            //half cicles at either end
            e.Graphics.DrawArc(linePen, this.Width / 2 - 80, -83, 160, 160, 0, 180);
            e.Graphics.DrawArc(linePen, this.Width / 2 - 80, 528, 160, 160, 180, 180);

            //circle decorations on either side
            e.Graphics.DrawEllipse(linePen, 60, 130, 90, 90);
            e.Graphics.FillEllipse(redBrush, 100, 170, 10, 10);

            e.Graphics.DrawEllipse(linePen, this.Width - 150, 130, 90, 90);
            e.Graphics.FillEllipse(redBrush, this.Width - 110, 170, 10, 10);

            e.Graphics.DrawEllipse(linePen, 60, this.Height - 220, 90, 90);
            e.Graphics.FillEllipse(redBrush, 100, this.Height - 180, 10, 10);

            e.Graphics.DrawEllipse(linePen, this.Width - 150, this.Height - 220, 90, 90);
            e.Graphics.FillEllipse(redBrush, this.Width - 110, this.Height - 180, 10, 10);

            //rectangle goals within the half circles
            e.Graphics.FillRectangle(blueBrush, goal1);
            e.Graphics.FillRectangle(blueBrush, goal2);

            e.Graphics.DrawRectangle(blueLinePen, goal1Rectangle);
            e.Graphics.DrawRectangle(blueLinePen, goal2Rectangle);

            //players
            e.Graphics.DrawRectangle(blackPen, player1);
            e.Graphics.DrawRectangle(blackPen, player2);

            e.Graphics.FillRectangle(redBrush, player1);
            e.Graphics.FillRectangle(blueBrush, player2);

            //puck
            e.Graphics.FillRectangle(blackBrush, puck);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //setup to store the previous location of the players
            temp1X = player1.X;
            temp1Y = player1.Y;

            temp2X = player2.X;
            temp2Y = player2.Y;

            //adjusts speed of the player based on the speedcounter the players
            player1Speed = p1SpeedCounter / 75 + 4;
            player2Speed = p2SpeedCounter / 75 + 4;

            label1.Text = $"{p2SpeedCounter}";

            //move ball 
            puck.X += ballXSpeed;
            puck.Y += ballYSpeed;

            //move player 1 
            if (wDown == true && player1.Y > 5)
            {
                player1.Y -= player1Speed;
            }

            if (sDown == true && player1.Y < this.Height / 2 - player1.Height)
            {
                player1.Y += player1Speed;
            }

            if (aDown == true && player1.X > 3)
            {
                player1.X -= player1Speed;
            }

            if (dDown == true && player1.X < this.Width - player1.Width - 5)
            {
                player1.X += player1Speed;
            }

            //move player 2 
            if (upArrowDown == true && player2.Y > this.Height / 2 + 2)
            {
                player2.Y -= player2Speed;
            }

            if (downArrowDown == true && player2.Y < this.Height - player2.Height - 3)
            {
                player2.Y += player2Speed;
            }

            if (leftArrowDown == true && player2.X > 3)
            {
                player2.X -= player2Speed;
            }

            if (rightArrowDown == true && player2.X < this.Width - player2.Width - 5)
            {
                player2.X += player2Speed;
            }

            //check if ball hit top or bottom wall and change direction if it does 
            if (puck.Y < 0 || puck.Y > this.Height - puck.Height)
            {
                ballYSpeed *= -1;  // or: ballYSpeed = -ballYSpeed; 
            }

            //check if ball hits either player. If it does change the direction 
            //and place the ball in front of the player hit 
            if (player1.IntersectsWith(puck) && ballXSpeed < 0)
            {
                ballXSpeed *= -1;
                puck.X = player1.X + player1.Width;
            }


            if (player2.IntersectsWith(puck) && ballXSpeed < 0)
            {
                ballXSpeed *= -1;
                puck.X = player2.X + puck.Width;
            }

            //check if the ball has hit a side wall
            if (puck.X > this.Width - puck.Width)
            {
                ballXSpeed *= -1;

            }
            else if (puck.X < 0)
            {
                ballXSpeed *= -1;
            }


            #region player 1 speed increase and slide mechanic

            //adjust speed of player 1 for increases
            if (temp1X != player1.X || temp1Y != player1.Y)
            {

                if (p1SpeedCounter < 150)
                {
                    p1SpeedCounter++;

                    temptemp1X = temp1X - (player1.X - temp1X);
                    temptemp1Y = temp1Y - (player1.Y - temp1Y);
                }
            }
            else if (p1SpeedCounter > 0)
            {
                p1SpeedCounter--;

                //add a sliding mechanic to the player using different directions
                if (temp1X - temptemp1X > 0 && temp1Y - temptemp1Y > 0)
                {
                    player1.X += player1Speed - 3;
                    player1.Y += player1Speed - 3;
                }
                else if (temp1X - temptemp1X < 0 && temp1Y - temptemp1Y < 0)
                {
                    player1.X -= player1Speed - 3;
                    player1.Y -= player1Speed - 3;
                }
                else if (temp1X - temptemp1X < 0 && temp1Y - temptemp1Y > 0)
                {
                    player1.X -= player1Speed - 3;
                    player1.Y += player1Speed - 3;
                }
                else if (temp1X - temptemp1X > 0 && temp1Y - temptemp1Y < 0)
                {
                    player1.X += player1Speed - 3;
                    player1.Y -= player1Speed - 3;
                }
                else if (temp1X - temptemp1X > 0)
                {
                    player1.X += player1Speed - 3;
                }
                else if (temp1X - temptemp1X < 0)
                {
                    player1.X -= player1Speed - 3;
                }
                else if (temp1Y - temptemp1Y > 0)
                {
                    player1.Y += player1Speed - 3;
                }
                else if (temp1Y + temptemp1Y > 0)
                {
                    player1.Y -= player1Speed - 3;
                }

                //stops player 1 from sliding out of bounds
                if (player1.X < 3)
                {
                    p1Reset();
                    player1.X = 3;
                }
                if (player1.X > this.Width - player1.Width - 5)
                {
                    p1Reset();
                    player1.X = this.Width - player1.Width - 5;
                }
                if (player1.Y < 5)
                {
                    p1Reset();
                    player1.Y = 5;
                }
                if (player1.Y > this.Height / 2 - player1.Height)
                {
                    p1Reset();
                    player1.Y = this.Height / 2 - player1.Height;
                }
            }

            #endregion

            #region player 2 speed increase and slide mechanic

            //adjust speed of player 2 for increases
            if (temp2X != player2.X || temp2Y != player2.Y)
            {

                if (p2SpeedCounter < 150)
                {
                    p2SpeedCounter++;

                    temptemp2X = temp2X - (player2.X - temp2X);
                    temptemp2Y = temp2Y - (player2.Y - temp2Y);
                }
            }
            else if (p2SpeedCounter > 0)
            {
                p2SpeedCounter--;

                //add a sliding mechanic to the player using different directions
                if (temp2X - temptemp2X > 0 && temp2Y - temptemp2Y > 0)
                {
                    player2.X += player2Speed - 3;
                    player2.Y += player2Speed - 3;
                }
                else if (temp2X - temptemp2X < 0 && temp2Y - temptemp2Y < 0)
                {
                    player2.X -= player2Speed - 3;
                    player2.Y -= player2Speed - 3;
                }
                else if (temp2X - temptemp2X < 0 && temp2Y - temptemp2Y > 0)
                {
                    player2.X -= player2Speed - 3;
                    player2.Y += player2Speed - 3;
                }
                else if (temp2X - temptemp2X > 0 && temp2Y - temptemp2Y < 0)
                {
                    player2.X += player2Speed - 3;
                    player2.Y -= player2Speed - 3;
                }
                else if (temp2X - temptemp2X > 0)
                {
                    player2.X += player2Speed - 3;
                }
                else if (temp2X - temptemp2X < 0)
                {
                    player2.X -= player2Speed - 3;
                }
                else if (temp2Y - temptemp2Y > 0)
                {
                    player2.Y += player2Speed - 3;
                }
                else if (temp2Y + temptemp2Y > 0)
                {
                    player2.Y -= player2Speed - 3;
                }

                //stops player 2 from sliding out of bounds
                if (player2.X < 3)
                {
                    p2Reset();
                    player2.X = 3;
                }
                if (player2.X > this.Width - player2.Width - 5)
                {
                    p2Reset();
                    player2.X = this.Width - player2.Width - 5;
                }
                if (player2.Y < this.Height / 2 + 2)
                {
                    p2Reset();
                    player2.Y = this.Height / 2 + 2;
                }
                if (player2.Y > this.Height - player2.Height - 3)
                {
                    p2Reset();
                    player2.Y = this.Height - player2.Height - 3;
                }
            }
            #endregion

            if (puck.IntersectsWith(goal1))
            {

            }
            else if (puck.IntersectsWith(goal2))
            {

            }

            Refresh();
        }


        //quick reset for each individual player speed
            public void p1Reset()
            {
                player1Speed = 4;
                p1SpeedCounter = 0;
            }

            public void p2Reset()
            {
                player2Speed = 4;
                p2SpeedCounter = 0;
            }
    }
}
