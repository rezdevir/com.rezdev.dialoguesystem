
using System.Collections.Generic;


public class DialogueStyleHelper
{

    // public static DialogueStyleHelper Instance { get;  set; }
    public bool HasStyle(List<DialogueStyleModel> styles)
    {
        if (styles == null)
            return false;
        if (styles.Count == 0)
            return false;


        return true;
    }

    public float CheckForSpeed(float delay, List<DialogueStyleModel> styles, int i)
    {
        foreach (var style in styles)
        {
            if (style.style.style_type.Equals("speed"))
            {
                if (i >= style.style.start && i <= style.style.end)
                {


                    return float.Parse(style.style.data);
                }
            }
        }
        return delay;
    }

    public string CheckForColor(string c, List<DialogueStyleModel> styles, int i)
    {
        foreach (var style in styles)
        {
            if (style.style.style_type.Equals("color"))
            {
                if (i >= style.style.start && i <= style.style.end)
                {

                    string out_text = "<color=" + style.style.data + ">" + c + "</color>";
                    // Debug.Log(style.style.data);
                    return out_text;
                }
            }
        }
        return c.ToString();
    }


    public string CheckForSize(string c, List<DialogueStyleModel> styles, int i)
    {
        foreach (var style in styles)
        {
            if (style.style.style_type.Equals("size"))
            {
                if (i >= style.style.start && i <= style.style.end)
                {

                    string out_text = "<size=" + style.style.data + ">" + c + "</size>";
                    // Debug.Log(style.style.data);
                    return out_text;
                }
            }
        }
        return c.ToString();
    }


    public string CheckForBold(string c, List<DialogueStyleModel> styles, int i)
    {
        foreach (var style in styles)
        {
            if (style.style.style_type.Equals("bold"))
            {
                if (i >= style.style.start && i <= style.style.end)
                {

                    string out_text = "<b=" + style.style.data + ">" + c + "</b>";
                    // Debug.Log(style.style.data);
                    return out_text;
                }
            }
        }
        return c.ToString();
    }

    public string CheckForItalic(string c, List<DialogueStyleModel> styles, int i)
    {
        foreach (var style in styles)
        {
            if (style.style.style_type.Equals("italic"))
            {
                if (i >= style.style.start && i <= style.style.end)
                {

                    string out_text = "<i=" + style.style.data + ">" + c + "</i>";
                    // Debug.Log(style.style.data);
                    return out_text;
                }
            }
        }
        return c.ToString();
    }



    public List<DialogueStyleModel> AnimationManager(Dialogue dialogue)
    {
        List<DialogueStyleModel> animation_list = new List<DialogueStyleModel>();
        //Return The Animation List !
        foreach (var style in dialogue.styles)
        {
            if (style.style.style_type == "animation")
            {
                //This is Animation
                animation_list.Add(style);
            }
        }
        return animation_list;
    }

    public Dialogue DialogueManager(Dialogue dialogue)
    {
        if (AnimationManager(dialogue).Count < 0) return dialogue;
        Dialogue new_dialogue_order = dialogue;

        return new_dialogue_order;
    }




}
