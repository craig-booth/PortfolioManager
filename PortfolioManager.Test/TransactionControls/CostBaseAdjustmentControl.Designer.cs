namespace PortfolioManager.Test.TransactionControls
{
    partial class CostBaseAdjustmentControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.dtpAdjustmentDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboASXCode = new System.Windows.Forms.ComboBox();
            this.rdoAmount = new System.Windows.Forms.RadioButton();
            this.rdoPercentage = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(129, 144);
            this.txtComment.Margin = new System.Windows.Forms.Padding(4);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(465, 115);
            this.txtComment.TabIndex = 48;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(129, 103);
            this.txtAmount.Margin = new System.Windows.Forms.Padding(4);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(132, 22);
            this.txtAmount.TabIndex = 46;
            // 
            // dtpAdjustmentDate
            // 
            this.dtpAdjustmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAdjustmentDate.Location = new System.Drawing.Point(129, 4);
            this.dtpAdjustmentDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpAdjustmentDate.Name = "dtpAdjustmentDate";
            this.dtpAdjustmentDate.Size = new System.Drawing.Size(132, 22);
            this.dtpAdjustmentDate.TabIndex = 43;
            this.dtpAdjustmentDate.ValueChanged += new System.EventHandler(this.dtpAdjustmentDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 144);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 17);
            this.label6.TabIndex = 49;
            this.label6.Text = "Comment";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 103);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 47;
            this.label4.Text = "Amount";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 17);
            this.label2.TabIndex = 45;
            this.label2.Text = "Adjustment Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 44;
            this.label1.Text = "ASX Code";
            // 
            // cboASXCode
            // 
            this.cboASXCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboASXCode.FormattingEnabled = true;
            this.cboASXCode.Location = new System.Drawing.Point(129, 36);
            this.cboASXCode.Margin = new System.Windows.Forms.Padding(4);
            this.cboASXCode.Name = "cboASXCode";
            this.cboASXCode.Size = new System.Drawing.Size(265, 24);
            this.cboASXCode.TabIndex = 42;
            // 
            // rdoAmount
            // 
            this.rdoAmount.AutoSize = true;
            this.rdoAmount.Checked = true;
            this.rdoAmount.Location = new System.Drawing.Point(129, 75);
            this.rdoAmount.Name = "rdoAmount";
            this.rdoAmount.Size = new System.Drawing.Size(77, 21);
            this.rdoAmount.TabIndex = 50;
            this.rdoAmount.TabStop = true;
            this.rdoAmount.Text = "Amount";
            this.rdoAmount.UseVisualStyleBackColor = true;
            // 
            // rdoPercentage
            // 
            this.rdoPercentage.AutoSize = true;
            this.rdoPercentage.Location = new System.Drawing.Point(245, 75);
            this.rdoPercentage.Name = "rdoPercentage";
            this.rdoPercentage.Size = new System.Drawing.Size(102, 21);
            this.rdoPercentage.TabIndex = 51;
            this.rdoPercentage.Text = "Percentage";
            this.rdoPercentage.UseVisualStyleBackColor = true;
            // 
            // CostBaseAdjustmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rdoPercentage);
            this.Controls.Add(this.rdoAmount);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.dtpAdjustmentDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboASXCode);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CostBaseAdjustmentControl";
            this.Size = new System.Drawing.Size(611, 287);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.DateTimePicker dtpAdjustmentDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboASXCode;
        private System.Windows.Forms.RadioButton rdoAmount;
        private System.Windows.Forms.RadioButton rdoPercentage;
    }
}
