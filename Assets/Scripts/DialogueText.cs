using UnityEngine;

public class DialogueText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DialogueText Instance;
    public AudioSource TextSfx;
    public AudioClip Textsound;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject); 
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TypingLoop()
    {
        Debug.Log("TypingLoop called, TextSfx: " + TextSfx + " Textsound: " + Textsound);
        if (!TextSfx.isPlaying)
        {
            TextSfx.clip = Textsound;
            TextSfx.loop = true;
            TextSfx.Play();
            
        }

    }

    public void StopTypingLoop()
    {
        TextSfx.Stop();
        TextSfx.loop = false; // add this
        TextSfx.clip = null;
    }
}
