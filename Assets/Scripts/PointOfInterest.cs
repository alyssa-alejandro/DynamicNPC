using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    [Header("POI Settings")]
    public string poiType = "Chest";
    public bool isExplored = false;

    // Call this when the companion reaches or opens the chest
    public void MarkAsExplored()
    {
        isExplored = true;
    }
}