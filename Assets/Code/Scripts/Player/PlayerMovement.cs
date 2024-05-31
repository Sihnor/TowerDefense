using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private Vector2 Movement;
        private Rigidbody Rigidbody;
        
        [SerializeField] private float Speed = 5f;
        [SerializeField] private float MaxSpeed = 25f;
        private bool BIsSpeeding;
        
        private void Awake()
        {
            this.Rigidbody = GetComponent<Rigidbody>();
            
            PlayerInput input = GetComponent<PlayerInput>();
            
            input.actions["Move"].performed += Move;
            input.actions["MoveFaster"].started += ctx => this.BIsSpeeding = true;
            input.actions["MoveFaster"].canceled += ctx => this.BIsSpeeding = false;
        }

        private void FixedUpdate()
        {
            this.Rigidbody.velocity += new Vector3(this.Movement.x, 0, this.Movement.y) * (Time.deltaTime * this.Speed * (this.BIsSpeeding ? 2 : 1));
            LimitVelocity();
        }

        private void Move(InputAction.CallbackContext context)
        {
            this.Movement = context.ReadValue<Vector2>();
        }
        
        private void LimitVelocity()
        {
            Vector3 velocity = this.Rigidbody.velocity;
            velocity.x = Mathf.Clamp(velocity.x, -this.MaxSpeed, this.MaxSpeed);
            velocity.z = Mathf.Clamp(velocity.z, -this.MaxSpeed, this.MaxSpeed);
            this.Rigidbody.velocity = velocity;
        }
    }
}