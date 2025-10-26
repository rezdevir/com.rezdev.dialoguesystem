using System.Collections;

using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    public string dialogname="test";
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(delay());
    }
    IEnumerator delay()
    {

        yield return new WaitForSeconds(1);
        // Start Instance of dialogue
        DialogManager.Instance.StartDialog(dialogname, false);
        DialogManager.Instance.OnEndDialogueLine += DialogLineEnd;
        DialogManager.Instance.OnStartDialogueLine += DialogLineStart;
        DialogManager.Instance.OnStartDialogue += DialogSystemStart;
        DialogManager.Instance.OnEndtDialogue += DialogSystemEnd;
    }
    void DialogLineEnd()
    {
        Debug.Log("At the end of Dialogue Line");
    }
    void DialogLineStart()
    {
        Debug.Log("Start of Dialogue Line");
    }
    void DialogSystemStart()
    {
        Debug.Log("Dialogue has started");
    }

        void DialogSystemEnd()
    {
        Debug.Log("Dialogue has Ended");
    }

}
