using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserPoolingApi.Enums
{
    public enum StatusEnum
    {
            New = 0,
            Viewed = 1,
            Contacted = 2,
            Interview = 3,
            SecondInterview = 4,
            FinalInterview = 5,
            Hired = 6,
            Rejected = 7
    }
}
