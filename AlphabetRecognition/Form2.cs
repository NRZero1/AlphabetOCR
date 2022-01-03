using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KNN_Training
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public double[,] matrixPopulation;
        double aspectRatio;
        string CSVFileName;
        string[] lines;

        private void openCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV Files|*.csv";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVFileName = openFileDialog1.FileName;
                lines = System.IO.File.ReadAllLines(CSVFileName, System.Text.Encoding.GetEncoding(932));
                DataTable dt = new DataTable();

                if (lines.Length > 0)
                {
                    string firstLine = lines[0];

                    string[] header = firstLine.Split(',');

                    foreach (string headerTables in header)
                    {
                        dt.Columns.Add(new DataColumn(headerTables));
                    }

                    for (int i = 1; i < lines.Length; i++)
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
                dataGridView1.DataSource = dt;
            }
        }

        private void openTestImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private int[,] Grayscale(int width, int height)
        {
            int[,] grayscale_data = new int[width, height];
            Bitmap bmp = new Bitmap(pictureBox1.Image);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    grayscale_data[x, y] = (int)Math.Round(((0.2989 * color.R) + (0.587 * color.G) + (0.114 * color.B)));
                }
            }

            return grayscale_data;
        }

        private bool[,] Biner(int[,] gray_data)
        {
            bool[,] binary_data = new bool[gray_data.GetLength(0), gray_data.GetLength(1)];

            for (int y = 0; y < gray_data.GetLength(1); y++)
            {
                for (int x = 0; x < gray_data.GetLength(0); x++)
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

            return binary_data;
        }

        boundary BoundingBox(bool[,] biner)
        {
            int xmintemp, xmaxtemp;
            int ymintemp, ymaxtemp;

            boundary boundingBox;

            boundingBox.xmin = 0;
            boundingBox.xmax = 0;
            boundingBox.ymin = 0;
            boundingBox.ymax = 0;

            for (int y = 0; y < biner.GetLength(1); y++)
            {
                for (int x = 0; x < biner.GetLength(0); x++)
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

            return boundingBox;
        }

        double[,] Population(boundary Boundary, bool[,] biner)
        {
            float matrixWidth;
            float matrixHeight;
            bool[,] matrixResult = new bool[Boundary.characterWidth, Boundary.characterHeight];
            double[,] result = new double[5, 5];

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    result[x, y] = 0;
                }
            }

            matrixWidth = (float)Boundary.characterWidth / 5;
            matrixHeight = (float)Boundary.characterHeight / 5;
            aspectRatio = (float)Boundary.characterWidth / (float)Boundary.characterHeight;

            for (int y = 0; y < Boundary.characterHeight; y++)
            {
                for (int x = 0; x < Boundary.characterWidth; x++)
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

            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    int n = 0;

                    for (int y = (int)MathF.Round(j * matrixHeight); y < (int)MathF.Round((j + 1) * matrixHeight); y++)
                    {
                        for (int x = (int)MathF.Round(i * matrixWidth); x < (int)MathF.Round((i + 1) * matrixWidth); x++)
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

            return result;
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool @isImageNull = pictureBox1.Image == null;

            if (!isImageNull)
            {
                if (CSVFileName != null)
                {
                    richTextBox1.Text = "";
                    richTextBox2.Text = "";
                    richTextBox3.Text = "";
                    richTextBox4.Text = "";

                    Bitmap bmp = new Bitmap(pictureBox1.Image);
                    int width = bmp.Width;
                    int height = bmp.Height;

                    int[,] gray_data = Grayscale(width, height);
                    bool[,] biner_data = Biner(gray_data);
                    boundary boundingBox = BoundingBox(biner_data);
                    matrixPopulation = Population(boundingBox, biner_data);

                    Classify();
                }
                else
                {
                    MessageBox.Show("CSV File has not been opened", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Image cannot be null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Classify()
        {
            List<string> populationListShaped = new List<string>();
            List<double> AspectRatio = new List<double>();
            List<string> Character = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] rowData = lines[i].Split(',');

                double a = Convert.ToDouble(rowData[2]);
                populationListShaped.Add(rowData[1]);
                Character.Add(rowData[0]);
                AspectRatio.Add(a);
            }

            double[,,] populationMatrixShaped = new double[populationListShaped.Count, 5, 5];

            for (int n = 0; n < populationListShaped.Count; n++)
            {
                string[] temp = populationListShaped[n].Split(';');
                int indexCounter = 0;

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        //richTextBox1.Text += temp[indexCounter] + "\r\n";
                        double t = Convert.ToDouble(temp[indexCounter]);
                        populationMatrixShaped[n, i, j] = t;
                        indexCounter++;
                    }
                }
            }

            double[] distanceList = new double[populationListShaped.Count];
            double distanceRatio;
            double[,] distanceMatrix = new double[5, 5];

            for (int n = 0; n < populationListShaped.Count; n++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        distanceMatrix[i, j] = Math.Abs(populationMatrixShaped[n, i, j] - matrixPopulation[i, j]);
                    }
                }
                distanceRatio = Math.Abs(AspectRatio[n] - aspectRatio);

                for (int j = 0; j < 5; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        distanceList[n] += distanceMatrix[i, j];
                    }    
                }
                distanceList[n] += distanceRatio;
            }

            int k = 3;

            double[] distanceListTemp = new double[distanceList.Length];
            int[] bestIndex = new int[k];

            Array.Copy(distanceList, distanceListTemp, distanceList.Length);

            Array.Sort(distanceListTemp);

            for (int i = 0; i < k; i ++)
            {
                richTextBox1.Text += distanceListTemp[i] + "\r\n";
            }

            for (int x = 0; x < k; x ++)
            {
                bestIndex[x] = Array.IndexOf(distanceList, distanceListTemp[x]);
            }

            /*for (int i = 0; i < distanceList.Length; i ++)
            {
                richTextBox1.Text += distanceList[i].ToString() + "\r\n";
                richTextBox2.Text += distanceListTemp[i].ToString() + "\r\n";
            }*/

            for (int i = 0; i < k; i ++)
            {
                richTextBox2.Text += bestIndex[i] + "\r\n";
                richTextBox3.Text += Character[bestIndex[i]] + "\r\n";
            }

            int count = 1;
            int tempCount;
            string frequentCharacter = Character[bestIndex[0]];
            string tempCharacter;

            for (int i = 0; i < k; i ++)
            {
                tempCharacter = Character[bestIndex[i]];
                tempCount = 0;
                
                for (int j = 0; j < k; j ++)
                {
                    if (tempCharacter == Character[bestIndex[j]])
                    {
                        tempCount++;
                    }
                }
                
                if (tempCount > count)
                {
                    frequentCharacter = tempCharacter;
                    count = tempCount;
                }
            }

            richTextBox4.Text = frequentCharacter;
        }
    }
}
