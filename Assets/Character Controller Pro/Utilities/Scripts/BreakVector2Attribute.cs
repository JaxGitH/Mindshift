using UnityEngine;

namespace Lightbug.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BreakVector2Attribute : PropertyAttribute
    {
        public string XLabel;
        public string YLabel;

        public BreakVector2Attribute(string xLabel, string yLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
        }
    }
}
