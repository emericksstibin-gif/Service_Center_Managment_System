
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeperdash_viewupdate : Form
    {
        private readonly string connectionString =
            @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;";

        public storekeeperdash_viewupdate()
        {
            InitializeComponent();
        }

        // Load stock details when the form opens
        private void storekeeperdash_viewupdate_Load(object sender, EventArgs e)
        {
            LoadStockDetails();
        }

        // Display all stock details in one DataGridView
        private void LoadStockDetails()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT
                            part_name AS [Part Name],
                            category AS [Category],
                            unit_price AS [Unit Price],
                            quantity AS [Available Stock],
                            stock_type AS [Stock Type],
                            supplier AS [Supplier]
                        FROM dbo.parts
                        ORDER BY part_name ASC";

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

                        // Header appearance
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
                            MessageBox.Show("No stock details found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading stock details: " + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
