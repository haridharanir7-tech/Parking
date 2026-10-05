l̥l̥l̥l̥l̥l̥<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ParkingDashboard.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>ParkingPro | Secure Login</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Poppins', sans-serif;
            background-color: #f8fafc;
            height: 100vh;
            display: flex;
            overflow: hidden;
        }

        .split-layout {
            display: flex;
            width: 100%;
            height: 100vh;
        }

        /* Image Side */
        .image-section {
            flex: 1.2;
            background: linear-gradient(rgba(17, 24, 39, 0.7), rgba(17, 24, 39, 0.8)), 
                        url('https://images.unsplash.com/photo-1573348722427-f1d6819fdf98?q=80&w=2000') center/cover no-repeat;
            position: relative;
            display: flex;
            flex-direction: column;
            justify-content: center;
            padding: 40px 10%;
            color: white;
        }

        .image-section .brand {
            position: absolute;
            top: 40px;
            left: 10%;
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 1px;
            display: flex;
            align-items: center;
        }

        .image-section .brand span {
            color: #8b5cf6;
        }

        .image-section h1 {
            font-size: 48px;
            font-weight: 700;
            line-height: 1.2;
            margin-bottom: 20px;
        }

        .image-section p {
            font-size: 18px;
            color: #cbd5e1;
            max-width: 80%;
            line-height: 1.6;
        }

        /* Login Side */
        .form-section {
            flex: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            background: white;
            box-shadow: -10px 0 30px rgba(0,0,0,0.03);
            z-index: 10;
        }

        .login-card {
            width: 100%;
            max-width: 420px;
            padding: 40px;
        }

        .login-card h2 {
            font-size: 32px;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 10px;
        }

        .login-card p {
            color: #64748b;
            font-size: 15px;
            margin-bottom: 35px;
        }

        .form-group {
            margin-bottom: 25px;
        }

        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #334155;
            font-weight: 500;
            font-size: 14px;
        }

        .form-group input {
            width: 100%;
            padding: 14px 16px;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            font-size: 15px;
            color: #1e293b;
            transition: all 0.3s ease;
            background: #f8fafc;
            outline: none;
        }

        .form-group input:focus {
            border-color: #8b5cf6;
            background: white;
            box-shadow: 0 0 0 4px rgba(139, 92, 246, 0.1);
        }

        .btn-login {
            width: 100%;
            padding: 15px;
            background: linear-gradient(135deg, #7c3aed, #6d28d9);
            color: white;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
            margin-top: 10px;
        }

        .btn-login:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(124, 58, 237, 0.3);
        }

        .error-message {
            color: #ef4444;
            font-size: 14px;
            margin-top: 15px;
            display: block;
            text-align: center;
            font-weight: 500;
        }

        @media (max-width: 900px) {
            .split-layout {
                flex-direction: column;
            }
            .image-section {
                flex: none;
                height: 35vh;
                padding: 30px;
                text-align: center;
            }
            .image-section .brand {
                position: relative;
                top: 0;
                left: 0;
                justify-content: center;
                margin-bottom: 20px;
            }
            .image-section p {
                display: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="split-layout">
        
        <!-- Left Side: Beautiful Image -->
        <div class="image-section">
            <div class="brand">Parking<span>Pro</span></div>
            <h1>Smart Parking<br />Management System</h1>
            <p>Experience the next generation of automated vehicle tracking, seamless slot assignment, and intelligent financial reporting all in one professional dashboard.</p>
        </div>

        <!-- Right Side: Login Form -->
        <div class="form-section">
            <div class="login-card">
                <h2>Welcome Back</h2>
                <p>Please enter your admin credentials to access the control center.</p>

                <div class="form-group">
                    <label>Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" placeholder="e.g. admin"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Access Dashboard" CssClass="btn-login" OnClick="btnLogin_Click" />

                <asp:Label ID="lblError" runat="server" CssClass="error-message"></asp:Label>
            </div>
        </div>

    </form>
</body>
</html>

