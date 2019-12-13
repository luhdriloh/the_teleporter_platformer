using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireSwitch : MonoBehaviour
{
    private Animator _animator;
    private Collider2D _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        ToggleSwitch(true);
    }

    public void ToggleSwitch(bool on)
    {
        _animator.SetBool("fireOn", on);
        _collider.enabled = on;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerPlatformerController>().Die();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("TeleportationOrb"))
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerPlatformerController>().DestroyOrb(collision.gameObject.GetComponent<TeleportationOrb>());
        }
    }
}
