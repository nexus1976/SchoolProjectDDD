using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public interface IPurchaseAggregate
    {
        Purchase Create(int customerId, double subtotal, State purchaseState);
        void Save(Purchase purchase, ICustomerAggregate customerAggregate, string purchaseFileName);
        IList<Purchase> GetPurchasesByCustomerId(int customerId);
    }
}
