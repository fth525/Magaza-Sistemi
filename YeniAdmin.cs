using Magaza5;
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
    public partial class YeniAdminForm : Form
    {
        public String transferKullaniciAdi;
        public String transferSifre;
        public String transferOp;

        public List<MainForm.Kullanici> kullanicilar;

        private void YeniAdminForm_Load(object sender, EventArgs e)
        {
            transferKullaniciAdi = "";
            transferSifre = "";
            transferOp = "iptal";

            lblAdminEkleHatali.Visible = false;
        }

        public YeniAdminForm()
        {
            InitializeComponent();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string inputKullaniciAdi = txtBoxKullaniciAdi.Text;
            string inputSifre = txtBoxSifre.Text;

            if (inputKullaniciAdi == "" || inputSifre == "")
            {
                lblAdminEkleHatali.Visible = true;

                return;
            }

            for (int i = 0; i < kullanicilar.Count(); i++)
            {
                MainForm.Kullanici tempKullanici = kullanicilar[i];

                if (inputKullaniciAdi == tempKullanici.kullaniciAdi)
                {
                    lblAdminEkleHatali.Visible = true;

                    return;
                }
            }

            transferKullaniciAdi = inputKullaniciAdi;
            transferSifre = inputSifre;
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
