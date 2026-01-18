using Magaza5;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Magaza5
{
    public partial class MainForm : Form
    {
        public enum KullaniciType
        {
            musteri,
            admin
        }

        public class Kullanici
        {
            public uint id;
            public string kullaniciAdi;
            public string sifre;
            public uint bakiye = 0;
            public KullaniciType type;

            public Kullanici(uint id, string kullaniciAdi, string sifre, KullaniciType type)
            {
                this.id = id;
                this.kullaniciAdi = kullaniciAdi;
                this.sifre = sifre;
                this.type = type;
            }
        }

        public List<Kullanici> kullanicilar = new List<Kullanici>();

        public Kullanici mevcutKullanici = null;

        String sqlConnString = new SQLiteConnectionStringBuilder { DataSource = "database.sqlite", Version = 3 }.ToString();
        SQLiteConnection sqlConn = null;
        SQLiteCommand sqlCmd = null;
        SQLiteDataReader sqlReader = null;

        public class Urun
        {
            public uint id;
            public String isim;
            public uint fiyat;
            public uint adet;
            //public String resim = "Magaza5Resources\\no image.png";
            public String resim = "Magaza5Resources\\no image.png";

            public Urun(uint id, string isim, uint fiyat, uint adet)
            {
                this.id = id;
                this.isim = isim;
                this.fiyat = fiyat;
                this.adet = adet;
            }
        }

        //public List<Urun> urunler = new List<Urun>();

        static uint nextUrunIdNumber = 0;
        uint nextUrunId()
        {
            return nextUrunIdNumber++;
        }

        static uint nextKullaniciIdNumber = 0;
        uint nextKullaniciId()
        {
            return nextKullaniciIdNumber++;
        }

        public MainForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConn = new SQLiteConnection(sqlConnString);
            sqlConn.Open();

            sqlCmd = sqlConn.CreateCommand();
            sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS urunler(" +
                "id INTEGER PRIMARY KEY ASC AUTOINCREMENT, " +
                "isim TEXT NOT NULL, " +
                "fiyat INTEGER NOT NULL, " +
                "adet INTEGER NOT NULL, " +
                "resim TEXT DEFAULT \"Magaza5Resources\\no image.png\")";
            sqlCmd.ExecuteNonQuery();

            sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS kullanicilar(" +
                "id INTEGER PRIMARY KEY ASC AUTOINCREMENT, " +
                "kullaniciAdi TEXT NOT NULL, " +
                "sifre TEXT NOT NULL, " +
                "bakiye INTEGER DEFAULT 0, " +
                "type TEXT NOT NULL)";
            sqlCmd.ExecuteNonQuery();

            sqlCmd.CommandText = "SELECT id FROM kullanicilar WHERE kullaniciAdi == \"admin\"";
            sqlReader = sqlCmd.ExecuteReader();

            bool isAdminExists = !(Convert.ToBoolean(sqlReader.StepCount == 0));
            sqlReader.Close();

            if (!isAdminExists)
            {
                sqlCmd.CommandText = "INSERT INTO kullanicilar" +
                    "(id, kullaniciAdi, sifre, type)" +
                    "VALUES" +
                    "(@id, @kullaniciAdi, @sifre, @type)";
                sqlCmd.Parameters.AddWithValue("@id", nextKullaniciId());
                sqlCmd.Parameters.AddWithValue("@kullaniciAdi", "admin");
                sqlCmd.Parameters.AddWithValue("@sifre", "123");
                sqlCmd.Parameters.AddWithValue("@type", "admin");
                sqlCmd.ExecuteNonQuery();
            }

            Kullanici admin = new Kullanici(nextKullaniciId(), "admin", "123", KullaniciType.admin);
            kullanicilar.Add(admin);

            lblGirisHatali.Visible = false;
            pnlGirisPage.BringToFront();
        }

        uint fetchIdFromName(object sender2, uint n)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender2;

            String temp = button.Name;
            temp = temp.Substring(Convert.ToInt32(7));
            char[] tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new String(tempArray);
            temp = temp.Substring(Convert.ToInt32(n));
            tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new string(tempArray);

            uint urunId = Convert.ToUInt32(temp);

            return urunId;
        }

        uint fetchIdFromTxtBoxName(object sender2, uint n)
        {
            System.Windows.Forms.TextBox txtBox = (System.Windows.Forms.TextBox)sender2;
            String temp = txtBox.Name;
            temp = temp.Substring(Convert.ToInt32(10));
            char[] tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new String(tempArray);
            temp = temp.Substring(Convert.ToInt32(n));
            tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new string(tempArray);

            uint urunId = Convert.ToUInt32(temp);

            return urunId;
        }

        uint fetchIdFromNameMagaza(object sender2, uint n)
        {
            System.Windows.Forms.Button button = (System.Windows.Forms.Button)sender2;

            String temp = button.Name;
            temp = temp.Substring(Convert.ToInt32(13));
            char[] tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new String(tempArray);
            temp = temp.Substring(Convert.ToInt32(n));
            tempArray = temp.ToCharArray();
            Array.Reverse(tempArray);
            temp = new string(tempArray);

            uint urunId = Convert.ToUInt32(temp);

            return urunId;
        }

        Urun getUrunById(uint urunId)
        {
            sqlCmd.CommandText = "SELECT id, isim, fiyat, adet, resim FROM urunler WHERE id == @id";
            sqlCmd.Parameters.AddWithValue("@id", urunId);
            sqlReader = sqlCmd.ExecuteReader();

            if (sqlReader.StepCount == 0) {
                sqlReader.Close();
                return null;
            };

            while (sqlReader.Read())
            {
                String isim = sqlReader.GetString(1);
                uint fiyat = Convert.ToUInt32(sqlReader.GetInt32(2));
                uint adet = Convert.ToUInt32(sqlReader.GetInt32(3));
                String resim = sqlReader.GetString(4);

                Urun urun = new Urun(urunId, isim, fiyat, adet);
                urun.resim = resim;

                sqlReader.Close();
                return urun;
            }

            // Unreachable
            return null;

            //for (int i = 0; i < urunler.Count(); i++)
            //{
            //    Urun urun = urunler[i];
            //    if (urun.id == urunId) { return urun; }
            //}

            //return null;
        }

        private void panelYenile()
        {
            txtBoxArama.Text = "";
            txtBoxFiyatMin.Text = "";
            txtBoxFiyatMax.Text = "";
            chckBoxStoktaVar.Checked = false;

            this.flowUrunler.Controls.Clear();
            List<Urun> urunsFromDb = new List<Urun>();
            sqlCmd.CommandText = "SELECT id, isim, fiyat, adet, resim FROM urunler";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String isim = sqlReader.GetString(1);
                uint fiyat = Convert.ToUInt32(sqlReader.GetInt32(2));
                uint adet = Convert.ToUInt32(sqlReader.GetInt32(3));
                String resim = sqlReader.GetString(4);

                Urun tempUrun = new Urun(id, isim, fiyat, adet);
                tempUrun.resim = resim;
                urunsFromDb.Add(tempUrun);
            }
            sqlReader.Close();

            for (int i = 0; i < urunsFromDb.Count; i++)
            {
                Urun tempUrun = urunsFromDb[i];

                uint id = tempUrun.id;

                /*
                this.pnlMagazaPage.Controls.Add(this.btnTest);
                this.pnlMagazaPage.Controls.Add(this.btnAdminGiris);
                this.pnlMagazaPage.Dock = System.Windows.Forms.DockStyle.Fill;
                this.pnlMagazaPage.Location = new System.Drawing.Point(0, 0);
                this.pnlMagazaPage.Name = "pnlMagazaPage";
                this.pnlMagazaPage.Size = new System.Drawing.Size(800, 450);
                this.pnlMagazaPage.TabIndex = 0;
                */

                /*
                this.label1.Location = new System.Drawing.Point(301, 147);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(85, 16);
                this.label1.TabIndex = 1;
                this.label1.Text = "Kullanıcı Adı :";
                */

                /*
                this.btnGiris.Location = new System.Drawing.Point(304, 227);
                this.btnGiris.Name = "btnGiris";
                this.btnGiris.Size = new System.Drawing.Size(188, 43);
                this.btnGiris.TabIndex = 5;
                this.btnGiris.Text = "Giriş";
                this.btnGiris.UseVisualStyleBackColor = true;
                this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
                */

                /*
                this.txtBoxSifre.Location = new System.Drawing.Point(392, 180);
                this.txtBoxSifre.Name = "txtBoxSifre";
                this.txtBoxSifre.Size = new System.Drawing.Size(100, 22);
                this.txtBoxSifre.TabIndex = 4;
                */

                System.Windows.Forms.Label tempUrunLblIsim = new System.Windows.Forms.Label();
                tempUrunLblIsim.Text = tempUrun.isim;
                tempUrunLblIsim.Location = new System.Drawing.Point(0, 0);
                tempUrunLblIsim.Size = new System.Drawing.Size(174, 30);
                tempUrunLblIsim.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblIsim.Font = new Font("Microsoft Sans Serif", 15);
                tempUrunLblIsim.Name = "lblUrun" + id + "Isim";

                System.Windows.Forms.Label tempUrunLblId = new System.Windows.Forms.Label();
                tempUrunLblId.Text = "Id: " + tempUrun.id.ToString();
                tempUrunLblId.Location = new System.Drawing.Point(0, 30);
                tempUrunLblId.Size = new System.Drawing.Size(100, 20);
                tempUrunLblId.Font = new Font("Microsoft Sans Serif", 7.8f, FontStyle.Bold);
                tempUrunLblId.Name = "lblUrun" + id + "Id";

                System.Windows.Forms.Label tempUrunLblFiyat = new System.Windows.Forms.Label();
                tempUrunLblFiyat.Text = "Fiyat: ";
                tempUrunLblFiyat.Location = new System.Drawing.Point(0, 55);
                tempUrunLblFiyat.Size = new System.Drawing.Size(70, 20);
                tempUrunLblFiyat.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblFiyat.Name = "lblUrun" + id + "Fiyat";

                System.Windows.Forms.TextBox tempUrunTxtBoxFiyat = new System.Windows.Forms.TextBox();
                tempUrunTxtBoxFiyat.Location = new System.Drawing.Point(70, 55);
                tempUrunTxtBoxFiyat.Text = tempUrun.fiyat.ToString();
                tempUrunTxtBoxFiyat.Name = "txtBoxUrun" + id + "Fiyat";

                void txtBoxUrunFiyat_TextChanged(object sender2, EventArgs e2)
                {
                    uint urunId = fetchIdFromTxtBoxName(sender2, 5);
                    Urun urun = getUrunById(urunId);

                    if (((System.Windows.Forms.TextBox)sender2).Text == "")
                    {
                        urun.fiyat = 0;
                        ((System.Windows.Forms.TextBox)sender2).Text = "0";

                        return;
                    }

                    urun.fiyat = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender2).Text);
                    sqlCmd.CommandText = "UPDATE urunler SET fiyat = @fiyat WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@fiyat", urun.fiyat);
                    sqlCmd.ExecuteNonQuery();
                }
                tempUrunTxtBoxFiyat.TextChanged += new EventHandler(txtBoxUrunFiyat_TextChanged);

                System.Windows.Forms.Label tempUrunLblAdet = new System.Windows.Forms.Label();
                tempUrunLblAdet.Text = "Stok: ";
                tempUrunLblAdet.Location = new System.Drawing.Point(0, 76);
                tempUrunLblAdet.Size = new System.Drawing.Size(70, 20);
                tempUrunLblAdet.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblAdet.Name = "lblUrun" + id + "Adet";

                System.Windows.Forms.TextBox tempUrunTxtBoxAdet = new System.Windows.Forms.TextBox();
                tempUrunTxtBoxAdet.Location = new System.Drawing.Point(70, 76);
                tempUrunTxtBoxAdet.Text = tempUrun.adet.ToString();
                tempUrunTxtBoxAdet.Name = "txtBoxUrun" + id + "Adet";

                void txtBoxUrunAdet_TextChanged(object sender2, EventArgs e2)
                {
                    uint urunId = fetchIdFromTxtBoxName(sender2, 4);
                    Urun urun = getUrunById(urunId);

                    if (((System.Windows.Forms.TextBox)sender2).Text == "")
                    {
                        urun.adet = 0;
                        ((System.Windows.Forms.TextBox)sender2).Text = "0";

                        return;
                    }

                    urun.adet = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender2).Text);
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();
                }
                tempUrunTxtBoxAdet.TextChanged += new EventHandler(txtBoxUrunAdet_TextChanged);

                System.Windows.Forms.Button tempUrunBtnAdetArttır = new System.Windows.Forms.Button();
                tempUrunBtnAdetArttır.Location = new System.Drawing.Point(0, 101);
                tempUrunBtnAdetArttır.Name = "btnUrun" + id + "AdetArttır";
                tempUrunBtnAdetArttır.Text = "Adet Arttır";
                tempUrunBtnAdetArttır.UseVisualStyleBackColor = true;

                void btnUrunAdetArttır_Click(object sender2, EventArgs e2)
                {
                    uint n = 10;
                    uint urunId = fetchIdFromName(sender2, n);
                    Urun urun = getUrunById(urunId);

                    urun.adet += 1;
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.TextBox txtBoxUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 10) == "txtBoxUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                txtBoxUrunAdet = (System.Windows.Forms.TextBox)control;
                            }
                        }
                    }

                    txtBoxUrunAdet.Text = urun.adet.ToString();
                }
                tempUrunBtnAdetArttır.Click += new System.EventHandler(btnUrunAdetArttır_Click);

                System.Windows.Forms.Button tempUrunBtnAdetDusur = new System.Windows.Forms.Button();
                tempUrunBtnAdetDusur.Location = new System.Drawing.Point(95, 101);
                tempUrunBtnAdetDusur.Name = "btnUrun" + id + "AdetDusur";
                tempUrunBtnAdetDusur.Text = "Adet Düşür";
                tempUrunBtnAdetDusur.UseVisualStyleBackColor = true;

                void btnUrunAdetDusur_Click(object sender2, EventArgs e2)
                {
                    uint n = 9;
                    uint urunId = fetchIdFromName(sender2, n);
                    Urun urun = getUrunById(urunId);

                    if (urun.adet == 0) { return; }

                    urun.adet -= 1; sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.TextBox txtBoxUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 10) == "txtBoxUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                txtBoxUrunAdet = (System.Windows.Forms.TextBox)control;
                            }
                        }
                    }

                    txtBoxUrunAdet.Text = urun.adet.ToString();
                }
                tempUrunBtnAdetDusur.Click += new System.EventHandler(btnUrunAdetDusur_Click);

                System.Windows.Forms.Button tempUrunBtnSil = new System.Windows.Forms.Button();
                tempUrunBtnSil.Location = new System.Drawing.Point(0, 136);
                tempUrunBtnSil.Size = new System.Drawing.Size(174, 20);
                tempUrunBtnSil.Name = "btnUrun" + id + "Sil";
                tempUrunBtnSil.Text = "Ürünü Sil";
                tempUrunBtnSil.UseVisualStyleBackColor = true;

                void btnUrunSil_Click(object sender2, EventArgs e2)
                {
                    UrunSilOnay urunSilOnay = new UrunSilOnay();
                    urunSilOnay.ShowDialog();

                    if (urunSilOnay.transferOp == "Hayır")
                    {
                        urunSilOnay.Close();
                        return;
                    }

                    urunSilOnay.Close();

                    uint n = 3;
                    uint urunId = fetchIdFromName(sender2, n);

                    //for (int i = 0; i < urunler.Count; i++)
                    //{
                    //    Urun urun = urunler[i];

                    //    if (urun.id == urunId)
                    //    {
                    //        urunler.RemoveAt(i);

                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;

                    //        this.flowUrunler.Controls.Remove(pnlUrun);
                    //        return;
                    //    }
                    //}

                    //for (int j = 0; j < urunler.Count; j++)
                    //{
                    //    Urun urun = urunler[j];

                    //    if (urun.id == urunId)
                    //    {
                    //        urunler.RemoveAt(j);

                    //        /*
                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;

                    //        for (int k = 0; k < this.flowMagazaUrunler.Controls.Count; k++)
                    //        {
                    //            System.Windows.Forms.Panel pnlMagazaUrun = (System.Windows.Forms.Panel)this.flowMagazaUrunler.Controls[k];

                    //            for (int l = 0; l < pnlMagazaUrun.Controls.Count; l++)
                    //            {
                    //                System.Windows.Forms.Control control = pnlMagazaUrun.Controls[l];
                    //                if (control.Name.Substring(0, 9) == "lblUrunId")
                    //                {
                    //                    if (Convert.ToUInt32(control.Name.Substring(9)) == urunId)
                    //                    {

                    //                    }
                    //                }
                    //            }
                    //        }

                    //        this.flowUrunler.Controls.Remove(pnlUrun);
                    //        */

                    //        /*
                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)this.flowUrunler.Controls[i];
                    //        System.Windows.Forms.Panel pnlMagazaUrun = (System.Windows.Forms.Panel)this.flowMagazaUrunler.Controls[i];
                    //        */
                    //        panelYenile();

                    //        return;
                    //    }
                    //}
                    sqlCmd.CommandText = "DELETE FROM urunler WHERE id = @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;
                    this.flowUrunler.Controls.Remove(pnlUrun);
                }
                tempUrunBtnSil.Click += new System.EventHandler(btnUrunSil_Click);

                PictureBox tempUrunPicBoxResim = new PictureBox();
                tempUrunPicBoxResim.Location = new System.Drawing.Point(4, 163);
                tempUrunPicBoxResim.Size = new System.Drawing.Size(165, 165);
                tempUrunPicBoxResim.Name = "picBoxUrun" + id + "Resim";
                tempUrunPicBoxResim.SizeMode = PictureBoxSizeMode.Zoom;
                tempUrunPicBoxResim.Image = Image.FromFile(tempUrun.resim);
                tempUrunPicBoxResim.BackColor = Color.White;

                void picBoxUrunResim_Click(object sender2, EventArgs e2)
                {
                    System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                    fileDialog.Filter = "Image files (*.png, *,jpg, *.jpeg, *.bmp) | *.png; *.jpg; *.jpeg; *.bmp";

                    if (!(fileDialog.ShowDialog() == DialogResult.OK)) return;
                    tempUrun.resim = fileDialog.FileName;
                    sqlCmd.CommandText = "UPDATE urunler SET resim = @resim WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.Parameters.AddWithValue("@resim", tempUrun.resim);
                    sqlCmd.ExecuteNonQuery();

                    panelYenile();
                }
                tempUrunPicBoxResim.Click += new EventHandler(picBoxUrunResim_Click);

                System.Windows.Forms.Panel tempPnlUrunPanel = new System.Windows.Forms.Panel();
                tempPnlUrunPanel.Name = "pnlUrun" + id;
                tempPnlUrunPanel.BackColor = System.Drawing.Color.PeachPuff;
                tempPnlUrunPanel.Size = new System.Drawing.Size(174, 335);

                tempPnlUrunPanel.Controls.Add(tempUrunLblId);
                tempPnlUrunPanel.Controls.Add(tempUrunLblIsim);
                tempPnlUrunPanel.Controls.Add(tempUrunLblFiyat);
                tempPnlUrunPanel.Controls.Add(tempUrunTxtBoxFiyat);
                tempPnlUrunPanel.Controls.Add(tempUrunLblAdet);
                tempPnlUrunPanel.Controls.Add(tempUrunTxtBoxAdet);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnAdetArttır);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnAdetDusur);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnSil);
                tempPnlUrunPanel.Controls.Add(tempUrunPicBoxResim);

                tempPnlUrunPanel.Parent = this.flowUrunler;
            }
            sqlReader.Close();
        }

        private void panelFiltreYenile()
        {
            this.flowUrunler.Controls.Clear();

            List<Urun> urunsFromDb = new List<Urun>();
            sqlCmd.CommandText = "SELECT id, isim, fiyat, adet, resim FROM urunler";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String isim = sqlReader.GetString(1);
                uint fiyat = Convert.ToUInt32(sqlReader.GetInt32(2));
                uint adet = Convert.ToUInt32(sqlReader.GetInt32(3));
                String resim = sqlReader.GetString(4);

                Urun tempUrun = new Urun(id, isim, fiyat, adet);
                tempUrun.resim = resim;
                urunsFromDb.Add(tempUrun);
            }
            sqlReader.Close();

            for (int i = 0; i < urunsFromDb.Count(); i++)
            {
                Urun tempUrun = urunsFromDb[i];

                if (chckBoxStoktaVar.Checked && tempUrun.adet == 0) continue;
                if (txtBoxFiyatMin.Text != "" && tempUrun.fiyat < Convert.ToUInt32(txtBoxFiyatMin.Text)) continue;
                if (txtBoxFiyatMax.Text != "" && tempUrun.fiyat > Convert.ToUInt32(txtBoxFiyatMax.Text)) continue;
                if (!(tempUrun.isim.ToLower().Contains(txtBoxArama.Text.ToLower()))) continue;

                uint id = tempUrun.id;

                /*
                this.pnlMagazaPage.Controls.Add(this.btnTest);
                this.pnlMagazaPage.Controls.Add(this.btnAdminGiris);
                this.pnlMagazaPage.Dock = System.Windows.Forms.DockStyle.Fill;
                this.pnlMagazaPage.Location = new System.Drawing.Point(0, 0);
                this.pnlMagazaPage.Name = "pnlMagazaPage";
                this.pnlMagazaPage.Size = new System.Drawing.Size(800, 450);
                this.pnlMagazaPage.TabIndex = 0;
                */

                /*
                this.label1.Location = new System.Drawing.Point(301, 147);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(85, 16);
                this.label1.TabIndex = 1;
                this.label1.Text = "Kullanıcı Adı :";
                */

                /*
                this.btnGiris.Location = new System.Drawing.Point(304, 227);
                this.btnGiris.Name = "btnGiris";
                this.btnGiris.Size = new System.Drawing.Size(188, 43);
                this.btnGiris.TabIndex = 5;
                this.btnGiris.Text = "Giriş";
                this.btnGiris.UseVisualStyleBackColor = true;
                this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
                */

                /*
                this.txtBoxSifre.Location = new System.Drawing.Point(392, 180);
                this.txtBoxSifre.Name = "txtBoxSifre";
                this.txtBoxSifre.Size = new System.Drawing.Size(100, 22);
                this.txtBoxSifre.TabIndex = 4;
                */

                System.Windows.Forms.Label tempUrunLblIsim = new System.Windows.Forms.Label();
                tempUrunLblIsim.Text = tempUrun.isim;
                tempUrunLblIsim.Location = new System.Drawing.Point(0, 0);
                tempUrunLblIsim.Size = new System.Drawing.Size(174, 30);
                tempUrunLblIsim.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblIsim.Font = new Font("Microsoft Sans Serif", 15);
                tempUrunLblIsim.Name = "lblUrun" + id + "Isim";

                System.Windows.Forms.Label tempUrunLblId = new System.Windows.Forms.Label();
                tempUrunLblId.Text = "Id: " + tempUrun.id.ToString();
                tempUrunLblId.Location = new System.Drawing.Point(0, 30);
                tempUrunLblId.Size = new System.Drawing.Size(100, 20);
                tempUrunLblId.Font = new Font("Microsoft Sans Serif", 7.8f, FontStyle.Bold);
                tempUrunLblId.Name = "lblUrun" + id + "Id";

                System.Windows.Forms.Label tempUrunLblFiyat = new System.Windows.Forms.Label();
                tempUrunLblFiyat.Text = "Fiyat: ";
                tempUrunLblFiyat.Location = new System.Drawing.Point(0, 55);
                tempUrunLblFiyat.Size = new System.Drawing.Size(70, 20);
                tempUrunLblFiyat.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblFiyat.Name = "lblUrun" + id + "Fiyat";

                System.Windows.Forms.TextBox tempUrunTxtBoxFiyat = new System.Windows.Forms.TextBox();
                tempUrunTxtBoxFiyat.Location = new System.Drawing.Point(70, 55);
                tempUrunTxtBoxFiyat.Text = tempUrun.fiyat.ToString();
                tempUrunTxtBoxFiyat.Name = "txtBoxUrun" + id + "Fiyat";

                void txtBoxUrunFiyat_TextChanged(object sender2, EventArgs e2)
                {
                    uint urunId = fetchIdFromTxtBoxName(sender2, 5);
                    Urun urun = getUrunById(urunId);

                    if (((System.Windows.Forms.TextBox)sender2).Text == "")
                    {
                        urun.fiyat = 0;
                        ((System.Windows.Forms.TextBox)sender2).Text = "0";

                        return;
                    }

                    urun.fiyat = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender2).Text);
                    sqlCmd.CommandText = "UPDATE urunler SET fiyat = @fiyat WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@fiyat", urun.fiyat);
                    sqlCmd.ExecuteNonQuery();
                }
                tempUrunTxtBoxFiyat.TextChanged += new EventHandler(txtBoxUrunFiyat_TextChanged);

                System.Windows.Forms.Label tempUrunLblAdet = new System.Windows.Forms.Label();
                tempUrunLblAdet.Text = "Stok: ";
                tempUrunLblAdet.Location = new System.Drawing.Point(0, 76);
                tempUrunLblAdet.Size = new System.Drawing.Size(70, 20);
                tempUrunLblAdet.TextAlign = ContentAlignment.MiddleCenter;
                tempUrunLblAdet.Name = "lblUrun" + id + "Adet";

                System.Windows.Forms.TextBox tempUrunTxtBoxAdet = new System.Windows.Forms.TextBox();
                tempUrunTxtBoxAdet.Location = new System.Drawing.Point(70, 76);
                tempUrunTxtBoxAdet.Text = tempUrun.adet.ToString();
                tempUrunTxtBoxAdet.Name = "txtBoxUrun" + id + "Adet";

                void txtBoxUrunAdet_TextChanged(object sender2, EventArgs e2)
                {
                    uint urunId = fetchIdFromTxtBoxName(sender2, 4);
                    Urun urun = getUrunById(urunId);

                    if (((System.Windows.Forms.TextBox)sender2).Text == "")
                    {
                        urun.adet = 0;
                        ((System.Windows.Forms.TextBox)sender2).Text = "0";

                        return;
                    }

                    urun.adet = Convert.ToUInt32(((System.Windows.Forms.TextBox)sender2).Text);
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();
                }
                tempUrunTxtBoxAdet.TextChanged += new EventHandler(txtBoxUrunAdet_TextChanged);

                System.Windows.Forms.Button tempUrunBtnAdetArttır = new System.Windows.Forms.Button();
                tempUrunBtnAdetArttır.Location = new System.Drawing.Point(0, 101);
                tempUrunBtnAdetArttır.Name = "btnUrun" + id + "AdetArttır";
                tempUrunBtnAdetArttır.Text = "Adet Arttır";
                tempUrunBtnAdetArttır.UseVisualStyleBackColor = true;

                void btnUrunAdetArttır_Click(object sender2, EventArgs e2)
                {
                    uint n = 10;
                    uint urunId = fetchIdFromName(sender2, n);
                    Urun urun = getUrunById(urunId);

                    urun.adet += 1;
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.TextBox txtBoxUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 10) == "txtBoxUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                txtBoxUrunAdet = (System.Windows.Forms.TextBox)control;
                            }
                        }
                    }

                    txtBoxUrunAdet.Text = urun.adet.ToString();
                }
                tempUrunBtnAdetArttır.Click += new System.EventHandler(btnUrunAdetArttır_Click);

                System.Windows.Forms.Button tempUrunBtnAdetDusur = new System.Windows.Forms.Button();
                tempUrunBtnAdetDusur.Location = new System.Drawing.Point(95, 101);
                tempUrunBtnAdetDusur.Name = "btnUrun" + id + "AdetDusur";
                tempUrunBtnAdetDusur.Text = "Adet Düşür";
                tempUrunBtnAdetDusur.UseVisualStyleBackColor = true;

                void btnUrunAdetDusur_Click(object sender2, EventArgs e2)
                {
                    uint n = 9;
                    uint urunId = fetchIdFromName(sender2, n);
                    Urun urun = getUrunById(urunId);

                    if (urun.adet == 0) { return; }

                    urun.adet -= 1; sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.TextBox txtBoxUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 10) == "txtBoxUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                txtBoxUrunAdet = (System.Windows.Forms.TextBox)control;
                            }
                        }
                    }

                    txtBoxUrunAdet.Text = urun.adet.ToString();
                }
                tempUrunBtnAdetDusur.Click += new System.EventHandler(btnUrunAdetDusur_Click);

                System.Windows.Forms.Button tempUrunBtnSil = new System.Windows.Forms.Button();
                tempUrunBtnSil.Location = new System.Drawing.Point(0, 136);
                tempUrunBtnSil.Size = new System.Drawing.Size(174, 20);
                tempUrunBtnSil.Name = "btnUrun" + id + "Sil";
                tempUrunBtnSil.Text = "Ürünü Sil";
                tempUrunBtnSil.UseVisualStyleBackColor = true;

                void btnUrunSil_Click(object sender2, EventArgs e2)
                {
                    UrunSilOnay urunSilOnay = new UrunSilOnay();
                    urunSilOnay.ShowDialog();

                    if (urunSilOnay.transferOp == "Hayır")
                    {
                        urunSilOnay.Close();
                        return;
                    }

                    urunSilOnay.Close();

                    uint n = 3;
                    uint urunId = fetchIdFromName(sender2, n);

                    //for (int i = 0; i < urunler.Count; i++)
                    //{
                    //    Urun urun = urunler[i];

                    //    if (urun.id == urunId)
                    //    {
                    //        urunler.RemoveAt(i);

                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;

                    //        this.flowUrunler.Controls.Remove(pnlUrun);
                    //        return;
                    //    }
                    //}

                    //for (int j = 0; j < urunler.Count; j++)
                    //{
                    //    Urun urun = urunler[j];

                    //    if (urun.id == urunId)
                    //    {
                    //        urunler.RemoveAt(j);

                    //        /*
                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;

                    //        for (int k = 0; k < this.flowMagazaUrunler.Controls.Count; k++)
                    //        {
                    //            System.Windows.Forms.Panel pnlMagazaUrun = (System.Windows.Forms.Panel)this.flowMagazaUrunler.Controls[k];

                    //            for (int l = 0; l < pnlMagazaUrun.Controls.Count; l++)
                    //            {
                    //                System.Windows.Forms.Control control = pnlMagazaUrun.Controls[l];
                    //                if (control.Name.Substring(0, 9) == "lblUrunId")
                    //                {
                    //                    if (Convert.ToUInt32(control.Name.Substring(9)) == urunId)
                    //                    {

                    //                    }
                    //                }
                    //            }
                    //        }

                    //        this.flowUrunler.Controls.Remove(pnlUrun);
                    //        */

                    //        /*
                    //        System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)this.flowUrunler.Controls[i];
                    //        System.Windows.Forms.Panel pnlMagazaUrun = (System.Windows.Forms.Panel)this.flowMagazaUrunler.Controls[i];
                    //        */
                    //        panelYenile();

                    //        return;
                    //    }
                    //}
                    sqlCmd.CommandText = "DELETE FROM urunler WHERE id = @id";
                    sqlCmd.Parameters.AddWithValue("@id", urunId);
                    sqlCmd.ExecuteNonQuery();

                    System.Windows.Forms.Panel pnlUrun = (System.Windows.Forms.Panel)((System.Windows.Forms.Button)sender2).Parent;
                    this.flowUrunler.Controls.Remove(pnlUrun);
                }
                tempUrunBtnSil.Click += new System.EventHandler(btnUrunSil_Click);

                PictureBox tempUrunPicBoxResim = new PictureBox();
                tempUrunPicBoxResim.Location = new System.Drawing.Point(4, 163);
                tempUrunPicBoxResim.Size = new System.Drawing.Size(165, 165);
                tempUrunPicBoxResim.Name = "picBoxUrun" + id + "Resim";
                tempUrunPicBoxResim.SizeMode = PictureBoxSizeMode.Zoom;
                tempUrunPicBoxResim.Image = Image.FromFile(tempUrun.resim);
                tempUrunPicBoxResim.BackColor = Color.White;

                void picBoxUrunResim_Click(object sender2, EventArgs e2)
                {
                    System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                    fileDialog.Filter = "Image files (*.png, *,jpg, *.jpeg, *.bmp) | *.png; *.jpg; *.jpeg; *.bmp";

                    if (!(fileDialog.ShowDialog() == DialogResult.OK)) return;
                    tempUrun.resim = fileDialog.FileName;
                    sqlCmd.CommandText = "UPDATE urunler SET resim = @resim WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.Parameters.AddWithValue("@resim", tempUrun.resim);
                    sqlCmd.ExecuteNonQuery();

                    panelYenile();
                }
                tempUrunPicBoxResim.Click += new EventHandler(picBoxUrunResim_Click);

                System.Windows.Forms.Panel tempPnlUrunPanel = new System.Windows.Forms.Panel();
                tempPnlUrunPanel.Name = "pnlUrun" + id;
                tempPnlUrunPanel.BackColor = System.Drawing.Color.PeachPuff;
                tempPnlUrunPanel.Size = new System.Drawing.Size(174, 335);

                tempPnlUrunPanel.Controls.Add(tempUrunLblId);
                tempPnlUrunPanel.Controls.Add(tempUrunLblIsim);
                tempPnlUrunPanel.Controls.Add(tempUrunLblFiyat);
                tempPnlUrunPanel.Controls.Add(tempUrunTxtBoxFiyat);
                tempPnlUrunPanel.Controls.Add(tempUrunLblAdet);
                tempPnlUrunPanel.Controls.Add(tempUrunTxtBoxAdet);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnAdetArttır);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnAdetDusur);
                tempPnlUrunPanel.Controls.Add(tempUrunBtnSil);
                tempPnlUrunPanel.Controls.Add(tempUrunPicBoxResim);

                tempPnlUrunPanel.Parent = this.flowUrunler;
            }
        }

        private void magazaYenile()
        {
            lblMerhaba.Text = "Merhaba " + mevcutKullanici.kullaniciAdi;

            txtBoxMagazaArama.Text = "";
            txtBoxMagazaFiyatMin.Text = "";
            txtBoxMagazaFiyatMax.Text = "";
            chckBoxMagazaStoktaVar.Checked = false;

            lblBakiye.Text = "Bakiye : " + mevcutKullanici.bakiye.ToString();

            flowMagazaUrunler.Controls.Clear();

            List<Urun> urunsFromDb = new List<Urun>();
            sqlCmd.CommandText = "SELECT id, isim, fiyat, adet, resim FROM urunler";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String isim = sqlReader.GetString(1);
                uint fiyat = Convert.ToUInt32(sqlReader.GetInt32(2));
                uint adet = Convert.ToUInt32(sqlReader.GetInt32(3));
                String resim = sqlReader.GetString(4);

                Urun tempUrun = new Urun(id, isim, fiyat, adet);
                tempUrun.resim = resim;
                urunsFromDb.Add(tempUrun);
            }
            sqlReader.Close();


            for (int i = 0; i < urunsFromDb.Count(); i++)
            {
                Urun tempUrun = urunsFromDb[i];

                uint id = tempUrun.id;

                System.Windows.Forms.Label tempLblMagazaUrunIsim = new System.Windows.Forms.Label();
                tempLblMagazaUrunIsim.Text = tempUrun.isim;
                tempLblMagazaUrunIsim.Location = new System.Drawing.Point(0, 0);
                tempLblMagazaUrunIsim.Size = new System.Drawing.Size(174, 30);
                tempLblMagazaUrunIsim.TextAlign = ContentAlignment.MiddleCenter;
                tempLblMagazaUrunIsim.Font = new Font("Microsoft Sans Serif", 15);
                tempLblMagazaUrunIsim.Name = "lblMagazaUrun" + id + "Isim";

                System.Windows.Forms.PictureBox tempPicBoxMagazaUrunResim = new System.Windows.Forms.PictureBox();
                tempPicBoxMagazaUrunResim.Location = new System.Drawing.Point(4, 30);
                tempPicBoxMagazaUrunResim.Size = new System.Drawing.Size(165, 165);
                tempPicBoxMagazaUrunResim.BackColor = System.Drawing.Color.White;
                tempPicBoxMagazaUrunResim.SizeMode = PictureBoxSizeMode.Zoom;
                tempPicBoxMagazaUrunResim.Image = Image.FromFile(tempUrun.resim);
                tempPicBoxMagazaUrunResim.Name = "picBoxMagazaUrun" + id + "Resim";

                System.Windows.Forms.Label tempLblMagazaUrunFiyat = new System.Windows.Forms.Label();
                tempLblMagazaUrunFiyat.Text = "Fiyat : " + tempUrun.fiyat;
                tempLblMagazaUrunFiyat.Location = new System.Drawing.Point(0, 205);
                tempLblMagazaUrunFiyat.Size = new System.Drawing.Size(87, 20);
                tempLblMagazaUrunFiyat.Name = "lblMagazaUrun" + id + "Fiyat";

                System.Windows.Forms.Label tempLblMagazaUrunAdet = new System.Windows.Forms.Label();
                tempLblMagazaUrunAdet.Text = "Stok : " + tempUrun.adet;
                tempLblMagazaUrunAdet.Location = new System.Drawing.Point(87, 205);
                tempLblMagazaUrunAdet.Size = new System.Drawing.Size(87, 20);
                tempLblMagazaUrunAdet.Name = "lblMagazaUrun" + id + "Adet";

                System.Windows.Forms.Button tempBtnMagazaUrunSatinAl = new System.Windows.Forms.Button();
                tempBtnMagazaUrunSatinAl.Text = "Satın Al";
                tempBtnMagazaUrunSatinAl.Location = new System.Drawing.Point(10, 227);
                tempBtnMagazaUrunSatinAl.Size = new System.Drawing.Size(154, 35);
                tempBtnMagazaUrunSatinAl.Name = "btnMagazaUrun" + id + "SatinAl";
                tempBtnMagazaUrunSatinAl.UseVisualStyleBackColor = true;
                if (tempUrun.adet == 0) tempBtnMagazaUrunSatinAl.Enabled = false;

                void btnMagazaUrunSatinAl_Click(object sender2, EventArgs e2)
                {
                    uint n = 7;
                    uint urunId = fetchIdFromNameMagaza(sender2, n);
                    Urun urun = getUrunById(urunId);

                    if (mevcutKullanici.bakiye < urun.fiyat)
                    {
                        MessageBox.Show("Yetersiz Bakiye");
                        return;
                    }

                    SatinAlOnay satinAlOnay = new SatinAlOnay();
                    satinAlOnay.ShowDialog();

                    if (satinAlOnay.transferOp == "Hayır")
                    {
                        satinAlOnay.Close();

                        return;
                    }

                    satinAlOnay.Close();

                    urun.adet -= 1;
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urun.id);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    mevcutKullanici.bakiye -= urun.fiyat;
                    sqlCmd.CommandText = "UPDATE kullanicilar SET bakiye = @bakiye WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", mevcutKullanici.id);
                    sqlCmd.Parameters.AddWithValue("@bakiye", mevcutKullanici.bakiye);
                    sqlCmd.ExecuteNonQuery();

                    lblBakiye.Text = "Bakiye : " + mevcutKullanici.bakiye.ToString();

                    System.Windows.Forms.Label lblMagazaUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 13) == "lblMagazaUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                lblMagazaUrunAdet = (System.Windows.Forms.Label)control;
                            }
                        }
                    }

                    lblMagazaUrunAdet.Text = "Stok : " + urun.adet;

                    if (urun.adet == 0)
                    {
                        ((System.Windows.Forms.Button)sender2).Enabled = false;
                    }
                }
                tempBtnMagazaUrunSatinAl.Click += new EventHandler(btnMagazaUrunSatinAl_Click);

                System.Windows.Forms.Panel tempPnlMagazaUrunPanel = new System.Windows.Forms.Panel();
                tempPnlMagazaUrunPanel.Name = "pnlMagazaUrun" + id;
                tempPnlMagazaUrunPanel.BackColor = System.Drawing.Color.LightYellow;
                tempPnlMagazaUrunPanel.Size = new System.Drawing.Size(174, 270);

                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunIsim);
                tempPnlMagazaUrunPanel.Controls.Add(tempPicBoxMagazaUrunResim);
                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunFiyat);
                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunAdet);
                tempPnlMagazaUrunPanel.Controls.Add(tempBtnMagazaUrunSatinAl);

                tempPnlMagazaUrunPanel.Parent = this.flowMagazaUrunler;
            }
        }

        private void magazaFiltreYenile()
        {
            flowMagazaUrunler.Controls.Clear();

            List<Urun> urunsFromDb = new List<Urun>();
            sqlCmd.CommandText = "SELECT id, isim, fiyat, adet, resim FROM urunler";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String isim = sqlReader.GetString(1);
                uint fiyat = Convert.ToUInt32(sqlReader.GetInt32(2));
                uint adet = Convert.ToUInt32(sqlReader.GetInt32(3));
                String resim = sqlReader.GetString(4);

                Urun tempUrun = new Urun(id, isim, fiyat, adet);
                tempUrun.resim = resim;
                urunsFromDb.Add(tempUrun);
            }
            sqlReader.Close();

            for (int i = 0; i < urunsFromDb.Count(); i++)
            {
                Urun tempUrun = urunsFromDb[i];

                if (chckBoxMagazaStoktaVar.Checked && tempUrun.adet == 0) continue;
                if (txtBoxMagazaFiyatMin.Text != "" && tempUrun.fiyat < Convert.ToUInt32(txtBoxMagazaFiyatMin.Text)) continue;
                if (txtBoxMagazaFiyatMax.Text != "" && tempUrun.fiyat > Convert.ToUInt32(txtBoxMagazaFiyatMax.Text)) continue;
                if (!(tempUrun.isim.ToLower().Contains(txtBoxMagazaArama.Text.ToLower()))) continue;

                //uint fiyatMin = 0;
                //uint fiyatMax = 0;
                //if (!(txtBoxFiyatMin.Text == "")) fiyatMin = Convert.ToUInt32(txtBoxFiyatMin);
                //if (!(txtBoxFiyatMax.Text == "")) fiyatMax = Convert.ToUInt32(txtBoxFiyatMax);

                //String inputSearch = txtBoxArama.Text;
                //if (
                //    !(tempUrun.isim.ToLower().Contains(inputSearch.ToLower())) ||
                //    !(tempUrun.fiyat >= fiyatMin) ||
                //    !(tempUrun.fiyat <= fiyatMax) ||
                //    !(chckBoxStoktaVar.Checked && tempUrun.adet > 0)
                //    ) continue;

                uint id = tempUrun.id;

                System.Windows.Forms.Label tempLblMagazaUrunIsim = new System.Windows.Forms.Label();
                tempLblMagazaUrunIsim.Text = tempUrun.isim;
                tempLblMagazaUrunIsim.Location = new System.Drawing.Point(0, 0);
                tempLblMagazaUrunIsim.Size = new System.Drawing.Size(174, 30);
                tempLblMagazaUrunIsim.TextAlign = ContentAlignment.MiddleCenter;
                tempLblMagazaUrunIsim.Font = new Font("Microsoft Sans Serif", 15);
                tempLblMagazaUrunIsim.Name = "lblMagazaUrun" + id + "Isim";

                System.Windows.Forms.PictureBox tempPicBoxMagazaUrunResim = new System.Windows.Forms.PictureBox();
                tempPicBoxMagazaUrunResim.Location = new System.Drawing.Point(4, 30);
                tempPicBoxMagazaUrunResim.Size = new System.Drawing.Size(165, 165);
                tempPicBoxMagazaUrunResim.BackColor = System.Drawing.Color.White;
                tempPicBoxMagazaUrunResim.SizeMode = PictureBoxSizeMode.Zoom;
                tempPicBoxMagazaUrunResim.Image = Image.FromFile(tempUrun.resim);
                tempPicBoxMagazaUrunResim.Name = "picBoxMagazaUrun" + id + "Resim";

                System.Windows.Forms.Label tempLblMagazaUrunFiyat = new System.Windows.Forms.Label();
                tempLblMagazaUrunFiyat.Text = "Fiyat : " + tempUrun.fiyat;
                tempLblMagazaUrunFiyat.Location = new System.Drawing.Point(0, 205);
                tempLblMagazaUrunFiyat.Size = new System.Drawing.Size(87, 20);
                tempLblMagazaUrunFiyat.Name = "lblMagazaUrun" + id + "Fiyat";

                System.Windows.Forms.Label tempLblMagazaUrunAdet = new System.Windows.Forms.Label();
                tempLblMagazaUrunAdet.Text = "Stok : " + tempUrun.adet;
                tempLblMagazaUrunAdet.Location = new System.Drawing.Point(87, 205);
                tempLblMagazaUrunAdet.Size = new System.Drawing.Size(87, 20);
                tempLblMagazaUrunAdet.Name = "lblMagazaUrun" + id + "Adet";

                System.Windows.Forms.Button tempBtnMagazaUrunSatinAl = new System.Windows.Forms.Button();
                tempBtnMagazaUrunSatinAl.Text = "Satın Al";
                tempBtnMagazaUrunSatinAl.Location = new System.Drawing.Point(10, 227);
                tempBtnMagazaUrunSatinAl.Size = new System.Drawing.Size(154, 35);
                tempBtnMagazaUrunSatinAl.Name = "btnMagazaUrun" + id + "SatinAl";
                tempBtnMagazaUrunSatinAl.UseVisualStyleBackColor = true;
                if (tempUrun.adet == 0) tempBtnMagazaUrunSatinAl.Enabled = false;

                void btnMagazaUrunSatinAl_Click(object sender2, EventArgs e2)
                {
                    uint n = 7;
                    uint urunId = fetchIdFromNameMagaza(sender2, n);
                    Urun urun = getUrunById(urunId);

                    if (mevcutKullanici.bakiye < urun.fiyat)
                    {
                        MessageBox.Show("Yetersiz Bakiye");
                        return;
                    }

                    SatinAlOnay satinAlOnay = new SatinAlOnay();
                    satinAlOnay.ShowDialog();

                    if (satinAlOnay.transferOp == "Hayır")
                    {
                        satinAlOnay.Close();

                        return;
                    }

                    satinAlOnay.Close();

                    urun.adet -= 1;
                    sqlCmd.CommandText = "UPDATE urunler SET adet = @adet WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", urun.id);
                    sqlCmd.Parameters.AddWithValue("@adet", urun.adet);
                    sqlCmd.ExecuteNonQuery();

                    mevcutKullanici.bakiye -= urun.fiyat;
                    sqlCmd.CommandText = "UPDATE kullanicilar SET bakiye = @bakiye WHERE id == @id";
                    sqlCmd.Parameters.AddWithValue("@id", mevcutKullanici.id);
                    sqlCmd.Parameters.AddWithValue("@bakiye", mevcutKullanici.bakiye);
                    sqlCmd.ExecuteNonQuery();

                    lblBakiye.Text = "Bakiye : " + mevcutKullanici.bakiye.ToString();

                    System.Windows.Forms.Label lblMagazaUrunAdet = null;
                    for (int j = 0; j < (((System.Windows.Forms.Button)sender2).Parent.Controls.Count); j++)
                    {
                        System.Windows.Forms.Control control = ((System.Windows.Forms.Button)sender2).Parent.Controls[j];

                        if (control.Name.Substring(0, 13) == "lblMagazaUrun")
                        {
                            String temp = control.Name;
                            char[] tempArray = temp.ToCharArray();
                            Array.Reverse(tempArray);
                            temp = new String(tempArray);

                            if (temp.Substring(0, 4) == "tedA")
                            {
                                lblMagazaUrunAdet = (System.Windows.Forms.Label)control;
                            }
                        }
                    }

                    lblMagazaUrunAdet.Text = "Ürün Adedi : " + urun.adet;

                    if (urun.adet == 0)
                    {
                        ((System.Windows.Forms.Button)sender2).Enabled = false;
                    }
                }
                tempBtnMagazaUrunSatinAl.Click += new EventHandler(btnMagazaUrunSatinAl_Click);

                System.Windows.Forms.Panel tempPnlMagazaUrunPanel = new System.Windows.Forms.Panel();
                tempPnlMagazaUrunPanel.Name = "pnlMagazaUrun" + id;
                tempPnlMagazaUrunPanel.BackColor = System.Drawing.Color.LightYellow;
                tempPnlMagazaUrunPanel.Size = new System.Drawing.Size(174, 270);

                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunIsim);
                tempPnlMagazaUrunPanel.Controls.Add(tempPicBoxMagazaUrunResim);
                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunFiyat);
                tempPnlMagazaUrunPanel.Controls.Add(tempLblMagazaUrunAdet);
                tempPnlMagazaUrunPanel.Controls.Add(tempBtnMagazaUrunSatinAl);

                tempPnlMagazaUrunPanel.Parent = this.flowMagazaUrunler;
            }
        }

        private void btnMagazayaDon_Click(object sender, EventArgs e)
        {
            this.pnlMagazaPage.BringToFront();
            magazaYenile();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string inputKullaniciAdi = txtBoxKullaniciAdi.Text;
            string inputSifre = txtBoxSifre.Text;

            List<Kullanici> kullanicisFromDB = new List<Kullanici>();
            sqlCmd.CommandText = "SELECT id, kullaniciAdi, sifre, bakiye, type FROM kullanicilar";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String kullaniciAdi = sqlReader.GetString(1);
                String sifre = sqlReader.GetString(2);
                uint bakiye= Convert.ToUInt32(sqlReader.GetInt32(3));
                String typeString = sqlReader.GetString(4);
                KullaniciType type = KullaniciType.musteri;
                if (typeString == "musteri") type = KullaniciType.musteri;
                if (typeString == "admin") type = KullaniciType.admin;
                Kullanici tempKullanici = new Kullanici(id, kullaniciAdi, sifre, type);
                tempKullanici.bakiye = bakiye;
                kullanicisFromDB.Add(tempKullanici);
            }
            sqlReader.Close();

            bool basarili = false;
            for (int i = 0; i < kullanicisFromDB.Count(); i++)
            {
                Kullanici tempKullanici = kullanicisFromDB[i];

                if (inputKullaniciAdi == tempKullanici.kullaniciAdi && inputSifre == tempKullanici.sifre)
                {
                    basarili = true;
                    mevcutKullanici = tempKullanici;
                }
                
            }

            if (basarili)
            {
                if (mevcutKullanici.type == KullaniciType.admin)
                {
                    btnPanel.Enabled = true;
                    btnPanel.Visible = true;
                }
                else
                {
                    btnPanel.Enabled = false;
                    btnPanel.Visible = false;
                }

                pnlMagazaPage.BringToFront();
                magazaYenile();
            }
            else
            {
                lblGirisHatali.Visible = true;
            }

            //String kullaniciAdi = txtBoxKullaiciAdi.Text;
            //String sifre = txtBoxSifre.Text;

            //if (kullaniciAdi == "admin" && sifre == "123")
            //{
            //    this.btnMagazayaDon.Parent = this.pnlAdminPage;
            //    this.pnlAdminPage.BringToFront();
            //    panelYenile();
            //}
            //else
            //{
            //    this.lblSifreHatali.Visible = true;
            //}
        }

        private void btnPanel_Click(object sender, EventArgs e)
        {
            // this.pnlTestPage.BringToFront();
            this.pnlAdminPage.BringToFront();
            panelYenile();
        }

        private void btnYeniUrun_Click(object sender, EventArgs e)
        {
            YeniÜrünForm yeniUrunForm = new YeniÜrünForm();
            yeniUrunForm.ShowDialog();

            String op = yeniUrunForm.transferOp;

            yeniUrunForm.Close();

            if (op == "iptal") { return; }

            String inputIsim = yeniUrunForm.transferIsim;
            uint inputFiyat = yeniUrunForm.transferFiyat;
            uint inputAdet = yeniUrunForm.transferAdet;

            sqlCmd.CommandText = "INSERT INTO urunler (isim, fiyat, adet) VALUES (@isim, @fiyat, @adet)";
            sqlCmd.Parameters.AddWithValue("@isim", inputIsim);
            sqlCmd.Parameters.AddWithValue("@fiyat", inputFiyat);
            sqlCmd.Parameters.AddWithValue("@adet", inputAdet);
            sqlCmd.ExecuteNonQuery();

            //sqlCmd.CommandText = "SELECT seq FROM sqlite_sequence WHERE name == \"kullanicilar\"";
            //sqlReader = sqlCmd.ExecuteReader();
            //while (sqlReader.Read())
            //{
            //    id = Convert.ToUInt32(sqlReader.GetInt32(0));
            //}
            //sqlReader.Close();

            //uint id = nextUrunId();

            /*
            if (id == 3)
            {
                urunler.RemoveAt(0);
                this.flowUrunler.Controls.RemoveAt(0);
            }
            */

            //Urun tempUrun = new Urun(id, inputIsim, inputFiyat, inputAdet);
            //urunler.Add(tempUrun);

            panelYenile();
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            btnDemoDatabase.Parent = pnlKayitOlPage;
            lblAdminKullaniciAdi.Parent = pnlKayitOlPage;
            lblAdminSifre.Parent = pnlKayitOlPage;

            txtBoxKayitKullaniciAdi.Text = "";
            txtBoxKayitSifre.Text = "";
            lblKayitHatali.Visible = false;
            pnlKayitOlPage.BringToFront();
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            btnDemoDatabase.Parent = pnlGirisPage;
            lblAdminKullaniciAdi.Parent = pnlGirisPage;
            lblAdminSifre.Parent = pnlGirisPage;

            txtBoxKullaniciAdi.Text = "";
            txtBoxSifre.Text = "";
            lblGirisHatali.Visible = false;
            pnlGirisPage.BringToFront();
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            string inputKullaniciAdi = txtBoxKayitKullaniciAdi.Text;
            string inputSifre = txtBoxKayitSifre.Text;

            if (inputKullaniciAdi == "" || inputSifre == "")
            {
                lblKayitHatali.Visible = true;

                return;
            }

            List<Kullanici> kullanicisFromDB = new List<Kullanici>();
            sqlCmd.CommandText = "SELECT id, kullaniciAdi, sifre, bakiye, type FROM kullanicilar";
            sqlReader = sqlCmd.ExecuteReader();
            while (sqlReader.Read())
            {
                uint id = Convert.ToUInt32(sqlReader.GetInt32(0));
                String kullaniciAdi = sqlReader.GetString(1);
                String sifre = sqlReader.GetString(2);
                uint bakiye = Convert.ToUInt32(sqlReader.GetInt32(3));
                String typeString = sqlReader.GetString(4);
                KullaniciType type = KullaniciType.musteri;
                if (typeString == "musteri") type = KullaniciType.musteri;
                if (typeString == "admin") type = KullaniciType.admin;
                Kullanici tempKullanici = new Kullanici(id, kullaniciAdi, sifre, type);
                tempKullanici.bakiye = bakiye;
                kullanicisFromDB.Add(tempKullanici);
            }
            sqlReader.Close();

            for (int i = 0; i < kullanicisFromDB.Count(); i++)
            {
                Kullanici tempKullanici = kullanicisFromDB[i];

                if (inputKullaniciAdi == tempKullanici.kullaniciAdi)
                {
                    lblKayitHatali.Visible = true;

                    return;
                }
            }

            Kullanici tempYeniKullanici = new Kullanici(nextKullaniciId(), inputKullaniciAdi, inputSifre, KullaniciType.musteri);
            //kullanicilar.Add(tempYeniKullanici);
            sqlCmd.CommandText = "INSERT INTO kullanicilar (kullaniciAdi, sifre, type) VALUES (@kullaniciAdi, @sifre, @type)";
            sqlCmd.Parameters.AddWithValue("@kullaniciAdi", tempYeniKullanici.kullaniciAdi);
            sqlCmd.Parameters.AddWithValue("@sifre", tempYeniKullanici.sifre);
            sqlCmd.Parameters.AddWithValue("@type", "musteri");
            sqlCmd.ExecuteNonQuery();

            mevcutKullanici = tempYeniKullanici;

            if (mevcutKullanici.type == KullaniciType.admin)
            {
                btnPanel.Enabled = true;
                btnPanel.Visible = true;
            }
            else
            {
                btnPanel.Enabled = false;
                btnPanel.Visible = false;
            }

            pnlMagazaPage.BringToFront();
            magazaYenile();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            mevcutKullanici = null;

            lblGirisHatali.Visible = false;
            txtBoxKullaniciAdi.Text = "";
            txtBoxSifre.Text = "";
            pnlGirisPage.BringToFront();
        }

        private void btnAdminEkle_Click(object sender, EventArgs e)
        {
            YeniAdminForm yeniAdminForm = new YeniAdminForm();
            yeniAdminForm.kullanicilar = kullanicilar;
            yeniAdminForm.ShowDialog();

            String op = yeniAdminForm.transferOp;

            yeniAdminForm.Close();

            if (op == "iptal") { return; }

            String kullaniciAdi = yeniAdminForm.transferKullaniciAdi;
            String sifre = yeniAdminForm.transferSifre;

            Kullanici tempKullanici = new Kullanici(nextKullaniciId(), kullaniciAdi, sifre, KullaniciType.admin);
            //kullanicilar.Add(tempKullanici);
            sqlCmd.CommandText = "INSERT INTO kullanicilar (kullaniciAdi, sifre, type) VALUES (@kullaniciAdi, @sifre, @type)";
            sqlCmd.Parameters.AddWithValue("@kullaniciAdi", tempKullanici.kullaniciAdi);
            sqlCmd.Parameters.AddWithValue("@sifre", tempKullanici.sifre);
            sqlCmd.Parameters.AddWithValue("@type", "admin");
            sqlCmd.ExecuteNonQuery();
        }

        private void txtBoxMagazaArama_TextChanged(object sender, EventArgs e)
        {
            magazaFiltreYenile();
        }

        private void txtBoxMagazaFiyatMin_TextChanged(object sender, EventArgs e)
        {
            magazaFiltreYenile();
        }

        private void txtBoxMagazaFiyatMax_TextChanged(object sender, EventArgs e)
        {
            magazaFiltreYenile();
        }

        private void chckBoxMagazaStoktaVar_CheckedChanged(object sender, EventArgs e)
        {
            magazaFiltreYenile();
        }

        private void txtBoxArama_TextChanged(object sender, EventArgs e)
        {
            panelFiltreYenile();
        }

        private void txtBoxFiyatMin_TextChanged(object sender, EventArgs e)
        {
            panelFiltreYenile();
        }

        private void txtBoxFiyatMax_TextChanged(object sender, EventArgs e)
        {
            panelFiltreYenile();
        }

        private void chckBoxStoktaVar_CheckedChanged(object sender, EventArgs e)
        {
            panelFiltreYenile();
        }

        private void btnBakiyeYukle_Click(object sender, EventArgs e)
        {
            BakiyeYukle bakiyeYukleForm = new BakiyeYukle();
            bakiyeYukleForm.ShowDialog();

            String op = bakiyeYukleForm.transferOp;
            uint miktar = bakiyeYukleForm.transferMiktar;

            bakiyeYukleForm.Close();

            if (op == "iptal") return;

            mevcutKullanici.bakiye += miktar;
            sqlCmd.CommandText = "UPDATE kullanicilar SET bakiye = @bakiye WHERE id == @id";
            sqlCmd.Parameters.AddWithValue("@id", mevcutKullanici.id);
            sqlCmd.Parameters.AddWithValue("@bakiye", mevcutKullanici.bakiye);
            sqlCmd.ExecuteNonQuery();

            lblBakiye.Text = "Bakiye : " + mevcutKullanici.bakiye.ToString();
        }

        private void btnDemoDatabase_Click(object sender, EventArgs e)
        {
            //Urun tempUrun = new Urun(nextUrunId(), "RTX 5090", 150, 20);
            //tempUrun.resim = "Magaza5Resources\\rtx 5090.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "RTX 4090", 100, 25);
            //tempUrun.resim = "Magaza5Resources\\rtx 4090.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "İntel Core İ9", 80, 0);
            //tempUrun.resim = "Magaza5Resources\\intel core i9.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "İntel Core İ7", 90, 7);
            //tempUrun.resim = "Magaza5Resources\\intel core i7.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "Laptop", 120, 29);
            //tempUrun.resim = "Magaza5Resources\\laptop.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "Tablet", 100, 0);
            //tempUrun.resim = "Magaza5Resources\\tablet.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "Mouse", 20, 30);
            //tempUrun.resim = "Magaza5Resources\\mouse.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "Klavye", 25, 28);
            //tempUrun.resim = "Magaza5Resources\\klavye.jpg";
            //urunler.Add(tempUrun);

            //tempUrun = new Urun(nextUrunId(), "Kulaklık", 24, 39);
            //tempUrun.resim = "Magaza5Resources\\kulaklık.jpg";
            //urunler.Add(tempUrun);

            sqlCmd.CommandText = "" +
                "DROP TABLE urunler;\n" +
                "\n" +
                "CREATE TABLE urunler (\n" +
                "   id INTEGER PRIMARY KEY ASC AUTOINCREMENT,\n" +
                "   isim TEXT NOT NULL,\n" +
                "   fiyat INTEGER NOT NULL,\n" +
                "   adet INTEGER NOT NULL,\n" +
                "   resim TEXT DEFAULT \"Magaza5Resources\\no image.png\"\n" +
                ");\n" +
                "\n\n" +
                "DROP TABLE kullanicilar;\n" +
                "\n" +
                "CREATE TABLE kullanicilar (\n" +
                "   id INTEGER PRIMARY KEY ASC AUTOINCREMENT,\n" +
                "   kullaniciAdi TEXT NOT NULL,\n" +
                "   sifre TEXT NOT NULL,\n" +
                "   bakiye INTEGER DEFAULT 0,\n" +
                "   type TEXT NOT NULL\n" +
                ");\n" +
                "\n" +
                "INSERT INTO kullanicilar\n" +
                "   (kullaniciAdi, sifre, type)\n" +
                "VALUES\n" +
                "   (\"admin\", \"123\", \"admin\"),\n" +
                "   (\"musteri\", \"123\", \"musteri\");\n" +
                "\n" +
                "INSERT INTO urunler\n" +
                "   (isim, fiyat, adet, resim)\n" +
                "VALUES\n" +
                "   (\"RTX 5090\", 150, 20, \"Magaza5Resources\\rtx 5090.jpg\"),\n" +
                "   (\"RTX 4090\", 100, 25, \"Magaza5Resources\\rtx 4090.jpg\"),\n" +
                "   (\"İntel Core İ9\", 80, 0, \"Magaza5Resources\\intel core i9.jpg\"),\n" +
                "   (\"İntel Core İ7\", 90, 7, \"Magaza5Resources\\intel core i7.jpg\"),\n" +
                "   (\"Laptop\", 120, 29, \"Magaza5Resources\\laptop.jpg\"),\n" +
                "   (\"Tablet\", 100, 0, \"Magaza5Resources\\tablet.jpg\"),\n" +
                "   (\"Mouse\", 20, 30, \"Magaza5Resources\\mouse.jpg\"),\n" +
                "   (\"Klavye\", 25, 28, \"Magaza5Resources\\klavye.jpg\"),\n" +
                "   (\"Kulaklık\", 24, 39, \"Magaza5Resources\\kulaklık.jpg\");\n" +
                "\n";
            MessageBox.Show(sqlCmd.CommandText);
            sqlCmd.ExecuteNonQuery();
        }
    }
}
