using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
                    cells[row, col] = cell;
                }
            }

        }
        private void Cell_Click(object sender, EventArgs e)
        {
            Panel clickedCell = (Panel)sender;
            int col = (int)clickedCell.Tag;
            int row = game.DropDisc(col);
            if (row != -1)
            {
                cells[row, col].BackColor = game.CurrentPlayer == 1 ? Color.Red : Color.Yellow;

                if (game.CheckWin(row, col))
                {
                    MessageBox.Show($"Player {game.CurrentPlayer} wins!");
                    Reset();
                    return;
                }

                game.SwitchPlayer();
            }
          
        }
        private void Reset() {
            for (int row = 0; row < Game.Rows; row++)
            {
                for (int col = 0; col < Game.Columns; col++)
                {
                    cells[row, col].BackColor = Color.White;
                }
            }
            game.Reset();
        }



    }
}
