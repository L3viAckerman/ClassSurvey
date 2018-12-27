using ClassSurvey.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using ClassSurvey.Modules.MUsers.Entity;
using ClassSurvey.Modules.MClasses.Entity;

namespace ClassSurvey.Modules.MClasses
{

    public class ClassService : CommonService, IClassService
    {
        public ClassService() : base()
        {
        }

        public int Count(UserEntity userEntity, ClassSearchEntity classSearchEntity)
        {
            if (classSearchEntity == null) classSearchEntity = new ClassSearchEntity();
            IQueryable<Class> classes = context.Classes;
            classes = Apply(classes, classSearchEntity);
            return classes.Count();
        }

        public float CountSurvey(UserEntity UserEntity, Guid ClassId)
        {
            float count = 0;
            List<StudentClass> studentClasses = context.StudentClasses.Include(sc => sc.Forms)
                .Where(sc => sc.ClassId == ClassId).ToList();
            var Class = context.Classes.FirstOrDefault(c => c.Id == ClassId);
            if (Class == null) throw new BadRequestException("Class not found!");
            foreach (var studentClass in studentClasses)
            {
                if (studentClass.Forms != null && studentClass.Forms.Count > 0)
                {
                    count += 1;
                }

            }

            return count / Class.StudentNumber;
        }
        public List<ClassEntity> List(UserEntity userEntity, ClassSearchEntity classSearchEntity)
        {
            if (classSearchEntity == null) classSearchEntity = new ClassSearchEntity();
            IQueryable<Class> classes = context.Classes.Include(s => s.StudentClasses).ThenInclude(sc => sc.Student)
                .Include(s => s.VersionSurvey).Include(c => c.Lecturer);

            classes = Apply(classes, classSearchEntity);
            //List<Class> list = classes.ToList();
            //classes = classSearchEntity.SkipAndTake(classes);

            return classes.Select(c => new ClassEntity(c, c.VersionSurvey, c.StudentClasses, c.Lecturer)).ToList();
        }


        public ClassEntity Get(UserEntity userEntity, Guid ClassId)
        {
            Class Class = context.Classes.Include(c => c.Lecturer).Include(c => c.StudentClasses).ThenInclude(sc => sc.Student).Include(s => s.VersionSurvey).FirstOrDefault(c => c.Id == ClassId);
            if (Class == null) throw new NotFoundException("Class not found!");
            List<Class> classes = context.Classes.Where(c => c.Semester == Class.Semester).ToList();
            if (//Class.OpenedDate != null && Class.ClosedDate != null && 
                //DateTime.Now > Class.OpenedDate  && DateTime.Now > Class.ClosedDate &&
                //string.IsNullOrEmpty(Class.M)
                true)
            {
                Average(Class.Semester);
                Average1(Class.Semester);
                Average2(Class.Semester);
                StandardDeviation(Class.Semester);
                StandardDeviation1(Class.Semester);
                StandardDeviation2(Class.Semester);
            }
            return new ClassEntity(Class, Class.VersionSurvey, Class.StudentClasses, Class.Lecturer);
        }

        public ClassEntity Update(UserEntity userEntity, Guid ClassId, ClassEntity classEntity)
        {
            Class Class = context.Classes.Include(c => c.StudentClasses).Include(s => s.VersionSurvey).FirstOrDefault(c => c.Id == ClassId);
            if (Class == null) throw new NotFoundException("Class not found!");
            Class updateClass = new Class(classEntity);
            updateClass.CopyTo(Class);
            context.SaveChanges();
            List<StudentClass> studentClasses = context.StudentClasses.Include(f => f.Forms).Where(sc => sc.ClassId == ClassId).ToList();
            List<StudentClass> Insert, Update, Delete;
            List<StudentClass> newStudentClasses = classEntity.StudentClasses == null
                ? new List<StudentClass>()
                : classEntity.StudentClasses.Select(sc => new StudentClass(sc)).ToList();
            Common<StudentClass>.Split(newStudentClasses, studentClasses, out Insert, out Update, out Delete);
            if (Insert != null)
                foreach (var sc in Insert)
                {
                    sc.Id = Guid.NewGuid();
                    sc.ClassId = Class.Id;
                    studentClasses.Add(sc);
                    context.StudentClasses.Add(sc);
                }

            if (Update != null)
                foreach (var sc in Update)
                {
                    var curStudentClass = studentClasses.FirstOrDefault(s => s.Id == sc.Id);
                    Common<StudentClass>.Copy(sc, curStudentClass);
                }

            if (Delete != null)
                foreach (var sc in Delete)
                {
                    var deleteStudentClass = studentClasses.FirstOrDefault(s => sc.Id == s.Id);
                    foreach (var form in deleteStudentClass.Forms)
                    {
                        context.Forms.Remove(form);
                    }

                    studentClasses.Remove(sc);
                    context.SaveChanges();
                    context.StudentClasses.Remove(deleteStudentClass);
                }

            context.SaveChanges();

            return new ClassEntity(Class);
        }

