using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace Quiz_Game
{
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
            string textResponse = Console.ReadLine();

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
            Console.WriteLine(question);
            return Console.ReadLine();
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
            Console.Write("(" + option1 + ", " + option2 + ", " + option3 + ")");
            do
            {
                var response = Console.ReadLine().Trim().ToLower();

                //we want to do a case insensitive check, so make sure to trim to remove whitespace, and also convert to lower case.
                //We'll still return the original option argument though, so we maintain the case we were given.
                if (response == option1.Trim().ToLower())
                {
                    return option1;
                }
                else if (response == option2.Trim().ToLower())
                {
                    return option2;
                }
                else if (response == option3.Trim().ToLower())
                {
                    return option3;
                }
            } while (true);
        }

        //AskMultipleChoice
        //Create a method for returning a number based on three posible options. The method will take three string arguments, print out the options to the Console, and return the number of the one they select
        /// <summary>
        /// Ask a multiple choice question, and return the number that they selected
        /// </summary>
        /// <param name="option1">The first option</param>
        /// <param name="option2">The second option</param>
        /// <param name="option3">The third option</param>
        /// <returns>The number the user selected, otherwise if invalid response, return NULL</returns>
        int? AskMultipleChoice(string option1, string option2, string option3)
        {
            Console.WriteLine("1) " + option1);
            Console.WriteLine("2) " + option2);
            Console.WriteLine("3) " + option3);

            //We only have three options, so make the user pick a number between 1 and 3
            return ReadNumberResponse(1, 3);
        }

        /// <summary>
        /// Read a Yes or No response from the user
        /// </summary>
        /// <returns>If the user entered text starting with 'y' return TRUE, otherwise return FALSE</returns>
        bool ReadYesNo()
        {
            string response = Console.ReadLine();
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
            Console.Write("Press [Enter] to continue");
            Console.ReadLine();
        }


        /// <summary>
        /// The main logic for the game
        /// </summary>
        public void Main(string[] args)
        {
            Console.WriteLine("\nWelcome to The Quizz Show!");

            //Get name and handle it
            string name = AskQuestion("\nWhat is your name?").ToLower().Trim();
            // Return char and concat substring
            name = char.ToUpper(name[0]) + name.Substring(1);

            Console.WriteLine("Welcome, " + name + ". It is a pleasure to have you on the show tonight.");
            Thread.Sleep(2000);
            //Raw qAmount
            string qAmountRaw = AskQuestion("\nHow many questions do you want to answer tonight, " + name + "? You can either play 7, 12 or 15 questions.");
            //Regexifying charcters and spaces out
            qAmountRaw = Regex.Replace(qAmountRaw, @"[a-zA-Z, ]", "");
            //Turn string to int
            int qAmount = Convert.ToInt32(qAmountRaw);

            if (qAmount == 7 || qAmount == 12 || qAmount == 15)
            {
                Console.WriteLine("\nYou chose to play: " + qAmount + " games.");
            }
            else
            {
                Console.WriteLine("\nYou did not enter 7 or 12 or 15.");
                Thread.Sleep(3000);
                return;
            }

        }








        /*
        
        string questionsFile = @"questions.txt";
        Dictionary<string, string> questionArray = new Dictionary<string, string>();
        int score = 0;


        LoadData();
            StartQuiz();

        Console.WriteLine("Your score is: {0}", score);
            Console.ReadLine();



        void LoadData()
        {
            FileInfo file = new FileInfo(questionsFile);
            if (file.Exists)
            {
                using (StreamReader reader = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] splitted = line.Split('=');
                        if (splitted.Length == 2)
                        {
                            questionArray.Add(splitted[0], splitted[1]);
                        }
                    }
                }
            }
        }
 
        void StartQuiz()
        {
            for (int i = 0; i<questionArray.Count; i++)
            {
                AskQuestions(i);
            }
        }
 
        void AskQuestions(int questionIndex)
        {
            string question = questionArray.Keys.ElementAt(questionIndex);
            string correctAnswer = questionArray.Values.ElementAt(questionIndex);
            Console.Write("{0} ", question);
            string userAnswer = Console.ReadLine();
 
            if (userAnswer != correctAnswer)
            {
                Console.WriteLine("\tWRONG ANSWER!");
                //AskQuestions(questionIndex);
                score -= 2;
            }
            else
            {
                score += 5;
            }
        }*/
    }
}