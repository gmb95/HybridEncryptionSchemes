using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Hybrid_Click(object sender, EventArgs e)
        {
            Hide();
            HybridForm Hyb = new HybridForm();
            Hyb.ShowDialog();
            Show();
        }

        private void EaM_bt_Click(object sender, EventArgs e)
        {
            Hide();
            EaMForm EaM = new EaMForm();
            EaM.ShowDialog();
            Show();
        }

        private void HybridM_bt_Click(object sender, EventArgs e)
        {
            Hide();
            HybridMForm HybM = new HybridMForm();
            HybM.ShowDialog();
            Show();
        }

        private void OCB_bt_Click(object sender, EventArgs e)
        {
            Hide();
            OCBForm OCB = new OCBForm();
            OCB.ShowDialog();
            Show();
        }
    }
}
