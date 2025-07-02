using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP_ConnectFour
{
    public class Game
    {
        public const int Rows = 6;
        public const int Columns = 7;
        private int[,] board = new int[Rows, Columns];
        public int CurrentPlayer { get;  set; } = 1;

        public void Reset()
        {
            board = new int[Rows, Columns];
            CurrentPlayer = 1;
        }

        public int DropDisc(int column)
        {
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (board[r, column] == 0)
                {
                    board[r, column] = CurrentPlayer;
                    return r;
                }
            }
            return -1;
        }


        public bool CheckWin(int row, int col)
        {
            int player = board[row, col];
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

        public void SwitchPlayer()
        {
            CurrentPlayer = 3 - CurrentPlayer;
        }
    }

}


