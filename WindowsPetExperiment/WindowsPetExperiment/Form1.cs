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

        private void MoveRight()
        {
            state = "moving";
            direction = 1;

            for (int i = 0; i < 300; i++)
            {
                SetLocation(new Point(Location.X + 1, Location.Y));
                Thread.Sleep(5);
            }

            state = "idle";
        }

        private void RunAnimation(Animation animation)
        {
            while (true)
            {
                try
                {
                    SetFrame(animation.NextFrame());
                    Thread.Sleep(100);
                } 
                catch (Exception)
                {
                    return;
                }
            }
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

                SetFrame(animationRunning.NextFrame());
                Thread.Sleep(animationRunning.FrameDelayInMilliseconds);
            }
        }

        private void Test()
        {
            Thread.Sleep(3000);

            MoveRight();

            Thread.Sleep(3000);

            state = "idle";
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
