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
            this.txtPercentage = new System.Windows.Forms.TextBox();
            this.dtpAdjustmentDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboASXCode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(97, 83);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(350, 94);
            this.txtComment.TabIndex = 48;
            // 
            // txtPercentage
            // 
            this.txtPercentage.Location = new System.Drawing.Point(97, 57);
            this.txtPercentage.Name = "txtPercentage";
            this.txtPercentage.Size = new System.Drawing.Size(100, 20);
            this.txtPercentage.TabIndex = 46;
            // 
            // dtpAdjustmentDate
            // 
            this.dtpAdjustmentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAdjustmentDate.Location = new System.Drawing.Point(97, 3);
            this.dtpAdjustmentDate.Name = "dtpAdjustmentDate";
            this.dtpAdjustmentDate.Size = new System.Drawing.Size(100, 20);
            this.dtpAdjustmentDate.TabIndex = 43;
            this.dtpAdjustmentDate.ValueChanged += new System.EventHandler(this.dtpAdjustmentDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 49;
            this.label6.Text = "Comment";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "Percentage";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 45;
            this.label2.Text = "Adjustment Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "ASX Code";
            // 
            // cboASXCode
            // 
            this.cboASXCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboASXCode.FormattingEnabled = true;
            this.cboASXCode.Location = new System.Drawing.Point(97, 29);
            this.cboASXCode.Name = "cboASXCode";
            this.cboASXCode.Size = new System.Drawing.Size(200, 21);
            this.cboASXCode.TabIndex = 42;
            // 
            // CostBaseAdjustmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtPercentage);
            this.Controls.Add(this.dtpAdjustmentDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboASXCode);
            this.Name = "CostBaseAdjustmentControl";
            this.Size = new System.Drawing.Size(458, 197);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtPercentage;
        private System.Windows.Forms.DateTimePicker dtpAdjustmentDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboASXCode;
    }
}
