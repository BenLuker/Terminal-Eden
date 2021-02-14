using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;

public class VNDialogue : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public RawImage coloredBar;

    public TextAnimator textAnimator; // TextAnimator API: https://www.textanimator.febucci.com/api/Febucci.UI.TextAnimator.html
    public TextAnimatorPlayer textAnimatorPlayer; // TextAnimatorPlayer API: https://www.textanimator.febucci.com/api/Febucci.UI.TextAnimatorPlayer.html

    public UnityEvent continueDialogue;
    private IEnumerator wait;

    #region Methods for Libretto

    public void NarratorTalk(string dialogue)
    {
        nameText.text = "";
        nameText.color = Color.white;
        coloredBar.color = Color.white;
        panel.SetActive(true);
        textAnimatorPlayer.ShowText(dialogue);
    }

    public void BenTalk(string dialogue)
    {
        nameText.text = "Ben";
        nameText.color = Color.red;
        coloredBar.color = Color.red;
        panel.SetActive(true);
        textAnimatorPlayer.ShowText(dialogue);
    }

    #endregion

    // To be called when the player clicks to continue dialogue
    public void ContinueDialogue()
    {
        // If all of the letters are typed out
        if (textAnimator.allLettersShown)
        {
            // Hide the dialogue panel
            panel.SetActive(false);

            // Tell the Libretto to play the next command
            continueDialogue.Invoke();
        }
        else
        {
            // Complete the dialogue
            textAnimator.ShowAllCharacters(true);
        }
    }
}
