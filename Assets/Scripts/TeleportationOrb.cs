using System.Collections;
using System.Collections.Generic;
using SpriteTrailRenderer;
using UnityEngine;


public class TeleportationOrb : MonoBehaviour
{
    public static int _orbsAllowedInPlay = 3;
    public float _maxVelocity = 15f;
    public float _distanceToCalculateSpeed = 7f;
    public float _maxGhostDistance = 3f;
    public float _minGroundNormalY = .65f;

    private Rigidbody2D _rigidbody;
    private LineRenderer _lineRenderer;
    private PlayerPlatformerController _player;
    private SpriteTrailRenderer.SpriteTrailRenderer _trail;
    private ContactFilter2D _contactFilter;

    private void OnEnable()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerPlatformerController>();
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _trail = GetComponent<SpriteTrailRenderer.SpriteTrailRenderer>();
        _rigidbody.isKinematic = true;
        _trail.enabled = false;
        _orbsAllowedInPlay--;

        _contactFilter.useTriggers = false;

        // this goes to your physics settings and get the layer matrix associated
        //  with your gameobjects layer
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        _contactFilter.useLayerMask = true;
    }

    private Vector2 GetThrownVelocityFromAimPoint(Vector2 aimPoint)
    {
        float maxVelocityPercent = Mathf.Min((aimPoint - (Vector2)transform.position).magnitude / _distanceToCalculateSpeed, 1);
        return (aimPoint - (Vector2)transform.position).normalized * _maxVelocity * maxVelocityPercent;
    }

    public void SetVelocity(Vector2 aimPoint)
    {
        RaycastHit2D[] collidersHit = new RaycastHit2D[1];

        int hits = _rigidbody.Cast(GetThrownVelocityFromAimPoint(aimPoint), _contactFilter, collidersHit, .0001f);

        if (hits > 0)
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerPlatformerController>().DestroyOrb(this);
        }

        _trail.enabled = true;

        transform.parent = null;
        _rigidbody.isKinematic = false;

        _lineRenderer.enabled = false;
        _rigidbody.velocity = GetThrownVelocityFromAimPoint(aimPoint);
    }

    public void CreateTrajectoryGhost(Vector3 aimPoint)
    {
        Vector3 position = transform.position;
        position.z = -2;
        transform.position = position;

        RaycastHit2D[] collidersHit = new RaycastHit2D[1];

        int wallImpacts = _rigidbody.Cast(GetThrownVelocityFromAimPoint(aimPoint), _contactFilter, collidersHit, .0001f);

        if (wallImpacts > 0)
        {
            return;
        }

        // have a maximum distance allowed for the trajectory simulation
        // then use a trail renderer to show the outline

        // we the x component as well as the y component and t for time
        Vector2 initialPosition = transform.position;
        Vector2 simulationVelocity = GetThrownVelocityFromAimPoint(aimPoint);

        // go in partitions of .1s until a distance of 3 is reached
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Platform", "Button"),
            useTriggers = true,
            useLayerMask = true
        };
        
        Stack<Vector3> simulationPoints = new Stack<Vector3>();
        float totalDistance = 0;
        float timeBeingSimulated = 0f;

        simulationPoints.Push(initialPosition);
        while (totalDistance < _maxGhostDistance)
        {
            timeBeingSimulated += .1f;
            float newXPosition = timeBeingSimulated * simulationVelocity.x + initialPosition.x;

            float newYDeltaDueToGravity = (Physics2D.gravity.y * _rigidbody.gravityScale * Mathf.Pow(timeBeingSimulated, 2)) * .5f;
            float newYPosition = timeBeingSimulated * simulationVelocity.y + newYDeltaDueToGravity + initialPosition.y;

            Vector3 newPosition = new Vector3(newXPosition, newYPosition, -1f);
            float distanceToAdd = ((Vector2)newPosition - (Vector2)simulationPoints.Peek()).magnitude;
            totalDistance += distanceToAdd;

            // see if we hit any colliders
            Vector2 directionOfRay = newPosition - simulationPoints.Peek();
            int hits = Physics2D.Raycast(simulationPoints.Peek(), directionOfRay, filter, collidersHit, distanceToAdd);
             
            if (hits > 0)
            {
                newPosition = collidersHit[0].point;
                newPosition.z = -1;
                simulationPoints.Push(newPosition);
                break;
            }

            simulationPoints.Push(newPosition);
        }

        _lineRenderer.positionCount = simulationPoints.Count;
        _lineRenderer.SetPositions(simulationPoints.ToArray());
    }

    public GameObject TransportPlayerToOrb()
    {
        if (IsGrounded())
        {
            Vector3 newPosition = transform.position + Vector3.up * .17f;
            _player.Teleport(newPosition, _rigidbody.velocity);
            _orbsAllowedInPlay++;
            return gameObject;
        }

        return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _rigidbody.velocity = Vector2.zero;
    }

    private bool IsGrounded()
    {
        bool grounded = false;

        // cast this objects rigid body using the current velocity
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = _rigidbody.Cast(Vector2.up, _contactFilter, hits, Physics2D.gravity.y * Time.deltaTime * _rigidbody.gravityScale);

        if (hitCount > 0)
        {

            if (hits[0].normal.y > _minGroundNormalY)
            {
                grounded = true;
            }
        }

        return grounded;
    }
}






































