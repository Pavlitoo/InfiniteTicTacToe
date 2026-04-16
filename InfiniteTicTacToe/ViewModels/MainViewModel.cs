using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using InfiniteTicTacToe.Models;
using InfiniteTicTacToe.Commands;

namespace InfiniteTicTacToe.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private GameEngine _gameEngine;
        private PlayerType _currentPlayer;
        private DispatcherTimer _gameTimer;
        private int _secondsElapsed;
        private string _timerText = "00:00";
        private bool _isPlaying = false;

        private string _player1Name = "";
        private string _player2Name = "";
        private string _currentPlayerStatus = "Очікування...";
        private string _playerColor = "#7f8fa6";

        public const int GridWidth = 15;
        public const int GridHeight = 15;

        public ObservableCollection<Cell> DrawnCells { get; set; }
        public RelayCommand MakeMoveCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }

        public string TimerText { get => _timerText; set { _timerText = value; OnPropertyChanged(); } }

        public string Player1Name
        {
            get => _player1Name;
            set { _player1Name = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public string Player2Name
        {
            get => _player2Name;
            set { _player2Name = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public string CurrentPlayerStatus { get => _currentPlayerStatus; set { _currentPlayerStatus = value; OnPropertyChanged(); } }
        public string PlayerColor { get => _playerColor; set { _playerColor = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            _gameEngine = new GameEngine();
            DrawnCells = new ObservableCollection<Cell>();
            _currentPlayer = PlayerType.Cross;

            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _gameTimer.Tick += (s, e) => { _secondsElapsed++; TimerText = TimeSpan.FromSeconds(_secondsElapsed).ToString(@"mm\:ss"); };

            MakeMoveCommand = new RelayCommand(ExecuteMakeMove);

            StartCommand = new RelayCommand(
                o => { _isPlaying = true; _gameTimer.Start(); UpdatePlayerInfo(); },
                o => !string.IsNullOrWhiteSpace(Player1Name) && !string.IsNullOrWhiteSpace(Player2Name)
            );

            PauseCommand = new RelayCommand(o => { _isPlaying = false; _gameTimer.Stop(); });
            ResetCommand = new RelayCommand(o => ResetGame());
        }

        private void ResetGame()
        {
            _gameTimer.Stop();
            _isPlaying = false;
            _secondsElapsed = 0;
            TimerText = "00:00";
            DrawnCells.Clear();
            _gameEngine.ClearBoard();
            _currentPlayer = PlayerType.Cross;
            CurrentPlayerStatus = "Очікування...";
            PlayerColor = "#7f8fa6";
        }

        private async void ExecuteMakeMove(object parameter)
        {
            if (!_isPlaying) return;

            if (parameter is Point clickPoint)
            {
                int x = (int)Math.Floor(clickPoint.X / Cell.CellSize);
                int y = (int)Math.Floor(clickPoint.Y / Cell.CellSize);

                if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight) return;

                if (_gameEngine.GetCell(x, y) == PlayerType.None)
                {
                    SystemSounds.Beep.Play();

                    _gameEngine.MakeMove(x, y, _currentPlayer);
                    DrawnCells.Add(new Cell(x, y, _currentPlayer));

                    var winningCells = _gameEngine.GetWinningCells(x, y, _currentPlayer);
                    if (winningCells != null)
                    {
                        _gameTimer.Stop();
                        _isPlaying = false;
                        SystemSounds.Asterisk.Play();

                        foreach (var wCell in winningCells)
                        {
                            var cellToUpdate = DrawnCells.FirstOrDefault(c => c.X == wCell.Item1 && c.Y == wCell.Item2);
                            if (cellToUpdate != null) cellToUpdate.IsWinningCell = true;
                        }

                        string winnerName = _currentPlayer == PlayerType.Cross ? Player1Name : Player2Name;
                        await Task.Delay(500);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var victoryWindow = new VictoryWindow(winnerName) { Owner = Application.Current.MainWindow };
                            victoryWindow.ShowDialog();
                        });

                        ResetGame();
                        return;
                    }

                    _currentPlayer = _currentPlayer == PlayerType.Cross ? PlayerType.Zero : PlayerType.Cross;
                    UpdatePlayerInfo();
                }
            }
        }

        private void UpdatePlayerInfo()
        {
            if (!_isPlaying) return;

            if (_currentPlayer == PlayerType.Cross)
            {
                CurrentPlayerStatus = $"{Player1Name} (X)";
                PlayerColor = "#e84118";
            }
            else
            {
                CurrentPlayerStatus = $"{Player2Name} (O)";
                PlayerColor = "#00a8ff";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}