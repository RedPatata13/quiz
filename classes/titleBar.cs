namespace quiz.classes;
using System.Drawing;
public class TitleBar : Panel
{
	public ControlButtons exit;
	public ControlButtons mini;
	public TitleBar(int x, int y)
	{
		Size = new Size(800, 50);
		Location = new Point(x, y);
		BackColor = ColorTranslator.FromHtml("#596375");
		
		mini = new ControlButtons(720, 0);
		mini.Text = "-";
		Controls.Add(mini);
		
		exit = new ControlButtons(760, 0);
		exit.Text = "X";
		exit.Click += ExitClick;
		Controls.Add(exit);
	}
	
	public void ExitClick(object sender, EventArgs e)
	{
		MessageBox.Show("Igot, Red Evan I.\nBSCS-1A\nFundamentals of Programming");
		Application.Exit();
		
	}
}

public class ControlButtons : Button
{
	public ControlButtons(int x, int y)
	{
		Size = new Size(40, 40);
		Location = new Point(x, y);
		FlatStyle = FlatStyle.Flat;
		FlatAppearance.BorderColor = ColorTranslator.FromHtml("#596375");
		FlatAppearance.BorderSize = 1;
		Font = new Font("Monserrat", 12);
	}
}
