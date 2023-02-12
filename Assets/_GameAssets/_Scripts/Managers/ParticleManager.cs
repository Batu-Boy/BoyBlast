using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    [SerializeField] private ParticleSystem starPoof;
    [SerializeField] private ParticleSystem confetti;

    public void PlayStarPoof(Vector3 pos,float delay = 0)
    {
        starPoof.transform.position = pos;
        DOVirtual.DelayedCall(delay, () => starPoof.Play());
    }

    public void PlayConfetti()
    {
        var camPos = Camera.main.transform.position;
        camPos.z = 0;
        confetti.transform.localScale = Camera.main.orthographicSize / 2.5f * Vector3.one;
        confetti.transform.position = camPos;
        confetti.Play();
    }
}
