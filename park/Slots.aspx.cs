using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Slots : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSlots();
            }
        }

        protected void ddlVehicleType_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (ddlVehicleType.SelectedValue == "Others")
            {
                otherVehicleGroup.Visible = true;
            }
            else
            {
                otherVehicleGroup.Visible = false;
                txtOtherVehicleType.Text = "";
            }
        }

        protected void btnAddSlot_Click(
            object sender,
            EventArgs e)
        {
            string slotNumber =
                txtSlotNumber.Text.Trim();

            string vehicleType =
                ddlVehicleType.SelectedValue;

            if (slotNumber == "")
            {
                ShowMessage(
                    "Please enter a slot number.",
                    false);

                return;
            }

            if (vehicleType == "Others")
            {
                vehicleType =
                    txtOtherVehicleType.Text.Trim();

                if (vehicleType == "")
                {
                    ShowMessage(
                        "Please specify the vehicle type.",
                        false);

                    return;
                }
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                // Check duplicate slot

                string checkQuery = @"
                    SELECT COUNT(*)
                    FROM ParkingSlots
                    WHERE SlotNumber = @SlotNumber";

                using (MySqlCommand checkCommand =
                    new MySqlCommand(
                        checkQuery,
                        connection))
                {
                    checkCommand.Parameters.AddWithValue(
                        "@SlotNumber",
                        slotNumber);

                    int count =
                        Convert.ToInt32(
                            checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        ShowMessage(
                            "This slot number already exists.",
                            false);

                        return;
                    }
                }

                // Insert slot

                string insertQuery = @"
                    INSERT INTO ParkingSlots
                    (
                        SlotNumber,
                        VehicleType,
                        Status
                    )
                    VALUES
                    (
                        @SlotNumber,
                        @VehicleType,
                        'Available'
                    )";

                using (MySqlCommand command =
                    new MySqlCommand(
                        insertQuery,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@SlotNumber",
                        slotNumber);

                    command.Parameters.AddWithValue(
                        "@VehicleType",
                        vehicleType);

                    command.ExecuteNonQuery();
                }
            }

            txtSlotNumber.Text = "";

            txtOtherVehicleType.Text = "";

            ddlVehicleType.SelectedIndex = 0;

            otherVehicleGroup.Visible = false;

            ShowMessage(
                "Parking slot added successfully.",
                true);

            LoadSlots();
        }

        private void LoadSlots()
        {
            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                string countQuery = @"
                    SELECT
                        COUNT(*) AS TotalSlots,

                        COALESCE(
                            SUM(
                                CASE
                                    WHEN Status = 'Available'
                                    THEN 1
                                    ELSE 0
                                END
                            ),
                            0
                        ) AS AvailableSlots,

                        COALESCE(
                            SUM(
                                CASE
                                    WHEN Status = 'Occupied'
                                    THEN 1
                                    ELSE 0
                                END
                            ),
                            0
                        ) AS OccupiedSlots

                    FROM ParkingSlots";

                using (MySqlCommand command =
                    new MySqlCommand(
                        countQuery,
                        connection))
                {
                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTotalSlots.Text =
                                reader["TotalSlots"].ToString();

                            lblAvailableSlots.Text =
                                reader["AvailableSlots"].ToString();

                            lblOccupiedSlots.Text =
                                reader["OccupiedSlots"].ToString();
                        }
                    }
                }

                string slotQuery = @"
                    SELECT
                        SlotId,
                        SlotNumber,
                        VehicleType,
                        Status
                    FROM ParkingSlots
                    ORDER BY SlotNumber";

                using (MySqlDataAdapter adapter =
                    new MySqlDataAdapter(
                        slotQuery,
                        connection))
                {
                    DataTable table =
                        new DataTable();

                    adapter.Fill(table);

                    rptSlots.DataSource =
                        table;

                    rptSlots.DataBind();
                }
            }
        }

        public string GetSlotClass(
            string status)
        {
            if (status == "Available")
            {
                return "available";
            }

            if (status == "Occupied")
            {
                return "occupied";
            }

            return "available";
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text =
                message;

            if (success)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Green;
            }
            else
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;
            }
        }
    }
}