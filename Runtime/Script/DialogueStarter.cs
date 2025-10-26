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

    }

}
