using System;
using System.Collections.Generic;

namespace InfiniteTicTacToe.Models
{
    public class GameEngine
    {
        private readonly Dictionary<string, PlayerType> _board = new Dictionary<string, PlayerType>();
        public int WinLength { get; } = 3;

        public void MakeMove(int x, int y, PlayerType player)
        {
            _board[$"{x},{y}"] = player;
        }

        public PlayerType GetCell(int x, int y)
        {
            return _board.TryGetValue($"{x},{y}", out var player) ? player : PlayerType.None;
        }

        public List<Tuple<int, int>> GetWinningCells(int lastX, int lastY, PlayerType player)
        {
            var directions = new[]
            {
                new { DX = 1, DY = 0 },
                new { DX = 0, DY = 1 },
                new { DX = 1, DY = 1 },
                new { DX = 1, DY = -1 }
            };

            foreach (var dir in directions)
            {
                var winningLine = new List<Tuple<int, int>> { Tuple.Create(lastX, lastY) };

                for (int i = 1; i < WinLength; i++)
                {
                    if (GetCell(lastX + i * dir.DX, lastY + i * dir.DY) == player)
                        winningLine.Add(Tuple.Create(lastX + i * dir.DX, lastY + i * dir.DY));
                    else break;
                }

                for (int i = 1; i < WinLength; i++)
                {
                    if (GetCell(lastX - i * dir.DX, lastY - i * dir.DY) == player)
                        winningLine.Add(Tuple.Create(lastX - i * dir.DX, lastY - i * dir.DY));
                    else break;
                }

                if (winningLine.Count >= WinLength) return winningLine;
            }
            return null;
        }

        public void ClearBoard()
        {
            _board.Clear();
        }
    }
}