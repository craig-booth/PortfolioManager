namespace PortfolioManager.Test
{
    partial class frmMultipleTransactions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tabTransactions = new System.Windows.Forms.TabControl();
            this.btnDeleteAttachment = new System.Windows.Forms.Button();
            this.btnViewAttachment = new System.Windows.Forms.Button();
            this.btnAddAttachment = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(386, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(305, 439);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 14;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tabTransactions
            // 
            this.tabTransactions.Location = new System.Drawing.Point(12, 12);
            this.tabTransactions.Name = "tabTransactions";
            this.tabTransactions.SelectedIndex = 0;
            this.tabTransactions.Size = new System.Drawing.Size(453, 421);
            this.tabTransactions.TabIndex = 16;
            // 
            // btnDeleteAttachment
            // 
            this.btnDeleteAttachment.Location = new System.Drawing.Point(98, 440);
            this.btnDeleteAttachment.Margin = new System.Windows.Forms.Padding(2);
            this.btnDeleteAttachment.Name = "btnDeleteAttachment";
            this.btnDeleteAttachment.Size = new System.Drawing.Size(92, 19);
            this.btnDeleteAttachment.TabIndex = 19;
            this.btnDeleteAttachment.Text = "Delete Attachment";
            this.btnDeleteAttachment.UseVisualStyleBackColor = true;
            this.btnDeleteAttachment.Click += new System.EventHandler(this.btnDeleteAttachment_Click);
            // 
            // btnViewAttachment
            // 
            this.btnViewAttachment.Location = new System.Drawing.Point(194, 441);
            this.btnViewAttachment.Margin = new System.Windows.Forms.Padding(2);
            this.btnViewAttachment.Name = "btnViewAttachment";
            this.btnViewAttachment.Size = new System.Drawing.Size(103, 19);
            this.btnViewAttachment.TabIndex = 18;
            this.btnViewAttachment.Text = "View Attachment";
            this.btnViewAttachment.UseVisualStyleBackColor = true;
            this.btnViewAttachment.Click += new System.EventHandler(this.btnViewAttachment_Click);
            // 
            // btnAddAttachment
            // 
            this.btnAddAttachment.Location = new System.Drawing.Point(0, 441);
            this.btnAddAttachment.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddAttachment.Name = "btnAddAttachment";
            this.btnAddAttachment.Size = new System.Drawing.Size(92, 19);
            this.btnAddAttachment.TabIndex = 17;
            this.btnAddAttachment.Text = "Add Attachment";
            this.btnAddAttachment.UseVisualStyleBackColor = true;
            this.btnAddAttachment.Click += new System.EventHandler(this.btnAddAttachment_Click);
            // 
            // frmMultipleTransactions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 469);
            this.Controls.Add(this.btnDeleteAttachment);
            this.Controls.Add(this.btnViewAttachment);
            this.Controls.Add(this.btnAddAttachment);
            this.Controls.Add(this.tabTransactions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "frmMultipleTransactions";
            this.Text = "frmMultipleTransactions";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabControl tabTransactions;
        private System.Windows.Forms.Button btnDeleteAttachment;
        private System.Windows.Forms.Button btnViewAttachment;
        private System.Windows.Forms.Button btnAddAttachment;
    }
}