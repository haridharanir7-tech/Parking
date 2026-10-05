<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Reports.aspx.cs"
    Inherits="ParkingDashboard.Reports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



<div class="page-header">
    <h1>Reports</h1>
    <p>Parking activity and payment summary</p>
</div>

<div class="cards">

    <div class="card">
        <div class="title">Total Parking Records</div>
        <div class="number">
            <asp:Label ID="lblTotalRecords"
                runat="server" />
        </div>
    </div>

    <div class="card">
        <div class="title">Currently Parked</div>
        <div class="number">
            <asp:Label ID="lblParked"
                runat="server" />
        </div>
    </div>

    <div class="card">
        <div class="title">Completed</div>
        <div class="number">
            <asp:Label ID="lblCompleted"
                runat="server" />
        </div>
    </div>

    <div class="card">
        <div class="title">Total Revenue</div>
        <div class="number">
            ?<asp:Label ID="lblRevenue"
                runat="server" />
        </div>
    </div>

</div>

<div class="panel">

    <h3>Vehicle Type Summary</h3>

    <asp:GridView ID="gvVehicleReport"
        runat="server"
        AutoGenerateColumns="False"
        CssClass="table">

        <Columns>

            <asp:BoundField DataField="VehicleType"
                HeaderText="Vehicle Type" />

            <asp:BoundField DataField="TotalVehicles"
                HeaderText="Total Vehicles" />

        </Columns>

    </asp:GridView>

</div>

</asp:Content>
