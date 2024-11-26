using UnityEngine;

public class InteractableDisplay : MonoBehaviour
{
    public GameObject objectModel; // The 3D object to display
    public Camera displayCamera; // Camera for rendering the object
    public Canvas blurCanvas; // UI Canvas or effect for background blur
    public float rotationSpeed = 100f; // Speed of rotation with the mouse

    private bool isDisplaying = false; // Check if the display screen is active
    private Transform originalParent; // Save the object's original parent transform

    public void TriggerDisplay()
    {
        if (!isDisplaying)
        {
            isDisplaying = true;

            // Activate blur and display components
            blurCanvas.gameObject.SetActive(true);
            displayCamera.gameObject.SetActive(true);

            // Detach the object model and make it visible to the display camera
            originalParent = objectModel.transform.parent;
            objectModel.transform.SetParent(displayCamera.transform);
            objectModel.SetActive(true);
        }
    }

    void Update()
    {
        if (isDisplaying)
        {
            HandleRotation();
            CheckExitDisplay();
        }
    }

    private void HandleRotation()
    {
        // Rotate the object with the mouse
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        objectModel.transform.Rotate(Vector3.up, -mouseX, Space.World);
        objectModel.transform.Rotate(Vector3.right, mouseY, Space.World);
    }

    private void CheckExitDisplay()
    {
        // Exit display mode when 'E' is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitDisplay();
        }
    }

    private void ExitDisplay()
    {
        isDisplaying = false;

        // Deactivate blur and display components
        blurCanvas.gameObject.SetActive(false);
        displayCamera.gameObject.SetActive(false);

        // Return the object model to its original parent
        objectModel.transform.SetParent(originalParent);
        objectModel.SetActive(false);
    }
}
