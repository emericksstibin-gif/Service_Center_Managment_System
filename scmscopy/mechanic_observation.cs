
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class mechanic_observation : Form
    {
        public mechanic_observation()
        {
            InitializeComponent();
        }

        private void mechanic_observation_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true; // Customer name
            textBox2.ReadOnly = true; // Complaint
            textBox3.ReadOnly = true; // Diagnosis
            textBox4.ReadOnly = true; // Vehicle model

            textBox5.Multiline = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            LoadVehicleIDs();
        }

        // Load VID values into ComboBox
        private void LoadVehicleIDs()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(Program.con.ConnectionString))
                {
                    string query = @"
                        SELECT DISTINCT CAST(vid AS VARCHAR(10)) AS vid
                        FROM dbo.complaint_details
                        WHERE vid IS NOT NULL
                        ORDER BY CAST(vid AS VARCHAR(10));";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox1.DataSource = null;
                        comboBox1.DisplayMember = "vid";
                        comboBox1.ValueMember = "vid";
                        comboBox1.DataSource = dt;
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading VID:\n" + ex.Message);
            }
        }

        // Validate selected VID
        private bool TryGetVID(out string vid)
        {
            vid = "";

            if (comboBox1.SelectedIndex < 0 ||
                comboBox1.SelectedValue == null ||
                comboBox1.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Please select a VID.");
                return false;
            }

            vid = comboBox1.SelectedValue.ToString().Trim();

            if (string.IsNullOrWhiteSpace(vid))
            {
                MessageBox.Show("Please select a valid VID.");
                return false;
            }

            return true;
        }

        // Search button
        private void button2_Click(object sender, EventArgs e)
        {
            string vid;

            if (!TryGetVID(out vid))
                return;

            ClearDetails();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(Program.con.ConnectionString))
                {
                    string query = @"
                        SELECT
                            c.customer_name,
                            c.vechicle_model,
                            d.complaint,
                            d.diagnosis
                        FROM dbo.complaint_details AS d
                        LEFT JOIN dbo.customer_details AS c
                            ON CAST(d.vid AS VARCHAR(10))
                             = CAST(c.vid AS VARCHAR(10))
                        WHERE CAST(d.vid AS VARCHAR(10)) = @vid;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@vid", SqlDbType.VarChar, 10).Value = vid;

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                textBox1.Text =
                                    dr["customer_name"] == DBNull.Value
                                    ? ""
                                    : dr["customer_name"].ToString();

                                textBox4.Text =
                                    dr["vechicle_model"] == DBNull.Value
                                    ? ""
                                    : dr["vechicle_model"].ToString();

                                textBox2.Text =
                                    dr["complaint"] == DBNull.Value
                                    ? ""
                                    : dr["complaint"].ToString();

                                textBox3.Text =
                                    dr["diagnosis"] == DBNull.Value
                                    ? ""
                                    : dr["diagnosis"].ToString();
                            }
                            else
                            {
                                MessageBox.Show(
                                    "No complaint found for VID: " + vid);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error searching details:\n" + ex.Message);
            }
        }

        // Clear displayed details
        private void ClearDetails()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // Save mechanic observation
        private void button1_Click(object sender, EventArgs e)
        {
            string vid;

            if (!TryGetVID(out vid))
                return;

            // Require a successful search before saving
            if (string.IsNullOrWhiteSpace(textBox2.Text) &&
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show(
                    "Please select a VID and click Search first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show(
                    "Please enter mechanic observation details.");
                textBox5.Focus();
                return;
            }

            string staffName = Program.uid;

            if (string.IsNullOrWhiteSpace(staffName))
            {
                MessageBox.Show(
                    "Mechanic login information is missing. Please log in again.");
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(Program.con.ConnectionString))
                {
                    string query = @"
                        INSERT INTO dbo.mechanic_observation
                            (vid, staff_name, observation_details)
                        VALUES
                            (@vid, @staff_name, @observation_details);";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // VID is text, for example V5000
                        cmd.Parameters.Add(
                            "@vid", SqlDbType.VarChar, 10).Value = vid;

                        cmd.Parameters.Add(
                            "@staff_name", SqlDbType.VarChar, 100).Value =
                            staffName.Trim();

                        cmd.Parameters.Add(
                            "@observation_details",
                            SqlDbType.VarChar, 1000).Value =
                            textBox5.Text.Trim();

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show(
                    "Mechanic observation saved successfully!");

                comboBox1.SelectedIndex = -1;
                ClearDetails();
                textBox5.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving mechanic observation:\n" + ex.Message);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // No code required here.
        }
    }
}
