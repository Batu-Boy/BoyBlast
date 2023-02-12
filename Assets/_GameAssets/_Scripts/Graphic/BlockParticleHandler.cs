using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockParticleHandler : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particles;

    public void PlayDestroy()
    {
        particles[0].Play();
    }
    
    public void PlayStarPoof()
    {
        particles[1].Play();
    }

    public void PlayStarTrail()
    {
        particles[2].Play();
    }

    public void StopStarTrail()
    {
        particles[2].Stop();
    }
}
