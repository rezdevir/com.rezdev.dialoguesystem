
using System.Collections.Generic;

using UnityEngine;

public class multiple_choice_dispatcher : MonoBehaviour
{

    public bool Is_Left=false;
    public GameObject choiceObj;
    public bool Is_Symmetric = false;
    void Start()
    {
        DialogManager.Instance.OnMultipleChoiceDelegate += Initiate;
    }
    public void Initiate(List<MultipleChoiceModel> choices,bool IsLeft,bool IsSymmetric)
    {
        
        if (IsLeft != Is_Left) return;
        if (Is_Symmetric != IsSymmetric) return;
        int counter = 4;
        foreach (var ch in choices)
        {
            if (counter < 0)
            {
                return;
            }
            
            counter--;
            var obj = Instantiate(choiceObj, transform);
            
                obj.GetComponent<Choice_Behaviour>().Initiate(ch,this,IsLeft);
        }
    }
    public void Clear()
    {
        var all_child = transform.childCount;
        for (int i = all_child-1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).transform.gameObject);
        }
    }
}
