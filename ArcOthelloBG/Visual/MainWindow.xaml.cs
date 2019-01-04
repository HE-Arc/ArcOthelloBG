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

                        Name = "Col" + letter + "Row" + (row + 1),
                        Content = letter + (row + 1).ToString()
                    };

                    btnMatrix[col, row].Click += new RoutedEventHandler(ClickHandler);

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

            if (this.currentPlayId == this.whiteId)
            {
                imageUri = new Uri("pack://application:,,,/Visual/bfm.png");
                this.currentPlayId = this.blackId;
            }
            else
            {
                imageUri = new Uri("pack://application:,,,/Visual/prixGarantie.jpg");
                this.currentPlayId = this.whiteId;
            }
                

            senderButton.Background = new ImageBrush()
            {
                ImageSource = new BitmapImage(imageUri),
                Stretch = Stretch.Fill
            };
            
        }

        public MainWindow()
        {
            InitializeComponent();
            
            int width = 7;
            int height = 7;
            this.blackId = 1;
            this.whiteId = 2;
            //if (new Random().Next(2)==0) //TO-DO: check who begins game
            //    this.currentPlayId = this.whiteId;
            //else
            this.currentPlayId = this.blackId;
            Game.Instance.init(width, height, this.whiteId, this.blackId);
            _initBoard(width, height);
            Uri imageUri=null;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Game.Instance.Board[i, j] == this.blackId)
                    {
                         imageUri = new Uri("pack://application:,,,/Visual/bfm.png");
                       
                    }
                    else if (Game.Instance.Board[i, j] == this.whiteId)
                    {
                         imageUri = new Uri("pack://application:,,,/Visual/prixGarantie.jpg");
                       
                    }

                    if (Game.Instance.Board[i, j] != 0)
                    {
                        this.btnMatrix[i, j].Background = new ImageBrush()
                        {
                            ImageSource = new BitmapImage(imageUri),
                            Stretch = Stretch.Fill
                        };
                    }
                    else if(Game.Instance.isPlayable(new Vector2(i,j),this.currentPlayId))//TO-DO: doesn't work !
                    {
                        this.btnMatrix[i, j].Content = "X";
                    }
                }
            }
        }
    }
}
