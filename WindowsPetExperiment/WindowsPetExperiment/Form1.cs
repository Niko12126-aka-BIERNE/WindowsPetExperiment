using System.Diagnostics;

namespace WindowsPetExperiment
{
    public partial class Form1 : Form
    {
        private Thread animationThread;
        private readonly Animation idleAnimation;
        private readonly Animation moveAnimation;
        private int direction = -1;
        private string state = "idle";

        public Form1()
        {
            idleAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_idle.png"), 32 * 3, 300);
            moveAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_walk.png"), 32 * 3, 100);
            InitializeComponent();
        }

        private void GoToLocation(Point location)
        {
            state = "moving";

            direction = Location.X > location.X ? -1 : 1;

            int speed = 5;

            int horizontalDistance = location.X - Location.X;
            int verticalDistance = location.Y - Location.Y;

            int totalDistance = (int)Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));

            double stepCount = (double)totalDistance / speed;

            double horizontalStepDistance = (double)(location.X - Location.X) / stepCount;
            double verticalStepDistance = (double)(location.Y - Location.Y) / stepCount;

            Point startLocation = Location;
            for (int i = 1; i <= stepCount; i++)
            {
                Point newLocation = new((int)(startLocation.X + horizontalStepDistance * i), (int)(startLocation.Y + verticalStepDistance * i));
                SetLocation(newLocation);

                Thread.Sleep(5);
            }

            state = "idle";
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            animationThread = new Thread(AnimationController);
            animationThread.Start();

            new Thread(Test).Start();
        }

        private void AnimationController()
        {
            Animation animationRunning = idleAnimation;
            Stopwatch animationTimer = new();

            while (true)
            {
                if (state.Equals("idle") && animationRunning != idleAnimation)
                {
                    animationRunning.Reset();
                    animationRunning = idleAnimation;
                }
                else if (state.Equals("moving") && animationRunning != moveAnimation)
                {
                    animationRunning.Reset();
                    animationRunning = moveAnimation;
                }

                if (animationTimer.ElapsedMilliseconds >= animationRunning.FrameDelayInMilliseconds)
                {
                    SetFrame(animationRunning.NextFrame());
                    animationTimer.Restart();
                }
                else if (!animationTimer.IsRunning)
                {
                    SetFrame(animationRunning.NextFrame());
                    animationTimer.Start();
                }
            }
        }

        private void Test()
        {
            Thread.Sleep(3000);

            GoToLocation(new Point(100, 100));

            Thread.Sleep(3000);

            GoToLocation(new Point(2000, 1000));
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

        private void SetFrame(Bitmap frame)
        {
            if (pictureBox1.InvokeRequired)
            {
                Invoke(() => {
                    Bitmap frameToDispplay = (Bitmap)frame.Clone();

                    if (direction > 0)
                    {
                        frameToDispplay.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }

                    pictureBox1.Image = frameToDispplay; 
                });
            }
            else
            {
                pictureBox1.Image = frame;
            }
        }
    }
}
