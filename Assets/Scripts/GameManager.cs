using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform SpawnPoint1;
    public Transform SpawnPoint2;

    public GameObject[] Enemies;

    private int index;
    private int randomPos;

    public TextMeshProUGUI RoundsText;
    public GameObject Pause;
    public GameObject GameOver;
    public GameObject GameComplete;
    public int rounds = 6;
    private int currRound = 0;
    private int noofEnemies = 3;

    private bool isSpawning = false;
    private bool Ispaused = false;

    public BuffManager BM;

    public Animator Portal;
    public Animator Wizard;

    //cam movement
    public GameObject portalPosition;
    public float camFocusDuratiion = 2.5f;
    public float slideDuration = 0.8f;
    public FollowPlayer FollowPlayer;
    private Camera maincam;


    //final phase
    public bool FinalPhase = false;
    public GameObject Boss;
    public int FinalEnemies = 16;
    public GameObject Daughter;
    public GameObject wizard_woman;
    public VideoPlayer Video;
    public GameObject VideoScreen;

    //sfx
    public AudioManager AM;
    public PlayerController PC;

    void Start()
    {

        PC = FindAnyObjectByType<PlayerController>();
        BM = GetComponent<BuffManager>();
        maincam = Camera.main;
        FollowPlayer = maincam.GetComponent<FollowPlayer>();
        currRound++;
        AM = AudioManager.Instance;
        StartRound();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Ispaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }

        if(isSpawning)
        {
            return;
        }
        int noOfEnemies = GameObject.FindObjectsByType<EnemyController>(FindObjectsSortMode.None).Length;

        if(FinalPhase && noOfEnemies == 0 &&!isSpawning)
        {
            WinGame();
        }

        if (currRound <= rounds && noOfEnemies == 0 &&!isSpawning) 
        {
            
            isSpawning= true;
            currRound++;
            PortalAnimation();
            if (currRound > rounds)
        {
                isSpawning = true;
                FinalPhase = true;
                StartCoroutine("FalseClimax");
                return;
  
        }
            BM.BuffPanel.SetActive(true);
            PC.inputdis = true;
            AM.BgDuck();
            AM.StopLoop();
            Time.timeScale=0f;
           
            BM.GenerateBuffs();



            
            
        }

        
            
    }


    IEnumerator FalseClimax()
    {
        
        
        PC.inputdis = true;
        AM.BgAudio.Stop();
        AM.StopLoop();
        VideoScreen.SetActive(true);
        Video.Play();
        float vidlen = (float) Video.length;

        yield return new WaitForSeconds(vidlen);
       

      

        VideoScreen.SetActive(false);
        AM.BgAudio.volume = 0;

        AM.ChangeBg();
        float vol = 0;
        while(vol<0.65)
        {
            vol += Time.deltaTime / 2;
            AM.BgAudio.volume = vol;
            yield return null;

        }
        PC.inputdis = false;
        StartCoroutine(Finalwave(FinalEnemies));
    }

    void WinGame()
    {
        FinalPhase = false;
        
        Invoke("EndGame", 3f);
        return;
    }

    IEnumerator Finalwave(int No_Of_Enemies)
    {
        HashSet<int> Boss_index = new HashSet<int> { 1, 5, 7, 10, 13, 15, 17, 18 , 20,22,24,25 };
        
        Destroy(Daughter);
        Destroy(wizard_woman);

        int SpawnTime_Low = 3;
        int SpawnTime_High = 7;
        int SpawnTime_Cond = No_Of_Enemies / 3;

        for(int i=0;i< No_Of_Enemies;i++)
        {
            if(Boss_index.Contains(i))
            {
                SpawnBoss();
            }
            else
            {
                Spawner(1);
            }
            
            if(i%SpawnTime_Cond==0 && i!=0)
            {
                SpawnTime_High--;
                SpawnTime_Low--;
            }


            int val = Random.Range(3, 7);
            yield return new WaitForSeconds(val);
        }

        isSpawning = false;

    }

  



    void SpawnBoss()
    {
        int randPos = Random.Range(1,3);
        Transform SpawnPos = (randPos == 1) ? SpawnPoint1 : SpawnPoint2;
        Instantiate(Boss, SpawnPos.position, Boss.transform.rotation);
    }
    IEnumerator SlideToPortal()
    {
        FollowPlayer.enabled = false;  //player follow

        Vector3 Start = maincam.transform.position;
        Vector3 Dest = new Vector3(portalPosition.transform.position.x, portalPosition.transform.position.y, maincam.transform.position.z); //without the mian cam z, value we will be at the positionso we cant see

        yield return SlideToPoint(Start, Dest, slideDuration);
        AM.PlaySFX(AM.portal);

        yield return new WaitForSeconds(camFocusDuratiion);

        Vector3 originalPos = FollowPlayer.Player.transform.position + FollowPlayer.offset;

        yield return SlideToPoint(maincam.transform.position, originalPos, slideDuration); //whewre the cam is main cam to teh popsition the player is at even if he moves

        FollowPlayer.enabled = true;






    }

    IEnumerator SlideToPoint(Vector3 start, Vector3 dest , float duration)
    {
        float t = 0;
        while(t< duration)
        {
            t += Time.deltaTime;
            float easeVal = 1f - Mathf.Pow(1f - (t/duration), 3); // as t gets bigger the slower the cam moves , easing
            maincam.transform.position = Vector3.Lerp(start, dest, easeVal);
            yield return null;


        }
        maincam.transform.position = dest;
    }
    void PortalAnimation()
    {

        Portal.SetInteger("Portal1", currRound);
        Wizard.SetTrigger("Portal");
        if(currRound%2==0)
        {
            StartCoroutine(SlideToPortal());

            
        }
    }
    void Resume()
    {
        Pause.SetActive(false);
        Time.timeScale = 1;
        Ispaused = false;
    }

    void Paused()
    {
        Pause.SetActive(true);
        Time.timeScale = 0;
        Ispaused = true;
    }
    public void StartRound()
    {
        isSpawning = true;
        
        RoundsText.text = "Round - " + currRound;
        RoundsText.gameObject.SetActive(true);

        Invoke("DisableText", 3f);
        Invoke("SpawnAfterDelay", 2f);



    }
    void EndGame()
    {
        SceneManager.LoadScene("Ending 3");
    }
    void DisableText()
    {
        RoundsText.gameObject.SetActive(false);
            
    }

    void SpawnAfterDelay()
    {
        int enemyCount = currRound * 2;
        Spawner(enemyCount);

        isSpawning = false;
    }
    void Spawner(int noofEnemies)
    {
        for (int i = 0; i < noofEnemies; i++)
        {

            randomPos = Random.Range(1, 3);
            Transform currSpawnPoint = (randomPos == 1) ? SpawnPoint1 : SpawnPoint2;
            index = Random.Range(0, Enemies.Length);

            Instantiate(Enemies[index], currSpawnPoint.position, Quaternion.identity);
        }
    }


    public void gameOver()
    {

        StartCoroutine("GameoverSeq");


    }

    IEnumerator GameoverSeq()
    {
        GameOver.SetActive(true);
        yield return new WaitForSeconds(2f);
        if (FinalPhase)
        {
            
            SceneManager.LoadScene("Ending 2");
            
        }
        else
        {
            SceneManager.LoadScene("Ending 1");
        }
            
    }

    public void SKIP()
    {
        StopCoroutine("FalseClimax");
        Video.Stop();
        
        VideoScreen.SetActive(false);
        AM.ChangeBg();
        PC.inputdis = false;
        StartCoroutine(Finalwave(FinalEnemies));

    }
}


