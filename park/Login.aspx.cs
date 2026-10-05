using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ParkingDashboard
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If user is already logged in, redirect to dashboard
                if (Session["AdminUser"] != null)
                {
                    Response.Redirect("Default.aspx");
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter both username and password.";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ParkingDB"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Admins WHERE Username = @Username AND PasswordHash = @Password";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // Login successful
                            Session["AdminUser"] = username;
                            Response.Redirect("Default.aspx", false);
                        }
                        else
                        {
                            // Login failed
                            lblError.Text = "Invalid username or password.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Database error: " + ex.Message;
                }
            }
        }
    }
}

