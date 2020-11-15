using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Mirror;

#region RumblePatterns
public enum RumblePattern
{
    Constant,
    Pulse,
    Progressive,
    ProgressivePulse
}
#endregion

public class Rumbler : NetworkBehaviour
{
    #region Cached
    [SyncVar]
    [SerializeField] private RumblePattern activeRumbePattern;
    [SyncVar]
    [SerializeField] private float rumbleDurration;
    [SyncVar]
    [SerializeField] private float rumbleDurrationTime;
    [SyncVar]
    [SerializeField] private float pulseDurration;
    [SyncVar]
    [SerializeField] private float lowA;
    [SyncVar]
    [SerializeField] private float lowStep;
    [SyncVar]
    [SerializeField] private float highA;
    [SyncVar]
    [SerializeField] private float highStep;
    [SyncVar]
    [SerializeField] private float rumbleStep;
    [SyncVar]
    [SerializeField] float lowStepVar;
    [SyncVar]
    [SerializeField] float highStepVar;
    [SyncVar]
    [SerializeField] private bool isMotorActive;



    public MasterUIController UIController;

    public PlayerInput _playerInput;

    public TMP_Text PercentageTextLow;

    public TMP_Text PercentageTextHigh;

    public TMP_Text CounterTimeText;

    public TMP_Text timeDotTime;
    #endregion

    private void Start()
    {
        PercentageTextLow = GameObject.Find("TestUI/LF %").GetComponent<TMP_Text>();
        PercentageTextHigh = GameObject.Find("TestUI/HF %").GetComponent<TMP_Text>();
        CounterTimeText = GameObject.Find("TimeCounter").GetComponent<TMP_Text>();
    }

    #region Methods
    // Public Methods
    public void RumbleConstant(float low, float high, float durration)
    {
        activeRumbePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        rumbleDurration = Time.time + durration + 10f;
    }

    public void RumblePulse(float low, float high, float burstTime, float durration)
    {
        activeRumbePattern = RumblePattern.Pulse;
        lowA = low;
        highA = high;
        rumbleStep = burstTime;
        pulseDurration = Time.time + burstTime;
        rumbleDurration = Time.time + durration + 10f;
        isMotorActive = true;
    }

    public void PositiveRumbleLinear(float durration)
    {
        activeRumbePattern = RumblePattern.Progressive;
        highStep = 1f / durration;
        lowStep = 1f / durration;
        rumbleDurration = Time.time + durration + 10f;
    }

    public void RumbleProgressivePulse(float durration)
    {
        activeRumbePattern = RumblePattern.ProgressivePulse;
        highStep = 1f / durration;
        lowStep = 1f / durration;
        rumbleStep = 1f;
        rumbleDurration = Time.time + durration + 10f;
        pulseDurration = rumbleStep / rumbleDurration;
        isMotorActive = true;
    }

    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            rumbleDurration = 0;

            gamepad.SetMotorSpeeds(0, 0);

