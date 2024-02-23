using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipBuildingKit : Equip
{
    public GameObject buildingWindow;
    public LayerMask placementLayerMask;
    public Vector3 placementPosition;
    public static EquipBuildingKit instance;
    public float placementUpdateRate = 0.03f;
    public float placementMaxDistance = 5.0f;
    public float rotateSpeed = 180.0f;

    private BuildingRecipe currentBuildingRecipe;
    private BuildingPreview currentBuildingPreview;
    private Camera cam;
    private float lastPlacementUpdateTime;
    private bool canPlace;
    private float currentYRotation;

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    private void Start()
    {
        buildingWindow = FindObjectOfType<BuildingWindow>(true).gameObject;
    }

    private void Update()
    {
        if (currentBuildingRecipe != null && currentBuildingPreview != null && Time.time - lastPlacementUpdateTime > placementUpdateRate)
        {
            lastPlacementUpdateTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, placementMaxDistance, placementLayerMask))
            {
                currentBuildingPreview.transform.position = hit.point;
                currentBuildingPreview.transform.up = hit.normal;
                currentBuildingPreview.transform.Rotate(new Vector3(0, currentYRotation, 0), Space.Self);

                if (!currentBuildingPreview.CollidingWithObjects())
                {
                    if (!canPlace)
                        currentBuildingPreview.CanPlace();

                    canPlace = true;
                }
                else
                {
                    if (!canPlace)
                        currentBuildingPreview.CannotPlace();

                    canPlace = false;
                }
            }
        }

        if (Keyboard.current.rKey.isPressed)
        {
            currentYRotation += rotateSpeed * Time.deltaTime;

            if (currentYRotation > 360)
                currentYRotation = 0.0f;
        }
    }

    private void OnDestroy()
    {
        if (currentBuildingPreview != null)
            Destroy(currentBuildingPreview.gameObject);
    }

    public override void OnAttackInput()
    {
        if (currentBuildingRecipe == null || currentBuildingPreview == null || !canPlace)
            return;

        Instantiate(currentBuildingRecipe.spawnPrefab, currentBuildingPreview.transform.position, currentBuildingPreview.transform.rotation);

        for (int x = 0; x < currentBuildingRecipe.cost.Length; x++)
        {
            for (int y = 0; y < currentBuildingRecipe.cost[x].quantity; y++)
            {
                Inventory.instance.RemoveItem(currentBuildingRecipe.cost[x].item);
            }
        }

        currentBuildingRecipe = null;
        Destroy(currentBuildingPreview.gameObject);
        currentBuildingPreview = null;
        canPlace = false;
        currentYRotation = 0;
    }

    public override void OnAltAttackInput()
    {
        if (currentBuildingPreview != null)
            Destroy(currentBuildingPreview.gameObject);

        buildingWindow.SetActive(true);
        PlayerController.instance.ToggleCursor(true);
    }

    public void SetNewBuildingRecipe(BuildingRecipe recipe)
    {
        currentBuildingRecipe = recipe;
        buildingWindow.SetActive(false);
        PlayerController.instance.ToggleCursor(false);

        currentBuildingPreview = Instantiate(recipe.previewPrefab).GetComponent<BuildingPreview>();
    }
}
