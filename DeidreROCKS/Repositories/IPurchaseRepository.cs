using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public interface IPurchaseRepository
    {
        Purchase Create(int customerId, double subtotal, State purchaseState);
        string PurchaseFileName { get; }
        void Add(Purchase purchase);
        IList<Purchase> GetPurchasesByCustomerId(int customerId);
    }
}
