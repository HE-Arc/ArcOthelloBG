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

namespace ArcOthelloBG
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        //TO-DO: ADAPT TO GAME LOGIC
        private int currentPlayId;
        private int whiteId;
        private int blackId;
        private Button[,] btnMatrix;
        private Uri blackUri;
        private Uri whiteUri;
        private List<Vector2> oldValidMoves;
        private int timeSecondBlack;
        private int timeSecondWhite;
        private Timer timerTime;

        private SolidColorBrush whiteBrush;
        private SolidColorBrush greenBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TimeWhite
        {
            get
            {
                TimeSpan result = TimeSpan.FromSeconds(this.timeSecondWhite);
                return result.ToString("mm':'ss");
            }
        }

        public string TimeBlack
        {
            get
            {
                TimeSpan result = TimeSpan.FromSeconds(this.timeSecondBlack);
                return result.ToString("mm':'ss");
            }
        }

        public int BlackScore
        {
            get
            {
                try
                {
                    return Game.Instance.BlackScore;
                }
                catch (NullReferenceException e) //first time, if board is not init
                {
                    return 0;
                }
            }
        }

        public int WhiteScore
        {
            get
            {
                try
                {
                    return Game.Instance.WhiteScore;
                }
                catch (NullReferenceException e) //first time, if board is not init
                {
                    return 0;
                }
            }
        }


        private void buildBoard(int colCount, int rowCount)
        {
            Grid grid = this.FindName("Board") as Grid;
            this.btnMatrix = new Button[colCount, rowCount];
            Label[] rowLabels = new Label[rowCount];
            Label[] colLabels = new Label[colCount];


            //The first column (row labels) should be smaller
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(20, GridUnitType.Pixel) //TO-DO: find dynamic solution
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
                        Content = letter + (row + 1).ToString(),
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

        private void BoardClickHandler(object sender, EventArgs e)
        {
            Button senderButton = (Button)sender;

            String[] colRowString = senderButton.Name.Split('_');
            int col = Convert.ToInt16(colRowString[1]);
            int row = Convert.ToInt16(colRowString[2]);
            Vector2 position = new Vector2(col, row);

            try
            {
                List<Vector2> changedPositions = Game.Instance.play(position, this.currentPlayId);

                Uri imageUri;
                if (this.currentPlayId == this.whiteId)
                {
                    imageUri = this.whiteUri;
                }
                else
                {
                    imageUri = this.blackUri;
                }

                changeCellImage(senderButton, imageUri);
                foreach (Vector2 changedPosition in changedPositions)
                {
                    changeCellImage(this.btnMatrix[changedPosition.X, changedPosition.Y], imageUri);
                }

                if (this.currentPlayId == this.whiteId || this.currentPlayId == this.blackId && changedPositions.Count > 0)
                {
                    RaisePropertyChanged("WhiteScore");
                }
                if (this.currentPlayId == this.blackId || this.currentPlayId == this.whiteId && changedPositions.Count > 0)
                {
                    RaisePropertyChanged("BlackScore");
                }

                this.passTurn();
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void mnuNewGameClick(object sender, EventArgs e)
        {
            resetBoard(buildGUI: false);
        }

        private void mnuLoadGameClick(object sender, EventArgs e)
        {
            Console.WriteLine("Loading");
        }

        private void mnuSaveGameClick(object sender, EventArgs e)
        {
            Console.WriteLine("Save game");
        }

        private void mnuExitClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnStartClick(object sender, EventArgs e)
        {
            resetBoard(buildGUI: true);
        }

        private void togglePlayerBorderColors()
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

        private void changeCellImage(Button cell, Uri imageUri)
        {
            cell.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(imageUri),
                Stretch = Stretch.Fill
            };
        }


        private void showValidMoves()
        {

            foreach (Vector2 CellCoor in this.oldValidMoves)
            {
                if (Game.Instance.getColor(CellCoor) == 0)
                {
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = this.whiteBrush;
                }
            }

            this.oldValidMoves.Clear();

            List<Vector2> validMoves = Game.Instance.getPositionsAvailable();

            foreach (Vector2 CellCoor in validMoves)
            {
                if (Game.Instance.getColor(CellCoor) == 0)
                {
                    this.oldValidMoves.Add(CellCoor);
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = this.greenBrush;
                }
            }

        }

        private void resetBoard(bool buildGUI)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                int width = Convert.ToInt16(appSettings["columns"]); ;
                int height = Convert.ToInt16(appSettings["rows"]); ;
                this.whiteId = Convert.ToInt16(appSettings["whiteId"]);
                this.blackId = Convert.ToInt16(appSettings["blackId"]);
                this.currentPlayId = this.blackId;

                this.togglePlayerBorderColors();

                Game.Instance.init(width, height, this.whiteId, this.blackId);

                this.setTimer();
                this.startTimer();

                if (buildGUI)
                {
                    Border blackPlayerBorder = this.FindName("BlackPlayerBorder") as Border;
                    Border whitePlayerBorder = this.FindName("WhitePlayerBorder") as Border;
                    BlackPlayerBorder.Opacity = 1;
                    WhitePlayerBorder.Opacity = 1;
                    Button startButton = this.FindName("btnStart") as Button;
                    Grid grid = this.FindName("Board") as Grid;
                    grid.Children.Remove(startButton);
                    buildBoard(width, height);
                }
                else
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            this.btnMatrix[x, y].Background = this.whiteBrush;
                        }
                    }
                }

                Uri imageUri = null;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var idPlayer = Game.Instance.getColor(new Vector2(i, j));

                        if (idPlayer == this.blackId)
                        {
                            imageUri = this.blackUri;
                        }
                        else if (idPlayer == this.whiteId)
                        {
                            imageUri = this.whiteUri;
                        }

                        if (idPlayer != 0)
                        {
                            changeCellImage(this.btnMatrix[i, j], imageUri);
                        }
                    }
                }
                this.showValidMoves();

                RaisePropertyChanged("BlackScore");

                RaisePropertyChanged("WhiteScore");
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

        }

        private void passTurn()
        {

            if (this.currentPlayId == this.whiteId)
            {
                this.currentPlayId = this.blackId;
            }
            else if (this.currentPlayId == this.blackId)
            {
                this.currentPlayId = this.whiteId;
            }

            this.togglePlayerBorderColors();
            showValidMoves();
        }

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

        private void setTimer()
        {
            this.timerTime = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            this.timerTime.Elapsed += OnTimedEvent;
            this.timerTime.AutoReset = true;
            this.timerTime.Enabled = true;
            //this.timerTime.Stop();
        }

        private void startTimer()
        {
            this.timerTime.Start();
        }

        private void stopTimer()
        {
            this.timerTime.Stop();
        }


        public MainWindow()
        {
            InitializeComponent();
            this.whiteBrush = new SolidColorBrush(Colors.White);
            this.greenBrush = new SolidColorBrush(Colors.LightGreen);
            this.blackUri = new Uri("pack://application:,,,/Visual/bfm.png");
            this.whiteUri = new Uri("pack://application:,,,/Visual/prixGarantie.jpg");
            DataContext = this;


            this.oldValidMoves = new List<Vector2>();

        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