            lowA = 0;
            highA = 0;
            highStep = 0;
            lowStep = 0;
            lowStepVar = 0;
            rumbleStep = 0;
            highStepVar = 0;
            isMotorActive = false;
            pulseDurration = 0;
            rumbleDurrationTime = 0;
            CounterTimeText.text = "00:00";
            PercentageTextLow.text = "0%";
            PercentageTextHigh.text = "0%";
        }
    }

    #endregion

    #region MonoBehaviors
    private void Update()
    {
        if (Time.time > rumbleDurration)
        {
            StopRumble();
            return;
        }

        var gamepad = GetGamepad();
        if (gamepad == null)
            return;

        switch (activeRumbePattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(lowA, highA);
                break;

            case RumblePattern.Pulse:

                if (Time.time > pulseDurration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDurration = Time.time + rumbleStep;
                    if (!isMotorActive)
                    {
                        gamepad.SetMotorSpeeds(0, 0);
                    }
                    else
                    {
                        gamepad.SetMotorSpeeds(lowA, highA);
                    }
                }

                break;
            case RumblePattern.Progressive:
                gamepad.SetMotorSpeeds(highA, lowA);

                if(UIController.HfToggle.isOn == false)
                {
                    lowA = 0;
                }
                else
                {
                    lowA += (lowStep * Time.deltaTime);
                }
                if(UIController.LfToggle.isOn == false)
                {
                    highA = 0;
                }
                else
                {
                    highA += (highStep * Time.deltaTime);
                }
                if (highA >= 1)
                {
                    highA = 1;
                }
                if(lowA >= 1)
                {
                    lowA = 1;
                }
                break;
            case RumblePattern.ProgressivePulse:
                if (Time.time > pulseDurration)
                {
                    isMotorActive = !isMotorActive;
                    if (!isMotorActive)
                    {
                        gamepad.SetMotorSpeeds(0, 0);
                    }
                    else
                    {
                        gamepad.SetMotorSpeeds(lowStepVar, highStepVar);

                        if (UIController.HfToggle.isOn == false)
                        {
                            lowStepVar = 0;
                        }
                        else
                        {
                            lowStepVar += (lowStep * Time.deltaTime);
                        }
                        if (UIController.LfToggle.isOn == false)
                        {
                            highStepVar = 0;
                        }
                        else
                        {
                            highStepVar += (highStep * Time.deltaTime);
                        }

                        if (highStepVar >= 1)
                        {
                            highA = 1;
                        }
                        if (lowStepVar >= 1)
                        {
                            lowA = 1;
                        }
                    }
                }
                break;
            default:
                break;
        }
        // Calculations for minuets and seconds
        rumbleDurrationTime = rumbleDurration - Time.time;
        rumbleDurrationTime = rumbleDurrationTime -= Time.deltaTime;
        float rumbleDurrationTimeMinutes = rumbleDurrationTime / 60;
        float rumbleDurrationTimeSeconds = rumbleDurrationTime % 60;
        //Rounding variable of the minuets and seconds to the string
        CounterTimeText.text = Mathf.FloorToInt(rumbleDurrationTimeMinutes).ToString("00") + Mathf.FloorToInt(rumbleDurrationTimeSeconds).ToString(":00");
        //Making sure the Time is always displayed as 0 after the rounding
        if (CounterTimeText.text == "00:00" || CounterTimeText.text == "-01-:01")
        {
            CounterTimeText.text = "00:00";
        }
        // Since HighA and LowA are now the opposite, we're switching the vaules to the opposite TextBox
        if (activeRumbePattern == RumblePattern.Progressive)
        {
            PercentageTextLow.text = highA.ToString("p0");
            PercentageTextHigh.text = lowA.ToString("p0");
        }
        if (activeRumbePattern == RumblePattern.ProgressivePulse)
        {
            PercentageTextLow.text = highStepVar.ToString("p0");
            PercentageTextHigh.text = lowStepVar.ToString("p0");
        }
        else
        {
            PercentageTextLow.text = lowA.ToString("p0");
            PercentageTextHigh.text = highA.ToString("p0");
        }
        timeDotTime.text = Mathf.FloorToInt(Time.time).ToString();
    }
    #endregion
    private void OnDestroy()
    {
        StopAllCoroutines();
        StopRumble();
    }
    public Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => _playerInput.devices.Any(d => d.deviceId == g.deviceId));

        #region Linq Query Equivalent Logic
        //Gamepad gamepad = null;
        //foreach (var g in Gamepad.all)
        //{
        //    foreach (var d in _playerInput.devices)
        //    {
        //        if(d.deviceId == g.deviceId)
        //        {
        //            gamepad = g;
        //            break;
        //        }
        //    }
        //    if(gamepad != null)
        //    {
        //        break;
        //    }
        //}
        //return gamepad;
        #endregion
    }
}