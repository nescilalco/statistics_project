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
        private TextBox txtServers, txtAttackers, txtProbability, txtIntermediateSteps;
        private Button btnSimulate;
        private PictureBox pictureBox;
        private PictureBox pictureBoxFrequency;
        private List<Form> openHistogramForms = new List<Form>();

        private int n, m, n_steps;
        private double p;

        HistogramForm histogramForm = null;

        public MainForm()
        {
            this.Text = "Hacker Penetration Simulation";
            this.Size = new Size(1600, 800);

            // unputs labels
            WinForms.Label lblServers = new WinForms.Label() { Text = "Number of Servers (n):", Location = new Point(10, 10), Width = 180 };
            WinForms.Label lblAttackers = new WinForms.Label() { Text = "Number of Attackers (m):", Location = new Point(10, 40), Width = 180 };
            WinForms.Label lblProbability = new WinForms.Label() { Text = "Penetration Probability (p):", Location = new Point(10, 70), Width = 180 };
            WinForms.Label lblIntermediateSteps = new WinForms.Label() { Text = "Intermediate Steps:", Location = new Point(10, 100), Width = 180 };


            // txtboxes
            txtServers = new TextBox() { Location = new Point(200, 10), Width = 100 };
            txtAttackers = new TextBox() { Location = new Point(200, 40), Width = 100 };
            txtProbability = new TextBox() { Location = new Point(200, 70), Width = 100 };
            txtIntermediateSteps = new TextBox() { Location = new Point(200, 100), Width = 100 };

            txtServers.Text = "20";
            txtAttackers.Text = "5";
            txtProbability.Text = "0,5";
            txtIntermediateSteps.Text = "10";

            // start simulation
            btnSimulate = new Button() { Text = "Simulate", Location = new Point(400, 40), Width = 100 };
            btnSimulate.Click += BtnSimulate_Click;

            // display graphical output
            pictureBox = new PictureBox() { Location = new Point(10, 150), Size = new Size(800, 600), BorderStyle = BorderStyle.FixedSingle };

            // picturebox for frequency graphs
            pictureBoxFrequency = new PictureBox() { Location = new Point(850, 150), Size = new Size(700, 600), BorderStyle = BorderStyle.FixedSingle };

            // controls
            this.Controls.Add(lblServers);
            this.Controls.Add(lblAttackers);
            this.Controls.Add(lblProbability);
            this.Controls.Add(lblIntermediateSteps);
            this.Controls.Add(txtServers);
            this.Controls.Add(txtAttackers);
            this.Controls.Add(txtProbability);
            this.Controls.Add(txtIntermediateSteps);
            this.Controls.Add(btnSimulate);
            this.Controls.Add(pictureBox);
            this.Controls.Add(pictureBoxFrequency);
        }

        // handler for the Simulate button click
        private void BtnSimulate_Click(object sender, EventArgs e)
        {
            // close the previous histograms if it exists
            foreach (var form in openHistogramForms)
            {
                form.Close();
            }
            // clear list after closing forms
            openHistogramForms.Clear();

            // Input validation
            if (int.TryParse(txtServers.Text, out n) && 
                int.TryParse(txtAttackers.Text, out m) &&
                double.TryParse(txtProbability.Text, out p) && 
                int.TryParse(txtIntermediateSteps.Text, out n_steps))
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
            int[,] results = new int[m, n]; // Matrix - m attackers n servers
            int[] finalPositions = new int[m];
            int[] positionAtNSteps = new int[m];
            int[] totalSuccessfulJumps = new int[n]; // successful jumps for each server


            // Attack simulation
            for (int i = 0; i < m; i++)
            {
                int currentPosition = 0; // start at position 0 for each attacker
                for (int j = 0; j < n; j++)
                {
                    // step up or down based on probability p
                    if (random.NextDouble() < p)
                    {
                        currentPosition = Math.Min(currentPosition + 1, 20); // Cap at 20
                        totalSuccessfulJumps[j]++;
                    }
                    else
                    {
                        currentPosition = Math.Max(currentPosition - 1, -20); // Floor at -20
                    }
                    results[i, j] = currentPosition; // update position

                    // if we reach n_steps, save position
                    if (j + 1 == n_steps)
                    {
                        positionAtNSteps[i] = currentPosition;
                    }
                }
                finalPositions[i] = currentPosition;
            }

            // relative frequencies
            double[] relativeSuccessfulJumps = new double[n];
            for (int j = 0; j < n; j++)
            {
                relativeSuccessfulJumps[j] = (double)totalSuccessfulJumps[j] / m; // normalize successes
            }


            // Graph dimensions and scaling
            int stepX = pictureBox.Width / (n + 1); // X-axis step for each server
            int graphTop = 300; // Increased top position for the graph
            int graphHeight = pictureBox.Height - graphTop - 50; // Space for histogram at the bottom
            int fixedMaxSuccesses = 20; // Fixed max number of successes for Y-axis
            float scaleY = (float)(graphHeight) / fixedMaxSuccesses; // Scale Y-axis based on fixed max successes

            Color[] colors = { Color.Blue, Color.Green, Color.Orange, Color.Purple, Color.Brown, Color.Pink };

            // vertical grid lines
            for (int j = 0; j <= n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawLine(Pens.LightGray, x, 50, x, graphTop + graphHeight); // Adjust y positions for grid
            }

            // horizontal grid lines
            for (int i = -fixedMaxSuccesses; i <= fixedMaxSuccesses; i++)
            {
                int y = graphTop + (int)((-i) * scaleY);
                g.DrawLine(Pens.LightGray, 10, y, pictureBox.Width - 10, y);
            }

            // X-axis markers
            for (int j = 0; j < n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawString($"{j + 1}", this.Font, Brushes.Black, x - 10, pictureBox.Height - 40);
            }

            // Y-axis markers
            for (int i = -fixedMaxSuccesses; i <= fixedMaxSuccesses; i++)
            {
                int y = graphTop + (int)((-i) * scaleY); // Invert the Y position
                g.DrawString(i.ToString(), this.Font, Brushes.Black, 5, y - 10);
            }


            // Line graph for each hacker
            for (int i = 0; i < m; i++)
            {
                Pen pen = new Pen(colors[i % colors.Length], 2);
                for (int j = 0; j < n - 1; j++)
                {
                    int x1 = stepX * (j + 1);
                    int y1 = graphTop + (int)((-results[i, j]) * scaleY); // Invert Y for results
                    int x2 = stepX * (j + 2);
                    int y2 = graphTop + (int)((-results[i, j + 1]) * scaleY); // Invert Y for results

                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }

            g.DrawString("Number of Successful Penetrations", new System.Drawing.Font(this.Font.FontFamily, 10), Brushes.Black, 10, 10);
            g.RotateTransform(90);
            g.DrawString("Number of Successful Penetrations", this.Font, Brushes.Black, -pictureBox.Height / 2, 10);
            g.ResetTransform();

            
            g.DrawString("Servers", this.Font, Brushes.Black, pictureBox.Width / 2 - 40, pictureBox.Height - 20);

            pictureBox.Image = bitmap;

            ShowHistogram(positionAtNSteps, "Histogram at Intermediate Steps");
            ShowHistogram(finalPositions, "Histogram at Final Step");

            // Plot Absolute and Relative Frequencies
            PlotFrequencies(totalSuccessfulJumps, relativeSuccessfulJumps);
        }


        private void ShowHistogram(int[] positions, string title)
        {
            histogramForm = new HistogramForm(positions);
            histogramForm.Text = title;
            //histogramForm.Size = new Size(500, 400);
            histogramForm.Show();
            openHistogramForms.Add(histogramForm);
        }

        // Function to calculate absolute and relative frequencies and plot frequency graphs
        private void PlotFrequencies(int[] absoluteFrequencies, double[] relativeFrequencies)
        {
            Bitmap bitmap = new Bitmap(pictureBoxFrequency.Width, pictureBoxFrequency.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);


            // Graph dimensions and scaling
            int n = absoluteFrequencies.Length;
            int stepX = pictureBoxFrequency.Width / (n + 1); // X-axis step for each server
            int graphTop = 70;
            int graphHeight = pictureBoxFrequency.Height - graphTop - 50;
            int fixedMaxSuccesses = 60; // Fixed max number of successes for Y-axis
            float scaleY = (float)(graphHeight) / fixedMaxSuccesses; // Scale Y-axis based on fixed max successes

            // vertical grid lines for each server
            for (int j = 0; j < n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawLine(Pens.LightGray, x, graphTop, x, graphTop + graphHeight);
            }

            // horizontal grid lines based max success value
            for (int i = 0; i <= fixedMaxSuccesses; i++)
            {
                int y = graphHeight - (int)(i * scaleY) + graphTop;
                g.DrawLine(Pens.LightGray, 10, y, pictureBoxFrequency.Width - 10, y);
                g.DrawString(i.ToString(), this.Font, Brushes.Black, 5, y - 10); // Y-axis markers for successes
            }

            
            // X-axis markers (1 to n)
            for (int j = 0; j < n; j++)
            {
                int x = stepX * (j + 1);
                g.DrawString($"{j + 1}", this.Font, Brushes.Black, x - 10, pictureBoxFrequency.Height - 40);
            }

            Pen absPen = new Pen(Color.Blue, 2); // Absolute frequency (blue)
            Pen relPen = new Pen(Color.Green, 2); // Relative frequency (green)

            // points for absolute frequencies
            List<Point> absPoints = new List<Point>();

            // plot abs frequencies
            for (int j = 0; j < n; j++)
            {
                int count = absoluteFrequencies[j];

                int y = graphHeight - (int)(count * scaleY) + graphTop;
                absPoints.Add(new Point(stepX * (j + 1), y)); // point to graph
            }

            // line connecting points
            if (absPoints.Count > 1)
            {
                g.DrawLines(absPen, absPoints.ToArray());
            }

            // points for rel frequencies
            List<Point> relPoints = new List<Point>();

            // plot relative frequencies
            for (int j = 0; j < n; j++)
            {
                double ratio = relativeFrequencies[j];

                int y = graphHeight - (int)(ratio * fixedMaxSuccesses * scaleY) + graphTop;
                relPoints.Add(new Point(stepX * (j + 1), y)); // point to graph
            }

            // Draw the line for relative frequency
            if (relPoints.Count > 1)
            {
                g.DrawLines(relPen, relPoints.ToArray());
            }

            // Labels
            g.DrawString("Absolute (Blue) & Relative (Green) Frequencies", new System.Drawing.Font(this.Font.FontFamily, 10), Brushes.Black, 10, 10);
            g.DrawString("Servers", this.Font, Brushes.Black, pictureBoxFrequency.Width / 2 - 40, pictureBoxFrequency.Height - 20);
            
            g.DrawString("Successful Jumps", new System.Drawing.Font(this.Font.FontFamily, 10), Brushes.Black, 10, 40);
            g.RotateTransform(90);
            g.DrawString("Successful Jumps", this.Font, Brushes.Black, -pictureBoxFrequency.Height / 2, 10);
            g.ResetTransform();

            pictureBoxFrequency.Image = bitmap;
        }



        [STAThread]
        static void Main()
        {
            WinForms.Application.Run(new MainForm());
        }
    }
}
