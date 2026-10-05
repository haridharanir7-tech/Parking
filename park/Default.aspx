<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ParkingDashboard._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <style>
        body { font-family: 'Poppins', sans-serif; background-color: #f8fafc; }
        .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
        .page-header h1 { font-size: 24px; font-weight: 700; color: #1e293b; margin: 0; }
        .page-header p { font-size: 14px; color: #64748b; margin: 5px 0 0 0; }
        .date-badge { background: white; border: 1px solid #e2e8f0; padding: 10px 16px; border-radius: 10px; font-size: 13px; font-weight: 500; color: #475569; display: flex; align-items: center; gap: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.02); }
        
        .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 30px; }
        .stat-card { background: white; border-radius: 16px; padding: 24px; border: 1px solid #e2e8f0; position: relative; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.02); transition: transform 0.2s; }
        .stat-card:hover { transform: translateY(-3px); box-shadow: 0 8px 25px rgba(0,0,0,0.05); }
        .stat-card.blue { background: linear-gradient(145deg, #ffffff, #f0f4ff); border-color: #e0e7ff; }
        .stat-card.green { background: linear-gradient(145deg, #ffffff, #f0fdf4); border-color: #dcfce3; }
        .stat-card.red { background: linear-gradient(145deg, #ffffff, #fff1f2); border-color: #ffe4e6; }
        .stat-card.orange { background: linear-gradient(145deg, #ffffff, #fff7ed); border-color: #ffedd5; }
        
        .stat-icon { width: 44px; height: 44px; border-radius: 12px; display: flex; align-items: center; justify-content: center; margin-bottom: 15px; font-size: 18px; color: white; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
        .stat-card.blue .stat-icon { background: linear-gradient(135deg, #6366f1, #4f46e5); }
        .stat-card.green .stat-icon { background: linear-gradient(135deg, #10b981, #059669); }
        .stat-card.red .stat-icon { background: linear-gradient(135deg, #f43f5e, #e11d48); }
        .stat-card.orange .stat-icon { background: linear-gradient(135deg, #f59e0b, #d97706); }
        
        .stat-title { font-size: 14px; font-weight: 600; color: #475569; margin-bottom: 5px; }
        .stat-value { font-size: 32px; font-weight: 700; color: #1e293b; }
        .stat-desc { font-size: 12px; color: #64748b; margin-top: 5px; }

        .dashboard-main { display: grid; grid-template-columns: 2fr 1fr; gap: 24px; margin-bottom: 30px; }
        .panel { background: white; border-radius: 16px; padding: 24px; border: 1px solid #e2e8f0; box-shadow: 0 4px 15px rgba(0,0,0,0.02); }
        .panel-title { font-size: 16px; font-weight: 600; color: #1e293b; margin-bottom: 20px; display: flex; justify-content: space-between; align-items: center; }
        
        /* Occupancy Donut */
        .donut-container { display: flex; align-items: center; justify-content: center; position: relative; height: 200px; }
        .donut-chart { width: 150px; height: 150px; border-radius: 50%; background: conic-gradient(#6366f1 var(--p), #e2e8f0 0); display: flex; align-items: center; justify-content: center; }
        .donut-inner { width: 110px; height: 110px; border-radius: 50%; background: white; display: flex; flex-direction: column; align-items: center; justify-content: center; }
        .donut-percent { font-size: 28px; font-weight: 700; color: #1e293b; }
        .donut-label { font-size: 12px; color: #64748b; }
        
        .recent-activity { display: flex; flex-direction: column; gap: 15px; }
        .activity-item { display: flex; align-items: center; gap: 15px; padding-bottom: 15px; border-bottom: 1px solid #f1f5f9; }
        .activity-item:last-child { border-bottom: none; padding-bottom: 0; }
        .act-icon { width: 36px; height: 36px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 14px; }
        .act-icon.in { background: #dcfce3; color: #16a34a; }
        .act-icon.out { background: #e0e7ff; color: #4f46e5; }
        .act-icon.paid { background: #ffedd5; color: #ea580c; }
        .act-details { flex: 1; }
        .act-title { font-size: 13px; font-weight: 600; color: #1e293b; }
        .act-time { font-size: 11px; color: #64748b; margin-top: 2px; }
        .act-badge { font-size: 11px; font-weight: 600; padding: 4px 10px; border-radius: 6px; }
        .act-badge.in { background: #dcfce3; color: #16a34a; }
        .act-badge.out { background: #e0e7ff; color: #4f46e5; }
        .act-badge.paid { background: #ffedd5; color: #ea580c; }
        
        /* Table overrides */
        .table { width: 100%; border-collapse: collapse; }
        .table th { background: #f8fafc; color: #64748b; font-size: 12px; font-weight: 600; padding: 12px 15px; text-transform: uppercase; text-align: left; }
        .table td { padding: 15px; border-bottom: 1px solid #f1f5f9; font-size: 14px; color: #1e293b; }

        @media (max-width: 1024px) {
            .stats-grid { grid-template-columns: repeat(2, 1fr); }
            .dashboard-main { grid-template-columns: 1fr; }
        }
    </style>

    <div class="page-header">
        <div>
            <h1>Good Morning, Parking Dashboard</h1>
            <p>Monitor your parking operations</p>
        </div>
        <div class="date-badge">
            <i class="far fa-calendar-alt" style="color: #6366f1;"></i>
            <asp:Label ID="lblCurrentDate" runat="server"></asp:Label>
        </div>
    </div>

    <!-- STATS -->
    <div class="stats-grid">
        <div class="stat-card blue">
            <div class="stat-icon"><i class="fas fa-parking"></i></div>
            <div class="stat-title">Total Slots</div>
            <div class="stat-value"><asp:Label ID="lblTotalSlots" runat="server">0</asp:Label></div>
            <div class="stat-desc">Registered parking spaces</div>
        </div>
        
        <div class="stat-card green">
            <div class="stat-icon"><i class="fas fa-check-circle"></i></div>
            <div class="stat-title">Available Slots</div>
            <div class="stat-value"><asp:Label ID="lblAvailableSlots" runat="server">0</asp:Label></div>
            <div class="stat-desc">Currently available</div>
        </div>
        
        <div class="stat-card red">
            <div class="stat-icon"><i class="fas fa-car"></i></div>
            <div class="stat-title">Occupied Slots</div>
            <div class="stat-value"><asp:Label ID="lblOccupiedSlots" runat="server">0</asp:Label></div>
            <div class="stat-desc">Currently occupied</div>
        </div>
        
        <div class="stat-card orange">
            <div class="stat-icon"><i class="fas fa-rupee-sign"></i></div>
            <div class="stat-title">Today's Revenue</div>
            <div class="stat-value">₹<asp:Label ID="lblTodayRevenue" runat="server">0</asp:Label></div>
            <div class="stat-desc">Total parking collection</div>
        </div>
    </div>

    <div class="dashboard-main">
        <!-- LEFT COLUMN -->
        <div style="display: flex; flex-direction: column; gap: 24px;">
            <div class="panel">
                <div class="panel-title">Parking Occupancy</div>
                <div style="display: flex; align-items: center; justify-content: space-around;">
                    <div class="donut-container">
                        <div class="donut-chart" id="occupancyDonut" runat="server" style="--p: 0%;">
                            <div class="donut-inner">
                                <div class="donut-percent"><asp:Label ID="lblOccupancyPercent" runat="server">0</asp:Label>%</div>
                                <div class="donut-label">Occupied</div>
                            </div>
                        </div>
                    </div>
                    <div>
                        <div style="font-size: 24px; font-weight: 700; color: #1e293b; margin-bottom: 10px;">
                            <asp:Label ID="lblOccupancyRatio" runat="server">0 / 0</asp:Label>
                        </div>
                        <div style="font-size: 13px; color: #64748b;">Slots Occupied</div>
                    </div>
                </div>
            </div>

            <div class="panel">
                <div class="panel-title">Currently Parked Vehicles</div>
                <asp:GridView ID="gvActive" runat="server" AutoGenerateColumns="False" 
                    GridLines="None" CssClass="table" Width="100%" EmptyDataText="No vehicles parked right now.">
                    <Columns>
                        <asp:BoundField DataField="VehicleNumber" HeaderText="Vehicle" />
                        <asp:BoundField DataField="VehicleType" HeaderText="Type" />
                        <asp:BoundField DataField="EntryTime" HeaderText="Entry Time" DataFormatString="{0:hh:mm tt}" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- RIGHT COLUMN -->
        <div class="panel">
            <div class="panel-title">Recent Activity</div>
            <div class="recent-activity">
                <asp:Repeater ID="rptActivity" runat="server">
                    <ItemTemplate>
                        <div class="activity-item">
                            <div class='act-icon <%# Eval("CssType") %>'><%# Eval("Icon") %></div>
                            <div class="act-details">
                                <div class="act-title"><%# Eval("Title") %></div>
                                <div class="act-time"><%# Eval("Time") %> - <%# Eval("VehicleNumber") %></div>
                            </div>
                            <div class='act-badge <%# Eval("CssType") %>'><%# Eval("Badge") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoActivity" runat="server" Text="No recent activity today." Visible="false" ForeColor="#64748b" Font-Size="13px"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>