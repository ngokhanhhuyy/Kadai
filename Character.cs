using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApp1
{
    internal class Character
    {
        public enum Directions
        {
            Top = 3,
            Bottom = 0,
            Left = 1,
            Right = 2
        }

        public enum AnimationFrames
        {
            FirstFrame = 0,
            SecondFrame = 1,
            ThirdFrame = 2
        }

        public enum ControlModes
        {
            PlayerControl,
            AIControl
        }

        // Initializing properties
        private int id;
        private ControlModes controlMode;       // To allow controlling by player
        private string imagePath;
        private Bitmap sourceBitmap;
        private int frameIndex;                // To handle which frame to be shown when moving
        private Directions direction;
        private Size rectSize;
        private Point position;
        private Rectangle movableRect;
        private int movingStep = 5;      // To handle how far character can move each time key event is called
        private Timer animationTimer;           // To handle the continuation of the animation
        private Timer frameTimer;               // To handle the change of the frame

        static int directionCount = Enum.GetNames(typeof(Directions)).Length;
        static int animationFrameCount = Enum.GetNames(typeof(AnimationFrames)).Length;

        public Character(int id, string imagePath, Size mapSize, ControlModes controlMode = ControlModes.AIControl)
        {
            // Initial properties
            this.id = id;
            this.imagePath = imagePath;
            this.controlMode = controlMode;
            this.sourceBitmap = new Bitmap(this.imagePath);
            this.frameIndex = (int)AnimationFrames.FirstFrame;
            this.direction = 0;
            // Calculate frame size (based on animation frame count and direction count)
            this.rectSize = new Size(
                width: sourceBitmap.Width / animationFrameCount,
                height: sourceBitmap.Height / directionCount
            );
            this.movableRect = new Rectangle(
                x: 0, y: 0,
                width: mapSize.Width - this.rectSize.Width,
                height: mapSize.Height - this.rectSize.Height
            );
            this.animationTimer = new Timer();
            this.animationTimer.Interval = 50;
            this.animationTimer.Tick += new EventHandler(AnimationTimer_Tick);

            this.frameTimer = new Timer();
            this.frameTimer.Interval = 350;
            this.frameTimer.Tick += new EventHandler(FrameTimer_Tick);
        }

        public int ID
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value >= 0)
                {
                    this.id = value;
                }
            }
        }

        public ControlModes ControlMode
        {
            get
            {
                return this.controlMode;
            }
            set
            {
                this.controlMode = value;
            }
        }

        public Point Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public Directions Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }

        public Size RectSize
        {
            get
            {
                return this.rectSize;
            }
            set
            {
                this.rectSize = value;
            }
        }

        public Bitmap BitmapObject
        {
            get
            {
                Bitmap sourceBitmap = new Bitmap(this.imagePath);
                Rectangle finalRect = new Rectangle(
                    x: this.frameIndex * this.rectSize.Width,
                    y: (int)this.direction * this.rectSize.Height,
                    width: this.rectSize.Width,
                    height: this.rectSize.Height);
                System.Drawing.Imaging.PixelFormat format = sourceBitmap.PixelFormat;
                Bitmap finalBitmap = sourceBitmap.Clone(finalRect, format);
                return finalBitmap;
            }
        }

        public int MovingStep
        {
            get
            {
                return this.movingStep;
            }
            set
            {
                this.movingStep = value;
            }
        }

        public void Move()
        {
            // Move character based on the current direction
            if (this.IsMovable())
            {
                switch (this.direction)
                {
                    case Directions.Top:
                        this.position.Y -= movingStep;
                        break;
                    case Directions.Bottom:
                        this.position.Y += movingStep;
                        break;
                    case Directions.Left:
                        this.position.X -= movingStep;
                        break;
                    case Directions.Right:
                        this.position.X += movingStep;
                        break;
                }
            }

            // Reset animation continuation timer
            if (this.animationTimer.Enabled)
            {
                this.animationTimer.Stop();
            }
            this.animationTimer.Start();
            // Start frame timer only when it is disabled
            if (!this.frameTimer.Enabled)
            {
                this.frameTimer.Start();
            }
        }

        private bool IsMovable()
        {
            bool exceedTop = this.position.Y - movingStep < 0;
            bool exceedBottom = this.position.Y + movingStep > this.movableRect.Height;
            bool exceedLeft = this.position.X - movingStep < 0;
            bool exceedRight = this.position.X + movingStep > this.movableRect.Width;
            var exceed = false;
            switch (this.direction)
            {
                case Directions.Top:
                    exceed = exceedTop;
                    break;
                case Directions.Bottom:
                    exceed = exceedBottom;
                    break;
                case Directions.Left:
                    exceed = exceedLeft;
                    break;
                case Directions.Right:
                    exceed = exceedRight;
                    break;
            }
            return !exceed;
        }


        public void ToNextAnimation()
        {
            if (this.frameIndex + 1 < animationFrameCount)
            {
                this.frameIndex += 1;
            }
            else
            {
                this.frameIndex = 0;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            this.frameTimer.Stop();
            this.frameIndex = (int)AnimationFrames.FirstFrame;
        }

        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            if (this.frameIndex + 1 < animationFrameCount)
            {
                this.frameIndex += 1;
            }
            else
            {
                this.frameIndex = 0;
            }
            this.frameTimer.Start();
        }
    }
}
