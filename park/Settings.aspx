<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Settings.aspx.cs"
    Inherits="ParkingDashboard.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



<div class="page-header">
    <h1>Settings</h1>
    <p>Configure parking rates and operating hours</p>
</div>

<div class="panel">

    <div class="form-grid">

        <div class="group">
            <label>Parking Name</label>
            <asp:TextBox ID="txtParkingName"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Car Rate</label>
            <asp:TextBox ID="txtCarRate"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Bike Rate</label>
            <asp:TextBox ID="txtBikeRate"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Van Rate</label>
            <asp:TextBox ID="txtVanRate"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Truck Rate</label>
            <asp:TextBox ID="txtTruckRate"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Bus Rate</label>
            <asp:TextBox ID="txtBusRate"
                runat="server"
                CssClass="input" />
        </div>

        <div class="group">
            <label>Opening Time</label>
            <asp:TextBox ID="txtOpenTime"
                runat="server"
                CssClass="input"
                TextMode="Time" />
        </div>

        <div class="group">
            <label>Closing Time</label>
            <asp:TextBox ID="txtCloseTime"
                runat="server"
                CssClass="input"
                TextMode="Time" />
        </div>

    </div>

    <asp:Button ID="btnSave"
        runat="server"
        Text="Save Settings"
        CssClass="btn"
        OnClick="btnSave_Click" />

    <asp:Label ID="lblMessage"
        runat="server"
        CssClass="message" />

</div>

</asp:Content>
