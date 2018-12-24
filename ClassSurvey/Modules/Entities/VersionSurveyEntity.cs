using ClassSurvey.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassSurvey.Entities;

namespace ClassSurvey.Entities
{
    public partial class VersionSurveyEntity : BaseEntity
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> ContentCategory { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<FormEntity> Forms { get; set; }
        public VersionSurveyEntity() : base()
        {

        }
        public VersionSurveyEntity(VersionSurvey versionSurvey, params object[] args) : base(versionSurvey)
        {
            if (!String.IsNullOrEmpty(this.Content))
            {
                ContentCategory = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.Content);
            }
                
            foreach (var arg in args)
            {
                if(arg is ICollection<Form> forms)
                {
                    this.Forms = forms.Select(s => new FormEntity(s)).ToList();
                }
            }
        }
    }
    public partial class VersionSurveySearchEntity : FilterEntity
    {
        public Guid Id { get; set; }
        public int? Version { get; set; }
        public string Year { get; set; }
    }
    
}
