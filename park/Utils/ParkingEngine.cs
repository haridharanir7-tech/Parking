using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace park.Utils
{
    public class ParkingEngine
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;
        }

        // Returns settings rates
        public static decimal GetRateForVehicleType(string vehicleType)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                string query = "SELECT CarRate, BikeRate, VanRate, TruckRate, BusRate FROM parkingsettings LIMIT 1";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            switch (vehicleType.ToLower())
                            {
                                case "bike":
                                case "scooter":
                                    return Convert.ToDecimal(reader["BikeRate"]);
                                case "car":
                                    return Convert.ToDecimal(reader["CarRate"]);
                                case "van":
                                    return Convert.ToDecimal(reader["VanRate"]);
                                case "truck":
                                    return Convert.ToDecimal(reader["TruckRate"]);
                                case "bus":
                                    return Convert.ToDecimal(reader["BusRate"]);
                                default:
                                    return 0m; // Fallback
                            }
                        }
                    }
                }
            }
            return 0m;
        }

        // Calculates exact fee based on hours (or fraction) parked
        public static decimal CalculateFee(string vehicleType, DateTime entryTime, DateTime exitTime)
        {
            decimal hourlyRate = GetRateForVehicleType(vehicleType);
            TimeSpan duration = exitTime - entryTime;
            double totalHours = duration.TotalHours;
            
            // Minimum 1 hour charge, and round up to next half-hour maybe?
            // Let's just charge hourly rate * Math.Ceiling(totalHours)
            if (totalHours < 1) totalHours = 1;
            else totalHours = Math.Ceiling(totalHours);
            
            return (decimal)totalHours * hourlyRate;
        }

        // Transactional: Exit Vehicle
        public static bool CompleteParking(int recordId, out decimal calculatedFee)
        {
            calculatedFee = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Get Record Details
                        string getRecordQuery = "SELECT VehicleNumber, VehicleType, SlotId, EntryTime FROM parkingrecords WHERE RecordId = @RecordId AND Status = 'Parked'";
                        string vehicleNumber = "";
                        string vehicleType = "";
                        int slotId = 0;
                        DateTime entryTime = DateTime.MinValue;

                        using (MySqlCommand cmd = new MySqlCommand(getRecordQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RecordId", recordId);
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    vehicleNumber = reader["VehicleNumber"].ToString();
                                    vehicleType = reader["VehicleType"].ToString();
                                    slotId = Convert.ToInt32(reader["SlotId"]);
                                    entryTime = Convert.ToDateTime(reader["EntryTime"]);
                                }
                                else
                                {
                                    return false; // Record not found or not parked
                                }
                            }
                        }

                        // 2. Calculate Fee
                        DateTime exitTime = DateTime.Now;
                        calculatedFee = CalculateFee(vehicleType, entryTime, exitTime);

                        // 3. Update Record
                        string updateRecordQuery = "UPDATE parkingrecords SET ExitTime = @ExitTime, ParkingFee = @Fee, Status = 'Completed' WHERE RecordId = @RecordId";
                        using (MySqlCommand cmd = new MySqlCommand(updateRecordQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ExitTime", exitTime);
                            cmd.Parameters.AddWithValue("@Fee", calculatedFee);
                            cmd.Parameters.AddWithValue("@RecordId", recordId);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. Free Slot
                        string updateSlotQuery = "UPDATE parkingslots SET Status = 'Available' WHERE SlotId = @SlotId";
                        using (MySqlCommand cmd = new MySqlCommand(updateSlotQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@SlotId", slotId);
                            cmd.ExecuteNonQuery();
                        }
                        
                        // 5. Create Pending Payment
                        string insertPaymentQuery = "INSERT INTO payments (RecordId, VehicleNumber, Amount, PaymentMethod, PaymentStatus, PaymentDate) VALUES (@RecordId, @VehicleNumber, @Amount, '', 'Pending', @PaymentDate)";
                        using (MySqlCommand cmd = new MySqlCommand(insertPaymentQuery, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@RecordId", recordId);
                            cmd.Parameters.AddWithValue("@VehicleNumber", vehicleNumber);
                            cmd.Parameters.AddWithValue("@Amount", calculatedFee);
                            cmd.Parameters.AddWithValue("@PaymentDate", exitTime);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
        public static string GetVehicleIcon(object vehicleTypeObj)
        {
            string type = vehicleTypeObj?.ToString() ?? "";
            switch (type)
            {
                case "Car": return "🚗 Car";
                case "Bike": return "🏍️ Bike";
                case "Truck": return "🚚 Truck";
                case "Bus": return "🚌 Bus";
                default: return "🚙 " + type;
            }
        }

        public static string GetVehicleImage(object vehicleTypeObj)
        {
            string type = vehicleTypeObj?.ToString() ?? "";
            string imgUrl = "";
            switch (type.ToLower())
            {
                case "car": imgUrl = "https://images.unsplash.com/photo-1494976388531-d1058494cdd8?auto=format&fit=crop&w=150&q=80"; break;
                case "bike": imgUrl = "https://images.unsplash.com/photo-1558981403-c5f9899a28bc?auto=format&fit=crop&w=150&q=80"; break;
                case "truck": imgUrl = "https://images.unsplash.com/photo-1601584115197-04ecc0da31d7?auto=format&fit=crop&w=150&q=80"; break;
                case "bus": imgUrl = "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?auto=format&fit=crop&w=150&q=80"; break;
                default: imgUrl = "https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?auto=format&fit=crop&w=150&q=80"; break;
            }
            return $"<img src='{imgUrl}' alt='{type}' style='width: 100%; height: 100px; object-fit: cover; border-radius: 8px; margin-bottom: 10px;' /><br/><strong>{type}</strong>";
        }
    }
}

