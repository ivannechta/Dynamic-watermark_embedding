using System;
using System.Windows.Forms;

namespace DWM
{
    public partial class Form1 : Form
    {
        public DWMConstructor D = new DWMConstructor();
        public Form1()
        {
            InitializeComponent();            
            D.SetBlockSize(10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Выберите файл с полезным содержимым";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                D.SetContentFilePath(openFileDialog1.FileName.ToString());
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label3.Text = "" + trackBar1.Value;
            D.SetBlockSize(trackBar1.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                D.Check();
                D.GenerateDWM();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    D.SaveDWM(saveFileDialog1.FileName);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Ошибка",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Выберите файл с сигнатурой";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
                D.SetFileSign(openFileDialog1.FileName.ToString());
            }
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            label6.Text = "" + trackBar2.Value;
            D.SetDWMSubfunctions(trackBar2.Value);
        }
    }
}
