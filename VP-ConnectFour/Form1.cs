using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VP_ConnectFour
{
    public partial class Form1 : Form
    {
        Game game = new Game();
        Panel[,] cells = new Panel[Game.Rows, Game.Columns];
        bool singlePlayerMode = false;
        Random rng = new Random();
        string difficulty = "Beginner";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int col = 0; col < Game.Columns; col++)
                {
                    Panel cell = (Panel)tlpGameGrid.GetControlFromPosition(col, row);
                    cell.BackColor = Color.White;
                    cell.Click += Cell_Click;
                    cell.Tag = col;
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, cell.Width, cell.Height);
                    cell.Region = new Region(path);
                    cells[row, col] = cell;
                }
            }
        }

        private async void Cell_Click(object sender, EventArgs e)
        {
            if (game.CurrentPlayer == 2 && singlePlayerMode)
                return;

            Panel clickedCell = (Panel)sender;
            int col = (int)clickedCell.Tag;
            int row = game.GetLowermostFreeRow(col);

            if (row != -1)
            {
                game.DropDisc(row, col, game.CurrentPlayer);
                cells[row, col].BackColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;

                if (game.CheckWin(row, col, game.CurrentPlayer))
                {
                    MessageBox.Show($"Player {game.CurrentPlayer} wins!");
                    Reset();
                    return;
                }

                if (game.IsBoardFull())
                {
                    MessageBox.Show("It's a tie!");
                    Reset();
                    return;
                }

                game.SwitchPlayer();

                if (singlePlayerMode && game.CurrentPlayer == 2)
                {
                    await Task.Delay(500);
                    AIMove();
                }
            }
        }

        private void AIMove()
        {
            int aiCol = -1;

            if (difficulty == "Beginner")
            {
                aiCol = BeginnerMove();
            }
            else if (difficulty == "Intermediate")
            {
                aiCol = IntermediateMove();
            }
            else if (difficulty == "Advanced")
            {
                aiCol = AdvancedMove();
            }

            if (aiCol != -1)
            {
                int aiRow = game.GetLowermostFreeRow(aiCol);
                game.DropDisc(aiRow, aiCol, game.CurrentPlayer);
                cells[aiRow, aiCol].BackColor = Color.Yellow;

                if (game.CheckWin(aiRow, aiCol, game.CurrentPlayer))
                {
                    MessageBox.Show("AI wins!");
                    Reset();
                    return;
                }

                if (game.IsBoardFull())
                {
                    MessageBox.Show("It's a tie!");
                    Reset();
                    return;
                }

                game.SwitchPlayer();
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
                            if (allowsImmediateHumanWin) break;  
                        }
                    }
                    int currentMoveScore = game.EvaluateBoard();

                    game.RemoveDisc(row, col);
                    
                    if (allowsImmediateHumanWin)
                    {
                        currentMoveScore = int.MinValue;
                    }

                    if (currentMoveScore > bestScore)
                    {
                        bestScore = currentMoveScore;
                        bestCol = col;
                    }
                    else if (currentMoveScore == bestScore)
                    {
                        int currentColDistanceToCenter = Math.Abs(col - Game.Columns/2);
                        int bestColDistanceToCenter = Math.Abs(bestCol - Game.Columns/2);

                        if (bestCol == -1 || currentColDistanceToCenter < bestColDistanceToCenter)
                        {
                            bestCol = col;
                        }
                      
                    }
                }
            }

            if (bestCol == -1 || bestScore == int.MinValue)
            {
                return BeginnerMove();
            }

            return bestCol;
        }





        private void Reset()
        {
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int col = 0; col < Game.Columns; col++)
                {
                    cells[row, col].BackColor = Color.White;
                }
            }
            game.Reset();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            singlePlayerMode = cbMode.SelectedItem.ToString() == "Single-Player";
            Reset();
        }

        private void cbDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            difficulty = cbDifficulty.SelectedItem.ToString();
        }
    }
}