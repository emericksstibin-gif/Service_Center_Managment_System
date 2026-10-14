using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace scmscopy
{
    public partial class mechanic_requestviewhome : Form
    {
        SqlConnection con = Program.con;
        public mechanic_requestviewhome()
        {
            InitializeComponent();
            load_vid();
        }
        public void load_vid()
        {
            string query = "SELECT vid FROM complaint_details";

            con.Open();

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();

            comboBox1.Items.Clear();

            while (dr.Read())
            {
                comboBox1.Items.Add(dr["vid"].ToString());
            }

            dr.Close();
            con.Close();


        }
        public void getAllComplaints()
        {
            string query = "SELECT * FROM complaint_details";

            SqlDataAdapter sda = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();
            sda.Fill(dt);

            dataGridView1.DataSource = dt;
        }
        //private void button1_Click(object sender, EventArgs e)
        //{
            
        //}

        private void mechanic_requestviewhome_Load(object sender, EventArgs e)
        {
            getAllComplaints();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date;

            string query = @"SELECT *
                     FROM complaint_details
                     WHERE reg_date BETWEEN @fromDate AND @toDate";

            SqlDataAdapter sda = new SqlDataAdapter(query, con);

            sda.SelectCommand.Parameters.Add("@fromDate", SqlDbType.Date)
                .Value = fromDate;

            sda.SelectCommand.Parameters.Add("@toDate", SqlDbType.Date)
                .Value = toDate;

            DataTable dt = new DataTable();
            sda.Fill(dt);

            dataGridView1.DataSource = dt;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select VID");
                return;
            }

            string query = @"
SELECT
    c.cid,
    c.vechicle,
    c.vechicle_type,
    c.vechicle_model,
    c.customer_name,
    c.contact,
    c.email,
    d.reg_date,
    d.complaint,
    d.diagnosis,
    d.complaint_status,
    d.advisor,
    d.mechanic
FROM customer_details c
INNER JOIN complaint_details d
    ON c.vid = d.vid
WHERE d.vid = @vid";

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@vid", comboBox1.Text);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    textBox2.Text = dr["cid"].ToString();
                    textBox3.Text = dr["vechicle"].ToString();
                    textBox4.Text = dr["vechicle_type"].ToString();
                    textBox5.Text = dr["vechicle_model"].ToString();
                    textBox6.Text = dr["customer_name"].ToString();
                    textBox7.Text = dr["contact"].ToString();
                    textBox8.Text = dr["email"].ToString();

                    dateTimePicker3.Value = Convert.ToDateTime(dr["reg_date"]);

                    textBox9.Text = dr["complaint"].ToString();
                    textBox10.Text = dr["diagnosis"].ToString();
                    textBox11.Text = dr["complaint_status"].ToString();
                    textBox12.Text = dr["mechanic"].ToString();
                }
                else
                {
                    MessageBox.Show("No record found");
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = @"
UPDATE complaint_details
SET
    diagnosis = @diagnosis,
    complaint_status = @status,
    observation = @observation
WHERE vid = @vid";
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@diagnosis", textBox13.Text);
            cmd.Parameters.AddWithValue("@status", comboBox2.Text);
            cmd.Parameters.AddWithValue("@observation", textBox15.Text);
            cmd.Parameters.AddWithValue("@vid", comboBox1.Text);

            con.Open();

            int result = cmd.ExecuteNonQuery();

            con.Close();

            if (result > 0)
            {
                MessageBox.Show("Updated successfully!");
            }
            else
            {
                MessageBox.Show("No record found for this VID.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text="";
            textBox15.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
        }
    }
}
