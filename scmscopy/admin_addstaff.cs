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
    public partial class admin_addstaff : Form
    {
        SqlConnection con = Program.con;
        public admin_addstaff()
        {
            InitializeComponent();
        }
        public void get_userid()
        {
            con.Open();
            string query = "select isnull(max(uid),100)+1 from staff_details";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                textBox2.Text = dr[0].ToString();
            }
            con.Close();
        }

        private void admin_addstaff_Load(object sender, EventArgs e)
        {
            get_userid();
            get_staffid();
        }
        public void get_staffid()
        {
            con.Open();
            string query = "select isnull(max(sid),100)+1 from staff_details";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                textBox1.Text = dr[0].ToString();
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            string query = "insert into user_details values(" + textBox2.Text + ",'" + textBox7.Text + "','" + textBox8.Text + "','" + comboBox2.Text + "')";
            string query2 = "insert into staff_details values(" + textBox1.Text + "," + textBox2.Text + ",'" + textBox3.Text + "','" + comboBox1.Text + "','" + textBox6.Text + "','" + textBox4.Text + "','" + textBox5.Text + "','" + comboBox2.Text + "','" + comboBox3.Text + "')";
            SqlCommand cmd1 = new SqlCommand(query, con);
            SqlCommand cmd2 = new SqlCommand(query2, con);
            if (cmd1.ExecuteNonQuery() > 0)
            {
                if (cmd2.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("adding successful");
                }
                else
                {
                    MessageBox.Show("adding failure");
                }
                con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
        }
    }
}
