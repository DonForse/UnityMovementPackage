using System;

namespace MovementPackage.Runtime.Scripts.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TabMenu : Attribute
    {
        public string TabName { get; private set; }

        public TabMenu(string tabName)
        {
            TabName = tabName;
        }
    }
}