using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace AnalogClock
{

    public class MyCircleForm : Form
    {
        public DateTime CurrentTime { get; set; } = DateTime.Now;
        public MyCircleForm()
        {
       
            this.DoubleBuffered = true; // Prevents flickering
            this.TopMost = true;
            this.Size = new Size(800, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.FormBorderStyle = FormBorderStyle.None;

            // Updates the time and redraws every 100ms
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) => {
                this.CurrentTime = DateTime.Now;
                this.Invalidate();
            };
            timer.Start();

            // to close
            this.KeyPreview = true;
            this.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Escape) Application.Exit();
            };
        }


        private void DrawHand(Graphics g, double angle, float lengthFactor, Pen pen, int cx, int cy, int radius)
        {
            double rads = (angle - 90) * (Math.PI / 180);
            float xEnd = (float)(cx + radius * lengthFactor * Math.Cos(rads));
            float yEnd = (float)(cy + radius * lengthFactor * Math.Sin(rads));
            g.DrawLine(pen, cx, cy, xEnd, yEnd);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Pen clockPen = new Pen(Color.Black, 7);
            Pen hourPen = new Pen(Color.Black, 5);
            Pen minutePen = new Pen(Color.Black, 3);
            Pen secondPen = new Pen(Color.Red, 1);
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;
            int radius = 300;

            g.DrawEllipse(clockPen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            g.FillEllipse(new SolidBrush(Color.Black), new Rectangle(centerX - radius / 50, centerY - radius / 50, radius / 25, radius / 25));

            // draw numbers
            Font numberFont = new Font("Lucida Console", 24, FontStyle.Bold);
            for (int i = 1; i <= 12; i++)
            {
                double angle = i * 30;
                double rads = (angle - 90) * (Math.PI / 180);

                float x = (float)(centerX + (radius * 1.1) * Math.Cos(rads));
                float y = (float)(centerY + (radius * 1.1) * Math.Sin(rads));

                string text = i.ToString();
                SizeF textSize = g.MeasureString(text, numberFont);
                g.DrawString(text, numberFont, Brushes.Black, x - (textSize.Width / 2), y - (textSize.Height / 2));
            }

            // draw marks
            for (int i = 1; i <= 60; i++)
            {
                double angle = i * 6;
                double rads = (angle - 90) * (Math.PI / 180);

                float xOuter = (float)(centerX + radius * Math.Cos(rads));
                float yOuter = (float)(centerY + radius * Math.Sin(rads));

                Pen markPen;
                int markSize;

                if (i % 15 == 0)
                {
                    markPen = new Pen(Color.Black, 4);
                    markSize = 20;
                }
                else if (i % 5 == 0)
                {
                    markPen = new Pen(Color.Black, 3);
                    markSize = 15;
                }
                else
                {
                    markPen = new Pen(Color.Gray, 1);
                    markSize = 10;
                }

                float xInner = (float)(centerX + (radius - markSize) * Math.Cos(rads));
                float yInner = (float)(centerY + (radius - markSize) * Math.Sin(rads));

                g.DrawLine(markPen, xOuter, yOuter, xInner, yInner);
            }

            // draw hands
            double sAngle = (CurrentTime.Second * 6);
            double mAngle = (CurrentTime.Minute * 6);
            double hAngle = ((CurrentTime.Hour % 12) * 30) + (CurrentTime.Minute * 0.5);

            DrawHand(g, sAngle, 0.9f, secondPen, centerX, centerY, radius);
            DrawHand(g, mAngle, 0.8f, minutePen, centerX, centerY, radius);
            DrawHand(g, hAngle, 0.65f, hourPen, centerX, centerY, radius);
        }
    }

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyCircleForm());
        }
    }
}