        public bool Delete(UserEntity userEntity, Guid ClassId)
        {
            var CurrentClass = context.Classes.FirstOrDefault(c => c.Id == ClassId);
            if (CurrentClass == null) return false;
            var CurrentStudentClasses = context.StudentClasses.Include(sc => sc.Forms).Where(sc => sc.ClassId == ClassId).ToList();
            if (CurrentStudentClasses != null)
            {
                foreach (var sc in CurrentStudentClasses)
                {

                    foreach (var form in sc.Forms)
                    {
                        context.Forms.Remove(form);
                    }

                    context.SaveChanges();
                    context.StudentClasses.Remove(sc);

                }

                context.SaveChanges();
            }

            context.Classes.Remove(CurrentClass);
            context.SaveChanges();
            return true;
        }

        public ClassEntity Create(byte[] data)
        {
            Class newClass = new Class();
            newClass.Id = Guid.NewGuid();
            if (data != null)
            {
                try
                {
                    List<StudentExcelModel> studentModelEntities =
                        ConvertToIEnumrable<StudentExcelModel>(data).ToList();
                    string lecturerCode = GetPropValueFromExcel(data, "Mã cán bộ:").Trim();
                    if (lecturerCode == "") throw new BadRequestException("Cannot get lectureCode!");
                    var lecturer = context.Lecturers.FirstOrDefault(l => l.LecturerCode.Trim() == lecturerCode);
                    newClass.LecturerId = lecturer.Id;
                    newClass.Subject = GetPropValueFromExcel(data, "Môn học:");
                    newClass.ClassCode = GetPropValueFromExcel(data, "Lớp môn học:");
                    newClass.StudentNumber = studentModelEntities.Count(sme => sme.Code != null);
                    Console.WriteLine(newClass.Subject + "  " + newClass.ClassCode + "    " + lecturerCode);
                    var Students = context.Students;
                    context.Classes.Add(newClass);
                    foreach (var studentModel in studentModelEntities.Where(sme => sme.Code != null))
                    {
                        var student = Students.FirstOrDefault(s =>
                            s.Code.ToString().Trim().Equals(studentModel.Code.Trim()));
                        if (student == null) throw new BadRequestException("Student not existed!");
                        var StudentClass = new StudentClass();
                        Console.WriteLine(studentModel.Code);
                        StudentClass.Id = Guid.NewGuid();
                        StudentClass.StudentId = student.Id;
                        StudentClass.ClassId = newClass.Id;
                        context.StudentClasses.Add(StudentClass);
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.ToString());
                }
            }
            context.SaveChanges();
            return new ClassEntity(newClass);
        }

