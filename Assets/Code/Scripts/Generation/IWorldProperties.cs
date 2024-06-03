using UnityEngine;

namespace Code.Scripts.Generation
{
    public interface IWorldProperties
    {
        float Height { get; }

        Vector2Int Coordinates { get; }

        /// <summary>
        /// Set the height of the object inside the quadrant
        /// </summary>
        /// <param name="factor"></param>
        public void SetHeight(float factor);

        /// <summary>
        /// Add the height of the object inside the quadrant
        /// </summary>
        /// <param name="factor"></param>
        public void AddHeight(float factor);

        /// <summary>
        /// Multiply the height of the object inside the quadrant
        /// </summary>
        /// <param name="factor"></param>
        public void MultiplyHeight(float factor);

        /// <summary>
        /// Get the height of the object inside the quadrant
        /// </summary>
        /// <returns></returns>
        public float GetHeight();

        /// <summary>
        /// Set the position of the object inside the quadrant
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2Int position);

        /// <summary>
        /// Get the position of the object inside the quadrant
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetCoordinates();
    }
}