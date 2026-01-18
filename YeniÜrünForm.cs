using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Magaza5
{
    public partial class YeniÜrünForm : Form
    {
        public String transferIsim;
        public uint transferFiyat;
        public uint transferAdet;
        public String transferOp;

        private void YeniÜrünForm_Load(object sender, EventArgs e)
        {
            transferIsim = "";
            transferFiyat = 0;
            transferAdet = 0;
            transferOp = "iptal";
        }

        public YeniÜrünForm()
        {
            InitializeComponent();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            transferIsim = this.txtBoxIsim.Text;
            transferFiyat = Convert.ToUInt32(this.txtBoxFiyat.Text.ToString());
            transferAdet = Convert.ToUInt32(this.txtBoxAdet.Text.ToString());
            transferOp = "ekle";

            this.Hide();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            transferOp = "iptal";

            this.Hide();
        }
    }
}
