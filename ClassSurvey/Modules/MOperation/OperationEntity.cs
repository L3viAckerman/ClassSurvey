using ClassSurvey.Models;
using ClassSurvey.Modules.MUsers.Entity;
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
        public string Method { get; set; }
        public int? Role { get; set; }
        public OperationEntity() { }

        public OperationEntity(Operation Operation)
        {
            this.Id = Operation.Id;
            this.Link = Operation.Link;
            this.Method = Operation.Method;
            this.Role = Operation.Role;
        }

        public Operation ToModel(Operation Operation = null)
        {
            if (Operation == null)
            {
                Operation = new Operation();
                Operation.Id = Guid.NewGuid();
            }
            Operation.Id = this.Id;
            Operation.Link = this.Link;
            Operation.Method = this.Method;
            string role = string.Join(",", this.Role);
            Operation.Role = (int)Enum.Parse(typeof(ROLES), role);

            return Operation;
        }
    }

}
