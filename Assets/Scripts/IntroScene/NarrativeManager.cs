using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NarrativeManager : MonoBehaviour
{
    public Text nameText;
    public Text narrativeText;
    public Button nextButton;

    public Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNarrative(Narrative narrative)
    {
        nextButton.gameObject.SetActive(true);
        
        // Debug.Log("Starting narrative: " + narrative.name);
        nameText.text = narrative.name;

        sentences.Clear();

        foreach (string sentence in narrative.sentences)
           sentences.Enqueue(sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            LoadNextScene();
            return;
        }

        string sentence = sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        // narrativeText.text = sentence;
        // Debug.Log(sentence);
    }

    IEnumerator TypeSentence(string sentence)
    {
        narrativeText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            narrativeText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(6);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
