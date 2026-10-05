<%@ Page Title="Vehicles" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Vehicles.aspx.cs"
    Inherits="ParkingDashboard.Vehicles" %>

<asp:Content ID="VehiclesContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    

    <div class="page-header">
        <div>
            <h1>Vehicles</h1>
            <p>View and search all parked vehicles</p>
        </div>
    </div>

    <div class="vehicle-panel">

        <div class="toolbar">

            <asp:TextBox
                ID="txtSearch"
                runat="server"
                CssClass="search-box"
                AutoPostBack="true"
                OnTextChanged="txtSearch_TextChanged"
                placeholder="Search vehicle number...">
            </asp:TextBox>

            <asp:Button
                ID="btnRefresh"
                runat="server"
                Text="Refresh"
                CssClass="refresh-btn"
                OnClick="btnRefresh_Click" />

        </div>

        <asp:GridView
            ID="gvVehicles"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="vehicle-table"
            EmptyDataText="No vehicles found.">

            <Columns>

                <asp:BoundField
                    DataField="VehicleNumber"
                    HeaderText="Vehicle Number" />

                <asp:TemplateField HeaderText="Vehicle Type">
                    <ItemTemplate>
                        <%# park.Utils.ParkingEngine.GetVehicleIcon(Eval("VehicleType")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField
                    DataField="SlotNumber"
                    HeaderText="Slot" />

                <asp:BoundField
                    DataField="EntryTime"
                    HeaderText="Entry Time"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" />

                <asp:TemplateField HeaderText="Status">

                    <ItemTemplate>

                        <span class='<%# GetStatusClass(Eval("Status").ToString()) %>'>
                            <%# Eval("Status") %>
                        </span>

                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

</asp:Content>
