using UnityEngine;

public class LinUtils
{
    public static float Projection(Vector3 v1, Vector3 v2)
    {
        return Vector3.Dot(v1, v2) / v2.magnitude;
    }

    public static void PlayAudioDetached(AudioSource audioSource)
    {
        if(!audioSource)
            return;
        
        audioSource.transform.SetParent(null);
        audioSource.Play();
        GameObject.Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
