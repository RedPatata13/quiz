namespace quiz.classes;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class QuestionWindow : Form
{
	private TitleBar titleBar;
	private Panel questionTextPanel;
	private TextBox questionDisplay;
	private List<QuestionButton> list;
	public EventHandler QuestionButtonClick;
	public EventHandler ExitQwClick;
	public EventHandler MiniClick;
	private readonly Question question;
	
	public QuestionWindow(Question question)
	{
		this.question = question;
		
		InitializeComponent();
		InitializeQuestionText();
		InitializeTitleBar();
		InitializeQuestionButtons();
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
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 600);
		this.FormBorderStyle = FormBorderStyle.FixedSingle;

        this.Text = string.Empty;
		this.ControlBox = false;
		this.BackColor = ColorTranslator.FromHtml("#596375");
	}
	private void InitializeQuestionText()
	{
		questionTextPanel = new Panel
		{
			Size = new Size(800, 200),
			Location = new Point(0, 150),
			ForeColor = Color.Black
		};
		
		questionDisplay = new TextBox
		{
			Multiline = true,
			Location = new Point(0, 0),
			Size = new Size(800, 200),
			BorderStyle = BorderStyle.None,
			BackColor = ColorTranslator.FromHtml("#596375"),
			Font = new Font("Comic Sans MS", 20),
			Text = question.GetQuestionText(),
			ForeColor = Color.Black,
			TextAlign = HorizontalAlignment.Center,
			ReadOnly = true,
			TabStop = false
		};

		
		questionTextPanel.Controls.Add(questionDisplay);
		Controls.Add(questionTextPanel);
	}
	private void InitializeQuestionButtons()
	{
		list = new List<QuestionButton>();
		List<Choice> q = question.GetSequence();
		for(int i = 0; i < q.Count; i++)
		{
			int x = i* 200;
			int y = 400;
			QuestionButton but = new QuestionButton(x, y);
			but.Text = q[i].GetText();
			but.SetIsBool(q[i].GetStatus());
			but.Click += (sender, e) => QuestionButtonClick?.Invoke(sender, e);
			but.Click += ShowCorrect;
			Controls.Add(but);
		}
	}
	private void ShowCorrect(object sender, EventArgs e)
	{
		if(sender is QuestionButton qb)
		{
			PopUp popup = new PopUp(qb.GetIsBool());
			popup.Show();
		}
	}
	public void HideQuiz()
	{
		this.Hide();
	}
	private void InitializeTitleBar()
	{
		titleBar = new TitleBar(0,0);
		titleBar.MouseDown += new MouseEventHandler(customDrag);
		titleBar.mini.Click += (sender, e) => MiniClick?.Invoke(sender, e);
		titleBar.exit.Click -= titleBar.ExitClick;
		titleBar.exit.Click += (sender, e) => ExitQwClick?.Invoke(sender, e);
		Controls.Add(titleBar);
	}
}
public class QuestionButton : Button
{
	private bool IsBool;
	
	public QuestionButton(int x, int y)
	{
		Size = new Size(200, 150);
		Location = new Point(x, y);
		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderColor = Color.Black;
		BackColor = Color.Gray;
		FlatAppearance.BorderSize = 1;
	}
	
	public void SetIsBool(bool isb)
	{
		this.IsBool = isb;
	}
	public bool GetIsBool()
	{
		return this.IsBool;
	}
}
