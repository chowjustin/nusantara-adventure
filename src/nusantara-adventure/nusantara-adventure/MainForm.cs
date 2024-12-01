

namespace nusantara_adventure
{
    public class MainForm : Form
    {
        private Button[] levelButtons;
        private Button exitButton;


        public MainForm()
        {
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            this.Text = "Main Menu";
            this.Size = new Size(400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeControls()
        {
            levelButtons = new Button[5];

            for (int i = 0; i < 5; i++)
            {
                levelButtons[i] = new Button
                {
                    Text = $"Level {i + 1}",
                    Location = new Point(150, 50 + (i * 50)),
                    Size = new Size(100, 40),
                    BackColor = Color.LightGreen
                };

                int levelIndex = i; // Capture the current level in the closure
                levelButtons[i].Click += (sender, e) => StartSelectedLevel(levelIndex);

                this.Controls.Add(levelButtons[i]);
            }

            exitButton = new Button
            {
                Text = "Exit",
                Location = new Point(150, 50 + (5 * 50)),
                Size = new Size(100, 30)
            };
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

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

    }
}