using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeper_home : Form
    {
        public storekeeper_home()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            login obj = new login();
            obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            storekeeper_addparts obj = new storekeeper_addparts();
            obj.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            storekeeper_updatestock obj = new storekeeper_updatestock();
            obj.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            storekeeper_viewpartreq obj = new storekeeper_viewpartreq();
            obj.Show();
        }
    }
}
