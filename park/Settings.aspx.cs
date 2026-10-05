using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Settings : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadSettings();
        }

        private void LoadSettings()
        {
            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT *
                    FROM ParkingSettings
                    LIMIT 1";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtParkingName.Text =
                                reader["ParkingName"].ToString();

                            txtCarRate.Text =
                                reader["CarRate"].ToString();

                            txtBikeRate.Text =
                                reader["BikeRate"].ToString();

                            txtVanRate.Text =
                                reader["VanRate"].ToString();

                            txtTruckRate.Text =
                                reader["TruckRate"].ToString();

                            txtBusRate.Text =
                                reader["BusRate"].ToString();

                            txtOpenTime.Text =
                                reader["OpenTime"].ToString();

                            txtCloseTime.Text =
                                reader["CloseTime"].ToString();
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            decimal carRate;
            decimal bikeRate;
            decimal vanRate;
            decimal truckRate;
            decimal busRate;

            if (!decimal.TryParse(txtCarRate.Text, out carRate) ||
                !decimal.TryParse(txtBikeRate.Text, out bikeRate) ||
                !decimal.TryParse(txtVanRate.Text, out vanRate) ||
                !decimal.TryParse(txtTruckRate.Text, out truckRate) ||
                !decimal.TryParse(txtBusRate.Text, out busRate))
            {
                ShowMessage("Please enter valid rates.", false);
                return;
            }

            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    UPDATE ParkingSettings
                    SET
                        ParkingName = @ParkingName,
                        CarRate = @CarRate,
                        BikeRate = @BikeRate,
                        VanRate = @VanRate,
                        TruckRate = @TruckRate,
                        BusRate = @BusRate,
                        OpenTime = @OpenTime,
                        CloseTime = @CloseTime
                    WHERE SettingId =
                        (SELECT SettingId FROM
                            (SELECT SettingId
                             FROM ParkingSettings
                             LIMIT 1) AS temp)";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@ParkingName",
                        txtParkingName.Text.Trim());

                    command.Parameters.AddWithValue(
                        "@CarRate", carRate);

                    command.Parameters.AddWithValue(
                        "@BikeRate", bikeRate);

                    command.Parameters.AddWithValue(
                        "@VanRate", vanRate);

                    command.Parameters.AddWithValue(
                        "@TruckRate", truckRate);

                    command.Parameters.AddWithValue(
                        "@BusRate", busRate);

                    command.Parameters.AddWithValue(
                        "@OpenTime",
                        txtOpenTime.Text);

                    command.Parameters.AddWithValue(
                        "@CloseTime",
                        txtCloseTime.Text);

                    command.ExecuteNonQuery();
                }
            }

            ShowMessage(
                "Settings saved successfully.",
                true);
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            lblMessage.Text = message;

            lblMessage.ForeColor = success
                ? System.Drawing.Color.Green
                : System.Drawing.Color.Red;
        }
    }
}