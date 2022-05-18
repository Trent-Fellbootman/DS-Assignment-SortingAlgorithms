using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sorter : MonoBehaviour
{
    [SerializeField]
    protected float swapSpeed;
    [SerializeField]
    protected float ANIM_aspectRatio;
    [SerializeField]
    protected GameObject itemSample;
    [SerializeField]
    protected int itemCount;
    [SerializeField]
    protected float minSize, maxSize;
    [SerializeField]
    protected Material normal, comparing, greaterThan, lessThan, sorted;
    [SerializeField]
    protected GameObject progressBar;

    protected Item[] items = null;
    protected Command[] commands;

    abstract protected void fillInSwapCommands();

    private SwapAnimationConfig animConfig;
    private bool operationInProgress = false;
    private OperationStatus operationStatus;
    private int currentCommandIndex = 0;
    private bool sortingDone = false;

    protected struct SwapAnimationConfig {
        public float aspectRatio;
    }

    protected struct Command {
        public enum CommandType {
            COMPARE, SWAP
        }

        public CommandType commandType;
        public int indexA, indexB;

        public Command(CommandType commandType, int indexA, int indexB) {
            this.commandType = commandType;
            this.indexA = indexA; this.indexB = indexB;
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
    }

    private void Awake() {
        animConfig = new SwapAnimationConfig();
        animConfig.aspectRatio = ANIM_aspectRatio;
        createItems();
        progressBar.transform.localPosition = new Vector3(0.0f, items[0].obj.transform.localPosition.y - items[0].obj.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, 0.0f);
        fillInSwapCommands();
    }

    // Update is called once per frame
    protected void Update() {
        if (!sortingDone) {
            performOperation();

            // Update progress bar
            GetComponentInChildren<ProgressBar>().progress = (currentCommandIndex - 1 + operationStatus.progressPercent) / commands.Length;
        }
    }

    private void performOperation() {
        if (!operationInProgress) {
            if (currentCommandIndex >= commands.Length) {
                sortingDone = true;
                foreach (Item item in items) {
                    item.obj.GetComponent<MeshRenderer>().material = sorted;
                }
                Debug.Log("Sorting finished");
                return;
            }

            Command currentCommand = commands[currentCommandIndex];
            operationInProgress = true;
            operationStatus.commandType = currentCommand.commandType;

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
                    Item tmp = A;
                    A = B;
                    B = tmp;
                }

                A.obj.GetComponent<MeshRenderer>().material = lessThan;
                B.obj.GetComponent<MeshRenderer>().material = greaterThan;

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

            currentCommandIndex++;
        } else {
            operationStatus.progressPercent += swapSpeed * Time.deltaTime;
            updateSwapItems(operationStatus);

            if (operationStatus.progressPercent >= 1) {
                if (operationStatus.commandType == Command.CommandType.SWAP) {
                    Item tmp = items[operationStatus.indexA];
                    items[operationStatus.indexA] = items[operationStatus.indexB];
                    items[operationStatus.indexB] = tmp;
                }

                items[operationStatus.indexA].obj.GetComponent<MeshRenderer>().material = normal;
                items[operationStatus.indexB].obj.GetComponent<MeshRenderer>().material = normal;

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
            items[i].obj.transform.localPosition = new Vector3(0.0f, (i - (items.Length - 1) / 2.0f) * itemSample.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, 0.0f);
            items[i].obj.GetComponent<MeshRenderer>().material = normal;
        }
    }
}
