using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField]
    private GameObject elementCount;
    [SerializeField]
    private GameObject minSize;
    [SerializeField]
    private GameObject maxSize;
    [SerializeField]
    private GameObject sortingAlgorithm;
    [SerializeField]
    private GameObject createSortingSystem;

    private SortingSystemCreateInfo currentInfo;
    private struct SortingSystemCreateInfo {
        public int elementCount;
        public bool elementCountAvailable;
        public float minSize;
        public bool minSizeAvailable;
        public float maxSize;
        public bool maxSizeAvailable;
        public SortingAlgorithm algorithm;
        public bool algorithmAvailable;

        public enum SortingAlgorithm {
            BUBBLE_SORT, QUICK_SORT
        }
    }

    private void Awake() {
        currentInfo.elementCountAvailable = currentInfo.minSizeAvailable = currentInfo.maxSizeAvailable = currentInfo.algorithmAvailable = false;

        elementCount.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(delegate (string input) {
            int value;
            if (int.TryParse(input, out value) && value > 0) {
                currentInfo.elementCount = value;
                currentInfo.elementCountAvailable = true;
            } else {
                elementCount.GetComponent<TMPro.TMP_InputField>().text = "";
                currentInfo.elementCountAvailable = false;
            }
        });

        minSize.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(delegate (string input) {
            float value;
            if (float.TryParse(input, out value) && value > 0) {
                currentInfo.minSize = value;
                currentInfo.minSizeAvailable = true;
            } else {
                minSize.GetComponent<TMPro.TMP_InputField>().text = "";
                currentInfo.minSizeAvailable = false;
            }
        });

        maxSize.GetComponent<TMPro.TMP_InputField>().onEndEdit.AddListener(delegate (string input) {
            float value;
            if (float.TryParse(input, out value) && value > 0) {
                currentInfo.maxSize = value;
                currentInfo.maxSizeAvailable = true;
            } else {
                maxSize.GetComponent<TMPro.TMP_InputField>().text = "";
                currentInfo.maxSizeAvailable = false;
            }
        });

        sortingAlgorithm.GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(delegate (int input) {
            switch (input) {
            case 0:
                currentInfo.algorithmAvailable = false;
                break;
            case 1:
                currentInfo.algorithm = SortingSystemCreateInfo.SortingAlgorithm.BUBBLE_SORT;
                currentInfo.algorithmAvailable = true;
                break;
            case 2:
                currentInfo.algorithm = SortingSystemCreateInfo.SortingAlgorithm.QUICK_SORT;
                currentInfo.algorithmAvailable = true;
                break;
            }
        });

        createSortingSystem.GetComponent<Button>().onClick.AddListener(delegate () {
            if (currentInfo.elementCountAvailable && currentInfo.minSizeAvailable && currentInfo.maxSizeAvailable && currentInfo.algorithmAvailable && currentInfo.maxSize > currentInfo.minSize) {
                Debug.Log("Creating new sorting system");
            } else {
                Debug.LogError("Invalid configuration");
            }
        });
    }
}
