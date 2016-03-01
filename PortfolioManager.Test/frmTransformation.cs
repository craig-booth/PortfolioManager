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
using StockManager.Service;

namespace PortfolioManager.Test
{
    public partial class frmTransformation : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockService _StockService;
        private Transformation _Transformation;
        private BindingList<ResultStockDataRecord> _ResultingStockRecords;
        private Stock _Stock;

        public frmTransformation()
        {
            InitializeComponent();
        }

        public frmTransformation(StockService stockService)
            : this()
        {
            _StockService = stockService;

            DataGridViewComboBoxColumn resultingStockColumn = grdResultingStocks.Columns["colResultingStock"] as DataGridViewComboBoxColumn;
            resultingStockColumn.DataPropertyName = "Stock";
            resultingStockColumn.DisplayMember = "Name";
            resultingStockColumn.ValueMember = "Id";           
            resultingStockColumn.Items.AddRange(_StockService.GetStocks().ToArray());

            grdResultingStocks.Columns["colOriginalUnits"].DataPropertyName = "OriginalUnits";
            grdResultingStocks.Columns["colNewUnits"].DataPropertyName = "NewUnits";
            grdResultingStocks.Columns["colCostBase"].DataPropertyName = "CostBase";
            grdResultingStocks.Columns["colAquisitionDate"].DataPropertyName = "AquisitionDate";

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
            public decimal CostBase { get; set; }
            public DateTime AquisitionDate { get; set; }
        }

        private void SetFormValues()
        {
            lblASXCode.Text = _StockService.GetASXCode(_Transformation.Stock);
            dtpRecordDate.Value = _Transformation.ActionDate;
            dtpImplementationDate.Value = _Transformation.ImplementationDate;
            txtDescription.Text = _Transformation.Description;
            txtCashComponent.Text = MathUtils.FormatCurrency(_Transformation.CashComponent, false);
            chkRolloverRelief.Checked = _Transformation.RolloverRefliefApplies;

            _ResultingStockRecords.Clear();
            foreach (ResultingStock resultingStock in _Transformation.ResultingStocks)
            {
                _ResultingStockRecords.Add(new ResultStockDataRecord()
                    {
                        Stock = resultingStock.Stock,
                        OriginalUnits = resultingStock.OriginalUnits,
                        NewUnits = resultingStock.NewUnits,
                        CostBase = resultingStock.CostBase,
                        AquisitionDate = resultingStock.AquisitionDate 
                    });
            }
        } 

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;
            chkRolloverRelief.Checked = true;

            if (ShowDialog() == DialogResult.OK)
            {
                return _Transformation;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _Transformation = corporateAction as Transformation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Transformation.ActionDate = dtpRecordDate.Value;
                _Transformation.ImplementationDate = dtpImplementationDate.Value;
                _Transformation.CashComponent = MathUtils.ParseDecimal(txtCashComponent.Text);
                _Transformation.RolloverRefliefApplies = chkRolloverRelief.Checked;
                _Transformation.Description = txtDescription.Text;

                // Delete are re-add result stocks
                _Transformation.ResultingStocks.RemoveAll(x => true);
                foreach (ResultStockDataRecord resultingStockDataRecord in _ResultingStockRecords)
                   _Transformation.AddResultStock(resultingStockDataRecord.Stock, resultingStockDataRecord.OriginalUnits, resultingStockDataRecord.NewUnits, resultingStockDataRecord.CostBase, resultingStockDataRecord.AquisitionDate);               

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
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _Transformation = corporateAction as Transformation;
            SetFormValues();
            return (ShowDialog() == DialogResult.OK);
        } 

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _Transformation = new Transformation(_Stock.Id, dtpRecordDate.Value,
                                                        dtpImplementationDate.Value,
                                                        MathUtils.ParseDecimal(txtCashComponent.Text),
                                                        chkRolloverRelief.Checked,
                                                        txtDescription.Text);


                foreach (ResultStockDataRecord resultingStockDataRecord in _ResultingStockRecords)
                    _Transformation.AddResultStock(resultingStockDataRecord.Stock, resultingStockDataRecord.OriginalUnits, resultingStockDataRecord.NewUnits, resultingStockDataRecord.CostBase, resultingStockDataRecord.AquisitionDate);
                
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

        private void chkRolloverRelief_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRolloverRelief.Checked)
            {
                grdResultingStocks.Columns["colCostBase"].HeaderText = "Costbase %";
                grdResultingStocks.Columns["colAquisitionDate"].Visible = false;
            }
            else
            {
                grdResultingStocks.Columns["colCostBase"].HeaderText = "Unit Costbase";
                grdResultingStocks.Columns["colAquisitionDate"].Visible = true;
            }
        }
    }
}
