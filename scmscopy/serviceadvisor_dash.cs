
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace scmscopy
{
    public partial class serviceadvisor_dash : Form
    {
        public serviceadvisor_dash()
        {
            InitializeComponent();
            fillinfo();
        }
        public void LoadServiceRequestBarChart()
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

                chart2.Series.Clear();
                chart2.ChartAreas.Clear();
                chart2.Legends.Clear();
                chart2.Titles.Clear();

                ChartArea area = new ChartArea("BarArea");
                chart2.ChartAreas.Add(area);

                chart2.Titles.Add("Service Requests by Status");

                Series series = new Series("Requests");
                series.ChartType = SeriesChartType.Bar;
                series.IsValueShownAsLabel = true;
                series.Label = "#VAL";
                series.IsXValueIndexed = true;

                chart2.Series.Add(series);

                string[] statuses =
                {
            "Assigned",
            "In Progress",
            "Completed"
        };

                Color[] colors =
                {
            Color.Orange,
            Color.DodgerBlue,
            Color.SeaGreen
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

                    int index = series.Points.AddXY(status, count);
                    series.Points[index].Color =
                        colors[Array.IndexOf(statuses, status)];
                }

                chart2.ChartAreas[0].AxisX.Title = "Request Status";
                chart2.ChartAreas[0].AxisY.Title = "Number of Requests";
                chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart2.ChartAreas[0].AxisY.Minimum = 0;
                chart2.ChartAreas[0].AxisY.Interval = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bar chart: " + ex.Message);
            }
        }

        public void LoadServiceRequestDoughnutChart()
        {
            try
            {
                string query = @"
            SELECT
                CASE
                    WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'assigned'
                        THEN 'Assigned'
                    WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'in progress'
                        THEN 'In Progress'
                    WHEN LOWER(LTRIM(RTRIM(complaint_status))) = 'completed'
                        THEN 'Completed'
                END AS RequestStatus,
                COUNT(*) AS TotalRequests
            FROM complaint_details
            WHERE LOWER(LTRIM(RTRIM(complaint_status)))
                IN ('assigned', 'in progress', 'completed')
            GROUP BY
                LOWER(LTRIM(RTRIM(complaint_status)) )";

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

                chart1.ChartAreas.Add(new ChartArea("DoughnutArea"));
                chart1.Legends.Add(new Legend("StatusLegend"));
                chart1.Titles.Add("Service Request Status");

                Series series = new Series("Requests");
                series.ChartType = SeriesChartType.Doughnut;
                series.IsValueShownAsLabel = true;
                series.Label = "#VALX: #VAL";
                series.LegendText = "#VALX";
                series["DoughnutRadius"] = "60";
                series["PieLabelStyle"] = "Outside";
                series["PieLineColor"] = "Black";

                chart1.Series.Add(series);

                string[] statuses =
                {
            "Assigned",
            "In Progress",
            "Completed"
        };

                Color[] colors =
                {
            Color.Orange,
            Color.DodgerBlue,
            Color.SeaGreen
        };

                foreach (string status in statuses)
                {
                    int count = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["RequestStatus"].ToString()
                            .Equals(status, StringComparison.OrdinalIgnoreCase))
                        {
                            count = Convert.ToInt32(row["TotalRequests"]);
                            break;
                        }
                    }

                    int index = series.Points.AddXY(status, count);
                    series.Points[index].Color =
                        colors[Array.IndexOf(statuses, status)];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void assignedRequests()
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


        public void completedRequests()
        {
            try
            {
                string query = @"SELECT COUNT(*)
                         FROM complaint_details
                         WHERE complaint_status = @status";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    cmd.Parameters.AddWithValue("@status", "Compleated");

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


        public void inProgressRequests()
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

                    label10.Text = cmd.ExecuteScalar().ToString();
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


        public void totalRequests()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM complaint_details";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    if (Program.con.State == ConnectionState.Closed)
                        Program.con.Open();

                    // Replace label1 with your actual dashboard label name
                    label9.Text = cmd.ExecuteScalar().ToString();
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


        // DISPLAY LOGGED-IN USER INFORMATION
        private void fillinfo()
        {
            // Admin is opening the Service Advisor dashboard
            if (Program.uid == "admin")
            {
                label4.Text = "Welcome Service Advisor";
                return;
            }

            // Staff login: display the logged-in username
            string query = @"
        SELECT username
        FROM user_details
        WHERE username = @username";

            using (SqlConnection con =
                new SqlConnection(Program.con.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@username", SqlDbType.VarChar, 100)
                        .Value = Program.uid;

                    con.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        label4.Text = "Welcome " + result.ToString();
                    }
                    else
                    {
                        label4.Text = "Welcome Service Advisor";
                    }
                }
            }
        }
        // BACK TO ADMIN HOME
        private void button1_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
            this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
        }

        // OPEN MECHANIC DASHBOARD
        private void button3_Click(object sender, EventArgs e)
        {
            mechanic_dash obj = new mechanic_dash();
            obj.Show();
        }

        // OPEN STORE KEEPER DASHBOARD
        private void button4_Click(object sender, EventArgs e)
        {
            storekeeper_dash obj = new storekeeper_dash();
            obj.Show();
        }

        // LOGOUT
        private void button7_Click(object sender, EventArgs e)
        {
            Program.uid = "";

            login obj = new login();
            obj.Show();
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            advidorhome_viewreq obj = new advidorhome_viewreq();
            obj.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            advisordash_observation obj = new advisordash_observation();
            obj.Show();
        }

        private void serviceadvisor_dash_Load(object sender, EventArgs e)
        {
            totalRequests();
            inProgressRequests();
            completedRequests();
            assignedRequests();
            LoadServiceRequestDoughnutChart();
            LoadServiceRequestBarChart();
        }
    }
}
