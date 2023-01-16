using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace flipperGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer GameAnimationTimer = new DispatcherTimer();
        private readonly DispatcherTimer AfterGameDelayTimer = new DispatcherTimer();

        private int afterGameDelayCounter;

        private bool ballMovementDirectionRight;
        private bool ballMovementDirectionLeft;
        private bool ballMovementDirectionDown;
        private bool ballMovementDirectionUp;

        private bool player1MovementDirectionDown;
        private bool player1MovementDirectionUp;

        private bool player2MovementDirectionDown;
        private bool player2MovementDirectionUp;

        private bool servePlayer1IsEnabled;
        private bool servePlayer2IsEnabled;

        //changeable values://////////////////////////////////////////////////////////////////
        private double ballMovementPerAnimationTick = 8.00; //only positive double values   //
        private double playerMovementPerAnimationTick = 5.00; //only positive double values //
                                                                                            //
        private int playingTimeBestOf = 10; //only positive + >2 int values                 //
        //////////////////////////////////////////////////////////////////////////////////////

        private double CurrentBallPositionX;
        private double CurrentBallPositionY;
        
        private double CurrentPlayer1PositionY;
        //double CurrentPlayer1PositionX;
        private double CurrentPlayer2PositionY;
        //double CurrentPlayer2PositionX;

        private int scorePlayer1;
        private int scorePlayer2;

        public MainWindow()
        {
            InitializeComponent();
            
            GameAnimationTimer.Interval = TimeSpan.FromMilliseconds(10);
            GameAnimationTimer.Tick += GameAnimationTimer_Tick;

            AfterGameDelayTimer.Interval = TimeSpan.FromMilliseconds(100);
            AfterGameDelayTimer.Tick += AfterGameDelayTimer_Tick;
        }

        private void AfterGameDelayTimer_Tick(object sender, EventArgs e)
        {
            afterGameDelayCounter++;

            if (afterGameDelayCounter >= 300)
            {
                Reset();
                Bt_Reset.IsEnabled = false;
                afterGameDelayCounter = 0;
                AfterGameDelayTimer.Stop();
            }
        }

        private void GameAnimationTimer_Tick(object sender, EventArgs e)
        {
            CurrentBallPositionX = Canvas.GetLeft(Ball);
            CurrentBallPositionY = Canvas.GetTop(Ball);

            CurrentPlayer1PositionY = Canvas.GetTop(PlayerLeft);
            CurrentPlayer2PositionY = Canvas.GetTop(PlayerRight);

            //BallMovement on X
            BallMovementX();
            //BallMovement on Y
            BallMovementY();
            //GameTableBorderDefinition: Up & Down
            GameTableBorderDefinitionY();

            //PlayerMovement
            Player1Movement();
            Player2Movement();

            BallMovementWhileServe();

            //player1 blocks ball Event
            BallgetsBlockedByPlayer1();

            //player2 blocks ball Event
            BallgetsBlockedByPlayer2();

            //GameEnd Event
            GameEndEvent();
        }

        private void MainWindow_KeyDownBinds(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    player2MovementDirectionDown = true;
                    break;
                case Key.Up:
                    player2MovementDirectionUp = true;
                    break;
                case Key.S:
                    player1MovementDirectionDown = true;
                    break;
                case Key.W:
                    player1MovementDirectionUp = true;
                    break;
            }
                
        }

        private void MainWindow_KeyUpBinds(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Down:
                    player2MovementDirectionDown = false;
                    break;
                case Key.Up:
                    player2MovementDirectionUp = false;
                    break;
                case Key.S:
                    player1MovementDirectionDown = false;
                    break;
                case Key.W:
                    player1MovementDirectionUp = false;
                    break;
            }

            if (servePlayer1IsEnabled)
            {
                switch (e.Key)
                {
                    //player1Serve with direction down
                    case Key.LeftCtrl:
                        ballMovementDirectionUp = false;
                        ballMovementDirectionDown = true;
                        ballMovementDirectionLeft = false;
                        ballMovementDirectionRight = true;

                        servePlayer1IsEnabled = false;

                        Lbl_Tip.Visibility = Visibility.Hidden;
                        break;
                    //player1Serve with direction up
                    case Key.LeftShift:
                        ballMovementDirectionDown = false;
                        ballMovementDirectionUp = true;
                        ballMovementDirectionLeft = false;
                        ballMovementDirectionRight = true;

                        servePlayer1IsEnabled = false;

                        Lbl_Tip.Visibility = Visibility.Hidden;
                        break;
                }
            }

            if (servePlayer2IsEnabled)
            {
                switch (e.Key)
                {
                    //player2Serve with direction down
                    case Key.RightCtrl:
                        ballMovementDirectionUp = false;
                        ballMovementDirectionDown = true;
                        ballMovementDirectionRight = false;
                        ballMovementDirectionLeft = true;

                        servePlayer2IsEnabled = false;

                        Lbl_Tip.Visibility = Visibility.Hidden;
                        break;
                    //player2Serve with direction up
                    case Key.RightShift:
                        ballMovementDirectionDown = false;
                        ballMovementDirectionUp = true;
                        ballMovementDirectionRight = false;
                        ballMovementDirectionLeft = true;

                        servePlayer2IsEnabled = false;

                        Lbl_Tip.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }

        private void Bt_Reset_Click(object sender, RoutedEventArgs e)
        {
            RB_player1Begins.IsEnabled = true;
            RB_player2Begins.IsEnabled = true;

            Reset();

            Bt_Reset.IsEnabled = false;
        }

        private double CanvasGameTableMidPlayerY()
        {
            return CanvasGameTableMidY() - PlayerMidY();
        }

        private double CanvasGameTableMidX()
        {
            return CanvasGameTable.ActualWidth / 2;
        }

        private double CanvasGameTableMidY()
        {
            return CanvasGameTable.ActualHeight / 2;
        }

        private double PlayerMidY()
        {
            return PlayerLeft.ActualHeight / 2;
        }

        private double PlayerMidX()
        {
            return PlayerLeft.ActualWidth / 2;
        }

        private double BallMid()
        {
            return Ball.ActualWidth / 2;
        }

        private double Lbl_TipMidX()
        {
            return Lbl_Tip.ActualWidth / 2;
        }

        private double Lbl_TipMidY()
        {
            return Lbl_Tip.ActualHeight / 2;
        }

        private void Bt_Info_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Reset()
        {
            GameAnimationTimer.Stop();
            ScoreReset();
            BallStopsMoving();
            RB_player1Begins.IsChecked = false;
            RB_player2Begins.IsChecked = false;
            RB_player1Begins.IsEnabled = true;
            RB_player2Begins.IsEnabled = true;
            Ball.Visibility = Visibility.Hidden;
            Canvas.SetLeft(PlayerLeft, 0);
            Canvas.SetTop(PlayerLeft, CanvasGameTableMidPlayerY());

            Canvas.SetLeft(PlayerRight, CanvasGameTable.ActualWidth - PlayerRight.ActualWidth);
            Canvas.SetTop(PlayerRight, CanvasGameTableMidPlayerY());

            Lbl_Tip.Content = "Choose the player to begin";
            Lbl_TipMidPosition();
        }

        private void ScoreReset()
        {
            scorePlayer1 = 0;
            scorePlayer2 = 0;
            RefreshScore();
        }

        private void RB_player1Begins_Checked(object sender, RoutedEventArgs e)
        {
            //Ball.Visibility = Visibility.Visible;
            Canvas.SetLeft(Ball, Canvas.GetLeft(PlayerLeft) + PlayerLeft.ActualWidth);
            Canvas.SetTop(Ball, Canvas.GetTop(PlayerLeft) + PlayerMidY() - BallMid());

            Bt_StartGame.IsEnabled = true;
            Bt_Reset.IsEnabled = true;

            Lbl_Tip.Content = "Click \"Play\" to start playing";
            Lbl_TipMidPosition();
        }

        private void RB_player2Begins_Checked(object sender, RoutedEventArgs e)
        {
            //Ball.Visibility = Visibility.Visible;
            Canvas.SetLeft(Ball, Canvas.GetLeft(PlayerRight) - Ball.ActualWidth);
            Canvas.SetTop(Ball, Canvas.GetTop(PlayerRight) + PlayerMidY() - BallMid());

            Bt_StartGame.IsEnabled = true;
            Bt_Reset.IsEnabled = true;

            Lbl_Tip.Content = "Click \"Play\" to start playing";
            Lbl_TipMidPosition();
        }

        private void Bt_StartGame_Click(object sender, RoutedEventArgs e)
        {
            RB_player1Begins.IsEnabled = false;
            RB_player2Begins.IsEnabled = false;

            if (RB_player1Begins.IsChecked == true)
            {
                servePlayer1IsEnabled = true;
                Lbl_Tip.Content = "Press \"LShift\" to serve the ball up!\nor\nPress \"LCtrl\" to serve the ball down!\n\nYou can choose the serve position by moving up and down with \"W\" and \"S\"!";
            }
            else if (RB_player2Begins.IsChecked == true)
            {
                servePlayer2IsEnabled = true;
                Lbl_Tip.Content = "Press \"RShift\" to serve the ball up!\nor\nPress \"RCtrl\" to serve the ball down!\n\nYou can choose the serve position by moving up and down with \"Up\" and \"Down\"!";
            }
            Lbl_TipMidPosition();

            Ball.Visibility = Visibility.Visible;

            GameAnimationTimer.Start();

            Bt_StartGame.IsEnabled = false;

            
        }

        private void BallMovementX()
        {
            if (ballMovementDirectionRight)
            {
                if (CanvasGameTable.ActualWidth - CurrentBallPositionX >= ballMovementPerAnimationTick)
                {
                    Canvas.SetLeft(Ball, CurrentBallPositionX + ballMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetLeft(Ball, CanvasGameTable.ActualWidth - Ball.ActualWidth);
                }
            }

            if (ballMovementDirectionLeft)
            {
                if (CurrentBallPositionX >= ballMovementPerAnimationTick)
                {
                    Canvas.SetLeft(Ball, CurrentBallPositionX - ballMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetLeft(Ball, 0);
                }
            }
        }

        private void BallMovementY()
        {
            if (ballMovementDirectionDown)
            {
                if (CanvasGameTable.ActualHeight - CurrentBallPositionY >= ballMovementPerAnimationTick)
                {
                    Canvas.SetTop(Ball, CurrentBallPositionY + ballMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(Ball, CanvasGameTable.ActualHeight - Ball.ActualHeight);
                }
            }

            if (ballMovementDirectionUp)
            {
                if (CurrentBallPositionY >= ballMovementPerAnimationTick)
                {
                    Canvas.SetTop(Ball, CurrentBallPositionY - ballMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(Ball, 0);
                }
            }
        }

        private void Player1Movement()
        {
            if (player1MovementDirectionDown && CurrentPlayer1PositionY < CanvasGameTable.ActualHeight - PlayerLeft.ActualHeight)
            {
                if (CanvasGameTable.ActualHeight - CurrentPlayer1PositionY >= playerMovementPerAnimationTick)
                {
                    Canvas.SetTop(PlayerLeft, CurrentPlayer1PositionY + playerMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(PlayerLeft, CanvasGameTable.ActualHeight - CurrentPlayer1PositionY);
                }
            }

            if (player1MovementDirectionUp && CurrentPlayer1PositionY > 0)
            {
                if (CurrentPlayer1PositionY >= playerMovementPerAnimationTick)
                {
                    Canvas.SetTop(PlayerLeft, CurrentPlayer1PositionY - playerMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(PlayerLeft, 0);
                }
            }
        }

        private void Player2Movement()
        {
            if (player2MovementDirectionDown && CurrentPlayer2PositionY < CanvasGameTable.ActualHeight - PlayerRight.ActualHeight)
            {
                if (CanvasGameTable.ActualHeight - CurrentPlayer2PositionY >= playerMovementPerAnimationTick)
                {
                    Canvas.SetTop(PlayerRight, CurrentPlayer2PositionY + playerMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(PlayerRight, CanvasGameTable.ActualHeight - CurrentPlayer2PositionY);
                }
            }

            if (player2MovementDirectionUp && CurrentPlayer2PositionY > 0)
            {
                if (CurrentPlayer2PositionY >= playerMovementPerAnimationTick)
                {
                    Canvas.SetTop(PlayerRight, CurrentPlayer2PositionY - playerMovementPerAnimationTick);
                }
                else
                {
                    Canvas.SetTop(PlayerRight, 0);
                }
            }
        }

        private void Player1HasScoredEvent()
        {
            scorePlayer1++;
            RefreshScore();
            BallStopsMoving();
            PlayerPositionReset();
            Player2GetsBall();
            Ball.Visibility = Visibility.Visible;
            //ready for player2Serve
            servePlayer2IsEnabled = true;

            Lbl_Tip.Content = "Press \"RShift\" to serve the ball up!\nor\nPress \"RCtrl\" to serve the ball down!\n\nYou can choose the serve position by moving up and down with \"Up\" and \"Down\"!";
            Lbl_TipMidPosition();
        }

        private void Player2HasScoredEvent()
        {
            scorePlayer2++;
            RefreshScore();
            BallStopsMoving();
            PlayerPositionReset();
            Player1GetsBall();
            Ball.Visibility = Visibility.Visible;
            //ready for player1Serve
            servePlayer1IsEnabled = true;

            Lbl_Tip.Content = "Press \"LShift\" to serve the ball up!\nor\nPress \"LCtrl\" to serve the ball down!\n\nYou can choose the serve position by moving up and down with \"W\" and \"S\"!";
            Lbl_TipMidPosition();
        }

        private void BallgetsBlockedByPlayer1()
        {
            if (CurrentBallPositionX <= PlayerLeft.ActualWidth && CurrentBallPositionX > 0 && CurrentBallPositionY <= CurrentPlayer1PositionY + PlayerLeft.ActualHeight && CurrentBallPositionY >= CurrentPlayer1PositionY &&
                servePlayer1IsEnabled == false && servePlayer2IsEnabled == false)
            {
                ballMovementDirectionLeft = false;
                ballMovementDirectionRight = true;
            }
            else if (CurrentBallPositionX <= 0)
            {
                Player2HasScoredEvent();
            }
        }

        private void BallgetsBlockedByPlayer2()
        {
            if (CurrentBallPositionX >= CanvasGameTable.ActualWidth - (Ball.ActualWidth + PlayerRight.ActualWidth) && CurrentBallPositionX <= CanvasGameTable.ActualWidth - Ball.ActualWidth &&
                CurrentBallPositionY <= CurrentPlayer2PositionY + PlayerLeft.ActualHeight && CurrentBallPositionY >= CurrentPlayer2PositionY && servePlayer1IsEnabled == false && servePlayer2IsEnabled == false)
            {
                ballMovementDirectionRight = false;
                ballMovementDirectionLeft = true;
            }
            else if (CurrentBallPositionX > CanvasGameTable.ActualWidth - Ball.ActualWidth)
            {
                Player1HasScoredEvent();
            }
        }

        private void GameTableBorderDefinitionY()
        {
            if (CurrentBallPositionY >= CanvasGameTable.ActualHeight - Ball.ActualHeight)
            {
                ballMovementDirectionDown = false;
                ballMovementDirectionUp = true;
            }
            else if (CurrentBallPositionY <= 0)
            {
                ballMovementDirectionUp = false;
                ballMovementDirectionDown = true;
            }
        }

        private void GameEndEvent()
        {
            if ((scorePlayer1 + scorePlayer2 == playingTimeBestOf) || (scorePlayer1 > playingTimeBestOf / 2) || (scorePlayer2 > playingTimeBestOf / 2))
            {
                GameAnimationTimer.Stop();

                if (scorePlayer1 == scorePlayer2)
                {
                    Lbl_Tip.Content = $"Draw!  Score: {scorePlayer1} : {scorePlayer2}";
                }
                else if (scorePlayer1 > scorePlayer2)
                {
                    Lbl_Tip.Content = $"Player1 wins!  Score: {scorePlayer1} : {scorePlayer2}";
                }
                else
                {
                    Lbl_Tip.Content = $"Player2 wins!  Score: {scorePlayer1} : {scorePlayer2}";
                }

                Lbl_TipMidPosition();
                Lbl_Tip.Visibility = Visibility.Visible;
                AfterGameDelayTimer.Start();
            }
        }

        private void RefreshScore()
        {
            Lbl_CurrentScore.Content = $"{scorePlayer1} : {scorePlayer2}";
            Lbl_TipMidPosition();
        }

        private void PlayerPositionReset()
        {
            Canvas.SetLeft(PlayerLeft, 0);
            Canvas.SetTop(PlayerLeft, CanvasGameTableMidPlayerY());
            Canvas.SetLeft(PlayerRight, CanvasGameTable.ActualWidth - PlayerRight.ActualWidth);
            Canvas.SetTop(PlayerRight, CanvasGameTableMidPlayerY());
            Lbl_TipMidPosition();
        }

        private void Player1GetsBall()
        {
            Canvas.SetLeft(Ball, Canvas.GetLeft(PlayerLeft) + PlayerLeft.ActualWidth);
            Canvas.SetTop(Ball, Canvas.GetTop(PlayerLeft) + PlayerMidY() - BallMid());
            Lbl_TipMidPosition();
        }

        private void Player2GetsBall()
        {
            Canvas.SetLeft(Ball, Canvas.GetLeft(PlayerRight) - Ball.ActualWidth);
            Canvas.SetTop(Ball, Canvas.GetTop(PlayerRight) + PlayerMidY() - BallMid());
            Lbl_TipMidPosition();
        }

        private void BallMovementWhileServe()
        {
            if (servePlayer1IsEnabled)
            {
                Player1GetsBall();
            }
            else if (servePlayer2IsEnabled)
            {
                Player2GetsBall();
            }
            Lbl_TipMidPosition();
        }

        private void BallStopsMoving()
        {
            ballMovementDirectionRight = false;
            ballMovementDirectionLeft = false;
            ballMovementDirectionDown = false;
            ballMovementDirectionUp = false;
            Lbl_TipMidPosition();
        }

        private void Lbl_TipMidPosition()
        {
            Canvas.SetLeft(Lbl_Tip, CanvasGameTableMidX() - Lbl_TipMidX());
            Canvas.SetTop(Lbl_Tip, CanvasGameTableMidY() - Lbl_TipMidY());
        }

    }
}
