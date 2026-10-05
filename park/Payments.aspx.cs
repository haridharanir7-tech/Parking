using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Payments : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPendingPayments();
                LoadCompletedPayments();
            }
        }

        private void LoadPendingPayments()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT PaymentId, RecordId, VehicleNumber, Amount, PaymentDate FROM Payments WHERE PaymentStatus = 'Pending' ORDER BY PaymentDate DESC";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gvPendingPayments.DataSource = dt;
                    gvPendingPayments.DataBind();
                }
            }
        }

        private void LoadCompletedPayments()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT PaymentId, RecordId, VehicleNumber, Amount, PaymentMethod, PaymentDate FROM Payments WHERE PaymentStatus = 'Paid' ORDER BY PaymentId DESC LIMIT 50";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    gvPayments.DataSource = dt;
                    gvPayments.DataBind();
                }
            }
        }

        protected void gvPendingPayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CollectPayment")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvPendingPayments.Rows[rowIndex];

                HiddenField hdnPaymentId = (HiddenField)row.FindControl("hdnPaymentId");
                DropDownList ddlMethod = (DropDownList)row.FindControl("ddlMethod");

                if (hdnPaymentId != null && ddlMethod != null)
                {
                    int paymentId = Convert.ToInt32(hdnPaymentId.Value);
                    string method = ddlMethod.SelectedValue;

                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "UPDATE Payments SET PaymentStatus = 'Paid', PaymentMethod = @Method WHERE PaymentId = @PaymentId";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Method", method);
                            cmd.Parameters.AddWithValue("@PaymentId", paymentId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    ShowMessage($"Payment collected successfully via {method}!", true);
                    LoadPendingPayments();
                    LoadCompletedPayments();
                }
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }

        protected void btnManualPay_Click(object sender, EventArgs e)
        {
            string searchVal = txtSearchPayment.Text.Trim();
            if (string.IsNullOrEmpty(searchVal)) { ShowMessage("Enter a vehicle number or Record ID.", false); return; }
            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Payments SET PaymentStatus = 'Paid', PaymentMethod = @Method WHERE (VehicleNumber = @Val OR RecordId = @Val) AND PaymentStatus = 'Pending'";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Method", ddlManualMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@Val", searchVal);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0) {
                        ShowMessage("Manual payment processed successfully!", true);
                        txtSearchPayment.Text = "";
                        LoadPendingPayments();
                        LoadCompletedPayments();
                    } else {
                        ShowMessage("No pending payment found for that Vehicle/Record.", false);
                    }
                }
            }
        }
    }
}