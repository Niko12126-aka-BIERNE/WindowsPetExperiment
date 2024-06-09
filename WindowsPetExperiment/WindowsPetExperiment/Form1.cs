namespace WindowsPetExperiment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Bitmap petImage = new("C:\\Users\\niko1\\OneDrive\\Pictures\\CuraLogo.png");
            InitializeComponent();
        }

        private void MoveRight(Form form)
        {
            for (int i = 0; i < 300; i++)
            {
                SetLocation(new Point(form.Location.X + 1, form.Location.Y));
                Thread.Sleep(5);
            }
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            new Thread(() => MoveRight(this)).Start();
        }

        private void SetLocation(Point location)
        {
            if (InvokeRequired)
            {
                Invoke(() => { Location = location; });
            }
            else
            {
                Location = location;
            }
        }
    }
}
