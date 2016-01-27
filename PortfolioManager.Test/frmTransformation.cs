using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Test
{
    public partial class frmTransformation : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockManager _StockManager;
        private Transformation _Transformation;
        private BindingList<ResultStockDataRecord> _ResultingStockRecords;
        private Stock _Stock;

        public frmTransformation()
        {
            InitializeComponent();
        }

        public frmTransformation(StockManager stockManager)
            : this()
        {
            _StockManager = stockManager;

            DataGridViewComboBoxColumn resultingStockColumn = grdResultingStocks.Columns["colResultingStock"] as DataGridViewComboBoxColumn;
            resultingStockColumn.DataPropertyName = "Stock";
            resultingStockColumn.DisplayMember = "Name";
            resultingStockColumn.ValueMember = "Id";           
            resultingStockColumn.Items.AddRange(_StockManager.GetStocks().ToArray());

            grdResultingStocks.Columns["colOriginalUnits"].DataPropertyName = "OriginalUnits";
            grdResultingStocks.Columns["colNewUnits"].DataPropertyName = "NewUnits";
            grdResultingStocks.Columns["colCostBase"].DataPropertyName = "CostBasePercentage";

            _ResultingStockRecords = new BindingList<ResultStockDataRecord>();
            _ResultingStockRecords.AllowEdit = true;
            _ResultingStockRecords.AllowNew = true;
            _ResultingStockRecords.AllowRemove = true;
            grdResultingStocks.DataSource = _ResultingStockRecords;
        }
        
        private class ResultStockDataRecord
        {
            public Guid Stock { get; set; }
            public int OriginalUnits { get; set; }
            public int NewUnits { get; set; }
            public decimal CostBasePercentage { get; set; }
        }

        private void SetFormValues()
        {
            lblASXCode.Text = _StockManager.GetASXCode(_Transformation.Stock);
            dtpRecordDate.Value = _Transformation.ActionDate;
            txtDescription.Text = _Transformation.Description;
            txtCashComponent.Text = MathUtils.FormatCurrency(_Transformation.CashComponent, false);

            _ResultingStockRecords.Clear();
            foreach (ResultingStock resultingStock in _Transformation.ResultingStocks)
            {
                _ResultingStockRecords.Add(new ResultStockDataRecord()
                    {
                        Stock = resultingStock.Stock,
                        OriginalUnits = resultingStock.OriginalUnits,
                        NewUnits = resultingStock.NewUnits,
                        CostBasePercentage = resultingStock.CostBase

                    });
            }
        } 

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;

            if (ShowDialog() == DialogResult.OK)
            {
                return _Transformation;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _Transformation = corporateAction as Transformation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Transformation.Change(dtpRecordDate.Value,
                                 dtpImplementationDate.Value,
                                 MathUtils.ParseDecimal(txtCashComponent.Text),
                                 txtDescription.Text);

                // Add/Update result stocks
                foreach (ResultStockDataRecord resultingStockDataRecord in _ResultingStockRecords)
                {
                    if (_Transformation.ResultingStocks.Any(x => x.Stock == resultingStockDataRecord.Stock))
                        _Transformation.ChangeResultStock(resultingStockDataRecord.Stock, resultingStockDataRecord.OriginalUnits, resultingStockDataRecord.NewUnits, resultingStockDataRecord.CostBasePercentage);
                    else
                        _Transformation.AddResultStock(resultingStockDataRecord.Stock, resultingStockDataRecord.OriginalUnits, resultingStockDataRecord.NewUnits, resultingStockDataRecord.CostBasePercentage);
                }

                // Delete result stocks
                foreach (ResultingStock resultStock in _Transformation.ResultingStocks.ToList())
                {
                    if (! _ResultingStockRecords.Any(x => x.Stock == resultStock.Stock))
                        _Transformation.DeleteResultStock(resultStock.Stock);
                }

                return true;
            }
            else
                return false;
        } 

        public void ViewCorporateAction(ICorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _Transformation = corporateAction as Transformation;
            SetFormValues();
            ShowDialog();
        } 

        public Boolean DeleteCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _Transformation = corporateAction as Transformation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Stock.DeleteCorporateAction(_Transformation);
                return true;
            }
            return
                false;
        } 

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _Transformation = _Stock.AddTransformation(dtpRecordDate.Value,
                                                        dtpImplementationDate.Value,
                                                        MathUtils.ParseDecimal(txtCashComponent.Text),
                                                        txtDescription.Text);

                foreach (ResultStockDataRecord resultingStockDataRecord in _ResultingStockRecords)
                {
                    _Transformation.AddResultStock(resultingStockDataRecord.Stock, resultingStockDataRecord.OriginalUnits, resultingStockDataRecord.NewUnits, resultingStockDataRecord.CostBasePercentage);
                }
            }
        }

        private void grdResultingStocks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Ignore Errors
        }

        private void grdResultingStocks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex].Name == "colDelete")
            {
                senderGrid.Rows.RemoveAt(e.RowIndex);
            }
        }


    }
}
