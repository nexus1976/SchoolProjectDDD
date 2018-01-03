using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public class PurchaseAggregate : IPurchaseAggregate
    {
        private HashSet<Purchase> _purchases = null;
        public PurchaseAggregate(IList<Purchase> purchases)
        {
            this._purchases = new HashSet<Purchase>(purchases ?? new List<Purchase>());
        }

        public Purchase Create(int customerId, double subtotal, State purchaseState)
        {
            return _create(customerId, subtotal, purchaseState);
        }
        public void Save(Purchase purchase, ICustomerAggregate customerAggregate, string purchaseFileName)
        {
            if (!File.Exists(purchaseFileName))
                throw new ArgumentException($"File {purchaseFileName} could not be found. Save Failed.");
            var customer = customerAggregate.GetCustomer(purchase.CustomerId);
            if (customer == null)
                throw new ArgumentException($"CustomerId {purchase.CustomerId} was not found on the aggregate. Save Failed.");
            if (!customer.CanMakePurchase(purchase.Total))
                throw new Exception("*******ERROR: INSUFFICENT CREDIT AVAILABLE. PURCHASE NOT RECORDED.*******");

            if (!customer.MakePurchase(purchase))
                throw new Exception("Unknown Error. Save Failed.");
            this._purchases.Add(purchase);

            string record = $"{customer.Id},{purchase.Subtotal},{purchase.PurchaseState.StateCode}";
            using (StreamWriter writer = File.AppendText(purchaseFileName))
            {
                writer.WriteLine(record);
            }
            return;
        }
        public IList<Purchase> GetPurchasesByCustomerId(int customerId)
        {
            return this._purchases.Where(d => d.CustomerId == customerId).ToList();
        }

        private static Purchase _create(int customerId, double subtotal, State purchaseState)
        {
            Purchase purchase = new Purchase()
            {
                CustomerId = customerId,
                PurchaseState = purchaseState,
                Subtotal = subtotal,
                Tax = purchaseState.TaxRate * subtotal,
                Total = (purchaseState.TaxRate * subtotal) + subtotal
            };
            return purchase;
        }
        public static IList<Purchase> GetPurchases(string purchasesFile, IStateAggregate stateAggregate)
        {
            List<Purchase> purchases = new List<Purchase>();
            if (!File.Exists(purchasesFile) || stateAggregate == null)
                return purchases;

            string line = null;
            using (StreamReader file = new StreamReader(purchasesFile))
            {
                int lineCounter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if (lineCounter > 0) //skip 1st line of headers
                    {
                        string[] fields = line.Split(',');
                        int customerId = Convert.ToInt32(fields[0]);
                        double subtotal = Convert.ToDouble(fields[1]);
                        string state = fields[2];

                        var purchaseState = stateAggregate.GetState(state);
                        if(purchaseState != null)
                        {
                            var purchase = _create(customerId, subtotal, purchaseState);
                            purchases.Add(purchase);
                        }
                    }
                    lineCounter++;
                }
            }
            return purchases;
        }
    }
}
