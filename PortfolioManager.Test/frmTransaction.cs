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
    public partial class frmTransaction : Form
    {
        private readonly StockService _StockService;
        private ITransactionControl _TransactionControl;
        private AttachmentService _AttachmentService;

        private Transaction _Transaction;
        private Guid _Attachment;

        public frmTransaction()
        {
            InitializeComponent();
        }

        public frmTransaction(StockService stockService, AttachmentService attachmentService)
            : this()
        {
            _StockService = stockService;
            _AttachmentService = attachmentService;
        }

        private void ShowControl(TransactionType type)
        {
            UserControl control;
            if (type == TransactionType.Aquisition)
                control = new AquisitionControl(_StockService);
            else if (type == TransactionType.CostBaseAdjustment)
                control = new CostBaseAdjustmentControl(_StockService);
            else if (type == TransactionType.Disposal)
                control = new DisposalControl(_StockService);
            else if (type == TransactionType.Income)
                control = new IncomeControl(_StockService);
            else if (type == TransactionType.OpeningBalance)
                control = new OpeningBalanceControl(_StockService);
            else if (type == TransactionType.ReturnOfCapital)
                control = new ReturnOfCapitalControl(_StockService);
            else if (type == TransactionType.UnitCountAdjustment)
                control = new UnitAdjustmentControl(_StockService);
            else
                throw new NotSupportedException();

            _TransactionControl = control as ITransactionControl;

            control.Visible = true;
            pnlTransaction.Controls.Add(control);
        }

        public Transaction CreateTransaction(TransactionType type)
        {
            ShowControl(type);

            _Transaction = null;
            _Attachment = Guid.Empty;

            btnAddAttachment.Enabled = true;
            btnDeleteAttachment.Enabled = false;
            btnViewAttachment.Enabled = false;

            if (ShowDialog() == DialogResult.OK)
            {
                _Transaction = _TransactionControl.CreateTransaction();
                _Transaction.Attachment = _Attachment;
            }
            else
                _Transaction = null;

            return _Transaction;
        }

        public bool EditTransaction(Transaction transaction)
        {
            _Transaction = transaction;
            _Attachment = transaction.Attachment;

            btnAddAttachment.Enabled = Guid.Equals(_Attachment, Guid.Empty);
            btnDeleteAttachment.Enabled = !Guid.Equals(_Attachment, Guid.Empty);
            btnViewAttachment.Enabled = !Guid.Equals(_Attachment, Guid.Empty);

            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(_Transaction);
            if (ShowDialog() == DialogResult.OK)
            {
                _TransactionControl.UpdateTransaction(_Transaction);
                _Transaction.Attachment = _Attachment;
                return true;
            }
            else
                return false;
        }

        public void ViewTransaction(Transaction transaction)
        {
            _Transaction = transaction;
            _Attachment = transaction.Attachment;

            btnAddAttachment.Enabled = false;
            btnDeleteAttachment.Enabled = false;
            btnViewAttachment.Enabled = !Guid.Equals(_Attachment, Guid.Empty);

            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(_Transaction);
            ShowDialog(); 
        }

        public bool DeleteTransaction(Transaction transaction)
        {
            _Transaction = transaction;
            _Attachment = transaction.Attachment;

            btnAddAttachment.Enabled = false;
            btnDeleteAttachment.Enabled = false;
            btnViewAttachment.Enabled = !Guid.Equals(_Attachment, Guid.Empty);

            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(_Transaction);
            return (ShowDialog() == DialogResult.OK); 
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

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }

}
