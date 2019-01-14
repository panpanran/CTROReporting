using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CTROLibrary.Model
{
    public class UserProfile
    {
        public UserProfile()
        {
            DateEdited = DateTime.Now;
        }
        [Key]
        public int UserProfileId { get; set; }

        public DateTime DateEdited { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool? Gender { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public double? ZipCode { get; set; }

        public double? ContactNo { get; set; }

        [Column("Id")]
        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}