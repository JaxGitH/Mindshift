using UnityEngine;

namespace Lightbug.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BreakVector3Attribute : PropertyAttribute 
    {
        public string XLabel;
        public string YLabel;
        public string ZLabel;

        public BreakVector3Attribute(string xLabel, string yLabel, string zLabel)
        {
            XLabel = xLabel;
            YLabel = yLabel;
            ZLabel = zLabel;
        }
    }
}
