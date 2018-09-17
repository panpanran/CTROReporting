using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWTicketLibrary.Model
{
    public class Participant
    {
        public string ParticipantId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Organization { get; set; }

        public string Phone { get; set; }
    }
}
