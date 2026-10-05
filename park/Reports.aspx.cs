using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Reports : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadReports();
        }

        private void LoadReports()
        {
            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                string parkingQuery = @"
                    SELECT
                        COUNT(*) AS TotalRecords,
                        COALESCE(SUM(Status = 'Parked'),0) AS Parked,
                        COALESCE(SUM(Status = 'Completed'),0) AS Completed
                    FROM ParkingRecords";

                using (MySqlCommand command =
                    new MySqlCommand(parkingQuery, connection))
                {
                    using (MySqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblTotalRecords.Text =
                                reader["TotalRecords"].ToString();

                            lblParked.Text =
                                reader["Parked"].ToString();

                            lblCompleted.Text =
                                reader["Completed"].ToString();
                        }
                    }
                }

                string revenueQuery = @"
                    SELECT COALESCE(SUM(Amount),0)
                    FROM Payments
                    WHERE PaymentStatus = 'Paid'";

                using (MySqlCommand command =
                    new MySqlCommand(revenueQuery, connection))
                {
                    decimal revenue =
                        Convert.ToDecimal(
                            command.ExecuteScalar());

                    lblRevenue.Text =
                        revenue.ToString("N2");
                }

                string vehicleQuery = @"
                    SELECT
                        VehicleType,
                        COUNT(*) AS TotalVehicles
                    FROM ParkingRecords
                    GROUP BY VehicleType
                    ORDER BY TotalVehicles DESC";

                using (MySqlDataAdapter adapter =
                    new MySqlDataAdapter(
                        vehicleQuery,
                        connection))
                {
                    DataTable table =
                        new DataTable();

                    adapter.Fill(table);

                    gvVehicleReport.DataSource =
                        table;

                    gvVehicleReport.DataBind();
                }
            }
        }
    }
}