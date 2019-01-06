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

namespace ArcOthelloBG
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //TO-DO: ADAPT TO GAME LOGIC
        private int currentPlayId;
        private int whiteId;
        private int blackId;
        private Button[,] btnMatrix;
        private String blackUri;
        private String whiteUri;
        private List<Vector2> oldValidMoves;


        private void _initBoard(int colCount, int rowCount)
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
                    //TO-DO: THE BUTTON IS TEMPORARY, WE SHOULD REPLACE IT WITH A MORE APPROPRIATE/CUSTOM WIDGET FOR A BOARD ELEMENT

                    btnMatrix[col, row] = new Button()
                    {

                        Name = Convert.ToString("_" + col + "_" + row), //because of XAML name restrictions, it must start with a "_"
                        Content = letter + (row + 1).ToString(),
                        Background = new SolidColorBrush(Colors.White)
                    };

                    btnMatrix[col, row].Click += new RoutedEventHandler(ClickHandler);

                    btnMatrix[col, row].Style = Resources["MyButtonStyle"] as Style;

                    //btnMatrix[col, row].MouseEnter += new MouseEventHandler((sender, e) =>
                    //{
                    //    Button senderButton = (Button)sender;
                    //    senderButton.BorderBrush = new SolidColorBrush(Colors.Red);
                    //});

                    Grid.SetColumn(btnMatrix[col, row], col + 1);
                    Grid.SetRow(btnMatrix[col, row], row + 1);
                    grid.Children.Add(btnMatrix[col, row]);

                }
            }
        }

        private void ClickHandler(object sender, EventArgs e)
        {
            //TO-DO: check game logic
            Button senderButton = (Button)sender;
            Uri imageUri;

            String[] colRowString = senderButton.Name.Split('_');
            int col = Convert.ToInt16(colRowString[1]);
            int row = Convert.ToInt16(colRowString[2]);
            Vector2 position = new Vector2(row, col);

            if (Game.Instance.isPlayable(position, this.currentPlayId))
            {

                Game.Instance.play(position, this.currentPlayId);

                StackPanel player1StackPanel = this.FindName("Player1") as StackPanel;
                StackPanel player2StackPanel = this.FindName("Player2") as StackPanel;


                if (this.currentPlayId == this.whiteId)
                {
                    imageUri = new Uri(this.whiteUri);
                    this.currentPlayId = this.blackId;
                    player1StackPanel.Background = new SolidColorBrush(Colors.PaleGreen);
                    player2StackPanel.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    imageUri = new Uri(this.blackUri);
                    this.currentPlayId = this.whiteId;
                    player1StackPanel.Background = new SolidColorBrush(Colors.White);
                    player2StackPanel.Background = new SolidColorBrush(Colors.PaleGreen);
                }

                showValidMoves();
                changeCellImage(senderButton, imageUri);
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
                    this.btnMatrix[CellCoor.Y, CellCoor.X].Background = new SolidColorBrush(Colors.White);
                }
            }

            this.oldValidMoves.Clear();

            List<Vector2> validMoves = Game.Instance.getPositionsAvailable(this.currentPlayId);
            foreach (Vector2 CellCoor in validMoves)
            {
                if (Game.Instance.getColor(CellCoor) == 0)
                {
                    this.oldValidMoves.Add(CellCoor);
                    this.btnMatrix[CellCoor.Y, CellCoor.X].Background = new SolidColorBrush(Colors.LightGreen);
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                int width = Convert.ToInt16(appSettings["columns"]); ;
                int height = Convert.ToInt16(appSettings["rows"]); ;
                this.whiteId = Convert.ToInt16(appSettings["whiteId"]);
                this.blackId = Convert.ToInt16(appSettings["blackId"]);

                this.blackUri = "pack://application:,,,/Visual/bfm.png";
                this.whiteUri = "pack://application:,,,/Visual/prixGarantie.jpg";

                this.oldValidMoves = new List<Vector2>();

                StackPanel playerStackPanel = this.FindName("Player1") as StackPanel;//Player who starts : black => player1 = black and player2 = white
                playerStackPanel.Background = new SolidColorBrush(Colors.PaleGreen);

                Game.Instance.init(width, height, this.whiteId, this.blackId);
                _initBoard(width, height);

                Uri imageUri = null;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var idPlayer = Game.Instance.getColor(new Vector2(i, j));

                        if (idPlayer == this.blackId)
                        {
                            imageUri = new Uri(this.blackUri);
                        }
                        else if (idPlayer == this.whiteId)
                        {
                            imageUri = new Uri(this.whiteUri);
                        }

                        if (idPlayer != 0)
                        {
                            changeCellImage(this.btnMatrix[i, j], imageUri);
                        }
                    }
                }
                this.showValidMoves();
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

        }
    }
}
