using ArcOthelloBG.Logic;
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
using System.Configuration;
using System.Timers;
using System.ComponentModel;
using ArcOthelloBG.Exceptions;

namespace ArcOthelloBG
{
    /// <summary>
    /// Interaction logic for GUI MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentPlayId;
        private int whiteId;
        private int blackId;
        private int emptyId;
        private int width;
        private int height;
        private Button[,] btnMatrix;
        private Uri blackUri;
        private Uri whiteUri;
        private List<Vector2> oldValidMoves;

        private UndoRedo undoRedo;

        private SolidColorBrush whiteBrush;
        private SolidColorBrush goldBrush;
        private SolidColorBrush greenBrush;

        private bool hasWon;

        private bool guiBuilded;
        private int winnerId;

        private const string BLACK_NAME = "BFM";
        private const string WHITE_NAME = "Prix Garantie";
        private const string EXTENSION = "Otello files (*.otl)|*.otl";

        public MainWindow()
        {
            InitializeComponent();
            this.whiteBrush = new SolidColorBrush(Colors.White);
            this.greenBrush = new SolidColorBrush(Colors.LightGreen);
            this.goldBrush = new SolidColorBrush(Colors.Gold);
            this.blackUri = new Uri("pack://application:,,,/Visual/bfm.png");
            this.whiteUri = new Uri("pack://application:,,,/Visual/prixGarantie.jpg");
            DataContext = this;
            this.undoRedo = new UndoRedo();
            this.hasWon = false;

            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                this.width = Convert.ToInt16(appSettings["columns"]);
                this.height = Convert.ToInt16(appSettings["rows"]);
                this.whiteId = Convert.ToInt16(appSettings["whiteId"]);
                this.blackId = Convert.ToInt16(appSettings["blackId"]);
                this.emptyId = Convert.ToInt16(appSettings["emptyId"]);

            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                this.width = 9;
                this.height = 7;
                this.whiteId = 2;
                this.blackId = 1;
                this.emptyId = -1;
            }

