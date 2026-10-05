<%@ Page Title="Parking Slots" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Slots.aspx.cs"
    Inherits="ParkingDashboard.Slots" %>

<asp:Content ID="SlotsContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    

    <div class="page-header">
        <h1>Parking Slots</h1>
        <p>Manage and monitor all parking slots</p>
    </div>

    <div class="slot-summary">

        <div class="summary-card">
            <div class="summary-title">Total Slots</div>

            <div class="summary-number">
                <asp:Label ID="lblTotalSlots"
                    runat="server"
                    Text="0">
                </asp:Label>
            </div>
        </div>

        <div class="summary-card">
            <div class="summary-title">Available</div>

            <div class="summary-number">
                <asp:Label ID="lblAvailableSlots"
                    runat="server"
                    Text="0">
                </asp:Label>
            </div>
        </div>

        <div class="summary-card">
            <div class="summary-title">Occupied</div>

            <div class="summary-number">
                <asp:Label ID="lblOccupiedSlots"
                    runat="server"
                    Text="0">
                </asp:Label>
            </div>
        </div>

    </div>

    <!-- ADD SLOT -->

    <div class="action-panel">

        <div class="action-row">

            <!-- SLOT NUMBER -->

            <div class="form-group">

                <label>Slot Number</label>

                <asp:TextBox ID="txtSlotNumber"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Example: C-01">
                </asp:TextBox>

            </div>

            <!-- VEHICLE TYPE -->

            <div class="form-group">

                <label>Vehicle Type</label>

                <asp:DropDownList ID="ddlVehicleType"
                    runat="server"
                    CssClass="form-control"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlVehicleType_SelectedIndexChanged">

                    <asp:ListItem Text="Bike" Value="Bike" />

                    <asp:ListItem Text="Car" Value="Car" />

                    <asp:ListItem Text="Van" Value="Van" />

                    <asp:ListItem Text="Scooty" Value="Scooty" />

                    <asp:ListItem Text="Truck" Value="Truck" />

                    <asp:ListItem Text="Bus" Value="Bus" />

                    <asp:ListItem Text="Others" Value="Others" />

                </asp:DropDownList>

            </div>

            <!-- OTHER VEHICLE TYPE -->

            <div class="form-group"
                id="otherVehicleGroup"
                runat="server"
                visible="false">

                <label>Specify Vehicle Type</label>

                <asp:TextBox ID="txtOtherVehicleType"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Enter vehicle type">
                </asp:TextBox>

            </div>

            <!-- ADD BUTTON -->

            <div class="form-group">

                <asp:Button ID="btnAddSlot"
                    runat="server"
                    Text="Add Slot"
                    CssClass="btn-add"
                    OnClick="btnAddSlot_Click" />

            </div>

        </div>

        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="message">
        </asp:Label>

    </div>

    <!-- ALL SLOTS -->

    <div class="slots-panel">

        <div class="slots-title">
            All Parking Slots
        </div>

        <div class="slots-grid">

            <asp:Repeater ID="rptSlots"
                runat="server">

                <ItemTemplate>

                    <div class="slot-card">

                        <div class="slot-number">
                            <%# Eval("SlotNumber") %>
                        </div>

                        <div class="slot-type" style="margin-top: 10px;">
                            <%# park.Utils.ParkingEngine.GetVehicleImage(Eval("VehicleType")) %>
                        </div>

                        <div>

                            <span class='<%# GetSlotClass(Eval("Status").ToString()) %>'>
                                <%# Eval("Status") %>
                            </span>

                        </div>

                    </div>

                </ItemTemplate>

            </asp:Repeater>

        </div>

    </div>

</asp:Content>
