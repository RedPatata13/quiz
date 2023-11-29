namespace quiz.classes;
using System.Drawing;
using System.Collections.Generic;
public class Question
{
	private List<Choice> ChoiceSequence = new List<Choice>();
	private readonly string question;
	private WrongChoice wc1;
	private WrongChoice wc2;
	private WrongChoice wc3;
	private CorrectChoice cc;
	private Random rand = new Random();
	
	public Question(string question, string wc1, string wc2, string wc3, string cc)
	{
		this.question = question;
		this.wc1 = new WrongChoice();
		this.wc2 = new WrongChoice();
		this.wc3 = new WrongChoice();
		this.cc = new CorrectChoice();
		
		SetChoices(this.wc1, wc1);
		SetChoices(this.wc2, wc2);
		SetChoices(this.wc3, wc3);
		SetChoices(this.cc, cc);
		
		AddToChoiceSequence(this.wc1);
		AddToChoiceSequence(this.wc2);
		AddToChoiceSequence(this.wc3);
		AddToChoiceSequence(this.cc);
		
		ShuffleSequence();
	}
	public string GetQuestionText()
	{
		return this.question;
	}
	public List<Choice> GetSequence()
	{
		return this.ChoiceSequence;
	}
	
	private void AddToChoiceSequence(Choice choice)
	{
		ChoiceSequence.Add(choice);
	}
	
	private void ShuffleSequence()
	{	
		for(int i = ChoiceSequence.Count - 1; i > 0; i--)
		{
			int j = rand.Next(i + 1);
			Choice temp = ChoiceSequence[i];
			ChoiceSequence[i] = ChoiceSequence[j];
			ChoiceSequence[j] = temp;
		}
	}
	
	private void SetChoices(Choice choice, string text)
	{
		choice.InitializeText(text);
	}
}
public class Choice
{
	private protected bool isCorrect;
	private protected string text;
	public string GetText()
	{
		return this.text;
	}
	public bool GetStatus()
	{
		return this.isCorrect;
	}
	
	public void InitializeText(string text){
		this.text = text;
	}
}

public class WrongChoice : Choice
{
	public WrongChoice()
	{
		isCorrect = false;
	}
}

public class CorrectChoice : Choice
{
	public CorrectChoice()
	{
		isCorrect = true;
	}
}
