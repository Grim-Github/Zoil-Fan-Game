using UnityEngine;

public class FootstepController : MonoBehaviour
{
    private AudioSource audioSource;
    private RaycastHit hit;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cast a ray downwards to detect the surface
        if (Physics.Raycast(transform.position +Vector3.up, Vector3.down, out hit))
        {
            //Debug.Log(hit.transform.GetComponent<Renderer>().material.name);
        }
    }

    private void PlayFootstepSound(AudioClip footstepSound)
    {
        if (footstepSound != null && !audioSource.isPlaying)
        {
            audioSource.clip = footstepSound;
            audioSource.Play();
        }
    }
}
