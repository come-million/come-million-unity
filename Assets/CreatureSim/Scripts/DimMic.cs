using UnityEngine;
using System.Collections;
using UnityEngine.Audio; // required for dealing with audiomixers

using System;

[RequireComponent(typeof(AudioSource))]
public class DimMic : MonoBehaviour
{
    public string MicrophoneNameInUse = "";


    //Written in part by Benjamin Outram

    //option to toggle the microphone listenter on startup or not
    public bool startMicOnStartup = true;

    //allows start and stop of listener at run time within the unity editor
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    //public to allow temporary listening over the speakers if you want of the mic output
    //but internally it toggles the output sound to the speakers of the audiosource depending
    //on if the microphone listener is on or off
    public bool disableOutputSound = false; 
 
     //an audio source also attached to the same object as this script is
     public AudioSource MicAudioSource;

    //make an audio mixer from the "create" menu, then drag it into the public field on this script.
    //double click the audio mixer and next to the "groups" section, click the "+" icon to add a 
    //child to the master group, rename it to "microphone".  Then in the audio source, in the "output" option, 
    //select this child of the master you have just created.
    //go back to the audiomixer inspector window, and click the "microphone" you just created, then in the 
    //inspector window, right click "Volume" and select "Expose Volume (of Microphone)" to script,
    //then back in the audiomixer window, in the corner click "Exposed Parameters", click on the "MyExposedParameter"
    //and rename it to "Volume"
    public AudioMixer masterMixer;

    float timeSinceRestart = 0;

    public bool MicInputEnabled = true;

    public bool FoundMicOnStart = false;

    public int NumMicDevicesFound = 100;
    void Start()
    {
        MicrophoneNameInUse = "";
        BasicHardwareTest();

        //CheckNumListeners(); // disable all listeners except for the/a required one 

        FoundMicOnStart = MicDeviceExists();
        if (!FoundMicOnStart)
            return;

        //start the microphone listener
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();
        }
    }

    void BasicHardwareTest()
    {
        // TEST MIC DEVICES 
        NumMicDevicesFound = Microphone.devices.Length;
        try
        {
            foreach (string str in Microphone.devices)
            {
                int minFreq, maxFreq;
                Microphone.GetDeviceCaps(str, out minFreq, out maxFreq);
                Debug.Log("Device name: " + str + " .... " + minFreq.ToString() + " --- " + maxFreq.ToString());
            }
        }
        catch { }
    }

    void CheckNumListeners()
    {
        AudioListener[] listeners = Resources.FindObjectsOfTypeAll<AudioListener>();
        if (listeners != null && listeners.Length > 1)
        {
            int numEnabledListeners = 0;
            for (int i = 0; i < listeners.Length; i++)
            {
                if (listeners[i] != null && listeners[i].enabled)
                    numEnabledListeners++;
            }

            if (numEnabledListeners > 1)
            {
                for (int i = 0; i < listeners.Length; i++)
                {
                    if (listeners[i] != null && listeners[i].enabled && listeners[i].transform == this.transform)
                    {
                        Debug.Log("Too Many Enabled Listeners! Disabled The Microphone AudioListener.");
                        listeners[i].enabled = false;
                    }
                }
            }
        }
    }   

    void Update()
    {
        //if (InstallationShowManager.Instance == null || !InstallationShowManager.Instance.LeanMeanMode)
        //{
        //    if (!MicDeviceExists())
        //        return;
        //}
        //else
        {
            if (!FoundMicOnStart)
                return;
        }

        //can use these variables that appear in the inspector, or can call the public functions directly from other scripts
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }
        //reset paramters to false because only want to execute once
        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        ////must run in update otherwise it doesnt seem to work
        MicrophoneIntoAudioSource(microphoneListenerOn);

        //can choose to unmute sound from inspector if desired
        DisableSound(!disableOutputSound);
    }


    //stops everything and returns audioclip to null
    public void StopMicrophoneListener()
    {
        //stop the microphone listener
        microphoneListenerOn = false;
        //reenable the master sound in mixer
        disableOutputSound = false;
        //remove mic from audiosource clip
        MicAudioSource.Stop();
        MicAudioSource.clip = null;

        Microphone.End(null);
    }


    public void StartMicrophoneListener()
    {
        //start the microphone listener
        microphoneListenerOn = true;
        //////disable sound output (dont want to hear mic input on the output!)
        ////disableOutputSound = true;  // YOSSI: DISABLED FOR MIC INPUT EXE TESTS
        //reset the audiosource
        RestartMicrophoneListener();
    }


    //controls whether the volume is on or off, use "off" for mic input (dont want to hear your own voice input!) 
    //and "on" for music input
    public void DisableSound(bool SoundOn)
    {

        float volume = 0;

        if (SoundOn)
        {
            volume = 0.0f;
        }
        else
        {
            volume = -80.0f;
        }

        masterMixer.SetFloat("MasterVolume", volume);
    }



    // restart microphone removes the clip from the audiosource
    public void RestartMicrophoneListener()
    {
        if (MicAudioSource == null)
            MicAudioSource = GetComponent<AudioSource>();

        //remove any soundfile in the audiosource
        MicAudioSource.clip = null;

        timeSinceRestart = Time.time;
    }

    bool MicDeviceExists()
    {
        if (MicInputEnabled && Microphone.devices.Length <= 0)
        {
            Debug.Log("No Microphone devices found. Disabled Mic Input.");
            MicInputEnabled = false;
            if (MicAudioSource != null && MicAudioSource.isPlaying)
                MicAudioSource.Stop();
            return false;
        }

        return true;
    }

    //puts the mic into the audiosource
    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        //if (InstallationShowManager.Instance == null || !InstallationShowManager.Instance.LeanMeanMode)
        //{
        //    if (!MicDeviceExists())
        //        return;
        //}
        //else
        {
            if (!FoundMicOnStart)
                return;
        }

        if (MicrophoneListenerOn)
        {
            //pause a little before setting clip to avoid lag and bugginess
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                string inputDeviceName = null;
                foreach (string str in Microphone.devices)
                {
                    //int minFreq, maxFreq;
                    //Microphone.GetDeviceCaps(str, out minFreq, out maxFreq);
                    //Utils.LogError("Device name: " + str + " .... " + minFreq.ToString() + " --- " + maxFreq.ToString());
                    Debug.Log("Device name: " + str);// + " .... " + minFreq.ToString() + " --- " + maxFreq.ToString());
                    inputDeviceName = str;
                }
                //src.clip = Microphone.Start(null, true, 10, 44100);

                //inputDeviceName = "Microphone";
                //inputDeviceName = "Input 1/2";
                //inputDeviceName = "Microphone (Xonar U7)";
                //inputDeviceName = "Line (Xonar U7)";
                //inputDeviceName = "Microphone (3 - Xonar U7)";
                //inputDeviceName = "Komplete Audio 6 Input 1/2 (Komplete Audio 6 WDM Audio)";

                MicrophoneNameInUse = inputDeviceName;// "Microphone (Komplete Audio 6)";//inputDeviceName
                if (true)
                {
                    MicAudioSource.clip = Microphone.Start(MicrophoneNameInUse, true, 10, 44100);
                    //MicAudioSource.clip = Microphone.Start(null, true, 1, 44100);

                    //wait until microphone position is found (?)
                    while (!(Microphone.GetPosition(MicrophoneNameInUse) > 0))
                    {
                    }

                    MicAudioSource.Play(); // Play the audio source
                }
            }
        }
    }

}