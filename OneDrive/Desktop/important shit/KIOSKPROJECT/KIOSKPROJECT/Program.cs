using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIOSK
{
    public class CashBox
    {
        public decimal penny;
        public decimal nickel;
        public decimal dime;
        public decimal quarter;
        public decimal halfDollar;
        public decimal coinDollar;
        public decimal one;
        public decimal two;
        public decimal five;
        public decimal ten;
        public decimal twenty;
        public decimal fifty;
        public decimal hundred;
    }

    public class TransactionInfo {

        public int transactionNumber;
        public string transactionDate;
        public string transactionTime;
        public decimal transactionCashPaymentAmt;
        public string transactionCardVendor;
        public decimal transactionCardPaymentAmt;
        public decimal transactionChangeGiven;
    }
    class Program
    {
        static CashBox CashBox;
        static TransactionInfo TransactionInfo;
        static void Main(string[] args)
        {
            CashBox = new CashBox();
            CashBox.penny = 100;
            CashBox.nickel = 100;
            CashBox.dime = 100;
            CashBox.quarter = 100;
            CashBox.halfDollar = 100;
            CashBox.coinDollar = 100;
            CashBox.one = 100;
            CashBox.two = 100;
            CashBox.five = 100;
            CashBox.ten = 100;
            CashBox.twenty = 100;
            CashBox.fifty = 100;
            CashBox.hundred = 100;

            TransactionInfo = new TransactionInfo();


            decimal totalOwed = 0;
            int paymentMethod;
            bool cardNotValid;
            string cardNum;
            bool enoughChange;
            decimal[] changeAndRefundAmt = new decimal[2];
            bool refund = false;
            int transactionNumber = 0;

            while (true)
            {
                Console.WriteLine("Hello. I am ChangeBot!");
                Console.WriteLine("Begin scanning items, and then press enter to select a payment option.");

                totalOwed = ScanItems(totalOwed);       //scan items, get cost, calculate change

                Console.WriteLine($"Total owed: ${totalOwed}");      //display total

                paymentMethod = PromptInt("Press 1 to pay with cash, 2 to pay with a card, or 3 to cancel."); //ask if paying with cash, card or both

                while (totalOwed > 0 && refund == false)
                {
                    if (paymentMethod == 1)     //cash payment
                    {
                        Console.WriteLine("Begin inserting your payment in valid U.S. bill/coin denominations, one at a time.");
                        changeAndRefundAmt = PayWithCash(totalOwed);    //call payment function, get change
                        enoughChange = CheckChange(changeAndRefundAmt[0]); //bool to determine if there is enough change to dispense
                                                                           //if change = 0
                        if (changeAndRefundAmt[0] == 0)//decimal array that holds the amount of change
                        {                           //to dispense and the amount to refund if the user chooses to cancel
                            TransactionInfo.transactionCardPaymentAmt = 0.00m;
                            TransactionInfo.transactionCardVendor = "CASH";
                            TransactionInfo.transactionChangeGiven = changeAndRefundAmt[0] * -1;
                            TransactionInfo.transactionCashPaymentAmt = totalOwed;
                            String date = DateTime.Now.ToString("dd-MM-yyyy");
                            TransactionInfo.transactionDate = date;
                            string time = DateTime.Now.ToString("h:mm:ss");
                            TransactionInfo.transactionTime = time;
                            Console.WriteLine($"Transaction complete.");
                            Console.WriteLine("You are not due any change.");

                            string transactionInfo = TransactionInfo.transactionNumber.ToString() + " " + TransactionInfo.transactionDate.ToString() + " " + TransactionInfo.transactionTime.ToString() + " "
                + TransactionInfo.transactionCashPaymentAmt.ToString() + " " + TransactionInfo.transactionCardVendor.ToString() + " " + TransactionInfo.transactionCardPaymentAmt.ToString() + " " +
                  TransactionInfo.transactionChangeGiven.ToString();

                            Console.WriteLine(transactionInfo);

                            StartLogger(transactionInfo);

                            totalOwed = 0;
                        }

                        //if change must be dispensed
                        if (changeAndRefundAmt[0] < 0 && enoughChange)
                        {
                            Console.WriteLine($"You are due: ${changeAndRefundAmt[0] * -1} in change.");
                            Console.WriteLine("Transaction complete. Dispensing change...");
                            DispenseChange(changeAndRefundAmt[0], enoughChange);

                            TransactionInfo.transactionCardPaymentAmt = 0.00m;
                            TransactionInfo.transactionCardVendor = "CASH";
                            TransactionInfo.transactionChangeGiven = changeAndRefundAmt[0] * -1;
                            String date = DateTime.Now.ToString("dd-MM-yyyy");
                            TransactionInfo.transactionDate = date;
                            TransactionInfo.transactionDate = date;
                            string time = DateTime.Now.ToString("h:mm:ss tt");
                            TransactionInfo.transactionTime = time;
                            TransactionInfo.transactionCashPaymentAmt = totalOwed;

                            totalOwed = 0;

                            
                        }
                        if (enoughChange == false)//if not enough change, change payment option
                        {
                            Console.WriteLine("The machine does not have enough to dispense your change. Pay with exact change, pay with card, or cancel and refund?");
                            Console.WriteLine("1: Pay with exact change");
                            Console.WriteLine("2: Pay with a card");
                            Console.WriteLine("3: Cancel and refund");

                            paymentMethod = PromptInt("Make a selection");
                        }

                        transactionNumber++;

                        TransactionInfo.transactionNumber = transactionNumber;
                    }//end if

                    else if (paymentMethod == 2)
                    {
                        Console.WriteLine("You have chosen to pay with a card.");

                        bool requestedCashBack = false;
                        decimal cashBackAmt = 0;

                        Console.WriteLine("Input your card number without dashes or spaces: ");

                        //test card number 4111111111111111

                        cardNum = Console.ReadLine();  //card number input
                        cardNotValid = ValidateCardNum(cardNum);
                        string[] moneyRequestSummary;

                        if (cardNotValid == false)      //if card is valid
                        {
                            Console.WriteLine("Would you like cashback? Y/N");
                            requestedCashBack = PromptYes("Would you like cashback? Y/N");

                            if (requestedCashBack)
                            {
                                cashBackAmt = 0;
                                cashBackAmt = PromptDecimal("Enter amount of cashback: ");
                                totalOwed = totalOwed + cashBackAmt;
                            }

                            decimal totalOwedMax = totalOwed;
                            moneyRequestSummary = MoneyRequest(cardNum, totalOwed);       //moneyrequest does 2 things: give a transaction a 50/50 chance of being fully paid(which would be a succeded transaction),
                                                                                          //and give a failed transaction a 50/50 chance of failing.

                            if (moneyRequestSummary[1] == "declined")//total fail transaction
                            {
                                Console.WriteLine("Transaction failed.");
                                Console.WriteLine();
                                Console.WriteLine("Select another payment method: ");
                                Console.WriteLine("1: Pay with cash");
                                Console.WriteLine("2: Pay with a card");
                                Console.WriteLine("3: Cancel and refund");

                                totalOwed = totalOwedMax - cashBackAmt;

                                paymentMethod = PromptInt("Make a selection");
                            }

                            if (moneyRequestSummary[1] != "declined")
                            {
                                totalOwed = decimal.Parse(moneyRequestSummary[1]);

                                if (totalOwed == totalOwedMax)
                                {
                                    totalOwed -= totalOwed;
                                }

                                Console.WriteLine($"Card number: {moneyRequestSummary[0]}");
                                Console.WriteLine($"Amount remaining: {totalOwed}");
                                Console.WriteLine();
                                //FULL PASS TRANSACTION
                                if (moneyRequestSummary[1] != "declined" && totalOwed == 0)     //if total is fully paid
                                {
                                    Console.WriteLine("Transaction complete!");                 //complete transaction

                                    TransactionInfo.transactionCardPaymentAmt = totalOwedMax;
                                    TransactionInfo.transactionChangeGiven = 0;
                                    TransactionInfo.transactionCashPaymentAmt = 0.00m;
                                    String date = DateTime.Now.ToString("dd-MM-yyyy");
                                    TransactionInfo.transactionDate = date;
                                    string time = DateTime.Now.ToString("h:mm:ss tt");
                                    TransactionInfo.transactionTime = time;
                                    

                                    enoughChange = CheckChange(cashBackAmt * -1);               //checkchange in case user wants cashback

                                    if (enoughChange && requestedCashBack)
                                    {
                                        //DISPENSE CASHBACK
                                        Console.WriteLine("Dispensing cashback...");
                                        DispenseChange(cashBackAmt * -1, enoughChange);
                                        Console.WriteLine("Transaction complete. Have a good day!");

                                        TransactionInfo.transactionChangeGiven = cashBackAmt;
                                    }

                                    else if (enoughChange == false && requestedCashBack)
                                    {
                                        Console.WriteLine("The machine does not have enough funds to give cashback.");
                                    }

                                    transactionNumber++;

                                    TransactionInfo.transactionNumber = transactionNumber;
                                }//end full pass transaction

                                //if not declined                   not fully paid      and cashback not requested        
                                if (moneyRequestSummary[1] != "declined" && totalOwed != 0 && requestedCashBack == false) //partial transaction success
                                {
                                    Console.WriteLine($"Transaction failed. Your total of {totalOwedMax} has been reduced to {totalOwed}.");

                                    Console.WriteLine();
                                    Console.WriteLine("Select another payment method:");
                                    Console.WriteLine("1: Pay with cash");
                                    Console.WriteLine("2: Pay with a card");
                                    Console.WriteLine("3: Cancel and refund");

                                    paymentMethod = PromptInt("Make a selection");
                                }
                                //if not declined           not fully paid     and cashback requested
                                if (moneyRequestSummary[1] != "declined" && totalOwed != 0 && requestedCashBack)
                                {
                                    totalOwed = totalOwed - cashBackAmt;
                                    Console.WriteLine("Insufficient funds for cashback.");
                                    Console.WriteLine($"Your remaining total is: {totalOwed}");
                                    Console.WriteLine();

                                    Console.WriteLine("Select another payment method:");
                                    Console.WriteLine("1: Pay with cash");
                                    Console.WriteLine("2: Pay with a card");
                                    Console.WriteLine("3: Cancel and refund");

                                    paymentMethod = PromptInt("Make a selection");

                                }
                            }
                        }
                    }

                    else if (paymentMethod == 3)
                    {
                        refund = true;
                        enoughChange = true;
                        Console.WriteLine("Cancelling and refunding... ");
                        DispenseChange(changeAndRefundAmt[1] * -1, enoughChange);
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                        Console.WriteLine("1: Pay with cash");
                        Console.WriteLine("2: Pay with a card");
                        Console.WriteLine("3: Cancel and refund");

                        paymentMethod = PromptInt("Make a selection");
                    }
                }

                //Console.WriteLine(TransactionInfo.transactionNumber);
                //Console.WriteLine(TransactionInfo.transactionDate);
                //Console.WriteLine(TransactionInfo.transactionTime);
                //Console.WriteLine(TransactionInfo.transactionCashPaymentAmt);
                //Console.WriteLine(TransactionInfo.transactionCardVendor);
                //Console.WriteLine(TransactionInfo.transactionCardPaymentAmt);
                //Console.WriteLine(TransactionInfo.transactionChangeGiven);

                

            }
        }//end main

        static void StartLogger(string transactionInfo)
        {


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "C:\\Users\\cockus fattus\\OneDrive\\Desktop\\important shit\\KIOSKPROJECT\\KIOSKPROJECT\\bin\\Debug\\KIOSKTRANSACTIONLOGGER.exe";
            startInfo.Arguments = transactionInfo;
            startInfo.UseShellExecute = false;
            Process.Start(startInfo);
            


        }

        static string[] MoneyRequest(string accountNumber, decimal totalOwed)
        {
            Random rand = new Random();
            //50/50 chance transaction passes or fails
            bool pass = rand.Next(100) < 50;
            //50/50 chance a failed transaction is declined
            bool declined = rand.Next(100) < 50;

            if (pass)
            {
                return new string[] { accountNumber, totalOwed.ToString() };
            }
            else
            {
                if (!declined)
                {
                    return new string[] { accountNumber, (totalOwed / rand.Next(2, 6)).ToString() };
                }
                else
                {
                    return new string[] { accountNumber, "declined" };
                }
            }
        }

        static bool ValidateCardNum(string cardNum)
        {   //get cardnum as array
            int[] cardNumArray = new int[cardNum.Length];
            int cardVendorNum = 0;
            int total = 0;
            for (int i = 0; i < cardNum.Length; i++)
            {
                cardNumArray[i] = (int)(cardNum[i] - '0');
                cardVendorNum = cardNumArray[0];
                //get first number of card number to identify vendor
            }
            //start from right, double each other digit, if greater than 9
            //then mod 10 and +1 to remainder
            for (int i = cardNumArray.Length - 2; i >= 0; i = i - 2)
            {
                int tempVal = cardNumArray[i];
                tempVal = tempVal * 2;
                if (tempVal > 9)
                {
                    tempVal = tempVal % 10 + 1;
                }
                cardNumArray[i] = tempVal;
            }
            //add all digits
            for (int i = 0; i < cardNumArray.Length; i++)
            {
                total += cardNumArray[i];
            }
            if (total % 10 == 0)
            {
                if (cardVendorNum == 3)
                {
                    Console.WriteLine("Vendor: American Express");
                    TransactionInfo.transactionCardVendor = "American Express";
                    return false;//returns false if total/10 is 0
                }
                if (cardVendorNum == 4)
                {
                    Console.WriteLine("Vendor: Visa");
                    TransactionInfo.transactionCardVendor = "Visa";
                    return false;
                }
                if (cardVendorNum == 5)
                {
                    Console.WriteLine("Vendor: Mastercard");
                    TransactionInfo.transactionCardVendor = "Mastercard";
                    return false;
                }
                if (cardVendorNum == 6)
                {
                    Console.WriteLine("Vendor: Discover");
                    TransactionInfo.transactionCardVendor = "Discover";
                    return false;
                }
            }//end if
            return true;
        }//end func

        static void DispenseChange(decimal change, bool enoughChange)
        {
            while (change < 0 && enoughChange == true)
            {
                //while amount of change to dispense is greater
                //than 0 and there is enough change in the machine,
                //the program will run through a greedy algorithm and
                //dispense the correct amount of change
                while ((change % 100.00m) > change && CashBox.hundred > 0)
                {
                    CashBox.hundred -= 1;
                    Console.WriteLine("$100.00 dispensed");
                    change += 100.00m;
                }
                while ((change % 50.00m) > change && CashBox.fifty > 0)
                {
                    CashBox.fifty -= 1;
                    Console.WriteLine("$50.00 dispensed");
                    change += 50.00m;
                }
                while ((change % 20.00m) > change && CashBox.twenty > 0)
                {
                    CashBox.twenty -= 1;
                    Console.WriteLine("$20.00 dispensed");
                    change += 20.00m;
                }
                while ((change % 10.00m) > change && CashBox.ten > 0)
                {
                    CashBox.ten -= 1;
                    Console.WriteLine("$10.00 dispensed");
                    change += 10.00m;
                }
                while ((change % 5.00m) > change && CashBox.five > 0)
                {
                    CashBox.five -= 1;
                    Console.WriteLine("$5.00 dispensed");
                    change += 5.00m;
                }
                while ((change % 2.00m) > change && CashBox.two > 0)
                {
                    CashBox.two -= 1;
                    Console.WriteLine("$2.00 dispensed");
                    change += 2.00m;
                }
                while ((change % 1.00m) > change && CashBox.one > 0)
                {
                    CashBox.one -= 1;
                    Console.WriteLine("$1.00 dispensed");
                    change += 1.00m;
                }
                while ((change % .50m) > change && CashBox.halfDollar > 0)
                {
                    CashBox.halfDollar -= 1;
                    Console.WriteLine(".50 dispensed");
                    change += .50m;
                }
                while ((change % .25m) > change && CashBox.quarter > 0)
                {
                    CashBox.quarter -= 1;
                    Console.WriteLine("$0.25 dispensed");
                    change += .25m;
                }
                while ((change % .10m) > change && CashBox.dime > 0)
                {
                    CashBox.dime -= 1;
                    Console.WriteLine("$0.10 dispensed");
                    change += .10m;
                }
                while ((change % .05m) > change && CashBox.nickel > 0)
                {
                    CashBox.nickel -= 1;
                    Console.WriteLine("$0.05 dispensed");
                    change += .05m;
                }
                while ((change % .01m) > change && CashBox.penny > 0)
                {
                    CashBox.penny -= 1; //subtracts money from the cashbox
                    Console.WriteLine("$0.01 dispensed");   //output message
                    change += .01m;     //updates change
                }
            }//end change while loop
        }//end make change function

        static bool CheckChange(decimal changeCount)
        {
            decimal pennyCount = CashBox.penny;
            decimal nickelCount = CashBox.nickel;
            decimal dimeCount = CashBox.dime;
            decimal quarterCount = CashBox.quarter;
            decimal halfDollarCount = CashBox.halfDollar;
            decimal oneCount = CashBox.one;
            decimal twoCount = CashBox.two;      //change count is a number that is
            decimal fiveCount = CashBox.five;    //equal to the amount of change that needs to be
            decimal tenCount = CashBox.ten;      //it uses a greedy algorithm and copies of the  
            decimal twentyCount = CashBox.twenty;//amounts of physical money in the machine to
            decimal fiftyCount = CashBox.fifty;  //see if there is enough to dispense the change.
            decimal hundredCount = CashBox.hundred;

            while (((changeCount % 100.00m) > changeCount) && hundredCount > 0)
            {
                hundredCount -= 1;
                changeCount += 100.00m;
            }
            while (((changeCount % 50.00m) > changeCount) && fiftyCount > 0)
            {
                fiftyCount -= 1;
                changeCount += 50.00m;
            }
            while (((changeCount % 20.00m) > changeCount) && twentyCount > 0)
            {
                fiftyCount -= 1;
                changeCount += 20.00m;
            }
            while (((changeCount % 10.00m) > changeCount) && tenCount > 0)
            {
                tenCount -= 1;
                changeCount += 10.00m;
            }
            while (((changeCount % 5.00m) > changeCount) && fiveCount > 0)
            {
                fiveCount -= 1;
                changeCount += 5.00m;
            }
            while (((changeCount % 2.00m) > changeCount) && twoCount > 0)
            {
                twoCount -= 1;
                changeCount += 2.00m;
            }
            while (((changeCount % 1.00m) > changeCount) && oneCount > 0)
            {
                oneCount -= 1;
                changeCount += 1.00m;
            }
            while (((changeCount % .50m) > changeCount) && halfDollarCount > 0)
            {
                halfDollarCount -= 1; //while the remainder of the change is greater 
                changeCount += .50m;  //than .50, changeCount will be updated and 
            }
            while (((changeCount % .25m) > changeCount) && quarterCount > 0)
            {
                quarterCount -= 1;
                changeCount += .25m;
            }
            while (((changeCount % .10m) > changeCount) && dimeCount > 0)
            {
                dimeCount -= 1;
                changeCount += .10m;
            }
            while (((changeCount % .05m) > changeCount) && nickelCount > 0)
            {
                nickelCount -= 1;
                changeCount += .05m;
            }
            while (((changeCount % .01m) > changeCount) && pennyCount > 0)
            {
                pennyCount -= 1;
                changeCount += .01m;
            }
            if (changeCount == 0)   //if there is enough change in the machine,
            {                       //then the amount of change left to dispense will
                return true;        //be 0 and the function will return true.
            }                       //otherwise, if there is not enough money in
            else return false;      //the machine to dispense, the changeCount will
                                    //be unable to be 0 and will return false.
        }// end checkchange         

        static decimal[] PayWithCash(decimal totalOwed)      //returns change
        {
            decimal moneyInserted;
            decimal change = 0;
            decimal totalMoneyInserted = 0;
            decimal totalOwedMax = totalOwed;
            bool billIsValid;

            while (totalOwed > 0) //while total owed is greater than 0, run loop
            {
                moneyInserted = PromptDecimal("Insert your payment: "); //prompts for payment
                billIsValid = ValidDenomination(moneyInserted);         //makes sure amount entered
                                                                        //is a valid denomination
                if (moneyInserted > 0 && billIsValid)
                {
                    if (moneyInserted == .01m)//if penny is inserted
                    {
                        CashBox.penny++;      //add a penny to cashbox
                        //add 1 cent to the amount of money inserted
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;
                        //subtract 1 cent from total
                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == .05m)
                    {
                        CashBox.nickel++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == .10m)
                    {
                        CashBox.dime++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == .25m)
                    {
                        CashBox.quarter++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == .50m)
                    {
                        CashBox.halfDollar++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 1.00m)
                    {
                        CashBox.one++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 2.00m)
                    {
                        CashBox.two++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 5.00m)
                    {
                        CashBox.five++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 10.00m)
                    {
                        CashBox.ten++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 20.00m)
                    {
                        CashBox.twenty++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 50.00m)
                    {
                        CashBox.fifty++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    if (moneyInserted == 100.00m)
                    {
                        CashBox.hundred++;
                        totalMoneyInserted = totalMoneyInserted + moneyInserted;

                        totalOwed = totalOwed - moneyInserted;
                    }
                    change = totalOwedMax - totalMoneyInserted; //calculates change to dispense by subtracting the
                                                                //amount of money inserted from the original total
                    if (totalOwed > 0)
                    {
                        Console.WriteLine($"Total remaining: ${totalOwed}");
                    }
                }
            }//end while
            return new decimal[] { change, totalMoneyInserted };
        }//end getpayment

        static decimal ScanItems(decimal totalOwed) //function to scan items, return total
        {
            decimal itemPrice = 0;
            string itemPriceString = "p";       //non empty string to run initiate loop
            bool keepRunning = true;

            while (itemPriceString != "" && keepRunning == true)        //whlie the input is not a blank string, the program will keep running
            {

                Console.WriteLine($"Scan item.");
                Console.Write("$ ");
                itemPriceString = Console.ReadLine();                   //gets price input, then parses below

                if (itemPriceString == "")      //if the item price string is empty, the loop will end and the function will return the total
                {
                    keepRunning = false;
                }//end if
                bool parseSuccess = decimal.TryParse(itemPriceString, out itemPrice);       //tryparse the string to convert to a decimal.
                                                                                            //if parse fails, invalid input error is shown
                if ((itemPrice < 0 || parseSuccess == false) && itemPriceString != "")      //less than zero validation
                {
                    Console.WriteLine("Item price can not be less than 0, or input is invalid.");

                    totalOwed = totalOwed - itemPrice; //prevents negative numbers from affecting total
                    keepRunning = true;
                }//end if

                totalOwed = totalOwed + itemPrice;       //to calculate total

            }//end while
            return totalOwed;
        }//end scanitems function

        static bool ValidDenomination(decimal moneyInserted)
        {   //checks if moneyInserted is equal to a valid denomination
            if (moneyInserted == .01m)
            {
                return true;
            }
            if (moneyInserted == .05m)
            {
                return true;
            }
            if (moneyInserted == .10m)
            {
                return true;
            }
            if (moneyInserted == .25m)
            {
                return true;
            }
            if (moneyInserted == .50m)
            {
                return true;
            }
            if (moneyInserted == 1.00m)
            {
                return true;
            }
            if (moneyInserted == 2.00m)
            {
                return true;
            }
            if (moneyInserted == 5.00m)
            {
                return true;
            }
            if (moneyInserted == 10.00m)
            {
                return true;
            }
            if (moneyInserted == 20.00m)
            {
                return true;
            }
            if (moneyInserted == 50.00m)
            {
                return true;
            }
            if (moneyInserted == 100.00m)
            {
                return true;
            }
            else
            {
                Console.WriteLine("That is not a valid denomination.");
                return false;
            }
        }


        #region prompts
        static string Prompt(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine();
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
        static int PromptInt(string message)
        {
            int parsedVal = 0;

            while (int.TryParse(Prompt(message), out parsedVal) == false)
            {
                Console.WriteLine("invalid value");
            }
            return parsedVal;
        }//input validation

        static double PromptDouble(string message)
        {
            double parsedVal = 0.0;

            while (double.TryParse(Prompt(message), out parsedVal) == false)
            {
                Console.WriteLine("invalid value");
            }
            return parsedVal;

        }//input validation

        static decimal PromptDecimal(string message)
        {
            decimal parsedVal = 0.0m;

            while (decimal.TryParse(Prompt(message), out parsedVal) == false)
            {
                Console.WriteLine("invalid value");
            }
            return parsedVal;


        }//input validation
        #endregion
    }

}//end class    
//end name
