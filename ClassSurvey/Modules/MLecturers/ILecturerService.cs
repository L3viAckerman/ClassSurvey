using ClassSurvey.Modules.MLecturers.Entity;
using ClassSurvey.Modules.MUsers.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassSurvey.Modules.MLecturers
{
    public interface ILecturerService : ITransientService
    {
        int Count(UserEntity userEntity, LecturerSearchEntity LecturerSearchEntity);
        List<LecturerEntity> List(UserEntity userEntity, LecturerSearchEntity LecturerSearchEntity);
        LecturerEntity Get(UserEntity userEntity, Guid LecturerId);
        LecturerEntity Update(UserEntity userEntity, Guid LecturerId, LecturerEntity lecturerEntity);
        bool Delete(UserEntity userEntity, Guid LecturerId);
        List<LecturerEntity> CreateFromExcel(byte[] data);
        LecturerEntity Create(LecturerExcelModel LecturerExcelModel);
    }
}
