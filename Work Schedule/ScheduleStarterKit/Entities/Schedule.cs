namespace ScheduleStarterKit.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    internal partial class Schedule
    {
        public int ScheduleID { get; set; }

        [Column(TypeName = "date")]
        public DateTime Day { get; set; }

        public int ShiftID { get; set; }

        public int EmployeeID { get; set; }

        [Column(TypeName = "money")]
        public decimal HourlyWage { get; set; }

        public bool OverTime { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual Shift Shift { get; set; }
    }
}
