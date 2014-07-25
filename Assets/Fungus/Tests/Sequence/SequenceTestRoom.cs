using UnityEngine;
using System.Collections;
using Fungus;

public class SequenceTestRoom : Room 
{
	public SequenceController sequenceController;

	void OnEnter() 
	{
		sequenceController.Execute();

		/*
		Sequence s = GetComponent<Sequence>();

		s.Add(new Say("New sequencer 1"));
		s.Add(new AddOption("Button 1", Clicked));
		//s.Add(new AddOption("Button 2", Clicked));
		//s.Add(new AddOption("Button 3", Clicked));
		s.Add(new Say("New sequencer 2"));
		s.Add(new Say("New sequencer 3"));

		s.Execute();
		*/
	}

	void Clicked()
	{
		/*
		Sequence s = GetComponent<Sequence>();

		s.Clear();
		s.Add(new Say("You have clicked"));
		s.Execute();
		*/
	}
}