            this.oldValidMoves = new List<Vector2>();
            Game.Instance.TurnSkipped += Game_TurnSkipped;
            Game.Instance.Won += Game_Won;
            this.guiBuilded = false;
        }



        /// <summary>
        /// Builds the board GUI
        /// </summary>
        /// <param name="colCount"></param>
        /// <param name="rowCount"></param>
        private void BuildBoard(int colCount, int rowCount)
        {
            Grid grid = this.FindName("Board") as Grid;
            this.btnMatrix = new Button[colCount, rowCount];
            Label[] rowLabels = new Label[rowCount];
            Label[] colLabels = new Label[colCount];


            //The first column (row labels) should be smaller
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(20, GridUnitType.Pixel)
            });

            for (int col = 0; col < colCount; col++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < rowCount; row++)
            {

                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });

                rowLabels[row] = new Label()
                {
                    Content = row + 1,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Grid.SetColumn(rowLabels[row], 0);
                Grid.SetRow(rowLabels[row], row + 1);
                grid.Children.Add(rowLabels[row]);
            }

            grid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(1, GridUnitType.Star)
            }); //we need one more row

            for (int col = 0; col < colCount; col++)
            {

                char letter = (char)(col + 'A');
                colLabels[col] = new Label()
                {
                    Content = letter,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                Grid.SetColumn(colLabels[col], col + 1);
                Grid.SetRow(colLabels[col], 0);
                grid.Children.Add(colLabels[col]);

                for (int row = 0; row < rowCount; row++)
                {

                    btnMatrix[col, row] = new Button()
                    {

                        Name = Convert.ToString("_" + col + "_" + row), //because of XAML name restrictions, it must start with a "_"
                        Background = this.whiteBrush
                    };

                    btnMatrix[col, row].Click += new RoutedEventHandler(BoardClickHandler);

                    btnMatrix[col, row].Style = Resources["MyButtonStyle"] as Style;


                    Grid.SetColumn(btnMatrix[col, row], col + 1);
                    Grid.SetRow(btnMatrix[col, row], row + 1);
                    grid.Children.Add(btnMatrix[col, row]);

                }
            }
        }

        /// <summary>
        /// Method called if a cell of the board is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoardClickHandler(object sender, EventArgs e)
        {
            Button senderButton = (Button)sender;

            TextBlock lblSkipped = this.FindName("lblSkipped") as TextBlock;
            lblSkipped.Visibility = Visibility.Hidden;

            String[] colRowString = senderButton.Name.Split('_');
            int col = Convert.ToInt16(colRowString[1]);
            int row = Convert.ToInt16(colRowString[2]);
            Vector2 position = new Vector2(col, row);

            try
            {
                this.undoRedo.DoAction(this.GetBoardState());
                List<Vector2> changedPositions = Game.Instance.Play(position, this.currentPlayId);

                if (this.hasWon)
                {
                    return;
                }

                Uri imageUri;
                if (this.currentPlayId == this.whiteId)
                {
                    imageUri = this.whiteUri;
                }
                else
                {
                    imageUri = this.blackUri;
                }

                ChangeCellImage(senderButton, imageUri);
                foreach (Vector2 changedPosition in changedPositions)
                {
                    ChangeCellImage(this.btnMatrix[changedPosition.X, changedPosition.Y], imageUri);
                }

                if (this.currentPlayId == this.whiteId || this.currentPlayId == this.blackId && changedPositions.Count > 0)
                {
                    RaisePropertyChanged("WhiteScore");
                    double opacityPrixG = 1.0 - (double)Game.Instance.BlackScore / (double)(width * height);
                    Image logoPrixG = this.FindName("LogoPrixG") as Image;
                    logoPrixG.Opacity = opacityPrixG;
                }
                if (this.currentPlayId == this.blackId || this.currentPlayId == this.whiteId && changedPositions.Count > 0)
                {
                    RaisePropertyChanged("BlackScore");
                    double opacityBFM = 1.0 - (double)Game.Instance.WhiteScore / (double)(width * height);
                    Image logoBFM = this.FindName("LogoBFM") as Image;
                    logoBFM.Opacity = opacityBFM;
                }

                this.currentPlayId = Game.Instance.PlayerToPlay;

                this.TogglePlayerBorderColors();
                this.ShowValidMoves();
            }
            catch (ArgumentException exception)
            {
                this.undoRedo.Undo(this.GetBoardState());
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// menu reset clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetBoard(object sender, EventArgs e)
        {
            MenuItem mnuResetgame = this.FindName("mnuResetGame") as MenuItem;
            if (mnuResetgame.IsEnabled)
                ResetBoard();
        }



        /// <summary>
        /// Action when exit menu item clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuExitClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Action if start menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartClick(object sender, EventArgs e)
        {
            ResetBoard();
        }

        /// <summary>
        /// Changes visualization of currently playing player 
        /// (if it's the black's player turn, the left area in the GUI containing his information gets its border green 
        /// and the white player's GUI area border gets white)
        /// </summary>
        private void TogglePlayerBorderColors()
        {
            Border blackPlayerBorder = this.FindName("BlackPlayerBorder") as Border;
            Border whitePlayerBorder = this.FindName("WhitePlayerBorder") as Border;
            if (this.currentPlayId == this.blackId)
            {
                whitePlayerBorder.BorderBrush = this.whiteBrush;
                blackPlayerBorder.BorderBrush = this.greenBrush;
            }
            else
            {
                whitePlayerBorder.BorderBrush = this.greenBrush;
                blackPlayerBorder.BorderBrush = this.whiteBrush;
            }
        }

        /// <summary>
        /// Changes the image of a cell, typically if a player puts a pawn on the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="imageUri"></param>
        private void ChangeCellImage(Button cell, Uri imageUri)
        {
            cell.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(imageUri),
                Stretch = Stretch.Fill
            };
        }


        /// <summary>
        /// Resets the game GUI (e.g. if a new game is started or the first time a game is launched)
        /// </summary>
        private void ResetBoard()
        {
            Game.Instance.Init(this.width, this.height, this.whiteId, this.blackId, this.emptyId);
            this.hasWon = false;
            this.currentPlayId = Game.Instance.PlayerToPlay;
            this.TogglePlayerBorderColors();

            Image logoBFM = this.FindName("LogoBFM") as Image;
            logoBFM.Opacity = 1.0;

            Image logoPrixG = this.FindName("LogoPrixG") as Image;
            logoPrixG.Opacity = 1.0;

            if (!guiBuilded)
            {
                guiBuilded = true;
                this.SetTimer();
                Border blackPlayerBorder = this.FindName("BlackPlayerBorder") as Border;
                Border whitePlayerBorder = this.FindName("WhitePlayerBorder") as Border;
                BlackPlayerBorder.Opacity = 1;
                WhitePlayerBorder.Opacity = 1;
                MenuItem mnuResetGame = this.FindName("mnuResetGame") as MenuItem;
                mnuResetGame.IsEnabled = true;
                MenuItem mnuSaveGame = this.FindName("mnuSaveGame") as MenuItem;
                mnuSaveGame.IsEnabled = true;
                Button startButton = this.FindName("btnStart") as Button;
                startButton.Visibility = Visibility.Collapsed;
                TextBlock lblWon = this.FindName("lblWon") as TextBlock;
                lblWon.Visibility = Visibility.Collapsed;
                TextBlock lblSkipped = this.FindName("lblSkipped") as TextBlock;
                lblSkipped.Visibility = Visibility.Hidden;
                BuildBoard(this.width, this.height);

            }
            else
            {
                for (int x = 0; x < this.width; x++)
                {
                    for (int y = 0; y < this.height; y++)
                    {
                        this.btnMatrix[x, y].Background = this.whiteBrush;
                    }
                }
            }

            Uri imageUri = null;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    var idPlayer = Game.Instance.GetColor(new Vector2(i, j));

                    if (idPlayer == this.blackId)
                    {
                        imageUri = this.blackUri;
                    }
                    else if (idPlayer == this.whiteId)
                    {
                        imageUri = this.whiteUri;
                    }

                    if (idPlayer != this.emptyId)
                    {
                        ChangeCellImage(this.btnMatrix[i, j], imageUri);
                    }
                }
            }
            this.ShowValidMoves();
            this.StartTimer();

            RaisePropertyChanged("BlackScore");
            RaisePropertyChanged("WhiteScore");
        }

    }
}
