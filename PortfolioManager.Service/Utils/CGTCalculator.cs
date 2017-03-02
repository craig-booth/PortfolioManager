using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Utils
{  

    public class ParcelSold
    {
        public int UnitsSold { get; private set; }
        public ShareParcel Parcel { get; private set; }
        public decimal AmountReceived { get; internal set; }

        public ParcelSold(int unitsSold, ShareParcel parcel)
        {
            UnitsSold = unitsSold;
            Parcel = parcel;
        }
    }

    public class CGTCalculation
    {
        private List<ParcelSold> _ParcelsSold;

        public DateTime DisposalDate { get; private set; }
        public int UnitsSold { get; private set; }
        public decimal AmountReceived { get; private set; }
        public decimal CapitalGain { get; private set; }
        public CGTCalculationMethod MethodUsed { get; private set; }
        public IReadOnlyCollection<ParcelSold> ParcelsSold 
        { 
            get
            {
                return _ParcelsSold;
            }
        }

        public CGTCalculation(DateTime disposalDate, decimal amountReceived, List<ParcelSold> parcelsSold, CGTCalculationMethod method)
        {
            DisposalDate = disposalDate;
            AmountReceived = amountReceived;
            MethodUsed = method;
            _ParcelsSold = new List<ParcelSold>(parcelsSold);

            /* Apportion amount received over each parcel */             
            ApportionedCurrencyValue[] apportionedAmountReceived = new ApportionedCurrencyValue[_ParcelsSold.Count];
            int i = 0;
            foreach (ParcelSold parcelSold in _ParcelsSold)
                apportionedAmountReceived[i++].Units = parcelSold.UnitsSold; ;
            MathUtils.ApportionAmount(amountReceived, apportionedAmountReceived);


            /* Calculate units sold and capital gain */
            decimal totalCostBase = 0.00M;
            i = 0;
            foreach (ParcelSold parcelSold in _ParcelsSold)
            {
                parcelSold.AmountReceived = apportionedAmountReceived[i++].Amount;

                UnitsSold += parcelSold.UnitsSold;
                if (parcelSold.UnitsSold == parcelSold.Parcel.Units)
                    totalCostBase += parcelSold.Parcel.CostBase;
                else
                    totalCostBase += parcelSold.Parcel.CostBase * ((decimal)parcelSold.UnitsSold / parcelSold.Parcel.Units);
            }

            CapitalGain = amountReceived - totalCostBase;          
        }

    }

    public static class CGTCalculator
    {
        private class CGTComparer : Comparer<ShareParcel>
        {
            public DateTime DisposalDate { get; private set; }
            public CGTCalculationMethod Method { get; private set; }

            public override int Compare(ShareParcel a, ShareParcel b)
            {
                if (Method == CGTCalculationMethod.FirstInFirstOut)
                    return a.AquisitionDate.CompareTo(b.AquisitionDate);
                else if (Method == CGTCalculationMethod.LastInFirstOut)
                    return b.AquisitionDate.CompareTo(a.AquisitionDate);
                else
                {
                    var discountAppliesA = (CGTMethodForParcel(a, DisposalDate) == CGTMethod.Discount);
                    var discountAppliesB = (CGTMethodForParcel(b, DisposalDate) == CGTMethod.Discount);

                    if (discountAppliesA && !discountAppliesB)
                        return -1;
                    else if (discountAppliesB && !discountAppliesA)
                        return 1;
                    else
                    {
                        decimal unitCostBaseA = a.CostBase / a.Units;
                        decimal unitCostBaseB = b.CostBase / b.Units;


                        if (Method == CGTCalculationMethod.MaximizeGain)
                        {
                            if (unitCostBaseA > unitCostBaseB)
                                return 1;
                            else if (unitCostBaseA < unitCostBaseB)
                                return -1;
                            else
                                return 0;
                        }
                        else
                        {
                            if (unitCostBaseA > unitCostBaseB)
                                return -1;
                            else if (unitCostBaseA < unitCostBaseB)
                                return 1;
                            else
                                return 0;
                        }
                    }
                }

            }

            public CGTComparer(DateTime disposalDate, CGTCalculationMethod method)
            {
                DisposalDate = disposalDate;
                Method = method;
            }
        }

        public static DateTime IndexationendDate = new DateTime(1999, 09, 21);

        public static CGTMethod CGTMethodForParcel(ShareParcel parcel, DateTime eventDate)
        {
            if (parcel.AquisitionDate < IndexationendDate)
                return CGTMethod.Indexation;
            else if ((eventDate - parcel.AquisitionDate).Days > 365)
                return CGTMethod.Discount;
            else
                return CGTMethod.Other;
        }

        public static decimal CGTDiscount(decimal cgtAmount)
        {
            if (cgtAmount > 0)
                return 0.50m * cgtAmount;
            else
                return 0.00m; 
        }

        public static CGTCalculation CalculateCapitalGain(IReadOnlyCollection<ShareParcel> parcelsOwned, DateTime saleDate, int unitsToSell, decimal amountReceived, CGTCalculationMethod method)
        {
            /* Sort in prefered sell order */
            var sortedParcels = parcelsOwned.Where(x => x.ToDate == DateUtils.NoEndDate).OrderBy(x => x, new CGTComparer(saleDate, method));

            /* Create list of parcels sold */
            var parcelsSold = new List<ParcelSold>();
            foreach (ShareParcel parcel in sortedParcels)
            {
                int units;
                if (unitsToSell < parcel.Units)
                    units = unitsToSell;
                else
                    units = parcel.Units;

                parcelsSold.Add(new ParcelSold(units, parcel));
                unitsToSell -= units;
                if (unitsToSell == 0)
                    break;
            }


            return new CGTCalculation(saleDate, amountReceived, parcelsSold, method);
        }

    }


}
