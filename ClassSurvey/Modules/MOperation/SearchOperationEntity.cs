using ClassSurvey.Models;
using System.Linq;

namespace ClassSurvey.Modules.MOperation
{
    public class SearchOperationEntity : FilterEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Method { get; set; }
        public string Role { get; set; }
        public IQueryable<Operation> ApplyTo(IQueryable<Operation> Operations)
        {
            //if (!string.IsNullOrEmpty(Name))
            //    Operations = Operations.Where(u => u.Name.ToLower().Equals(Name.ToLower()));
            //return Operations;
            return Operations;
        }
    }
}