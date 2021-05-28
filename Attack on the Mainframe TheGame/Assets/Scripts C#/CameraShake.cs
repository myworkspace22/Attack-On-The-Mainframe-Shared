using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Animator camAnim;
    public Animator livesText;

    public void CamShake()
    {
        camAnim.SetTrigger("Shake");
        livesText.SetTrigger("LosingLives");
    }
}
