using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    public Narrative narrative;

    public void TriggerNarrative()
    {
        FindObjectOfType<NarrativeManager>().StartNarrative(narrative);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
