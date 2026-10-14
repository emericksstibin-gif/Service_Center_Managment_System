using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class admin_viewstaff : Form
    {
        SqlConnection con = Program.con;

        public admin_viewstaff()
        {
            InitializeComponent();
        }

        // Show all staff
        public void getstaffdetails()
        {
            string query = "SELECT * FROM staff_details";

            SqlDataAdapter sda = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            dataGridView1.DataSource = dt;
        }

        // Search staff based on selected staff type
        public void searchstaff()
        {
            con.Open();
            string query = "SELECT * FROM staff_details WHERE staff_type = @staff_type";

            SqlDataAdapter sda = new SqlDataAdapter(query, con);

            // Convert ComboBox text to lowercase
            // because database values are lowercase
            string selectedType = comboBox1.Text.ToLower();

            sda.SelectCommand.Parameters.AddWithValue(
                "@staff_type",
                selectedType
            );

            DataTable dt = new DataTable();

            sda.Fill(dt);

            dataGridView1.DataSource = dt;
            con.Close();
        }

        // Form Load
        private void admin_viewstaff_Load(object sender, EventArgs e)
        {
            // Select All initially
            comboBox1.SelectedIndex = 0;

            // Show all staff when form opens
            getstaffdetails();
        }

        // Search Button
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select staff type");
                return;
            }

            if (comboBox1.Text == "All")
            {
                // Show all staff
                getstaffdetails();
            }
            else
            {
                // Show selected staff type
                searchstaff();
            }
        }

        // Back Button
        private void button2_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();

            obj.Show();

            this.Hide();
        }
    }
}