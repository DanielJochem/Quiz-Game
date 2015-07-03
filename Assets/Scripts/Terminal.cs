using UnityEngine;
using System.Collections;
using System.ComponentModel;

/// <summary>
/// Terminal Controller
/// </summary>
public class Terminal : MonoBehaviour
{
    private System.Exception programException = null;

    //If the application is stopped while we're waiting on input, we can error on exit. This is
    //set to false when the instance is destroyed. We can't check the instance from another thread
    //bacause unity doesn't let you use == in another thread (override equals is dumb)
    //This is a threadsafe way of knowing when the instance has been destroyed
    private static bool terminated = false;

    //We need this to be threadsafe, and can't use GameObjects in other threads - make our own lock object
    private static object lockObject = new object();

    /// <summary>
    /// A reference to the game object responsible for outputting to the text area
    /// </summary>
    [SerializeField]
    private UITextInput textInput;

    /// <summary>
    /// The UI component for displaying the text area of the terminal
    /// </summary>
    [SerializeField]
    private UITextArea textArea;

    /// <summary>
    /// The background worker responsible for running the program
    /// </summary>
    private BackgroundWorker programWorker;

    /// <summary>
    /// A random number generator
    /// </summary>
    private System.Random random = new System.Random();

    /// <summary>
    /// A place to store the input response for the awaiting program thread
    /// </summary>
    private string programInputResponse;

    /// <summary>
    /// The program thread has requested input
    /// </summary>
    private bool requestingInput;

    /// <summary>
    /// If the boot message should be displayed on start
    /// </summary>
    public bool playBootMessage = false;

    /// <summary>
    /// Print the boot message
    /// </summary>
    void BootMessage()
    {
        Terminal.WriteLine("-= SAEos v6.0.4a =-");
        Terminal.WriteLine();
        Terminal.WriteLine("COM1:         at 0375f");
        Terminal.WriteLine("SER1:         at 1476a");
        Terminal.WriteLine("RAM : Checking 640 KBytes ................................ OK.");
        Terminal.WriteLine("Date: " + System.DateTime.Now.AddYears(-30).ToShortDateString());
        Terminal.WriteLine();
        Terminal.WriteLine("Boot complete. All OK.");
        Terminal.WriteLine();
    }

    /// <summary>
    /// The terminal singleton instance
    /// </summary>
    private static Terminal Instance { get; set; }

    /// <summary>
    /// This is called whenever a game object is enabled
    /// </summary>
    void OnEnable()
    {
        //Subscribe to the onInput event. This will be invoked whenever the player enters text
        //By only subscribing in enable/disable, it means we will only process this event when the terminal is enabled.
        UITextInput.onInput += UITextInput_onInput;
    }

    /// <summary>
    /// This is called whenever a game object is disabled
    /// </summary>
    void OnDisable()
    {
        //Unsubscribe to the onInput event when disabled
        //This will also be called when the game object is destroyed. We must unsubscribe from static events when
        //destroyed of the game object will stay in memory.
        UITextInput.onInput -= UITextInput_onInput;
    }

    /// <summary>
    /// The method that's called whenever the user enters text input
    /// </summary>
    /// <param name="inputText">The input from the user</param>
    void UITextInput_onInput(string inputText)
    {
        lock (this)
        {
            programInputResponse = inputText;
        }
    }

    /// <summary>
    /// This is called once, and only once, before all other game objects have Start() invoked
    /// </summary>
    void Awake()
    {
        //This is an implementation of the Singleton pattern
        //We hold on to a single static instance of Terminal
        //If there is antoher instance, we need to automatically destroy it
        if (Instance != null)
        { //We have an existing instance
            GameObject.Destroy(this); //Destroy the new instance (this)
            return; //Abort the awake, this game object is about to be destroyed
        }

        //If we get here, we are the only instance - so set this game object to the static Instance variable
        Instance = this;
        terminated = false;

        //Make sure there is no text in the termianl text component
        textArea.Clear();
    }

    /// <summary>
    /// This is called when the game object has been destroyed
    /// </summary>
    void OnDestroy()
    {
        //We want to make sure we clean up when destroying singletons
        //If this game object is the active singleton, clear the static variable holding the instance of the singleton
        if (Instance == this)
        {
            terminated = true;
            Instance = null;
        }
    }

