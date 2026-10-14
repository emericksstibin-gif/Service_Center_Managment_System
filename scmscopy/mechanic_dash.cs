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
    public partial class mechanic_dash : Form
    {
        SqlConnection con = Program.con;
        public mechanic_dash()
        {
            InitializeComponent();
        }
       
        public void LoadMechanicLineChart()
        {
            try
            {
                string query = @"
            SELECT RequestStatus, COUNT(*) AS TotalRequests
            FROM
            (
                SELECT
                    CASE
                        WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'assigned'
                            THEN 'Assigned'
                        WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'in progress'
                            THEN 'In Progress'
                        WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'completed'
                            THEN 'Completed'
                    END AS RequestStatus
                FROM complaint_details
                WHERE LOWER(LTRIM(RTRIM(complaint_status)))
                    IN ('assigned', 'in progress', 'completed')
            ) AS R
            GROUP BY RequestStatus";

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

                ChartArea area = new ChartArea("LineArea");
                chart1.ChartAreas.Add(area);

                chart1.Titles.Add("Mechanic Service Requests");

                Series series = new Series("Requests");
                series.ChartType = SeriesChartType.Line;
                series.BorderWidth = 3;
                series.MarkerStyle = MarkerStyle.Circle;
                series.MarkerSize = 9;
                series.IsValueShownAsLabel = true;
                series.Color = Color.DodgerBlue;

                chart1.Series.Add(series);

                string[] statuses =
                {
            "Assigned",
            "In Progress",
            "Completed"
        };

                foreach (string status in statuses)
                {
                    int count = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["RequestStatus"].ToString() == status)
                        {
                            count = Convert.ToInt32(row["TotalRequests"]);
                            break;
                        }
                    }

                    series.Points.AddXY(status, count);
                }

                chart1.ChartAreas[0].AxisX.Title = "Request Status";
                chart1.ChartAreas[0].AxisY.Title = "Number of Requests";
                chart1.ChartAreas[0].AxisY.Minimum = 0;
                chart1.ChartAreas[0].AxisY.Interval = 1;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading mechanic line chart: " + ex.Message);
            }
        }

        public void mechanicCompletedToday()
        {
            try
            {
                string query = @"SELECT COUNT(*)
                         FROM complaint_details
                         WHERE complaint_status = @status
                         AND CAST(reg_date AS DATE) = @today";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    cmd.Parameters.AddWithValue("@status", "Completed");
                    cmd.Parameters.AddWithValue("@today", DateTime.Today);

                    if (Program.con.State == ConnectionState.Closed)
                        Program.con.Open();

                    label16.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (Program.con.State == ConnectionState.Open)
                    Program.con.Close();
            }
        }


        public void mechanicInProgressRequests()
        {
            try
            {
                string query = @"SELECT COUNT(*)
                         FROM complaint_details
                         WHERE complaint_status = @status";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    cmd.Parameters.AddWithValue("@status", "In Progress");

                    if (Program.con.State == ConnectionState.Closed)
                        Program.con.Open();

                    label14.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (Program.con.State == ConnectionState.Open)
                    Program.con.Close();
            }
        }


        public void mechanicAssignedRequests()
        {
            try
            {
                string query = @"SELECT COUNT(*)
                         FROM complaint_details
                         WHERE complaint_status = @status";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    cmd.Parameters.AddWithValue("@status", "Assigned");

                    if (Program.con.State == ConnectionState.Closed)
                        Program.con.Open();

                    label12.Text = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (Program.con.State == ConnectionState.Open)
                    Program.con.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serviceadvisor_dash obj = new serviceadvisor_dash();
            obj.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            storekeeper_dash obj = new storekeeper_dash();
            obj.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            login obj = new login();
            obj.Show();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            mechanicdash_viewrequest obj = new mechanicdash_viewrequest();
            obj.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            mechanicdash_observation obj = new mechanicdash_observation();
            obj.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mechanicdash_viewpaersreq obj = new mechanicdash_viewpaersreq();
            obj.Show();
        }

        private void mechanic_dash_Load(object sender, EventArgs e)
        {
            
            mechanicAssignedRequests();
            mechanicInProgressRequests();
            mechanicCompletedToday();
            LoadMechanicLineChart();
            
            
        }
    }
}
