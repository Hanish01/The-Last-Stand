using System;
using System.Collections.Generic;
using TMPro;

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class DialogueManagerEndings : MonoBehaviour
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
    
    public GameObject DialoguePanel;

    public bool showCredits;
    public AudioClip textsfx;
    AudioSource AS;


    public AudioSource SFXAUDIO;
    public AudioSource BGM;

    public string NextScene = "Title screen";

   
    
    public int currslide = 0;


    Coroutine Typingcoroutine;
    bool isTyping;
    bool isTransitioning;
    public DialogueText DT;


   
    public AudioClip SlideMusic;
    public AudioClip tenseDrums;



    public Image fadeBlackPanel;
    public Image slideFadepanel;
    public GameObject EndingTxt;
    public String[] Arr;

    public TextMeshProUGUI Credits;

    
    void Start()
    {
        DT = DialogueText.Instance;
        Debug.Log("DT in Ending 3: " + DT);
        Debug.Log("BGM in Ending 3: " + BGM);
        fadeBlackPanel.color = new Color(0, 0, 0, 1);
        ShowSlide(0);
        BGM.clip = SlideMusic;
        BGM.loop = true;
        BGM.Play();
        StartCoroutine("FadeInStart");
        


    }


 
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           
            if (isTyping || isTransitioning  )
            {
                return;
            }


            StartCoroutine("Nextslide");            
               

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
            yield return new WaitForSeconds(0.1f);
        }
        isTyping = false;
      
        DT.StopTypingLoop();
        Debug.Log("Typing stopped");

    }


    void ShowSlide(int index)
    {



        Slide slide = Slides[index];

        
        if (slide.Img!= null)
        {
            Bg.sprite = slide.Img;
            CharName.text = slide.CharName;
            if(slide.SlideSFX!=null)
            {
                SFXAUDIO.PlayOneShot(slide.SlideSFX);
            }

            if(index>=11 && showCredits)
            {
                DialoguePanel.SetActive(false);
                return;
            }
            StartCoroutine(Textdisplay(slide.DialogueText, Dialogue));
            
        }
    }




    IEnumerator FadeInStart()
    {
        float alpha = 1;
        while(alpha>0)
        {
            alpha = alpha - Time.deltaTime;
            fadeBlackPanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
    }
   IEnumerator Nextslide()
    {
        isTransitioning = true;
        StopCoroutine("Textdisplay");
        float alpha = 0;
        while (alpha < 1)
        {
            slideFadepanel.color = new Color(0, 0, 0, alpha);
            alpha += (float)Time.deltaTime * 1.5f;
            yield return null;

        }

        currslide++;

        if(currslide >= Slides.Count)
        {
            DialoguePanel.SetActive(false);
            
            if (showCredits)
            {

                StartCoroutine("EndingTransition");
                yield break;
                
            }
            else
            {
                EndingTxt.SetActive(true);
                Bg.gameObject.SetActive(false);
                StartCoroutine("LoadNewScene");
                yield break;
            }







           
        }



        ShowSlide(currslide);

        

        while (alpha > 0)
        {
            slideFadepanel.color = new Color(0, 0, 0, alpha);
            alpha -= (float)Time.deltaTime * 1.5f;
            yield return null;

        }

        isTransitioning = false;
    }


    IEnumerator LoadNewScene()
    {
        float vol = BGM.volume;
        while(vol>0)
        {
            vol = vol - Time.deltaTime/3;
            BGM.volume = vol;

            yield return null;
        }
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(NextScene);
    }


    IEnumerator EndingTransition()
    {

        
        int index = 0;
        while(index < Arr.Length)
        {
            Credits.text = Arr[index];
            float  alpha = 0;
            while(alpha<1)
            {
                alpha += Time.deltaTime;
                Credits.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            yield return new WaitForSeconds(2f);
            
            while (alpha > 0 )
            {
                alpha -= Time.deltaTime;
               
                Credits.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            index++;

        }


        float vol = BGM.volume;
        while (vol > 0)
        {
            vol = vol - Time.deltaTime / 6;
            BGM.volume = vol;

            yield return null;
        }



        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(NextScene);
    }

    
}
