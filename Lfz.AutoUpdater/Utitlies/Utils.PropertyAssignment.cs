using System;
using System.Collections.Generic;

namespace Lfz.AutoUpdater.Utitlies
{
    internal static partial class Utils
    {
         

        #region 根据属性名称-值列表设置对象值

        /// <summary> 
        /// 根据属性名称-值列表设置对象值
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="obj"></param> 
        /// <param name="propertyNameValue"></param>
        /// <returns></returns>
        public static void SetValue<TElement>(TElement obj, IEnumerable<KeyValuePair<string, string>> propertyNameValue)
            where TElement : class
        {
            if (obj == null) return;
            var elementType = typeof(TElement);
            foreach (var item in propertyNameValue)
            {
                try
                {
                    if (item.Value == null) continue;
                    var property = elementType.GetProperty(item.Key);
                    if (property == null) continue;
                    var propertyType = property.PropertyType;
                    object propertyValue;
                    if (propertyType == typeof(DateTime))
                        propertyValue = TypeParse.StrToDateTime(item.Value, DateTime.Now);
                    else if (propertyType == typeof(DateTime?))
                    {
                        DateTime temp;
                        if (!string.IsNullOrEmpty(item.Value) && DateTime.TryParse(item.Value, out temp))
                        {
                            propertyValue = (DateTime?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(Guid))
                        propertyValue = TypeParse.StrToGuid(item.Value);
                    else if (propertyType == typeof(Guid?))
                    {
                        Guid temp;
                        if (!string.IsNullOrEmpty(item.Value) && Guid.TryParse(item.Value, out temp))
                        {
                            propertyValue = (Guid?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(Double?))
                    {
                        Double temp;
                        if (!string.IsNullOrEmpty(item.Value) && Double.TryParse(item.Value, out temp))
                        {
                            propertyValue = (Double?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(int?))
                    {
                        int temp;
                        if (!string.IsNullOrEmpty(item.Value) && int.TryParse(item.Value, out temp))
                        {
                            propertyValue = (int?)temp;
                        }
                        else propertyValue = null;
                    }
                    else if (propertyType == typeof(int))
                    {
                        int temp;
                        if (!string.IsNullOrEmpty(item.Value) && int.TryParse(item.Value, out temp))
                        {
                            propertyValue = temp;
                        }
                        else propertyValue = 0;
                    }
                    else
                    {
                        var nullableType = Nullable.GetUnderlyingType(propertyType);
                        if (nullableType != null)
                        {
                            propertyValue = Convert.ChangeType(item.Value, nullableType);
                        }
                        else
                        {
                            propertyValue = Convert.ChangeType(item.Value, propertyType);
                        }
                    }
                    property.SetValue(obj, propertyValue, new object[] { });
                }
                catch (Exception e)
                {

                }
            }
        }


        #endregion
    }

}
