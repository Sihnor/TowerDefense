using Code.Scripts.PropertyAttributes;
using UnityEngine;

namespace Code.Scripts.ScriptableScripts
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObject/Settings/LevelSettings", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        [SerializeField, RangeStep(5, 23, 2)] private int QuadrantSize;
        [SerializeField] private int IntermediateTargets;
        
        public int GetQuadrantSize()
        {
            return this.QuadrantSize;
        }
        
        public void SetQuadrantSize(int size)
        {
            this.QuadrantSize = size;
        }
        
        public int GetIntermediateTargets()
        {
            return this.IntermediateTargets;
        }

        public void SetIntermediateTargets(int targets)
        {
            this.IntermediateTargets = targets;
        }

    }
}