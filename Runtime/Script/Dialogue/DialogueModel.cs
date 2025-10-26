using UnityEngine;
using System.Collections.Generic;

public class Dialogue
{
    public string speaker { get; set; }
    public string emotion { get; set; }
    public bool isLeft { get; set; }
    public string text { get; set; }
    public string branch_id { get; set; }
    public string action_name { get; set; }
    public List<DialogueStyleModel> styles {get; set; }
    public List<MultipleChoiceModel> multiple_choice { get; set; }

}


// public enum StyleType {color,speed};

public class DialogueStyleModel
{
    // public StyleType StyleType { get; set; }
    public DStyle style {get; set;}

}


public class DStyle
{
    public string style_type { get; set; }
    public int start { get; set; }
    public int end { get; set; }
    public string data { get; set; }
}

public class MultipleChoiceModel
{
    public string choice_head { get; set; }
    public string branch_name { get; set; }
}

public class Dialogue_Data
{
    public List<Dialogue> dialogs { get; set; }

}
