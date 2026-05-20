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
        private bool _isAiThinking = false;

        private string _player1Name = "Гравець 1";
        private string _player2Name = "Гравець 2";
        private string _currentPlayerStatus = "Очікування...";
        private string _playerColor = "#7f8fa6";
        private string _gameMode = "2 гравці";
        private string _pauseButtonText = "ПАУЗА";

        private bool _isCoinTossVisible;
        private bool _isFlipping;
        private string _coinText = "?";
        private string _tossStatusText = "";

        public ObservableCollection<Cell> DrawnCells { get; set; }
        public RelayCommand MakeMoveCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }
        public RelayCommand ResetCommand { get; set; }

        public string TimerText { get => _timerText; set { _timerText = value; OnPropertyChanged(); } }
        public string Player1Name { get => _player1Name; set { _player1Name = value; OnPropertyChanged(); OnPlayerNamesChanged(); } }
        public string Player2Name { get => _player2Name; set { _player2Name = value; OnPropertyChanged(); OnPlayerNamesChanged(); } }
        public string CurrentPlayerStatus { get => _currentPlayerStatus; set { _currentPlayerStatus = value; OnPropertyChanged(); } }
        public string PlayerColor { get => _playerColor; set { _playerColor = value; OnPropertyChanged(); } }
        public string GameMode { get => _gameMode; set { _gameMode = value; OnPropertyChanged(); OnPlayerNamesChanged(); } }
        public string PauseButtonText { get => _pauseButtonText; set { _pauseButtonText = value; OnPropertyChanged(); } }

        public bool IsAiMode => GameMode == "1 гравець";
        public bool IsAiThinking { get => _isAiThinking; set { _isAiThinking = value; OnPropertyChanged(); } }

        public bool IsCoinTossVisible { get => _isCoinTossVisible; set { _isCoinTossVisible = value; OnPropertyChanged(); } }
        public bool IsFlipping { get => _isFlipping; set { _isFlipping = value; OnPropertyChanged(); } }
        public string CoinText { get => _coinText; set { _coinText = value; OnPropertyChanged(); } }
        public string TossStatusText { get => _tossStatusText; set { _tossStatusText = value; OnPropertyChanged(); } }

        public MainViewModel()
        {
            _gameEngine = new GameEngine();
            DrawnCells = new ObservableCollection<Cell>();
            _currentPlayer = PlayerType.Cross;

            _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _gameTimer.Tick += (s, e) => { _secondsElapsed++; TimerText = TimeSpan.FromSeconds(_secondsElapsed).ToString(@"mm\:ss"); };

            MakeMoveCommand = new RelayCommand(ExecuteMakeMove);
            StartCommand = new RelayCommand(o => StartGame(), o => CanStart());
            PauseCommand = new RelayCommand(o => TogglePause());
            ResetCommand = new RelayCommand(o => ResetGame());
        }

        private bool CanStart() => !string.IsNullOrWhiteSpace(Player1Name) && !string.IsNullOrWhiteSpace(Player2Name) && !IsCoinTossVisible;
        private void OnPlayerNamesChanged() => CommandManager.InvalidateRequerySuggested();

        private void TogglePause()
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                _gameTimer.Stop();
                PauseButtonText = "ПРОДОВЖИТИ";
            }
            else if (!_isPlaying && _secondsElapsed > 0 && !IsCoinTossVisible)
            {
                _isPlaying = true;
                _gameTimer.Start();
                PauseButtonText = "ПАУЗА";
            }
        }

        private void ResetGame()
        {
            _gameTimer.Stop();
            _isPlaying = false;
            _isAiThinking = false;
            _secondsElapsed = 0;
            TimerText = "00:00";
            PauseButtonText = "ПАУЗА";
            DrawnCells.Clear();
            _gameEngine.ClearBoard();
            _currentPlayer = PlayerType.Cross;
            CurrentPlayerStatus = "Очікування...";
            PlayerColor = "#7f8fa6";
        }

        private void StartGame()
        {
            if (IsAiMode)
            {
                ResetGame();
                _isPlaying = true;
                _gameTimer.Start();
                UpdatePlayerInfo();
                _ = ExecuteAiMove();
            }
            else
            {
                _ = PerformCoinToss();
            }
        }

        private async Task PerformCoinToss()
        {
            ResetGame();
            CommandManager.InvalidateRequerySuggested();

            IsCoinTossVisible = true;
            IsFlipping = true;
            CoinText = "?";
            TossStatusText = "Підкидаємо монетку...";

            SystemSounds.Beep.Play();
            await Task.Delay(2000);

            IsFlipping = false;

            Random rnd = new Random();
            bool player1Wins = rnd.Next(2) == 0;

            if (player1Wins)
            {
                CoinText = "Орел";
                TossStatusText = $"Першим ходить {Player1Name}!";
                _currentPlayer = PlayerType.Cross;
            }
            else
            {
                CoinText = "Решка";
                TossStatusText = $"Першим ходить {Player2Name}!";
                _currentPlayer = PlayerType.Zero;
            }

            SystemSounds.Asterisk.Play();
            await Task.Delay(2500);

            IsCoinTossVisible = false;
            _isPlaying = true;
            _gameTimer.Start();
            UpdatePlayerInfo();
            CommandManager.InvalidateRequerySuggested();
        }

        private async void ExecuteMakeMove(object parameter)
        {
            if (!_isPlaying || IsAiThinking) return;

            if (parameter is Point clickPoint)
            {
                int x = (int)Math.Floor(clickPoint.X / Cell.CellSize);
                int y = (int)Math.Floor(clickPoint.Y / Cell.CellSize);

                if (x < 0 || x >= 3 || y < 0 || y >= 3) return;

                if (_gameEngine.GetCell(x, y) == PlayerType.None)
                {
                    SystemSounds.Beep.Play();

                    _gameEngine.MakeMove(x, y, _currentPlayer);
                    DrawnCells.Add(new Cell(x, y, _currentPlayer));

                    if (CheckEndGame(x, y)) return;

                    _currentPlayer = _currentPlayer == PlayerType.Cross ? PlayerType.Zero : PlayerType.Cross;
                    UpdatePlayerInfo();

                    if (IsAiMode)
                    {
                        await ExecuteAiMove();
                    }
                }
            }
        }

        private async Task ExecuteAiMove()
        {
            IsAiThinking = true;
            await Task.Delay(500);

            var aiMove = _gameEngine.GetAiMove(PlayerType.Cross, PlayerType.Zero);

            if (aiMove != null)
            {
                SystemSounds.Beep.Play();
                int x = aiMove.Item1;
                int y = aiMove.Item2;

                _gameEngine.MakeMove(x, y, PlayerType.Cross);
                DrawnCells.Add(new Cell(x, y, PlayerType.Cross));

                if (!CheckEndGame(x, y))
                {
                    _currentPlayer = PlayerType.Zero;
                    UpdatePlayerInfo();
                }
            }

            IsAiThinking = false;
        }

        private bool CheckEndGame(int lastX, int lastY)
        {
            var winningCells = _gameEngine.GetWinningCells(lastX, lastY, _currentPlayer);
            if (winningCells != null)
            {
                EndGame(winningCells);
                return true;
            }

            if (DrawnCells.Count >= 9)
            {
                EndGame(null, true);
                return true;
            }

            return false;
        }

        private async void EndGame(System.Collections.Generic.List<Tuple<int, int>> winningCells, bool isDraw = false)
        {
            _gameTimer.Stop();
            _isPlaying = false;
            SystemSounds.Asterisk.Play();

            if (!isDraw && winningCells != null)
            {
                foreach (var wCell in winningCells)
                {
                    var cellToUpdate = DrawnCells.FirstOrDefault(c => c.X == wCell.Item1 && c.Y == wCell.Item2);
                    if (cellToUpdate != null) cellToUpdate.IsWinningCell = true;
                }
            }

            // Додана затримка в 3 секунди, щоб встигнути зробити скріншот!
            await Task.Delay(3000);

            string winnerName = isDraw ? "Нічия" : (_currentPlayer == PlayerType.Cross ? Player1Name : Player2Name);
            if (!isDraw && _currentPlayer == PlayerType.Zero) winnerName = Player2Name;
            if (!isDraw && _currentPlayer == PlayerType.Cross) winnerName = Player1Name;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var victoryWindow = new VictoryWindow(winnerName, isDraw) { Owner = Application.Current.MainWindow };
                victoryWindow.ShowDialog();
            });

            ResetGame();
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