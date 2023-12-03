namespace quiz;
using System.Runtime.InteropServices;
using System.Drawing;
using quiz.classes;

public partial class View : Form
{
	private TitleBar titleBar;
	private Label welcomeText;
	private DifficultySelectionButton easy;
	private DifficultySelectionButton medium;
	private DifficultySelectionButton hard;
	private List<QuestionWindow> QuestionTracker;
	private int currentQuestion;
	private int currentScore;
	public EventHandler DiffSelect;
    public View()
    {
        InitializeComponent();
		InitializeTitleBar();
		InitializeWelcomeText();
		InitializeDifficultySelectionButtons();
    }
	[DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
	private extern static void ReleaseCapture();
	
	[DllImport("user32.DLL", EntryPoint = "SendMessage")]
	private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
	
	private void customDrag(object sender, MouseEventArgs e){
		ReleaseCapture();
		SendMessage(this.Handle, 0x112, 0xf012, 0);
	}
	private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 600);
		this.FormBorderStyle = FormBorderStyle.FixedSingle;

        this.Text = string.Empty;
		this.ControlBox = false;
		this.BackColor = ColorTranslator.FromHtml("#596375");
		
		this.StartPosition = FormStartPosition.Manual;
		Screen primaryScreen = Screen.PrimaryScreen;
		
		int screenWidth = primaryScreen.Bounds.Width;
		int screenHeight = primaryScreen.Bounds.Height;
		int formsWidth = this.Width;
		int formsHeight = this.Height;
		
		int x = screenWidth / 2 - formsWidth / 2;
		int y = screenHeight / 2 - formsHeight / 2;
		this.Location = new Point(x , y);
		
		this.FormClosing += (sender , e) => {
			this.Dispose();
		};
    }
	private void InitializeTitleBar()
	{
		titleBar = new TitleBar(0,0);
		titleBar.mini.Click += (sender, e) => {
			this.WindowState = FormWindowState.Minimized;
		};
		titleBar.MouseDown += new MouseEventHandler(customDrag);
		Controls.Add(titleBar);
	}
	private void InitializeDifficultySelectionButtons()
	{
		int x = this.Width / 2 - 50;
		easy = new DifficultySelectionButton(x, 320);
		medium = new DifficultySelectionButton(x, 360);
		hard = new DifficultySelectionButton(x, 400);
		
		easy.BackColor = Color.Green;
		easy.Text = "Easy";
		easy.Click += (sender, e) => DiffSelect?.Invoke(sender, e);
		
		medium.BackColor = Color.Yellow;
		medium.Text = "Medium";
		medium.Click += (sender, e) => DiffSelect?.Invoke(sender ,e);
		
		hard.BackColor = Color.Red;
		hard.Text = "Hard";
		hard.Click += (sender, e) => DiffSelect?.Invoke(sender, e);
		
		Controls.Add(easy);
		Controls.Add(medium);
		Controls.Add(hard);
	}
	private void InitializeWelcomeText()
	{
		welcomeText = new Label{
			Size = new Size(800, 150),
			Font = new Font("Comic Sans MS", 28),
			Location = new Point(0, 150),
			Text = "Welcome To Quiz!",
			TextAlign = ContentAlignment.MiddleCenter
		};
		
		Controls.Add(welcomeText);
	}
	public void StartQuiz(List<QuestionWindow> qwa)
	{
		this.Hide();
		currentQuestion = 0;
		currentScore = 0;
		QuestionTracker = qwa;
		QuestionTracker[currentQuestion].Show();
		
		for(int i = 0; i < QuestionTracker.Count; i++){
			QuestionWindow qw = QuestionTracker[i];
			qw.QuestionButtonClick += QuestionButtonClick;
			qw.QuestionButtonClick += IncreaseScore;
			qw.ExitQwClick += ExitQw;
			qw.MiniClick += MiniQw;
		}
		QuestionTracker[QuestionTracker.Count - 1].QuestionButtonClick -= QuestionButtonClick;//stop moving to another question past 5
		QuestionTracker[QuestionTracker.Count - 1].QuestionButtonClick += RedirectToQuiz;
		void RedirectToQuiz(object sender, EventArgs e)
		{
			QuestionWindow qwLast = QuestionTracker[QuestionTracker.Count - 1];
			qwLast.QuestionButtonClick -= IncreaseScore;
			qwLast.MiniClick -= MiniQw;
			qwLast.ExitQwClick -= ExitQw;
			qwLast.Dispose();
			PopUp popup = new PopUp(true);
			popup.StartPosition = FormStartPosition.Manual;
			
			int x = this.Location.X + this.Width + 5;
			int y = this.Location.Y + (this.Height / 2);
			popup.Location = new Point(x , y);
			popup.SetScore(currentScore);
			popup.Show();
			this.Show();
		}
		void IncreaseScore(object sender, EventArgs e)
		{
			if(sender is QuestionButton qb)
			{
				if(qb.GetIsBool()) this.currentScore++;
			}
		}
		void MiniQw(object sender, EventArgs e)
		{
			QuestionWindow qw = QuestionTracker[currentQuestion];
			qw.WindowState = FormWindowState.Minimized;
		}
		void ExitQw(object sender, EventArgs e)
		{
			QuestionWindow qw = QuestionTracker[currentQuestion];
			this.Show();
			qw.ExitQwClick -= ExitQw;
			qw.MiniClick -= MiniQw;
			qw.Dispose();
		};
		void QuestionButtonClick(object sender, EventArgs e) {
			if(currentQuestion < QuestionTracker.Count){
				QuestionWindow qw = QuestionTracker[currentQuestion++];
				qw.MiniClick -= MiniQw;
				qw.ExitQwClick -= ExitQw;
				qw.QuestionButtonClick -= QuestionButtonClick;
				qw.QuestionButtonClick -= IncreaseScore;
				qw.Dispose();
				QuestionTracker[currentQuestion].Show();
			}
			
		}
	}
}

