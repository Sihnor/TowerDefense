using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private GameObject FollowTarget;
        [SerializeField] private int MaxZoom = 10;
        private int CurrentZoom = 0;


        private void Awake()
        {
            PlayerInput input = GetComponent<PlayerInput>();
            
            input.actions["Zoom"].performed += Zoom;
        }
        
        private void Zoom(InputAction.CallbackContext context)
        {
            float zoom = context.ReadValue<float>();
            zoom = Mathf.Clamp(zoom, -1, 1);
            
            if (this.CurrentZoom + zoom < -this.MaxZoom || this.CurrentZoom + zoom > this.MaxZoom)
                return;
            
            this.CurrentZoom += (int)zoom;
            
            this.FollowTarget.transform.position += this.FollowTarget.transform.forward * zoom;
        }
    }
}