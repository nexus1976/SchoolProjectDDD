using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public interface ICustomerAggregate
    {
        Customer GetCustomer(int customerId);
        double SumTotalPurchaseByState(string stateCode);
        double SumTotalTaxByState(string stateCode);
        IEnumerable<Customer> Customers { get; }
    }
}
