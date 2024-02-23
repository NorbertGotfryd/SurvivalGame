using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class IntereactionManager : MonoBehaviour
{
    public LayerMask layerMask;
    public TextMeshProUGUI promptText;
    public float checkRate = 0.05f; //for performance
    public float maxCheckDistance;

    private IInteractable currentIntereactable;
    private GameObject currentIntereactGameObject;
    private Camera cam;
    private float lastCheckTime;

    private void Start()
    {
        cam = Camera.main; //we store Camera.main in variable because of performance
    }

    private void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                //if our object is NOT current select object then make it current select object
                if (hit.collider.gameObject != currentIntereactGameObject)
                {
                    currentIntereactGameObject = hit.collider.gameObject;
                    currentIntereactable = hit.collider.GetComponent<IInteractable>();
                    SetPrompText();
                }
            }
            else
            {
                currentIntereactable = null;
                currentIntereactGameObject = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    void SetPrompText()
    {
        promptText.gameObject.SetActive(transform);
        promptText.text = string.Format("<b>[E]</b> {0}", currentIntereactable.GetInteractPrompt());
    }

    public void OnIntereactInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && currentIntereactable != null)
        {
            currentIntereactable.OnIntereact();
            currentIntereactable = null;
            currentIntereactGameObject = null;
            promptText.gameObject.SetActive(false);
        }
    }
}

public interface IInteractable
{
    string GetInteractPrompt();
    void OnIntereact();
}
