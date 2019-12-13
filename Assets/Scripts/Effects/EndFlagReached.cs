using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Collider2D))]
public class EndFlagReached : MonoBehaviour
{
    public AudioClip _winClip;
    public string _nextSceneName;
    private Collider2D _collider;
    private Animator _animator;
    private AudioSource _audiosource;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audiosource = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetBool("end", true);
        StartCoroutine(StartNextLevel(4));
        GetComponent<Collider2D>().enabled = false;
    }

    IEnumerator StartNextLevel(float timeToWait)
    {
        _audiosource.PlayOneShot(_winClip);
        yield return new WaitForSeconds(2f);

        GameObject.FindWithTag("Transition").GetComponent<LevelTransitionEffect>().EndLevel();

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_nextSceneName);
    }
}
