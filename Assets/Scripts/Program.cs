using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

/// <summary>
/// Terminal Program Code
/// </summary>
public class Program
{    
    //ReadNumberResposne
    //Create a method that will read a number within a range. If the user enters a number outside of that range, return null
    /// <summary>
    /// Read a number within a given range. If the number is outside of that range, return null.
    /// </summary>
    /// <param name="min">The minimum number to pick from (inclusive)</param>
    /// <param name="max">The maximum number to pick from (inclusive)</param>
    /// <returns>A number between the minimum and maximum (inclusive), or NULL if invalid.</returns>
    int? ReadNumberResponse(int min, int max)
    {
        int response;
        string textResponse = Terminal.ReadLine();

        if (int.TryParse(textResponse, out response))
        {
            if (response > max) return null;
            return response;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Output a question and return the response
    /// </summary>
    /// <param name="question">The question to ask the player</param>
    /// <returns>The answer entered by the player</returns>
    string AskQuestion(string question)
    {
        Terminal.WriteLine(question);
        return Terminal.ReadLine();
    }

	int AskHitpoints(string numQ)
	{
		Terminal.WriteLine(numQ);
		int convert = Convert.ToInt32 (Terminal.ReadLine ());
		if (convert > 100) {
			return Terminal.Random(1,101);
		}
		return convert;
	}

    /// <summary>
    /// Ask for a selection from three different options
    /// </summary>
    /// <param name="option1">The first option</param>
    /// <param name="option2">The second option</param>
    /// <param name="option3">The third option</param>
    /// <returns>The option the player entered. If the player entered an invalid option, return NULL.</returns>
    string AskForSelection(string option1, string option2, string option3)
    {
        Terminal.Write("(" + option1 + ", " + option2 + ", " + option3 + ")");
		do {
						var response = Terminal.ReadLine ().Trim ().ToLower ();

						//we want to do a case insensitive check, so make sure to trim to remove whitespace, and also convert to lower case.
						//We'll still return the original option argument though, so we maintain the case we were given.
						if (response == option1.Trim ().ToLower ()) {
								return option1;
						} else if (response == option2.Trim ().ToLower ()) {
								return option2;
						} else if (response == option3.Trim ().ToLower ()) {
								return option3;
						}
				} while (true);
    }

    //AskMultipleChoice
    //Create a method for returning a number based on three posible options. The method will take three string arguments, print out the options to the terminal, and return the number of the one they select
    /// <summary>
    /// Ask a multiple choice question, and return the number that they selected
    /// </summary>
    /// <param name="option1">The first option</param>
    /// <param name="option2">The second option</param>
    /// <param name="option3">The third option</param>
    /// <returns>The number the user selected, otherwise if invalid response, return NULL</returns>
    int? AskMultipleChoice(string option1, string option2, string option3)
    {
        Terminal.WriteLine("1) " + option1);
        Terminal.WriteLine("2) " + option2);
        Terminal.WriteLine("3) " + option3);

        //We only have three options, so make the user pick a number between 1 and 3
        return ReadNumberResponse(1, 3);
    }

    /// <summary>
    /// Read a Yes or No response from the user
    /// </summary>
    /// <returns>If the user entered text starting with 'y' return TRUE, otherwise return FALSE</returns>
    bool ReadYesNo()
    {
        string response = Terminal.ReadLine();
        return isYesResponse(response);
    }

    /// <summary>
    /// Prompt the user with a confirmation question
    /// </summary>
    /// <returns>If the user enters something starting with a 'y', return TRUE, otherwise return FALSE.</returns>
    bool ConfirmResponse()
    {
        string response = AskQuestion("Are you sure? [y/n]");
        return isYesResponse(response);
    }

    /// <summary>
    /// Check if the player has entered a response starting with 'y'
    /// </summary>
    /// <param name="response">If the player entered something starting with 'y', return TRUE, otherwise return FALSE.</param>
    /// <returns></returns>
    bool isYesResponse(string response)
    {
        if (string.IsNullOrEmpty(response) || response.ToLower()[0] != 'y')
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// Output a wait message and wait for the player to press enter to continue
    /// </summary>
    void WaitForEnter()
    {
        Terminal.Write("Press [Enter] to continue");
        Terminal.ReadLine();
    }

    /// <summary>
    /// The main logic for the game
    /// </summary>
    public void Run()
    {
		Terminal.WriteLine("\nWelcome to The Quizz Show!");

		//Get name and handle it
		string name = AskQuestion("\nWhat is your name?").ToLower().Trim ();
		// Return char and concat substring
		name = char.ToUpper(name[0]) + name.Substring(1);

		Terminal.WriteLine("Welcome, " + name + ". It is a pleasure to have you on the show tonight.");
		Thread.Sleep(2000);
		//Raw qAmount
		string qAmountRaw = AskQuestion("\nHow many questions do you want to answer tonight, " + name + "? You can either play 7, 12 or 15 questions.");
		//Regexifying charcters and spaces out
		qAmountRaw = Regex.Replace(qAmountRaw, @"[a-zA-Z, ]", "");
		//Turn string to int
		int qAmount = Convert.ToInt32(qAmountRaw);

		if (qAmount == 7 || qAmount == 12 || qAmount == 15) {
			Terminal.WriteLine("\nYou chose to play: " + qAmount + " games.");
		} else {
			Terminal.WriteLine("\nYou did not enter 7 or 12 or 15.");
			Thread.Sleep(3000);
			return;
		}

		int score = 0;



















































		/*
        string player1 = "Bob ";
        string player2 = "Billy ";
        string player3 = "Timmy ";
        string player4 = "Zack ";
        string player5 = "Rhys ";

        string[] names = new string[5];
		names [0] = player1;
		names [1] = player2;
		names [2] = player3;
		names [3] = player4;
		names [4] = player5;


       Terminal.WriteLine("Pick a number between 1 and 3");
        int? selection = ReadNumberResponse(1, 3);
        
        Terminal.WriteLine(names[1-1] + names[3-1] + names[5-1]);



		int hp1 = AskHitpoints ("How many HP (1,100?) for player 1?");
		int hp2 = AskHitpoints ("How many HP (1,100?) for player 2?");
		int hp3 = AskHitpoints ("How many HP (1,100?) for player 3?");
		int hp4 = AskHitpoints ("How many HP (1,100?) for player 4?");
		int hp5 = AskHitpoints ("How many HP (1,100?) for player 5?");

		int[] numbers = new int[5];
		numbers [0] = hp1;
		numbers [1] = hp2;
		numbers [2] = hp3;
		numbers [3] = hp4;
		numbers [4] = hp5;

		for (var i = 0; i < 5; i++) {
			Terminal.WriteLine (names [i] + " has " + numbers [i] + " hitpoints.");
		}


		Terminal.WriteLine ("Enter a number between 1 and 100.");
		int numTimes = Convert.ToInt32(Terminal.ReadLine ());

		//for (int i = numTimes; i > 0; i--) {
		//	Terminal.WriteLine("Current number is: " + i);
		//}

		if (numTimes > 100) {
			return;
		} else if ((numTimes % 3) == 0 && (numTimes % 5) == 0) {
			Terminal.WriteLine ("FizzBuzz");
		} else if ((numTimes % 3) == 0) {
			Terminal.WriteLine ("Fizz");
		} else if ((numTimes % 5) == 0) {
			Terminal.WriteLine("Buzz");
		} else {
			Terminal.WriteLine("Your number is not a multiple of 3 or 5");
		}*/
    }
}
