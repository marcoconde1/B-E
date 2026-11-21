using UnityEngine;

namespace Game.InGame.Player
{
    // Movimiento estilo plataforma con gravedad:
    // A / D (o flechas) para moverse, W o Espacio para saltar.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _jumpForce = 7f;

        private Rigidbody2D _rigidbody;
        private float _horizontal;
        private bool _jumpRequested;
        private bool _isGrounded;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.freezeRotation = true; // para que no gire al chocar
        }

        private void Update()
        {
            _horizontal = Input.GetAxisRaw("Horizontal"); // A / D

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _jumpRequested = true;
            }
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rigidbody.linearVelocity;
            velocity.x = _horizontal * _speed; // la Y la controla la gravedad

            if (_jumpRequested && _isGrounded)
            {
                velocity.y = _jumpForce;
                _isGrounded = false;
            }
            _jumpRequested = false;

            _rigidbody.linearVelocity = velocity;
        }

        // Está en el suelo si toca algo por debajo
        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    _isGrounded = true;
                    return;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            _isGrounded = false;
        }
    }
}
