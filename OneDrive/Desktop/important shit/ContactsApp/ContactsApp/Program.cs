using System;
using System.IO;

namespace ContactsApp
{
    class Program
    {
        public static string filePath = "C:\\Users\\cockus fattus\\OneDrive\\Documents\\contacts.cnt";

        static void Main(string[] args)
        {
            int userSelection = 0;
            bool exitProgram = false;

            while (!exitProgram)
            {
                exitProgram = false;
                Console.WriteLine("Welcome to your contacts!\n");
                userSelection = GetSelection();
                //selection menu
                if (userSelection == 0)
                {
                    AddRecord();
                }
                else if (userSelection == 1)
                {
                    SearchForName();
                }
                else if (userSelection == 2)
                {
                    ModifyPhoneNumberSelect();
                }
                else if (userSelection == 3)
                {
                    ModifyEmailSelect();
                }
                else if (userSelection == 4)
                {
                    DeleteContactSelect();
                }
                else if (userSelection == 5)
                {
                    ListContacts();
                }
                else if (userSelection == 6)
                {
                    Environment.Exit(0);
                    exitProgram = true;
                }
            }
        }

        static void AddRecord()
        {
            Console.WriteLine("You have chosen to add a new record");
            StreamWriter infile = File.AppendText(filePath);

            string[] recordInputArray = new string[4]; //array to hold record input

            Console.WriteLine("Input first name:");
            recordInputArray[0] = Console.ReadLine().Trim().ToLower();   //inputs for all record slots

            Console.WriteLine("Input last name:");
            recordInputArray[1] = Console.ReadLine().Trim().ToLower();


            int phoneNum = PromptInt("Input phone number: ");
            recordInputArray[2] = phoneNum.ToString();

            Console.WriteLine("Input email:");
            recordInputArray[3] = Console.ReadLine().Trim().ToLower();


            string recordInputAsString = string.Join(" ", recordInputArray);
            infile.WriteLine(recordInputAsString);

            infile.Close();//file close

            Console.WriteLine("Returning to menu...");//allows user to make another selection
            Console.WriteLine();
            return;
        }

        static void SearchForName()
        {
            Console.WriteLine("You have chosen to search for a name");

            Console.WriteLine("Please input the first name of the person you are looking for");
            string firstName = Console.ReadLine().Trim().ToLower();      //get first name

            Console.WriteLine("Please input the last name of the person you are looking for");
            string lastName = Console.ReadLine().Trim().ToLower();       //get last name

            bool matchFound = false;    //bool to prevent loop repeating after finding a match
            StreamReader recordData = new StreamReader(filePath);

            while (recordData.EndOfStream == false && matchFound == false)
            {
                string record = recordData.ReadLine();  //current record
                string[] fields = record.Split();       //fields (fn, ln, pn, em)

                if (firstName == fields[0] && lastName == fields[1])
                {
                    matchFound = true;
                    Console.WriteLine("Match found!");
                    Console.WriteLine("Displaying contact...");

                    for (int index = 0; index < fields.Length; index++)
                    {
                        Console.Write(fields[index] + " ");     //displays contact info
                    }
                    Console.WriteLine();
                }
                else
                {
                    matchFound = false;
                }
            }

            if (recordData.EndOfStream && !matchFound)
            {
                Console.WriteLine("Contact not found. Returning to menu...");
            }
            recordData.Close();//close file

            return;
        }

        static void ModifyPhoneNumberSelect()
        {
            Console.WriteLine("You have chosen to modify a phone number");

            Console.WriteLine("Please enter the first name of the contact you would like to modify:");
            string firstName = Console.ReadLine().Trim().ToLower();     //get first

            Console.WriteLine("Please enter the last name of the contact you would like to modify:");
            string lastName = Console.ReadLine().Trim().ToLower();      //get last
            int lineNum = 0;
            string recordToModify = "";
            bool matchFound = false;    //had a bug that would make the while loop repeat after changing a record. this prevented that issue

            StreamReader recordData = new StreamReader(filePath);

            while (recordData.EndOfStream == false && !matchFound)
            {
                string record = recordData.ReadLine();      //current record
                string[] fields = record.Split();           //fields of current record

                if (firstName != fields[0] && lastName != fields[1])
                {
                    lineNum++;                                           //if the current record fields do not match the required values,
                    matchFound = false;                                  //increment the line number, if they do match, then the record to
                }                                                        //modify is on that line number

                if (firstName == fields[0] && lastName == fields[1])
                {
                    matchFound = true;
                    Console.WriteLine("Match found!");
                    Console.WriteLine("Displaying contact...");

                    for (int index = 0; index < fields.Length; index++)
                    {
                        Console.Write(fields[index] + " ");
                    }

                    Console.WriteLine();

                    int phoneNum = PromptInt("Input phone number: ");
                    fields[2] = phoneNum.ToString();
                    
                    Console.WriteLine();

                    Console.WriteLine("New contact info: ");
                    for (int i = 0; i < 4; i++)
                    {

                        Console.Write(fields[i] + " ");
                    }

                    recordToModify = string.Join(" ", fields);
                }//end if
                else
                {
                    matchFound = false;
                }
            }
            recordData.Close();

            if (matchFound)
            {
                ModifyNumber(recordToModify, lineNum);
            }

            if (!matchFound)
            {
                Console.WriteLine("Contact not found. Returning to menu...");
                return;
            }   
        }

