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
    public partial class admin_deletestaff : Form
    {
        SqlConnection con = Program.con;
        public admin_deletestaff()
        {
            InitializeComponent();
            fillcombo();
        }
        public void fillcombo()
        {
            comboBox1.Items.Clear();
            string query = "select sid from staff_details";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr[0].ToString());
            }
            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            string query = "select * from staff_details where sid='" + comboBox1.Text + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                textBox3.Text = dr[2].ToString();
                textBox1.Text = dr[5].ToString();
                textBox2.Text = dr[6].ToString();
            }
            con.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            string query = "delete from staff_details where sid ='" + comboBox1.Text + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("student details is Deleted");
                textBox3.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                comboBox1.Text = "";

            }
            else
            {
                MessageBox.Show("error while deleteing student data");
            }
            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
        }
    }
}
