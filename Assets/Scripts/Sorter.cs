using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sorter : MonoBehaviour {
    [SerializeField]
    public float swapSpeed;
    [SerializeField]
    protected float ANIM_aspectRatio;
    [SerializeField]
    protected GameObject itemSample;
    [SerializeField]
    public int itemCount;
    [SerializeField]
    public float minSize, maxSize;
    [SerializeField]
    protected Material normal, comparing, greaterThan, lessThan, sorted, pivot;
    [SerializeField]
    protected GameObject progressBar;
    [SerializeField]
    public GameObject title;
    [SerializeField]
    protected Vector3 playerOffset;

    protected Item[] items = null;
    protected Command[] commands;

    abstract protected void fillInSwapCommands();

    private SwapAnimationConfig animConfig;
    private bool operationInProgress = false;
    private OperationStatus operationStatus;
    private int currentCommandIndex = 0;
    [SerializeField]
    public bool sortingDone = false;

    [HideInInspector]
    public GameObject player = null;

    protected struct SwapAnimationConfig {
        public float aspectRatio;
    }

    protected struct Command {
        public enum CommandType {
            COMPARE, SWAP
        }

        public CommandType commandType;
        public int indexA, indexB;
        public int playerFocus;
        public int pivot;

        public Command(CommandType commandType, int indexA, int indexB, int playerFocus, int pivot = -1) {
            this.commandType = commandType;
            this.indexA = indexA; this.indexB = indexB;
            this.playerFocus = playerFocus;
            this.pivot = pivot;
        }
    }

    protected struct Item {
        public GameObject obj;
        public float key;
    }

    private struct OperationStatus {
        public Command.CommandType commandType;
        public int indexA, indexB;
        public Item A, B;
        public Vector3 originalA, originalB;
        public float progressPercent;
        public int playerFocus;
        public int pivot;
    }

    private void Start() {
        animConfig = new SwapAnimationConfig();
        animConfig.aspectRatio = ANIM_aspectRatio;
        createItems();
        progressBar.transform.localPosition = new Vector3(progressBar.transform.localPosition.x, progressBar.transform.localPosition.y + items[0].obj.transform.localPosition.y - items[0].obj.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, progressBar.transform.localPosition.z);
        title.transform.localPosition = new Vector3(title.transform.localPosition.x, title.transform.localPosition.y + items[items.Length - 1].obj.transform.localPosition.y + items[items.Length - 1].obj.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, title.transform.localPosition.z);
        fillInSwapCommands();
    }

    // Update is called once per frame
    protected void Update() {
        if (!sortingDone) {
            performOperation();

            // Update progress bar
            GetComponentInChildren<ProgressBar>().progress = (currentCommandIndex - 1 + operationStatus.progressPercent) / commands.Length;

            // Update player
            if (player != null && currentCommandIndex < commands.Length) {
                float tmpY = (1 - operationStatus.progressPercent) * items[operationStatus.playerFocus].obj.transform.localPosition.y + operationStatus.progressPercent * items[commands[currentCommandIndex].playerFocus].obj.transform.localPosition.y;
                player.transform.parent = transform;
                player.transform.localPosition = new Vector3(0.0f, tmpY, 0.0f) + playerOffset;
            }
        }
    }

    private void performOperation() {
        if (commands == null) {
            Debug.LogError("Commands are null");
        } else if (!operationInProgress) {
            if (currentCommandIndex >= commands.Length) {
                sortingDone = true;
                foreach (Item item in items) {
                    item.obj.GetComponent<MeshRenderer>().material = sorted;
                }
                if (player) {
                    player.transform.parent = null;
                    player.GetComponent<PlayerController>().controlled = false;
                    player = null;
                }
                Debug.Log("Sorting finished");
                return;
            }

            Command currentCommand = commands[currentCommandIndex];
            operationInProgress = true;
            operationStatus.commandType = currentCommand.commandType;
            operationStatus.playerFocus = currentCommand.playerFocus;
            operationStatus.pivot = currentCommand.pivot;

            switch (currentCommand.commandType) {
            case Command.CommandType.SWAP:
                operationStatus.indexA = currentCommand.indexA;
                operationStatus.indexB = currentCommand.indexB;
                operationStatus.A = items[currentCommand.indexA];
                operationStatus.B = items[currentCommand.indexB];
                operationStatus.originalA = operationStatus.A.obj.transform.localPosition;
                operationStatus.originalB = operationStatus.B.obj.transform.localPosition;
                operationStatus.progressPercent = 0.0f;

                Item A = items[currentCommand.indexA], B = items[currentCommand.indexB];
                if (A.key > B.key) {
                    Item tmp_item = A;
                    A = B;
                    B = tmp_item;
                }

                A.obj.GetComponent<MeshRenderer>().material = lessThan;
                B.obj.GetComponent<MeshRenderer>().material = greaterThan;

                Item tmp = items[operationStatus.indexA];
                items[operationStatus.indexA] = items[operationStatus.indexB];
                items[operationStatus.indexB] = tmp;

                break;

            case Command.CommandType.COMPARE:
                operationStatus.indexA = currentCommand.indexA;
                operationStatus.indexB = currentCommand.indexB;
                operationStatus.A = items[currentCommand.indexA];
                operationStatus.B = items[currentCommand.indexB];
                operationStatus.progressPercent = 0.0f;

                items[operationStatus.indexA].obj.GetComponent<MeshRenderer>().material = comparing;
                items[operationStatus.indexB].obj.GetComponent<MeshRenderer>().material = comparing;

                break;
            }

            if (currentCommand.pivot != -1) {
                items[currentCommand.pivot].obj.GetComponent<MeshRenderer>().material = pivot;
            }

            currentCommandIndex++;
        } else {
            operationStatus.progressPercent += swapSpeed * Time.deltaTime;
            updateSwapItems(operationStatus);

            if (operationStatus.progressPercent >= 1) {
                items[operationStatus.indexA].obj.GetComponent<MeshRenderer>().material = normal;
                items[operationStatus.indexB].obj.GetComponent<MeshRenderer>().material = normal;

                if (operationStatus.pivot != -1 && currentCommandIndex < commands.Length && operationStatus.pivot != commands[currentCommandIndex].pivot) {
                    items[operationStatus.pivot].obj.GetComponent<MeshRenderer>().material = normal;
                }

                operationInProgress = false;
            }
        }
    }

    private void updateSwapItems(OperationStatus status) {
        if (status.progressPercent >= 1) {
            status.progressPercent = 1;
        }

        switch (status.commandType) {
        case Command.CommandType.SWAP:
            Vector3 center = (status.originalA + status.originalB) / 2;
            Vector3 halfAxisA = status.originalA - center;
            Vector3 halfAxisB = Vector3.Cross(new Vector3(0, 0, 1), halfAxisA) * animConfig.aspectRatio;

            float angle = status.progressPercent * Mathf.PI;

            status.A.obj.transform.localPosition = center + halfAxisB * Mathf.Sin(angle) + halfAxisA * Mathf.Cos(angle);
            status.B.obj.transform.localPosition = center - halfAxisB * Mathf.Sin(angle) - halfAxisA * Mathf.Cos(angle);

            // if (status.progressPercent == 1) {
            //     status.A.obj.transform.localPosition = status.originalB;
            //     status.B.obj.transform.localPosition = status.originalA;
            // }

            break;

        case Command.CommandType.COMPARE:
            break;
        }
    }

    private void createItems() {
        items = new Item[itemCount];
        for (int i = 0; i < items.Length; i++) {
            items[i].key = Random.Range(minSize, maxSize);

            items[i].obj = Instantiate(itemSample);
            items[i].obj.transform.parent = transform;
            items[i].obj.transform.localScale = new Vector3(items[i].key, 1.0f, 1.0f);
            items[i].obj.transform.localEulerAngles = Vector3.zero;
            items[i].obj.transform.localPosition = new Vector3(0.0f, (i - (items.Length - 1) / 2.0f) * itemSample.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, 0.0f);
            items[i].obj.GetComponent<MeshRenderer>().material = normal;
        }
    }
}
