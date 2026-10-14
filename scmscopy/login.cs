
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Check empty fields
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please enter username and password.");
                textBox1.Focus();
                return;
            }

            // ADMIN LOGIN
            if (textBox1.Text.Trim() == "admin" &&
                textBox2.Text == "pwd")
            {
                Program.uid = "admin";

                MessageBox.Show("Login successful");

                admin_home obj = new admin_home();
                obj.Show();
                this.Hide();

                return;
            }

            // STAFF LOGIN
            try
            {
                using (SqlConnection con =
                    new SqlConnection(Program.con.ConnectionString))
                {
                    string query = @"
                        SELECT username, usertype
                        FROM user_details
                        WHERE username = @username
                          AND password = @password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@username", SqlDbType.VarChar, 100)
                            .Value = textBox1.Text.Trim();

                        cmd.Parameters.Add("@password", SqlDbType.VarChar, 100)
                            .Value = textBox2.Text;

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Store logged-in username
                                Program.uid = dr["username"].ToString();

                                // Get user type
                                string userType = dr["usertype"]
                                    .ToString()
                                    .Trim()
                                    .ToLower();

                                Form home = null;

                                if (userType == "mechanic")
                                {
                                    home = new mechanic_home();
                                }
                                else if (userType == "store keeper")
                                {
                                    home = new storekeeper_home();
                                }
                                else if (userType == "service advisor")
                                {
                                    home = new serviceadvisor_home();
                                }

                                if (home != null)
                                {
                                    MessageBox.Show("Login successful");

                                    home.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    Program.uid = "";

                                    MessageBox.Show(
                                        "Invalid user type: " + userType);
                                }
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Username and password mismatch.");

                                textBox1.Clear();
                                textBox2.Clear();
                                textBox1.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Login error:\n" + ex.Message);
            }
        }
    }
}