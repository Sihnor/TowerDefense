using Code.Scripts.Generation;
using UnityEngine;

namespace Code.Scripts
{
    public class Tower : MonoBehaviour, IStackable
    {
        public IStackable MeinPlace { get; set; }

        public Vector3 GetPositionForPlacement()
        {
            Vector3 offset = new Vector3(0, 1, 0);

            offset += this.transform.position;

            return offset;
        }
    }
}