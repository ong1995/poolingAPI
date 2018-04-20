using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserPoolingApi.ViewModels
{
    public class UserSkillViewModel
    {
        public int UserSkillsId { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public DisplayUserViewModel User { get; set; }
        public SkillViewModel Skill { get; set; }
    }
}