        static int GetRecordCount()
        {
            StreamReader countLines = new StreamReader(filePath);
            int lineCount = 0;

            while (countLines.EndOfStream == false)
            {
                countLines.ReadLine();  //reads a line from the file
                lineCount++;            //increments to get the number of lines
            }
            countLines.Close();
            return lineCount;           //return the line count
        }

        static string[] GetAllRecords()
        {
            int lineCount = GetRecordCount();
            string[] allRecords = new string[lineCount];
            StreamReader readFile = new StreamReader(filePath);

            for (int i = 0; i < lineCount; i++)
            {
                allRecords[i] = readFile.ReadLine();//store all records into array
            }
            readFile.Close();

            return allRecords;
        }

        static void ModifyNumber(string recordToModify, int lineNum)
        {
            int lineCount = GetRecordCount();
            string[] allRecords = new string[lineCount];    //array that stores all records
            allRecords = GetAllRecords();                   //gets all records

            allRecords[lineNum] = recordToModify;           //recordToModify is the new record that will replace the old one

            ClearFile();        //clears all text from contacts.cnt

            StreamWriter writeFile = new StreamWriter(filePath);

            for (int i = 0; i < allRecords.Length; i++)
            {
                writeFile.WriteLine(allRecords[i]);   //rewrites all the current records and replaces the selected coontact
            }
            writeFile.Close();

            Console.WriteLine("Returning to menu...");
            Console.WriteLine();
            return;
        }

        static void ModifyEmailSelect()
        {
            Console.WriteLine("You have chosen to modify an email");

            Console.WriteLine("Please enter the first name of the contact you would like to modify:");
            string firstName = Console.ReadLine().Trim().ToLower();     //get first

            Console.WriteLine("Please enter the last name of the contact you would like to modify:");
            string lastName = Console.ReadLine().Trim().ToLower();      //get last

            int lineNum = 0;
            string recordToModify = "";
            bool matchFound = false;    //had a bug that would make the while loop repeat after changing a record. this prevented that issue

            StreamReader recordData = new StreamReader(filePath);

            while (recordData.EndOfStream == false && !matchFound)
            {
                string record = recordData.ReadLine();      //current record
                string[] fields = record.Split();           //fields of current record

                if (firstName != fields[0] && lastName != fields[1])
                {
                    lineNum++;                                           //if the current record fields do not match the required values,
                    matchFound = false;                                  //increment the line number, if they do match, then the record to
                }                                                        //modify is on that line number

                if (firstName == fields[0] && lastName == fields[1])
                {
                    matchFound = true;
                    Console.WriteLine("Match found!");
                    Console.WriteLine("Displaying contact...");

                    for (int index = 0; index < fields.Length; index++)
                    {
                        Console.Write(fields[index] + " ");
                    }

                    Console.WriteLine();

                    Console.WriteLine("Input new email for this contact: ");
                    fields[3] = Console.ReadLine();

                    Console.WriteLine();

                    Console.WriteLine("New contact info: ");
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write(fields[i] + " ");
                    }

                    Console.WriteLine();

                    recordToModify = string.Join(" ", fields);
                }//end if
                else
                {
                    matchFound = false;
                }
            }

            recordData.Close();

            if (matchFound)
            {
                ModifyEmail(recordToModify, lineNum);
            }

            if (!matchFound)
            {

                Console.WriteLine("Contact not found. Returning to menu...");
            }
            recordData.Close();

            return;
        }

        static void ModifyEmail(string recordToModify, int lineNum)
        {
            int lineCount = GetRecordCount();
            string[] allRecords = new string[lineCount];    //array that stores all records
            allRecords = GetAllRecords();                   //gets all records

            allRecords[lineNum] = recordToModify;           //recordToModify is the new record that will replace the old one

            ClearFile();        //clears all text from contacts.cnt

            StreamWriter writeFile = new StreamWriter(filePath);

            for (int i = 0; i < allRecords.Length; i++)
            {
                writeFile.WriteLine(allRecords[i]);   //rewrites all the current records and replaces the selected coontact
            }
            writeFile.Close();

            Console.WriteLine("Returning to menu...");
            Console.WriteLine();
            return;
        }

