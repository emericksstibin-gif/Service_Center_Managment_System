
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeper_viewpartreq : Form
    {
        private readonly string connectionString =
            @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;";

        public storekeeper_viewpartreq()
        {
            InitializeComponent();

            // Configure ComboBox with colored dots.
            SetupStatusComboBox();

            // Connect DataGridView events.
            dataGridView1.CellPainting += dataGridView1_CellPainting;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.CellClick += dataGridView1_CellClick;

            // Connect Update button.
            button1.Click += button1_Click;

            // Load all requests.
            LoadRequests();
        }

        // ---------------- STATUS COMBOBOX ----------------

        private void SetupStatusComboBox()
        {
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();

            comboBox1.Items.Add("Red");
            comboBox1.Items.Add("Green");

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.ItemHeight = 25;

            comboBox1.DrawItem -= comboBox1_DrawItem;
            comboBox1.DrawItem += comboBox1_DrawItem;
        }

        private void comboBox1_DrawItem(
            object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            string status = comboBox1.Items[e.Index].ToString();

            Color dotColor = status == "Red"
                ? Color.Red
                : Color.Green;

            int diameter = 15;
            int x = e.Bounds.X + 8;
            int y = e.Bounds.Y +
                (e.Bounds.Height - diameter) / 2;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (SolidBrush brush = new SolidBrush(dotColor))
            {
                e.Graphics.FillEllipse(
                    brush, x, y, diameter, diameter);
            }

            using (SolidBrush textBrush =
                new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(
                    status,
                    e.Font,
                    textBrush,
                    e.Bounds.X + 32,
                    e.Bounds.Y +
                    (e.Bounds.Height - e.Font.Height) / 2);
            }

            e.DrawFocusRectangle();
        }

        // ---------------- LOAD ALL PART REQUESTS ----------------

        private void LoadRequests()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT request_id, vid, mechanic,
                               part_name, quantity, remark,
                               request_status
                        FROM dbo.parts_request
                        ORDER BY request_id DESC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = null;
                        dataGridView1.Columns.Clear();
                        dataGridView1.DataSource = dt;

                        // Hide the stored status text.
                        dataGridView1.Columns[
                            "request_status"].Visible = false;

                        dataGridView1.Columns[
                            "request_id"].HeaderText = "Request ID";

                        dataGridView1.Columns[
                            "vid"].HeaderText = "Vehicle ID";

                        dataGridView1.Columns[
                            "mechanic"].HeaderText = "Mechanic";

                        dataGridView1.Columns[
                            "part_name"].HeaderText = "Part Name";

                        dataGridView1.Columns[
                            "quantity"].HeaderText = "Quantity";

                        dataGridView1.Columns[
                            "remark"].HeaderText = "Remark";

                        // Add the last Status column.
                        DataGridViewTextBoxColumn statusColumn =
                            new DataGridViewTextBoxColumn();

                        statusColumn.Name = "statusDot";
                        statusColumn.HeaderText = "Status";
                        statusColumn.Width = 75;
                        statusColumn.ReadOnly = true;
                        statusColumn.SortMode =
                            DataGridViewColumnSortMode.NotSortable;

                        dataGridView1.Columns.Add(statusColumn);

                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.MultiSelect = false;

                        dataGridView1.SelectionMode =
                            DataGridViewSelectionMode.FullRowSelect;

                        dataGridView1.AutoSizeColumnsMode =
                            DataGridViewAutoSizeColumnsMode.Fill;

                        dataGridView1.RowTemplate.Height = 38;
                        dataGridView1.AllowUserToResizeRows = false;

                        dataGridView1.Columns["statusDot"].DisplayIndex =
                            dataGridView1.Columns.Count - 1;

                        dataGridView1.ClearSelection();
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading part requests: " + ex.Message);
            }
        }

        // ---------------- DRAW STATUS DOT IN GRID ----------------

        private void dataGridView1_CellPainting(
            object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name
                != "statusDot")
                return;

            e.PaintBackground(e.CellBounds, true);

            string status = Convert.ToString(
                dataGridView1.Rows[e.RowIndex]
                    .Cells["request_status"].Value);

            Color dotColor =
                string.Equals(status, "Green",
                    StringComparison.OrdinalIgnoreCase)
                ? Color.Green
                : Color.Red;

            int diameter = 23;

            int x = e.CellBounds.X +
                (e.CellBounds.Width - diameter) / 2;

            int y = e.CellBounds.Y +
                (e.CellBounds.Height - diameter) / 2;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (SolidBrush brush =
                new SolidBrush(dotColor))
            {
                e.Graphics.FillEllipse(
                    brush, x, y, diameter, diameter);
            }

            e.Paint(e.CellBounds,
                DataGridViewPaintParts.Border);

            e.Handled = true;
        }

        // ---------------- SELECT REQUEST ----------------

        private void dataGridView1_SelectionChanged(
            object sender, EventArgs e)
        {
            SetComboBoxFromSelectedRow();
        }

        private void dataGridView1_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                SetComboBoxFromSelectedRow();
        }

        private void SetComboBoxFromSelectedRow()
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.IsNewRow ||
                !dataGridView1.Columns.Contains("request_status"))
                return;

            object value = dataGridView1.CurrentRow.Cells[
                "request_status"].Value;

            if (value == null || value == DBNull.Value)
            {
                comboBox1.SelectedIndex = -1;
                return;
            }

            string status = value.ToString();

            comboBox1.SelectedItem =
                string.Equals(status, "Green",
                    StringComparison.OrdinalIgnoreCase)
                ? "Green"
                : "Red";
        }

        // ---------------- UPDATE BUTTON ----------------

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.IsNewRow)
            {
                MessageBox.Show(
                    "Please select a part request from the grid.");
                return;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select Red or Green.");
                comboBox1.DroppedDown = true;
                return;
            }

            int requestId = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells[
                    "request_id"].Value);

            string newStatus =
                comboBox1.SelectedItem.ToString();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        UPDATE dbo.parts_request
                        SET request_status = @status
                        WHERE request_id = @request_id";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@status", SqlDbType.VarChar, 10)
                            .Value = newStatus;

                        cmd.Parameters.Add(
                            "@request_id", SqlDbType.Int)
                            .Value = requestId;

                        con.Open();

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            dataGridView1.CurrentRow.Cells[
                                "request_status"].Value = newStatus;

                            dataGridView1.Invalidate();

                            MessageBox.Show(
                                "Request status updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Request not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating request status: " + ex.Message);
            }
        }
    }
}
