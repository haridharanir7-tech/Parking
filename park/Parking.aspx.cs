using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Parking : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadParkingRecords();
            }
        }


        protected void btnSaveParking_Click(object sender, EventArgs e)
        {
            string vehicleNumber = txtVehicleNumber.Text.Trim();
            string vehicleType = ddlVehicleType.SelectedValue;

            if (string.IsNullOrEmpty(vehicleNumber))
            {
                ShowMessage("Please enter the vehicle number.", false);
                return;
            }

            DateTime entryDateTime = DateTime.Now;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                // 1. Check if vehicle is already parked
                string checkParkedQuery = "SELECT COUNT(*) FROM ParkingRecords WHERE VehicleNumber = @VehicleNumber AND Status = 'Parked'";
                using (MySqlCommand checkCmd = new MySqlCommand(checkParkedQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        ShowMessage("This vehicle is already parked.", false);
                        return;
                    }
                }

                using (MySqlTransaction tx = connection.BeginTransaction())
                {
                    try
                    {
                        // 2. Find an available slot for this vehicle type
                        string findSlotQuery = "SELECT SlotId, SlotNumber FROM ParkingSlots WHERE VehicleType = @VehicleType AND Status = 'Available' LIMIT 1 FOR UPDATE";
                        int slotId = 0;
                        string slotNumber = "";
                        
                        using (MySqlCommand findCmd = new MySqlCommand(findSlotQuery, connection, tx))
                        {
                            findCmd.Parameters.AddWithValue("@VehicleType", vehicleType);
                            using (MySqlDataReader reader = findCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    slotId = Convert.ToInt32(reader["SlotId"]);
                                    slotNumber = reader["SlotNumber"].ToString();
                                }
                            }
                        }

                        if (slotId == 0)
                        {
                            ShowMessage($"No available slots for {vehicleType}.", false);
                            return;
                        }

                        // 3. Mark slot as Occupied
                        string updateSlotQuery = "UPDATE ParkingSlots SET Status = 'Occupied' WHERE SlotId = @SlotId";
                        using (MySqlCommand updateSlotCmd = new MySqlCommand(updateSlotQuery, connection, tx))
                        {
                            updateSlotCmd.Parameters.AddWithValue("@SlotId", slotId);
                            updateSlotCmd.ExecuteNonQuery();
                        }

                        // 4. Insert Parking Record
                        string insertQuery = @"
                            INSERT INTO ParkingRecords 
                            (VehicleNumber, VehicleType, SlotId, EntryTime, Status) 
                            VALUES (@VehicleNumber, @VehicleType, @SlotId, @EntryTime, 'Parked')";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection, tx))
                        {
                            insertCmd.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
                            insertCmd.Parameters.AddWithValue("@VehicleType", vehicleType);
                            insertCmd.Parameters.AddWithValue("@SlotId", slotId);
                            insertCmd.Parameters.AddWithValue("@EntryTime", entryDateTime);
                            insertCmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        ClearForm();
                        ShowMessage($"Vehicle parked successfully in slot {slotNumber}.", true);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ShowMessage("An error occurred: " + ex.Message, false);
                    }
                }
            }

            LoadParkingRecords();
        }

        private int GetOrCreateSlot(
            MySqlConnection connection,
            string slotNumber,
            string vehicleType)
        {
            string selectQuery = @"
                SELECT SlotId
                FROM ParkingSlots
                WHERE SlotNumber = @SlotNumber
                LIMIT 1";

            using (MySqlCommand command =
                   new MySqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue(
                    "@SlotNumber",
                    slotNumber
                );

                object result =
                    command.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
            }

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
                   new MySqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue(
                    "@SlotNumber",
                    slotNumber
                );

                command.Parameters.AddWithValue(
                    "@VehicleType",
                    vehicleType
                );

                command.ExecuteNonQuery();
            }

            return Convert.ToInt32(
                new MySqlCommand(
                    "SELECT LAST_INSERT_ID()",
                    connection
                ).ExecuteScalar()
            );
        }

        private void LoadParkingRecords()
        {
            using (MySqlConnection connection =
                   new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        pr.RecordId,
                        pr.VehicleNumber,
                        ps.SlotNumber,
                        pr.VehicleType,
                        pr.EntryTime,
                        pr.Status
                    FROM ParkingRecords pr
                    INNER JOIN ParkingSlots ps
                        ON pr.SlotId = ps.SlotId
                    ORDER BY pr.RecordId DESC";

                using (MySqlDataAdapter adapter =
                       new MySqlDataAdapter(query, connection))
                {
                    DataTable table =
                        new DataTable();

                    adapter.Fill(table);

                    table.Columns.Add(
                        "EntryDate",
                        typeof(DateTime)
                    );

                    table.Columns.Add(
                        "Duration",
                        typeof(string)
                    );

                    foreach (DataRow row in table.Rows)
                    {
                        DateTime entry =
                            Convert.ToDateTime(row["EntryTime"]);

                        row["EntryDate"] = entry.Date;

                        row["Duration"] =
                            GetDurationText(entry);
                    }

                    gvParking.DataSource = table;

                    gvParking.DataBind();
                }
            }
        }

        private string GetDurationText(
            DateTime entryTime)
        {
            TimeSpan duration =
                DateTime.Now - entryTime;

            if (duration.TotalMinutes < 60)
            {
                return
                    Math.Max(
                        0,
                        (int)duration.TotalMinutes
                    ) + " min";
            }

            return
                ((int)duration.TotalHours) +
                " hr " +
                duration.Minutes +
                " min";
        }

        public string GetStatusClass(string status)
        {
            if (status == "Parked")
            {
                return "badge parked";
            }

            if (status == "Completed")
            {
                return "badge completed";
            }

            return "badge";
        }

        protected void gvParking_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ExitVehicle")
            {
                int recordId = Convert.ToInt32(e.CommandArgument);
                decimal fee;
                
                try
                {
                    if (park.Utils.ParkingEngine.CompleteParking(recordId, out fee))
                    {
                        ShowMessage($"Vehicle exited successfully. Parking Fee: ?{fee}. Proceed to Payments to collect.", true);
                        LoadParkingRecords();
                    }
                    else
                    {
                        ShowMessage("Could not complete parking. Vehicle may already be exited.", false);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("An error occurred during exit: " + ex.Message, false);
                }
            }
        }

        private void ClearForm()
        {
            txtVehicleNumber.Text = "";
            ddlVehicleType.SelectedIndex = 0;
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;

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