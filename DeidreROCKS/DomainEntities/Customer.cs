using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace DeidreROCKS
{
    public class Customer
    {
        public int Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        [StringLength(50)]
        public string AddressLine1 { get; set; }
        [StringLength(50)]
        public string AddressLine2 { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        public State State { get; set; }
        [StringLength(5)]
        public string Zip { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public double CreditLimit { get; set; }
        public IList<Purchase> Purchases { get; private set; }
        public double SumOfAllTotals
        {
            get
            {
                if (this.Purchases != null && this.Purchases.Any())
                    return this.Purchases.Sum(d => d.Total);
                else
                    return 0;
            }
        }
        public double SumOfAllSubTotals
        {
            get
            {
                if (this.Purchases != null && this.Purchases.Any())
                    return this.Purchases.Sum(d => d.Subtotal);
                else
                    return 0;
            }
        }
        public double SumOfAllTaxAmounts
        {
            get
            {
                if (this.Purchases != null && this.Purchases.Any())
                    return this.Purchases.Sum(d => d.Tax);
                else
                    return 0;
            }
        }
        public bool CanMakePurchase(double newPurchaseTotal)
        {
            return (this.SumOfAllTotals + newPurchaseTotal) <= this.CreditLimit;
        }
        public bool MakePurchase(Purchase purchase)
        {
            if (purchase == null) return false;
            if (this.CanMakePurchase(purchase.Total))
            {
                this.Purchases.Add(purchase);
                return true;
            }
            else
                return false;
        }

        public Customer(IList<Purchase> purchases = null)
        {
            if (purchases == null)
                this.Purchases = new List<Purchase>();
            else
                this.Purchases = purchases;
        }
    }
}
