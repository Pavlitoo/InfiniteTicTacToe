using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data; // Не забудь додати
using System.Globalization; // Не забудь додати
using InfiniteTicTacToe.ViewModels;

namespace InfiniteTicTacToe
{
    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _clickPosition;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Автоматичне центрування дошки при запуску
            CanvasTranslate.X = (GameGrid.ActualWidth - GameBoard.Width) / 2;
            CanvasTranslate.Y = (GameGrid.ActualHeight - GameBoard.Height) / 2;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _clickPosition = e.GetPosition(this);
            GameBoard.CaptureMouse();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            GameBoard.ReleaseMouseCapture();

            Point currentPosition = e.GetPosition(this);

            if (Math.Abs(currentPosition.X - _clickPosition.X) < 5 &&
                Math.Abs(currentPosition.Y - _clickPosition.Y) < 5)
            {
                if (DataContext is MainViewModel vm)
                {
                    // Точний клік відносно самої дошки
                    Point clickOnBoard = e.GetPosition(GameBoard);
                    vm.MakeMoveCommand.Execute(clickOnBoard);
                }
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                CanvasTranslate.X += currentPosition.X - _clickPosition.X;
                CanvasTranslate.Y += currentPosition.Y - _clickPosition.Y;
                _clickPosition = currentPosition;
            }
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            if (CanvasScale.ScaleX * zoomFactor > 0.3 && CanvasScale.ScaleX * zoomFactor < 5.0)
            {
                CanvasScale.ScaleX *= zoomFactor;
                CanvasScale.ScaleY *= zoomFactor;
            }
        }
    }

    // --- НОВИЙ ДОПОМІЖНИЙ КЛАС-КОНВЕРТЕР (У цьому ж файлі, або окремо) ---
    // Необхідний для роботи RadioButton у режимі MVVM
    public class StringMatchesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter?.ToString() : Binding.DoNothing;
        }
    }
}