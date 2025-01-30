using UnityEngine;

namespace Lightbug.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class BooleanButtonAttribute : PropertyAttribute
    {
        public string Label = null;
        public string FalseLabel;
        public string TrueLabel;
        public bool FalseLabelFirst;

        public BooleanButtonAttribute(string label, string falseLabel, string trueLabel, bool falseLabelFirst)
        {
            Label = label;
            FalseLabelFirst = falseLabelFirst;
            FalseLabel = falseLabel;
            TrueLabel = trueLabel;
        }
    }
}

