

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace nusantara_adventure
{
    public class MainForm : Form
    {
        private Button[] levelButtons;
        private Button exitButton;
        private Image _bgImage;

        public MainForm()
        {
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            this.Text = "Main Menu";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            using (MemoryStream ms = new MemoryStream(Resource.bgform))
            {
                _bgImage = Image.FromStream(ms);
            }

            this.DoubleBuffered = true;

            this.Paint += MainForm_Paint;
        }

        private void InitializeControls()
        {
            levelButtons = new Button[5];

            int xPos = (this.ClientSize.Width - 100) / 2;

            for (int i = 0; i < 5; i++)
            {
                levelButtons[i] = new Button
                {
                    Text = $"Level {i + 1}",
                    Location = new Point(xPos, 120 + (i * 60)),
                    Size = new Size(100, 40),
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                };

                levelButtons[i].FlatAppearance.BorderColor = Color.Black;
                levelButtons[i].FlatAppearance.BorderSize = 3;


                int levelIndex = i; // Capture the current level in the closure
                levelButtons[i].Click += (sender, e) => StartSelectedLevel(levelIndex);

                this.Controls.Add(levelButtons[i]);
            }

            exitButton = new Button
            {
                Text = "Exit",
                Location = new Point(xPos, 120 + (5 * 60)),
                Size = new Size(100, 40),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };

            exitButton.FlatAppearance.BorderColor = Color.White;
            exitButton.FlatAppearance.BorderSize = 3;

            exitButton.Click += ExitButton_Click;

            this.Controls.Add(exitButton);
        }

        private void StartSelectedLevel(int levelIndex)
        {
            GameForm gameForm = new GameForm(levelIndex);
            gameForm.FormClosed += (s, args) => this.Show();
            gameForm.Show();
            this.Hide();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            // Draw the background image
          
            e.Graphics.DrawImage(_bgImage, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

            string text = "Choose The Level To Play";
            Font font = new Font("Poppins Semibold", 20);

            // Measure the string width
            SizeF textSize = e.Graphics.MeasureString(text, font);

            // Calculate the X position to center the string
            float xPos = (this.ClientSize.Width - textSize.Width) / 2;

            // Draw the string at the calculated position (top-center)
            e.Graphics.DrawString(text, font, Brushes.Gold, xPos, 50);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
        }

    }
}