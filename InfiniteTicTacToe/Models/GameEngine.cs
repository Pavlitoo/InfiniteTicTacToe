using System;
using System.Collections.Generic;
using System.Linq;

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
                new { DX = 1, DY = 0 }, // Горизонталь
                new { DX = 0, DY = 1 }, // Вертикаль
                new { DX = 1, DY = 1 }, // Діагональ /
                new { DX = 1, DY = -1 } // Діагональ \
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

        public Tuple<int, int> GetAiMove(PlayerType aiPlayer, PlayerType humanPlayer)
        {
            var move = FindBestMove(aiPlayer, aiPlayer);
            if (move != null) return move;

            move = FindBestMove(humanPlayer, aiPlayer);
            if (move != null) return move;

            List<Tuple<int, int>> openCells = new List<Tuple<int, int>>();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (GetCell(i, j) == PlayerType.None) openCells.Add(Tuple.Create(i, j));

            if (openCells.Any(c => c.Item1 == 1 && c.Item2 == 1)) return Tuple.Create(1, 1);

            var corners = new[] { Tuple.Create(0, 0), Tuple.Create(0, 2), Tuple.Create(2, 0), Tuple.Create(2, 2) };
            foreach (var corner in corners) if (openCells.Contains(corner)) return corner;

            return openCells.FirstOrDefault();
        }

        private Tuple<int, int> FindBestMove(PlayerType targetPlayer, PlayerType aiPlayer)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (GetCell(x, y) == PlayerType.None)
                    {
                        MakeMove(x, y, targetPlayer);

                        bool isWin = GetWinningCells(x, y, targetPlayer) != null;

                        _board.Remove($"{x},{y}");

                        if (isWin) return Tuple.Create(x, y);
                    }
                }
            }
            return null;
        }

        public void ClearBoard()
        {
            _board.Clear();
        }
    }
}