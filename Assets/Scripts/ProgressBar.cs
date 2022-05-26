using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [HideInInspector]
    public float progress = 0;

    private Vector3 originalScale;

    private void Awake() {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(progress * originalScale.x, originalScale.y, originalScale.z);
    }
}
