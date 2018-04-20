using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserPoolingApi.Models
{
    public class UserSkills
    {
        public int UserSkillsId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Skill")]
        public int SkillId { get; set; }
        public User User { get; set; }
        public Skill Skill { get; set; }
    }
}
