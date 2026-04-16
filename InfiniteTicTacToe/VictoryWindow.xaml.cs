using System.Windows;

namespace InfiniteTicTacToe
{
    public partial class VictoryWindow : Window
    {
        public VictoryWindow(string winnerName)
        {
            InitializeComponent();
            WinnerText.Text = $"{winnerName} виграв гру!";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}