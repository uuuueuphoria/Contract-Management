using ScheduleStarterKit.DAL;
using ScheduleStarterKit.ViewModels;
using ScheduleStarterKit.Entities;
using ScheduleStarterKit.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMIT2018Common.UserControls;

namespace ScheduleStarterKit.BLL
{
    [DataObject]
    public class ContractController
    {
        #region Query Methods
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<KeyValueOption<int>> ListClients()
        {
            using (var context = new WorkScheduleContext())
            {
                var result = context.Locations.Select(x => new KeyValueOption<int> { Key = x.LocationID, DisplayText = x.Name });
                return result.ToList();
            }
        }

        public ClientLocation GetClientContracts(int locationId)
        {
            using (var context = new WorkScheduleContext())
            {
                var result = from place in context.Locations
                             where place.LocationID == locationId
                             select new ClientLocation
                             {
                                 Name = place.Name,
                                 Address = place.Street + " " + place.City + ", " + place.Province,
                                 Phone = place.Phone,
                                 Contact = place.Contact,
                                 Contracts = from contract in place.PlacementContracts
                                             select new ContractInfo
                                             {
                                                 ContractId = contract.PlacementContractID,
                                                 Title = contract.Title,
                                                 Requirements = contract.Requirements,
                                                 Duration = new Duration
                                                 {
                                                     From = contract.StartDate,
                                                     To = contract.EndDate
                                                 }
                                             }
                             };
                return result.SingleOrDefault();
            }
        }

        public List<ContractInfo> GetContractInfo(int LocationID)
        {
            #region TODO: Student Code Here
            using (var context = new WorkScheduleContext())
            {

                var results = from x in context.PlacementContracts
                              where x.LocationID == LocationID && x.EndDate >= DateTime.Now
                              select new ContractInfo
                              {
                                  ContractId = x.PlacementContractID,
                                  Title = x.Title,
                                  From = x.StartDate,
                                  To = x.EndDate
                              };
                return results.ToList();
            }
            #endregion
        }

        public ContractInfo GetContractDetail(int contractId)
        {
            using (var context = new WorkScheduleContext())
            {
                var result = from x in context.PlacementContracts
                             where x.PlacementContractID == contractId
                             select new ContractInfo
                             {
                                 ContractId = contractId,
                                 Title = x.Title,
                                 Requirements = x.Requirements,
                                 From=x.StartDate,
                                 To=x.EndDate
                             };
                return result.SingleOrDefault();
            }
        }



        public List<SkillSummary> ListContractSkills(int contractId)
        {
            #region TODO: Student Code Here
            using (var context = new WorkScheduleContext())
            {
                var results = from x in context.ContractSkills
                              where x.ContractID==contractId
                              select new SkillSummary
                              {
                                  SkillId = x.SkillID,
                                  Description = x.Skill.Description,
                                  RequiresTicket = x.Skill.RequiresTicket,
                                  CandidateCount = x.Skill.EmployeeSkills.Count(),
                                  Required =x.NumberOfEmployees
                              };
                return results.ToList();
            }
            throw new NotImplementedException(nameof(ListContractSkills));
            #endregion
        }


        public SkillSummary GetSkills(int skillId)
        {
            using (var context = new WorkScheduleContext())
            {
                var skill = from x in context.Skills
                            where x.SkillID == skillId
                            select new SkillSummary
                            {
                                SkillId = x.SkillID,
                                Description = x.Description,
                                RequiresTicket = x.RequiresTicket,
                                CandidateCount = x.EmployeeSkills.Count(),
                                Required = (byte)x.ContractSkills.Count()
                            };
                return skill.SingleOrDefault();
            }
        }
        public List<SkillSummary> ListSkills()
        {
            #region TODO: Student Code Here
            using (var context = new WorkScheduleContext())
            {
                var results = from x in context.Skills
                              select new SkillSummary
                              {
                                  SkillId = x.SkillID,
                                  Description = x.Description,
                                  RequiresTicket = x.RequiresTicket,
                                  CandidateCount = x.EmployeeSkills.Count(),
                                  Required = (byte)x.ContractSkills.Count()          
                             };
                return results.ToList();
            }
            throw new NotImplementedException(nameof(ListSkills));
            #endregion
        }
        #endregion

