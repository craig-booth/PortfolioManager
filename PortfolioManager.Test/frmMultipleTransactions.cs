using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Test.TransactionControls;
using PortfolioManager.Service;

namespace PortfolioManager.Test
{
    public partial class frmMultipleTransactions : Form
    {
        private readonly StockService _StockService;
        private AttachmentService _AttachmentService;

        private IEnumerable<Transaction> _Transactions;
        private Guid _Attachment;

        public frmMultipleTransactions()
        {
            InitializeComponent();
        }

        public frmMultipleTransactions(StockService stockService, AttachmentService attachmentService)
            : this()
        {
            _StockService = stockService;
            _AttachmentService = attachmentService;
        }

        private void AddTransactionTab(Transaction transaction)
        {
            UserControl control;
            string label;
            if (transaction.Type == TransactionType.Aquisition)
            {
                control = new AquisitionControl(_StockService);
                label = "Aquisition";
            }
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
            {
                control = new CostBaseAdjustmentControl(_StockService);
                label = "Cost Base Adjustment";
            }
            else if (transaction.Type == TransactionType.Disposal)
            {
                control = new DisposalControl(_StockService);
                label = "Disposal";
            }
            else if (transaction.Type == TransactionType.Income)
            {
                control = new IncomeControl(_StockService);
                label = "Income";
            }
            else if (transaction.Type == TransactionType.OpeningBalance)
            {
                control = new OpeningBalanceControl(_StockService);
                label = "Opening Balance";
            }
            else if (transaction.Type == TransactionType.ReturnOfCapital)
            {
                control = new ReturnOfCapitalControl(_StockService);
                label = "Return Of Capital";
            }
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
            {
                control = new UnitAdjustmentControl(_StockService);
                label = "Unit Count Adjustment";
            }
            else
                throw new NotSupportedException();

            var tabPage = new TabPage(label);
            tabTransactions.TabPages.Add(tabPage);
            tabPage.Controls.Add(control);
            tabPage.Tag = transaction;

            control.Visible = true;  
            (control as ITransactionControl).DisplayTransaction(transaction);
        }

        public bool EditTransactions(IEnumerable<Transaction> transactions)
        {

            _Transactions = transactions;
            _Attachment = Guid.Empty;

            btnAddAttachment.Enabled = true;
            btnDeleteAttachment.Enabled = false;
            btnViewAttachment.Enabled = false;

            foreach (Transaction transaction in transactions)
                AddTransactionTab(transaction);

            if (ShowDialog() == DialogResult.OK)
            {
                foreach (TabPage tabPage in tabTransactions.TabPages)
                {
                    ITransactionControl transactionControl = tabPage.Controls[0] as ITransactionControl;
                    Transaction transaction = tabPage.Tag as Transaction;

                    transactionControl.UpdateTransaction(transaction);
                }

                return true;
            }
            else
                return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (var transaction in _Transactions)
            {
                transaction.Attachment = _Attachment;
            }
        }

        private void btnAddAttachment_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var attachment = _AttachmentService.CreateAttachment(fileDialog.FileName);
                _Attachment = attachment.Id;

                btnAddAttachment.Enabled = false;
                btnDeleteAttachment.Enabled = true;
                btnViewAttachment.Enabled = true;
            }

        }

        private void btnViewAttachment_Click(object sender, EventArgs e)
        {
            var attachment = _AttachmentService.GetAttachment(_Attachment);

            // Create a temporary file to store the attachment
            var tempPath = Path.Combine(Path.GetTempPath(), "PortfolioManger");
            Directory.CreateDirectory(tempPath);
            var attachmentFileName = Path.ChangeExtension(Path.Combine(tempPath, attachment.Id.ToString()), attachment.Extension);
            var attachmentFile = new FileStream(attachmentFileName, FileMode.Create);

            attachment.Data.CopyTo(attachmentFile);

            attachmentFile.Close();

            var startInfo = new ProcessStartInfo(attachmentFileName);
            Process.Start(startInfo);
        }

        private void btnDeleteAttachment_Click(object sender, EventArgs e)
        {
            _AttachmentService.DeleteAttachment(_Attachment);

            _Attachment = Guid.Empty;

            btnAddAttachment.Enabled = true;
            btnDeleteAttachment.Enabled = false;
            btnViewAttachment.Enabled = false;
        }
    }
}
