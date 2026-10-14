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
    public partial class mechanic_home : Form
    {
        public mechanic_home()
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
            mechanic_requestviewhome obj = new mechanic_requestviewhome();
            obj.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mechanic_addpartreq obj = new mechanic_addpartreq();
            obj.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mechanic_observation obj = new mechanic_observation();
            obj.Show();
        }
    }
}
