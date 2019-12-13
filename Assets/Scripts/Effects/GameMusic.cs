using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusic : MonoBehaviour
{
    public static GameMusic _gameMusicInstance;
    public List<AudioClip> _musicClips;

    private AudioSource _audiosource;
    private int _songPlaying;
    private int _addValue;

    private void Awake()
    {
        if (_gameMusicInstance == null)
        {
            _gameMusicInstance = this;
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        _audiosource = GetComponent<AudioSource>();
        _songPlaying = Random.Range(0, _musicClips.Count);
        _addValue = Random.Range(1, _musicClips.Count);
        _audiosource.clip = _musicClips[_songPlaying];
        _audiosource.Play();
        Invoke("ChangeClips", _audiosource.clip.length);
    }

    private void ChangeClips()
    {
        _songPlaying = (_songPlaying + _addValue) % _musicClips.Count;
        _audiosource.clip = _musicClips[_songPlaying];
        _audiosource.Play();
        Invoke("ChangeClips", _audiosource.clip.length);
    }
}