        private IQueryable<Class> Apply(IQueryable<Class> classes, ClassSearchEntity classSearchEntity)
        {
            if (classSearchEntity.ClassCode != null)
            {
                classes = classes.Where(c => c.ClassCode == classSearchEntity.ClassCode);
            }

            if (classSearchEntity.LecturerId != Guid.Empty)
            {
                classes = classes.Where(c => c.LecturerId == classSearchEntity.LecturerId);
            }
            if (classSearchEntity.VersionId != Guid.Empty)
            {
                classes = classes.Where(c => c.VersionSurveyId == classSearchEntity.VersionId);
            }
            if (classSearchEntity.Subject != null)
            {
                classes = classes.Where(c =>
                    c.Subject.Contains(classSearchEntity.Subject) || classSearchEntity.Subject.Contains(c.Subject));
            }

            if (classSearchEntity.Semester != null)
            {
                classes = classes.Where(c => c.Semester.Equals(classSearchEntity.Semester));
            }
            if (classSearchEntity.OpenedDate.HasValue && classSearchEntity.OpenedDate.Value != DateTime.MinValue)
            {
                classes = classes.Where(c => c.OpenedDate.Value.CompareTo(DateTime.Now) == -1);
            }
            if (classSearchEntity.ClosedDate.HasValue && classSearchEntity.ClosedDate.Value != DateTime.MinValue)
            {
                classes = classes.Where(c => c.ClosedDate.Value.CompareTo(DateTime.Now) == -1);
            }
            return classes;
        }
        private void Average(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();
            //List<Dictionary<string, float>> averages = new List<Dictionary<string, float>>();
            foreach (var Class in classes)
            {
                List<StudentClass> studentClasses = context.StudentClasses.Where(sc => sc.ClassId == Class.Id).ToList(); //Get All Student Classes Which have same ClassId
                if (studentClasses != null)
                {
                    List<Dictionary<string, double>> surveyResults = new List<Dictionary<string, double>>(); //Get Survey Result from all student classes that we found before
                    Dictionary<string, double> Ms = new Dictionary<string, double>(); //Get results with Key and M
                    foreach (var studentClass in studentClasses)
                    {
                        Form survey = context.Forms.FirstOrDefault(s => s.StudentClassId == studentClass.Id);
                        if (survey != null)
                        {
                            Dictionary<string, double> surveyContent = new Dictionary<string, double>();
                            surveyContent = JsonConvert.DeserializeObject<Dictionary<string, double>>(survey.Content);
                            surveyResults.Add(surveyContent);
                        }
                    }

                    if (surveyResults.Count > 0)
                    {
                        var keys = surveyResults[0].Keys; //Get all keys from survey results
                        foreach (var key in keys)
                        {
                            List<int> values = surveyResults.Select(sr => Convert.ToInt32(sr[key])).ToList(); //find values that have the same key
                            int sum = 0;
                            values.ForEach(v => sum += v);
                            double average = sum / values.Count();
                            Ms.Add(key, average);

                        }
                        Class.M = JsonConvert.SerializeObject(Ms);

                    }
                }


                //averages.Add(Ms);
            }
            //return averages;
            context.SaveChanges();
        }
        private void StandardDeviation(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();
            //List<Class> classes = context.Classes.ToList();
            //List<Dictionary<string, double>> deviations = new List<Dictionary<string, double>>();
            foreach (var Class in classes)
            {

                List<StudentClass> studentClasses = context.StudentClasses.Where(sc => sc.ClassId == Class.Id).ToList();
                if (studentClasses != null)
                {
                    List<Dictionary<string, double>> surveyResults = new List<Dictionary<string, double>>();
                    Dictionary<string, double> Stds = new Dictionary<string, double>();
                    foreach (var studentClass in studentClasses)
                    {
                        Form survey = context.Forms.FirstOrDefault(s => s.StudentClassId == studentClass.Id);
                        if (survey != null)
                        {
                            Dictionary<string, double> surveyContent =
                                JsonConvert.DeserializeObject<Dictionary<string, double>>(survey.Content);
                            surveyResults.Add(surveyContent);
                        }

                    }

                    if (surveyResults.Count > 0)
                    {
                        var keys = surveyResults[0].Keys;
                        foreach (var key in keys)
                        {
                            List<int> values = surveyResults.Select(sr => Convert.ToInt32(sr[key])).ToList();
                            if (values.Count == 0) return;
                            float sum = 0;
                            values.ForEach(v => sum += v);

                            float average = sum / values.Count();
                            double variance = 0;
                            values.ForEach(v => variance += Math.Pow((v - average), 2));
                            double deviation = Math.Sqrt(variance);
                            Stds.Add(key, deviation);
                        }
                        Class.Std = JsonConvert.SerializeObject(Stds);

                    }

                }

                //deviations.Add(Stds);
            }
            context.SaveChanges();
            //return deviations;
        }
        private void Average1(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();

            //List<Class> classes = context.Classes.ToList();
            foreach (var Class in classes)
            {
                string subject = Class.Subject;
                Guid LecturerId = Class.LecturerId;
                if (!String.IsNullOrEmpty(Class.M))
                {
                    Dictionary<string, string> Ms = JsonConvert.DeserializeObject<Dictionary<string, string>>(Class.M);
                    Dictionary<string, double> M1s = new Dictionary<string, double>();
                    List<string> keys = Ms.Keys.ToList();
                    foreach (var key in keys)
                    {
                        List<double> values = new List<double>();
                        foreach (var otherClass in classes.Where(c => c.LecturerId != LecturerId && c.Subject == subject))
                        {
                            if (!String.IsNullOrEmpty(otherClass.M))
                            {
                                Dictionary<string, string> M = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherClass.M);
                                values.Add(Convert.ToDouble(M[key]));
                            }

                        }
                        double sum = 0;
                        if (values.Count == 0) return;
                        values.ForEach(v => sum += v);
                        double M1 = sum / values.Count();
                        M1s.Add(key, M1);
                    }
                    Class.M1 = JsonConvert.SerializeObject(M1s);

                }

            }
            context.SaveChanges();
        }
        private void StandardDeviation1(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();

            foreach (var Class in classes)
            {
                if (!String.IsNullOrEmpty(Class.Std))
                {
                    string subject = Class.Subject;
                    Guid LecturerId = Class.LecturerId;
                    Dictionary<string, string> Stds = JsonConvert.DeserializeObject<Dictionary<string, string>>(Class.Std);
                    Dictionary<string, double> Std1s = new Dictionary<string, double>();
                    List<string> keys = Stds.Keys.ToList();
                    foreach (var key in keys)
                    {
                        List<double> values = new List<double>();
                        foreach (var otherClass in classes.Where(c => c.LecturerId != LecturerId && c.Subject == subject))
                        {
                            if (!String.IsNullOrEmpty(otherClass.Std))
                            {
                                Dictionary<string, string> Std = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherClass.Std);
                                values.Add(Convert.ToDouble(Std[key]));
                            }

                        }
                        double sum = 0;
                        if (values.Count == 0) return;
                        values.ForEach(v => sum += v);

                        double M1 = sum / values.Count();
                        double variance = 0;
                        values.ForEach(v => variance += Math.Pow((v - M1), 2));
                        double Std1 = Math.Sqrt(variance);
                        Std1s.Add(key, Std1);
                    }
                    Class.Std1 = JsonConvert.SerializeObject(Std1s);

                }

            }
            context.SaveChanges();
        }
        private void Average2(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();
            foreach (var Class in classes)
            {
                if (!string.IsNullOrEmpty(Class.M))
                {
                    string subject = Class.Subject;
                    Guid LecturerId = Class.LecturerId;
                    Dictionary<string, string> Ms = JsonConvert.DeserializeObject<Dictionary<string, string>>(Class.M);
                    Dictionary<string, double> M2s = new Dictionary<string, double>();
                    List<string> keys = Ms.Keys.ToList();
                    foreach (var key in keys)
                    {
                        List<double> values = new List<double>();
                        foreach (var otherClass in classes.Where(c => c.LecturerId == LecturerId && c.Subject != subject))
                        {
                            if (!String.IsNullOrEmpty(otherClass.M))
                            {
                                Dictionary<string, string> M = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherClass.M);
                                values.Add(Convert.ToDouble(M[key]));
                            }

                        }

                        if (values.Count == 0) return;
                        double sum = 0;
                        values.ForEach(v => sum += v);
                        double M2 = sum / values.Count();
                        M2s.Add(key, M2);
                    }
                    Class.M2 = JsonConvert.SerializeObject(M2s);
                }
            }
            context.SaveChanges();
        }
        private void StandardDeviation2(string Semester)
        {
            List<Class> classes = context.Classes.Where(c => c.Semester == Semester).ToList();
            foreach (var Class in classes)
            {
                if (!String.IsNullOrEmpty(Class.Std))
                {
                    string subject = Class.Subject;
                    Guid LecturerId = Class.LecturerId;
                    Dictionary<string, string> Stds = JsonConvert.DeserializeObject<Dictionary<string, string>>(Class.Std);
                    Dictionary<string, double> Std2s = new Dictionary<string, double>();
                    List<string> keys = Stds.Keys.ToList();
                    foreach (var key in keys)
                    {
                        List<double> values = new List<double>();
                        foreach (var otherClass in classes.Where(c => c.LecturerId == LecturerId && c.Subject != subject))
                        {
                            if (!String.IsNullOrEmpty(otherClass.Std))
                            {
                                Dictionary<string, string> Std = JsonConvert.DeserializeObject<Dictionary<string, string>>(otherClass.Std);
                                values.Add(Convert.ToDouble(Std[key]));
                            }

                        }

                        if (values.Count == 0) return;
                        double sum = 0;
                        values.ForEach(v => sum += v);
                        double M2 = sum / values.Count();
                        double variance = 0;
                        values.ForEach(v => variance += Math.Pow((v - M2), 2));
                        double Std2 = Math.Sqrt(variance);
                        //Console.WriteLine("std2:" + values.Count() + variance + " " + M2);
                        Std2s.Add(key, Std2);
                    }
                    Class.Std2 = JsonConvert.SerializeObject(Std2s);
                }


            }
            context.SaveChanges();
        }
    }

    public class StudentExcelModel
    {
        [Column(2)] public string Code { get; set; }
    }
}