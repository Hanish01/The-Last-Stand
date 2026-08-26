using System;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class DialogueManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [System.Serializable]
    public class Slide
    {
        public Sprite Img;
        public string CharName;
        [TextArea] public string DialogueText;
        public AudioClip SlideSFX;

    }

    public List<Slide> Slides;

    public Image Bg;
    public TextMeshProUGUI CharName;
    public TextMeshProUGUI Dialogue;
    public TextMeshProUGUI NarrationText;
    public GameObject DialoguePanel;
    public GameObject NarrationPanel;
    public GameObject SpaceButton;

    public AudioClip textsfx;
    AudioSource AS;


    public AudioSource SFXAUDIO;
    public AudioSource BGM;

    public string NextScene = "MAIN GAMEEE";

    public bool IsnarrationDone = false;
    public int currNarrLine = 0;
    public int currslide = 0;


    Coroutine Typingcoroutine;
    bool isTyping;

    public DialogueText DT;


    public AudioClip NarrationMusic;
    public AudioClip SlideMusic;
    public AudioClip tenseDrums;



    public Image fadeBlackPanel;

    public VideoPlayer vidplayer;
    public GameObject vidscreen;

    public string[] narrationLines = {
        "Three travellers, fleeing into the unknown ...",
        "...found themselves trapped in a realm between the worlds.",
        "A FATHER And His daughter",
        "AND their Wizard Friend",
        "No way out. No way back. STUCK",
        
        "But there may be a small bit of Hope."
    };

  
    void Start()
    {
        
        DT = DialogueText.Instance;
        Debug.Log("DT is: " + DT);
        DialoguePanel.SetActive(false);
        NarrationPanel.SetActive(true);
        ShowNarration();
        BGM.clip = NarrationMusic;
        BGM.loop = true;
        BGM.Play();
    }


    void ShowNarration()
    {
        
        StartCoroutine(Textdisplay(narrationLines[currNarrLine], NarrationText));
       
       

    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isTyping)
            {
                return;
            }
            if(IsnarrationDone)
            {
                //slide stuff
               
                Nextslide();
                

            }
            else
            {
                NextNarrLine();
            }
               

        }

       
    }


    IEnumerator Textdisplay(string text, TextMeshProUGUI textonscreen)
    {
        
        isTyping = true;
        DT.TypingLoop();

        textonscreen.text = "";
        foreach (char i in text.ToCharArray())
        {
            textonscreen.text += i;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
        DT.StopTypingLoop();
        SpaceButton.SetActive(true);
    }


    void NextNarrLine()
    {
        SpaceButton.SetActive(false);

        currNarrLine++;
        NarrationText.text = "";

        if(currNarrLine >= narrationLines.Length)
        {
            IsnarrationDone = true;
            BGM.Stop();
            BGM.loop = false;
            DialoguePanel.SetActive(true);
            NarrationPanel.SetActive(false);
            StartCoroutine(FadeOut(false));
            
            ShowSlide(0);
            StartCoroutine("FadeIn");
            BGM.clip = SlideMusic;
            BGM.loop = true;
            BGM.Play();
            return;
        }
        ShowNarration();
    }

    void ShowSlide(int index)
    {
        Slide slide = Slides[index];

        if (index == 6)
        {
            BGM.clip = tenseDrums;
            BGM.loop = true;
            BGM.Play();
        }
        if (slide.Img!= null)
        {
            Bg.sprite = slide.Img;
            CharName.text = slide.CharName;
            if(slide.SlideSFX!=null)
            {
                SFXAUDIO.PlayOneShot(slide.SlideSFX);
            }
            
            StartCoroutine(Textdisplay(slide.DialogueText, Dialogue));


            
        }
    }


    IEnumerator FadeOut(bool playvid)
    {
        float alpha = 0;

        while(alpha< 1)
        {
            alpha += Time.deltaTime;
            fadeBlackPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
           

        }
        
        if(playvid)
        {
            vidscreen.SetActive(true);
            vidplayer.Prepare();

            while(!vidplayer.isPrepared)
            {
                yield return null;
            }
            
            vidplayer.Play();
           
            while(vidplayer.isPlaying)
            {
                yield return null;
            }
            SceneManager.LoadScene(NextScene);
        }


        
        
    }

    IEnumerator FadeIn()
    {
        float alpha = 1;
        while(alpha>0)
        {
            alpha = alpha - Time.deltaTime;
            fadeBlackPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
    void Nextslide()
    {
        currslide++;
        if(currslide >= Slides.Count)
        {

            StartCoroutine(FadeOut(true));
            
            return;
        }

        ShowSlide(currslide);
    }

    public void SKIP()
    {
        SceneManager.LoadScene(NextScene);
    }
}
