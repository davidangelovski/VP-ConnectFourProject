﻿using System;
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
        bool inputEnabled = true;
        int p1Score = 0;
        int p2Score = 0;

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
                    cell.Click += Panel_Click;
                    cell.Tag = col;
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, cell.Width, cell.Height);
                    cell.Region = new Region(path);
                    cells[row, col] = cell;
                }
            }
        }

        private async Task AnimateDiscFall(int row, int col, Color discColor)
        {
            for (int r = 0; r <= row; r++)
            {
                cells[r, col].BackColor = discColor;

                if (r > 0)
                {
                    cells[r - 1, col].BackColor = Color.White;
                }

                await Task.Delay(50);
            }
        }

        private async void HandleMove(int col)
        {
            if (!inputEnabled)
            {
                return;
            }
            inputEnabled = false;
            int row = game.GetLowermostFreeRow(col);
            if (row == -1) return;

            game.DropDisc(row, col, game.CurrentPlayer);
            Color discColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;
            await AnimateDiscFall(row, col, discColor);
            if (game.CheckWin(row, col, game.CurrentPlayer))
            {
                if (game.CurrentPlayer == 1)
                {
                    p1Score++;
                }
                else 
                {
                    p2Score++;
                }
                UpdateScoreLabels();
                lbTurn.Text = $"Player {game.CurrentPlayer} wins! Well played!\nReset the board to play again!";
                inputEnabled = false;
                return;
            }

            if (game.IsBoardFull())
            {
                lbTurn.Text = "It's a tie!\nReset the board to play again";
                inputEnabled = false;
                return;
            }

            game.SwitchPlayer();
            switchTurnText();
            if (singlePlayerMode && game.CurrentPlayer == 2)
            {
                await Task.Delay(500);
                AIMove();
            }
            else 
            {
                inputEnabled = true;
            }
        }


        private async void AIMove()
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
                Color discColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;
                await AnimateDiscFall(aiRow, aiCol, discColor);
                if (game.CheckWin(aiRow, aiCol, game.CurrentPlayer))
                {
                    p2Score++;
                    UpdateScoreLabels();
                    lbTurn.Text ="AI wins! Better luck next time.\nReset the board to play again!";
                    inputEnabled = false;
                    return;
                }

                if (game.IsBoardFull())
                {
                    lbTurn.Text = "It's a tie!\nReset the board to play again";
                    inputEnabled = false;
                    return;
                }

                game.SwitchPlayer();
                switchTurnText();
                inputEnabled = true;
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
            inputEnabled = true;
            lbTurn.Text = $"Click on the board to begin";
            lbTurn.ForeColor = Color.White;
            game.Reset();
        }

       

        private void switchTurnText()
        {
            if (singlePlayerMode && game.CurrentPlayer == 2)
            {
                lbTurn.Text = "AI's Turn";
            }
            else 
            {
                lbTurn.Text = $"Player {game.CurrentPlayer}'s Turn";
            }
            lbTurn.ForeColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;
        }
        private void UpdateScoreLabels() 
        {
            lbP1Score.Text = $"Player 1: {p1Score}";
            if (singlePlayerMode)
            {
                lbP2Score.Text = $"AI: {p2Score}";
            }
            else
            {
                lbP2Score.Text = $"Player 2: {p2Score}";
            }
        }
        private void tlpGameGrid_MouseClick(object sender, MouseEventArgs e)
        {
            int colWidth = tlpGameGrid.Width / Game.Columns;
            int clickedCol = e.X / colWidth;

            if (clickedCol < 0 || clickedCol >= Game.Columns)
                return;

            HandleMove(clickedCol);
        }
        private void Panel_Click(object sender, EventArgs e)
        {
            Panel clickedCell = (Panel)sender;
            int col = (int)clickedCell.Tag;
            HandleMove(col);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void rbBeginner_Click(object sender, EventArgs e)
        {
            difficulty = "Beginner";
        }

        private void rbIntermediate_Click(object sender, EventArgs e)
        {
            difficulty = "Intermediate";

        }

        private void rbAdvanced_Click(object sender, EventArgs e)
        {
            difficulty = "Advanced";
        }

        private void rbSingle_Click(object sender, EventArgs e)
        {
            rbSingle.Checked = true;
            rbMulti.Checked = false;
            singlePlayerMode = true;
            gbDifficulty.Visible = true;
            p1Score = 0;
            p2Score = 0;
            UpdateScoreLabels();
            Reset();
        }

        private void rbMulti_Click(object sender, EventArgs e)
        {
            rbMulti.Checked = true;
            rbSingle.Checked = false;
            singlePlayerMode = false;
            gbDifficulty.Visible = false;
            p1Score = 0;
            p2Score = 0;
            UpdateScoreLabels();
            Reset();
        }


        private void btResetScores_Click(object sender, EventArgs e)
        {
            p1Score = 0;
            p2Score = 0;
            UpdateScoreLabels();
        }
    }
}