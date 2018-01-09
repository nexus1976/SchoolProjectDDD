using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private HashSet<Purchase> _purchases = null;
        private string _purchaseFileName = null;
        public PurchaseRepository(IList<Purchase> purchases, string purchaseFileName)
        {
            this._purchases = new HashSet<Purchase>(purchases ?? new List<Purchase>());
            this._purchaseFileName = purchaseFileName;
        }

        public Purchase Create(int customerId, double subtotal, State purchaseState)
        {
            return _create(customerId, subtotal, purchaseState);
        }
        public string PurchaseFileName
        {
            get { return this._purchaseFileName; }
        }
        public void Add(Purchase purchase)
        {
            if (purchase != null)
            {
                string record = $"{purchase.CustomerId},{purchase.Subtotal},{purchase.PurchaseState.StateCode}";
                using (StreamWriter writer = File.AppendText(this._purchaseFileName))
                {
                    writer.WriteLine(record);
                }
                this._purchases.Add(purchase);
            }
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
        public static IList<Purchase> GetPurchases(string purchasesFile, IStateRepository stateAggregate)
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
