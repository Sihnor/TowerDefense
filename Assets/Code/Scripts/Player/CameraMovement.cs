using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private GameObject FollowTarget;
        
        private void Awake()
        {
            PlayerInput input = GetComponent<PlayerInput>();
            
            input.actions["Zoom"].performed += Zoom;
        }
        
        private void Zoom(InputAction.CallbackContext context)
        {
            float zoom = context.ReadValue<float>();
            zoom = Mathf.Clamp(zoom, -1, 1);
            
            this.FollowTarget.transform.position += FollowTarget.transform.forward * zoom;
        }
    }
}