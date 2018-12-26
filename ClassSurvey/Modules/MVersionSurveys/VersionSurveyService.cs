using System;
using System.Collections.Generic;
using System.Linq;
using ClassSurvey.Models;
using ClassSurvey.Modules.MUsers.Entity;
using ClassSurvey.Modules.MVersionSurveys.Entity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace ClassSurvey.Modules.MVersionSurveys
{
    public class VersionSurveyService : CommonService, IVersionSurveyService
    {
        public int Count(UserEntity userEntity, VersionSurveySearchEntity VersionSurveySearchEntity)
        {
            if (VersionSurveySearchEntity == null) VersionSurveySearchEntity = new VersionSurveySearchEntity();
            IQueryable<VersionSurvey> VersionSurveys = context.VersionSurveys;
            VersionSurveys = Apply(VersionSurveys, VersionSurveySearchEntity);
            return VersionSurveys.Count();
        }

        public List<VersionSurveyEntity> List(UserEntity userEntity, VersionSurveySearchEntity VersionSurveySearchEntity)
        {
            if (VersionSurveySearchEntity == null) VersionSurveySearchEntity = new VersionSurveySearchEntity();
            IQueryable<VersionSurvey> VersionSurveys = context.VersionSurveys;
            VersionSurveys = Apply(VersionSurveys, VersionSurveySearchEntity);
            //VersionSurveys = VersionSurveySearchEntity.SkipAndTake(VersionSurveys);
            return VersionSurveys.Select(l => new VersionSurveyEntity(l)).ToList();
        }

        public VersionSurveyEntity Get(UserEntity userEntity, Guid VersionSurveyId)
        {
            VersionSurvey VersionSurvey = context.VersionSurveys.FirstOrDefault(c => c.Id == VersionSurveyId); ///add include later
            if (VersionSurvey == null) throw new NotFoundException("VersionSurvey not found!");
            return new VersionSurveyEntity(VersionSurvey);
        }
        public VersionSurveyEntity Update(UserEntity userEntity, Guid VersionSurveyId, VersionSurveyEntity VersionSurveyEntity)
        {
            if(VersionSurveyEntity.Content.Equals(String.Empty) 
               || VersionSurveyEntity.Content == null
               || VersionSurveyEntity.Version == -1)
                throw new BadRequestException("Field must not empty!");
            VersionSurvey VersionSurvey = context.VersionSurveys.FirstOrDefault(c => c.Id == VersionSurveyId); //add include later
            if (VersionSurvey == null) throw new NotFoundException("VersionSurvey not found!");
            VersionSurvey updateVersionSurvey = new VersionSurvey(VersionSurveyEntity);
            DateTime? createdDate = VersionSurvey.CreatedDate;
            updateVersionSurvey.CopyTo(VersionSurvey);
            VersionSurvey.ModifiedDate = DateTime.Now;
            VersionSurvey.CreatedDate = createdDate;
            context.SaveChanges();
            return new VersionSurveyEntity(VersionSurvey);
        }

        public VersionSurveyEntity Create(UserEntity userEntity, VersionSurveyEntity versionSurveyEntity)
        {
            if(versionSurveyEntity.Content.Equals(String.Empty) 
               || versionSurveyEntity.Content == null 
               || versionSurveyEntity.Version == -1)
                throw new BadRequestException("Field must not empty!");
            VersionSurvey versionSurvey = new VersionSurvey(versionSurveyEntity);
            versionSurvey.Id = Guid.NewGuid();
            versionSurvey.CreatedDate = DateTime.Now;
            context.VersionSurveys.Add(versionSurvey);
            context.SaveChanges();
            return new VersionSurveyEntity(versionSurvey);
        }
        public bool Delete(UserEntity userEntity, Guid VersionSurveyId)
        {
            var CurrentVersionSurvey = context.VersionSurveys.FirstOrDefault(c => c.Id == VersionSurveyId);
            if (CurrentVersionSurvey == null) return false;
            context.VersionSurveys.Remove(CurrentVersionSurvey);
            context.SaveChanges();
            return true;
        }
        private IQueryable<VersionSurvey> Apply(IQueryable<VersionSurvey> VersionSurveys, VersionSurveySearchEntity VersionSurveySearchEntity)
        {
            if (VersionSurveySearchEntity.Version.HasValue)
            {
                VersionSurveys = VersionSurveys.Where(vs => vs.Version.Equals(VersionSurveySearchEntity.Version.Value));
            }
            if (VersionSurveySearchEntity.Year != null)
            {
                VersionSurveys = VersionSurveys.Where(vs => vs.CreatedDate.Value.Year.ToString().Equals(VersionSurveySearchEntity.Year));
            }
            return VersionSurveys;
        }
    }
}