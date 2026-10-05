using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Records : System.Web.UI.Page
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadRecords();
        }

        private void LoadRecords()
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
                        pr.ExitTime,
                        pr.Status
                    FROM ParkingRecords pr
                    INNER JOIN ParkingSlots ps
                        ON pr.SlotId = ps.SlotId
                    WHERE
                        (@Search = ''
                        OR pr.VehicleNumber LIKE CONCAT('%', @Search, '%'))
                    AND
                        (@Status = ''
                        OR pr.Status = @Status)
                    ORDER BY pr.RecordId DESC";

                using (MySqlCommand command =
                    new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Search", txtSearch.Text.Trim());

                    command.Parameters.AddWithValue(
                        "@Status", ddlStatus.SelectedValue);

                    using (MySqlDataAdapter adapter =
                        new MySqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();

                        adapter.Fill(table);

                        gvRecords.DataSource = table;
                        gvRecords.DataBind();
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadRecords();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedIndex = 0;
            LoadRecords();
        }

        public string GetStatusClass(string status)
        {
            if (status == "Parked")
                return "badge parked";

            if (status == "Completed")
                return "badge completed";

            return "badge";
        }

        protected void gvRecords_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ExitVehicle")
            {
                int recordId = Convert.ToInt32(e.CommandArgument);
                decimal fee;
                
                try
                {
                    if (park.Utils.ParkingEngine.CompleteParking(recordId, out fee))
                    {
                        LoadRecords();
                    }
                }
                catch
                {
                }
            }
        }
    }
}