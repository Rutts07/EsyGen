using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStart()
    {
        // Debug.Log("Start");
        SceneManager.LoadScene(5);
    }

    public void OnClickControls()
    {
        Debug.Log("Controls");
        SceneManager.LoadScene(8);
    }

    public void OnClickCredits()
    {
        Debug.Log("Credits");
        SceneManager.LoadScene(9);
    }

    public void OnClickExit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
