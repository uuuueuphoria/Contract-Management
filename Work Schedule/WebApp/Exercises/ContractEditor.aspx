<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContractEditor.aspx.cs" Inherits="WebApp.Exercises.ContractEditor" %>

<%@ Register Src="~/UserControls/MessageUserControl.ascx" TagPrefix="uc1" TagName="MessageUserControl" %>



<%-- DO NOT MODIFY the Markup except where marked and when adding Event Handlers (e.g.: OnRowCommand="" or OnClick="") --%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="jimbotron">
        <h1>Contract Editor <!-- example icon -->
    <i data-feather="circle"></i></h1>
        <div class="row">
            <uc1:MessageUserControl runat="server" id="MessageUserControl" />
        </div>
    </div>
    <div class="row">
        <div class="col-md-6">
            <h3>Client</h3>
            <asp:DropDownList ID="ClientDropDown" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ClientDropDown_SelectedIndexChanged" DataSourceID="ClientDataSource" DataTextField="DisplayText" DataValueField="DisplayValue">
                <asp:ListItem Value="0">[Select a Client]</asp:ListItem>
            </asp:DropDownList>
            <asp:ObjectDataSource ID="ClientDataSource" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="ListClients" TypeName="ScheduleStarterKit.BLL.ContractController" OnSelected="SelectCheckForException"  />
            <h4>
                <asp:Image ID="ClientImage" runat="server" CssClass="rounded-circle img-thumbnail float-right" Style="height: 80px; width: 80px;" />
                <asp:Label ID="ClientName" runat="server" />
            </h4>
            <asp:Label ID="ClientAddress" runat="server" />
            <br />
            <asp:Label ID="ClientContact" runat="server" />
            <br />
            <br />
            <asp:Label ID="ContractRequirements" runat="server" />
        </div>
        <div class="col-md-6">
            <h3>Active Contracts
                <asp:LinkButton ID="NewContract" runat="server" CssClass="btn btn-success btn-xs" Visible="false" OnClick="NewContract_Click">New</asp:LinkButton></h3>
            <asp:GridView ID="ClientContracts" runat="server" CssClass="table table-hover table-sm" ItemType="ScheduleStarterKit.ViewModels.ContractInfo" DataKeyNames="ContractId" AutoGenerateColumns="false" SelectedIndex="-1" OnSelectedIndexChanged="ClientContracts_SelectedIndexChanged">
                <Columns>
                    <asp:ButtonField CommandName="Select" Text="Edit" ControlStyle-CssClass="btn btn-secondary btn-xs" />
                   <asp:TemplateField Visible="false">
                       <ItemTemplate>
                           <asp:Label runat="server" ID="ContractId"
                            Text='<%# Eval("ContractId") %>'></asp:Label>
                       </ItemTemplate>
                   </asp:TemplateField>
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:TemplateField HeaderText="From">
                        <ItemTemplate><%# Item.From.ToLongDateString() %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="To">
                        <ItemTemplate><%# Item.To.ToLongDateString() %></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Panel ID="AddEditPanel" runat="server" Visible="false">
                <hr />
                <asp:Label ID="Label1" runat="server" AssociatedControlID="ContractTitle">Contract Title</asp:Label>
                <asp:TextBox ID="ContractTitle" runat="server" CssClass="form-control" />
                <asp:Label ID="Label2" runat="server" AssociatedControlID="Requirements">Requirements</asp:Label>
                <asp:TextBox ID="Requirements" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                <asp:Label ID="Label3" runat="server" AssociatedControlID="FromDate">Duration</asp:Label>
                <%-- Phase 3 Markup goes here --%>
                <div class="form-inline">
                    <div class="form-group row">
                        <div class="input-group">
                            <div class="input-group-prepend">
                                <span class="input-group-text">From</span>
                            </div>
                            <asp:TextBox ID="FromDate" runat="server" CssClass="form-control" TextMode="Date" />
                            <div class="input-group-append">
                                <div class="input-group-prepend">
                                    <span class="input-group-text">To</span>
                                </div>
                                <asp:TextBox ID="ToDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>
                        &nbsp;&nbsp;
                        <asp:LinkButton ID="SaveContract" runat="server" CssClass="btn btn-primary" OnClick="SaveContract_Click">Save</asp:LinkButton>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <div class="row">
        <div class="col-md-6">
            <h4>Available Skills</h4>
            <asp:GridView ID="AvailableSkills" runat="server" CssClass="table table-hover table-sm" ItemType="ScheduleStarterKit.ViewModels.SkillSummary" DataKeyNames="SkillId" AutoGenerateColumns="false"  OnRowCommand="AvailableSkill_RowCommand" >
                <EmptyDataTemplate><i>No available skills</i></EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:HiddenField ID="SkillId" runat="server" Value="<%# Item.SkillId %>" />
                            <asp:Label ID="Description" runat="server" Text="<%# Item.Description %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Candidates/Ticket">
                        <ItemTemplate>
                            <asp:Label ID="CandidateCount" runat="server" Text="<%# Item.CandidateCount %>" />
                            Candidates
                            /
                            <asp:CheckBox ID="Ticket" runat="server" Checked="<%# Item.RequiresTicket %>" Text="Ticket" Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:ButtonField 
                       buttontype="Button" Text="Add" CommandName="Add" ControlStyle-CssClass="btn btn-success btn-xs" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="col-md-6">
            <h4>Required Skills</h4>
            <asp:GridView ID="RequiredSkills" runat="server" CssClass="table table-hover table-sm" ItemType="ScheduleStarterKit.ViewModels.SkillSummary" DataKeyNames="SkillId" AutoGenerateColumns="false" OnRowCommand="RequiredSkills_RowCommand" >
                <EmptyDataTemplate><i>No required skills</i></EmptyDataTemplate>
                <Columns>
                    <asp:ButtonField Text="Remove" CommandName="Remove" ControlStyle-CssClass="btn btn-danger btn-xs" />
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:HiddenField ID="SkillId" runat="server" Value="<%# Item.SkillId %>" />
                            <asp:Label ID="Description" runat="server" Text="<%# Item.Description %>" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Candidates/Ticket">
                        <ItemTemplate>
                            <asp:Label ID="CandidateCount" runat="server" Text="<%# Item.CandidateCount %>" />
                            Candidates
                            /
                            <asp:CheckBox ID="Ticket" runat="server" Checked="<%# Item.RequiresTicket %>" Text="Ticket" Enabled="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Required" ItemStyle-CssClass="col-md-1">
                        <ItemTemplate>
                            <asp:TextBox ID="Required" runat="server" Text="<%# Item.Required %>"
                                CssClass="form-control form-control-sm" TextMode="Number" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
