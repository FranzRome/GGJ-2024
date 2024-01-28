using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
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
    public void PlaySoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //random sound
        int rand = Random.Range(0, audioClip.Length);
        //spawn in Object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //Assign audio clip
        audioSource.clip = audioClip[rand];
        //Assign volume
        audioSource.volume = volume;
        //Play sound
        audioSource.Play();
        //Get lenght of clip
        float clipLength = audioSource.clip.length;
        //Destroy after stop
        Destroy(audioSource.gameObject, clipLength);
    }
}
