using System;
using System.Linq;

namespace VP_ConnectFour
{
    public class Game
    {
        public const int Rows = 6;
        public const int Columns = 7;
        public int[,] board = new int[Rows, Columns];
        public int CurrentPlayer { get; set; } = 1;

        public void Reset()
        {
            board = new int[Rows, Columns];
            CurrentPlayer = 1;
        }

        public void DropDisc(int row, int column, int player)
        {
            board[row, column] = player;
        }

        public void RemoveDisc(int row, int column)
        {
            board[row, column] = 0;
        }

        public int GetLowermostFreeRow(int column)
        {
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (board[r, column] == 0)
                {
                    return r;
                }
            }
            return -1;
        }

        public bool CheckWin(int row, int col, int player)
        {
            return CheckDirection(row, col, -1, 0, player) + CheckDirection(row, col, 1, 0, player) >= 3 ||
                   CheckDirection(row, col, 0, -1, player) + CheckDirection(row, col, 0, 1, player) >= 3 ||
                   CheckDirection(row, col, -1, -1, player) + CheckDirection(row, col, 1, 1, player) >= 3 ||
                   CheckDirection(row, col, -1, 1, player) + CheckDirection(row, col, 1, -1, player) >= 3;
        }

        private int CheckDirection(int row, int col, int dRow, int dCol, int player)
        {
            int count = 0;
            for (int i = 1; i <= 3; i++)
            {
                int r = row + dRow * i;
                int c = col + dCol * i;
                if (r >= 0 && r < Rows && c >= 0 && c < Columns && board[r, c] == player)
                    count++;
                else
                    break;
            }
            return count;
        }
        public int EvaluateBoard()
        {
            int score = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns - 3; c++)
                {
                    int[] segment = new int[4];
                    for (int i = 0; i <= 3; i++)
                    {
                        if (board[r,c+i] != 0 || r == GetLowermostFreeRow(c + i))
                        {
                            segment[i] = board[r, c + i];
                        }
                    }
                    
                    score += ScoreSegment(segment);
                }
            }

            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r < Rows - 3; r++)
                {
                    int[] segment = new int[4];
                    for (int i = 0; i <= 3; i++)
                    {
                        if (board[r + i, c] != 0 || r + i == GetLowermostFreeRow(c))
                        {
                            segment[i] = board[r + i, c];
                        }
                    }
                    score += ScoreSegment(segment);
                }
            }

            for (int r = 0; r < Rows - 3; r++)
            {
                for (int c = 0; c < Columns - 3; c++)
                {
                    int[] segment = new int[4];
                    for (int i = 0; i <= 3; i++)
                    {
                        if (board[r + i, c + i] != 0 || r + i == GetLowermostFreeRow(c + i))
                        {
                            segment[i] = board[r + i, c + i];
                        }
                    }
                    score += ScoreSegment(segment);
                }
            }

            for (int r = 0; r < Rows - 3; r++)
            {
                for (int c = 3; c < Columns; c++)
                {
                    int[] segment = new int[4];
                    for (int i = 0; i <= 3; i++)
                    {
                        if (board[r + i, c - i] != 0 || r + i == GetLowermostFreeRow(c - i))
                        {
                            segment[i] = board[r + i, c - i];
                        }
                    }
                    score += ScoreSegment(segment);
                }
            }
            return score;
        }

        private int ScoreSegment(int[] segment)
        {
            int emptyCount = 0;
            int humanCount = 0;
            int aiCount = 0;
            for (int i = 0; i < segment.Length; i++)
            {
                if (segment[i] == 0)
                {
                    emptyCount++;
                }
                else if (segment[i] == 1)
                {
                    humanCount++;
                }
                else if (segment[i] == 2)
                {
                    aiCount++;
                }

            }
            if (aiCount == 4)
            {
                return 100000;
            }
            else if (humanCount == 4)
            {
                return -100000;
            }

            else if (aiCount == 3 && emptyCount == 1)
            {
                return 1000;
            }
            else if (humanCount == 3 && emptyCount == 1) 
            {
                return -1000; 
            }

            else if (aiCount == 2 && emptyCount == 2)
            {
                return 100;
            }
            else if (humanCount == 2 && emptyCount == 2)
            {
                return -100;
            }

            return 0;
        }

        public bool IsBoardFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (board[0, col] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = 3 - CurrentPlayer;
        }
    }
}