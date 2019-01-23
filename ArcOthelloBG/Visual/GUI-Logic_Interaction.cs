using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.ComponentModel;
using System.Windows;
using ArcOthelloBG.Logic;
using ArcOthelloBG.Exceptions;
using System.Windows.Controls;

namespace ArcOthelloBG
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int timeSecondBlack;
        private int timeSecondWhite;
        private Timer timerTime;


        public string TimeWhite
        {
            get
            {
                TimeSpan result = TimeSpan.FromSeconds(this.timeSecondWhite);
                return $"Time: {result.ToString("mm':'ss")}";
            }
        }

        public string TimeBlack
        {
            get
            {
                TimeSpan result = TimeSpan.FromSeconds(this.timeSecondBlack);
                return $"Time: {result.ToString("mm':'ss")}";
            }
        }

        public String BlackScore
        {
            get
            {
                try
                {
                    return $"Score: {Game.Instance.BlackScore.ToString()}";
                }
                catch (NullReferenceException e) //first time, if board is not init
                {
                    return "Score: 0";
                }
            }
        }

        public String WhiteScore
        {
            get
            {
                try
                {
                    return $"Score: {Game.Instance.WhiteScore.ToString()}";
                }
                catch (NullReferenceException e) //first time, if board is not init
                {
                    return "Score: 0";
                }
            }
        }

        /// <summary>
        /// get the board state
        /// </summary>
        /// <returns>state</returns>
        private BoardState GetBoardState()
        {
            BoardState state = Game.Instance.BoardState;
            state.BlackTime = this.timeSecondBlack;
            state.WhiteTime = this.timeSecondWhite;

            return state;
        }

        /// <summary>
        /// Update GUI to show available moves for the current player
        /// </summary>
        private void ShowValidMoves()
        {

            foreach (Vector2 CellCoor in this.oldValidMoves)
            {
                if (Game.Instance.GetColor(CellCoor) == this.emptyId)
                {
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = this.whiteBrush;
                }
            }

            this.oldValidMoves.Clear();
            List<Vector2> validMoves = Game.Instance.GetPositionsAvailable();

            foreach (Vector2 CellCoor in validMoves)
            {
                if (Game.Instance.GetColor(CellCoor) == this.emptyId)
                {
                    this.oldValidMoves.Add(CellCoor);
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = this.greenBrush;
                }
            }

        }

        /// <summary>
        /// action when open menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBoard(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = EXTENSION,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.ResetBoard();
                Game.Instance.LoadState(BoardFileManager.LoadStateFromFile(openFileDialog.FileName));

                this.timeSecondWhite = Game.Instance.BoardState.WhiteTime;
                this.timeSecondBlack = Game.Instance.BoardState.BlackTime;

                RaisePropertyChanged("TimeBlack");
                RaisePropertyChanged("TimeWhite");

                this.GetBoardStateAndRefreshGUI();
            }
        }

        /// <summary>
        /// action when saved menu clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBoard(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = EXTENSION,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                BoardState state = this.GetBoardState();

                state.WhiteTime = this.timeSecondWhite;
                state.BlackTime = this.timeSecondBlack;

                BoardFileManager.SaveToFile(saveFileDialog.FileName, this.GetBoardState());
            }

            Console.WriteLine("Game saved");
        }

        /// <summary>
        /// action when redo menu item clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoBoard(object sender, EventArgs e)
        {
            try
            {
                Game.Instance.LoadState(this.undoRedo.Undo(this.GetBoardState()));
                this.GetBoardStateAndRefreshGUI();
            }
            catch (StackEmptyException) { }
        }

        /// <summary>
        /// action when redo menu item clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoBoard(object sender, EventArgs e)
        {
            try
            {
                Game.Instance.LoadState(this.undoRedo.Redo(this.GetBoardState()));
                this.GetBoardStateAndRefreshGUI();
            }
            catch (StackEmptyException) { }
        }

        /// <summary>
        /// Event occuring every second, counts the time a player has played
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (this.currentPlayId == this.blackId)
            {
                this.timeSecondBlack++;
                RaisePropertyChanged("TimeBlack");
            }
            else if (this.currentPlayId == this.whiteId)
            {
                this.timeSecondWhite++;
                RaisePropertyChanged("TimeWhite");
            }
        }

        /// <summary>
        /// Set the timer that counts the time of each player
        /// </summary>
        private void SetTimer()
        {

            this.timerTime = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            this.timerTime.Elapsed += OnTimedEvent;
            this.timerTime.AutoReset = true;
            this.timerTime.Enabled = true;

        }

        /// <summary>
        /// start the timer that counts the time of each player
        /// </summary>
        private void StartTimer()
        {
            this.timerTime.Stop();
            this.timeSecondBlack = 0;
            this.timeSecondWhite = 0;
            this.timerTime.Start();
            RaisePropertyChanged("TimeBlack");
            RaisePropertyChanged("TimeWhite");
        }

        /// <summary>
        /// Stop the timer that counts the time of each player
        /// </summary>
        private void StopTimer()
        {
            this.timerTime.Stop();
        }

        /// <summary>
        /// Called if a end game signal comes from the logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_Won(object sender, EventHandling.WinEventArgs e)
        {
            this.winnerId = e.PlayerId;

            this.hasWon = true;

            this.WinGame();
        }

        /// <summary>
        /// Method used to show on the GUI the winning screen
        /// </summary>
        private void WinGame()
        {
            this.StopTimer();
            RaisePropertyChanged("WhiteScore");
            RaisePropertyChanged("BlackScore");
            MenuItem mnuResetgame = this.FindName("mnuResetGame") as MenuItem;
            mnuResetgame.IsEnabled = false;
            Button startButton = this.FindName("btnStart") as Button;
            startButton.Visibility = Visibility.Visible;

            TextBlock lblWon = this.FindName("lblWon") as TextBlock;
            lblWon.Visibility = Visibility.Visible;

            TextBlock lblSkipped = this.FindName("lblSkipped") as TextBlock;
            lblSkipped.Visibility = Visibility.Hidden;

            int whiteScore = Game.Instance.WhiteScore;
            int blackScore = Game.Instance.BlackScore;

            int winnerScore = whiteScore > blackScore ? whiteScore : blackScore;

            String winnerString = "";
            if (this.winnerId == this.whiteId)
                winnerString = WHITE_NAME;
            else if (this.winnerId == this.blackId)
                winnerString = BLACK_NAME;

            lblWon.Text = $"The {winnerString} was elected the best beer in the world with {winnerScore} points";
            this.guiBuilded = false;
            Grid grid = this.FindName("Board") as Grid;
            List<UIElement> childrenToRemove = new List<UIElement>();
            foreach (UIElement child in grid.Children)
            {
                if (!(child is StackPanel))
                {
                    childrenToRemove.Add(child);
                }
            }

            foreach (UIElement child in childrenToRemove)
            {
                grid.Children.Remove(child);
            }

            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            MenuItem mnuSaveGame = this.FindName("mnuSaveGame") as MenuItem;
            mnuSaveGame.IsEnabled = false;

            Border blackPlayerBorder = this.FindName("BlackPlayerBorder") as Border;
            Border whitePlayerBorder = this.FindName("WhitePlayerBorder") as Border;



            if (this.winnerId == this.whiteId)
            {
                BlackPlayerBorder.Opacity = 0.25;
                WhitePlayerBorder.BorderBrush = this.goldBrush;
                BlackPlayerBorder.BorderBrush = this.whiteBrush;
                Image logoPrixG = this.FindName("LogoPrixG") as Image;
                logoPrixG.Opacity = 1.0;
            }
            else if (this.winnerId == this.blackId)
            {
                WhitePlayerBorder.Opacity = 0.25;
                BlackPlayerBorder.BorderBrush = this.goldBrush;
                WhitePlayerBorder.BorderBrush = this.whiteBrush;
                Image logoBFM = this.FindName("LogoBFM") as Image;
                logoBFM.Opacity = 1.0;
            }
        }

        /// <summary>
        /// Gets the board state from the logic and updates the GUI
        /// </summary>
        private void GetBoardStateAndRefreshGUI()
        {
            BoardState state = this.GetBoardState();
            int[,] Board = state.Board;
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if (Board[i, j] == this.whiteId)
                        this.ChangeCellImage(btnMatrix[i, j], this.whiteUri);
                    else if (Board[i, j] == this.blackId)
                        this.ChangeCellImage(btnMatrix[i, j], this.blackUri);
                    else btnMatrix[i, j].Background = this.whiteBrush;
                }
            }
            ShowValidMoves();
            RaisePropertyChanged("WhiteScore");
            RaisePropertyChanged("BlackScore");
            this.timeSecondBlack = state.BlackTime;
            this.timeSecondWhite = state.WhiteTime;
            RaisePropertyChanged("TimeBlack");
            RaisePropertyChanged("TimeWhite");

            this.currentPlayId = Game.Instance.PlayerToPlay;

            this.TogglePlayerBorderColors();

        }

        /// <summary>
        /// Called if a signal from the logic say that a turn was skipped because a player ad no valid moves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game_TurnSkipped(object sender, EventHandling.SkipTurnEventArgs e)
        {
            if (!this.hasWon)
            {
                TextBlock lblSkipped = this.FindName("lblSkipped") as TextBlock;
                lblSkipped.Visibility = Visibility.Visible;

                string player = "";

                if (this.currentPlayId == this.whiteId)
                    player = BLACK_NAME;
                else if (this.currentPlayId == this.blackId)
                    player = WHITE_NAME;

                lblSkipped.Text = $"{player} had no move available and skipped his turn";
            }
        }

        /// <summary>
        /// Used for databinding
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
