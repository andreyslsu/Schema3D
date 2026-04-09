using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Elevator Doors")]
    public Transform leftDoor;
    public Transform rightDoor;
    public float doorOpenDistance = 2f;
    public float doorOpenSpeed = 2f;

    [Header("Level Complete")]
    public LevelData currentLevelData;
    public float levelCompleteDelay = 0.5f;

    [Header("Entrance Settings")]
    public bool isEntranceElevator = false; 
    public float entranceOpenDelay = 2f;     

    // Door positions
    private Vector3 leftDoorClosedPos;
    private Vector3 rightDoorClosedPos;
    private Vector3 leftDoorOpenPos;
    private Vector3 rightDoorOpenPos;

    private bool isOpen = false;

    private void Start()
    {
        // Store door positions
        if (leftDoor != null)
        {
            leftDoorClosedPos = leftDoor.localPosition;
            leftDoorOpenPos = leftDoorClosedPos + Vector3.left * doorOpenDistance;
        }

        if (rightDoor != null)
        {
            rightDoorClosedPos = rightDoor.localPosition;
            rightDoorOpenPos = rightDoorClosedPos + Vector3.right * doorOpenDistance;
        }

        if (isEntranceElevator)
            StartCoroutine(EntranceOpen());
        else
            CloseDoors(); 
    }

    private IEnumerator EntranceOpen()
    {
        CloseDoors();

        yield return new WaitForSeconds(entranceOpenDelay);

        OpenElevator();

        Debug.Log("Entrance elevator opened!");
    }

    private void CloseDoors()
    {
        if (leftDoor != null)
            leftDoor.localPosition = leftDoorClosedPos;

        if (rightDoor != null)
            rightDoor.localPosition = rightDoorClosedPos;

        isOpen = false;
    }

    public void OpenElevator()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(OpenDoors());
    }

    private IEnumerator OpenDoors()
    {
        float elapsed = 0f;
        float duration = 1f / doorOpenSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (leftDoor != null)
                leftDoor.localPosition = Vector3.Lerp(
                    leftDoorClosedPos, leftDoorOpenPos, t);

            if (rightDoor != null)
                rightDoor.localPosition = Vector3.Lerp(
                    rightDoorClosedPos, rightDoorOpenPos, t);

            yield return null;
        }

        Debug.Log("Doors fully open!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isOpen) return;

        
        if (isEntranceElevator) return;

        Debug.Log("Player entered exit elevator!");
        StartCoroutine(TriggerLevelComplete());
    }

    private IEnumerator TriggerLevelComplete()
    {
        yield return new WaitForSeconds(levelCompleteDelay);

        if (LevelCompleteUI.Instance != null)
            LevelCompleteUI.Instance.ShowLevelComplete(currentLevelData);
        else
            Debug.LogWarning("LevelCompleteUI not found!");
    }
}