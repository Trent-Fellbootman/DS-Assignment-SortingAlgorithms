using System.Net.Http.Headers;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
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
    [SerializeField]
    private GameObject[] prototypes;
    [SerializeField]
    private float playerInstantiationOffset;

    private GameObject player;

    private bool menuActive;

    private SortingSystemCreateInfo currentInfo;
    private struct SortingSystemCreateInfo {
        public int elementCount;
        public bool elementCountAvailable;
        public float minSize;
        public bool minSizeAvailable;
        public float maxSize;
        public bool maxSizeAvailable;
        public int algorithm;
        public bool algorithmAvailable;
    }

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");

        menuActive = gameObject.activeSelf;

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
            if (input > 0 && input <= prototypes.Length) {
                currentInfo.algorithmAvailable = true;
                currentInfo.algorithm = input;
            } else {
                currentInfo.algorithmAvailable = false;
            }
        });

        createSortingSystem.GetComponent<Button>().onClick.AddListener(delegate () {
            if (currentInfo.elementCountAvailable && currentInfo.minSizeAvailable && currentInfo.maxSizeAvailable && currentInfo.algorithmAvailable && currentInfo.maxSize > currentInfo.minSize) {

                GameObject newSorter = Instantiate(prototypes[currentInfo.algorithm - 1]);
                Sorter sorterComponent = newSorter.GetComponent<Sorter>();
                sorterComponent.itemCount = currentInfo.elementCount;
                sorterComponent.minSize = currentInfo.minSize;
                sorterComponent.maxSize = currentInfo.maxSize;

                newSorter.transform.position = player.transform.position + playerInstantiationOffset * player.transform.forward;
                newSorter.transform.eulerAngles = Vector3.zero;
                newSorter.transform.Rotate(0, player.transform.rotation.eulerAngles.y, 0, Space.World);
            }
        });
    }
}
