using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        static int characterCount = 16;
        Character[] characterList = new Character[characterCount];
        int[] stackIndex = new int[characterCount];
        Character[] sortedCharacters = new Character[characterCount];
        Timer timer;
        string projectPath = System.IO.Path.GetFullPath(@"..\..\");
        bool firstFrame = true;
        public Form1()
        {
            InitializeComponent();
            this.timer = new Timer();
            this.timer.Interval = 500;
            this.timer.Tick += new EventHandler(Timer_Tick);
            this.timer.Start();

            this.Size = new Size(400, 400);
            this.ClientSize = this.Size;
            this.Text = "Kadai";
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(Form1_Paint);

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            Random random = new Random();
            for (int i = 0; i < characterCount; i++)
            {
                int imageName = random.Next(0, 17);
                characterList[i] = new Character(
                    id: i,
                    imagePath: $"{projectPath}/images/{i}.png",
                    mapSize: this.ClientSize
                );
                if (i == 10)
                {
                    characterList[i].ControlMode = Character.ControlModes.PlayerControl;
                }
                else
                {
                    characterList[i].ControlMode = Character.ControlModes.AIControl;
                }
                characterList[i].Position = new Point(
                    x: random.Next(0, (this.ClientSize.Width - characterList[i].RectSize.Width) / characterList[i].MovingStep) * characterList[i].MovingStep,
                    y: random.Next(0, (this.ClientSize.Height - characterList[i].RectSize.Height) / characterList[i].MovingStep) * characterList[i].MovingStep
                );
                characterList[i].Direction = (int)Character.Directions.Bottom;
                stackIndex[i] = i;
            }
            
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
        }

        public void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Bitmap mapBitmap = new Bitmap($"{this.projectPath}/images/map.png");
            // e.Graphics.DrawImage(mapBitmap, 0, 0);

            // Sort characters' indices based on positions and ID
            var sortedByY = characterList.OrderBy(characterList => characterList.Position.Y);
            var sortedByX = sortedByY.ThenBy(characterList => characterList.Position.X);
            var sorted = sortedByX.ToArray();
            for (int i = 0; i < sorted.Length; i++)
            {
                sortedCharacters[i] = sorted[i];
            }
            Console.WriteLine(stackIndex);
            for (int i = 0; i < sortedCharacters.Length; i++)
            {
                Character currentCharacter = sortedCharacters[i];
                e.Graphics.DrawImage(
                    image: currentCharacter.BitmapObject,
                    point: currentCharacter.Position
                );

            }

            this.firstFrame = false;
        }

        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < this.characterList.Length; i++)
            {
                Character currentCharacter = this.characterList[i];
                if (currentCharacter.ControlMode == Character.ControlModes.PlayerControl)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Up:
                            currentCharacter.Direction = Character.Directions.Top;
                            break;
                        case Keys.Down:
                            currentCharacter.Direction = Character.Directions.Bottom;
                            break;
                        case Keys.Left:
                            currentCharacter.Direction = Character.Directions.Left;
                            break;
                        case Keys.Right:
                            currentCharacter.Direction = Character.Directions.Right;
                            break;
                    }
                    currentCharacter.Move();
                }
            }
            Refresh();
        }

        private void swapStackIndex(int firstIndex, int secondIndex)
        {
            var _tempIndex = stackIndex[firstIndex];
            stackIndex[firstIndex] = stackIndex[secondIndex];
            stackIndex[secondIndex] = _tempIndex;
        }
    }
}
