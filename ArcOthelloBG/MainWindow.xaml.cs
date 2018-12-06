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

        private void _initBoard(int colCount,int rowCount)
        {
            Grid grid = this.FindName("Board") as Grid;

            Button[,] btnMatrix = new Button[colCount, rowCount];

            for(int col=0;col<colCount;col++)
            {
                for(int row=0;row<rowCount;row++)
                {
                    //THE BUTTON IS TEMPORARY, WE SHOULD REPLACE IT WITH A MORE APPROPRIATE WIDGET FOR A BOARD ELEMENT
                    btnMatrix[col,row]= new Button()
                    {
                       Name="Col"+col+"Row"+row,
                       Content=(char)(col+'A')+row.ToString()
                    };
                    Grid.SetColumn(btnMatrix[col, row], col);
                    Grid.SetRow(btnMatrix[col, row], row);
                    grid.Children.Add(btnMatrix[col, row]);
                    //wtf, only the last added button is visible and takes all the horizontal space
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _initBoard(9, 7);

        }
    }
}
