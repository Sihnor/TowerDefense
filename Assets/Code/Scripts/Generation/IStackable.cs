using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public interface IStackable
    {
        public IStackable MeinPlace { get; set; }

        /// <summary>
        /// Add a IStackable object to last GameObject of the stack.
        /// </summary>
        /// <param name="stackable"></param>
        public void SetPlaceable(IStackable stackable)
        {
            IStackable currentPlace = this.MeinPlace;
            
            if (currentPlace == null) this.MeinPlace = stackable;
            else currentPlace.SetPlaceable(stackable);
        }

        /// <summary>
        /// Check if the IStackable object can be placed on the stack.
        /// </summary>
        /// <param name="maxStack">How many Objects can stack.</param>
        /// <returns></returns>
        public bool CanBePlaced(int maxStack = 3)
        {
            IStackable checkPlace = this.MeinPlace;
            for (int i = 0; i < maxStack; i++)
            {
                if (checkPlace == null) return true;
                checkPlace = checkPlace.MeinPlace;
            }

            return false;
        }

        /// <summary>
        /// Get the last GameObject in the stack of IStackable objects for the position.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPositionForPlacement();
        
        /// <summary>
        /// Get the last GameObject in the stack of IStackable objects.
        /// </summary>
        /// <returns></returns>
        public IStackable GetLastPlace()
        {
            IStackable currentPlace = this.MeinPlace;
            
            return currentPlace == null ? this : currentPlace.GetLastPlace();
        }
    }
}