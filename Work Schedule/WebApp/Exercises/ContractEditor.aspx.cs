using ScheduleStarterKit.BLL;
using ScheduleStarterKit.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Exercises
{
    public partial class ContractEditor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region error handling
        protected void SelectCheckForException(object sender,
                                       ObjectDataSourceStatusEventArgs e)
        {
            MessageUserControl.HandleDataBoundException(e);
        }
        protected void InsertCheckForException(object sender,
                                              ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                MessageUserControl.ShowInfo("Success", "Album has been added.");
            }
            else
            {
                MessageUserControl.HandleDataBoundException(e);
            }
        }
        protected void UpdateCheckForException(object sender,
                                               ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                MessageUserControl.ShowInfo("Success", "Album has been updated.");
            }
            else
            {
                MessageUserControl.HandleDataBoundException(e);
            }
        }
        protected void DeleteCheckForException(object sender,
                                                ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                MessageUserControl.ShowInfo("Success", "Album has been removed.");
            }
            else
            {
                MessageUserControl.HandleDataBoundException(e);
            }
        }

        #endregion
        #region Student Code - Free to modify
        protected void ClientDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

            RequiredSkills.DataSource = null;
            RequiredSkills.DataBind();
            AvailableSkills.DataSource = null;
            AvailableSkills.DataBind();
            AddEditPanel.Visible = false;
            if (ClientDropDown.SelectedIndex > 0)
            {
                MessageUserControl.TryRun(() =>
                {
                    var controller = new ContractController();
                    var info = controller.GetClientContracts(int.Parse(ClientDropDown.SelectedValue));
                    string contact = $"{info.Contact} ({info.Phone})";
                    string url = $"~/Images/{info.Contact.Replace(" ", string.Empty)}.jpg";
                    SetClient(info.Name, info.Address, contact, url, info.Contracts);
                    List<ContractInfo> display = controller.GetContractInfo(int.Parse(ClientDropDown.SelectedValue));
                    NewContract.Visible = true;
                    ClientContracts.DataSource = display;
                    ClientContracts.DataBind();

                }, "Client", "View client info and contracts");      
            }
            else
            {
                SetClient(string.Empty, string.Empty, string.Empty, string.Empty, null);
                NewContract.Visible = false;
                ClientContracts.DataSource = null;
                ClientContracts.DataBind();
            }
        }

        private void SetClient(string name, string address, string contact, string imageUrl, IEnumerable<ContractInfo> clientContractData )
        {
            ClientName.Text = name;
            ClientAddress.Text = address;
            ClientContact.Text = contact;
            ClientImage.ImageUrl = imageUrl;
        }
        #endregion

        protected void ClientContracts_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequiredSkills.DataSource = null;
            RequiredSkills.DataBind();
            AvailableSkills.DataSource = null;
            AvailableSkills.DataBind();
            int contractid = 0;
            contractid = int.Parse((ClientContracts.SelectedRow.FindControl("ContractId") as Label).Text);

            MessageUserControl.TryRun(() =>
            {
                var controller = new ContractController();
                var info = controller.GetContractDetail(contractid);
                if (info == null)
                {
                    AddEditPanel.Visible = false;
                    throw new Exception("Contract no longer of file");
                }
                else
                {
                    AddEditPanel.Visible = true;
                    ContractTitle.Text = info.Title;
                    Requirements.Text = info.Requirements;
                    FromDate.Text = info.From.ToString("yyyy-MM-dd");
                    ToDate.Text = info.To.ToString("yyyy-MM-dd");
                    SaveContract.Text = "Save #" + contractid.ToString();
                }
                List<SkillSummary> requiredSkills = controller.ListContractSkills(contractid);
                if (requiredSkills.Count> 0)
                {
                    RequiredSkills.DataSource = requiredSkills;
                    RequiredSkills.DataBind();
                }
                //load available skills
                List<SkillSummary> availableSkills = controller.ListSkills();
                if (availableSkills.Count > 0)
                {
                    if (requiredSkills.Count == 0)
                    {
                        AvailableSkills.DataSource = availableSkills;
                        AvailableSkills.DataBind();
                    }
                    else
                    {
                        IEnumerable<SkillSummary> remainingAvailableSkills = availableSkills.Except(requiredSkills);
                        AvailableSkills.DataSource = remainingAvailableSkills;
                        AvailableSkills.DataBind();
                    }
                }

            },"Contract Details","View contract details");     
        }

        protected void NewContract_Click(object sender, EventArgs e)
        {
            AddEditPanel.Visible = true;
            ContractTitle.Text = "";
            Requirements.Text = "";
            FromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            SaveContract.Text = "Save (New)";

            MessageUserControl.TryRun(() =>
            {
                var controller = new ContractController();
                List<SkillSummary> skills = controller.ListSkills();
                if (skills.Count > 0)
                {
                    AvailableSkills.DataSource = skills;
                    AvailableSkills.DataBind();
                }
            },"Contract Details","View contract details");

            RequiredSkills.DataSource = null;
            RequiredSkills.DataBind();
        }

        protected void AvailableSkill_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Add")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow agvrow = AvailableSkills.Rows[index];

                MessageUserControl.TryRun(() =>
                {
                    //make a copy of available skills table
                    List<SkillSummary> copyAvailableSkills = new List<SkillSummary>();
                    foreach (GridViewRow test in AvailableSkills.Rows)
                    {
                        SkillSummary line = new SkillSummary();
                        line.SkillId = int.Parse((test.FindControl("SkillId") as HiddenField).Value);
                        line.Description = (test.FindControl("Description") as Label).Text;
                        line.CandidateCount = int.Parse((test.FindControl("CandidateCount") as Label).Text);
                        CheckBox uck = test.FindControl("Ticket") as CheckBox;
                        if (uck.Checked)
                        {
                            line.RequiresTicket = true;
                        }
                        else
                        {
                            line.RequiresTicket = false;
                        }

                        line.Required = 2;
                        copyAvailableSkills.Add(line);
                    }


                    //get the data for the command row
                    int skillid = int.Parse((agvrow.FindControl("SkillId") as HiddenField).Value);
                    //delete the rows for required skills
                    copyAvailableSkills.RemoveAll(r => r.SkillId == skillid);
                    //bind data for available skill gridview
                    AvailableSkills.DataSource = copyAvailableSkills;
                    AvailableSkills.DataBind();

                    //make a copy of required skills table
                    List<SkillSummary> copyRequiredSkills = new List<SkillSummary>();
                    foreach (GridViewRow test in RequiredSkills.Rows)
                    {
                        SkillSummary skill = new SkillSummary();
                        skill.SkillId = int.Parse((test.FindControl("SkillId") as HiddenField).Value);
                        skill.Description = (test.FindControl("Description") as Label).Text;
                        skill.CandidateCount = int.Parse((test.FindControl("CandidateCount") as Label).Text);
                        CheckBox ck = test.FindControl("Ticket") as CheckBox;
                        if (ck.Checked)
                        {
                            skill.RequiresTicket = true;
                        }
                        else
                        {
                            skill.RequiresTicket = false;
                        }

                        skill.Required = byte.Parse((test.FindControl("Required") as TextBox).Text);
                        copyRequiredSkills.Add(skill);
                    }
                    //get the data for the command row
                    SkillSummary row = new SkillSummary();
                    row.SkillId = int.Parse((agvrow.FindControl("SkillId") as HiddenField).Value);
                    row.Description = (agvrow.FindControl("Description") as Label).Text;
                    CheckBox ticket = agvrow.FindControl("Ticket") as CheckBox;
                    if (ticket.Checked)
                    {
                        row.RequiresTicket = true;
                    }
                    else
                    {
                        row.RequiresTicket = false;
                    }
                    row.CandidateCount = int.Parse((agvrow.FindControl("CandidateCount") as Label).Text);
                    row.Required = 0;
                    //add the rows for required skills
                    copyRequiredSkills.Add(row);
                    //bind data for required skill gridview
                    RequiredSkills.DataSource = copyRequiredSkills;
                    RequiredSkills.DataBind();


                }, "Add Skills", "Display the skill set");

            }
        }
        protected void RequiredSkills_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow agvrow = RequiredSkills.Rows[index];

                MessageUserControl.TryRun(() =>
                {
                    //make a copy of available skills table
                    List<SkillSummary> copyAvailableSkills = new List<SkillSummary>();
                    foreach (GridViewRow test in AvailableSkills.Rows)
                    {
                        SkillSummary line = new SkillSummary();
                        line.SkillId = int.Parse((test.FindControl("SkillId") as HiddenField).Value);
                        line.Description = (test.FindControl("Description") as Label).Text;
                        line.CandidateCount = int.Parse((test.FindControl("CandidateCount") as Label).Text);
                        CheckBox uck = test.FindControl("Ticket") as CheckBox;
                        if (uck.Checked)
                        {
                            line.RequiresTicket = true;
                        }
                        else
                        {
                            line.RequiresTicket = false;
                        }

                        line.Required = 2;
                        copyAvailableSkills.Add(line);
                    }


                    //get the data for the command row
                    SkillSummary row = new SkillSummary();
                    row.SkillId = int.Parse((agvrow.FindControl("SkillId") as HiddenField).Value);
                    row.Description = (agvrow.FindControl("Description") as Label).Text;
                    CheckBox ticket = agvrow.FindControl("Ticket") as CheckBox;
                    if (ticket.Checked)
                    {
                        row.RequiresTicket = true;
                    }
                    else
                    {
                        row.RequiresTicket = false;
                    }
                    row.CandidateCount = int.Parse((agvrow.FindControl("CandidateCount") as Label).Text);
                    row.Required = 2;
                    //add the rows for required skills
                    copyAvailableSkills.Add(row);

                    //bind data for available skill gridview
                    AvailableSkills.DataSource = copyAvailableSkills;
                    AvailableSkills.DataBind();

                    //make a copy of required skills table
                    List<SkillSummary> copyRequiredSkills = new List<SkillSummary>();
                    foreach (GridViewRow test in RequiredSkills.Rows)
                    {
                        SkillSummary skill = new SkillSummary();
                        skill.SkillId = int.Parse((test.FindControl("SkillId") as HiddenField).Value);
                        skill.Description = (test.FindControl("Description") as Label).Text;
                        skill.CandidateCount = int.Parse((test.FindControl("CandidateCount") as Label).Text);
                        CheckBox ck = test.FindControl("Ticket") as CheckBox;
                        if (ck.Checked)
                        {
                            skill.RequiresTicket = true;
                        }
                        else
                        {
                            skill.RequiresTicket = false;
                        }

                        skill.Required = byte.Parse((test.FindControl("Required") as TextBox).Text);
                        copyRequiredSkills.Add(skill);
                    }
                    //get the data for the command row
                    int skillid = int.Parse((agvrow.FindControl("SkillId") as HiddenField).Value);

                    //delete the rows for required skills
                    copyRequiredSkills.RemoveAll(r => r.SkillId == skillid);
                    //bind data for required skill gridview
                    RequiredSkills.DataSource = copyRequiredSkills;
                    RequiredSkills.DataBind();


                }, "Delete Skills", "Display the skill set");

            }
        }

        protected void SaveContract_Click(object sender, EventArgs e)
        {
            int contractId;
            Contract contract = new Contract();
            if(SaveContract.Text=="Save (New)")
            {
                contract.ClientLocationId = ClientDropDown.SelectedIndex;
                contract.CurrentContractId = 0;
                contract.Title = ContractTitle.Text;
                contract.Requirements = Requirements.Text;
                contract.ContractPeriod = new Duration();
                contract.ContractPeriod.From = DateTime.Parse(FromDate.Text);
                contract.ContractPeriod.To = DateTime.Parse(ToDate.Text);
                List<SkilledWorkerCount> skills = new List<SkilledWorkerCount>();
                foreach (GridViewRow item in RequiredSkills.Rows)
                {
                    SkilledWorkerCount skill = new SkilledWorkerCount();
                    skill.SkillId = int.Parse((item.FindControl("SkillId") as HiddenField).Value);
                    skill.WorkerCount = byte.Parse((item.FindControl("Required") as TextBox).Text);
                    skills.Add(skill);
                }
                contract.WorkerSkills = skills;
            }
            else
            {
                contract.ClientLocationId = ClientDropDown.SelectedIndex;
                contract.CurrentContractId = int.Parse((ClientContracts.SelectedRow.FindControl("ContractId") as Label).Text);
                contract.Title = ContractTitle.Text;
                contract.Requirements = Requirements.Text;
                contract.ContractPeriod = new Duration();
                contract.ContractPeriod.From = DateTime.Parse(FromDate.Text);
                contract.ContractPeriod.To = DateTime.Parse(ToDate.Text);
                List<SkilledWorkerCount> skills = new List<SkilledWorkerCount>();
                
                foreach (GridViewRow item in RequiredSkills.Rows)
                {
                    SkilledWorkerCount skill = new SkilledWorkerCount();
                    skill.SkillId = int.Parse((item.FindControl("SkillId") as HiddenField).Value);
                    skill.WorkerCount = byte.Parse((item.FindControl("Required") as TextBox).Text);
                    skills.Add(skill);
                }
                contract.WorkerSkills = skills;
            }

            MessageUserControl.TryRun(() =>
            {
                var controller = new ContractController();
                contractId = controller.Save(contract);
            }, "Save contract", "View saving details");

            //rebind the data
            var controller2 = new ContractController();
            List<ContractInfo> display = controller2.GetContractInfo(int.Parse(ClientDropDown.SelectedValue));
            NewContract.Visible = true;
            ClientContracts.DataSource = display;
            ClientContracts.DataBind();
        }
    }
}