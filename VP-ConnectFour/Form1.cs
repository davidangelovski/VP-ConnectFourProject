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
        static Game game = new Game();
        AIPlayer AI = new AIPlayer(game);
        Panel[,] cells = new Panel[Game.Rows, Game.Columns];
        bool singlePlayerMode = false;
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
            int row = game.GetLowermostFreeRow(col);

            if (row == -1)
            {
                return;
            }

            inputEnabled = false;
            
            
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
                setBoredFullText();
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
            int aiCol = AI.GetMove(difficulty);
            if (aiCol == -1)
            {
                return;
            }
            int aiRow = game.GetLowermostFreeRow(aiCol);
            game.DropDisc(aiRow, aiCol, game.CurrentPlayer);
            Color discColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;
            await AnimateDiscFall(aiRow, aiCol, discColor);
            if (game.CheckWin(aiRow, aiCol, game.CurrentPlayer))
            {
                p2Score++;
                UpdateScoreLabels();
                lbTurn.Text = "AI wins! Better luck next time.\nReset the board to play again!";
                inputEnabled = false;
                return;
            }

            if (game.IsBoardFull())
            {
                setBoredFullText();
                inputEnabled = false;
                return;
            }

            game.SwitchPlayer();
            switchTurnText();
            inputEnabled = true;
            
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
        private void setBoredFullText() 
        {
            lbTurn.Text = "It's a tie!\nReset the board to play again";
            lbTurn.ForeColor = Color.White;
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
            ResetScores();
            Reset();
        }

        private void rbIntermediate_Click(object sender, EventArgs e)
        {
            difficulty = "Intermediate";
            ResetScores();
            Reset();
        }

        private void rbAdvanced_Click(object sender, EventArgs e)
        {
            difficulty = "Advanced";
            ResetScores();
            Reset();
        }
        private void rbExtreme_Click(object sender, EventArgs e)
        {
            difficulty = "Extreme";
            ResetScores();

            Reset();
        }

        private void rbSingle_Click(object sender, EventArgs e)
        {
            rbSingle.Checked = true;
            rbMulti.Checked = false;
            singlePlayerMode = true;
            gbDifficulty.Visible = true;
            ResetScores();
            Reset();
        }

        private void rbMulti_Click(object sender, EventArgs e)
        {
            rbMulti.Checked = true;
            rbSingle.Checked = false;
            singlePlayerMode = false;
            gbDifficulty.Visible = false;
            ResetScores();
            Reset();
        }


        private void btResetScores_Click(object sender, EventArgs e)
        {
            ResetScores();
        }
        private void ResetScores() 
        {
            p1Score = 0;
            p2Score = 0;
            UpdateScoreLabels();
        }

       
    }
}
