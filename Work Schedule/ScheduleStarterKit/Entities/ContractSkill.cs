namespace ScheduleStarterKit.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class ContractSkill
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContractID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SkillID { get; set; }

        public byte NumberOfEmployees { get; set; }

        public virtual PlacementContract PlacementContract { get; set; }

        public virtual Skill Skill { get; set; }
    }
}
