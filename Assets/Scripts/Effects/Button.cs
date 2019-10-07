using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ButtonPressed(PuzzleColor color);

public enum PuzzleColor
{ 
    RED = 1,
    BLUE,
    YELLOW,
    GREEN
}

public class Button : MonoBehaviour
{
    public static event ButtonPressed _buttonEvent;
    public float _buttonPressDepth = -0.152f;
    public PuzzleColor _buttonColor;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private bool _pressed;

    // for the button to go down we need
    // - distance to go down
    // - the time to deactivate the button for

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _pressed = false;
    }


    private bool CheckPlayerCollision()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = LayerMask.GetMask("Player"),
            useTriggers = true,
            useLayerMask = true
        };

        Collider2D[] result = new Collider2D[1];
        int collidersHit = _collider.OverlapCollider(filter, result);
        return collidersHit > 0;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 newPosition = transform.position;
        newPosition.y += _buttonPressDepth;
        transform.position = newPosition;
        _pressed = true;

        OnButtonPress();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        Vector3 newPosition = transform.position;
        newPosition.y -= _buttonPressDepth;
        transform.position = newPosition;
        _pressed = false;
    }


    public static void AddButtonPressedListener(ButtonPressed buttonPressEvent)
    {
        _buttonEvent += buttonPressEvent;
    }


    public void OnButtonPress()
    {
        if (_buttonEvent != null)
        {
            _buttonEvent(_buttonColor);
        }
    }
}
