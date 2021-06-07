using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VillainDialog : MonoBehaviour
{
    private int dialogShown;

    public int[] waveIndex;

    public VillainDialogPanels[] villiangPanels;

    private WaveSpawner waveSpawned;

    private int villianIndex;

    private void Start()
    {
        waveSpawned = BuildManager.instance.GetComponent<WaveSpawner>();
        waveSpawned.OnWavePriceLocked += WaveStart;
    }
    private void OnDestroy()
    {
        waveSpawned.OnWavePriceLocked -= WaveStart;
    }
    private void WaveStart()
    {
        for (int i = 0; i < waveIndex.Length; i++)
        {
            if (waveSpawned.waveIndex == waveIndex[i])
            {
                villiangPanels[i].gameObject.SetActive(true);
                Time.timeScale = 0;
                villianIndex = i;
                dialogShown = 1;
                break;
            }
        }
    }
    public void CloseWindow()
    {
        if(dialogShown < villiangPanels[villianIndex].dialogPanels.Length)
        {
            villiangPanels[villianIndex].dialogPanels[dialogShown - 1].SetActive(false);
            villiangPanels[villianIndex].dialogPanels[dialogShown].SetActive(true);
            dialogShown++;
        }
        else
        {
            Time.timeScale = waveSpawned.gameSpeed;
            villiangPanels[villianIndex].gameObject.SetActive(false);
        }
    }
}
