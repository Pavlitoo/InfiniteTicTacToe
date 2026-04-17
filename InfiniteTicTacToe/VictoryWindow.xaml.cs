using System.Windows;

namespace InfiniteTicTacToe
{
    public partial class VictoryWindow : Window
    {
        public VictoryWindow(string winnerName, bool isDraw = false)
        {
            InitializeComponent();

            if (isDraw)
            {
                IconText.Text = "🤝";
                IconBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                WinnerText.Text = $"🤝 НІЧИЯ!";
                WinnerText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
            }
            else
            {
                WinnerText.Text = $"🏆 ПЕРЕМОГА!\n{winnerName}";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}