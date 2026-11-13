using System;

namespace TGFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DesignerConfigAttribute : Attribute
    {
        public string Section { get; }

        public DesignerConfigAttribute(string section)
        {
            Section = string.IsNullOrWhiteSpace(section) ? "General" : section;
        }
    }
}
