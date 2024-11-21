using System.ComponentModel;

namespace Domain.Extensions
{
    public static class EnumExtensions
    {
        // Extension method to get the description of an enum value
        public static string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? enumValue.ToString() : attribute.Description;
        }
    }
}
