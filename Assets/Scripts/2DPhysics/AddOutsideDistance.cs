using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class AddOutsideDistance : MonoBehaviour
{
    public Vector2 _addOutsideDistance = Vector2.zero;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.position = _rigidbody.position + _addOutsideDistance;
        _addOutsideDistance = Vector2.zero;
    }
}
