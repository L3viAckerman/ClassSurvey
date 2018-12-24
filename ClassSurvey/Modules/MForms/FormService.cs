using ClassSurvey.Entities;
using ClassSurvey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassSurvey.Entities;
using ClassSurvey.Entities;
using ClassSurvey.Models;

namespace ClassSurvey.Modules.MForms
{
    

    public class FormService :CommonService, IFormService
    {
        public int Count(UserEntity userEntity, FormSearchEntity FormSearchEntity)
        {
            if (FormSearchEntity == null) FormSearchEntity = new FormSearchEntity();
            IQueryable<Form> Forms = context.Forms;
            Forms = Apply(Forms, FormSearchEntity);
            return Forms.Count();
        }
        
        public List<FormEntity> List(UserEntity userEntity, FormSearchEntity FormSearchEntity)
        {
            if (FormSearchEntity == null) FormSearchEntity = new FormSearchEntity();
            IQueryable<Form> Forms = context.Forms;
            Forms = Apply(Forms, FormSearchEntity);
            //Forms = FormSearchEntity.SkipAndTake(Forms);
            return Forms.Select(l => new FormEntity(l)).ToList();
        }

        public FormEntity Get(UserEntity userEntity, Guid FormId)
        {
            Form Form = context.Forms.FirstOrDefault(c => c.Id == FormId); ///add include later
            if (Form == null) throw new NotFoundException("Form Not Found");
            return new FormEntity(Form);
        }
        public FormEntity Update(UserEntity userEntity, Guid FormId, FormEntity FormEntity)
        {
            if (FormValidator(FormEntity))
            {
                Form Form = context.Forms.FirstOrDefault(c => c.Id == FormId); //add include later
                if (Form == null) throw new NotFoundException("Form Not Found");
                Form updateForm = new Form(FormEntity);
                updateForm.CopyTo(Form);
                context.SaveChanges();
                return new FormEntity(Form);
            }
            throw new BadRequestException("Cannot update");
            
        }

        public FormEntity Create(UserEntity userEntity, FormEntity FormEntity)
        {
            if (FormValidator(FormEntity))
            {
                Form Form = new Form(FormEntity);
                Form.Id = Guid.NewGuid();
                context.Forms.Add(Form);
                context.SaveChanges();
                return new FormEntity(Form);
            }
            throw new BadRequestException("Cannot Create");

          
        }

        
        public bool Delete(UserEntity userEntity, Guid FormId)
        {
            var CurrentForm = context.Forms.FirstOrDefault(c => c.Id == FormId);
            if (CurrentForm == null) return false;
            FormEntity FormEntity = new FormEntity(CurrentForm);
            if (FormValidator(FormEntity))
            {
                
                context.Forms.Remove(CurrentForm);
                context.SaveChanges();
                return true;
            }

            return false;
        }
        private IQueryable<Form> Apply(IQueryable<Form> Forms, FormSearchEntity FormSearchEntity)
        {
            if (FormSearchEntity.StudentClassId != Guid.Empty)
            {
                Forms = Forms.Where(vs => vs.StudentClassId.Equals(FormSearchEntity.StudentClassId));
            }

            return Forms;
        }
        private bool FormValidator(FormEntity FormEntity)
        {

            StudentClass studentClass = context.StudentClasses.Where(sc => sc.Id == FormEntity.StudentClassId).FirstOrDefault();
            if (studentClass == null) return false;
            Class Class = context.Classes.Where(c => c.Id == studentClass.ClassId).FirstOrDefault();
            if (Class == null) return false;
            if (DateTime.Now >= Class.OpenedDate && DateTime.Now <= Class.ClosedDate)
                return true;
            return false;
        }
    }
}
