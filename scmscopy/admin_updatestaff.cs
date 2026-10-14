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
    public partial class admin_updatestaff : Form
    {
        SqlConnection con = Program.con;
        public admin_updatestaff()
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

        private void button1_Click(object sender, EventArgs e)
        {
            string query = "select * from staff_details where sid='" + comboBox1.Text + "'";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                textBox1.Text = dr[2].ToString();
                textBox2.Text = dr[4].ToString();
                comboBox2.Text = dr[3].ToString();
                textBox3.Text = dr[5].ToString();
                textBox4.Text = dr[6].ToString();
                comboBox3.Text = dr[7].ToString();
                comboBox4.Text = dr[8].ToString();
                textBox5.Text = dr[1].ToString();
            }
            con.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string query = "update staff_details set staff_name='" + textBox1.Text + "',contact='" + textBox2.Text + "',email='" + textBox3.Text + "',address='" + textBox4.Text + "',staff_type='" + comboBox3.Text + "',status='" + comboBox4.Text + "'where sid='" + comboBox1.Text + "'";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            if (cmd.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("details updated successfully");
            }
            con.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
        }
    }
}
