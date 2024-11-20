using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basic_interaction : MonoBehaviour
{


    public Camera playerCamera;
    public GameObject eIndicatorPrefab; // Prefab for the "E" indicator
    public float interactionRange = 3f; // Distance at which objects are considered nearby
    private GameObject eIndicatorInstance; // Active instance of the "E" indicator
    private Transform currentInteractable; // The interactable object in focus



 
    void Update()
    {
        
        DetectNearbyObjects();
        HandleInteraction();
    }

    

    void DetectNearbyObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        bool foundInteractable = false; // Track if an interactable was found

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Interactable"))
            {
                ShowEIndicator(collider.transform);
                foundInteractable = true;
                break; // Stop checking further
            }
        }

        if (!foundInteractable) // No interactable objects nearby
        {
            HideEIndicator();
        }
    }


    void HandleInteraction()
    {
        if (currentInteractable == null || eIndicatorInstance == null) return;

        // Check if the player is pointing at the object
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // Center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform == currentInteractable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithObject(currentInteractable.gameObject);
            }
        }
    }

    void ShowEIndicator(Transform target)
    {


        if (eIndicatorInstance == null)
        {
            eIndicatorInstance = Instantiate(eIndicatorPrefab); // Create the indicator instance
        }

        // Ensure the indicator is active and position it correctly
        eIndicatorInstance.SetActive(true);
        eIndicatorInstance.transform.position = target.position + Vector3.up * 0.1f; // Position above the object
        eIndicatorInstance.transform.LookAt(playerCamera.transform); // Ensure it faces the player


        // Flip the object by rotating it 180 degrees on the y-axis
        eIndicatorInstance.transform.Rotate(0f, 180f, 0f); // Flip the object 180 degrees on the y-axis

        currentInteractable = target; // Set the current interactable

    }

    void HideEIndicator()
    {
        if (eIndicatorInstance != null)
        {
            eIndicatorInstance.SetActive(false); // Hide the indicator completely
        }

        currentInteractable = null; // Clear the current interactable reference
    }


    void InteractWithObject(GameObject interactableObject)
    {
        Debug.Log($"Interacted with: {interactableObject.name}");
        // Add specific interaction logic here
    }
}
