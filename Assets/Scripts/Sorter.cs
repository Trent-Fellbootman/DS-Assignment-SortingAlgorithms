using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorter : MonoBehaviour
{
    public float swapSpeed;
    public float ANIM_aspectRatio;

    protected GameObject[] items;

    private SwapAnimationConfig animConfig;
    private bool swapInProgress = false;
    private SwapStatus swapStatus;

    public struct SwapAnimationConfig {
        public float aspectRatio;
    }

    private struct SwapStatus {
        public GameObject A, B;
        public Vector3 originalA, originalB;
        public float progressPercent;
    }

    private void Awake() {
        animConfig = new SwapAnimationConfig();
        animConfig.aspectRatio = ANIM_aspectRatio;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (swapInProgress) {
            swapStatus.progressPercent += swapSpeed * Time.deltaTime;
            updateSwapItems(swapStatus);
            if (swapStatus.progressPercent >= 1) {
                swapInProgress = false;
            }
        }
    }

    protected void swap(int indexA, int indexB) {
        if (!swapInProgress) {
            swapInProgress = true;

            swapStatus = new SwapStatus();
            swapStatus.A = items[indexA];
            swapStatus.B = items[indexB];
            swapStatus.originalA = swapStatus.A.transform.localPosition;
            swapStatus.originalB = swapStatus.B.transform.localPosition;
            swapStatus.progressPercent = 0.0f;
        }
    }

    private void updateSwapItems(SwapStatus status) {
        if (status.progressPercent >= 1) {
            status.progressPercent = 1;
        }

        Vector3 center = (status.originalA + status.originalB) / 2;
        float halfAxisA = (status.originalA - center).magnitude;
        float halfAxisB = animConfig.aspectRatio * halfAxisA;

        float angle = status.progressPercent * Mathf.PI;

        status.A.transform.localPosition = center + new Vector3(halfAxisB * Mathf.Sin(angle), halfAxisA * Mathf.Cos(angle), 0.0f);
        status.B.transform.localPosition = center - new Vector3(halfAxisB * Mathf.Sin(angle), halfAxisA * Mathf.Cos(angle), 0.0f);
    }
}
