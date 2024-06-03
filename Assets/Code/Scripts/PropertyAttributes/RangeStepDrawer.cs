using UnityEditor;
using UnityEngine;

namespace Code.Scripts.PropertyAttributes
{
    [CustomPropertyDrawer (typeof(RangeStep))]
    public class RangeStepDrawer : PropertyDrawer
    {
        private int Value;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeStep rangeAttribute = (RangeStep)base.attribute;
 
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                int value = EditorGUI.IntSlider(position, label, property.intValue, rangeAttribute.min, rangeAttribute.max);
                
                property.intValue = IsNumberInRow(rangeAttribute.min, rangeAttribute.step, value) ? value : GetNextBigNumberInRow(rangeAttribute.min, rangeAttribute.step, value);
                this.Value = property.intValue;
            }
            else
            {
                EditorGUI.LabelField (position, label.text, "Use Range with float or int.");
            }
        }

        private static bool IsNumberInRow(int start, int step, int checkNumber)
        {
            for (int i = start; i <= checkNumber; i += step)
            {
                if (i == checkNumber)
                {
                    return true;
                }
            }

            return false;
        }
        
        private static int GetNextBigNumberInRow(int start, int step, int checkNumber)
        {
            for (int i = start; i <= checkNumber + step; i += step)
            {
                if (i > checkNumber)
                {
                    return i;
                }
            }

            return start;
        }
    }
}