<%@ Page Title="Parking" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Parking.aspx.cs"
    Inherits="ParkingDashboard.Parking" %>

<asp:Content ID="ParkingContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    


    <!-- PAGE HEADER -->

    <div class="page-header">

        <div>
            <h1>Parking Management</h1>
            <p>Manage vehicle parking and monitor parking activity.</p>
        </div>

        <button type="button"
                class="add-button"
                onclick="openParkingModal()">
            Add Parking
        </button>

    </div>


    <!-- PARKING TABLE -->

    <div class="parking-panel">

        <div class="panel-title">
            Current Parking
        </div>

        <asp:GridView ID="gvParking"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="parking-table"
            GridLines="None"
            EmptyDataText="No parking records found."
            OnRowCommand="gvParking_RowCommand">

            <Columns>

                <asp:BoundField
                    DataField="RecordId"
                    HeaderText="ID" />

                <asp:BoundField
                    DataField="VehicleNumber"
                    HeaderText="Vehicle Number" />

                <asp:BoundField
                    DataField="SlotNumber"
                    HeaderText="Slot" />

                <asp:TemplateField HeaderText="Vehicle Type">
                    <ItemTemplate>
                        <%# park.Utils.ParkingEngine.GetVehicleIcon(Eval("VehicleType")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField
                    DataField="EntryDate"
                    HeaderText="Date"
                    DataFormatString="{0:dd-MM-yyyy}" />

                <asp:BoundField
                    DataField="EntryTime"
                    HeaderText="Entry Time"
                    DataFormatString="{0:hh:mm tt}" />

                <asp:BoundField
                    DataField="Duration"
                    HeaderText="Duration" />

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


    <!-- ADD PARKING MODAL -->

    <div id="parkingModal"
         class="modal-overlay">

        <div class="modal">

            <div class="modal-header">

                <h2>Add Parking</h2>

                <button type="button"
                        class="close-button"
                        onclick="closeParkingModal()">
                    &times;
                </button>

            </div>


            <div class="modal-body">

                <div class="form-grid">

                    <!-- VEHICLE NUMBER -->

                    <div class="form-group">

                        <label>
                            Vehicle Number
                        </label>

                        <asp:TextBox ID="txtVehicleNumber"
                            runat="server"
                            CssClass="input uppercase-input"
                            placeholder="Example: TN45AB1234"
                            MaxLength="10"
                            autocomplete="off"
                            oninput="formatVehicleNumber(this)">
                        </asp:TextBox>
                        <small class="input-hint">Format: TN45AB1234 (Letters & Numbers)</small>

                    </div>


                    <!-- VEHICLE TYPE -->

                    <div class="form-group">

                        <label>
                            Vehicle Type
                        </label>

                        <asp:DropDownList ID="ddlVehicleType"
                            runat="server"
                            CssClass="select">

                            <asp:ListItem Text="Car" Value="Car" />

                            <asp:ListItem Text="Bike" Value="Bike" />

                            <asp:ListItem Text="Truck" Value="Truck" />

                            <asp:ListItem Text="Other" Value="Other" />

                        </asp:DropDownList>

                    </div>
                </div>
                
                <div class="form-grid" style="margin-top: 20px;">
                    <!-- DRIVER NAME -->
                    <div class="form-group">
                        <label>Driver Name</label>
                        <asp:TextBox ID="txtDriverName" runat="server" CssClass="input" placeholder="e.g. John Doe"></asp:TextBox>
                    </div>

                    <!-- CONTACT NUMBER -->
                    <div class="form-group">
                        <label>Contact Number</label>
                        <asp:TextBox ID="txtContact" runat="server" CssClass="input" placeholder="e.g. 9876543210"></asp:TextBox>
                    </div>
                </div>


                <asp:Label ID="lblMessage"
                    runat="server"
                    CssClass="message">
                </asp:Label>

            </div>


            <div class="modal-footer">

                <button type="button"
                        class="cancel-button"
                        onclick="closeParkingModal()">
                    Cancel
                </button>

                <asp:Button ID="btnSaveParking"
                    runat="server"
                    Text="Save Parking"
                    CssClass="save-button"
                    OnClick="btnSaveParking_Click"
                    OnClientClick="return validateParkingForm();" />

            </div>

        </div>

    </div>


    <script>

        function openParkingModal() {
            var modal = document.getElementById("parkingModal");
            if (modal) {
                modal.style.display = "flex";
            }
        }

        function closeParkingModal() {
            var modal = document.getElementById("parkingModal");
            if (modal) {
                modal.style.display = "none";
            }
        }

        window.onclick = function (event) {

            var modal = document.getElementById("parkingModal");

            if (event.target === modal) {
                closeParkingModal();
            }

        };

        function formatVehicleNumber(input) {
            // Force uppercase Caps-Lock format and allow only alphanumeric characters
            input.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '');
        }

        function formatSlotNumber(input) {
            // Force uppercase Caps-Lock format and allow alphanumeric and hyphen
            input.value = input.value.toUpperCase().replace(/[^A-Z0-9\-]/g, '');
        }

        function validateParkingForm() {
            var txtVehicle = document.getElementById('<%= txtVehicleNumber.ClientID %>');
            var lblMsg = document.getElementById('<%= lblMessage.ClientID %>');

            var vehicleVal = txtVehicle ? txtVehicle.value.trim().toUpperCase() : '';

            if (txtVehicle) txtVehicle.value = vehicleVal;

            if (!vehicleVal) {
                if (lblMsg) {
                    lblMsg.style.color = '#dc2626';
                    lblMsg.innerText = 'Please enter the vehicle number.';
                }
                if (txtVehicle) txtVehicle.focus();
                return false;
            }

            var vehiclePattern = /^([A-Z]{2}[0-9]{1,2}[A-Z]{0,3}[0-9]{4}|[0-9]{2}BH[0-9]{4}[A-Z]{1,2})$/;
            if (!vehiclePattern.test(vehicleVal)) {
                if (lblMsg) {
                    lblMsg.style.color = '#dc2626';
                    lblMsg.innerText = 'Invalid vehicle number format! Must be uppercase format like TN45AB1234.';
                }
                if (txtVehicle) txtVehicle.focus();
                return false;
            }

            return true;
        }

    </script>

</asp:Content>
