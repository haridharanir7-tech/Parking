<%@ Page Title="Records" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Records.aspx.cs"
    Inherits="ParkingDashboard.Records" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



<div class="page-header">
    <h1>Parking Records</h1>
    <p>View and search complete parking history</p>
</div>

<div class="panel">
    <div class="filters">

        <div class="group">
            <label>Vehicle Number</label>
            <asp:TextBox ID="txtSearch" runat="server"
                CssClass="input"
                placeholder="Search vehicle">
            </asp:TextBox>
        </div>

        <div class="group">
            <label>Status</label>
            <asp:DropDownList ID="ddlStatus" runat="server"
                CssClass="input">
                <asp:ListItem Text="All" Value="" />
                <asp:ListItem Text="Parked" Value="Parked" />
                <asp:ListItem Text="Completed" Value="Completed" />
            </asp:DropDownList>
        </div>

        <asp:Button ID="btnSearch" runat="server"
            Text="Search"
            CssClass="btn"
            OnClick="btnSearch_Click" />

        <asp:Button ID="btnClear" runat="server"
            Text="Clear"
            CssClass="btn"
            OnClick="btnClear_Click" />

    </div>
</div>

<div class="panel">

<asp:GridView ID="gvRecords" runat="server"
    AutoGenerateColumns="False"
    CssClass="table"
    EmptyDataText="No parking records found."
    OnRowCommand="gvRecords_RowCommand">

    <Columns>

        <asp:BoundField DataField="RecordId"
            HeaderText="ID" />

        <asp:BoundField DataField="VehicleNumber"
            HeaderText="Vehicle No." />

        <asp:BoundField DataField="SlotNumber"
            HeaderText="Slot" />

        <asp:BoundField DataField="VehicleType"
            HeaderText="Vehicle Type" />

        <asp:BoundField DataField="EntryTime"
            HeaderText="Entry Time"
            DataFormatString="{0:dd-MM-yyyy HH:mm}" />

        <asp:BoundField DataField="ExitTime"
            HeaderText="Exit Time"
            DataFormatString="{0:dd-MM-yyyy HH:mm}" />

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <span class='<%# GetStatusClass(Eval("Status").ToString()) %>'>
                    <%# Eval("Status") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Action">
            <ItemTemplate>
                <asp:LinkButton ID="btnExit" runat="server" 
                                CommandName="ExitVehicle" 
                                CommandArgument='<%# Eval("RecordId") %>'
                                Visible='<%# Eval("Status").ToString() == "Parked" %>'
                                CssClass="save-button" 
                                Style="padding: 6px 12px; font-size: 12px; background: #eab308;"
                                OnClientClick="return confirm('Are you sure you want to exit this vehicle and calculate fee?');">
                    Exit Vehicle
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</asp:GridView>

</div>

</asp:Content>

