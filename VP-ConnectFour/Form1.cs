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
            int row = game.GetBottomRow(col);
            
            if (row != -1)
            {
                game.DropDisc(row, col);
                cells[row, col].BackColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;

                if (game.CheckWin(row, col, game.CurrentPlayer))
                {
                    MessageBox.Show($"Player {game.CurrentPlayer} wins!");
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
            if (aiCol != -1)
            {
                int aiRow = game.GetBottomRow(aiCol);
                game.DropDisc(aiRow, aiCol);
                cells[aiRow, aiCol].BackColor = Color.Yellow;

                if (game.CheckWin(aiRow, aiCol,game.CurrentPlayer))
                {
                    MessageBox.Show("AI wins!");
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
                if (game.GetBottomRow(col) != -1)
                    validCols.Add(col);
            }
            return validCols.Count > 0 ? validCols[rng.Next(validCols.Count)] : -1;
        }

        private int IntermediateMove() {

            
            for (int col = 0; col < Game.Columns; col++)
            {
                int testRow = game.GetBottomRow(col);
                if (testRow != -1 && game.CheckWin(testRow, col, 2))
                    return col;
            }

            for (int col = 0; col < Game.Columns; col++)
            {
                int testRow = game.GetBottomRow(col);
                if (testRow != -1 && game.CheckWin(testRow, col, 1))
                    return col;
            }

            return BeginnerMove();
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
