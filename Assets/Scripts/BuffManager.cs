using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //buff1
    public TMP_Text Title1;
    public TMP_Text desc1;
    public Button buffbutton1;

    //buff2
    public TMP_Text Title2;
    public TMP_Text desc2;
    public Button buffbutton2;

    public GameObject BuffPanel;
    private PlayerHealth ph;
    private GameManager gm;

    public GameObject playerFeet;
    public ParticleSystem buffeffect;
    public AudioManager AM;
    public PlayerController PC;

    private void Start()
    {
        AM = AudioManager.Instance;
        PC = FindAnyObjectByType<PlayerController>();
        
    }
    public void GenerateBuffs()
    {

        
        ph = FindAnyObjectByType<PlayerHealth>();
        gm = GetComponent<GameManager>();
        buffbutton1.onClick.RemoveAllListeners();
        buffbutton2.onClick.RemoveAllListeners();
        int randombuff1=Random.Range(0, ph.Healthtitles.Count);
        int randombuff2 = Random.Range(0, ph.Specialtitles.Count);
        Debug.Log("Health Buff Count = " + ph.Healthtitles.Count);

        Title1.text = ph.Healthtitles[randombuff1];
        desc1.text = ph.Healthdesc[randombuff1];
        buffbutton1.onClick.AddListener(() =>
        {
            ph.Healthfunct[randombuff1].Invoke();

            ph.Healthfunct.RemoveAt(randombuff1);
            ph.Healthtitles.RemoveAt(randombuff1);
            ph.Healthdesc.RemoveAt(randombuff1);
            PC.inputdis = false;
            BuffPanel.SetActive(false);
            Time.timeScale = 1f;
            AM.BgNormal();
           
            AM.PlaySFX(AM.BuffActivate);
            gm.StartRound();
            Instantiate(buffeffect, playerFeet.transform.position, Quaternion.identity);

        });

        Title2.text = ph.Specialtitles[randombuff2];
        desc2.text = ph.Specialdesc[randombuff2];
        buffbutton2.onClick.AddListener(() =>
        {
            ph.Specialfunct[randombuff2].Invoke();

            ph.Specialfunct.RemoveAt(randombuff2);
            ph.Specialtitles.RemoveAt(randombuff2);
            ph.Specialdesc.RemoveAt(randombuff2);
            BuffPanel.SetActive(false);
            Time.timeScale=1f;
            PC.inputdis = false;
            
            AM.PlaySFX(AM.BuffActivate);
            AM.BgNormal();
            gm.StartRound();
            Instantiate(buffeffect, playerFeet.transform.position, Quaternion.identity);
        });



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
