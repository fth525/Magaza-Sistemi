namespace Magaza5
{
    public partial class BakiyeYukle
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxBakiyeYukleMiktar = new System.Windows.Forms.TextBox();
            this.btnBakiyeYukleOnay = new System.Windows.Forms.Button();
            this.btnBakiyeYukleIptal = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mitar :";
            // 
            // txtBoxBakiyeYukleMiktar
            // 
            this.txtBoxBakiyeYukleMiktar.Location = new System.Drawing.Point(61, 10);
            this.txtBoxBakiyeYukleMiktar.Name = "txtBoxBakiyeYukleMiktar";
            this.txtBoxBakiyeYukleMiktar.Size = new System.Drawing.Size(100, 22);
            this.txtBoxBakiyeYukleMiktar.TabIndex = 1;
            // 
            // btnBakiyeYukleOnay
            // 
            this.btnBakiyeYukleOnay.Location = new System.Drawing.Point(16, 51);
            this.btnBakiyeYukleOnay.Name = "btnBakiyeYukleOnay";
            this.btnBakiyeYukleOnay.Size = new System.Drawing.Size(145, 39);
            this.btnBakiyeYukleOnay.TabIndex = 2;
            this.btnBakiyeYukleOnay.Text = "Yükle";
            this.btnBakiyeYukleOnay.UseVisualStyleBackColor = true;
            this.btnBakiyeYukleOnay.Click += new System.EventHandler(this.btnBakiyeYukleOnay_Click);
            // 
            // btnBakiyeYukleIptal
            // 
            this.btnBakiyeYukleIptal.Location = new System.Drawing.Point(34, 98);
            this.btnBakiyeYukleIptal.Name = "btnBakiyeYukleIptal";
            this.btnBakiyeYukleIptal.Size = new System.Drawing.Size(100, 23);
            this.btnBakiyeYukleIptal.TabIndex = 2;
            this.btnBakiyeYukleIptal.Text = "İptal";
            this.btnBakiyeYukleIptal.UseVisualStyleBackColor = true;
            this.btnBakiyeYukleIptal.Click += new System.EventHandler(this.btnBakiyeYukleIptal_Click);
            // 
            // BakiyeYukle
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(174, 136);
            this.ControlBox = false;
            this.Controls.Add(this.btnBakiyeYukleIptal);
            this.Controls.Add(this.btnBakiyeYukleOnay);
            this.Controls.Add(this.txtBoxBakiyeYukleMiktar);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BakiyeYukle";
            this.Text = "BakiyeYukle";
            this.Load += new System.EventHandler(this.BakiyeYukle_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBoxBakiyeYukleMiktar;
        private System.Windows.Forms.Button btnBakiyeYukleOnay;
        private System.Windows.Forms.Button btnBakiyeYukleIptal;
    }
}