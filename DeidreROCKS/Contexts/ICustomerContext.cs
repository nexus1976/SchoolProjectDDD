using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public interface ICustomerContext
    {
        double SumTotalPurchaseByState(string stateCode);
        double SumTotalTaxByState(string stateCode);
        void MakePurchase(Purchase purchase);
    }
}
