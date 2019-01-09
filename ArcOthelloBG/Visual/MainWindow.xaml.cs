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


            String[] colRowString = senderButton.Name.Split('_');
            int col = Convert.ToInt16(colRowString[1]);
            int row = Convert.ToInt16(colRowString[2]);
            Vector2 position = new Vector2(col, row);

            try
            {
                Game.Instance.play(position, this.currentPlayId);

                Uri imageUri;
                if (this.currentPlayId == this.whiteId)
                {
                    imageUri = new Uri(this.whiteUri);
                    this.currentPlayId = this.blackId;
                }
                else
                {
                    imageUri = new Uri(this.blackUri);
                    this.currentPlayId = this.whiteId;
                }
                this.togglePlayerBorderColors();

                showValidMoves();
                changeCellImage(senderButton, imageUri);
            }
            catch(ArgumentException exception)
            {

            }
        }

        private void togglePlayerBorderColors()
        {
            Border blackPlayerBorder = this.FindName("BlackPlayerBorder") as Border;
            Border whitePlayerBorder = this.FindName("WhitePlayerBorder") as Border;
            if (this.currentPlayId == this.blackId)
            {
                whitePlayerBorder.BorderBrush = new SolidColorBrush(Colors.White);
                blackPlayerBorder.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
            else
            {
                whitePlayerBorder.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
                blackPlayerBorder.BorderBrush = new SolidColorBrush(Colors.White);
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
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = new SolidColorBrush(Colors.White);
                }
            }

            this.oldValidMoves.Clear();

            List<Vector2> validMoves = Game.Instance.getPositionsAvailable(this.currentPlayId);
            foreach (Vector2 CellCoor in validMoves)
            {
                if (Game.Instance.getColor(CellCoor) == 0)
                {
                    this.oldValidMoves.Add(CellCoor);
                    this.btnMatrix[CellCoor.X, CellCoor.Y].Background = new SolidColorBrush(Colors.LightGreen);
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
                this.currentPlayId = this.blackId;

                this.blackUri = "pack://application:,,,/Visual/bfm.png";
                this.whiteUri = "pack://application:,,,/Visual/prixGarantie.jpg";

                this.oldValidMoves = new List<Vector2>();

                this.togglePlayerBorderColors();

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
