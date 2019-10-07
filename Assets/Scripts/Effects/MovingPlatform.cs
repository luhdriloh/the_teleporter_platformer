﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float _maxSpeed = 1f;
    public float _maxXDistanceMove = 2;

    private Collider2D _collider;
    private PlayerPlatformerController _player;
    private Vector3 _newPosition;
    private float _minXDistance;
    private bool _movingLeft = true;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _newPosition = transform.position;
        _minXDistance = transform.position.x;
        _maxXDistanceMove += _minXDistance;

        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerPlatformerController>();
        }
    }

    private void Update()
    {
        _newPosition = transform.position;

        if (transform.position.x < _minXDistance)
        {
            _movingLeft = !_movingLeft;
            _maxSpeed *= -1;
            Vector3 newPosition = transform.position;
            newPosition.x = _minXDistance + .001f;
        }
        else if (transform.position.x > _maxXDistanceMove)
        {
            _movingLeft = !_movingLeft;
            _maxSpeed *= -1;
            Vector3 newPosition = transform.position;
            newPosition.x = _maxXDistanceMove - .001f;
        }

        CheckCollisions();
        _newPosition.x = _newPosition.x + _maxSpeed * Time.deltaTime;
    }


    private void CheckCollisions()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player", "TeleportationOrb"),
            useTriggers = true,
            useLayerMask = true
        };

        Collider2D[] result = new Collider2D[4];
        int collidersHit = _collider.OverlapCollider(filter, result);

        for (int i = 0; i < collidersHit; i++)
        {
            result[i].GetComponent<AddOutsideDistance>()._addOutsideDistance.x = _maxSpeed * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        transform.position = _newPosition;
    }
}