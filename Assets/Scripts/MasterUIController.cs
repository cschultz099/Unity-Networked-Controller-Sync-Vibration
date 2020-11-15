using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MasterUIController : NetworkBehaviour
{

    private float rumbleTime = 20f;

    public Rumbler rumbler;
    public GameObject LFToggle;
    public Toggle LfToggle;
    public GameObject HFToggle;
    public Toggle HfToggle;
    public GameObject LFSlider;
    public GameObject HFSlider;
    public GameObject PFSlider;
    public Slider lowSlider;
    public Slider highSlider;
    public Slider PulseFreqSlider;

    private RumblePattern rumblePattern = RumblePattern.Constant;

    private int[] timeDropdown = new int[] { 20, 50, 110, 170, 290, 590, 890, 1190, 1790 };
    private RumblePattern[] rumbleMode = new RumblePattern[] { RumblePattern.Constant, RumblePattern.Progressive, RumblePattern.Pulse, RumblePattern.ProgressivePulse };

    public void SetDurration(int selectedValue)
    {
        rumbleTime = timeDropdown[selectedValue];
    }

    public void SetRumbleMode(int selectedValue)
    {
        rumblePattern = rumbleMode[selectedValue];
    }

    public void StartPressed()
    {
        switch (rumblePattern)
        {
            case RumblePattern.Constant:
                rumbler.RumbleConstant(lowSlider.value, highSlider.value, rumbleTime);
                break;
            case RumblePattern.Pulse:
                rumbler.RumblePulse(lowSlider.value, highSlider.value, PulseFreqSlider.value, rumbleTime);
                break;
            case RumblePattern.Progressive:
                rumbler.PositiveRumbleLinear(rumbleTime);
                break;
            case RumblePattern.ProgressivePulse:
                rumbler.RumbleProgressivePulse(rumbleTime);
                break;
            default:
                break;
        }
        
    }

    private void Update()
    {
        if(rumblePattern == RumblePattern.Progressive)
        {
            LFToggle.SetActive(true);
            HFToggle.SetActive(true);
            LFSlider.SetActive(false);
            HFSlider.SetActive(false);
        }
        else
        {
            LFToggle.SetActive(false);
            HFToggle.SetActive(false);
            LFSlider.SetActive(true);
            HFSlider.SetActive(true);
        }

        if(rumblePattern == RumblePattern.Pulse)
        {
            PFSlider.SetActive(true);
        }
        else
        {
            PFSlider.SetActive(false);
        }
        if(rumblePattern == RumblePattern.ProgressivePulse)
        {
            LFSlider.SetActive(false);
            HFSlider.SetActive(false);
            LFToggle.SetActive(true);
            HFToggle.SetActive(true);
        }
    }

    public void StopPressed()
    {
        rumbler.StopRumble();
    }
}
