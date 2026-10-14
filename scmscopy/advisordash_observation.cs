
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class advisordash_observation : Form
    {
        SqlConnection con = Program.con;

        public advisordash_observation()
        {
            InitializeComponent();

            dataGridView1.CellContentClick +=
                dataGridView1_CellContentClick;

            this.Load += advisordash_observation_Load;
        }

        private void advisordash_observation_Load(
            object sender, EventArgs e)
        {
            LoadAdvisorObservations();
        }

        private void LoadAdvisorObservations()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string query = @"
                    SELECT
                        observation_id AS [Observation ID],
                        vid AS [VID],
                        staff_name AS [Service Advisor],
                        observation_details AS [Observation],
                        observation_date AS [Observation Date],
                        review_status AS [Review Status],
                        admin_remark AS [Admin Remark]
                    FROM dbo.advisor_observation
                    ORDER BY observation_date DESC";

                SqlDataAdapter da =
                    new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                AddApproveRejectButtons();

                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.RowHeadersVisible = false;

                ApplyColours();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading observations: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void AddApproveRejectButtons()
        {
            if (!dataGridView1.Columns.Contains("Approve"))
            {
                DataGridViewButtonColumn approveButton =
                    new DataGridViewButtonColumn();

                approveButton.Name = "Approve";
                approveButton.HeaderText = "Approve";
                approveButton.Text = "Approve";
                approveButton.UseColumnTextForButtonValue = true;
                approveButton.FlatStyle = FlatStyle.Flat;

                dataGridView1.Columns.Add(approveButton);
            }

            if (!dataGridView1.Columns.Contains("Reject"))
            {
                DataGridViewButtonColumn rejectButton =
                    new DataGridViewButtonColumn();

                rejectButton.Name = "Reject";
                rejectButton.HeaderText = "Reject";
                rejectButton.Text = "Reject";
                rejectButton.UseColumnTextForButtonValue = true;
                rejectButton.FlatStyle = FlatStyle.Flat;

                dataGridView1.Columns.Add(rejectButton);
            }
        }

        private void ApplyColours()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string status = Convert.ToString(
                    row.Cells["Review Status"].Value).Trim();

                // Approve button:
                // Blue after approval, green otherwise
                if (status.Equals("Approved",
                    StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["Approve"].Style.BackColor =
                        Color.Blue;
                    row.Cells["Approve"].Style.ForeColor =
                        Color.White;
                    row.Cells["Approve"].Style.SelectionBackColor =
                        Color.DarkBlue;
                }
                else
                {
                    row.Cells["Approve"].Style.BackColor =
                        Color.Green;
                    row.Cells["Approve"].Style.ForeColor =
                        Color.White;
                    row.Cells["Approve"].Style.SelectionBackColor =
                        Color.DarkGreen;
                }

                // Reject button stays red
                row.Cells["Reject"].Style.BackColor = Color.Red;
                row.Cells["Reject"].Style.ForeColor = Color.White;
                row.Cells["Reject"].Style.SelectionBackColor =
                    Color.DarkRed;

                row.Cells["Approve"].Style.SelectionForeColor =
                    Color.White;
                row.Cells["Reject"].Style.SelectionForeColor =
                    Color.White;

                // Review status cell colours
                if (status.Equals("Approved",
                    StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["Review Status"].Style.BackColor =
                        Color.LightBlue;
                    row.Cells["Review Status"].Style.ForeColor =
                        Color.DarkBlue;
                }
                else if (status.Equals("Rejected",
                    StringComparison.OrdinalIgnoreCase))
                {
                    row.Cells["Review Status"].Style.BackColor =
                        Color.LightCoral;
                    row.Cells["Review Status"].Style.ForeColor =
                        Color.DarkRed;
                }
            }
        }

        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName =
                dataGridView1.Columns[e.ColumnIndex].Name;

            if (columnName != "Approve" &&
                columnName != "Reject")
                return;

            DataGridViewRow selectedRow =
                dataGridView1.Rows[e.RowIndex];

            string observationId = Convert.ToString(
                selectedRow.Cells["Observation ID"].Value);

            string newStatus =
                columnName == "Approve"
                ? "Approved"
                : "Rejected";

            DialogResult result = MessageBox.Show(
                "Do you want to mark this observation as " +
                newStatus + "?",
                "Confirm Status",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            UpdateObservationStatus(observationId, newStatus);
        }

        private void UpdateObservationStatus(
            string observationId, string newStatus)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string query = @"
                    UPDATE dbo.advisor_observation
                    SET review_status = @status
                    WHERE observation_id = @id";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@status",
                        SqlDbType.VarChar, 20).Value = newStatus;

                    cmd.Parameters.Add("@id",
                        SqlDbType.Int).Value =
                        Convert.ToInt32(observationId);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show(
                            "Observation " + newStatus +
                            " successfully.",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Observation not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating status: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            LoadAdvisorObservations();
        }
    }
}
