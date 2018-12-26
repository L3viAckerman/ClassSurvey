using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ClassSurvey
{
    public class Common<T>
    {
        public static void Copy(T From, T To)
        {
            List<PropertyInfo> sources = From.GetType().GetProperties().ToList();
            List<PropertyInfo> destinations = To.GetType().GetProperties().ToList();
            foreach (PropertyInfo source in sources)
                if (source.Name.Equals("Cx"))
                    continue;
                else
                if (source.PropertyType == typeof(Guid) ||
                    source.PropertyType == typeof(Guid?) ||
                    source.PropertyType == typeof(string) ||
                    source.PropertyType == typeof(int) ||
                    source.PropertyType == typeof(int?) ||
                    source.PropertyType == typeof(long) ||
                    source.PropertyType == typeof(long?) ||
                    source.PropertyType == typeof(byte) ||
                    source.PropertyType == typeof(byte[]) ||
                    source.PropertyType == typeof(byte?) ||
                    source.PropertyType == typeof(double) ||
                    source.PropertyType == typeof(double?) ||
                    source.PropertyType == typeof(decimal) ||
                    source.PropertyType == typeof(decimal?) ||
                    source.PropertyType == typeof(DateTime) ||
                    source.PropertyType == typeof(DateTime?) ||
                    source.PropertyType == typeof(Array) ||
                    source.PropertyType == typeof(bool) ||
                    source.PropertyType == typeof(bool?))
                {
                    PropertyInfo destination = destinations.Where(d => d.Name.Equals(source.Name)).FirstOrDefault();
                    if (destination != null) destination.SetValue(To, source.GetValue(From));
                }
        }

        public static void Split(ICollection<T> New, ICollection<T> Current, out List<T> Insert, out List<T> Update, out List<T> Delete)
        {
            Insert = new List<T>();
            Update = new List<T>();
            Delete = new List<T>();
            foreach (T C in Current)
            {
                T tmp = New.Where(N => N.Equals(C)).FirstOrDefault();
                if (tmp == null)
                    Delete.Add(C);
                else
                {
                    Update.Add(tmp);
                }
            }
            foreach (T N in New)
            {
                if (!Current.Any(C => C.Equals(N)))
                    Insert.Add(N);
            }
        }
    }
}
