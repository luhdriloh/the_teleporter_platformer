using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerPlatformerController : PhysicsObject
{
    public GameObject _teleportationOrbPrototype;
    public ParticleSystem _onLandParticleSystem;
    public Transform _throwBallPosition;
    public static int _orbsAllowedInPlay = 2;
    public float _maxSpeed = 7f;
    public float _jumpTakeOffSpeed = 7f;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _onGround;

    private static List<TeleportationOrb> _orbs = new List<TeleportationOrb>();
    private TeleportationOrb _currentOrbBeingThrown;
    private bool _teleported;
    private Vector2 _teleportSpeed;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _onGround = true;
        //_teleported = false;
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

        if (_grounded && !_onGround)
        {
            _onLandParticleSystem.Emit(30);
        }

        _onGround = _grounded;

        _animator.SetBool("Grounded", _grounded);
        _animator.SetFloat("VelocityX", Mathf.Abs(_velocity.x) / _maxSpeed);
        _animator.SetFloat("VelocityY", _velocity.y);

        GetPlayerInput();

        _targetVelocity = move * _maxSpeed;
    }

    private void GetPlayerInput()
    {
        // set up teleportation orb
        if (Input.GetMouseButtonDown(0) && _orbs.Count < 2)
        {
            TeleportationOrb orb = Instantiate(_teleportationOrbPrototype, _throwBallPosition.position, Quaternion.identity).GetComponent<TeleportationOrb>();
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

        // set up animator values
        if (_teleported)
        {
            _velocity = _teleportSpeed;
            _teleported = false;
            return;
        }
    }

    public void Teleport(Vector3 positionToTeleport, Vector3 velocity)
    {
        positionToTeleport.z = transform.position.z;
        _teleportSpeed = velocity;
        _teleported = true;

        //Vector2 castVector = positionToTeleport - transform.position;
        //transform.position = positionToTeleport;

        //RaycastHit2D[] hitBuffer = new RaycastHit2D[2];
        //int count = 0;
        //int i = 0;

        //do
        //{
        //    i++;
        //    count = _rigidbody.Cast(Vector2.up, _contactFilter, hitBuffer, .01f);
        //    Debug.Log(count);
        //    // get the closest point
        //    if (count > 0)
        //    {
        //        _point = hitBuffer[0].collider.ClosestPoint(positionToTeleport);
        //        Vector2 newPositionDirection = ((Vector2)positionToTeleport - _point).normalized * .5f;
        //        positionToTeleport += (Vector3)newPositionDirection;
        //        castVector = positionToTeleport - transform.position;
        //    }
        //}
        //while (count > 0 && i < 6);

        transform.position = positionToTeleport + Vector3.up * .6f;
    }
}

