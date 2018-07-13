using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Attitude_Loose.Models
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