        #region Command Methods
        private List<string> errors = new List<string>();
        public int Save(Contract contract)
        {
            #region TODO: Student Code Here
            PlacementContract placementContract = null;
            ContractSkill contractSkill = null;
            Location location = null;
            using (var context=new WorkScheduleContext())
            {
                if (string.IsNullOrEmpty(contract.Title))
                {
                    errors.Add("Contract title is required");
                }else if (contract.Title.Length < 5)
                {
                    errors.Add("Contract title must be at least 5 characters");
                }
                if (contract.ContractPeriod.From > contract.ContractPeriod.To)
                {
                    errors.Add("Contract period is invalid, start date must earlier than end date");
                }else if (contract.ContractPeriod.To < DateTime.Now)
                {
                    errors.Add("Contract end date must be in the future");
                }
                Skill exists = null;
                foreach(SkilledWorkerCount skill in contract.WorkerSkills)
                {
                    exists = (from x in context.Skills
                              where x.SkillID == skill.SkillId
                              select x).FirstOrDefault();
                    if (exists == null)
                    {
                        errors.Add($"{skill.SkillId} is not exist");
                    }
                    if(skill.WorkerCount<1 || skill.WorkerCount > 4)
                    {
                        errors.Add($"{exists.Description} required worker must between 1 and 4. ");
                    }
                }
                if (contract.CurrentContractId == 0)
                {
                    if (contract.ContractPeriod.From < DateTime.Now)
                    {
                        errors.Add("New contract cannot start in a past date");
                    }
                    location = context.Locations.Find(contract.ClientLocationId);
                    if (location == null)
                    {
                        errors.Add($"Location: {contract.ClientLocationId} is not on the file. ");
                    }
                    if (errors.Count == 0)
                    {
                        //create new contract
                        placementContract = new PlacementContract();
                        placementContract.LocationID = contract.ClientLocationId;
                        placementContract.Title = contract.Title;
                        placementContract.StartDate = contract.ContractPeriod.From;
                        placementContract.EndDate = contract.ContractPeriod.To;
                        placementContract.Requirements = contract.Requirements;
                        placementContract.Cancellation = null;
                        placementContract.Reason = null;
                        context.PlacementContracts.Add(placementContract);
                        foreach (SkilledWorkerCount skill in contract.WorkerSkills)
                        {
                            contractSkill = new ContractSkill();
                            contractSkill.SkillID = skill.SkillId;
                            contractSkill.NumberOfEmployees = skill.WorkerCount;
                            placementContract.ContractSkills.Add(contractSkill);
                        }
                    }
                }
                else
                {
                    location = (from x in context.Locations
                                where x.LocationID == contract.ClientLocationId
                                select x).FirstOrDefault();
                    if (location == null)
                    {
                        errors.Add($"Location {contract.ClientLocationId} is not exist");
                    }
                    placementContract = context.PlacementContracts.Find(contract.CurrentContractId);
                    if (placementContract == null)
                    {
                        errors.Add("Contract is not exist. ");
                    }
                    else if (placementContract.Cancellation != null)
                    {
                        errors.Add("The contract is not active. ");
                    }
                    //update the contract
                    placementContract.LocationID = contract.ClientLocationId;
                    placementContract.Title = contract.Title;
                    placementContract.StartDate = contract.ContractPeriod.From;
                    placementContract.EndDate = contract.ContractPeriod.To;
                    placementContract.Requirements = contract.Requirements;
                    context.Entry(placementContract).State = System.Data.Entity.EntityState.Modified;

                    //delete skills
                    var currentContractSkills = (from x in context.ContractSkills
                                                 where x.ContractID == placementContract.PlacementContractID
                                                 select x).ToList();
                    foreach(ContractSkill skill in currentContractSkills)
                    {
                        context.ContractSkills.Remove(skill);
                    }
                    //re-add the skills
                    foreach(SkilledWorkerCount skill in contract.WorkerSkills)
                    {
                        contractSkill = new ContractSkill();
                        contractSkill.SkillID = skill.SkillId;
                        contractSkill.ContractID = contract.CurrentContractId;
                        contractSkill.NumberOfEmployees = skill.WorkerCount;
                        context.ContractSkills.Add(contractSkill);
                    }
                    
                }
                if (errors.Count > 0)
                {
                    throw new BusinessRuleException("your transaction contains following errors: ", errors);
                }
                context.SaveChanges();
                return placementContract.PlacementContractID;
            }
            #endregion
        }
        #endregion
    }
}
