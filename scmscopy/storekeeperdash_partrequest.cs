
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeperdash_partrequest : Form
    {
        private readonly string connectionString =
            @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;";

        public storekeeperdash_partrequest()
        {
            InitializeComponent();
        }

        // Load all parts requests when the form opens
        private void storekeeperdash_partrequest_Load(
            object sender, EventArgs e)
        {
            LoadPartRequests();
        }

        // Display all parts requests in the DataGridView
        private void LoadPartRequests()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT
                            request_id AS [Request ID],
                            vid AS [Vehicle ID],
                            mechanic AS [Mechanic],
                            part_name AS [Part Name],
                            quantity AS [Requested Quantity],
                            remark AS [Remark]
                        FROM dbo.parts_request
                        ORDER BY request_id DESC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Grid settings
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.AllowUserToDeleteRows = false;
                        dataGridView1.AllowUserToResizeRows = false;
                        dataGridView1.AutoGenerateColumns = true;

                        dataGridView1.AutoSizeColumnsMode =
                            DataGridViewAutoSizeColumnsMode.Fill;

                        dataGridView1.SelectionMode =
                            DataGridViewSelectionMode.FullRowSelect;

                        dataGridView1.MultiSelect = false;
                        dataGridView1.RowHeadersVisible = false;
                        dataGridView1.RowTemplate.Height = 30;

                        // Header and cell appearance
                        dataGridView1.EnableHeadersVisualStyles = false;

                        dataGridView1.ColumnHeadersDefaultCellStyle.Font =
                            new System.Drawing.Font(
                                "Segoe UI", 10,
                                System.Drawing.FontStyle.Bold);

                        dataGridView1.DefaultCellStyle.Font =
                            new System.Drawing.Font("Segoe UI", 10);

                        dataGridView1.ColumnHeadersHeight = 35;

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show(
                                "No parts requests found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading parts requests: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
