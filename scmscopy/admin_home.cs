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
using System.Windows.Forms.DataVisualization.Charting;

namespace scmscopy
{
    public partial class admin_home : Form
    {
        SqlConnection con = Program.con;
        public admin_home()
        {
            InitializeComponent();
            totalostaff();
            totalmechanic();
            totalserviceadvisor();
            totalstorekeeper();
        }

        public void LoadStaffPieChart()
        {
            try
            {
                string query = @"
            SELECT LOWER(staff_type) AS staff_type,
                   COUNT(*) AS TotalStaff
            FROM staff_details
            WHERE LOWER(staff_type) IN
            ('service advisor', 'mechanic', 'store keeper')
            GROUP BY LOWER(staff_type)";

                DataTable dt = new DataTable();

                using (SqlDataAdapter da =
                       new SqlDataAdapter(query, Program.con))
                {
                    da.Fill(dt);
                }

                chart2.Series.Clear();
                chart2.ChartAreas.Clear();
                chart2.Legends.Clear();
                chart2.Titles.Clear();

                chart2.ChartAreas.Add(new ChartArea("StaffPieArea"));
                chart2.Legends.Add(new Legend("StaffLegend"));
                chart2.Titles.Add("Staff Distribution");

                Series series = new Series("Staff");
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = true;
                series.Label = "#VALX: #VAL";
                series.LegendText = "#VALX";

                chart2.Series.Add(series);

                string[] staffTypes =
                {
            "service advisor",
            "mechanic",
            "store keeper"
        };

                string[] displayNames =
                {
            "Service Advisors",
            "Mechanics",
            "Store Keepers"
        };

                Color[] pieColors =
                {
            Color.DodgerBlue,
            Color.SeaGreen,
            Color.Orange
        };

                foreach (int i in new int[] { 0, 1, 2 })
                {
                    int count = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["staff_type"].ToString()
                            .Equals(staffTypes[i],
                            StringComparison.OrdinalIgnoreCase))
                        {
                            count = Convert.ToInt32(row["TotalStaff"]);
                            break;
                        }
                    }

                    int index = series.Points.AddXY(displayNames[i], count);
                    series.Points[index].Color = pieColors[i];
                }

                chart2.Series[0]["PieLabelStyle"] = "Outside";
                chart2.Series[0]["PieLineColor"] = "Black";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void LoadStaffBarChart()
        {
            try
            {
                string query = @"
    SELECT LOWER(staff_type) AS staff_type,
           COUNT(*) AS TotalStaff
    FROM staff_details
    WHERE LOWER(staff_type) IN
    ('service advisor', 'mechanic', 'store keeper')
    GROUP BY LOWER(staff_type)";

                DataTable dt = new DataTable();

                using (SqlDataAdapter da =
                       new SqlDataAdapter(query, Program.con))
                {
                    da.Fill(dt);
                }

                chart1.Series.Clear();
                chart1.ChartAreas.Clear();
                chart1.Legends.Clear();
                chart1.Titles.Clear();

                chart1.ChartAreas.Add(new ChartArea("StaffArea"));
                chart1.Titles.Add("Staff Details");

                Series series = new Series("Total Staff");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;

                chart1.Series.Add(series);

                string[] staffTypes =
                {
            "service advisor",
            "mechanic",
            "store keeper"
        };

                string[] displayNames =
                {
            "Service Advisors",
            "Mechanics",
            "Store Keepers"
        };

                Color[] barColors =
                {
            Color.DodgerBlue,
            Color.SeaGreen,
            Color.Orange
        };

                foreach (int i in new int[] { 0, 1, 2 })
                {
                    int count = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["staff_type"].ToString()
                            .Equals(staffTypes[i],
                            StringComparison.OrdinalIgnoreCase))
                        {
                            count = Convert.ToInt32(row["TotalStaff"]);
                            break;
                        }
                    }

                    int index = series.Points.AddXY(displayNames[i], count);
                    series.Points[index].Color = barColors[i];
                }

                chart1.ChartAreas[0].AxisX.Title = "Staff Category";
                chart1.ChartAreas[0].AxisY.Title = "Number of Staff";
                chart1.ChartAreas[0].AxisY.Minimum = 0;
                chart1.ChartAreas[0].AxisY.Interval = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public void totalostaff()
        {
            string query = "select count(*) from staff_details";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            int count = (int)cmd.ExecuteScalar();
            label9.Text = count.ToString();
            con.Close();
        }
        public void totalmechanic()
        {
            string query = "SELECT COUNT(*) FROM staff_details WHERE staff_type = 'mechanic'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            int count = (int)cmd.ExecuteScalar();
            label10.Text = count.ToString();

            con.Close();
        }
        public void totalserviceadvisor()
        {
            string query = "SELECT COUNT(*) FROM staff_details WHERE staff_type = 'service advisor'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            int count = (int)cmd.ExecuteScalar();
            label12.Text = count.ToString();

            con.Close();
        }
        public void totalstorekeeper()
        {
            string query = "SELECT COUNT(*) FROM staff_details WHERE staff_type = 'store keeper'";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();

            int count = (int)cmd.ExecuteScalar();
            label14.Text = count.ToString();

            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serviceadvisor_dash obj = new serviceadvisor_dash();
            obj.Show();
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            login obj = new login();
            obj.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            storekeeper_dash obj = new storekeeper_dash();
            obj.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mechanic_dash obj = new mechanic_dash();
            obj.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            admin_addstaff obj = new admin_addstaff();
            obj.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            admin_viewstaff obj = new admin_viewstaff();
            obj.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            admin_updatestaff obj = new admin_updatestaff();
            obj.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            admin_deletestaff obj = new admin_deletestaff();
            obj.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel5_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void admin_home_Load(object sender, EventArgs e)
        {
            LoadStaffBarChart();
            LoadStaffPieChart();
        }

        //private void label9_Click(object sender, EventArgs e)
        //{

        //}

        // private void button12_Click(object sender, EventArgs e)
        // {

        //}
    }
}
