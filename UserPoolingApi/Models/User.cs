using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserPoolingApi.Enums;

namespace UserPoolingApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Filename { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public ICollection<UserSkills> UserSkills { get; set; }
    }
}
