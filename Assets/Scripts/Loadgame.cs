using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loadgame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool iscontrolpanelOn;
    public bool isInfopanelOn;
    public GameObject controlpane;
    public GameObject infopanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(iscontrolpanelOn)
            {
                controlpane.SetActive(false);
                iscontrolpanelOn = false;
            }
            else if(isInfopanelOn)
            {
                infopanel.SetActive(false);
                isInfopanelOn = false;

            }
        }    
    }


    public void Control()
    {
        controlpane.SetActive(true);
        iscontrolpanelOn = true;
    }

    public void Info()
    {
        infopanel.SetActive(true);
        isInfopanelOn = true;
    }

    public void loadScene()
    {
        SceneManager.LoadScene("Prologue");
    }


    public void QuitGame()
    {
        Application.Quit();
    }


}
