using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// A component for managing input from the user
/// 
/// Warning - this is a bit hacky. Sometimes you need to wrangle ui components to get them to do what you want
/// </summary>
public class UITextInput : MonoBehaviour {

    public bool isAvailable { get; private set; }

    /// <summary>
    /// An event that is raised whenever the user enters text
    /// </summary>
    public static event System.Action<string> onInput;

    /// <summary>
    /// A mask that will be enabled whenever input is blocked
    /// </summary>
    [SerializeField]
    Image inactiveMask;

    /// <summary>
    /// The field used to collect user input
    /// </summary>
    [SerializeField]
    InputField input;

    void Awake()
    {
        //We want to automatically disable input when printing starts, so monitor the start event
        UITextArea.onPrintStart += UITextArea_onPrintStart;
        UITextArea.onPrintEnd += UITextArea_onPrintEnd;

        isAvailable = true;
    }

    void OnDestroy()
    {
        //Always unsubscribe from static events when disabled
        UITextArea.onPrintStart -= UITextArea_onPrintStart;
        UITextArea.onPrintEnd -= UITextArea_onPrintEnd;
    }

    void UITextArea_onPrintEnd()
    {
        //If printing has finished, make the input available agian
        isAvailable = true;
    }

    void UITextArea_onPrintStart()
    {
        //Printing has started, so disable input
        BlockInput();
    }

	void Start () {
        BlockInput();
	}

	void Update () {
        if (!input.isFocused && input.IsInteractable())
        {
            //This is pretty hacky - once we select the game object, we want to manually send a click event so it
            //becomes for realz focus, not just sort-of focused
            EventSystem.current.SetSelectedGameObject(input.gameObject);
            input.OnPointerClick(new PointerEventData(EventSystem.current));
        }
	}

    /// <summary>
    /// Block the user from accessing this component
    /// </summary>
    public void BlockInput()
    {
        isAvailable = false;
        input.interactable = false;
        inactiveMask.gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable the input
    /// </summary>
    public void EnableInput()
    {
        input.interactable = true;
        inactiveMask.gameObject.SetActive(false);
    }

    public void OnInputSubmit()
    {
        //We've received input, so read it, clear the text control
        var inputValue = input.text;
        input.text = string.Empty;
        BlockInput();

        //Not notify anyone listening that we have new input
        if (onInput != null)
        {
            onInput(inputValue);
        }
    }

    public void OnInputChange()
    {
        //This is an amazingly terrible hack to get UnityUI input fields to actually clear themselves
        if (input.text.Length == 1 && char.IsWhiteSpace(input.text[0]))
        {
            input.text = string.Empty;
        }
    }
}
