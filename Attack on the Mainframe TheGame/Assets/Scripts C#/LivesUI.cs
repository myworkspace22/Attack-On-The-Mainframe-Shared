using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    public PlayerStats playerStats;

    private void Update()
    {
        livesText.text = "RIBBOW (" + playerStats.Lives.ToString() + ")"; //alternativ "Remaining Lives: "
    }
}
