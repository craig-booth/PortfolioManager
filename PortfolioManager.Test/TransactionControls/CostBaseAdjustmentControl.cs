﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class CostBaseAdjustmentControl : UserControl, ITransactionControl 
    {
        private StockManager _StockManager;

        public CostBaseAdjustmentControl()
        {
            InitializeComponent();
        }

        public CostBaseAdjustmentControl(StockManager stockManager)
            : this()
        {
            _StockManager = stockManager;
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new CostBaseAdjustment();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            CostBaseAdjustment costbaseAdjustment = transaction as CostBaseAdjustment;

            dtpAdjustmentDate.Value = costbaseAdjustment.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == costbaseAdjustment.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            if (costbaseAdjustment.Method == AdjustmentMethod.Amount)
            {
                rdoAmount.Checked = true;
                txtAmount.Text =  MathUtils.FormatCurrency(costbaseAdjustment.Value);
            }
            else
            {
                rdoPercentage.Checked = true;
                txtAmount.Text = (costbaseAdjustment.Value * 100).ToString("#0.##");
            }
            txtComment.Text = costbaseAdjustment.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            CostBaseAdjustment costbaseAdjustment = transaction as CostBaseAdjustment;

            Stock stock = cboASXCode.SelectedItem as Stock;
            costbaseAdjustment.ASXCode = stock.ASXCode;
            costbaseAdjustment.TransactionDate = dtpAdjustmentDate.Value;
            if (rdoAmount.Checked)
                costbaseAdjustment.Method = AdjustmentMethod.Amount;
            else if (rdoPercentage.Checked)
                costbaseAdjustment.Method = AdjustmentMethod.Percentage;
            costbaseAdjustment.Value = MathUtils.ParseDecimal(txtAmount.Text) / 100;      
            costbaseAdjustment.Comment = txtComment.Text;
        }

        private void dtpAdjustmentDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockManager.GetStocks(dtpAdjustmentDate.Value).Where(x => x.Type != StockType.StapledSecurity).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }
        
    }
}

