
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class mechanicdash_viewrequest : Form
    {
        SqlConnection con = Program.con;

        public mechanicdash_viewrequest()
        {
            InitializeComponent();
        }

        // Load all complaints when the form opens
        private void mechanicdash_viewrequest_Load(object sender, EventArgs e)
        {
            getAllComplaints();
        }

        // Display all complaint records
        public void getAllComplaints()
        {
            try
            {
                string query = "SELECT * FROM complaint_details";

                SqlDataAdapter sda = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                dataGridView1.DataSource = dt;

                // Grid design
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.RowHeadersVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading complaints: " + ex.Message);
            }
        }

        // Search complaints between selected dates
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show(
                    "From Date cannot be greater than To Date.");
                return;
            }

            try
            {
                string query = @"
                    SELECT *
                    FROM complaint_details
                    WHERE reg_date >= @fromDate
                      AND reg_date < @toDate";

                SqlDataAdapter sda = new SqlDataAdapter(query, con);

                sda.SelectCommand.Parameters.Add(
                    "@fromDate", SqlDbType.DateTime).Value = fromDate;

                sda.SelectCommand.Parameters.Add(
                    "@toDate", SqlDbType.DateTime).Value =
                    toDate.AddDays(1);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No complaints found for the selected dates.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching dates: " + ex.Message);
            }
        }
    }
}
