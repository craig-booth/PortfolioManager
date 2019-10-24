﻿using System;
using System.Collections.Generic;
using System.Linq;
using Booth.Common;

using PortfolioManager.Domain.Portfolios;

namespace PortfolioManager.Domain.Utils
{
    public class ParcelSold
    {
        public Parcel Parcel { get; private set; }

        public DateTime DisposalDate { get; private set; }
        public int UnitsSold { get; private set; }   
        public decimal CostBase { get; private set; }
        public decimal AmountReceived { get; private set; }
        public decimal CapitalGain { get; private set; }
        public CGTMethod CgtMethod { get; private set; }

        public ParcelSold(Parcel parcel, int unitsSold, DateTime disposalDate)
        {
            Parcel = parcel;
            UnitsSold = unitsSold;
            DisposalDate = disposalDate;   
        }

        public void CalculateCapitalGain(decimal amountReceived)
        {
            AmountReceived = amountReceived;
            CgtMethod = CgtCalculator.CgtMethodForParcel(Parcel.AquisitionDate, DisposalDate);

            var properties = Parcel.Properties[DisposalDate];
            if (UnitsSold == properties.Units)
                CostBase = properties.CostBase;                
            else
                CostBase = (properties.CostBase * ((decimal)UnitsSold / properties.Units)).ToCurrency(RoundingRule.Round);

            CapitalGain = amountReceived - CostBase;
        }
    }

    public class CgtCalculation
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

        public CgtCalculation(DateTime disposalDate, decimal amountReceived, List<ParcelSold> parcelsSold, CGTCalculationMethod method)
        {
            DisposalDate = disposalDate;
            AmountReceived = amountReceived;
            MethodUsed = method;
            _ParcelsSold = new List<ParcelSold>(parcelsSold);

            // Apportion amount received over each parcel 
            ApportionedCurrencyValue[] apportionedAmountReceived = new ApportionedCurrencyValue[_ParcelsSold.Count];
            int i = 0;
            foreach (ParcelSold parcelSold in _ParcelsSold)
                apportionedAmountReceived[i++].Units = parcelSold.UnitsSold; ;
            MathUtils.ApportionAmount(amountReceived, apportionedAmountReceived);


            // Calculate units sold and capital gain
            i = 0;
            foreach (ParcelSold parcelSold in _ParcelsSold)
            {
                parcelSold.CalculateCapitalGain(apportionedAmountReceived[i++].Amount);

                UnitsSold += parcelSold.UnitsSold;
                CapitalGain += parcelSold.CapitalGain;
            }

            
        }

    }

    public static class CgtCalculator
    {
        private class CgtComparer : Comparer<Parcel>
        {
            public DateTime DisposalDate { get; private set; }
            public CGTCalculationMethod Method { get; private set; }

            public override int Compare(Parcel a, Parcel b)
            {
                if (Method == CGTCalculationMethod.FirstInFirstOut)
                    return a.AquisitionDate.CompareTo(b.AquisitionDate);
                else if (Method == CGTCalculationMethod.LastInFirstOut)
                    return b.AquisitionDate.CompareTo(a.AquisitionDate);
                else
                {
                    var discountAppliesA = (CgtMethodForParcel(a.AquisitionDate, DisposalDate) == CGTMethod.Discount);
                    var discountAppliesB = (CgtMethodForParcel(b.AquisitionDate, DisposalDate) == CGTMethod.Discount);

                    if (discountAppliesA && !discountAppliesB)
                        return -1;
                    else if (discountAppliesB && !discountAppliesA)
                        return 1;
                    else
                    {
                        decimal unitCostBaseA = a.Properties[DisposalDate].CostBase / a.Properties[DisposalDate].Units;
                        decimal unitCostBaseB = b.Properties[DisposalDate].CostBase / b.Properties[DisposalDate].Units;


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

            public CgtComparer(DateTime disposalDate, CGTCalculationMethod method)
            {
                DisposalDate = disposalDate;
                Method = method;
            }
        }

        public static DateTime IndexationEndDate = new DateTime(1999, 09, 21);

        public static CGTMethod CgtMethodForParcel(DateTime aquisitionDate, DateTime eventDate)
        {
            if (aquisitionDate < IndexationEndDate)
                return CGTMethod.Indexation;
            else if ((eventDate - aquisitionDate).Days > 365)
                return CGTMethod.Discount;
            else
                return CGTMethod.Other;
        }

        public static decimal CgtDiscount(decimal cgtAmount)
        {
            if (cgtAmount > 0)
                return 0.50m * cgtAmount;
            else
                return 0.00m;
        }

        public static CgtCalculation CalculateCapitalGain(IEnumerable<Parcel> parcelsOwned, DateTime saleDate, int unitsToSell, decimal amountReceived, CGTCalculationMethod method)
        {
            // Sort in prefered sell order
            var sortedParcels = parcelsOwned.Where(x => x.EffectivePeriod.ToDate == DateUtils.NoEndDate).OrderBy(x => x, new CgtComparer(saleDate, method));

            /* Create list of parcels sold */
            var parcelsSold = new List<ParcelSold>();
            foreach (var parcel in sortedParcels)
            {
                var parcelProperties = parcel.Properties[saleDate];
                var units = Math.Min(parcelProperties.Units, unitsToSell);

                parcelsSold.Add(new ParcelSold(parcel, units, saleDate));
                unitsToSell -= units;
                if (unitsToSell == 0)
                    break;
            }


            return new CgtCalculation(saleDate, amountReceived, parcelsSold, method);
        }

    }


}
