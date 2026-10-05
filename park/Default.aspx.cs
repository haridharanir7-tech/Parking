using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class _Default : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblCurrentDate.Text = "Today<br/><span style='font-size: 11px; color: #858b96;'>" + DateTime.Now.ToString("ddd, dd MMM yyyy") + "</span>";
                LoadDashboardStats();
                LoadOccupancyChart();
                LoadActiveVehicles();
                LoadRecentActivity();
            }
        }

        private 
            
            void LoadDashboardStats()

        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Slots
                using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) as Total, SUM(CASE WHEN Status='Available' THEN 1 ELSE 0 END) as Available, SUM(CASE WHEN Status='Occupied' THEN 1 ELSE 0 END) as Occupied FROM ParkingSlots", conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;
                            int available = reader["Available"] != DBNull.Value ? Convert.ToInt32(reader["Available"]) : 0;
                            int occupied = reader["Occupied"] != DBNull.Value ? Convert.ToInt32(reader["Occupied"]) : 0;

                            lblTotalSlots.Text = total.ToString();
                            lblAvailableSlots.Text = available.ToString();
                            lblOccupiedSlots.Text = occupied.ToString();
                        }
                    }
                }

                // Revenue
                using (MySqlCommand cmd = new MySqlCommand("SELECT SUM(Amount) FROM Payments WHERE PaymentStatus='Paid' AND DATE(PaymentDate) = CURDATE()", conn))
                {
                    object result = cmd.ExecuteScalar();
                    lblTodayRevenue.Text = (result != DBNull.Value && result != null) ? Convert.ToDecimal(result).ToString("0") : "0";
                }
            }
        }

        private void LoadOccupancyChart()
        {
            int occupied = int.Parse(lblOccupiedSlots.Text);
            int total = int.Parse(lblTotalSlots.Text);

            lblOccupancyRatio.Text = $"{occupied} / {total}";
            
            if (total > 0)
            {
                int percentage = (int)Math.Round((double)occupied / total * 100);
                lblOccupancyPercent.Text = percentage.ToString();
                occupancyDonut.Attributes["style"] = $"--p: {percentage}%;";
            }
        }

        private void LoadActiveVehicles()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT VehicleNumber, VehicleType, EntryTime, Status FROM ParkingRecords WHERE Status = 'Parked' ORDER BY EntryTime DESC LIMIT 10";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gvActive.DataSource = dt;
                    gvActive.DataBind();
                }
            }
        }

        private void LoadRecentActivity()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CssType");
            dt.Columns.Add("Icon");
            dt.Columns.Add("Title");
            dt.Columns.Add("Time");
            dt.Columns.Add("VehicleNumber");
            dt.Columns.Add("Badge");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                // Get recent entries
                string entryQuery = "SELECT VehicleNumber, EntryTime as ActivityTime, 'IN' as Action FROM ParkingRecords WHERE DATE(EntryTime) = CURDATE() ORDER BY EntryTime DESC LIMIT 5";
                
                // Get recent payments (exits)
                string exitQuery = "SELECT VehicleNumber, PaymentDate as ActivityTime, 'OUT' as Action FROM Payments WHERE PaymentStatus='Paid' AND DATE(PaymentDate) = CURDATE() ORDER BY PaymentDate DESC LIMIT 5";

                List<ActivityItem> activities = new List<ActivityItem>();

                using (MySqlCommand cmd = new MySqlCommand(entryQuery, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activities.Add(new ActivityItem { 
                                VehicleNumber = reader["VehicleNumber"].ToString(), 
                                Time = Convert.ToDateTime(reader["ActivityTime"]), 
                                Action = "IN" 
                            });
                        }
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(exitQuery, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activities.Add(new ActivityItem { 
                                VehicleNumber = reader["VehicleNumber"].ToString(), 
                                Time = Convert.ToDateTime(reader["ActivityTime"]), 
                                Action = "OUT" 
                            });
                        }
                    }
                }

                activities.Sort((a, b) => b.Time.CompareTo(a.Time));

                foreach (var item in activities)
                {
                    DataRow row = dt.NewRow();
                    row["VehicleNumber"] = item.VehicleNumber;
                    row["Time"] = item.Time.ToString("hh:mm tt");
                    
                    if (item.Action == "IN")
                    {
                        row["CssType"] = "in";
                        row["Icon"] = "<i class=\"fas fa-car-side\"></i>";
                        row["Title"] = "Vehicle Entered";
                        row["Badge"] = "IN";
                    }
                    else
                    {
                        row["CssType"] = "paid";
                        row["Icon"] = "<i class=\"fas fa-wallet\"></i>";
                        row["Title"] = "Payment Received";
                        row["Badge"] = "PAID";
                    }
                    dt.Rows.Add(row);
                }

                if (dt.Rows.Count > 0)
                {
                    rptActivity.DataSource = dt;
                    rptActivity.DataBind();
                }
                else
                {
                    lblNoActivity.Visible = true;
                }
            }
        }

        private class ActivityItem
        {
            public string VehicleNumber { get; set; }
            public DateTime Time { get; set; }
            public string Action { get; set; }
        }
    }
}