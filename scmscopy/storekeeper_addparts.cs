
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeper_addparts : Form
    {
        private readonly string connectionString =
            @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;";

        public storekeeper_addparts()
        {
            InitializeComponent();
        }

        // ---------------- FORM LOAD ----------------

        private void storekeeper_addparts_Load(object sender, EventArgs e)
        {
            textBox1.Text = "1500.00";
            LoadParts();
        }

        // ---------------- LOAD GRID ----------------

        private void LoadParts()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT part_name, category, unit_price,
                               quantity, stock_type, supplier
                        FROM dbo.parts
                        ORDER BY part_name ASC";

                    SqlDataAdapter da =
                        new SqlDataAdapter(query, con);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.DataSource = dt;

                    DataGridViewButtonColumn editButton =
                        new DataGridViewButtonColumn();

                    editButton.Name = "Edit";
                    editButton.HeaderText = "Edit";
                    editButton.Text = "Edit";
                    editButton.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(editButton);

                    DataGridViewButtonColumn deleteButton =
                        new DataGridViewButtonColumn();

                    deleteButton.Name = "Delete";
                    deleteButton.HeaderText = "Delete";
                    deleteButton.Text = "Delete";
                    deleteButton.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(deleteButton);

                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading parts: " + ex.Message);
            }
        }

        // ---------------- UNIT PRICE ----------------

        private void textBoxUnitPrice_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1500.00")
                textBox1.Clear();
        }

        private void textBoxUnitPrice_Leave(object sender, EventArgs e)
        {
            decimal price;

            if (string.IsNullOrWhiteSpace(textBox1.Text))
                return;

            if (decimal.TryParse(textBox1.Text, out price) && price >= 0)
                textBox1.Text = price.ToString("0.00");
            else
                MessageBox.Show("Please enter a valid unit price.");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        // ---------------- SAVE NEW PART ----------------

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(comboBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show("Please enter a quantity greater than zero.");
                return;
            }

            decimal price;

            if (!decimal.TryParse(textBox1.Text, out price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid unit price.");
                textBox1.Focus();
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO dbo.parts
                        (part_name, category, unit_price,
                         quantity, stock_type, supplier)
                        VALUES
                        (@part_name, @category, @unit_price,
                         @quantity, @stock_type, @supplier)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@part_name",
                            SqlDbType.VarChar, 100).Value =
                            textBox6.Text.Trim();

                        cmd.Parameters.Add("@category",
                            SqlDbType.VarChar, 50).Value =
                            comboBox1.Text.Trim();

                        SqlParameter priceParameter =
                            cmd.Parameters.Add("@unit_price", SqlDbType.Decimal);
                        priceParameter.Precision = 10;
                        priceParameter.Scale = 2;
                        priceParameter.Value = price;

                        cmd.Parameters.Add("@quantity", SqlDbType.Int).Value =
                            Convert.ToInt32(numericUpDown1.Value);

                        cmd.Parameters.Add("@stock_type",
                            SqlDbType.VarChar, 30).Value =
                            comboBox2.Text.Trim();

                        cmd.Parameters.Add("@supplier",
                            SqlDbType.VarChar, 100).Value =
                            textBox5.Text.Trim();

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Part saved successfully!");
                ClearFields();
                LoadParts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving part: " + ex.Message);
            }
        }

        // ---------------- EDIT AND DELETE BUTTONS ----------------

        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string columnName =
                dataGridView1.Columns[e.ColumnIndex].Name;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            object[] values = new object[6];

            string[] columnNames =
            {
                "part_name",
                "category",
                "unit_price",
                "quantity",
                "stock_type",
                "supplier"
            };

            for (int i = 0; i < columnNames.Length; i++)
            {
                values[i] = row.Cells[columnNames[i]].Value;
            }

            // EDIT: Open the Update Stock page.
            if (columnName == "Edit")
            {
                string partName = Convert.ToString(values[0]);
                string category = Convert.ToString(values[1]);
                decimal unitPrice = Convert.ToDecimal(values[2]);
                int quantity = Convert.ToInt32(values[3]);
                string stockType = Convert.ToString(values[4]);
                string supplier = Convert.ToString(values[5]);

                storekeeper_updatestock updateForm =
                    new storekeeper_updatestock(
                        partName,
                        category,
                        unitPrice,
                        quantity,
                        stockType,
                        supplier);

                updateForm.ShowDialog();

                // Refresh grid after the update page closes.
                LoadParts();
            }

            // DELETE: Delete the selected row from the database.
            else if (columnName == "Delete")
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this part?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                try
                {
                    using (SqlConnection con =
                        new SqlConnection(connectionString))
                    {
                        string query = @"
                            DELETE FROM dbo.parts
                            WHERE
                                (part_name = @name OR
                                 (part_name IS NULL AND @name IS NULL))
                            AND (category = @category OR
                                 (category IS NULL AND @category IS NULL))
                            AND (unit_price = @price OR
                                 (unit_price IS NULL AND @price IS NULL))
                            AND (quantity = @quantity OR
                                 (quantity IS NULL AND @quantity IS NULL))
                            AND (stock_type = @stock OR
                                 (stock_type IS NULL AND @stock IS NULL))
                            AND (supplier = @supplier OR
                                 (supplier IS NULL AND @supplier IS NULL))";

                        using (SqlCommand cmd =
                            new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add("@name",
                                SqlDbType.VarChar, 100).Value =
                                values[0] ?? DBNull.Value;

                            cmd.Parameters.Add("@category",
                                SqlDbType.VarChar, 50).Value =
                                values[1] ?? DBNull.Value;

                            SqlParameter priceParameter =
                                cmd.Parameters.Add("@price", SqlDbType.Decimal);
                            priceParameter.Precision = 10;
                            priceParameter.Scale = 2;
                            priceParameter.Value = values[2] ?? DBNull.Value;

                            cmd.Parameters.Add("@quantity",
                                SqlDbType.Int).Value =
                                values[3] ?? DBNull.Value;

                            cmd.Parameters.Add("@stock",
                                SqlDbType.VarChar, 30).Value =
                                values[4] ?? DBNull.Value;

                            cmd.Parameters.Add("@supplier",
                                SqlDbType.VarChar, 100).Value =
                                values[5] ?? DBNull.Value;

                            con.Open();
                            int deleted = cmd.ExecuteNonQuery();

                            if (deleted == 0)
                            {
                                MessageBox.Show("Part was not found.");
                                return;
                            }
                        }
                    }

                    MessageBox.Show("Part deleted successfully!");
                    LoadParts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting part: " + ex.Message);
                }
            }
        }

        // ---------------- CLEAR FIELDS ----------------

        private void ClearFields()
        {
            textBox6.Clear();
            comboBox1.SelectedIndex = -1;
            textBox1.Clear();
            numericUpDown1.Value = numericUpDown1.Minimum;
            comboBox2.SelectedIndex = -1;
            textBox5.Clear();
        }
    }
}
