 <%@ Page Title="Payments" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Payments.aspx.cs"
    Inherits="ParkingDashboard.Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



<div class="page-header">
    <h1>Payments</h1>
    <p>Manage parking payments and payment history</p>
</div>

<div class="panel">
    <div class="panel-title" style="font-size: 16px; font-weight: 600; margin-bottom: 15px;">Process Payment Manually</div>
    <div class="form-grid" style="grid-template-columns: 1fr 1fr 1fr; align-items: end; margin-bottom: 25px;">
        <div class="form-group">
            <label>Vehicle No. / Record ID</label>
            <asp:TextBox ID="txtSearchPayment" runat="server" CssClass="input" placeholder="e.g. TN45..."></asp:TextBox>
        </div>
        <div class="form-group">
            <label>Payment Method</label>
            <asp:DropDownList ID="ddlManualMethod" runat="server" CssClass="select">
                <asp:ListItem Text="Cash" Value="Cash" />
                <asp:ListItem Text="UPI" Value="UPI" />
                <asp:ListItem Text="Card" Value="Card" />
            </asp:DropDownList>
        </div>
        <div class="form-group">
            <asp:Button ID="btnManualPay" runat="server" Text="Process Payment" CssClass="btn" style="background: #10b981;" OnClick="btnManualPay_Click" />
        </div>
    </div>
    
    <div class="panel-title" style="font-size: 16px; font-weight: 600; margin-bottom: 15px;">Pending Payments</div>
    
    <asp:GridView ID="gvPendingPayments" runat="server"
        AutoGenerateColumns="False"
        CssClass="table"
        EmptyDataText="No pending payments right now."
        OnRowCommand="gvPendingPayments_RowCommand">

        <Columns>
            <asp:BoundField DataField="PaymentId" HeaderText="ID" />
            <asp:BoundField DataField="RecordId" HeaderText="Record" />
            <asp:BoundField DataField="VehicleNumber" HeaderText="Vehicle No." />
            <asp:BoundField DataField="Amount" HeaderText="Amount Due" DataFormatString="₹{0:0.00}" />
            <asp:BoundField DataField="PaymentDate" HeaderText="Exit Time" DataFormatString="{0:dd-MM-yyyy hh:mm tt}" />
            
            <asp:TemplateField HeaderText="Payment Method">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlMethod" runat="server" CssClass="input" Style="width: 120px; padding: 5px;">
                        <asp:ListItem Text="Cash" Value="Cash" />
                        <asp:ListItem Text="UPI" Value="UPI" />
                        <asp:ListItem Text="Card" Value="Card" />
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton ID="btnCollect" runat="server" 
                        CommandName="CollectPayment" 
                        CommandArgument='<%# Container.DataItemIndex %>'
                        CssClass="btn" Style="background-color: #10b981;">
                        Collect Payment
                    </asp:LinkButton>
                    <asp:HiddenField ID="hdnPaymentId" runat="server" Value='<%# Eval("PaymentId") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>
</div>

<div class="panel">

<asp:GridView ID="gvPayments" runat="server"
    AutoGenerateColumns="False"
    CssClass="table"
    EmptyDataText="No payments found.">

    <Columns>

        <asp:BoundField DataField="PaymentId"
            HeaderText="Payment ID" />

        <asp:BoundField DataField="RecordId"
            HeaderText="Record ID" />

        <asp:BoundField DataField="VehicleNumber"
            HeaderText="Vehicle No." />

        <asp:BoundField DataField="Amount"
            HeaderText="Amount"
            DataFormatString="₹ {0:N2}" />

        <asp:BoundField DataField="PaymentMethod"
            HeaderText="Method" />

        <asp:BoundField DataField="PaymentStatus"
            HeaderText="Status" />

        <asp:BoundField DataField="PaymentDate"
            HeaderText="Date"
            DataFormatString="{0:dd-MM-yyyy HH:mm}" />

    </Columns>

</asp:GridView>

</div>

</asp:Content>
