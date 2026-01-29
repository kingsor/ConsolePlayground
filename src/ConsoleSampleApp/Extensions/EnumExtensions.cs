using System.ComponentModel;
using System.Reflection;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace ConsoleSampleApp.Extensions
{
    /// <summary>
    /// Provides a set of static methods for querying objects that represent an enumeration.
    /// </summary>
    public static class EnumExtensions
    {
        public static T GetEnumFromString<T>(string value)
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            else
            {
                string[] enumNames = Enum.GetNames(typeof(T));
                foreach (string enumName in enumNames)
                {
                    object e = Enum.Parse(typeof(T), enumName);
                    if (value == GetDescription((Enum)e))
                    {
                        return (T)e;
                    }
                }
            }
            throw new ArgumentException("The value '" + value
                + "' does not match a valid enum name or description.");
        }

        public static string GetDescription(this Enum value)
        {
            string result = value.ToString();
            
            FieldInfo field = value.GetType().GetField(value.ToString());

            if(field is not null)
            {
                DescriptionAttribute? attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

                if(attribute is not null)
                {
                    result = attribute.Description;
                }
            }

            return result;
        }

        // the following two methods were found here:
        // https://gist.github.com/maftieu/6cd02e9804c98e8fe87b

        /// <summary>
        /// Fetch the description of the <paramref name="enumType"/> enumeration value.
        /// </summary>
        /// <param name="enumType">Enumeration value for which to return the description.</param>
        /// <returns>The description of the provided enumeration value.</returns>
        public static string Description(this Enum enumType)
        {
            var memInfo = enumType.GetType().GetMember(enumType.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return enumType.ToString();
        }

        /// <summary>
        /// Indicates wether the <paramref name="enumType"/> enum value has a description.
        /// </summary>
        /// <param name="enumType">Enumeration value for which to indicate wether is has a description.</param>
        /// <returns>true if a description is defined on the enumeration value ; otherwise false.</returns>
        public static bool HasDescription(this Enum enumType)
        {
            return !string.IsNullOrWhiteSpace(enumType.Description());
        }



        // found here: https://stackoverflow.com/a/4367868
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }
    }
}
