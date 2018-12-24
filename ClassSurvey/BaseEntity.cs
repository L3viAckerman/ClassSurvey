using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;

namespace ClassSurvey
{
    public interface ICommand { }
    public class BaseEntity
    {
        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        public void AddError(string Key, string Value)
        {
            if (Errors == null) Errors = new Dictionary<string, string>();
            if (Errors.ContainsKey(Key))
                Errors[Key] += Value;
            else
                Errors.Add(Key, Value);
        }

        public string ConvertToUnsign(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            string[] signs = new string[] {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };
            for (int i = 1; i < signs.Length; i++)
            {
                for (int j = 0; j < signs[i].Length; j++)
                {
                    str = str.Replace(signs[i][j], signs[0][i - 1]);
                }
            }

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c.Equals(' ') || ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z') || c.Equals(',') || c.Equals('.') || ('0' <= c && c <= '9'))
                {
                    continue;
                }
                else
                {
                    str = str.Replace(c, ' ');
                }
            }
            return str;
        }

        public BaseEntity() { }
        public BaseEntity(object obj)
        {
            Common<object>.Copy(obj, this);
        }

    }


    public interface IFilterEntity
    {
        int Take { get; set; }
        int Skip { get; set; }
        string OrderBy { get; set; }
        OrderType? OrderType { get; set; }
    }

    public class FilterEntity : IFilterEntity
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public string OrderBy { get; set; }
        public OrderType? OrderType { get; set; }

        public IQueryable<T> Order<T>(IQueryable<T> source)
        {
            if (!string.IsNullOrEmpty(OrderBy))
            {
                OrderType = OrderType == null ? ClassSurvey.OrderType.ASC : OrderType;

                string command = this.OrderType == ClassSurvey.OrderType.ASC ? "OrderBy" : "OrderByDescending";
                var type = typeof(T);
                var property = type.GetProperty(OrderBy);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                    source.Expression, Expression.Quote(orderByExpression));
                return source.Provider.CreateQuery<T>(resultExpression);
            }
            else
                return source;
        }

        public IQueryable<T> SkipAndTake<T>(IQueryable<T> source)
        {
            if (Skip == 0 && Take == 0)
            {
                Skip = 0; Take = 10;
            }
            return source.Skip(Skip * Take).Take(Take);
        }
    }

    public enum OrderType
    {
        NONE = 0,
        DESC = 1,
        ASC = 2
    }
}
