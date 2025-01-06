using UnityEngine;
using TMPro;

public class QualitySettingsControl : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
     
        // Set the initial value of the dropdown to the current quality level
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        qualityDropdown.value = currentQualityLevel > 2 ? 0 : (currentQualityLevel > 0 ? 1 : 2);
        qualityDropdown.RefreshShownValue();

        // Add a listener to call the OnQualityChanged method whenever the dropdown value changes
        qualityDropdown.onValueChanged.AddListener(delegate { OnQualityChanged(qualityDropdown); });
    }

    // This method will be called whenever the dropdown value changes
    void OnQualityChanged(TMP_Dropdown change)
    {
        Debug.Log("Dropdown changed + " + change);
        switch (change.value)
        {
            case 0:
                QualitySettings.SetQualityLevel(5, true); // High
                break;
            case 1:
                QualitySettings.SetQualityLevel(2, true); // Medium
                break;
            case 2:
                QualitySettings.SetQualityLevel(0, true); // Low
                break;
        }
    }
}