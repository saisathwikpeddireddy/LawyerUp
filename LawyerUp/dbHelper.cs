using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Data.SQLite;


namespace LawyerUp
{
    public static class dbHelper
    {
        public static void GetAllUsers()
        {

            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();
                    //var query = @"Insert into user(name) Values('Sai2') ;";
                    cmd.CommandText = @"Select * from user;";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("id   name");
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["id"] + "    " + reader["name"]);
                        }
                    }
                    connection.Close();
                }
            }


            Console.WriteLine("Database connection successfull" + "  " + connection.ConnectionString);
        }

        public static int InsertUser(string username)
        {
            using System.Data.SQLite.SQLiteConnection connection =
                      new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();
                    SQLiteCommand CheckUser = new SQLiteCommand($"SELECT * FROM user where user.name = '{username}'", connection);

                    using (System.Data.SQLite.SQLiteDataReader reader = CheckUser.ExecuteReader())
                    {

                        //Console.WriteLine("Checking if username available...");
                        while (reader.Read())
                        {
                            Console.WriteLine($"User with name '{username}' already exists.\n" +
                                $"Press 'Y' if Would you like to continue with the same name.\n" +
                                $"Press'N' if you would like to reset the username");
                            string reply = Console.ReadLine();
                            if (reply == "Y" || reply == "y")
                            {
                               // Console.WriteLine("The Game Continues...");

                                return 0;

                                break;
                            }
                            else if (reply == "N" || reply == "n")
                            {

                                return 1;

                            }
                            else
                            {
                                Console.WriteLine(@$"Only 'Y/y' and 'N/n' are accepted,please verify your input and type again!");
                                // validator = false;
                                return 2;
                            }
                        }
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }


                        SQLiteCommand insertQuery = new SQLiteCommand($"INSERT INTO user(name) VALUES ('{username}')", connection);

                        insertQuery.ExecuteNonQuery();
                        Console.WriteLine("User succesfully created!");
                    }

                    return 0;

                    connection.Close();
                }
            }
        }

        public static void GetAllCrimes()
        {

            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    cmd.CommandText = @"Select * from Crime;";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("id   Crime   hostVal");
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["id"] + "    " + reader["name"] + "    " + reader["hostVal"]);
                        }
                    }
                    connection.Close();
                }
            }



        }

        public static int GetCrimeVal(string crimeID)
        {

            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    cmd.CommandText = @$"Select hostVal from Crime where Crime.id={crimeID} ;";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        //Console.WriteLine("Crime Value Detected");
                        while (reader.Read())
                        {
                            return (int)reader["hostVal"];
                        }
                    }

                    connection.Close();
                    return -1;
                }
            }
        }

        public static (int,int) GetRndClient(string crimeID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    // SQLiteCommand.cmd = new SQLiteCommand ($" SELECT ClientInfo.firstName,ClientInfo.secondName,ClientInfo.Age,ClientInfo.Country, ClientInfo.Gender,ClientInfo.'Marital Status',ClientInfo.Occupation,ClientInfo.'Criminal history',Crime.name FROM ClientInfo INNER JOIN Crime  ON ClientInfo.Crime_id = Crime.id WHERE Crime.name='{crimeName}'",connection);
                    cmd.CommandText = $" SELECT ClientInfo.firstName,ClientInfo.secondName,ClientInfo.Age,ClientInfo.Country, ClientInfo.Gender,ClientInfo.'Marital Status',ClientInfo.Occupation,ClientInfo.'Criminal history',Crime.name FROM ClientInfo INNER JOIN Crime  ON ClientInfo.Crime_id = Crime.id WHERE Crime.id='{crimeID}'ORDER By RANDOM () LIMIT 1";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        // Console.WriteLine(reader.Read());
                        if (reader.Read() == false || crimeID == "4" || crimeID == "5" || crimeID == "6")
                        {
                            //Console.WriteLine(reader.Read());
                            return (-1,-1);
                        }
                    }
                    cmd.CommandText = $" SELECT ClientInfo.id,ClientInfo.firstName,ClientInfo.hostVal,ClientInfo.secondName,ClientInfo.Age,ClientInfo.Country, ClientInfo.Gender,ClientInfo.'Marital Status',ClientInfo.Occupation,ClientInfo.'Criminal history',Crime.name FROM ClientInfo INNER JOIN Crime  ON ClientInfo.Crime_id = Crime.id WHERE Crime.id='{crimeID}'ORDER By RANDOM () LIMIT 1";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        //Console.WriteLine(" Client_Name  Age Country Gender  Marital_Status     Occupation  Criminal_History    Crime_Commited   ");
                        while (reader.Read())
                        {
                            int clientID = reader.GetInt32("id");
                            int client_val = (int)reader["hostVal"];
                            Console.WriteLine("\n\t\tClient_Name:" + reader["firstName"] + " " + reader["secondName"] + "    \n" + "\t\tAge:" + reader["Age"] + "   \n" + "\t\tCountry:" + reader["Country"]
                                + "  \n" + "\t\tMarital Status:" + reader["Marital Status"] + "  \n" + "\t\tOccupation:" + reader["Occupation"] + "  \n" + "\t\tCriminal History:" + reader["Criminal History"] + "  \n" + "\t\tCrime Commited:" + reader["name"]);
                            connection.Close();
                            return (client_val,clientID);
                        }

                        connection.Close();
                        return (0,0);
                    }

                }
            }
        }

        public static Tuple<int, int> GetDP(string crimeID, int question_no, int hints_taken)
        {
            
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();
                    // for ( int i = 1; i < 11; i++)
                    {
                        cmd.CommandText = $" SELECT DP.question,OP.option_1,OP.option_2,OP.option_3,OP.hostVal_1,OP.hostVal_2,OP.hostVal_3 from 'Desicion Point' as DP INNER JOIN 'Options' as OP ON DP.id=OP.DP_id WHERE DP.Crime_id={crimeID} AND DP.question_no={question_no} ;";

                        using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {

                                Console.WriteLine("\nQuestion "+question_no +" : "+ reader["question"] + " \n\t1." + reader["option_1"] + " \n\t2." + reader["option_2"] + " \n\t3." + reader["option_3"]);
                                Console.WriteLine("\nYour Answer : ");
                                string Ans = Console.ReadLine();



                                if (Ans == "H")
                                {

                                    hints_taken++;

                                    if (hints_taken <= 2)

                                        Ans = GetHint(crimeID, question_no);
                                    else
                                        Console.WriteLine("Oops...You used all the 2 hints available!");



                                }
                                if (Ans == "1")
                                {
                                    var opt_val = reader["hostVal_1"];
                                    return new Tuple<int, int>((int)opt_val, hints_taken);
                                }
                                else if (Ans == "2")
                                {
                                    var opt_val = reader["hostVal_2"];
                                    return new Tuple<int, int>((int)opt_val, hints_taken);
                                }
                                else if (Ans == "3")
                                {
                                    var opt_val = reader["hostVal_3"];
                                    return new Tuple<int, int>((int)opt_val, hints_taken);
                                }

                                else
                                {
                                    return new Tuple<int, int>(0, hints_taken);
                                }

                            }

                        }
                    }
                }
            }
            return new Tuple<int, int>(0, hints_taken);
        }

        public static string GetHint(string crimeID, int question_no)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    {
                        cmd.CommandText =
                         @$" SELECT min(hostVal_1, hostVal_2, hostVal_3) AS minValue,
                         (CASE min(hostVal_1, hostVal_2, hostVal_3)
                           WHEN hostVal_1 THEN 'Option_1'
                           WHEN hostVal_2 THEN 'Option_2'
                           WHEN hostVal_3 THEN 'Option_3'
                          END) as option_no from 'Desicion Point' as DP INNER JOIN 'Options' as OP ON DP.id=OP.DP_id 
                          WHERE DP.Crime_id={crimeID} AND DP.question_no={question_no} ;";

                        using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Console.WriteLine("For this question, " + reader["option_no"] + " will result in the least sentence possible!");
                                return Console.ReadLine();
                            }


                        }
                    }
                    connection.Close();
                    return "Complete";
                }
            }
        }

        public static void AddTrialScore(int sessionId, int userID, int trialScore, int ClientInfoId,string username)
        {
            using System.Data.SQLite.SQLiteConnection connection =
                      new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();


                    SQLiteCommand insertQuery = new SQLiteCommand($"INSERT INTO trial(Session_id,user_id,trial_score,clientInfo_id) VALUES ('{sessionId}','{userID}','{trialScore}','{ClientInfoId}')", connection);

                    insertQuery.ExecuteNonQuery();
                    // Console.WriteLine("Trial succesfully created!");
                    Console.WriteLine($"\nCongratulations {username}!You have secured '{trialScore}'/100 for this trial");
                    connection.Close();
                }

            }
        }

        public static void CreateSession(int UserId)
        {
            using System.Data.SQLite.SQLiteConnection connection =
                      new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();


                    SQLiteCommand insertQuery = new SQLiteCommand($"INSERT INTO Session(user_id,session_score) VALUES ({UserId},0)", connection);

                    insertQuery.ExecuteNonQuery();

                    // Console.WriteLine("Trial succesfully created!");
                  //  Console.WriteLine($"A new session is created for {UserId}");
                    connection.Close();

                }

            }

        }

        public static int GetUserId(string username)
         {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();
                    //var query = @"Insert into user(name) Values('Sai2') ;";
                    cmd.CommandText = @$"Select id from user where name = '{username}';";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                       
                        while (reader.Read())
                        {
                            var UserId = reader.GetInt32("id");
                            //Console.WriteLine(UserId + "is id of   " +username);
                            connection.Close();
                            return UserId;
                        }
                    }
                    connection.Close();
                }
            }           
            return 0;
         }

        public static int GetSessionId(int userID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();
                    
                    cmd.CommandText = @$"Select id from session where user_id = {userID} order by id desc limit 1;";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var SessionId = reader.GetInt32("id");
                           // Console.WriteLine(SessionId + "is id of   " + userID);
                            connection.Close();
                            return SessionId;
                        }
                    }
                    connection.Close();
                }
            }
            return 0;
        }

        public static void UpdateSessionScore(int sessionId, int trialScore)
        {
            using System.Data.SQLite.SQLiteConnection connection =
                      new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();


                    SQLiteCommand updateQuery = new SQLiteCommand($"update session set session_score = session_score+{trialScore} where id = {sessionId};", connection);

                    updateQuery.ExecuteNonQuery();
                    // Console.WriteLine("Trial succesfully created!");
                   // Console.WriteLine($"Congratulations!We have added {trialScore} for this session : {sessionId}");
                    connection.Close();
                }

            }
        }

        public static int GetMaxOptVal_Sum(int crimeID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    {
                        cmd.CommandText =
                         @$"SELECT sum(max(hostVal_1, hostVal_2, hostVal_3)) AS maxValueSum,
                         (CASE max(hostVal_1, hostVal_2, hostVal_3)
                           WHEN hostVal_1 THEN 'Option_1'
                           WHEN hostVal_2 THEN 'Option_2'
                           WHEN hostVal_3 THEN 'Option_3'
                          END) as option_no from 'Desicion Point' as DP INNER JOIN 'Options' as OP ON DP.id=OP.DP_id 
                          WHERE DP.Crime_id={crimeID} AND DP.question_no<11 ; ";

                        using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                        {


                            while (reader.Read())
                            {
                                var maxVal = reader.GetInt32("maxValueSum");
                               // Console.WriteLine("For this crime, " + maxVal + " is the max sum!");
                                connection.Close();
                                return maxVal;
                            }


                        }
                    }
                    connection.Close();
                    return 0;
                }
            }
        }

        public static int GetMinOptVal_Sum(int crimeID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    {
                        cmd.CommandText =
                         @$"SELECT sum(min(hostVal_1, hostVal_2, hostVal_3)) AS minValueSum,
                         (CASE min(hostVal_1, hostVal_2, hostVal_3)
                           WHEN hostVal_1 THEN 'Option_1'
                           WHEN hostVal_2 THEN 'Option_2'
                           WHEN hostVal_3 THEN 'Option_3'
                          END) as option_no from 'Desicion Point' as DP INNER JOIN 'Options' as OP ON DP.id=OP.DP_id 
                          WHERE DP.Crime_id={crimeID} AND DP.question_no<11 ; ";

                        using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                        {


                            while (reader.Read())
                            {
                                var minVal = reader.GetInt32("minValueSum");
                              //  Console.WriteLine("For this crime, " + minVal + " is the max sum!");
                                connection.Close();
                                return minVal;
                            }


                        }
                    }
                    connection.Close();
                    return 0;
                }
            }
        }

        public static int GetSessionScore(int sessionID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    cmd.CommandText = @$"Select session_score from session where id = {sessionID};";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var SessionScore = reader.GetInt32("session_score");
                            Console.WriteLine("\nTotal score for the current session  :   "+SessionScore);
                            connection.Close();
                            return SessionScore;
                        }
                    }
                    connection.Close();
                }
            }
            return 0;
        }

        public static void SessionTrials(int sessionID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    cmd.CommandText = @$"SELECT trial_score from Trial where Session_id={sessionID};";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        int i = 1;
                        while (reader.Read())
                        {
                            
                            var TrialScore = reader.GetInt32("trial_score");
                            Console.WriteLine("\n");
                            Console.WriteLine("Trial #"+i+"  Score    :    "+TrialScore+" out of 100");
                            i++;
                           
                        }
                    }
                    connection.Close();
                }
            }
           ;
        }

        public static int GetHighScore(int userID)
        {
            using System.Data.SQLite.SQLiteConnection connection =
               new System.Data.SQLite.SQLiteConnection("Datasource = SQLiteDB.db");
            {
                using (System.Data.SQLite.SQLiteCommand cmd =
                    new System.Data.SQLite.SQLiteCommand(connection))
                {
                    connection.Open();

                    cmd.CommandText = @$"Select session_score from session where user_id = {userID} order by session_score desc limit 1;";
                    using (System.Data.SQLite.SQLiteDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var HighScore = reader.GetInt32("session_score");
                            Console.WriteLine($"\nYour HighScore : {HighScore}\n");
                            connection.Close();
                            return HighScore;
                        }
                    }
                    connection.Close();
                }
            }
            return 0;
        }
    }
}
    


