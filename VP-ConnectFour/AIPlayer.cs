using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP_ConnectFour
{
    public class AIPlayer
    {
        private Game game;
        private Random rng = new Random();

        public AIPlayer(Game game)
        {
            this.game = game;
        }
        public int GetMove(string difficulty)
        {
            if (difficulty == "Beginner")
            {
                return BeginnerMove();
            }
            else if (difficulty == "Intermediate")
            {
                return IntermediateMove();
            }
            else if (difficulty == "Advanced")
            {
                return AdvancedMove();
            }
            else
            {
                return ExtremeMove();
            }
        }
        private int BeginnerMove()
        {
            List<int> validCols = new List<int>();
            for (int col = 0; col < Game.Columns; col++)
            {
                if (game.GetLowermostFreeRow(col) != -1)
                    validCols.Add(col);
            }
            return validCols.Count > 0 ? validCols[rng.Next(validCols.Count)] : -1;
        }

        private int IntermediateMove()
        {

            for (int col = 0; col < Game.Columns; col++)
            {
                int testRow = game.GetLowermostFreeRow(col);
                if (testRow != -1 && game.CheckWin(testRow, col, 2))
                    return col;
            }

            for (int col = 0; col < Game.Columns; col++)
            {
                int testRow = game.GetLowermostFreeRow(col);
                if (testRow != -1 && game.CheckWin(testRow, col, 1))
                    return col;
            }

            return BeginnerMove();
        }

        private int AdvancedMove()
        {
            int bestScore = int.MinValue;
            int bestCol = -1;

            for (int col = 0; col < Game.Columns; col++)
            {
                int row = game.GetLowermostFreeRow(col);
                if (row != -1)
                {
                    if (game.CheckWin(row, col, 2))
                    {
                        return col;
                    }
                    game.DropDisc(row, col, 2);
                    int currentMoveScore;

                    if (CheckInstantLoss())
                    {
                        currentMoveScore = int.MinValue;
                    }
                    else
                    {
                        currentMoveScore = game.EvaluateBoard();
                    }
                    game.RemoveDisc(row, col);


                    if (currentMoveScore > bestScore)
                    {
                        bestScore = currentMoveScore;
                        bestCol = col;
                    }

                }
            }

            if (bestCol == -1 || bestScore == int.MinValue)
            {
                return BeginnerMove();
            }

            return bestCol;
        }

        private int ExtremeMove()
        {
            int bestScore = int.MinValue;
            int bestCol = -1;

            for (int col = 0; col < Game.Columns; col++)
            {
                int row = game.GetLowermostFreeRow(col);
                if (row != -1)
                {
                    if (game.CheckWin(row, col, 2))
                    {
                        return col;
                    }
                    game.DropDisc(row, col, 2);

                    int currentMoveScore;

                    if (CheckInstantLoss())
                    {
                        currentMoveScore = int.MinValue;
                    }
                    else
                    {
                        currentMoveScore = Minimax(4, false, int.MinValue, int.MaxValue);
                    }
                    game.RemoveDisc(row, col);


                    if (currentMoveScore > bestScore)
                    {
                        bestScore = currentMoveScore;
                        bestCol = col;
                    }
                }
            }

            if (bestCol != -1)
            {
                return bestCol;
            }

            return BeginnerMove();
        }
        private int Minimax(int depth, bool isMaximizing, int alpha, int beta)
        {
            if (depth == 0 || game.IsBoardFull())
                return game.EvaluateBoard();

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int col = 0; col < Game.Columns; col++)
                {
                    int row = game.GetLowermostFreeRow(col);
                    if (row != -1)
                    {
                        if (game.CheckWin(row, col, 2))
                        {
                            return 1000000;
                        }
                        game.DropDisc(row, col, 2);
                        int eval = Minimax(depth - 1, false, alpha, beta);
                        game.RemoveDisc(row, col);
                        maxEval = Math.Max(maxEval, eval);
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha) break;
                    }
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int col = 0; col < Game.Columns; col++)
                {
                    int row = game.GetLowermostFreeRow(col);
                    if (row != -1)
                    {
                        if (game.CheckWin(row, col, 1))
                        {
                            return -1000000;
                        }
                        game.DropDisc(row, col, 1);
                        int eval = Minimax(depth - 1, true, alpha, beta);
                        game.RemoveDisc(row, col);
                        minEval = Math.Min(minEval, eval);
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha) break;
                    }
                }
                return minEval;
            }
        }
        private bool CheckInstantLoss()
        {
            bool allowsImmediateHumanWin = false;
            for (int humanCheckCol = 0; humanCheckCol < Game.Columns; humanCheckCol++)
            {
                int humanCheckRow = game.GetLowermostFreeRow(humanCheckCol);
                if (humanCheckRow != -1)
                {
                    if (game.CheckWin(humanCheckRow, humanCheckCol, 1))
                    {
                        allowsImmediateHumanWin = true;
                    }
                    if (allowsImmediateHumanWin)
                    {
                        break;
                    }
                }
            }
            return allowsImmediateHumanWin;
        }
    }
}
