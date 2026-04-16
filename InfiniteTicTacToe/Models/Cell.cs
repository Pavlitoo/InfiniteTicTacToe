using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InfiniteTicTacToe.Models
{
    public class Cell : INotifyPropertyChanged
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PlayerType Player { get; set; }

        public const int CellSize = 50;
        public double CanvasX => X * CellSize;
        public double CanvasY => Y * CellSize;

        private bool _isWinningCell;
        public bool IsWinningCell
        {
            get => _isWinningCell;
            set { _isWinningCell = value; OnPropertyChanged(); }
        }

        public Cell(int x, int y, PlayerType player)
        {
            X = x;
            Y = y;
            Player = player;
            IsWinningCell = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}