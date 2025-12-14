using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    // Changed: Store LIST of objects per image type (not single object)
    private Dictionary<string, List<GameObject>> spawnedPrefabs = new Dictionary<string, List<GameObject>>();

    // Changed: Track which object belongs to which tracked image
    private Dictionary<TrackableId, GameObject> trackedImageToObject = new Dictionary<TrackableId, GameObject>();

    private Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    private string[] someArray = new string[]{"Image1", "Image2", "Image3"};

    private void Start()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        // Initialize lists for each prefab type (but don't instantiate yet)
        foreach (GameObject prefab in placeablePrefabs)
        {
            spawnedPrefabs.Add(prefab.name, new List<GameObject>());
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if(trackedImage != null)
        {
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                // Disable object for this specific tracked image
                if (trackedImageToObject.ContainsKey(trackedImage.trackableId))
                {
                    GameObject obj = trackedImageToObject[trackedImage.trackableId];
                    if (obj != null)
                    {
                        obj.transform.SetParent(null);
                        obj.SetActive(false);
                    }
                }
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log(trackedImage.gameObject.name + " is being tracked.");
                
                // Check if we already have an object for this specific tracked image
                if (!trackedImageToObject.ContainsKey(trackedImage.trackableId))
                {
                    // Create NEW object for this tracked image
                    GameObject prefabToInstantiate = null;
                    foreach (GameObject prefab in placeablePrefabs)
                    {
                        if (prefab.name == trackedImage.referenceImage.name)
                        {
                            prefabToInstantiate = prefab;
                            break;
                        }
                    }
                    
                    if (prefabToInstantiate != null)
                    {
                        GameObject newPrefab = Instantiate(prefabToInstantiate);
                        newPrefab.name = prefabToInstantiate.name + "_" + trackedImage.trackableId;
                        newPrefab.SetActive(false);
                        
                        // Add to our tracking dictionaries
                        spawnedPrefabs[trackedImage.referenceImage.name].Add(newPrefab);
                        spawnedObjects.Add(newPrefab, prefabToInstantiate);
                        trackedImageToObject.Add(trackedImage.trackableId, newPrefab);
                        
                        Debug.Log("Created new object: " + newPrefab.name + " for image: " + trackedImage.trackableId);
                    }
                }
                
                // Enable/position the object for this tracked image
                if (trackedImageToObject.ContainsKey(trackedImage.trackableId))
                {
                    GameObject obj = trackedImageToObject[trackedImage.trackableId];
                    if (obj != null && obj.transform.parent != trackedImage.transform)
                    {
                        Debug.Log("Enabling associated content: " + obj.name);
                        obj.transform.SetParent(trackedImage.transform);
                        obj.transform.localPosition = spawnedObjects[obj].transform.localPosition;
                        obj.transform.localRotation = spawnedObjects[obj].transform.localRotation;
                        obj.SetActive(true);
                    }
                }
            }
        }
    }
}