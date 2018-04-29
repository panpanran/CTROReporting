using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Attitude_Loose.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserProfileId { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public bool? Gender { get; set; }

        public string Address { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string City { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string State { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Country { get; set; }

        public double? ZipCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public double? ContactNo { get; set; }

        public bool RequestSent { get; set; }

        public bool Following { get; set; }

        public string UserName { get; set; }
    }

    public class UserProfileFormModel
    {
        public UserProfileFormModel()
        {
            DateEdited = DateTime.Now;
            //DateOfBirth = DateTime.Now;
        }

        public int UserProfileId { get; set; }

        public DateTime DateEdited { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public bool? Gender { get; set; }

        public string Address { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string City { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string State { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Country { get; set; }

        public double? ZipCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        public double? ContactNo { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Internet URL")]
        public string Url { get; set; }

        public bool IsUrl { get; set; }

        [Display(Name = "Local file")]
        public HttpPostedFileBase File { get; set; }

        public string LocalPath { get; set; }

        public bool IsFile { get; set; }

        [Range(0, int.MaxValue)]
        public int X { get; set; }

        [Range(0, int.MaxValue)]
        public int Y { get; set; }

        [Range(1, int.MaxValue)]
        public int Width { get; set; }

        [Range(1, int.MaxValue)]
        public int Height { get; set; }

    }
}