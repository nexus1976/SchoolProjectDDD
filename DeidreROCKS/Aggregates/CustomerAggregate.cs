using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public class CustomerAggregate : ICustomerAggregate
    {
        IDictionary<int, Customer> _customers = null;
        public CustomerAggregate(IList<Customer> customers)
        {
            this._customers = new Dictionary<int, Customer>();
            if (customers != null)
            {
                foreach (var customer in customers)
                {
                    this._customers.Add(customer.Id, customer);
                }
            }
        }

        public Customer GetCustomer(int customerId)
        {
            if (customerId <= 0 || this._customers == null || !this._customers.Any())
                return null;
            Customer returnValue = null;
            if (this._customers.TryGetValue(customerId, out returnValue))
                return returnValue;
            else
                return null;
        }
        public double SumTotalPurchaseByState(string stateCode)
        {
            var customers = this._customers.Values.ToList();
            var sumForState = customers.Select(c => c.Purchases.Where(p => p.PurchaseState.StateCode == stateCode).Select(p => p.Total).Sum()).Sum();
            return sumForState;
        }
        public double SumTotalTaxByState(string stateCode)
        {
            var customers = this._customers.Values.ToList();
            var sumForState = customers.Select(c => c.Purchases.Where(p => p.PurchaseState.StateCode == stateCode).Select(p => p.Tax).Sum()).Sum();
            return sumForState;
        }
        public IEnumerable<Customer> Customers
        {
            get
            {
                if (this._customers == null) yield return null;
                foreach (var item in this._customers)
                {
                    yield return item.Value;
                }
            }
        }

        public static IList<Customer> GetCustomers(string customerFile, IPurchaseAggregate purchaseAggregate, IStateAggregate stateAggregate)
        {
            var customers = new List<Customer>();
            if (!File.Exists(customerFile))
                return customers;

            string line = null;

            using (StreamReader file = new StreamReader(customerFile))
            {
                int lineCounter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    if(lineCounter > 0) //skip 1st line of headers
                    {
                        string[] fields = line.Split(',');
                        int customerId = Convert.ToInt32(fields[0]);
                        IList<Purchase> purchases = purchaseAggregate.GetPurchasesByCustomerId(customerId);
                        var customer = new Customer(purchases)
                        {
                            Id = customerId,
                            Name = fields[1],
                            AddressLine1 = fields[2],
                            AddressLine2 = fields[3],
                            City = fields[4],
                            State = stateAggregate.GetState(fields[5]),
                            Zip = fields[6],
                            CreditLimit = Convert.ToDouble(fields[7])
                        };
                        customers.Add(customer);
                    }
                    lineCounter++;
                }
                file.Close();
            }

            return customers;
        }
    }
}
