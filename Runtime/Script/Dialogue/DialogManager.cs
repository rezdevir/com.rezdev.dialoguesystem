using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;


[RequireComponent(typeof(ActionComponent))]
[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Dialogue/Dialogue Manager")]
public class DialogManager : MonoBehaviour
{
    #region  delegates
    public static DialogManager Instance { get; private set; }
    public delegate void DoActionDelegate(string action);
    public DoActionDelegate OnActionDelegate;

    public delegate void MultipleChoiceDelegate(List<MultipleChoiceModel> multipleChoices, bool IsLeft,bool IsSymmetric);
    public MultipleChoiceDelegate OnMultipleChoiceDelegate;



    public delegate void EndDialogueLineDelegate();
    public EndDialogueLineDelegate OnEndDialogueLine;

    public delegate void StartDialogueLineDelegate();
    public StartDialogueLineDelegate OnStartDialogueLine;
    public delegate void StartDialogueDelegate();
    public StartDialogueDelegate OnStartDialogue;
    public delegate void EndDialogueDelegate();
    public EndDialogueDelegate OnEndtDialogue;


    #endregion delegate
    #region Inspector
    [Header("********************* UI Component *********************")]
    [Header("Next Click Object")]
    public GameObject click_Place;
    public TMP_Text name_speaker_1_txt;
    public TMP_Text text_speaker_1_txt;
    public TMP_Text name_speaker_2_txt;
    public TMP_Text text_speaker_2_txt;
    public GameObject img_speaker_1;
    public GameObject img_speaker_2;
    public GameObject Container_1;
    public GameObject Container_2;

    [Space(30)]
    [Header("********************* Settings *********************")]
    [Header("Number of click for fast skip")]
    [SerializeField] private int click_times = 1;
    [Header("Next Click Text")]
    [SerializeField] private string CLICK_FOR_NEXT = "Click for next line";


    int click_times_;

    bool isFast = false;

    [Header("Symmetric Dialogue (2 Player Show)")]
    [SerializeField] bool Is_Symmetric = false;
    [Header("The time that takes each word be written")]
    [SerializeField] private float delay = 0.1f;
    [Header("Folder Address: Resources\\ROOT_FOLDER\\ROOT_FOLDER_NAME")]
    [SerializeField] private string ROOT_FOLDER_NAME = "JSONS";
    [SerializeField] private string ROOT_FOLDER = "Dialogues";
    [SerializeField] private string FORMAT = "json";
    const string MAIN_BRANCH = "Main_Branch";
    string current_branch = MAIN_BRANCH;
    // bool isWriting = false;
    bool isClicked = false;
    [Header("Audio Source")]
    public AudioSource audio_;
    bool isMultipleChoice = false;

    [Space(10)]
    [Header("********************* Animation Manager *********************")]
    [Header("Waving Animation")]
    [SerializeField] private float amplitude_wave = 5f;
    [SerializeField] private float frequency_wave = 2f;
    [SerializeField] private float speed_wave = 2f;
    [Header("Shaking Animation")]
    [SerializeField] private float noise_rate_x = 2f;
    [SerializeField] private float noise_rate_y = 2f;

    #endregion Inspector

    List<DialogueStyleModel> AnimationList = new List<DialogueStyleModel>();
    DialogueStyleHelper DialogueStyle = new DialogueStyleHelper();
    // int current_char = 0;
    // TMP_TextInfo textInfo;
    bool Is_Speaking = false;
    TMP_Text Current_Text;
    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public void ClickingForNext()
    {
        if (click_Place.activeSelf && !isMultipleChoice)
        {

            isFast = false;

            click_times_--;
            if (click_times_ <= 0)
            {
                click_times_ = click_times;
                click_Place.SetActive(false);
                isClicked = true;
            }
        }
        else
        {
            isFast = true;

        }
    }


    bool is_start = false;

    /// <summary>
    /// File Dialoag adress .dialog
    /// </summary>
    /// <param name="name of dialogue file"></param>
    public void StartDialog(string name)
    {

        current_branch = MAIN_BRANCH;

        if (name.Equals(""))
        {
            Debug.LogError("Dialogue Name Cannot be Empty");
            return;
        }
        click_times_ = click_times;
        //Fetch dialogue file
        var dialogues = fetch(name);

        if (dialogues == null)
        {
            Debug.LogError("Dialogue File Is not exist");
            return;
        }
        if (dialogues.Count == 0)
        {
            Debug.LogError("No Dialogue found");
            return;
        }
        //Show dialogue UI
        this.gameObject.GetComponent<Transform>().GetChild(0).gameObject.SetActive(true);
        //Setting up UI
        First_Add_Image(dialogues);
       

        //Start coroutine
        StartCoroutine(Dialogue_queue(dialogues));

        //TODO:FIX THIS

        // 1- Blocking and Non-Blocking
    }
    IEnumerator Dialogue_queue(List<Dialogue> dialogues)
    {
        OnStartDialogue?. Invoke();
        foreach (var d in dialogues)
        {
            AudioPlayer();

            yield return StartCoroutine(Dialogue_Coroutine(delay, d));

            yield return new WaitUntil(() =>
            {
                AudioPlayer();
                return isClicked;
            });
        }
        this.gameObject.GetComponent<Transform>().GetChild(0).gameObject.SetActive(false);
        OnEndtDialogue?. Invoke();
    }


    void AudioPlayer()
    {
      
        if (audio_.isPlaying)
        { audio_.Stop(); }
        else
        {

            audio_.Play();
        }
    }


    public void SelectChoice(MultipleChoiceModel choice)
    {
        current_branch = choice.branch_name;
        isMultipleChoice = false;
        isFast = false;
        isClicked = true;
        //remove selection base
    }

    bool Check_Choice(List<MultipleChoiceModel> choices, bool IsLeft)
    {
        OnMultipleChoiceDelegate?.Invoke(choices, IsLeft,Is_Symmetric);
        if (choices.Count > 0)
            return true;
        return false;
    }
    void setClickPlace(string text, bool set_active)
    {
        click_Place.SetActive(set_active);
        click_Place.GetComponent<TMP_Text>().text = text;
    }
    private List<Dialogue> fetch(string name)
    {
        //Make the path it's in Resources folder
        string[] paths = { "Assets", "Resources", ROOT_FOLDER, ROOT_FOLDER_NAME, name + "." + FORMAT };
        string filePath = Path.Combine(paths);
        string json = "";
        //Approach for android fetch
        if (!File.Exists(filePath))
        {
            return null;
        }
#if UNITY_ANDROID

        
        using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON: " + www.error);
                yield break;
            }

            json = www.downloadHandler.text;
        }
#else
        //default reading file

        json = File.ReadAllText(filePath);

#endif



        Dialogue_Data loaded = JsonConvert.DeserializeObject<Dialogue_Data>(json);
        return loaded.dialogs;
    }

    private void Action_Arrived(string action)
    {

        OnActionDelegate?.Invoke(action);
    }


    //NewMethode
    #region  dialogue line Reader
    IEnumerator Dialogue_Coroutine(float delay_, Dialogue dialog)
    {
        OnStartDialogueLine?. Invoke();
        is_start = true;
        bool HasStyle = false;
        string dialogue_text = dialog.text;
        // current_branch = dialog.branch_id;
        if (current_branch.Equals(dialog.branch_id))
        {


            //if Dialouge has Style
            //Calculate Lenght
            //Check HasStyle => true
            HasStyle = DialogueStyle.HasStyle(dialog.styles);
            int lenghtLine = dialogue_text.Length;

            var delayy = delay_;
            isClicked = false;
            string converting_text = JsonToText(dialog);
            GameObject Character_Icon;
            
            bool IsLeft = dialog.isLeft;
            DialogueContainerModel container = Set_Dialogue_Container(dialog, converting_text);
            Current_Text = container.text_mesh;
            Character_Icon = container.Icon;

            for (int i = 0; i < lenghtLine; i++)
            {
                if (!Is_Speaking) Is_Speaking = true;
                //  text_speaker_1_txt.ForceMeshUpdate();
                // current_char = i ;
                if (HasStyle)
                {
                    //Check for speed style
                    delayy = DialogueStyle.CheckForSpeed(delay_, dialog.styles, i);

                }

                if (isFast)
                {
                    delayy = delay / 10;
                }

                // changeImage()
                changeImage(Character_Icon, dialog, i);
                Current_Text.maxVisibleCharacters = i + 1;

                yield return new WaitForSeconds(delayy);



            }
            Is_Speaking = false;
            isFast = false;
            if (!dialog.action_name.Equals(""))
                Action_Arrived(dialog.action_name);

            if (!Is_Symmetric)
                IsLeft = true;
            isMultipleChoice = Check_Choice(dialog.multiple_choice, IsLeft);

            if (!isMultipleChoice)
                setClickPlace(CLICK_FOR_NEXT, true);
        }

    OnEndDialogueLine?. Invoke();
    }

    DialogueContainerModel Set_Dialogue_Container(Dialogue dialog,string converting_text)
    {
        TMP_Text The_TexBox;
        GameObject Icon;            
            if (Is_Symmetric)
            {
                if (!dialog.isLeft)
                {
                    //Second Text_Container
                    name_speaker_2_txt.text = dialog.speaker;
                    text_speaker_2_txt.text = converting_text;
                    text_speaker_2_txt.gameObject.SetActive(true);
                    text_speaker_2_txt.ForceMeshUpdate();
                    The_TexBox = text_speaker_2_txt;
                    // textInfo = text_speaker_1_txt.textInfo;
                    text_speaker_2_txt.maxVisibleCharacters = 0;
                    Icon = img_speaker_2;
                Container_2.SetActive(true);
                    Container_1.SetActive(true);
                }
                else
                {
                    //First
                    name_speaker_1_txt.text = dialog.speaker;
                    text_speaker_1_txt.text = converting_text;
                    text_speaker_1_txt.gameObject.SetActive(true);
                    text_speaker_1_txt.ForceMeshUpdate();
                    The_TexBox = text_speaker_1_txt;
                    // textInfo = text_speaker_1_txt.textInfo;
                    text_speaker_1_txt.maxVisibleCharacters = 0;
                    Icon = img_speaker_1;
                }

            }
            else
            {
                name_speaker_1_txt.text = dialog.speaker;
                text_speaker_1_txt.text = converting_text;
                text_speaker_1_txt.gameObject.SetActive(true);
                text_speaker_1_txt.ForceMeshUpdate();
                The_TexBox = text_speaker_1_txt;
                // textInfo = text_speaker_1_txt.textInfo;
                text_speaker_1_txt.maxVisibleCharacters = 0;
                Icon = img_speaker_1;
            }

         DialogueContainerModel container = new DialogueContainerModel
        {
            text_mesh = The_TexBox,
            Icon = Icon

        };
        return container;
    }

    string JsonToText(Dialogue dialog)
    {

        string dialogue_text = dialog.text;
        int lenghtLine = dialogue_text.Length;
        bool HasStyle = DialogueStyle.HasStyle(dialog.styles);
        string tmp = "";
        for (int i = 0; i < lenghtLine; i++)
        {
            string char_temp = dialogue_text[i].ToString();
            if (HasStyle)
            {
                // Check for color style 
                char_temp = DialogueStyle.CheckForItalic(char_temp, dialog.styles, i);
                char_temp = DialogueStyle.CheckForBold(char_temp, dialog.styles, i);
                char_temp = DialogueStyle.CheckForColor(char_temp, dialog.styles, i);
                char_temp = DialogueStyle.CheckForSize(char_temp, dialog.styles, i);
          
          
           }
            tmp = tmp + char_temp;
        }

        AnimationList = DialogueStyle.AnimationManager(dialog);
        return tmp;
    }

    #endregion  dialogue line Reader



    // Dialogue Animation manager
    void LateUpdate()
    {
        if (is_start)
        {
            TexAnimationManager(Current_Text);
        }

    }

    #region  Icon Animation



    void changeImage(GameObject image_place, Dialogue dialog, int seq)
    {

        string folder_name = "talking-" + dialog.speaker + "-" + dialog.emotion;
        Sprite[] sprites = Resources.LoadAll<Sprite>(ROOT_FOLDER + "/Characters/" + folder_name);
        int nsprite = sprites.Count();
        if (nsprite == 0)
        {
            Debug.LogError("Dialogue Characters " + folder_name + " is missing");
            return;
        }
        int mod = seq % nsprite;
       
        image_place.GetComponent<Image>().sprite = sprites[mod];

    }



    void First_Add_Image(List<Dialogue> dialogues)
    {
        // List<Dialogue> spekers_name_s = new List<Dialogue>();
        Dialogue speaker_name_1 = new Dialogue();
        Dialogue speaker_name_2 = new Dialogue();
        bool isGetLeft = false;
        bool isGetRight = false;

        foreach (var d in dialogues)
        {
            if (d.isLeft || !Is_Symmetric)
            {
                isGetLeft = true;

                speaker_name_1 = d;
            }
            else
            {
                isGetRight = true;
                speaker_name_2 = d;
            }
            if (isGetLeft && isGetRight)
            {
                break;
            }
        }


        if (isGetLeft || !Is_Symmetric)
        {
            name_speaker_1_txt.text = speaker_name_1.speaker;
            changeImage(img_speaker_1, speaker_name_1, 0);
        }
        if (isGetRight)
        {
            name_speaker_2_txt.text = speaker_name_2.speaker;
            changeImage(img_speaker_2, speaker_name_2, 0);
        }
    }

    
    #endregion







    #region Text Animation
    void UpdateTheTextMesh(TMP_Text tmp_text,TMP_TextInfo textInfo)
    {
      
        
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {

            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp_text.UpdateGeometry(meshInfo.mesh, i);
        }
        // return tmp_text;
    }
    void TexAnimationManager(TMP_Text tmp_text)
    {
        TMP_TextInfo textInfo = tmp_text.textInfo;
        if (AnimationList.Count > 0)
        {
            tmp_text.ForceMeshUpdate();
            foreach (var text_animaiton in AnimationList)
            {
                switch (text_animaiton.style.data)
                {
                    case "waving":
                       textInfo= WaveAnimation(textInfo,
                        text_animaiton.style.start, text_animaiton.style.end);
                        break;
                    case "shaking":
                       textInfo= ShakingAnimation(textInfo,
                        text_animaiton.style.start, text_animaiton.style.end);
                        break;

                }
            }

            UpdateTheTextMesh(tmp_text,textInfo);

        }


    }

    TMP_TextInfo WaveAnimation(TMP_TextInfo textInfo,int start, int end)
    {
        // int len = text_mesh.text.Length;
        for (int i = start; i <= end; i++)
        {

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Calculate wave offset
            float wave = Mathf.Sin(Time.time * speed_wave + i * frequency_wave) * amplitude_wave;

            // Move each of the 4 vertices (quad)
            for (int j = 0; j < 4; j++)
                vertices[vertexIndex + j].y += wave;
        }

        return textInfo;

    }

    TMP_TextInfo ShakingAnimation(TMP_TextInfo textInfo,int start, int end)
    {
        // int len = text_mesh.text.Length;
        for (int i = start; i <= end; i++)
        {

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            textInfo.characterInfo[i].scale = 14 * charInfo.scale;
            float noise_x = UnityEngine.Random.Range(-noise_rate_x, noise_rate_x);
            float noise_y = UnityEngine.Random.Range(-noise_rate_y, noise_rate_y);

            // Move each of the 4 vertices (quad)
            for (int j = 0; j < 4; j++)
            {

                vertices[vertexIndex + j].y += noise_y;
                vertices[vertexIndex + j].x += noise_x;
            }

        }
        return textInfo;
    }
#endregion Text Animation
}
