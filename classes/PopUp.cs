namespace quiz.classes;

public class PopUp : Form
{
	private Label label;
	public PopUp(bool IsCorrect)
	{
		if(IsCorrect)
		{
			this.BackColor = Color.Green;
			label = new Label{
				Dock = DockStyle.Fill,
				Text = "Correct!",
				Font = new Font("Comic Sans MS", 18),
				TextAlign = ContentAlignment.MiddleCenter
			};
			Controls.Add(label);
		}
		else 
		{
			this.BackColor = Color.Red;
			label = new Label{
				Dock = DockStyle.Fill,
				Text = "Wrong"!,
				Font = new Font("Comic Sans MS", 18),
				TextAlign = ContentAlignment.MiddleCenter
			};
			Controls.Add(label);
		}
	}
	
	private void InitializeComponent()
	{
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(400, 400);
		this.FormBorderStyle = FormBorderStyle.FixedSingle;
	}
}
