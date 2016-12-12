using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Text exampleText;

    //This function is called from the RecoServer script
    //Use the newSpokenWord data however you see fit
    public void UseVoiceData(string newSpokenWord)
    {
        //use voice data here!
        exampleText.text = newSpokenWord;
    }

}
