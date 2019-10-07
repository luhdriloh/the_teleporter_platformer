using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerPlatformerController : PhysicsObject
{
    public static int _orbsAllowedInPlay = 2;
    public float _maxSpeed = 7f;
    public float _jumpTakeOffSpeed = 7f;
    public GameObject _teleportationOrbPrototype;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private TeleportationOrb _currentOrbBeingThrown;
    private bool _teleported;
    private Vector2 _teleportSpeed;
    private static List<TeleportationOrb> _orbs = new List<TeleportationOrb>();



    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _teleported = false;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && _grounded)
        {
            _velocity.y = _jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (_velocity.y > 0)
            {
                _velocity.y *= .5f;
            }
        }

        bool flipSprite = (_spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        // set up teleportation orb
        if (Input.GetMouseButtonDown(0) && _orbs.Count < 2)
        {
            TeleportationOrb orb = Instantiate(_teleportationOrbPrototype, transform.position + Vector3.back, Quaternion.identity).GetComponent<TeleportationOrb>();
            orb.transform.SetParent(transform);
            _currentOrbBeingThrown = orb;
            _orbs.Add(orb);
        }
        else if (Input.GetMouseButton(0) && _currentOrbBeingThrown != null)
        {
            Vector3 aimpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             _currentOrbBeingThrown.CreateTrajectoryGhost(aimpoint);
        }
        else if (Input.GetMouseButtonUp(0) && _currentOrbBeingThrown != null)
        {
            _currentOrbBeingThrown.SetVelocity(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _currentOrbBeingThrown = null;
        }
        else if (Input.GetMouseButtonDown(1) && _orbs.Count > 0)
        {
            TeleportationOrb orb = _orbs[0];
            _orbs.RemoveAt(0);

            orb.TransportPlayerToOrb();
        }

        // get the x and the y speed
        // grounded or not
        //_animator.SetFloat("velocityX", Mathf.Abs(_velocity.x) / _maxSpeed);

        // set up animator values
        if (_teleported)
        {
            _velocity = _teleportSpeed;
            _teleported = false;
            return;
        }

        _targetVelocity = move * _maxSpeed;
    }

    public void Teleport(Vector3 positionToTeleport, Vector3 velocity)
    {
        positionToTeleport.z = transform.position.z;
        _teleportSpeed = velocity;
        transform.position = positionToTeleport;
        _teleported = true;
    }
}

