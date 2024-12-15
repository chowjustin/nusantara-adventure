using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace nusantara_adventure
{
    public class MainForm : Form
    {
        private Button[] _levelButtons;
        private Button _exitButton;
        private Image _bgImage;
        private const int TOTAL_LEVEL = 5;

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
            
            InitializeBackground();
            this.Paint += MainForm_Paint;
        }

        private void InitializeBackground()
        {
            using (MemoryStream ms = new MemoryStream(Resource.bgform))
            {
                _bgImage = Image.FromStream(ms);
            }

            this.DoubleBuffered = true;
        }

        private void InitializeControls()
        {
            _levelButtons = new Button[TOTAL_LEVEL];

            int xPos = (this.ClientSize.Width - 100) / 2;

            for (int i = 0; i < TOTAL_LEVEL; i++)
            {
                _levelButtons[i] = new Button
                {
                    Text = $"Level {i + 1}",
                    Location = new Point(xPos, 120 + (i * 60)),
                    Size = new Size(100, 40),
                    BackColor = Color.Red,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                };

                _levelButtons[i].FlatAppearance.BorderColor = Color.Black;
                _levelButtons[i].FlatAppearance.BorderSize = 3;

                int levelIndex = i;
                _levelButtons[i].Click += (sender, e) => StartSelectedLevel(levelIndex);

                this.Controls.Add(_levelButtons[i]);
            }

            _exitButton = new Button
            {
                Text = "Exit",
                Location = new Point(xPos, 120 + (TOTAL_LEVEL * 60)),
                Size = new Size(100, 40),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };

            _exitButton.FlatAppearance.BorderColor = Color.White;
            _exitButton.FlatAppearance.BorderSize = 3;
            _exitButton.Click += ExitButton_Click;

            this.Controls.Add(_exitButton);
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
            e.Graphics.DrawImage(_bgImage, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

            string text = "Choose The Level To Play";
            Font font = new Font("Poppins Semibold", 20);

            SizeF textSize = e.Graphics.MeasureString(text, font);
            float xPos = (this.ClientSize.Width - textSize.Width) / 2;

            e.Graphics.DrawString(text, font, Brushes.Gold, xPos, 50);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}