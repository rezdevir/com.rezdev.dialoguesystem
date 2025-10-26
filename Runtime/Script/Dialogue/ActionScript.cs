
using UnityEngine;
using System;
public class ActionScript : MonoBehaviour,IDialogueAction
{
    public string action_name;
    public void The_Action(Action act)
    {
        act?.Invoke();
    }
    public void Action_Name(string action_name)
    {
        this.action_name = action_name;
    }
}
