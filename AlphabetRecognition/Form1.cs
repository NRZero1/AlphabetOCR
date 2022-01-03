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

namespace KNN_Training
{
    struct boundary
    {
        public int xmin;
        public int xmax;
        public int ymin;
        public int ymax;
        public int characterWidth;
        public int characterHeight;
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            InitializeComponent();
        }

        public string CSVFileName;
        public double[,] matrixPopulation;
        double aspectRatio;
        //public string[] lines;

        private void openCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Files|*.csv";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVFileName = openFileDialog1.FileName;
                string[] lines = System.IO.File.ReadAllLines(CSVFileName, System.Text.Encoding.GetEncoding(932));
            }
        }

        /*private void openCSV(string filePath)
        {
            //DataTable dt = new DataTable();
            

            if (lines.Length > 0)
            {
                string firstLine = lines[0];

                string[] header = firstLine.Split(',');

                foreach (string headerTables in header)
                {
                    dt.Columns.Add(new DataColumn(headerTables));
                }

                for (int i = 1; i < lines.Length; i ++)
                {
                    string[] data = lines[i].Split(',');
                    DataRow row = dt.NewRow();
                    int columnIndex = 0;

                    foreach (string headerTables in header)
                    {
                        row[headerTables] = data[columnIndex++];
                    }

                    dt.Rows.Add(row);
                }
            }
            dataGridView2.DataSource = dt;
        }*/

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        int[,] Grayscale(int width, int height)
        {
            int[,] grayscale_data = new int[width, height];
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            
            for (int y = 0; y < height; y ++)
            {
                for (int x = 0; x < width; x ++)
                {
                    Color color = bmp.GetPixel(x, y);
                    grayscale_data[x, y] = (int)Math.Round(((0.2989 * color.R) + (0.587 * color.G) + (0.114 * color.B)));
                }
            }

            /*for (int y = 0; y < height; y ++)
            {
                for (int x = 0; x < width; x ++)
                {
                    Color gray = Color.FromArgb(grayscale_data[x, y], grayscale_data[x, y], grayscale_data[x, y]);
                    ((Bitmap)pictureBox1.Image).SetPixel(x, y, gray);
                }
            }*/
            return grayscale_data;
        }

        bool[,] Biner(int[,] gray_data)
        {
            bool[,] binary_data = new bool[gray_data.GetLength(0), gray_data.GetLength(1)];

            for (int y = 0; y < gray_data.GetLength(1); y ++)
            {
                for (int x = 0; x < gray_data.GetLength(0); x ++)
                {
                    if (gray_data[x, y] > 127)
                    {
                        binary_data[x, y] = true;
                    }
                    else
                    {
                        binary_data[x, y] = false;
                    }
                }
            }
            
            /*for (int y = 0; y < height; y ++)
            {
                for (int x = 0; x < width; x ++)
                {
                    if (binary_data[x, y] == false)
                    {
                        ((Bitmap)pictureBox1.Image).SetPixel(x, y, Color.Black);
                    }
                }
            }*/
            return binary_data;
        }

        boundary BoundingBox(bool[,] biner)
        {
            int xmintemp, xmaxtemp;
            int ymintemp, ymaxtemp;

            boundary boundingBox = new boundary();

            boundingBox.xmin = 0;
            boundingBox.xmax = 0;
            boundingBox.ymin = 0;
            boundingBox.ymax = 0;
            
            for (int y = 0; y < biner.GetLength(1); y ++)
            {
                for (int x = 0; x < biner.GetLength(0); x ++)
                {
                    if (biner[x, y] == false)
                    {
                        if (boundingBox.xmin == 0)
                        {
                            boundingBox.xmin = x;
                        }
                        else if (boundingBox.xmin != 0)
                        {
                            xmintemp = x;
                            if (xmintemp < boundingBox.xmin)
                            {
                                boundingBox.xmin = xmintemp;
                            }
                        }

                        if (boundingBox.xmax == 0)
                        {   
                            boundingBox.xmax = x;
                        }
                        else if (boundingBox.xmax != 0)
                        {
                            xmaxtemp = x;

                            if (xmaxtemp > boundingBox.xmax)
                            {
                                boundingBox.xmax = xmaxtemp;
                            }
                        }

                        if (boundingBox.ymin == 0)
                        {
                            boundingBox.ymin = y;
                        }
                        else if (boundingBox.ymin != 0)
                        {
                            ymintemp = y;
                            if (ymintemp < boundingBox.ymin)
                            {
                                boundingBox.ymin = ymintemp;
                            }
                        }
                        
                        if (boundingBox.ymax == 0)
                        {
                            boundingBox.ymax = y;
                        }
                        else if (boundingBox.ymax != 0)
                        {
                            ymaxtemp = y;
                            if (ymaxtemp > boundingBox.ymax)
                            {
                                boundingBox.ymax = ymaxtemp;
                            }
                        }
                    }
                }
            }
            boundingBox.characterWidth = (boundingBox.xmax - boundingBox.xmin) + 1;
            boundingBox.characterHeight = (boundingBox.ymax - boundingBox.ymin) + 1;
            //Graphics g = Graphics.FromImage(((Bitmap)pictureBox1.Image));
            //Pen blackPen = new Pen(Color.Black, 10);
            //g.DrawRectangle(blackPen, boundingBox.xmin, boundingBox.ymin, boundingBox.xmax, boundingBox.ymax);
            /*for (int x = boundingBox.xmin; x <= boundingBox.xmax; x ++)
            {
                ((Bitmap)pictureBox1.Image).SetPixel(x, boundingBox.ymin, Color.Black);
            }

            for (int y = boundingBox.ymin; y <= boundingBox.ymax; y ++ )
            {
                ((Bitmap)pictureBox1.Image).SetPixel(boundingBox.xmin, y, Color.Black);
            }

            for (int x = boundingBox.xmin; x <= boundingBox.xmax; x ++)
            {
                ((Bitmap)pictureBox1.Image).SetPixel(x, boundingBox.ymax, Color.Black);
            }

            for (int y = boundingBox.ymin; y <= boundingBox.ymax; y ++)
            {
                ((Bitmap)pictureBox1.Image).SetPixel(boundingBox.xmax, y, Color.Black);
            }*/

            return boundingBox;
        }

        double[,] Population(boundary Boundary, bool[,] biner)
        {
            float matrixWidth;
            float matrixHeight;
            bool[,] matrixResult = new bool[Boundary.characterWidth, Boundary.characterHeight];            
            double[,] result = new double[5, 5];
            /*double eachValue;
            int CounterX;
            int CounterY;
            int IndexX;
            int IndexY;*/
            //double totalResult;

            for (int y = 0; y < 5; y ++)
            {
                for (int x = 0; x < 5; x ++)
                {
                    result[x, y] = 0;
                }
            }

            matrixWidth = (float)Boundary.characterWidth / 5;
            matrixHeight = (float)Boundary.characterHeight / 5;
            aspectRatio = (float)Boundary.characterWidth / (float)Boundary.characterHeight;

            //mul = matrixWidth * matrixHeight;
            //div = 1 / mul;
            //eachValue = (1.0 / (matrixWidth * matrixHeight)) * 100.0;


            for (int y = 0; y < Boundary.characterHeight; y ++)
            {
                for (int x = 0; x < Boundary.characterWidth; x ++)
                {
                    if (biner[x + Boundary.xmin, y + Boundary.ymin] == false)
                    {
                        matrixResult[x, y] = false;
                    }
                    else
                    {
                        matrixResult[x, y] = true;
                    }
                }
            }

            for (int j = 0; j < 5; j ++)
            {
                for (int i = 0; i < 5; i ++)
                {
                    int n = 0;
                    
                    for (int y = (int)MathF.Round(j * matrixHeight); y < (int)MathF.Round((j + 1) * matrixHeight); y ++)
                    {
                        for (int x = (int)MathF.Round(i * matrixWidth); x < (int)MathF.Round((i + 1) * matrixWidth); x ++)
                        {
                            if (matrixResult[x, y] == false)
                            {
                                result[i, j] += 1;
                            }
                            n += 1;
                        }
                    }
                    result[i, j] = (result[i, j] / (double)n) * 100.0;
                }
            }

            /*CounterX = 1;
            CounterY = 1;

            for (int y = 0; y < Boundary.characterHeight; y ++)
            {
                for (int x = 0; x < Boundary.characterWidth; x ++)
                {
                    if (matrixResult[x, y] == false)
                    {
                        IndexX = CounterX - 1;
                        IndexY = CounterY - 1;
                        result[IndexX, IndexY] += eachValue;
                    }

                    if (x == CounterX * matrixWidth - 1)
                    {
                        CounterX += 1;
                    }
                    else if (CounterX > 5)
                    {
                        CounterX = 1;
                    }
                }
                if (y == CounterY * matrixHeight - 1)
                {
                    CounterY += 1;
                }
            }

            totalResult = 0;
            for (int y = 0; y < 5; y ++)
            {
                for (int x = 0; x < 5; x ++)
                {
                    result[x, y] = Math.Round(result[x, y], 3);
                }
            }*/

            //txtJapChar.Text = aspectRatio.ToString();
            //txtCharacterName.Text = matrixHeight.ToString();
            txtRatio.Text = aspectRatio.ToString();

            return result;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isImageNull = pictureBox1.Image == null;

            if (!isImageNull)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                int width = bmp.Width;
                int height = bmp.Height;

                int[,] gray_data = Grayscale(width, height);
                bool[,] biner_data = Biner(gray_data);
                boundary boundingBox = BoundingBox(biner_data);
                matrixPopulation = Population(boundingBox, biner_data);
            }
            else
            {
                MessageBox.Show("Image cannot be null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addRecord(string characterName)
        {
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(CSVFileName, true, System.Text.Encoding.UTF8))
            {
                string populationLine = "";
                for (int j = 0; j < 5; j ++)
                {
                    for (int i = 0; i < 5; i ++)
                    {
                        if (i == 4 && j == 4)
                        {
                            populationLine += matrixPopulation[i, j].ToString();
                        }
                        else
                        {
                            populationLine += matrixPopulation[i, j].ToString() + ";";
                        }
                    }
                }
                write.WriteLine(characterName + "," + populationLine + "," + aspectRatio);
                Console.WriteLine(populationLine);
                Console.ReadLine();
            }
        }

        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CSVFileName == null)
            {
                MessageBox.Show("CSV File has not been opened", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool isImageNull = pictureBox1.Image == null;
                if (!isImageNull)
                {
                    //addRecord(txtCharacterName.Text, cmbCharacterType.SelectedItem.ToString(), txtJapChar.Text);
                    addRecord(txtCharacterName.Text);
                }
                else
                {
                    MessageBox.Show("Image cannot be null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }   
        }

        private void showCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var CSVForm = new Form3(CSVFileName);
            if (CSVFileName == null)
            {
                MessageBox.Show("CSV File has not been opened", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CSVForm.Show();
            }
        }

        private void testKNNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var testKNN = new Form2();
            testKNN.Show();
        }
    }
}
