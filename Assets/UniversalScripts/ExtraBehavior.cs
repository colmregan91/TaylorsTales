using UnityEngine;

public class ExtraBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioClip splash;
    [SerializeField] private AudioSource source;
    public void Behavior()
    {
        particle.Play(true);
        source.PlayOneShot(splash);
    }
}
