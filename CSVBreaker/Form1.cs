using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CSVBreaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            try
            {
                prgsBar.Visible = true;

                prgsBar.Maximum = 100;
                prgsBar.Step = 1;
                prgsBar.Value = 1;

                int MAX = Convert.ToInt32(txtNoOfrecords.Text.Trim());

                using (var reader = new StreamReader(txtInputFilePath.Text.Trim()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    string outFilePath = txtInputFilePath.Text.Trim().Substring(txtInputFilePath.Text.Trim().LastIndexOf("\\") + 1);
                    outFilePath = outFilePath.Substring(0, outFilePath.Count() - 4);

                    csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header;
                    csv.Configuration.HeaderValidated = null;
                    csv.Configuration.Delimiter = txtInputFileDelimeter.Text.Trim();
                    csv.Configuration.BadDataFound = null;

                    var records = csv.GetRecords<dynamic>().ToList();
                    int range = 0;

                    if (records.Count > MAX)
                    {
                        range = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(records.Count) / MAX));
                    }

                    prgsBar.Value = 100 - range;

                    int start = 0, end = 0;

                    for (int i = 0; i < range; i++)
                    {
                        start = MAX * i + (i == 0 ? 0 : 1);
                        end = records.Count - start >= MAX ? MAX : records.Count - start;
                        var r = records.GetRange(start, end);


                        using (var writer = new StreamWriter($"{txtOutputFilePath.Text.Trim()}{outFilePath}{i + 1}.csv"))
                        using (var new_csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            new_csv.Configuration.Delimiter = txtOutputFileDelimeter.Text.Trim();
                            new_csv.WriteRecords(r);
                        }
                        prgsBar.Value++;
                    }

                    prgsBar.Value = 100;
                    MessageBox.Show("Split completed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtOutputFilePath.Text = "";
            txtInputFilePath.Text = "";
            txtInputFileDelimeter.Text = "";
            txtNoOfrecords.Text = "";
            txtOutputFileDelimeter.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
