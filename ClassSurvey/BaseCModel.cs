using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ClassSurvey
{
    public abstract class Base
    {
        public Base() { }
        public Base(object obj)
        {
            Common<object>.Copy(obj, this);
        }
        public abstract bool Equals(Base other);
        public void CopyTo(Base b)
        {
            List<PropertyInfo> sources = this.GetType().GetProperties().ToList();
            List<PropertyInfo> destinations = b.GetType().GetProperties().ToList();
            foreach (PropertyInfo source in sources)
                if ((source.PropertyType.IsGenericType && source.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)) ||
                    source.PropertyType.IsSubclassOf(typeof(Base)) ||
                    source.Name.Equals("Cx")
                    || source.Name.Equals("Id"))
                    continue;
                else
                {
                    PropertyInfo destination = destinations.Where(d => d.Name.Equals(source.Name)).FirstOrDefault();
                    if (destination != null) destination.SetValue(b, source.GetValue(this));
                }
        }
    }
}
