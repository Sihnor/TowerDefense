using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Generation
{
    public interface IStackable
    {
        public IStackable MeinPlace { get; set; }

        public void SetPlaceable(IStackable stackable)
        {
            if (this.MeinPlace == null)
            {
                this.MeinPlace = stackable;
            }
            else
            {
                if (this.MeinPlace.MeinPlace == null)
                {
                    this.MeinPlace.MeinPlace = stackable;
                }
                else
                {
                    this.MeinPlace.MeinPlace.MeinPlace = stackable;
                }
            }
        }
        
        public bool CanBePlaced()
        {
            return this.MeinPlace == null || this.MeinPlace.MeinPlace == null || this.MeinPlace.MeinPlace.MeinPlace == null;
        }

        public Vector3 GetPosition();
        
        public IStackable GetLastPlace()
        {
            if (this.MeinPlace == null)
            {
                return this;
            }
            if (this.MeinPlace.MeinPlace == null)
            {
                return this.MeinPlace;
            }
            if (this.MeinPlace.MeinPlace.MeinPlace == null)
            {
                return this.MeinPlace.MeinPlace;
            }
            
            return null;
        }
    }
}