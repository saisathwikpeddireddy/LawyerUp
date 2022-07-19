using System;
using System.Data.SQLite;
using System.Numerics;


namespace LawyerUp
{
    
    internal class Program
    {
        static void Main(string[] args)
        {
            DisplayInstructions();
            string newtrial = "No";                     
                Console.WriteLine("Please enter your name to continue....");
                string username = Console.ReadLine();
                int namecheck = dbHelper.InsertUser(username);
                while (namecheck == 1)
                {
                    Console.WriteLine("Please type your name again...");
                    username = Console.ReadLine();
                    namecheck = dbHelper.InsertUser(username);
                    if (namecheck == 0)
                    {
                        break;
                    }
                }
                while (namecheck == 2)
                {
                    Console.WriteLine("Please type your name again...");
                    username = Console.ReadLine();
                    namecheck = dbHelper.InsertUser(username);
                    if (namecheck == 0)
                    {
                        break;
                    }
                }
           dbHelper.CreateSession(dbHelper.GetUserId(username));
           int userID = dbHelper.GetUserId(username);
           int sessionID = dbHelper.GetSessionId(userID);
           int HighScore = dbHelper.GetHighScore(userID); 
            do
            {
                int hints_taken = 0;
                Console.WriteLine("Type and choose a number representing any of the following Crime categories() to continue... (Example Input:1)\n      #1. Murder\n      #2. Robbery\n      #3. Reckless Driving\n");
                string crimeID = Console.ReadLine();
                int crimeVal = dbHelper.GetCrimeVal(crimeID);
                
                    var RndClient = dbHelper.GetRndClient(crimeID);
                
                int ClientVal = RndClient.Item1;
                int ClientID = RndClient.Item2;               
                while (ClientVal == -1)
                {
                    Console.WriteLine("Please type the chosen Crime category number again...(Note: Input should only be among '1,2 or 3'!)");
                    crimeID = Console.ReadLine();
                    ClientVal = dbHelper.GetRndClient(crimeID).Item1;
                   // Console.WriteLine(ClientVal);
                    if (ClientVal != -1)
                    {
                        break;
                    }
                }
                

                int[] opt_values = new int[10];
                for (int i = 1; i < 11; i++)
                {
                    int j = i - 1;
                    var result = dbHelper.GetDP(crimeID, i, hints_taken);
                    opt_values[j] = result.Item1;
                    hints_taken = result.Item2;                  
                    while (opt_values[j] == 0)
                    {
                        Console.WriteLine("Please ensure that the typed value is among'1','2' or '3'...\n");
                        var resultNew = dbHelper.GetDP(crimeID, i, hints_taken);
                        opt_values[j] = resultNew.Item1;
                        hints_taken = resultNew.Item2;                      
                        if (opt_values[j] != 0) break;
                    }
                }
              

                {
                    float length = len_sen(opt_values, ClientVal, crimeVal);
                    int Max_optValSum = dbHelper.GetMaxOptVal_Sum(int.Parse(crimeID));
                    float Maxlength = len_sen(Max_optValSum, ClientVal, crimeVal);
                    int Min_optValSum = dbHelper.GetMinOptVal_Sum(int.Parse(crimeID));
                    float Minlength = len_sen(Min_optValSum, ClientVal, crimeVal);
                    int len_reduced = (int)(Maxlength - length);
                    float trialscore = trialScore(Maxlength, Minlength, length) * 100;

                    Console.WriteLine($"\nThe client could've gotten a maximum of '{Maxlength}' years for their actions!");
                    Console.WriteLine($"\nYour desicions helped the client to get '{len_reduced}' years less than the Maximum sentence!");
                    dbHelper.AddTrialScore(sessionID, userID, (int)trialscore, ClientID,username);
                    dbHelper.UpdateSessionScore(sessionID, (int)trialscore);

                }

                do
                {
                    Console.WriteLine("\nDo you want to try defending another client?\nType 'Yes/yes' to continue or 'No/no' to end game\n\n");
                    newtrial = Console.ReadLine();

                } while (newtrial != "Yes" && newtrial != "No" && newtrial != "yes" && newtrial != "no");
            }   
            while (newtrial=="Yes"||newtrial=="yes");
            
            dbHelper.SessionTrials(sessionID);
           int currentSessionScore = dbHelper.GetSessionScore(sessionID);
            if (HighScore < currentSessionScore)
            {
                              
                Console.WriteLine($"\nYay {username} !!! You have made a new High Score of '{currentSessionScore}'.Good job!");
            }
            else 
            {
                Console.WriteLine($"\nHey {username},Your High Score still stays at '{HighScore}'.Try again to beat it!");
            }

                  
        }

        public static float trialScore(float Max,float Min,float Act)
        {
         float Score = (Max - Act) / (Max -Min);
            return Score;
        }
        public static float len_sen(int[] opt_values, int client_val, int crime_val)
        {
            float len = 0;
            foreach (int opt_value in opt_values)
            {
                 len =(len+opt_value) ;
            }
            len = (float)(Math.Pow(len,0.8) * Math.Pow(crime_val,1)* Math.Pow(client_val,1/5));
            Console.WriteLine($"\nLength of the sentence given to the client  :   {Math.Round(len / 12)} years");
            return (int)(len/12);
        }
        public static float len_sen(int opt_values_sum, int client_val, int crime_val)
        {
                      
          float  len = (float)(Math.Pow(opt_values_sum, 0.8) * Math.Pow(crime_val, 1) * Math.Pow(client_val, 1 / 5));
            //Console.WriteLine($"{Math.Round(len / 12)} years is the length of the sentence given to the client");
            return (int)(len / 12);
        }

        public static void DisplayInstructions()
        {
            Console.WriteLine("\t\t\t\t\tGame Name    :   LawyerUp");
            Console.WriteLine("\t\t\t\t   Developer Name    :  Sai Sathwik P");
            Console.WriteLine("\nIntro  :");
            Console.WriteLine("\nYou will be playing as a lawyer with the main goal being to reduce the " +
                "sentence length by defending a guilty client navigating the trial by making appropriate choices .");
            Console.WriteLine("\n\nHow to Play?");
            Console.WriteLine("\n\t# Choose any of the crimes given to get a client assigned ");
            Console.WriteLine("\n\t# Answer the questions posed in the trial by typing the appropriate option_no .\n\tNote:Try to pick the options that might possibly reduce the client's sentence length");
            Console.WriteLine("\n\t# You get 2 Hints to use if you are between options.\n\tNote:You can use these hints by typing 'H' as answer to any question!");
            Console.WriteLine("\n\tYour total score keeps adding up the more trials you play!\n\nAll the best!!!\n");
        }

    }

       
}