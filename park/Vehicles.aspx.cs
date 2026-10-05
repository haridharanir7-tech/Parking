using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Vehicles : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadVehicles("");
            }
        }

        private void LoadVehicles(string search)
        {
            using (MySqlConnection connection =
                new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        pr.VehicleNumber,
                        pr.VehicleType,
                        ps.SlotNumber,
                        pr.EntryTime,
                        pr.Status
                    FROM ParkingRecords pr
                    LEFT JOIN ParkingSlots ps
                        ON pr.SlotId = ps.SlotId
                    WHERE pr.VehicleNumber LIKE @Search
                    ORDER BY pr.RecordId DESC";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Search",
                        "%" + search + "%"
                    );

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();

                        adapter.Fill(table);

                        gvVehicles.DataSource = table;
                        gvVehicles.DataBind();
                    }
                }
            }
        }

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            LoadVehicles(txtSearch.Text.Trim());
        }

        protected void btnRefresh_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            LoadVehicles("");
        }

        public string GetStatusClass(string status)
        {
            if (status == "Parked")
            {
                return "badge parked";
            }

            return "badge completed";
        }
    }
}