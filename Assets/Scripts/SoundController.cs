using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource SFXSource;

    public AudioClip Jump;
    public AudioClip CoinPickup;
    public AudioClip Explode;

    public void PlayJumpSFX()
    {
        SFXSource.Stop();
        SFXSource.clip = Jump;
        SFXSource.Play();
    }

    public void PlayCoinPickupSFX()
    {
        SFXSource.Stop();
        SFXSource.clip = CoinPickup;
        SFXSource.Play();
    }

    public void PlayExplodeSFX()
    {
        SFXSource.Stop();
        SFXSource.clip = Explode;
        SFXSource.Play();
    }
}
