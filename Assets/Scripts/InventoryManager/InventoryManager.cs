using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private List<string> collectedFragments = new List<string>();

    public void AddFragment(string clue)
    {
        if (!collectedFragments.Contains(clue))
        {
            collectedFragments.Add(clue);

            // Update hotbar 
            UIManager.Instance.UpdateFragmentHotbar();
        }
    }

    public List<string> GetFragments()
    {
        return collectedFragments;
    }

    public int GetFragmentCount()
    {
        return collectedFragments.Count;
    }
}
