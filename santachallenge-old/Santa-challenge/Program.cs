using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClassicLogger;
using Microsoft.Win32;

namespace Santa_challenge
{
    /// <summary>
    /// Program is designed to solve the santa challenge by Reaktor
    /// https://traveling-santa.reaktor.com/
    /// nick: hope-santa-brings-vergi
    /// </summary>
    public class Program
    {
        public const char SEPARATOR = ';';
        public const string BASH_SYMBOL = "<santa-helper> ";
        const string PROGRAM_NAME = "Santa-challenge.exe";

        private static ILogger logger;

        public static void Main(string[] args)
        {
            logger = InitializeLogger("Main");

            try
            {
                // Use stopwatch for time measuring
                var watch = new Stopwatch();
                watch.Start();

                Print("Looking for the nice list");

                var nicelist = FileHandler.ReadNiceList(ListSize.Small100);
                
                if (ListOk(nicelist))
                {
                    PrintEmptyLine();

                    TimeElapsedAndStartAgain(watch);
                    Print("List ok. Creating WishList-object...");

                    WishList list = new WishList(nicelist);

                    TimeElapsedAndStartAgain(watch);
                    Print("Object ok. Choose which algorithm to use");
                    PrintEmptyLine();

                    Print("1. Go through the list in normal order =). Correct output test");
                    Print("2. Go to closest nodes, return. Again to closest nodes, return...");
                    Print("3. Use Kruskal's minimum spanning tree algorithm");
                    Print("4. MiniMax");
                    Print("5. TODO");
                    Print("6. TODO");
                    Print("7. TODO");
                    Print("8. TODO");
                    Print("0. Quit");

                    Console.Write(BASH_SYMBOL);
                    var numberPressed = Console.ReadKey().KeyChar;
                    PrintEmptyLine();
                    PrintEmptyLine();
                    var algorithm = new Algorithm(list);
                    var answerList = new List<string>();

                    bool orderCreationOk = true;


                    if (numberPressed == '1')
                    {
                        answerList = algorithm.GoThroughListInOrder();
                    }
                    else if (numberPressed == '2')
                    {
                        answerList = algorithm.UseClosestNodes();
                    }
                    else if (numberPressed == '3')
                    {
                        answerList = algorithm.UseKruskal(watch);
                    }
                    else if (numberPressed == '4')
                    {
                        throw new NotImplementedException();
                    }
                    else if (numberPressed == '5')
                    {
                        throw new NotImplementedException();
                    }
                    else if (numberPressed == '6')
                    {
                        throw new NotImplementedException();
                    }
                    else if (numberPressed == '7')
                    {
                        throw new NotImplementedException();
                    }
                    else if (numberPressed == '8')
                    {
                        throw new NotImplementedException();
                    }
                    else if (numberPressed == '0')
                    {
                        Print("Software closed");
                        orderCreationOk = false;
                    }
                    else
                    {
                        Print("Invalid input");
                        orderCreationOk = false;
                    }

                    if (orderCreationOk && answerList.Count > 0)
                    {
                        TimeElapsedAndStartAgain(watch);
                        Print("Order created. Writing output file");

                        FileHandler.WriteAnswerFile(answerList);

                        TimeElapsedAndStartAgain(watch);
                        Print("File created");
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            // Give function name as exception message to this
            catch (NotImplementedException ex)
            {
                // 
                logger.Debug(ex.ToString());
                Debug.WriteLine(ex.ToString());
                Print("ERROR: Feature [" + ex.Message + "] not implemented yet. Contact " + PROGRAM_NAME + " provider for more info.");
            }
            catch (UnauthorizedAccessException ex)
            {
                // 
                logger.Debug(ex.ToString());
                Debug.WriteLine(ex.ToString());
                Print("ERROR: Unauthorized access. Please start app again in admin mode.");
            }
            catch (IOException ex)
            {
                // 
                logger.Debug(ex.ToString());
                Debug.WriteLine(ex.ToString());
                Print("ERROR: IOException " + ex.Message);
            }
            // Random exception
            catch (Exception ex)
            {
                logger.Debug(ex.ToString());
                Debug.WriteLine(ex.ToString());
                Print("CRITICAL ERROR: Exception " + ex.ToString());
            }


            PrintEmptyLine();
            Print("Press any key to exit...");
            Console.ReadKey();
        }

        public static ILogger InitializeLogger(Type type)
        {
            ILogger logger = LoggerFactory.GetLogger(type);
            logger.Debug("Initializing " + type.Name + "...");

            return logger;
        }

        public static ILogger InitializeLogger(string functionName)
        {
            ILogger logger = LoggerFactory.GetLogger(functionName);
            logger.Debug("Initializing " + functionName + "...");

            return logger;
        }

        public static void TimeElapsedAndStartAgain(Stopwatch watch)
        {
            watch.Stop();

            Print("Execution time: " + watch.ElapsedMilliseconds + "ms");
            watch.Reset();
            watch.Start();
        }

        private static bool ListOk(List<string> list)
        {
            if (list == null) return false;
            if (list.Count == 0) return false;

            return true;
        }

        public static void Print(string text)
        {
            logger.Info(BASH_SYMBOL + text);
            Console.WriteLine(BASH_SYMBOL + text);
        }
        public static void PrintEmptyLine()
        {
            Console.WriteLine();
        }

    }
}
