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
    public partial class storekeeper_dash : Form
    {
        SqlConnection con = Program.con;
        public storekeeper_dash()
        {
            InitializeComponent();
        }
        public void LoadStoreKeeperDoughnutChart()
        {
            try
            {
                string query = @"
            SELECT category, SUM(quantity) AS TotalQuantity
            FROM parts
            WHERE category IS NOT NULL
              AND LTRIM(RTRIM(category)) <> ''
            GROUP BY category
            ORDER BY category";

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
                chart1.Legends.Add(new Legend("CategoryLegend"));
                chart1.Titles.Add("Parts Stock by Category");

                Series series = new Series("Stock");
                series.ChartType = SeriesChartType.Doughnut;
                series.IsValueShownAsLabel = true;
                series.Label = "#VALX: #VAL";
                series.LegendText = "#VALX";
                series["DoughnutRadius"] = "60";

                chart1.Series.Add(series);

                Color[] colors =
                {
            Color.DodgerBlue,
            Color.Orange,
            Color.SeaGreen,
            Color.MediumPurple,
            Color.Tomato,
            Color.Teal,
            Color.Goldenrod,
            Color.DeepPink
        };

                foreach (DataRow row in dt.Rows)
                {
                    string category = row["category"].ToString();
                    int quantity = Convert.ToInt32(row["TotalQuantity"]);

                    int index = series.Points.AddXY(category, quantity);

                    series.Points[index].Color =
                        colors[index % colors.Length];
                }

                if (dt.Rows.Count == 0)
                {
                    chart1.Titles.Add("No parts stock data available");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading store keeper doughnut chart: " + ex.Message);
            }
        }

        public void totalSuppliers()
        {
            try
            {
                string query = @"SELECT COUNT(DISTINCT supplier)
                         FROM parts
                         WHERE supplier IS NOT NULL
                         AND LTRIM(RTRIM(supplier)) <> ''";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
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


        public void totalQuantity()
        {
            try
            {
                string query = "SELECT ISNULL(SUM(quantity), 0) FROM parts";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
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


        


        public void lowStockParts()
        {
            try
            {
                string query = @"SELECT COUNT(*)
                         FROM parts
                         WHERE quantity <= 5";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
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


        public void totalParts()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM parts";

                using (SqlCommand cmd = new SqlCommand(query, Program.con))
                {
                    if (Program.con.State == ConnectionState.Closed)
                        Program.con.Open();

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


        private void button1_Click(object sender, EventArgs e)
        {
            admin_home obj = new admin_home();
            obj.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mechanic_dash obj = new mechanic_dash();
            obj.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serviceadvisor_dash obj = new serviceadvisor_dash();
            obj.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            login obj = new login();
            obj.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            storekeeperdash_viewupdate obj = new storekeeperdash_viewupdate();
            obj.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            storekeeperdash_partrequest obj = new storekeeperdash_partrequest();
            obj.Show();
        }

        private void storekeeper_dash_Load(object sender, EventArgs e)
        {
            totalParts();
            lowStockParts();
            totalSuppliers();
            totalQuantity();
            LoadStoreKeeperDoughnutChart();
        }
    }
}
