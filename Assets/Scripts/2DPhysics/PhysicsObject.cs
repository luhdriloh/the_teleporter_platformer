using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public bool _gravityEnabled = true;
    public float _gravityModifier = 1f;
    public float _minGroundNormalY = .65f;

    protected Vector2 _targetVelocity;
    protected bool _grounded;
    protected Vector2 _groundNormal;

    protected Vector2 _velocity = Vector2.zero;
    protected Rigidbody2D _rigidbody;
    protected ContactFilter2D _contactFilter;
    protected RaycastHit2D[] _hitbuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected float _minMoveDistance = 0.001f;
    protected const float _shellRadius = 0.01f;


    protected virtual void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _contactFilter.useTriggers = false;

        // this goes to your physics settings and get the layer matrix associated
        //  with your gameobjects layer
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter.useLayerMask = true;
        _groundNormal = Vector2.up;
    }

    private void Update()
    {
        _targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    private void FixedUpdate()
    {
        if (!_gravityEnabled)
        {
            return;
        }

        _velocity += _gravityModifier * Physics2D.gravity * Time.deltaTime;
        _velocity.x = _targetVelocity.x;
        _grounded = false;

        Vector2 deltaPosition = _velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    private void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;
        if (distance > _minMoveDistance)
        {
            int count = _rigidbody.Cast(move, _contactFilter, _hitbuffer, distance + _shellRadius);
            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(_hitbuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            { 
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    _grounded = true;
                    if (yMovement)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        _rigidbody.position = _rigidbody.position + move.normalized * distance;
    }

    protected virtual void ComputeVelocity()
    { 
    
    }
}























