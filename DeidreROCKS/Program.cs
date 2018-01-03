using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    class Program
    {
        static IStateAggregate stateAggregate = null;
        static ICustomerAggregate customerAggregate = null;
        static IPurchaseAggregate purchaseAggregate = null;
        static string customerFileName = null;
        static string purchaseFileName = null;
        static void Main(string[] args)
        {
            customerFileName = getFilePath() + "Data\\CustomerInvoicingData.csv";
            purchaseFileName = getFilePath() + "Data\\SmallGroup-SalesData.csv";

            stateAggregate = new StateAggregate(StateAggregate.GetStates());
            purchaseAggregate = new PurchaseAggregate(PurchaseAggregate.GetPurchases(purchaseFileName, stateAggregate));
            customerAggregate = new CustomerAggregate(CustomerAggregate.GetCustomers(customerFileName, purchaseAggregate, stateAggregate));

            displayMenu();
        }

        private static void displayMenu()
        {
            Console.Clear();
            Console.WriteLine("\n***********************************************************************");
            Console.WriteLine("                 PURCHASE REPORT and INVOICING APPLICATION               ");
            Console.WriteLine("                                MENU                                     ");
            Console.WriteLine("***********************************************************************\n\n");
            Console.WriteLine("\t\t\tPlease Pick an Option...\n");
            Console.WriteLine("\t   1 = Summary of Purchases Report (by State)");
            Console.WriteLine("\t   2 = Tax Summary Report (by State) ");
            Console.WriteLine("\t   3 = Display All Purchases Report");
            Console.WriteLine("\t   4 = Show All Customer Names");
            Console.WriteLine("\t   5 = Summary of Purchases Report (by Customer)");
            Console.WriteLine("\t   6 = Add New Purchase Record");
            Console.WriteLine("\t   7 = Invoicing");
            Console.WriteLine("\t   8 = Exit Program\n");

            //Read Users Answer (should be 1 thru 8)
            string userAnswer = Console.ReadLine();
            if (userAnswer == "1") { displayPurchaseSummaryByState(stateAggregate, customerAggregate); }
            else if (userAnswer == "2") { displayTaxSummary(stateAggregate, customerAggregate); }
            else if (userAnswer == "3") { displayAllPurchases(customerAggregate); }
            else if (userAnswer == "4") { displayCustomers(customerAggregate); }
            else if (userAnswer == "5") { displayPurchaseSummaryByCustomer(customerAggregate); }
            else if (userAnswer == "6") { displayAddNewPurchase(customerAggregate, purchaseAggregate); }
            else if (userAnswer == "7") { displayCreateInvoice(customerAggregate); }
            else if (userAnswer == "8") { Environment.Exit(0); }
            else displayMenu();
        }
        private static void displayPurchaseSummaryByState(IStateAggregate statesAggregate, ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n.................... PURCHASE TOTAL SUMMARY (By State) ....................\n");

            foreach (var state in statesAggregate.States)
            {
                var sumForState = customerAggregate.SumTotalPurchaseByState(state.StateCode);
                Console.WriteLine($"Total {state.StateName} Purchases: {sumForState.ToString("C")}");
            }

            returnToMainMenu();
            return;
        }
        private static void displayTaxSummary(IStateAggregate statesAggregate, ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n................. TAX SUMMARY REPORT BY STATE ..................\n");

            foreach (var state in statesAggregate.States)
            {
                var sumForState = customerAggregate.SumTotalTaxByState(state.StateCode);
                Console.WriteLine($"Total {state.StateName} State Tax Owed: {sumForState.ToString("C")}");
            }

            returnToMainMenu();
            return;
        }
        private static void displayAllPurchases(ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n................ PURCHASE SUMMARY REPORT (All Purchases) ...................\n");

            foreach (var customer in customerAggregate.Customers)
            {
                if(customer.Purchases != null && customer.Purchases.Any())
                {
                    foreach (var purchase in customer.Purchases)
                    {
                        Console.WriteLine($"Customer ID: {customer.Id} " +
                            $"\tState: {customer.State.StateCode} " +
                            $"\tTax Rate: {customer.State.TaxRate}" +
                            $"\tSubtotal: {purchase.Subtotal.ToString("C")} " +
                            $"\tTax Amount: {purchase.Tax.ToString("C")}" +
                            $"\tTotal Purchase Amount: {purchase.Total.ToString("C")}");
                    }
                    Console.WriteLine();
                }
            }

            returnToMainMenu();
            return;
        }
        private static void displayCustomers(ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n.................... CUSTOMER NAMES ON RECORD .......................\n");

            foreach (var customer in customerAggregate.Customers)
            {
                Console.WriteLine($"Customer Name: {(customer.Name ?? string.Empty).Trim()}\tCustomer ID: {customer.Id}");
            }

            returnToMainMenu();
            return;
        }
        private static void displayPurchaseSummaryByCustomer(ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n................... PURCHASE TOTAL SUMMARY (By Customer) ....................\n");

            foreach (var customer in customerAggregate.Customers)
            {
                Console.WriteLine($"Customer ID: {customer.Id}");
                Console.WriteLine($"Customer Name: {customer.Name}");
                Console.WriteLine($"Total for ALL Purchases: {customer.SumOfAllTotals.ToString("C")}");
                Console.WriteLine();
            }

            returnToMainMenu();
            return;
        }
        private static void displayAddNewPurchase(ICustomerAggregate customerAggregate, IPurchaseAggregate purchaseAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n................. ADD A NEW PURCHASE RECORD ....................\n");

            Console.WriteLine("\n\nCurrent Customer's on Record for reference as needed:\n");
            displayCustomerGraph(customerAggregate);

            var customer = collectCustomerFromUser(customerAggregate);
            if(customer == null)
            {
                displayMenu();
                return;
            }

            double? subtotal = collectSubTotalFromUser();
            if(!subtotal.HasValue)
            {
                displayMenu();
                return;
            }

            State purchaseState = collectPurchaseStateFromUser(stateAggregate);
            if(purchaseState == null)
            {
                displayMenu();
                return;
            }

            try
            {
                var purchase = purchaseAggregate.Create(customer.Id, subtotal.Value, purchaseState);
                purchaseAggregate.Save(purchase, customerAggregate, purchaseFileName);
                Console.WriteLine("Purchase Successfully Recorded.\n");
                Console.WriteLine("\n\nPress Enter to return to the menu");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                displayMenu();
            }
            return;
        }
        private static void displayCreateInvoice(ICustomerAggregate customerAggregate)
        {
            Console.Clear();
            Console.WriteLine("\n>>>>>>>>>>>>>>>>>>>>>>>>  INVOICING  <<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine("\n\nAvailable Customers to Invoice:");

            displayCustomerGraph(customerAggregate);

            Console.WriteLine("\n\nPlease Enter The Customer ID of the Customer Invoice to Print : ");
            var customer = collectCustomerFromUser(customerAggregate);
            if (customer == null)
            {
                displayMenu();
                return;
            }

            string content = $"Invoicing R Us \r\n" +
                $"100 N University Drive \r\n" +
                $"Edmond, OK 73034 \r\n" +
                $"Phone: (405) 974-2828 \r\n\r\n\r\n" +
                $"BILL TO: \r\n{customer.Name} \r\n" +
                $"{customer.AddressLine1} \r\n" +
                $"{customer.AddressLine2} \r\n" +
                $"{customer.City}, {customer.State.StateCode} {customer.Zip} \r\n\r\n\r\n\r\n" +
                $"DESCRIPTION\t\t\t\t\t" + "AMOUNT\r\n\r\n" +
                $"Purchases SubTotal:\t\t\t\t{customer.SumOfAllSubTotals.ToString("C")}\r\n" +
                $"Tax Amount: \t\t\t\t\t{customer.SumOfAllTaxAmounts.ToString("C")}\r\n" +
                $"TOTAL:\t\t\t\t\t\t{customer.SumOfAllTotals.ToString("C")}\r\n" +
                $"\r\nThank You For Your Business! ";

            string path = getFilePath() + $"Customer_{customer.Id}_Invoice.txt";
            File.WriteAllText(path, content);

            Console.WriteLine($"The Invoice has been created to file: {path}");

            returnToMainMenu();
            return;
        }
        private static Customer collectCustomerFromUser(ICustomerAggregate customerAggregate)
        {
            bool collecting = true;
            int customerId = 0;
            Customer customer = null;

            do
            {
                Console.WriteLine("\n\nPlease enter the Customer ID for this purchase (press Escape to cancel): ");

                ConsoleKeyInfo cki;
                string userInput = string.Empty;
                do
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Escape)
                        return null;
                    userInput += cki.KeyChar;
                } while (cki.Key != ConsoleKey.Enter);

                if (int.TryParse(userInput, out customerId))
                {
                    customer = customerAggregate.GetCustomer(customerId);
                    if (customer != null)
                        collecting = false;
                }
                
                //if we're still collection a customer id at this point, either we entered jibberish or an invalid customer id
                if (collecting)
                {
                    Console.WriteLine("Error - This is not a valid customer ID.\nPlease try again.");
                }
            } while (collecting);
            return customer;
        }
        private static double? collectSubTotalFromUser()
        {
            bool collecting = true;
            double subtotal = 0;

            do
            {
                Console.WriteLine("\nPlease enter the SubTotal for this purchase (press Escape to cancel): ");

                ConsoleKeyInfo cki;
                string userInput = string.Empty;
                do
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Escape)
                        return null;
                    userInput += cki.KeyChar;
                } while (cki.Key != ConsoleKey.Enter);
                
                if(double.TryParse(userInput, out subtotal))
                {
                    collecting = false;
                }
                else
                {
                    Console.WriteLine("Error - This is not a valid value.\nPlease try again.");
                }
            } while (collecting);
            return subtotal;
        }
        private static State collectPurchaseStateFromUser(IStateAggregate stateAggregate)
        {
            bool collecting = true;
            State state = null;

            do
            {
                Console.WriteLine("\nPlease Enter a State Code for This Purchase (press Escape to cancel): ");

                ConsoleKeyInfo cki;
                string userInput = string.Empty;
                do
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Escape)
                        return null;
                    userInput += cki.KeyChar;
                } while (cki.Key != ConsoleKey.Enter);

                state = stateAggregate.GetState(userInput);
                if (state != null)
                    collecting = false;

                if (collecting)
                {
                    Console.WriteLine("Error - This is not a valid State Code.\nPlease try again.");
                }
            } while (collecting);
            return state;
        }
        private static string getFilePath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }
        private static void displayCustomerGraph(ICustomerAggregate customerAggregate)
        {
            foreach (var displayCustomer in customerAggregate.Customers)
            {
                Console.WriteLine($"Name: {(displayCustomer.Name ?? string.Empty).Trim()}" +
                    $"\tCustomer ID: {displayCustomer.Id}" +
                    $"\tLocation on Record: {displayCustomer.State.StateCode}");
            }
            return;
        }
        private static void returnToMainMenu()
        {
            Console.WriteLine("\n\nPress Enter to return to the menu");
            Console.ReadLine();
            displayMenu();
            return;
        }
    }
}