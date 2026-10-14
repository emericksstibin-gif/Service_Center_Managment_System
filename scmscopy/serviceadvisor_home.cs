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
    public partial class serviceadvisor_home : Form
    {
        public serviceadvisor_home()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            login obj = new login();
            obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            advisor_addrequest obj = new advisor_addrequest();
            obj.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            advisorview_home obj = new advisorview_home();
            obj.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            advisor_observation obj = new advisor_observation();
            obj.Show();
        }
    }
}
