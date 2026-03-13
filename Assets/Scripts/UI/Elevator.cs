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
    public bool isEntranceElevator = false;  // True = opens on spawn 
    public float entranceOpenDelay = 2f;     // Dramatic delay 

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

        // Entrance elevator opens automatically after delay
        // Exit elevator stays closed until keycard used
        if (isEntranceElevator)
            StartCoroutine(EntranceOpen());
        else
            CloseDoors(); // Exit elevator starts closed 
    }

    // Dramatic entrance open after delay
    private IEnumerator EntranceOpen()
    {
        // Doors closed at first
        CloseDoors();

        // Wait for dramatic effect
        yield return new WaitForSeconds(entranceOpenDelay);

        // Then open
        OpenElevator();

        Debug.Log("Entrance elevator opened!");
    }

    // Closes doors instantly
    private void CloseDoors()
    {
        if (leftDoor != null)
            leftDoor.localPosition = leftDoorClosedPos;

        if (rightDoor != null)
            rightDoor.localPosition = rightDoorClosedPos;

        isOpen = false;
    }

    // Called by ElevatorPanel when keycard accepted
    public void OpenElevator()
    {
        if (isOpen) return;
        isOpen = true;
        StartCoroutine(OpenDoors());
    }

    // Smoothly slides doors open
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

    // Triggered when player walks into elevator
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isOpen) return;

        // Only exit elevator triggers level complete
        if (isEntranceElevator) return;

        Debug.Log("Player entered exit elevator!");
        StartCoroutine(TriggerLevelComplete());
    }

    // Waits then triggers level complete
    private IEnumerator TriggerLevelComplete()
    {
        yield return new WaitForSeconds(levelCompleteDelay);

        if (LevelCompleteUI.Instance != null)
            LevelCompleteUI.Instance.ShowLevelComplete(currentLevelData);
        else
            Debug.LogWarning("LevelCompleteUI not found!");
    }
}