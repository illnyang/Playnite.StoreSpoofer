using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace StoreSpoofer.Extensions
{
    internal static class EnumExtensions
    {
        public static string EnumGetDescription<T>(this T e) where T : IConvertible
        {
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val != e.ToInt32(CultureInfo.InvariantCulture))
                    continue;

                var memName = type.GetEnumName(val);

                if (memName == null)
                    continue;
                
                var memInfo = type.GetMember(memName);

                if (memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                {
                    return descriptionAttribute.Description;
                }
            }

            return null;
        }

        public static bool EnumFromDescription<T>(this string description, out T result) where T : IConvertible
        {
            var values = Enum.GetValues(typeof(T));

            foreach (var val in values)
            {
                var memName = typeof(T).GetEnumName(val);

                if (memName == null)
                    continue;

                var memInfo = typeof(T).GetMember(memName);

                if (!(memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute))
                    continue;

                if (string.Equals(descriptionAttribute.Description, description, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = (T)val;
                    return true;
                }
            }
            
            result = default(T);
            return false;
        }
    }
}
    
