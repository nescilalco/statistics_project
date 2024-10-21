using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using static System.Net.Mime.MediaTypeNames;
using WinForms = System.Windows.Forms;

namespace HackerPenetration
{
    public class MainForm : Form
    {
        private TextBox txtServers, txtAttackers, txtProbability;
        private Button btnSimulate;
        private PictureBox pictureBox;

        private int n, m;
        private double p;

        HistogramForm histogramForm = null;

        public MainForm()
        {
            this.Text = "Hacker Penetration Simulation";
            this.Size = new Size(800, 600);

            // unputs labels
            WinForms.Label lblServers = new WinForms.Label() { Text = "Number of Servers (n):", Location = new Point(10, 10), Width = 180 };
            WinForms.Label lblAttackers = new WinForms.Label() { Text = "Number of Attackers (m):", Location = new Point(10, 40), Width = 180 };
            WinForms.Label lblProbability = new WinForms.Label() { Text = "Penetration Probability (p):", Location = new Point(10, 70), Width = 180 };

            // txtboxes
            txtServers = new TextBox() { Location = new Point(200, 10), Width = 100 };
            txtAttackers = new TextBox() { Location = new Point(200, 40), Width = 100 };
            txtProbability = new TextBox() { Location = new Point(200, 70), Width = 100 };

            txtServers.Text = "10";
            txtAttackers.Text = "5";
            txtProbability.Text = "0,3"; 

            // start simulation
            btnSimulate = new Button() { Text = "Simulate", Location = new Point(400, 40), Width = 100 };
            btnSimulate.Click += BtnSimulate_Click;

            // display graphical output
            pictureBox = new PictureBox() { Location = new Point(10, 100), Size = new Size(760, 450), BorderStyle = BorderStyle.FixedSingle };

            // controls
            this.Controls.Add(lblServers);
            this.Controls.Add(lblAttackers);
            this.Controls.Add(lblProbability);
            this.Controls.Add(txtServers);
            this.Controls.Add(txtAttackers);
            this.Controls.Add(txtProbability);
            this.Controls.Add(btnSimulate);
            this.Controls.Add(pictureBox);
        }

        // event handler for the Simulate button click
        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            // close the previous histogram if it exists
            histogramForm?.Close();
            
            // input validation
            if (int.TryParse(txtServers.Text, out n) && int.TryParse(txtAttackers.Text, out m) && double.TryParse(txtProbability.Text, out p))
            {
                SimulatePenetration();
            }
            else
            {
                MessageBox.Show("Please enter valid values.");
            }
        }

        private void SimulatePenetration()
        {
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            Random random = new Random();
            int[,] results = new int[m, n]; // matrix - m attackers n servers
            int[] successCounts = new int[m]; // successes for each attacker

            // attack simulation
            for (int i = 0; i < m; i++)
            {
                int successes = 0;
                for (int j = 0; j < n; j++)
                {
                    if (random.NextDouble() <= p)
                    {
                        successes++;
                    }
                    results[i, j] = successes;
                }
                successCounts[i] = successes;
            }

            // graph dimensions and scaling
            int stepX = pictureBox.Width / (n + 1); // X-axis step for each server
            int graphHeight = pictureBox.Height - 150; // space for histogram
            int maxSuccesses = m; // max number of successes to scale Y-axis
            float scaleY = (float)(graphHeight - 50) / maxSuccesses; // Y-axis based on max successes

            Color[] colors = { Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Brown, Color.Pink };

            // grid
            for (int j = 0; j <= n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawLine(Pens.LightGray, x, 50, x, graphHeight + 50); // Adjust y positions for grid
            }

            // horizontal grid lines based on max succ
            for (int i = 0; i <= maxSuccesses; i++)
            {
                int y = graphHeight - (int)(i * scaleY);
                g.DrawLine(Pens.LightGray, 10, y, pictureBox.Width - 10, y);
            }

            // X-axis markers (1 to n)
            for (int j = 0; j < n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawString($"{j + 1}", this.Font, Brushes.Black, x - 10, pictureBox.Height - 40);
            }

            // line graph for each hacker
            for (int i = 0; i < m; i++)
            {
                Pen pen = new Pen(colors[i % colors.Length], 2);
                for (int j = 0; j < n - 1; j++)
                {
                    int x1 = stepX * (j + 1);
                    int y1 = graphHeight - (int)(results[i, j] * scaleY);
                    int x2 = stepX * (j + 2);
                    int y2 = graphHeight - (int)(results[i, j + 1] * scaleY);

                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }

            g.DrawString("Number of Successful Penetrations", new System.Drawing.Font(this.Font.FontFamily, 10), Brushes.Black, 10, 10);
            g.RotateTransform(90);
            g.DrawString("Number of Successful Penetrations", this.Font, Brushes.Black, -pictureBox.Height / 2, 10);
            g.ResetTransform();

            // Y-axis markers for cumulative succ
            for (int i = 0; i <= maxSuccesses; i++)
            {
                int y = graphHeight - (int)(i * scaleY);
                g.DrawString(i.ToString(), this.Font, Brushes.Black, 5, y - 10);
            }

            g.DrawString("Servers", this.Font, Brushes.Black, pictureBox.Width / 2 - 40, pictureBox.Height - 20);

            pictureBox.Image = bitmap;

            ShowHistogram(successCounts);
        }



        private void ShowHistogram(int[] successCounts)
        {
            histogramForm = new HistogramForm(successCounts);
            histogramForm.Show();
        }



        [STAThread]
        static void Main()
        {
            WinForms.Application.Run(new MainForm());
        }
    }
}
