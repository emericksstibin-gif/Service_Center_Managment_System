using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace scmscopy
{
    public partial class advidorhome_viewreq : Form
    {
        SqlConnection con = Program.con;
        public advidorhome_viewreq()
        {
            InitializeComponent();
        }

        private void advidorhome_viewreq_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            comboBox1.Items.Add("Customer Details");
            comboBox1.Items.Add("Complaint Details");

            comboBox1.SelectedIndex = -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Customer Details")
            {
                string query = "SELECT * FROM customer_details";

                SqlDataAdapter sda = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            else if (comboBox1.Text == "Complaint Details")
            {
                string query = "SELECT * FROM complaint_details";

                SqlDataAdapter sda = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }
    }
}
