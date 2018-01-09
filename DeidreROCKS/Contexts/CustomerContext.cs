using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeidreROCKS
{
    public class CustomerContext: ICustomerContext
    {
        private ICustomerRepository _customerRepository = null;
        private IPurchaseRepository _purchaseRepository = null;
        public CustomerContext(ICustomerRepository customerRepository, IPurchaseRepository purchaseRepository)
        {
            this._customerRepository = customerRepository;
            this._purchaseRepository = purchaseRepository;
        }
        public double SumTotalPurchaseByState(string stateCode)
        {
            var sumForState = this._customerRepository.Customers.Select(c => c.Purchases.Where(p => p.PurchaseState.StateCode == stateCode).Select(p => p.Total).Sum()).Sum();
            return sumForState;
        }
        public double SumTotalTaxByState(string stateCode)
        {
            var sumForState = this._customerRepository.Customers.Select(c => c.Purchases.Where(p => p.PurchaseState.StateCode == stateCode).Select(p => p.Tax).Sum()).Sum();
            return sumForState;
        }
        public void MakePurchase(Purchase purchase)
        {
            if (!File.Exists(this._purchaseRepository.PurchaseFileName))
                throw new ArgumentException($"File {this._purchaseRepository.PurchaseFileName} could not be found. Save Failed.");
            var customer = this._customerRepository.GetCustomer(purchase.CustomerId);
            if (customer == null)
                throw new ArgumentException($"CustomerId {purchase.CustomerId} was not found on the aggregate. Save Failed.");
            if (!customer.CanMakePurchase(purchase.Total))
                throw new Exception("*******ERROR: INSUFFICENT CREDIT AVAILABLE. PURCHASE NOT RECORDED.*******");
            if (!customer.MakePurchase(purchase))
                throw new Exception("Unknown Error. Save Failed.");
            else
                this._purchaseRepository.Add(purchase);
            return;
        }
    }
}
