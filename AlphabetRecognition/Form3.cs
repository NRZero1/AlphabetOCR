using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KNN_Training
{
    public partial class Form3 : Form
    {
        public string filePath;
        public Form3(string filePath)
        {
            this.filePath = filePath;
            InitializeComponent();
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            DataTable dt = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(filePath, System.Text.Encoding.GetEncoding(932));

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
}
