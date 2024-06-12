using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsPetExperiment
{
    public partial class Home : Form
    {
        public Point CurrentLocation { get; private set; }
        private bool mouseDown;
        private Point lastLocation;
        public static bool StayAtHome { get; private set; }

        public Home()
        {
            StayAtHome = false;

            InitializeComponent();
            CurrentLocation = new Point(Location.X + pictureBox1.Image.Width / 2, Location.Y + pictureBox1.Image.Height);

            IntPtr handle = Handle;
            new Thread(() => Application.Run(new Form1(handle))).Start();
        }

        private void HomeMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                lastLocation = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (!Size.Equals(pictureBox1.Image.Size))
                {
                    Size = pictureBox1.Image.Size;
                }

                StayAtHome = !StayAtHome;
            }
        }

        private void HomeMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
                CurrentLocation = new Point(Location.X + pictureBox1.Image.Width / 2, Location.Y + pictureBox1.Image.Height);
            }
        }

        private void HomeMouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
    }
}
