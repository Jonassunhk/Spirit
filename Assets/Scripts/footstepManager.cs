using UnityEngine;
using UnityEngine.InputSystem;

public class FootstepController : MonoBehaviour
{
    public AudioClip[] footstepClips; // Array of footstep sounds
    private AudioSource audioSource;
    public Animator playerAnimator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void onRightFootStep()
    {
        PlayFootstepSound();
    }

    public void onLeftFootStep()
    {
        PlayFootstepSound();
    }

    public void PlayFootstepSound()
    {
        if (!playerAnimator.IsInTransition(0))
        {
           AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];
           audioSource.clip = randomClip;
           audioSource.volume = Random.Range(9, 10) / 10.0f;
           //audioSource.pitch = Random.Range(10, 11) / 10.0f;
           audioSource.Play();
        }

    }
}