        static void DeleteContactSelect()
        {
            Console.WriteLine("You have chosen to delete a contact");

            Console.WriteLine("Please enter the first name of the contact you would like to delete:");
            string firstName = Console.ReadLine().Trim().ToLower();     //get first

            Console.WriteLine("Please enter the last name of the contact you would like to delete:");
            string lastName = Console.ReadLine().Trim().ToLower();      //get last

            int lineNum = 0;
            string recordToDelete = "";
            bool matchFound = false;    //had a bug that would make the while loop repeat after changing a record. this prevented that issue

            StreamReader recordData = new StreamReader(filePath);

            while (recordData.EndOfStream == false && !matchFound)
            {

                string record = recordData.ReadLine();      //current record
                string[] fields = record.Split();           //fields of current record

                if (firstName != fields[0] && lastName != fields[1])
                {
                    lineNum++;                                           //if the current record fields do not match the required values,
                    matchFound = false;                                  //increment the line number, if they do match, then the record to
                }                                                        //modify is on that line number

                if (firstName == fields[0] && lastName == fields[1])
                {
                    matchFound = true;
                    Console.WriteLine("Match found!");
                    Console.WriteLine("Displaying contact...");

                    for (int index = 0; index < fields.Length; index++)
                    {
                        Console.Write(fields[index] + " ");
                    }

                    recordToDelete = string.Join(" ", fields);
                }//end if
            }

            if (recordData.EndOfStream && !matchFound)
            {
                Console.WriteLine("Contact not found. Returning to menu...");
                return;
            }

            recordData.Close();

            if (matchFound)
            {
                Console.WriteLine("Are you sure you want to delete this contact? Y/N");
                bool deleteContact = PromptYes("");

                if (deleteContact)
                {
                    Console.WriteLine("Deleting contact...");
                    DeleteContact(recordToDelete);
                }
                else
                {
                    Console.WriteLine("Returning to menu...");
                    return;
                }
            }
        }

        static void DeleteContact(string strLineToDelete)
        {
            string strFilePath = filePath;
            string strSearchText = strLineToDelete;
            string strOldText;
            string n = "";
            StreamReader sr = File.OpenText(strFilePath);
            while ((strOldText = sr.ReadLine()) != null)
            {
                if (!strOldText.Contains(strSearchText))
                {
                    n += strOldText + Environment.NewLine;
                }
            }
            sr.Close();
            File.WriteAllText(strFilePath, n);
        }

        static void ClearFile()
        {
            string path = filePath;
            var myFile = File.Create(path);
            myFile.Close();
        }

        static void ListContacts()
        {
            string fileName = filePath;
            StreamReader recordData = new StreamReader(fileName);

            Console.WriteLine("Listing contacts...");

            while (recordData.EndOfStream == false)
            {
                string record = recordData.ReadLine();
                string[] fields = record.Split();

                for (int index = 0; index < fields.Length; index++)
                {
                    Console.Write(fields[index] + " ");
                }//end for
                Console.WriteLine();
            }
            recordData.Close();

            Console.WriteLine("Returning to menu...");
            Console.WriteLine();
            return;
        }

        static int GetSelection()
        {
            int userSelection = 0;  //user's selection stored as int
            bool selectionValid;// bool to make sure selections are valid

            Console.WriteLine("0. Add a new record");
            Console.WriteLine("1. Search for a name");
            Console.WriteLine("2. Modify a phone number");
            Console.WriteLine("3. Modify an email");
            Console.WriteLine("4. Delete a record");
            Console.WriteLine("5. List all contacts");
            Console.WriteLine("6. Exit the program");

            userSelection = PromptInt("Please make a selection");//promptint only allows ints
            selectionValid = ValidateSelection(userSelection);    //validateselection

            while (!selectionValid)//validation loop
            {
                userSelection = PromptInt("Invalid Selection");
                selectionValid = ValidateSelection(userSelection);
            }//end while

            return userSelection;
        }

        static bool ValidateSelection(int userSelection)
        {
            if (userSelection == 0 || userSelection == 1 || userSelection == 2 || userSelection == 3 || userSelection == 4 || userSelection == 5 || userSelection == 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region promptsnshi
        static string Prompt(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
        }//input validation
       
        static int PromptInt(string message)
        {
            int parsedVal = 0;

            while (int.TryParse(Prompt(message), out parsedVal) == false)
            {
                Console.Write("");
            }
            return parsedVal;
        }//input validation

        static bool PromptYes(string message)
        {
            bool validInput = false;
            string input;

            do
            {
                input = Console.ReadKey(true).KeyChar.ToString().ToUpper();

            if (input == "Y")
            {
                return true;
            }
            else if (input == "N")
            {
                return false;
            }

            } while (!validInput);
            return false;
        }
        static double PromptDouble(string message)
        {
            double parsedVal = 0.0;

            while (double.TryParse(Prompt(message), out parsedVal) == false)
            {
                Console.WriteLine("invalid value");
            }
            return parsedVal;

        }//input validation
        #endregion
    }
}
