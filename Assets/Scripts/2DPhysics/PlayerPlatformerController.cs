using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerPlatformerController : PhysicsObject
{
    public GameObject _teleportationOrbPrototype;
    public ParticleSystem _onLandParticleSystem;
    public ParticleSystem _onRunParticleSystem;
    public Transform _throwBallPosition;
    public static int _orbsAllowedInPlay = 2;
    public float _maxSpeed = 7f;
    public float _jumpTakeOffSpeed = 7f;

    public AudioClip _hit;
    public AudioClip _jump;
    public AudioClip _teleport;
    public AudioClip _throw;

    private AudioSource _audiosource;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private bool _onGround;
    private bool _dead;
    private bool _running;

    private List<TeleportationOrb> _orbs;
    private TeleportationOrb _currentOrbBeingThrown;
    private bool _teleported;
    private Vector2 _teleportSpeed;

    private void Awake()
    {
        _orbs = new List<TeleportationOrb>();
        _audiosource = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _onGround = true;
        _running = false;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        if (!_dead)
        {
            move.x = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(move.x) > Mathf.Epsilon && !_running && _grounded)
            {
                _running = true;
                _onRunParticleSystem.Play();
            }
            else if ((Mathf.Abs(move.x) < Mathf.Epsilon && _running) || !_grounded)
            {
                _running = false;
                _onRunParticleSystem.Stop();
            }
        }

        if (Input.GetButtonDown("Jump") && _grounded && !_dead)
        {
            _audiosource.PlayOneShot(_jump);
            _velocity.y = _jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump") && !_dead)
        {
            if (_velocity.y > 0)
            {
                _velocity.y *= .5f;
            }
        }

        bool flipped = Mathf.Abs(transform.rotation.y) > Mathf.Epsilon;
        bool flipSprite = (flipped ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite)
        {
            Vector3 eulers = transform.eulerAngles;
            eulers.y = flipped ? 0f : -180f;
            transform.eulerAngles = eulers;
        }

        if (_grounded && !_onGround)
        {
            _onLandParticleSystem.Emit(30);
        }

        _onGround = _grounded;

        if (!_dead)
        {
            _animator.SetBool("Grounded", _grounded);
            _animator.SetFloat("VelocityX", Mathf.Abs(_velocity.x) / _maxSpeed);
            _animator.SetFloat("VelocityY", _velocity.y);
            GetPlayerInput();
        }

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
            _audiosource.PlayOneShot(_throw);
            _currentOrbBeingThrown.SetVelocity(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            _currentOrbBeingThrown = null;
        }
        else if (Input.GetMouseButtonDown(1) && _orbs.Count > 0)
        {
            TeleportationOrb orb = _orbs[0];
            GameObject orbToTransportTo = orb.TransportPlayerToOrb();

            if (orbToTransportTo != null)
            {
                _orbs.RemoveAt(0);
                Destroy(orbToTransportTo);
            }
        }

        // set up animator values
        if (_teleported)
        {
            _velocity = _teleportSpeed;
            _teleported = false;
            return;
        }
    }

    public void DestroyOrb(TeleportationOrb orbToDestroy)
    {
        if (_orbs.Count > 0)
        {
            TeleportationOrb orbToRemove = null;
            foreach (TeleportationOrb currentOrb in _orbs)
            {
                if (currentOrb == orbToDestroy)
                {
                    orbToRemove = currentOrb;
                    break;
                }
            }

            if (orbToRemove != null)
            {
                _orbs.Remove(orbToRemove);
                Destroy(orbToRemove.gameObject);
            }
        }
    }

    public void Die()
    {
        _dead = true;
        _audiosource.PlayOneShot(_hit);
        GetComponent<Collider2D>().enabled = false;
        GameObject.FindWithTag("VirtualCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = null;
        _animator.SetTrigger("Dead");
        _velocity.y = 40f;
        _rigidbody.rotation = Random.Range(30, 180);

        StartCoroutine("Restart");
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

        transform.position = positionToTeleport + Vector3.up * 1f;
        _audiosource.PlayOneShot(_teleport);
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);

        GameObject.FindWithTag("Transition").GetComponent<LevelTransitionEffect>().EndLevel();

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

