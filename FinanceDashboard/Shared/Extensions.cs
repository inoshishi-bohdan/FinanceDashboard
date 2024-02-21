using System.Reflection;

namespace FinanceDashboard.Shared
{
    public static class Extensions
    {
        public static string? GetDisplayAs(this Enum value)
        {
            var members = value.GetType().GetMember(value.ToString());
            if (members.Length == 0) return null;
            var attribute = members.First().GetCustomAttribute<DisplayAsAttribute>();
            if (attribute == null) return null;
            return attribute.DisplayAs;
        }

        public static string GetDisplayAsOrName(this Enum value)
        {
            var members = value.GetType().GetMember(value.ToString());
            if (members.Length == 0) return value.ToString();
            var attribute = members.First().GetCustomAttribute<DisplayAsAttribute>();
            if (attribute == null) return value.ToString();
            return attribute.DisplayAs;
        }
    }
}
