using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUI : MonoBehaviour
{
    public TextMeshProUGUI livesText;

    private void Update()
    {
        livesText.text = "RIBBOW (<color=#00FF00>" + PlayerStats.Lives.ToString() + "</color>)"; //alternativ "Remaining Lives: "
    }
}
