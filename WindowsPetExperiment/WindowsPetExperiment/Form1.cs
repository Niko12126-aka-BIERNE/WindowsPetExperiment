using System.Diagnostics;
using static WindowsPetExperiment.WindowManager;

namespace WindowsPetExperiment
{
    public partial class Form1 : Form
    {
        public static IntPtr PetHandle { get; private set; }
        private readonly Animation idleAnimation;
        private readonly Animation moveAnimation;
        private int direction = -1;
        private string animationState = "idle";
        private string movementState = "nowhere";
        public static IntPtr HomeHandle { get; private set; }

        public Form1(IntPtr homeHandle)
        {
            HomeHandle = homeHandle;
            PetHandle = Handle;
            idleAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_idle.png"), 32 * 3, 300);
            moveAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_walk.png"), 32 * 3, 100);
            InitializeComponent();
        }

        private void Form1_Shown_1(object sender, EventArgs e)
        {
            new Thread(AnimationController).Start();
            new Thread(MovementController).Start();

            new Thread(Test).Start();
        }

        private void AnimationController()
        {
            Animation animationRunning = idleAnimation;
            Stopwatch animationTimer = new();

            while (true)
            {
                if (animationState.Equals("idle") && animationRunning != idleAnimation)
                {
                    animationRunning.Reset();
                    animationRunning = idleAnimation;
                }
                else if (animationState.Equals("moving") && animationRunning != moveAnimation)
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

        private void MovementController()
        {
            int speedInPixelsPerSec = 1000;

            while (true)
            {
                Thread.Sleep(5);

                if (Home.StayAtHome)
                {
                    GoTowardsHome(speedInPixelsPerSec);
                    continue;
                }

                switch (movementState)
                {
                    case "nowhere":
                        animationState = "idle";
                        break;

                    case "mouse":
                        GoTowardsMouse(speedInPixelsPerSec);
                        break;

                    case "focusedWindow":
                        GoTowardsFocusedWindow(speedInPixelsPerSec);
                        break;
                }
            }
        }

        private void GoTowardsHome(int pixelsPerSec)
        {
            LINE line = GetTopLine(HomeHandle);
            GoTowardsLocation(new Point((line.Point1.X + line.Point2.X) / 2, line.Point1.Y), pixelsPerSec); //Offset this so it sits on top of the home sprite... not the window
        }

        private void GoTowardsFocusedWindow(int pixelsPerSec)
        {
            LINE? line = GetNewFocusedWindowTopLine();
            if (line is not null)
            {
                GoTowardsLocation(new Point((line.Value.Point1.X + line.Value.Point2.X) / 2, line.Value.Point1.Y), pixelsPerSec);
            }
        }

        private void GoTowardsLocation(Point location, int pixelsPerSec)
        {
            if (location.Equals(GetLocation()))
            {
                animationState = "idle";
                return;
            }

            animationState = "moving";
            direction = GetLocation().X > location.X ? -1 : 1;

            int speed = pixelsPerSec / 200;

            Point directionVector = new(location.X - GetLocation().X, location.Y - GetLocation().Y);
            double magnitude = Math.Sqrt(Math.Pow(directionVector.X, 2) + Math.Pow(directionVector.Y, 2));

            if (magnitude <= speed)
            {
                SetLocation(location);
                return;
            }

            (double normalizedX, double normalizedY) = (directionVector.X / magnitude, directionVector.Y / magnitude);

            Point newLocation = new(GetLocation().X + (normalizedX * speed).Round(), GetLocation().Y + (normalizedY * speed).Round());

            SetLocation(newLocation);
        }

        private void Test()
        {
            Thread.Sleep(3000);

            movementState = "mouse";
            Thread.Sleep(10000);

            movementState = "focusedWindow";
            Thread.Sleep(10000);

            movementState = "nowhere";
        }

        private void GoTowardsMouse(int pixelsPerSec)
        {
            Point mouseLocation = MouseManager.GetMouseLocation();
            GoTowardsLocation(mouseLocation, pixelsPerSec);
        }

        private void SetLocation(Point location)
        {
            if (InvokeRequired)
            {
                Invoke(() => { Location = new Point(location.X - 32 * 3 / 2, location.Y - 32 * 3); });
            }
            else
            {
                Location = new Point(location.X - 32 * 3 / 2, location.Y - 32 * 3);
            }
        }

        private Point GetLocation()
        {
            return new Point(Location.X + 32 * 3 / 2, Location.Y + 32 * 3);
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

    public static class RoundableDouble
    {
        public static int Round(this double number)
        {
            return number - Math.Round(number) == 0.5 ? (int)Math.Ceiling(number) : (int)Math.Round(number);
        }
    }
}
