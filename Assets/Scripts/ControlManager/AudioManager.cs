using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceVFX;
    
    [Header("VFX")]
    [SerializeField] private AudioClip swordSound;
    [SerializeField] private AudioClip footStep;
    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioClip focus;
    [SerializeField] private AudioClip brokenSand;
    
    [Header("Music")]
    [SerializeField] private AudioClip StartMusic;
    [SerializeField] private AudioClip FightMusic;
    
    
    
    
    public void PlaySwordSound()
    {
        audioSourceVFX.PlayOneShot(swordSound);
     
    }
    public void PlayHurtSound()
    {
        audioSourceVFX.PlayOneShot(hurt);
     
    }
    public void PlayFocusSound()
    {
        audioSourceVFX.PlayOneShot(focus);
     
    }

    public void PlayBrokenSandSound()
    {
        audioSourceVFX.PlayOneShot(brokenSand);
    }
    public void PlayFootStep(bool isPlay)
    {
        audioSourceVFX.clip = footStep;
        if (isPlay)
        {
            audioSourceVFX.Play();
        }
        else
        {
            audioSourceVFX.Stop();
        }
    }
    
    private void PlayMusic(AudioClip clip)
    {
        audioSourceMusic.clip = clip;
        audioSourceMusic.Play();
    }
    public void PlayStartMusic()
    {
        PlayMusic(StartMusic);
    }

    public void PlayFightMusic()
    {
        PlayMusic(FightMusic);
    }
}
