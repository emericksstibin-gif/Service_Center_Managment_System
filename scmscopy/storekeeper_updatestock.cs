
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace scmscopy
{
    public partial class storekeeper_updatestock : Form
    {
        private readonly string connectionString =
            @"Server=DESKTOP-BB8L005\SQLEXPRESS;Database=copy_db;Integrated Security=True;";

        private object[] originalValues = null;
        private bool partLoaded = false;

        // Normal opening: search for a part.
        public storekeeper_updatestock()
        {
            InitializeComponent();

            SetReadOnlyFields();
            LoadStockTypes();
            LoadPartNames();
        }

        // Opening from the Edit button in Add Parts.
        public storekeeper_updatestock(
            string partName,
            string category,
            decimal unitPrice,
            int quantity,
            string stockType,
            string supplier)
        {
            InitializeComponent();

            SetReadOnlyFields();
            LoadStockTypes();
            LoadPartNames();

            FillPartDetails(
                partName, category, unitPrice,
                quantity, stockType, supplier);

            originalValues = new object[]
            {
                partName, category, unitPrice,
                quantity, stockType, supplier
            };

            partLoaded = true;
        }

        // ---------------- READ-ONLY FIELDS ----------------

        private void SetReadOnlyFields()
        {
            textBox6.ReadOnly = true;  // Part Name
            comboBox1.Enabled = false; // Category
            textBox5.ReadOnly = true;  // Supplier
        }

        // ---------------- FORM LOAD ----------------

        private void storekeeper_updatestock_Load(
            object sender, EventArgs e)
        {
            LoadPartNames();
            LoadStockTypes();
            LoadParts();
        }

        // Keep this method if your Designer already uses this event.
        private void storekeeper_updatestock_Load_1(
            object sender, EventArgs e)
        {
            storekeeper_updatestock_Load(sender, e);
        }

        // ---------------- LOAD PART NAMES ----------------

        private void LoadPartNames()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT LTRIM(RTRIM(part_name)) AS part_name
                        FROM dbo.parts
                        WHERE part_name IS NOT NULL
                          AND LTRIM(RTRIM(part_name)) <> ''
                        ORDER BY part_name ASC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox3.DataSource = null;
                        comboBox3.Items.Clear();

                        comboBox3.DisplayMember = "part_name";
                        comboBox3.ValueMember = "part_name";
                        comboBox3.DataSource = dt;
                        comboBox3.DropDownStyle =
                            ComboBoxStyle.DropDownList;
                        comboBox3.SelectedIndex = -1;
                    }

                    if (comboBox3.Items.Count == 0)
                    {
                        // No warning here: the table may be empty
                        // while the form is first being initialized.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading part names: " + ex.Message);
            }
        }

        // ---------------- LOAD STOCK TYPES ----------------

        private void LoadStockTypes()
        {
            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT stock_type
                        FROM dbo.parts
                        WHERE stock_type IS NOT NULL
                          AND LTRIM(RTRIM(stock_type)) <> ''
                        ORDER BY stock_type ASC";

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox2.DataSource = null;
                        comboBox2.Items.Clear();

                        comboBox2.DisplayMember = "stock_type";
                        comboBox2.ValueMember = "stock_type";
                        comboBox2.DataSource = dt;
                        comboBox2.DropDownStyle =
                            ComboBoxStyle.DropDownList;
                        comboBox2.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading stock types: " + ex.Message);
            }
        }

        // ---------------- LOAD ALL PARTS INTO GRID ----------------

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

                    using (SqlDataAdapter da =
                        new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt;

                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AutoSizeColumnsMode =
                            DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView1.SelectionMode =
                            DataGridViewSelectionMode.FullRowSelect;
                        dataGridView1.MultiSelect = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading parts into grid: " + ex.Message);
            }
        }

        // ---------------- SEARCH BUTTON ----------------
        // Connect the Search button Click event to this method.
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a part name.");
                comboBox3.DroppedDown = true;
                return;
            }

            string selectedPart = comboBox3.Text.Trim();

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 1 part_name, category,
                                     unit_price, quantity,
                                     stock_type, supplier
                        FROM dbo.parts
                        WHERE part_name = @part_name
                        ORDER BY category, supplier";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@part_name", SqlDbType.VarChar, 100)
                            .Value = selectedPart;

                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                partLoaded = false;
                                originalValues = null;

                                MessageBox.Show("Part not found.");
                                return;
                            }

                            string partName =
                                Convert.ToString(dr["part_name"]);

                            string category =
                                Convert.ToString(dr["category"]);

                            decimal price =
                                Convert.ToDecimal(dr["unit_price"]);

                            int quantity =
                                Convert.ToInt32(dr["quantity"]);

                            string stockType =
                                Convert.ToString(dr["stock_type"]);

                            string supplier =
                                Convert.ToString(dr["supplier"]);

                            FillPartDetails(
                                partName, category, price,
                                quantity, stockType, supplier);

                            originalValues = new object[]
                            {
                                dr["part_name"],
                                dr["category"],
                                dr["unit_price"],
                                dr["quantity"],
                                dr["stock_type"],
                                dr["supplier"]
                            };

                            partLoaded = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error searching part: " + ex.Message);
            }
        }

        // ---------------- FILL ALL FIELDS ----------------

        private void FillPartDetails(
            string partName,
            string category,
            decimal unitPrice,
            int quantity,
            string stockType,
            string supplier)
        {
            textBox6.Text = partName;
            comboBox1.Text = category;
            textBox5.Text = supplier;
            textBox1.Text = unitPrice.ToString("0.00");

            decimal qty = quantity;

            if (qty < numericUpDown1.Minimum)
                qty = numericUpDown1.Minimum;

            if (qty > numericUpDown1.Maximum)
                qty = numericUpDown1.Maximum;

            numericUpDown1.Value = qty;

            // Ensure the existing stock value can be selected.
            if (comboBox2.DataSource is DataTable stockTable)
            {
                bool found = false;

                foreach (DataRow row in stockTable.Rows)
                {
                    if (string.Equals(
                        Convert.ToString(row["stock_type"]),
                        stockType,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found && !string.IsNullOrWhiteSpace(stockType))
                {
                    stockTable.Rows.Add(stockType);
                    stockTable.DefaultView.Sort = "stock_type ASC";
                }
            }

            comboBox2.SelectedValue = stockType;

            if (comboBox2.SelectedIndex == -1)
                comboBox2.Text = stockType;

            SetReadOnlyFields();
        }

        // ---------------- UPDATE BUTTON ----------------
        // Your Update button uses button3_Click.
        private void button3_Click(object sender, EventArgs e)
        {
            if (!partLoaded || originalValues == null)
            {
                MessageBox.Show(
                    "Please search for a part or open one using Edit first.");
                return;
            }

            decimal newPrice;

            if (!decimal.TryParse(textBox1.Text, out newPrice)
                || newPrice <= 0)
            {
                MessageBox.Show("Please enter a valid unit price.");
                textBox1.Focus();
                return;
            }

            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show(
                    "Quantity must be greater than zero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Please select a stock type.");
                return;
            }

            try
            {
                using (SqlConnection con =
                    new SqlConnection(connectionString))
                {
                    string query = @"
                        UPDATE dbo.parts
                        SET unit_price = @newPrice,
                            quantity = @newQuantity,
                            stock_type = @newStock
                        WHERE
                            part_name = @oldName
                            AND category = @oldCategory
                            AND unit_price = @oldPrice
                            AND quantity = @oldQuantity
                            AND stock_type = @oldStock
                            AND supplier = @oldSupplier";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        SqlParameter newPriceParameter =
                            cmd.Parameters.Add(
                                "@newPrice", SqlDbType.Decimal);
                        newPriceParameter.Precision = 10;
                        newPriceParameter.Scale = 2;
                        newPriceParameter.Value = newPrice;

                        cmd.Parameters.Add(
                            "@newQuantity", SqlDbType.Int).Value =
                            Convert.ToInt32(numericUpDown1.Value);

                        cmd.Parameters.Add(
                            "@newStock", SqlDbType.VarChar, 30).Value =
                            comboBox2.Text.Trim();

                        cmd.Parameters.Add(
                            "@oldName", SqlDbType.VarChar, 100).Value =
                            originalValues[0];

                        cmd.Parameters.Add(
                            "@oldCategory", SqlDbType.VarChar, 50).Value =
                            originalValues[1];

                        SqlParameter oldPriceParameter =
                            cmd.Parameters.Add(
                                "@oldPrice", SqlDbType.Decimal);
                        oldPriceParameter.Precision = 10;
                        oldPriceParameter.Scale = 2;
                        oldPriceParameter.Value = originalValues[2];

                        cmd.Parameters.Add(
                            "@oldQuantity", SqlDbType.Int).Value =
                            originalValues[3];

                        cmd.Parameters.Add(
                            "@oldStock", SqlDbType.VarChar, 30).Value =
                            originalValues[4];

                        cmd.Parameters.Add(
                            "@oldSupplier", SqlDbType.VarChar, 100).Value =
                            originalValues[5];

                        con.Open();

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show(
                                "Stock updated successfully!");

                            // Refresh the grid and reload dropdown values.
                            LoadParts();
                            LoadStockTypes();
                            LoadPartNames();

                            partLoaded = false;
                            originalValues = null;

                            // Keep the page open to show the refreshed grid.
                        }
                        else
                        {
                            MessageBox.Show(
                                "No matching part was updated. " +
                                "Please search for the part again.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error updating stock: " + ex.Message);
            }
        }

        // ---------------- CLEAR BUTTON ----------------
        // Your Clear button uses button4_Click.
        private void button4_Click(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = -1;

            textBox6.Clear();
            comboBox1.SelectedIndex = -1;
            textBox5.Clear();
            textBox1.Clear();

            numericUpDown1.Value = numericUpDown1.Minimum;
            comboBox2.SelectedIndex = -1;

            originalValues = null;
            partLoaded = false;
        }
    }
}
