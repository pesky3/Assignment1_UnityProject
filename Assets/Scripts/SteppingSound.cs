using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteppingSound : MonoBehaviour
{
    public AudioSource audioSource; 
    public List<AudioClip> grassSounds; 
    public List<AudioClip> rockSounds; 
    public List<AudioClip> metalSounds;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f; 
    public float stepInterval = 0.5f; 

    private float stepTimer = 0.0f;
    private string currentSurface = "rock"; 

    void Update()
    {


        if (Input.GetKey(KeyCode.LeftShift))
        {
            stepInterval = 0.35f;
        }
        else
        {
            stepInterval = 0.5f; //slower step sound when not sprinting
        }
  
        if (IsPlayerMoving())
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                UpdateCurrentSurface();
                PlayStepSound();
                stepTimer = stepInterval;
            }
        }


    }

    bool IsPlayerMoving()
    {
        return Input.GetAxis("Vertical") != 0;
    }

    void PlayStepSound()
    {
        List<AudioClip> selectedClips = rockSounds; //default surface

        //determine sound list based on surface type
        switch (currentSurface)
        {
            case "grass":
                selectedClips = grassSounds;
                break;
            case "metal":
                selectedClips = metalSounds;
                break;
            case "rock":
            default:
                selectedClips = rockSounds;
                break;
        }

        //randomly select a clip and put some variance
        AudioClip clip = selectedClips[Random.Range(0, selectedClips.Count)];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(clip);
    }

    void UpdateCurrentSurface()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f; 
        Vector3 direction = Vector3.down;

     
        if (Physics.Raycast(origin, direction, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("Grass"))
            {
                currentSurface = "grass";
            }
            else if (hit.collider.CompareTag("Rock"))
            {
                currentSurface = "rock";
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                currentSurface = "metal";
            }

            Debug.Log($"Surface detected: {currentSurface}");
        }
        else
        {
            Debug.Log("No surface detected.");
        }
    }
}
