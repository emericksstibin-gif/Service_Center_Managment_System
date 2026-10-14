
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class mechanicdash_viewpaersreq : Form
    {
        SqlConnection con = Program.con;

        public mechanicdash_viewpaersreq()
        {
            InitializeComponent();
        }

        private void mechanicdash_viewpaersreq_Load(object sender, EventArgs e)
        {
            LoadPartsRequests();
        }

        public void LoadPartsRequests()
        {
            try
            {
                string query = @"
                    SELECT
                        request_id AS [Request ID],
                        vid AS [Vehicle ID],
                        mechanic AS [Mechanic],
                        part_name AS [Part Name],
                        quantity AS [Quantity],
                        remark AS [Remark]
                    FROM parts_request
                    ORDER BY request_id DESC";

                SqlDataAdapter sda = new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                dataGridView1.DataSource = dt;

                // Grid settings
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AutoGenerateColumns = true;

                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dataGridView1.MultiSelect = false;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.RowTemplate.Height = 30;

                // Grid font settings
                dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                    new System.Drawing.Font(
                        "Segoe UI", 10,
                        System.Drawing.FontStyle.Bold);

                dataGridView1.DefaultCellStyle.Font =
                    new System.Drawing.Font("Segoe UI", 10);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No parts requests found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading parts requests: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
