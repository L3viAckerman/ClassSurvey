
namespace ClassSurvey.Modules
{ 
    public abstract class CommonHandler
    {
        
        public string Name { get; set; }
        public CommonHandler()
        {
            
        }
        public abstract void Handle(string json);
    }
}
