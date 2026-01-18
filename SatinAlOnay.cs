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
    public partial class SatinAlOnay : Form
    {
        public String transferOp;

        public SatinAlOnay()
        {
            InitializeComponent();
        }

        private void SatinAlOnay_Load(object sender, EventArgs e)
        {
            transferOp = "Hayır";
        }

        private void btnEvet_Click(object sender, EventArgs e)
        {
            transferOp = "Evet";

            this.Hide();
        }

        private void btnHayir_Click(object sender, EventArgs e)
        {
            transferOp = "Hayır";

            this.Hide();
        }
    }
}
