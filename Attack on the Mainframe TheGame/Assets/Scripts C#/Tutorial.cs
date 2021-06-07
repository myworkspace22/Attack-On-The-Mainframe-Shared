using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int[] waveIndex;

    private WaveSpawner waveSpawned;

    public GameObject[] panels;

    private void Start()
    {
        waveSpawned = BuildManager.instance.GetComponent<WaveSpawner>();
        waveSpawned.OnWaveEnded += WaveEnd;
        //panels[0].SetActive(true);
    }
    private void OnDestroy()
    {
        waveSpawned.OnWaveEnded -= WaveEnd;
    }

    private void WaveEnd()
    {
        for (int i = 0; i < waveIndex.Length; i++)
        {
            if (waveSpawned.waveIndex + 1 == waveIndex[i])
            {
                panels[i].SetActive(true);
                waveSpawned.isPaused = true;
                break;
            }
        }
    }
    public void CloseWindow()
    {
        waveSpawned.isPaused = false;
    }
}
