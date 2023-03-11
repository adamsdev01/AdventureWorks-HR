using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanResources.Data.Models
{
    /// <summary>
    /// Lookup table containing the departments within the Adventure Works Cycles company.
    /// </summary>
    public partial class Department
    {
        public Department()
        {
            EmployeeDepartmentHistories = new HashSet<EmployeeDepartmentHistory>();
        }

        /// <summary>
        /// Primary key for Department records.
        /// </summary>
        public short DepartmentId { get; set; }
        /// <summary>
        /// Name of the department.
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Name of the group to which the department belongs.
        /// </summary>
        public string GroupName { get; set; } = null!;
        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<EmployeeDepartmentHistory> EmployeeDepartmentHistories { get; set; }

        [NotMapped]
        //this property will contain the row index for display purposes
        public int RowIndex { get; set; }
    }
}
