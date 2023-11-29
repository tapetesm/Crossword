using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Crossword
{
    public partial class Form1 : Form
    {
        Form2 form2 = new Form2();

        List<IdCells> idCells = new List<IdCells>();
        public string file = Application.StartupPath + "\\puzzle.pzl";

        public Form1()
        {
            BuildWordList();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBoard();
            
            form2.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
            form2.StartPosition = FormStartPosition.Manual;
            form2.Show();
            form2.dataGridView1.AutoResizeColumns();
        }

        private void BuildWordList()
        {
            string line = "";
            using (StreamReader sr = new StreamReader(file))
            {
                line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null) 
                {
                    string[] l = line.Split('|');
                    idCells.Add(new IdCells(Int32.Parse(l[0]), Int32.Parse(l[1]), l[2], l[3], l[4], l[5]));
                    form2.dataGridView1.Rows.Add(new string[] { l[3], l[2], l[5] });
                }
            }
        }

        private void InitializeBoard()
        {
            dataGridView1.BackgroundColor = Color.Black;
            dataGridView1.DefaultCellStyle.BackColor = Color.Black;
            for (int i = 0; i < 20; i++)
            {
                dataGridView1.Rows.Add();
            }

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.Width = dataGridView1.Width / dataGridView1.Columns.Count;
            }
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                r.Height = dataGridView1.Height / dataGridView1.Rows.Count;
            }
            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                for (int col = 0; col < dataGridView1.Columns.Count; col++)
                {
                    dataGridView1[col, row].ReadOnly = true;
                }
            }

            foreach (IdCells i in idCells)
            {
                int start_col = i.X;
                int start_row = i.Y;
                char[] word = i.word.ToCharArray();
                for (int j = 0; j < word.Length; j++)
                {
                    if (i.direction.ToUpper() == "ПО ГОРИЗОНТАЛИ")
                    {
                        FormatCell(start_row, start_col + j, word[j].ToString());
                    }
                    else if (i.direction.ToUpper() == "ПО ВЕРТИКАЛИ")
                    {
                        FormatCell(start_row + j, start_col, word[j].ToString());
                    }
                }
            }
        }

        private void FormatCell(int row, int col, string letter)
        {
            DataGridViewCell c = dataGridView1[col, row];
            c.Style.BackColor = Color.White;
            c.ReadOnly = false;
            c.Style.SelectionBackColor = Color.Cyan;
            c.Tag = letter;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper();
                if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Length > 1)
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Substring(0, 1);
                }
                if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Equals(dataGridView1[e.ColumnIndex, e.RowIndex].Tag.ToString().ToUpper()))
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkGreen;
                }
                else
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.DarkRed;
                }
            }
            catch
            {

            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string number = "";

            if (idCells.Any(c => (number = c.number) != "" && c.X == e.ColumnIndex && c.Y == e.RowIndex)) 
            {
                Rectangle r = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);
                e.Graphics.FillRectangle(Brushes.White, r);
                Font f = new Font(e.CellStyle.Font.FontFamily, 7);
                e.Graphics.DrawString(number, f, Brushes.Black, r);
                e.PaintContent(e.ClipBounds);
                e.Handled = true;
            }
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            form2.SetDesktopLocation(this.Location.X + this.Width + 1, this.Location.Y);
        }
    }

    public class IdCells
    {
        public int X;
        public int Y;
        public string direction;
        public string number;
        public string word;
        public string clue;

        public IdCells(int x, int y, string d, string n, string w, string c)
        {
            this.X = x;
            this.Y = y;
            this.direction = d;
            this.number = n;
            this.word = w;
            this.clue = c;
        }
    }
}