public class DifficultySelectionButton : Button
{
	public DifficultySelectionButton(int x, int y)
	{
		Size = new Size(100, 40);
		Location = new Point(x, y);
		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderColor = Color.Black;
		FlatAppearance.BorderSize = 1;
	}
}
public class Controller
{
	private View view;
	private Model model;
	
	public Controller(View view, Model model)
	{
		this.view = view;
		this.model = model;
		
		view.DiffSelect += SendDifficulty;
	}
	
	private void SendDifficulty(object sender, EventArgs e)
	{
		if(sender is Button button)
		{
			string text = button.Text.ToLower();
			List<QuestionWindow> qwa = model.makeQuizWindow(text);	
			view.StartQuiz(qwa);
		}
		
	}
}
public class Model
{
	private readonly string[] easyQuestions = new string[]
	{
		"What is 1 + 1?",
		"Who created the Mona Lisa",
		"Coffee or Milk Tea. NOTE: There is only ONE answer...?",
		"Who developed the Pythagorean Theorem",
		"When is Tuesday"
	};
	
	private readonly string[] mediumQuestions = new string[]
	{
		"Synonym of Mitochondria",
		"If a variable is a house, then this is the equivalent of storing it's address in it",
		"What data type is the equivalent of True/False?",
		"Which is an Olympian God?",
		"What is 'Lorem Ipsum' usually used for?"
	};
	
	private readonly string[] hardQuestions = new string[]
	{
		"Which among the listed algorithms require a stack, a list/array/queue and returns a list/array?",
		"Which among the following is a mapping algorithm?",
		"What number system is used the most in the field of Computer Science?",
		"What is the max length of a double variable?",
		"What character MUST ABSOLUTELY be added at the end of every string in C?",
	};
	
	private readonly string[,] easyWrongChoices = new string[,]
	{
		{"5", "4", "3"},
		{"Galileo Galilei", "Teenage Mutant Ninja Turtle", "Christopher Nolan"},
		{"Milk Tea", "Milk Tea", "Milk Tea"},
		{"Leonardo Da Vinci", "Vishnu", "Susano'o"},
		{"Probably Monday ngl", "100% Sunday", "On Black Friday."}
	};
	
	private readonly string[,] mediumWrongChoices = new string[,]
	{
		{"Mitospera", "Ribosome", "Nucleus"},
		{"Location", "Address", "Memory Location"},
		{"Float", "Int", "u_int64"},
		{"Asura", "Anubis", "Goetia"},
		{"Prayer", "Poetry", "Mantra"}
	};
	
	private readonly string[,] hardWrongChoices = new string[,]
	{
		{"Graph", "Binary Search", "Linear Search"},
		{"Linear Search", "Binary Search", "Doubly-Linked List"},
		{"base32", "base8", "base16"},
		{"13", "14", "16"},
		{"\\n", "\\t", "\\m"}
	};
	
	private readonly string[] easyCorrectChoices = new string[]
	{
		"2",
		"Leonardo Da Vinci",
		"Coffee",
		"Pythagoras",
		"On Tuesday"
	};
	private readonly string[] mediumCorrectChoices = new string[]
	{
		"POWERHOUSE OF THE CELL",
		"Pointer",
		"Boolean",
		"Ares",
		"Filler Text"
	};
	private readonly string[] hardCorrectChoices = new string[]
	{
		"Shunting Yard Algorithm",
		"A* Algorithm",
		"base2",
		"15",
		"\\0"
	};
	public List<QuestionWindow> makeQuizWindow(string difficulty)
	{
		List<QuestionWindow> questionWindowList = new List<QuestionWindow>();
		List<Question> questionList = makeQuestionList(difficulty);
		
		for(int i = 0; i < questionList.Count; i++)
		{
			QuestionWindow qw = new QuestionWindow(questionList[i]);
			questionWindowList.Add(qw);
		}
		
		return questionWindowList;
	}
	private List<Question> makeQuestionList(string difficulty)
	{
		List<Question> qArray = new List<Question>();
		switch(difficulty)
		{
			case "easy":
				for(int i = 0; i < easyQuestions.Length; i++)
				{
					string wc1 = easyWrongChoices[i, 0];
					string wc2 = easyWrongChoices[i, 1];
					string wc3 = easyWrongChoices[i, 2];
					string cc = easyCorrectChoices[i];
					Question q = new Question(easyQuestions[i], wc1, wc2, wc3, cc);
					
					qArray.Add(q);
				}
				break;
			case "medium":
				for(int i = 0; i < mediumQuestions.Length; i++)
				{
					string wc1 = mediumWrongChoices[i, 0];
					string wc2 = mediumWrongChoices[i, 1];
					string wc3 = mediumWrongChoices[i, 2];
					string cc = mediumCorrectChoices[i];
					Question q = new Question(mediumQuestions[i], wc1, wc2, wc3, cc);
					
					qArray.Add(q);
				}
				break;
			case "hard":
				for(int i = 0; i < hardQuestions.Length; i++)
				{
					string wc1 = hardWrongChoices[i, 0];
					string wc2 = hardWrongChoices[i, 1];
					string wc3 = hardWrongChoices[i, 2];
					string cc = hardCorrectChoices[i];
					Question q = new Question(hardQuestions[i], wc1, wc2, wc3, cc);
					
					qArray.Add(q);
				}
				break;
		}
		
		return qArray;
	}
	
}


