using UnityEngine;
using TMPro;
public class Choice_Behaviour : MonoBehaviour
{
    TMP_Text text_text;
    Color c_color;
    MultipleChoiceModel thisChoice;

    multiple_choice_dispatcher mParent;
    // Start is called before the first frame update

    RectTransform tr;
    void Awake()
    {
        text_text = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        c_color = text_text.color;
      
    }

    public void OnEnter()
    {
        text_text.fontStyle = FontStyles.Bold;
        text_text.color = Color.green;
        Debug.Log("Enter");
    }
    public void OnExit()
    {
        text_text.fontStyle = FontStyles.Normal;
        text_text.color = c_color;
        Debug.Log("Exit");
    }
    public void OnClick()
    {
        DialogManager.Instance.SelectChoice(thisChoice);
        mParent.Clear();
       
    }
    // Update is called once per frame
    public void Initiate(MultipleChoiceModel ch,multiple_choice_dispatcher p,bool IsLeft)
    {
          tr = GetComponent<RectTransform>();
        Debug.Log(IsLeft);
        if (!IsLeft)
            tr.Rotate(new Vector3(0,180,0)) ;
        mParent = p;
        thisChoice = ch;
        text_text.text = thisChoice.choice_head;
    }
}
