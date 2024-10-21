using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HackerPenetration
{
    public partial class HistogramForm : Form
    {
        private int[] successCounts;

        public HistogramForm(int[] successCounts)
        {
            InitializeComponent(); // Initializes the form components
            this.successCounts = successCounts;
            this.Load += HistogramForm_Load; // Attach Load event handler
        }

        private void HistogramForm_Load(object sender, EventArgs e)
        {
            DrawHistogram(); // Draw the histogram on load
            CalculateStatistics(); // Calculate and display statistics
        }

        private void DrawHistogram()
        {
            Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height); // Create a bitmap for the histogram
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);

            int maxSuccesses = successCounts.Max(); // Get the max success for scaling
            int barWidth = pictureBox.Width / (successCounts.Length + 1); // Width of each histogram bar

            for (int i = 0; i < successCounts.Length; i++)
            {
                int barHeight = successCounts[i] * 10; // Scale for visualization purposes
                g.FillRectangle(Brushes.Red, i * barWidth + 10, pictureBox.Height - barHeight, barWidth - 20, barHeight); // Offset the bar position
                g.DrawString(successCounts[i].ToString(), this.Font, Brushes.Black, i * barWidth + 10, pictureBox.Height - barHeight - 20); // Draw number below the bar
            }

            pictureBox.Image = bitmap; // Set the PictureBox image to the drawn bitmap
        }

        private void CalculateStatistics()
        {
            double average = successCounts.Average(); // Calculate average
            double variance = successCounts.Select(x => Math.Pow(x - average, 2)).Average(); // Calculate variance

            lblAverage.Text = $"Average: {average:F2}"; // Format to 2 decimal places
            lblVariance.Text = $"Variance: {variance:F2}"; // Format to 2 decimal places
        }
    }
}
