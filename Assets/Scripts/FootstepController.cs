using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioClip grassFootstepSound;
    public AudioClip woodFootstepSound;
    public AudioClip stoneFootstepSound;

    private AudioSource audioSource;
    private RaycastHit hit;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cast a ray downwards to detect the surface
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f))
        {
            string materialTag = hit.collider.tag;

            Debug.Log(materialTag);

            switch (materialTag)
            {
                case "Grass":
                    PlayFootstepSound(grassFootstepSound);
                    break;
                case "Wood":
                    PlayFootstepSound(woodFootstepSound);
                    break;
                case "Stone":
                    PlayFootstepSound(stoneFootstepSound);
                    break;
            }
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
