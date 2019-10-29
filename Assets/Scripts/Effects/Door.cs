using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    public PuzzleColor _doorColor;
    public Vector2 _directionToMove;
    public Transform _moveDirectionArrow;
    public float _distanceToTravel;
    public float _speed;
    public bool _onlyOnPress;

    private Collider2D _collider;
    private Vector3 _moveStart;
    private Vector3 _moveEnd;
    private Vector2 _originalArrowDirection;
    private bool _buttonPressed = false;

    private void Awake()
    {
        Button.AddButtonPressedListener(ButtonPressedHandler);
        // do not need to bounding 'volume' of the renderer more so the direct size
        _moveStart = transform.position;
        _moveEnd = _moveStart + (Vector3)_directionToMove.normalized * _distanceToTravel;
        _originalArrowDirection = transform.up;
        _collider = GetComponent<Collider2D>();
    }


    private void ButtonPressedHandler(PuzzleColor color, bool state)
    {
        if (color != _doorColor)
        {
            return;
        }

        _buttonPressed = _onlyOnPress ? state : true;
    }

    private void FixedUpdate()
    {
        if (_buttonPressed)
        {

            // Distance moved equals elapsed time times speed.
            Vector3 delta = _directionToMove * Time.fixedDeltaTime * _speed;
            Vector3 newPosition = (Vector2)delta + (Vector2)transform.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
            CheckCollisions(delta);

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = ((Vector2)_moveStart - (Vector2)transform.position).magnitude / _distanceToTravel;

            if (fractionOfJourney >= 1)
            {
                _directionToMove *= -1;
                transform.position = _moveEnd;

                Vector3 temp = _moveEnd;
                _moveEnd = _moveStart;
                _moveStart = temp;

                // change the door move direction
                // you multiply quaternions together to 'add' them up
                //Vector3 newDirectionVector = (_moveEnd - _moveStart).normalized;
                //var angle = Vector3.Angle(_originalArrowDirection, _moveDirectionArrow.up + newDirectionVector);
                //Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                //_moveDirectionArrow.rotation = _moveDirectionArrow.rotation * rotation;

                if (_moveDirectionArrow != null)
                {
                    _moveDirectionArrow.transform.Rotate(new Vector3(0, 0, 180));
                }

                _buttonPressed = false;
            }
        }
    }


    private void CheckCollisions(Vector3 delta)
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
            result[i].GetComponent<AddOutsideDistance>().AddOutsideDistanceVector(delta);
        }
    }

}
