using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MOperation.Entity
{
    public class OperationEntity
    {
        public Guid Id { get; set; }
        public string Link { get; set; }
        public int? Role { get; set; }
        public string Method { get; set; }
    }
}
