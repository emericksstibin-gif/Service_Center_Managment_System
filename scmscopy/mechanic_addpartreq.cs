
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class mechanic_addpartreq : Form
    {
        SqlConnection con = Program.con;

        public mechanic_addpartreq()
        {
            InitializeComponent();
        }

        // Load VID values into ComboBox
        private void LoadVID()
        {
            try
            {
                string query = @"
                    SELECT DISTINCT vid
                    FROM complaint_details
                    ORDER BY vid";

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
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading VID: " + ex.Message);
            }
        }

        // Generate the next Request ID for display
        private void GenerateRequestID()
        {
            try
            {
                string query = @"
                    SELECT ISNULL(MAX(request_id), 99) + 1
                    FROM dbo.parts_request";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();

                    textBox9.Text =
                        cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error generating Request ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // Load saved parts requests into DataGridView
        private void LoadPartsRequest()
        {
            try
            {
                string query = @"
                    SELECT
                        request_id,
                        vid,
                        mechanic,
                        part_name,
                        quantity,
                        remark
                    FROM dbo.parts_request
                    ORDER BY request_id DESC";

                using (SqlDataAdapter da =
                    new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                }

                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.RowHeadersVisible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading parts requests: " + ex.Message);
            }
        }
       

        // Form Load event
        private void mechanic_addpartreq_Load(
            object sender, EventArgs e)
        {
            LoadVID();
            GenerateRequestID();
            LoadPartsRequest();
            
        }

        // Search button: load service request details using VID
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1 ||
                comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select VID first.");
                return;
            }

            string vid = comboBox1.SelectedValue.ToString().Trim();

            try
            {
                string query = @"
                    SELECT
                        c.vid,
                        c.reg_date,
                        d.customer_name,
                        d.vechicle,
                        d.vechicle_type,
                        d.vechicle_model,
                        d.contact,
                        c.complaint,
                        c.mechanic,
                        c.complaint_status
                    FROM complaint_details AS c
                    LEFT JOIN customer_details AS d
                        ON c.vid = d.vid
                    WHERE c.vid = @vid";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@vid", SqlDbType.VarChar, 10).Value = vid;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Customer and vehicle details
                            if (dr["reg_date"] != DBNull.Value)
                            {
                                dateTimePicker3.Value =
                                    Convert.ToDateTime(dr["reg_date"]);
                            }

                            textBox6.Text =
                                dr["customer_name"].ToString();

                            textBox3.Text =
                                dr["vechicle"].ToString();

                            textBox4.Text =
                                dr["vechicle_type"].ToString();

                            textBox5.Text =
                                dr["vechicle_model"].ToString();

                            textBox7.Text =
                                dr["contact"].ToString();

                            // Service request details
                            textBox1.Text =
                                dr["complaint"].ToString();

                            textBox2.Text =
                                dr["mechanic"].ToString();

                            textBox8.Text =
                                dr["complaint_status"].ToString();
                        }
                        else
                        {
                            MessageBox.Show(
                                "No service request found for " + vid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error searching service request: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // Save Parts Request button
        private void button1_Click(object sender, EventArgs e)
        {
            // Validate VID
            if (comboBox1.SelectedIndex == -1 ||
                comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select VID first.");
                return;
            }

            // Make sure service request details have been searched
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show(
                    "Please search for a service request first.");
                return;
            }

            // Validate Part Name
            if (comboBox2.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Please select Part Name.");
                return;
            }

            // Validate Quantity
            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show(
                    "Please enter a quantity greater than zero.");
                return;
            }

            // Read values from controls
            string vid =
                comboBox1.SelectedValue.ToString().Trim();

            string mechanic = textBox2.Text.Trim();
            string partName = comboBox2.Text.Trim();
            int quantity = Convert.ToInt32(numericUpDown1.Value);
            string remark = textBox10.Text.Trim();

            try
            {
                // request_id is IDENTITY, so do not insert it manually
                string query = @"
                    INSERT INTO dbo.parts_request
                        (vid, mechanic, part_name, quantity, remark)
                    VALUES
                        (@vid, @mechanic, @part_name, @quantity, @remark);

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newRequestId;

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    cmd.Parameters.Add(
                        "@vid", SqlDbType.VarChar, 10).Value = vid;

                    cmd.Parameters.Add(
                        "@mechanic", SqlDbType.VarChar, 50).Value =
                        mechanic;

                    cmd.Parameters.Add(
                        "@part_name", SqlDbType.VarChar, 100).Value =
                        partName;

                    cmd.Parameters.Add(
                        "@quantity", SqlDbType.Int).Value = quantity;

                    cmd.Parameters.Add(
                        "@remark", SqlDbType.VarChar, 255).Value =
                        string.IsNullOrWhiteSpace(remark)
                            ? (object)DBNull.Value
                            : remark;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    newRequestId =
                        Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Display the ID that was actually saved
                textBox9.Text = newRequestId.ToString();

                // Show success message
                MessageBox.Show(
                    "Parts request saved successfully!\nRequest ID: "
                    + newRequestId);

                // Refresh grid to show the newly saved request
                LoadPartsRequest();

                // Clear the part-request input controls
                comboBox2.SelectedIndex = -1;
                numericUpDown1.Value = 0;
                textBox10.Clear();

                // Generate the next Request ID
                GenerateRequestID();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving parts request: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void panel2_Paint(
            object sender, PaintEventArgs e)
        {
        }

        private void panel3_Paint(
            object sender, PaintEventArgs e)
        {
        }
    }
}
