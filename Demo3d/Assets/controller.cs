using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour {
	//public GameObject ground;
	private bool walking = false;
	private Vector3 spawnPoint;

    public LayerMask groundLayers;

    public float speed = 5;

    private Rigidbody rb;

    public GameObject controllerPivot;
    public GameObject messageCanvas;


    public Material cubeInactiveMaterial;
    public Material cubeHoverMaterial;
    public Material cubeActiveMaterial;

    private Renderer controllerCursorRenderer;

    // Currently selected GameObject.
    private GameObject selectedObject;

    // True if we are dragging the currently selected GameObject.
    private bool dragging;


    private void UpdatePointer()
    {
        if (GvrController.State != GvrConnectionState.Connected)
        {
            controllerPivot.SetActive(false);
        }
        controllerPivot.SetActive(true);
        controllerPivot.transform.rotation = GvrController.Orientation;

        if (dragging)
        {
            if (GvrController.TouchUp)
            {
                EndDragging();
            }
        }
        else
        {
            RaycastHit hitInfo;
            Vector3 rayDirection = GvrController.Orientation * Vector3.forward;
            if (Physics.Raycast(Vector3.zero, rayDirection, out hitInfo))
            {
                if (hitInfo.collider && hitInfo.collider.gameObject)
                {
                    SetSelectedObject(hitInfo.collider.gameObject);
                }
            }
            else
            {
                SetSelectedObject(null);
            }
            if (GvrController.TouchDown && selectedObject != null)
            {
                StartDragging();
            }
        }
    }

    private void EndDragging()
    {
        dragging = false;
        selectedObject.GetComponent<Renderer>().material = cubeHoverMaterial;

        // Stop dragging the cube along.
        selectedObject.transform.SetParent(null, true);
    }

    private void SetSelectedObject(GameObject obj)
    {
        if (null != selectedObject)
        {
            selectedObject.GetComponent<Renderer>().material = cubeInactiveMaterial;
        }
        if (null != obj)
        {
            obj.GetComponent<Renderer>().material = cubeHoverMaterial;
        }
        selectedObject = obj;
    }

    private void StartDragging()
    {
        dragging = true;
        selectedObject.GetComponent<Renderer>().material = cubeActiveMaterial;

        // Reparent the active cube so it's part of the ControllerPivot object. That will
        // make it move with the controller.
        selectedObject.transform.SetParent(controllerPivot.transform, true);
    }
    // Use this for initialization
    void Start () {
		spawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		if (walking) {
			transform.position = transform.position + Camera.main.transform.forward * .5f * Time.deltaTime;
		}

		if (transform.position.y < -10f) {
			transform.position = spawnPoint;
		}
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider.name.Contains ("Plane")) {
				walking = false;

			} else {
				walking = true;
			}
		}
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        rb.AddForce(movement * speed);

    }
}
