﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Service.Interface;

using PortfolioManager.Service.Obsolete;

namespace PortfolioManager.UI.ViewModels.Transactions
{
    class ReturnOfCapitalViewModel : TransactionViewModel
    {
        private decimal _Amount;
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;

                ClearErrors();

                if (_Amount < 0.00m)
                    AddError("Amount must not be less than 0");
            }
        }

        private DateTime _PaymentDate;
        public DateTime PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                _PaymentDate = value;

                ClearErrors();

                if (_PaymentDate < RecordDate)
                    AddError("Payment Date must be after the Record date");
            }
        }

        public bool CreateCashTransaction { get; set; }

        public ReturnOfCapitalViewModel(ReturnOfCapitalTransactionItem returnOfCapital, IStockService stockService, IHoldingService holdingService)
            : base(returnOfCapital, TransactionStockSelection.NonStapledStocks(true), stockService, holdingService)
        {

        }

        protected override void CopyTransactionToFields()
        {
            base.CopyTransactionToFields();

            if (Transaction != null)
            {
                PaymentDate = ((ReturnOfCapitalTransactionItem)Transaction).TransactionDate;
                Amount = ((ReturnOfCapitalTransactionItem)Transaction).Amount;
                CreateCashTransaction = ((ReturnOfCapitalTransactionItem)Transaction).CreateCashTransaction;
            }
            else
            {
                PaymentDate = RecordDate;
                Amount = 0.00m;
                CreateCashTransaction = true;
            }
        }

        protected override void CopyFieldsToTransaction()
        {
            if (Transaction == null)
                Transaction = new ReturnOfCapitalTransactionItem();

            base.CopyFieldsToTransaction();

            var returnOfCapital = (ReturnOfCapitalTransactionItem)Transaction;
            returnOfCapital.TransactionDate = PaymentDate;
            returnOfCapital.Amount = Amount;
            returnOfCapital.CreateCashTransaction = CreateCashTransaction;
        }


    }
}
