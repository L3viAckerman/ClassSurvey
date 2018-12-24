using ClassSurvey.Entities;
using ClassSurvey.Models;
using ClassSurvey.Modules.MClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace ClassSurvey.Modules.MSurveys
{
    
    public class SurveyService : CommonService, ISurveyService
    {
        public void CreateOrUpdate(UserEntity userEntity, SurveyEntity SurveyEntity)
        {
            foreach(var Id in SurveyEntity.ClassGuids)
            {
                Class Class = context.Classes.FirstOrDefault(c => c.Id == Id);
                Class.OpenedDate = DateTime.Parse(SurveyEntity.OpenedDate.ToString());
                Class.ClosedDate = DateTime.Parse(SurveyEntity.ClosedDate.ToString());
                Class.Semester = GetSemester(Class.OpenedDate.Value);
                //var StudentClasses = context.StudentClasses.Where(sc => sc.ClassId == Id).ToList();
                Class.VersionSurveyId = SurveyEntity.VersionSurveyId;
                
            }
            context.SaveChanges();
        }

        private string GetSemester(DateTime openedDate)
        {
            int year = openedDate.Year;
            int month = openedDate.Month;
            if ((month >= 1 && month <= 5) || month == 12)
            {
                if(month == 12) return "HK1 " + year + "-" + (year+1);
                return "HK1 " + (year-1) + "-" + (year);
            }
            else if (month >= 6 && month <= 8)
            {
                return "HK2 " + (year-1) + "-" + (year);
            }
            else
            {
                return "Ki he " + (year-1) + "-" + (year);
            }
        }
    }
}
