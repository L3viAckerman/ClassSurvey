using ClassSurvey.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MStudents
{
    public interface IStudentService : ITransientService
    {
        int Count(UserEntity userEntity, StudentSearchEntity StudentSearchEntity);
        List<StudentEntity> List(UserEntity userEntity, StudentSearchEntity StudentSearchEntity);
        StudentEntity Get(UserEntity userEntity, Guid StudentId);
        StudentEntity Update(UserEntity userEntity, Guid StudentId, StudentEntity StudentEntity);
        bool Delete(UserEntity userEntity, Guid StudentId);
        List<StudentEntity> CreateFromExcel(byte[] data);
        StudentEntity Create(StudentExcelModel StudentExcelModel);
        List<ClassEntity> GetClasses(Guid StudentId);

    }
}
