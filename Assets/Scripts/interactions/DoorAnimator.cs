using UnityEngine;
using System.Collections;

public class DoorAnimator : MonoBehaviour
{
    public enum DoorType
    {
        Slide,
        Swing
    }

    public enum SlideAxis
    {
        X,Y,Z
    }

    public enum SwingAxis
    {
        Y,X,Z   
    }

    [Header("Door Type")]
    public DoorType doorType = DoorType.Slide;

    [Header("Door Meshes")]
    public Transform doorA;         // single doors lang
    public Transform doorB;         // double doors type shi -- add sa susunod na levels 

    [Header("Slide Settings")]
    public SlideAxis slideAxis = SlideAxis.X;
    public float slideDistance = 2f;
    public float slideSpeed = 2f;

    [Header("Swing Settings")]
    public SwingAxis swingAxis = SwingAxis.Y;
    public float swingAngle = 90f;
    public float swingSpeed = 2f;
    public bool swingInward = true;

    [Header("Options")]
    public bool isDoubleDoor = false;
    public bool autoClose = false;
    public float autoCloseDelay = 3f;
    public bool startsOpen = false;

    // State
    private bool isOpen = false;
    private bool isAnimating = false;

    // Stored transforms
    private Vector3 doorAClosedPos;
    private Vector3 doorAOpenPos;
    private Vector3 doorBClosedPos;
    private Vector3 doorBOpenPos;
    private Quaternion doorAClosedRot;
    private Quaternion doorAOpenRot;

    private void Start()
    {
        InitializeDoor();

        if (startsOpen)
            OpenDoor();
    }

    private void InitializeDoor()
    {
        if (doorA == null) return;

        if (doorType == DoorType.Slide)
        {
            doorAClosedPos = doorA.localPosition;
            doorAOpenPos = doorAClosedPos +
                GetSlideVector() * slideDistance;

            if (isDoubleDoor && doorB != null)
            {
                doorBClosedPos = doorB.localPosition;
                doorBOpenPos = doorBClosedPos -
                    GetSlideVector() * slideDistance;
            }
        }
        else 
        {
            doorAClosedRot = doorA.localRotation;
            
            Vector3 axis = GetSwingVector();

            doorAOpenRot = Quaternion.AngleAxis(
                swingInward ? -swingAngle : swingAngle,
                axis) * doorAClosedRot;
        }
    }
    private Vector3 GetSlideVector()
    {
        switch (slideAxis)
        {
            case SlideAxis.X: return Vector3.right;
            case SlideAxis.Y: return Vector3.up;
            case SlideAxis.Z: return Vector3.forward;
            default: return Vector3.right;
        }
    }

    // Gets swing axis vector
    private Vector3 GetSwingVector()
    {
        switch (swingAxis)
        {
            case SwingAxis.Y: return Vector3.up;
            case SwingAxis.X: return Vector3.right;
            case SwingAxis.Z: return Vector3.forward;
            default: return Vector3.up;
        }
    }

    // =========================================
    // PUBLIC CONTROLS
    // =========================================

    public void OpenDoor()
    {
        if (isOpen || isAnimating) return;
        isOpen = true;
        StartCoroutine(AnimateDoor(true));

        if (autoClose)
            StartCoroutine(AutoClose());
    }

    public void CloseDoor()
    {
        if (!isOpen || isAnimating) return;
        isOpen = false;
        StartCoroutine(AnimateDoor(false));
    }

    public void ToggleDoor()
    {
        if (isAnimating) return;
        if (isOpen) CloseDoor();
        else OpenDoor();
    }

    public bool IsOpen() => isOpen;

    // =========================================
    // ANIMATION
    // =========================================

    private IEnumerator AnimateDoor(bool opening)
    {
        isAnimating = true;

        float elapsed = 0f;
        float duration = doorType == DoorType.Slide
            ? 1f / slideSpeed
            : 1f / swingSpeed;

        Vector3 startPosA = doorA != null ?
            doorA.localPosition : Vector3.zero;
        Vector3 startPosB = doorB != null ?
            doorB.localPosition : Vector3.zero;
        Quaternion startRotA = doorA != null ?
            doorA.localRotation : Quaternion.identity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth ease in out
            t = t * t * (3f - 2f * t);

            if (doorType == DoorType.Slide)
            {
                if (doorA != null)
                    doorA.localPosition = Vector3.Lerp(
                        startPosA,
                        opening ? doorAOpenPos : doorAClosedPos,
                        t);

                if (isDoubleDoor && doorB != null)
                    doorB.localPosition = Vector3.Lerp(
                        startPosB,
                        opening ? doorBOpenPos : doorBClosedPos,
                        t);
            }
            else // Swing
            {
                if (doorA != null)
                    doorA.localRotation = Quaternion.Lerp(
                        startRotA,
                        opening ? doorAOpenRot : doorAClosedRot,
                        t);
            }

            yield return null;
        }

        // Snap to final position
        if (doorType == DoorType.Slide)
        {
            if (doorA != null)
                doorA.localPosition =
                    opening ? doorAOpenPos : doorAClosedPos;

            if (isDoubleDoor && doorB != null)
                doorB.localPosition =
                    opening ? doorBOpenPos : doorBClosedPos;
        }
        else
        {
            if (doorA != null)
                doorA.localRotation =
                    opening ? doorAOpenRot : doorAClosedRot;
        }

        isAnimating = false;
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDoor();
    }
}