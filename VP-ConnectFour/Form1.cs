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
        private const int Rows = 6;
        private const int Columns = 7;
        private Panel[,] cells = new Panel[Rows, Columns];
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Panel cell = (Panel)tlpGameGrid.GetControlFromPosition(col, row);
                    cell.BackColor = Color.White;
                    cells[row, col] = cell;
                }
            }

        }
    }
}
