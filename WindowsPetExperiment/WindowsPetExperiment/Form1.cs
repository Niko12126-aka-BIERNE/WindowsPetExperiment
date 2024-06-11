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

        public Form1()
        {
            PetHandle = Handle;
            idleAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_idle.png"), 32 * 3, 300);
            moveAnimation = Animation.FromSpriteSheetAndMetaData(new Bitmap("C:\\Users\\niko1\\OneDrive\\Pictures\\Clownfish_walk.png"), 32 * 3, 100);
            InitializeComponent();
        }

        private void GoToLocation(Point location)
        {
            animationState = "moving";

            direction = GetLocation().X > location.X ? -1 : 1;

            int speed = 5;

            int horizontalDistance = location.X - GetLocation().X;
            int verticalDistance = location.Y - GetLocation().Y;

            int totalDistance = (int)Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));

            double stepCount = (double)totalDistance / speed;

            double horizontalStepDistance = (location.X - GetLocation().X) / stepCount;
            double verticalStepDistance = (location.Y - GetLocation().Y) / stepCount;

            Point startLocation = GetLocation();
            for (int i = 1; i <= stepCount; i++)
            {
                Point newLocation = new((int)(startLocation.X + horizontalStepDistance * i), (int)(startLocation.Y + verticalStepDistance * i));
                SetLocation(newLocation);

                Thread.Sleep(5);
            }

            animationState = "idle";
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
            while (true)
            {
                switch (movementState)
                {
                    case "nowhere":
                        animationState = "idle";
                        break;

                    case "mouse":
                        GoTowardsMouse();
                        break;

                    case "focusedWindow":
                        GoTowardsFocusedWindow();
                        break;
                }

                Thread.Sleep(5);
            }
        }

        private void GoTowardsFocusedWindow()
        {
            LINE? line = GetNewFocusedWindowTopLine();
            if (line is not null)
            {
                GoTowardsLocation(new Point((line.Value.Point1.X + line.Value.Point2.X) / 2, line.Value.Point1.Y));
            }
        }

        private void GoTowardsLocation(Point location)
        {
            if (location.Equals(GetLocation()))
            {
                animationState = "idle";
                return;
            }

            animationState = "moving";

            int speed = 5;

            Point directionVector = new(location.X - GetLocation().X, location.Y - GetLocation().Y);
            double magnitude = Math.Sqrt(Math.Pow(directionVector.X, 2) + Math.Pow(directionVector.Y, 2));

            if (magnitude <= speed)
            {
                SetLocation(location);
                return;
            }

            (double normalizedX, double normalizedY) = (directionVector.X / magnitude, directionVector.Y / magnitude);

            Point newLocation = new(GetLocation().X + (int)(normalizedX * speed), GetLocation().Y + (int)(normalizedY * speed));

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

        private void GoTowardsMouse()
        {
            Point mouseLocation = MouseManager.GetMouseLocation();
            GoTowardsLocation(mouseLocation);
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
}