    /// <summary>
    /// This is called after every game object in the scene has had Awake() invoked.
    /// This will only ever be called once per GameObject
    /// </summary>
    void Start()
    {
        if (playBootMessage)
        {
            BootMessage();
        }

        programWorker = new BackgroundWorker();
        programWorker.DoWork += programWorker_DoWork;
        programWorker.RunWorkerCompleted += programWorker_RunWorkerCompleted;
        programWorker.RunWorkerAsync();
    }

    void programWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (programException != null)
        {
            Debug.LogError(programException);
        }
    }

    void programWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            Terminal.Write(""); //game dev fix! Prime the text area with an empty value. FIXED FOREVER!

            var program = new Program();
            program.Run();

            WriteLine();
            WriteLine("Program terminated.");
        }
        catch (System.Exception ex)
        {
            lock (lockObject)
            {
                programException = ex;
            }
        }
    }

    /// <summary>
    /// A static method to write a message to the terminal
    /// </summary>
    /// <param name="message">The message to write to the terminal</param>
    public static void Write(string message)
    {
        lock (lockObject)
        {
            if (terminated) return;

            //Write the message
            Instance.textArea.Write(message);
        }
    }

    /// <summary>
    /// Write a message to the terminal and add a new line
    /// </summary>
    /// <param name="message">The message to write to the terminal</param>
    public static void WriteLine(string message)
    {
        lock (lockObject)
        {
            if (terminated) return;

            //Write the message
            Instance.textArea.WriteLine(message);
        }
    }

    /// <summary>
    /// Write a blank line to the terminal
    /// </summary>
    public static void WriteLine()
    {
        lock (lockObject)
        {
            if (terminated) return;

            //Write the message
            Instance.textArea.WriteLine(string.Empty);
        }
    }

    void Update()
    {
        lock (lockObject)
        {
            if (requestingInput && textInput.isAvailable)
            {
                //The program thread has requested a value from the input component, and it's available for use
                //enable it now.
                requestingInput = false;
                Instance.textInput.EnableInput();
            }
        }
    }

    /// <summary>
    /// Read a line
    /// 
    /// This will block the program until the user has entered a value into the input box
    /// </summary>
    /// <returns></returns>
    public static string ReadLine()
    {
        if (terminated) return string.Empty;

        lock (lockObject)
        {
            if (terminated) return string.Empty;
            Instance.requestingInput = true;
        }

        string returnValue = null;
        while (returnValue == null)
        {
            //Sleep to prevent a tight loop
            System.Threading.Thread.Sleep(10);

            lock (lockObject)
            {
                //If we're exiting the game, we may have been terminated
                //we just want to check that Instance still exists, but we
                //cant use Instance == null because unity doesn't like that happening
                //outside of the main thread
                if (terminated)
                {
                    return string.Empty;
                }

                returnValue = Instance.programInputResponse;
                if (returnValue != null)
                {
                    Instance.programInputResponse = null;
                }
            }
        }

        return returnValue;
    }

    /// <summary>
    /// Wait for a certain number of seconds
    /// </summary>
    /// <param name="seconds">The time to wait, in seconds</param>
    public static void Wait(float seconds)
    {
        System.Threading.Thread.Sleep((int)(seconds * 1000f));
    }

    /// <summary>
    /// Generate a random integer between min and max
    /// </summary>
    /// <param name="min">The minimum random number (inclusive)</param>
    /// <param name="max">The maximum random number (exclusive)</param>
    /// <returns>A random number between min and max</returns>
    public static float Random(float min, float max)
    {
        if (terminated) return min;

        lock (Instance)
        {
            return min + ((max - min) * (float)Instance.random.NextDouble());
        }
    }

    /// <summary>
    /// Generate a random float between min and max
    /// </summary>
    /// <param name="min">The minimum random number (inclusive)</param>
    /// <param name="max">The maximum random number (exclusive)</param>
    /// <returns>A random number between min and max</returns>
    public static int Random(int min, int max)
    {
        if (terminated) return min;

        lock (lockObject)
        {
            return Instance.random.Next(min, max);
        }
    }
}
