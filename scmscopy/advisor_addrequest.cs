
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class advisor_addrequest : Form
    {
        SqlConnection con = Program.con;

        public advisor_addrequest()
        {
            InitializeComponent();
        }

        // ---------------- GENERATE CUSTOMER ID ----------------

        public void get_customerid()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string query = @"
                    SELECT ISNULL(
                        MAX(TRY_CAST(SUBSTRING(cid, 2, LEN(cid)) AS INT)),
                        4999
                    ) + 1
                    FROM dbo.customer_details
                    WHERE cid LIKE 'C%'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    object result = cmd.ExecuteScalar();
                    textBox1.Text = "C" + Convert.ToString(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating customer ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ---------------- GENERATE VEHICLE ID ----------------

        public void get_vehicleid()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string query = @"
                    SELECT ISNULL(
                        MAX(TRY_CAST(SUBSTRING(vid, 2, LEN(vid)) AS INT)),
                        4999
                    ) + 1
                    FROM dbo.complaint_details
                    WHERE vid LIKE 'V%'";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    object result = cmd.ExecuteScalar();
                    textBox14.Text = "V" + Convert.ToString(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating vehicle ID: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        // ---------------- FORM LOAD ----------------

        private void advisor_addrequest_Load(object sender, EventArgs e)
        {
            get_customerid();
            get_vehicleid();
            loadAdvisorNames();
            loadmechanicNames();
        }

        // ---------------- LOAD SERVICE ADVISORS ----------------

        private void loadAdvisorNames()
        {
            try
            {
                string query = @"
                    SELECT staff_name
                    FROM dbo.staff_details
                    WHERE staff_type = 'service advisor'
                    ORDER BY staff_name";

                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox3.DataSource = dt;
                    comboBox3.DisplayMember = "staff_name";
                    comboBox3.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading advisors: " + ex.Message);
            }
        }

        // ---------------- LOAD MECHANICS ----------------

        private void loadmechanicNames()
        {
            try
            {
                string query = @"
                    SELECT staff_name
                    FROM dbo.staff_details
                    WHERE staff_type = 'mechanic'
                    ORDER BY staff_name";

                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    comboBox4.DataSource = dt;
                    comboBox4.DisplayMember = "staff_name";
                    comboBox4.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading mechanics: " + ex.Message);
            }
        }

        // ---------------- SAVE CUSTOMER AND SERVICE REQUEST ----------------

        private void button1_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                MessageBox.Show("Please select Assign.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox14.Text))
            {
                MessageBox.Show("Customer ID or Vehicle ID is missing.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Please enter the customer name and complaint.");
                return;
            }

            decimal amount;

            if (!decimal.TryParse(textBox5.Text.Trim(), out amount))
            {
                MessageBox.Show("Please enter a valid amount.");
                textBox5.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(comboBox3.Text) ||
                string.IsNullOrWhiteSpace(comboBox4.Text))
            {
                MessageBox.Show("Please select a service advisor and mechanic.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(
                @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;"))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Save customer details INCLUDING vid
                        string query1 = @"
                            INSERT INTO dbo.customer_details
                            (cid, vid, vechicle, vechicle_type, vechicle_model,
                             customer_name, contact, email)
                            VALUES
                            (@cid, @vid, @vehicle, @vehicle_type, @vehicle_model,
                             @customer_name, @contact, @email)";

                        using (SqlCommand cmd1 =
                            new SqlCommand(query1, connection, transaction))
                        {
                            cmd1.Parameters.Add("@cid", SqlDbType.VarChar, 10)
                                .Value = textBox1.Text.Trim();

                            cmd1.Parameters.Add("@vid", SqlDbType.VarChar, 10)
                                .Value = textBox14.Text.Trim();

                            cmd1.Parameters.Add("@vehicle", SqlDbType.VarChar, 10)
                                .Value = comboBox1.Text.Trim();

                            cmd1.Parameters.Add("@vehicle_type", SqlDbType.VarChar, 10)
                                .Value = comboBox2.Text.Trim();

                            cmd1.Parameters.Add("@vehicle_model", SqlDbType.VarChar, 10)
                                .Value = textBox3.Text.Trim();

                            cmd1.Parameters.Add("@customer_name", SqlDbType.VarChar, 10)
                                .Value = textBox6.Text.Trim();

                            cmd1.Parameters.Add("@contact", SqlDbType.VarChar, 12)
                                .Value = textBox8.Text.Trim();

                            cmd1.Parameters.Add("@email", SqlDbType.VarChar, 30)
                                .Value = textBox7.Text.Trim();

                            cmd1.ExecuteNonQuery();
                        }

                        // 2. Save complaint/service request with the SAME vid
                        string query2 = @"
                            INSERT INTO dbo.complaint_details
                            (vid, reg_date, complaint, diagnosis,
                             complaint_status, amount, advisor, mechanic)
                            VALUES
                            (@vid, @reg_date, @complaint, @diagnosis,
                             @status, @amount, @advisor, @mechanic)";

                        using (SqlCommand cmd2 =
                            new SqlCommand(query2, connection, transaction))
                        {
                            cmd2.Parameters.Add("@vid", SqlDbType.VarChar, 10)
                                .Value = textBox14.Text.Trim();

                            cmd2.Parameters.Add("@reg_date", SqlDbType.Date)
                                .Value = dateTimePicker1.Value.Date;

                            cmd2.Parameters.Add("@complaint", SqlDbType.VarChar, 100)
                                .Value = textBox12.Text.Trim();

                            cmd2.Parameters.Add("@diagnosis", SqlDbType.VarChar, 100)
                                .Value = textBox10.Text.Trim();

                            cmd2.Parameters.Add("@status", SqlDbType.VarChar, 15)
                                .Value = "Assigned";

                            SqlParameter amountParameter =
                                cmd2.Parameters.Add("@amount", SqlDbType.Decimal);

                            amountParameter.Precision = 10;
                            amountParameter.Scale = 2;
                            amountParameter.Value = amount;

                            cmd2.Parameters.Add("@advisor", SqlDbType.VarChar, 30)
                                .Value = comboBox3.Text.Trim();

                            cmd2.Parameters.Add("@mechanic", SqlDbType.VarChar, 15)
                                .Value = comboBox4.Text.Trim();

                            cmd2.ExecuteNonQuery();
                        }

                        // Commit both inserts together
                        transaction.Commit();

                        MessageBox.Show(
                            "Customer and Service Request Saved Successfully.");

                        // Clear form
                        textBox3.Clear();
                        textBox6.Clear();
                        textBox5.Clear();
                        textBox7.Clear();
                        textBox8.Clear();
                        textBox10.Clear();
                        textBox12.Clear();

                        comboBox1.SelectedIndex = -1;
                        comboBox2.SelectedIndex = -1;
                        comboBox3.SelectedIndex = -1;
                        comboBox4.SelectedIndex = -1;

                        checkBox1.Checked = false;

                        // Generate new IDs for the next request
                        get_customerid();
                        get_vehicleid();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                            // Transaction may already have ended.
                        }

                        MessageBox.Show(
                            "Error saving customer or service request:\n" +
                            ex.Message);
                    }
                }
            }
        }
    }
}
