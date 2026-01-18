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
    public partial class BakiyeYukle : Form
    {
        public uint transferMiktar;
        public String transferOp;

        public BakiyeYukle()
        {
            InitializeComponent();
        }

        private void BakiyeYukle_Load(object sender, EventArgs e)
        {
            txtBoxBakiyeYukleMiktar.Text = "";
            transferMiktar = 0;
            transferOp = "iptal";
        }

        private void btnBakiyeYukleOnay_Click(object sender, EventArgs e)
        {
            transferMiktar = Convert.ToUInt32(txtBoxBakiyeYukleMiktar.Text);
            transferOp = "onay";

            this.Hide();
        }

        private void btnBakiyeYukleIptal_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
