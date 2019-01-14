using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTROLibrary.Model
{
    public class PASolution
    {
        public string SolutionId { get; set; }

        public string Email { get; set; }

        public string Summary { get; set; }

        public string FullName { get; set; }

        public string AssignedTo { get; set; }

        public string State { get; set; }

        public string OrganizationName { get; set; }

        public string Original_incoming_email { get; set; }

        public string Internal_analysis { get; set; }

    }
}
