using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Numerics;

/*
 * Air Hockey Mini-Game
 * Logan Gillett
 * Mr. T
 * 28.11.22
 * ICS3U
 */

namespace Air_Hockey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Variables

        //sounds
        SoundPlayer hitNoise = new SoundPlayer(Properties.Resources.hittingNoise);
        SoundPlayer scoreNoise = new SoundPlayer(Properties.Resources.scoringNoise);

        //player visual rectangles
        Rectangle player1 = new Rectangle(237, 150, 30, 30);
        Rectangle player2 = new Rectangle(237, 420, 30, 30);

        //goal rectangles
        Rectangle goal1Rectangle = new Rectangle(210, 0, 105, 30);
        Rectangle goal2Rectangle = new Rectangle(210, 578, 105, 30);
        Rectangle goal1 = new Rectangle(185, 0, 155, 5);
        Rectangle goal2 = new Rectangle(185, 605, 155, 5);

        //player rectangles for ball physics
        //player 1 rectangles
        Rectangle p1Up = new Rectangle(0, 0, 32, 4);
        Rectangle p1Down = new Rectangle(0, 0, 32, 4);
        Rectangle p1Left = new Rectangle(0, 0, 4, 32);
        Rectangle p1Right = new Rectangle(0, 0, 4, 32);

        //player 2 rectangles
        Rectangle p2Up = new Rectangle(0, 0, 32, 4);
        Rectangle p2Down = new Rectangle(0, 0, 32, 4);
        Rectangle p2Left = new Rectangle(0, 0, 4, 32);
        Rectangle p2Right = new Rectangle(0, 0, 4, 32);

        //puck rectangle
        Rectangle puck = new Rectangle(243, 285, 15, 15);

        //brushes for colouring the puck and players
        Brush blackBrush = new SolidBrush(Color.Black);
        Brush blueBrush = new SolidBrush(Color.SteelBlue);
        Brush redBrush = new SolidBrush(Color.Red);
        Brush transparentBrush = new SolidBrush(Color.Transparent);

        //pens for colouring the arena
        Pen linePen = new Pen(Color.Red, 3);
        Pen blueLinePen = new Pen(Color.SteelBlue, 3);
        Pen blackPen = new Pen(Color.Black, 5);

        //controls for each individual player
        //player 1
        bool wDown = false;
        bool sDown = false;
        bool aDown = false;
        bool dDown = false;

        //player 2
        bool upArrowDown = false;
        bool downArrowDown = false;
        bool leftArrowDown = false;
        bool rightArrowDown = false;

        //player and puck speeds
        int player1Speed = 4;
        int player2Speed = 4;
        int puckXSpeed = 6;
        int puckYSpeed = 6;

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

        //variables that find the center of each circle
        int puckCenterX;
        int puckCenterY;

        int player1CenterX;
        int player1CenterY;

        int player2CenterX;
        int player2CenterY;

        double playerRise;
        double playerRun;
        double playerTheta;

        double puckRise;
        double puckRun;
        double puckTheta;

        double differenceTheta;

        

        //temporary variable to store the pucks previous position
        int tempPuckX = 0;
        int tempPuckY = 0;

        //adjustable speed counter that directly affects the speed of each player among other uses
        int p1SpeedCounter = 0;
        int p2SpeedCounter = 0;

        //scoring variables
        int resetPuck = 0;

        //player score variables
        int player1Score = 0;
        int player2Score = 0;


        #endregion

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //switch block to check for button imputs
            //adjusts player movement based upon the respective input
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
            //switch block to check for button releases
            //stops movement of the respective variable upon release
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
            //e.Graphics.DrawEllipse(blackPen, player1);
            //e.Graphics.DrawEllipse(blackPen, player2);

            e.Graphics.DrawRectangle(blackPen, player1);
            e.Graphics.DrawRectangle(blackPen, player2);

            //e.Graphics.FillEllipse(redBrush, player1);
            //e.Graphics.FillEllipse(blueBrush, player2);

            e.Graphics.FillRectangle(redBrush, player1);
            e.Graphics.FillRectangle(blueBrush, player2);

            //rectangles within the visual rectangle of player 1
            e.Graphics.FillRectangle(transparentBrush, p1Up);
            e.Graphics.FillRectangle(transparentBrush, p1Down);
            e.Graphics.FillRectangle(transparentBrush, p1Left);
            e.Graphics.FillRectangle(transparentBrush, p1Right);

            //rectangles within the visual rectangle of player 2
            e.Graphics.FillRectangle(transparentBrush, p2Up);
            e.Graphics.FillRectangle(transparentBrush, p2Down);
            e.Graphics.FillRectangle(transparentBrush, p2Left);
            e.Graphics.FillRectangle(transparentBrush, p2Right);

            //puck
            //e.Graphics.FillEllipse(blackBrush, puck);
            e.Graphics.FillRectangle(blackBrush, puck);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {

            //puck reset after a goal feature
            if (resetPuck == 1 && player1.IntersectsWith(puck) || resetPuck == 1 && player2.IntersectsWith(puck))
            {
                resetPuck = 0;
            }

            //setup to store the previous location of the players
            temp1X = player1.X;
            temp1Y = player1.Y;

            temp2X = player2.X;
            temp2Y = player2.Y;

            //setup to store previous location of the puck
            tempPuckX = puck.X;
            tempPuckY = puck.Y;

            //adjusts speed of the player based on the speedcounter the players
            player1Speed = p1SpeedCounter / 75 + 4;
            player2Speed = p2SpeedCounter / 75 + 4;

            //moves  puck 
            if (resetPuck == 0)
            {
                puck.X += puckXSpeed;
                puck.Y += puckYSpeed;
            }

            //moves  player 1 
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

            //moves  player 2 
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

            //check if puck hit top or bottom wall and change direction if it does 
            if (puck.Y < 0 || puck.Y > this.Height - puck.Height)
            {
                puckYSpeed *= -1;  // or: puckYSpeed = -puckYSpeed; 
            }

            #region old intersection mechanics

            //player 1 box intersection mechanics
            if (p1Up.IntersectsWith(puck))
            {
                puckYSpeed *= -1;
                puck.Y = player1.Y - puck.Height - 2;
                hitNoise.Play();
                Refresh();
            }
            else if (p1Down.IntersectsWith(puck))
            {
                puckYSpeed *= -1;
                puck.Y = player1.Y + puck.Height + 20;
                hitNoise.Play();
                Refresh();
            }
            else if (p1Right.IntersectsWith(puck))
            {
                puckXSpeed *= -1;
                puck.X = player1.X + puck.Width + 16;
                hitNoise.Play();
                Refresh();
            }
            else if (p1Left.IntersectsWith(puck))
            {
                puckXSpeed *= -1;
                puck.X = player1.X - puck.Width - 2;
                hitNoise.Play();
                Refresh();
            }

            //player 2 box intersection mechanics
            if (p2Up.IntersectsWith(puck))
            {
                puckYSpeed *= -1;
                puck.Y = player2.Y - puck.Height - 2;
                hitNoise.Play();
                Refresh();
            }
            else if (p2Down.IntersectsWith(puck))
            {
                puckYSpeed *= -1;
                puck.Y = player2.Y + puck.Height + 20;
                hitNoise.Play();
                Refresh();
            }
            else if (p2Right.IntersectsWith(puck))
            {
                puckXSpeed *= -1;
                puck.X = player2.X + puck.Width + 26;
                hitNoise.Play();
                Refresh();
            }
            else if (p2Left.IntersectsWith(puck))
            {
                puckXSpeed *= -1;
                puck.X = player2.X - puck.Width - 2;
                hitNoise.Play();
                Refresh();
            }

            #endregion

            #region new intersection mechanics

            //puckCenterX = puck.X + 10;
            //puckCenterY = puck.Y + 10;

            //player1CenterX = player1.X + 25;
            //player1CenterY = player1.Y + 25;

            //player2CenterX = player2.X + 25;
            //player2CenterY = player2.Y + 25;

            ////player 1
            //if (puckCenterX < player1CenterX && puckCenterY < player1CenterY) //top left
            //{
            //    double distance = Math.Sqrt(Math.Pow(puckCenterX - player1CenterX, 2) + Math.Pow(puckCenterY - player1CenterY, 2));

            //    if (distance < puck.Width / 2 + player1.Width / 2)
            //    {
            //        //calulating player theta
            //        playerRise = player1CenterY - puck.Y;
            //        playerRun = player1CenterX - puck.X;

            //        playerTheta = Math.Atan(playerRise / playerRun) * 180 / Math.PI;
            //        //player1ScoreLabel.Text = $"{playerTheta}";

            //        playerTheta += 90;

            //        //calculating puck theta
            //        puckRise = puck.Y - tempPuckY;
            //        puckRun = puck.X - tempPuckX;

            //        puckTheta = Math.Atan(puckRise / puckRun) * 180 / Math.PI;

            //        differenceTheta = playerTheta - puckTheta;
            //        differenceTheta = 180 - differenceTheta;

            //        player1ScoreLabel.Text = $"{puckTheta}";

            //    }
            //}
            //if (puckCenterX > player1CenterX && puckCenterY > player1CenterY) //bottom right
            //{

            //}
            //if (puckCenterX > player1CenterX && puckCenterY < player1CenterY) //top right
            //{

            //}
            //if (puckCenterX < player1CenterX && puckCenterY > player1CenterY) //bottom left
            //{

            //}

            #endregion

            #region Side Wall Collision
            //stops puck from sliding outside of the map
            if (puck.Y < this.Height / 2)
            {
                if (puck.X < 0)
                {
                    puck.X += 4;
                    player1.X += 4;
                }
                else if (puck.X > this.Width)
                {
                    puck.X -= 4;
                    player1.X -= 4;
                }
                else if (puck.Y < 0)
                {
                    puck.Y += 4;
                    player1.Y += 4;
                }
            }
            else
            {
                if (puck.X < 0)
                {
                    puck.X += 4;
                    player2.X += 4;
                }
                else if (puck.X > this.Width)
                {
                    puck.X -= 4;
                    player2.X -= 4;
                }
                else if (puck.Y > this.Height)
                {
                    puck.Y -= 5;
                    player2.Y -= 5;
                }
            }

            //check if the puck has hit a side wall
            if (puck.X > this.Width - puck.Width)
            {
                puckXSpeed *= -1;
            }

            else if (puck.X < 0)
            {
                puckXSpeed *= -1;
            }

            #endregion

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

            #region Goals and Scoring

            //checks to see if the puck has hit either goal
            if (puck.IntersectsWith(goal1))
            {
                gameReset();
                puck.Y = 210;

                player2Score++;
                player2ScoreLabel.Text = $"{player2Score}";
            }
            else if (puck.IntersectsWith(goal2))
            {
                gameReset();
                puck.Y = 380;

                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";
            }

            //checks to see if either player is at 3 points
            if (player1Score >= 3)
            {
                //stops the timer and displays the win screen
                gameTimer.Stop();
                winLabel.Text = "Player 1 Wins";

                //shows the reset button
                restartButton.Visible = true;
            }
            if (player2Score >= 3)
            {
                //stops the timer and displays the win screen
                gameTimer.Stop();
                winLabel.Text = "Player 2 Wins";

                //shows the reset button
                restartButton.Visible = true;
            }

            #endregion

            //Player 1 rectangle followers
            p1Up.X = player1.X + 1;
            p1Up.Y = player1.Y - 2;

            p1Down.X = player1.X + 1;
            p1Down.Y = player1.Y + 28;

            p1Left.X = player1.X - 2;
            p1Left.Y = player1.Y;

            p1Right.X = player1.X + 29;
            p1Right.Y = player1.Y;

            //Player 2 rectangle followers
            p2Up.X = player2.X + 1;
            p2Up.Y = player2.Y - 2;

            p2Down.X = player2.X + 1;
            p2Down.Y = player2.Y + 28;

            p2Left.X = player2.X - 2;
            p2Left.Y = player2.Y;

            p2Right.X = player2.X + 29;
            p2Right.Y = player2.Y;

            //Refresh of the entire screen
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

        //repeatable code for the reset of a game after a goal is scored
        public void gameReset()
        {
            //adjusts reset variable to track if a new play has been started
            resetPuck = 1;

            //resets each players speed counts to avoid sliding and starting speed boosts
            p1SpeedCounter = 0;
            p2SpeedCounter = 0;

            //resets player positions and pucks x position
            player1.X = 247;
            player1.Y = 150;

            player2.X = 247;
            player2.Y = 420;

            puck.X = 255;

            scoreNoise.Play();
            Refresh();
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            //disables the reset button until visible again
            restartButton.Visible = false;

            //starts up the timer again
            gameTimer.Start();

            //resets each variable and displayable
            player1Score = 0;
            player2Score = 0;

            player1ScoreLabel.Text = "0";
            player2ScoreLabel.Text = "0";
            winLabel.Text = "";

            //necessary for a button press to be used
            this.Focus();
        }
    }
}