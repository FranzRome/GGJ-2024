using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public GameObject audioGameObj;
    public AudioClip[] jumpAudioClips;
    public AudioClip[] pushAudioClips;


    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    public void PlaySoundFXClip(string instanceName, AudioClip[] audioClip, float volume)
    {
        GameObject instance = Instantiate(audioGameObj);
        AudioSource audioSource = instance.GetComponent<AudioSource>();

        instance.name = instanceName;
        instance.transform.SetParent(transform);
        instance.transform.localPosition = Vector3.zero;

        audioSource.clip = audioClip[Random.RandomRange(0, audioClip.Length)];
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(instance, audioSource.clip.length);
    }


    public void PlayJumpAudio()
    {
        PlaySoundFXClip("JumpSFX", jumpAudioClips, 0.5f);
    }

    public void PlayPushAudio()
    {
        PlaySoundFXClip("PushSFX", pushAudioClips, 0.5f);
    }
}
