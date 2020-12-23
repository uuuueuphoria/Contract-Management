using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleStarterKit.ViewModels
{
    #region Query POCOs
    public class Duration
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public class ClientLocation
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Contact { get; set; }
        public IEnumerable<ContractInfo> Contracts { get; set; }
            = new HashSet<ContractInfo>();
    }
    public class ContractInfo
    {
        public int ContractId { get; set; }
        public string Title { get; set; }
        public Duration Duration { get; set; }
        public string Requirements { get; set; }
        public DateTime From { get; internal set; }
        public DateTime To { get; internal set; }
    }
    public class SkillSummary
    {
        public int SkillId { get; set; }
        public string Description { get; set; }
        public bool RequiresTicket { get; set; }
        public int CandidateCount { get; set; }
        public byte Required { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SkillSummary summary &&
                   SkillId == summary.SkillId &&
                   Description == summary.Description &&
                   RequiresTicket == summary.RequiresTicket;
        }

        public override int GetHashCode()
        {
            var hashCode = -993622047;
            hashCode = hashCode * -1521134295 + SkillId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            hashCode = hashCode * -1521134295 + RequiresTicket.GetHashCode();
            return hashCode;
        }
    }
    #endregion

    #region Command POCOs
    public class SkilledWorkerCount
    {
        public int SkillId { get; set; }
        public byte WorkerCount { get; set; }
    }
    public class Contract
    {
        public int ClientLocationId { get; set; }
        public int CurrentContractId { get; set; }
        public string Title { get; set; }
        public string Requirements { get; set; }
        public Duration ContractPeriod { get; set; }
        public IEnumerable<SkilledWorkerCount> WorkerSkills { get; set; }
    }
    #endregion
}
