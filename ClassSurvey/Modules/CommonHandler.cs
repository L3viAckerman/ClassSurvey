
namespace ClassSurvey.Modules
{ 
    public abstract class CommonHandler
    {
        //protected WebApplication4Entities WebApplication4Entities;
        public string Name { get; set; }
        public CommonHandler()
        {
            //WebApplication4Entities = new WebApplication4Entities(null);
        }
        public abstract void Handle(string json);
    }
}
