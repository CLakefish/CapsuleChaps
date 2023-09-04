using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] internal Transform cameraPivot;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform controller, viewModel;

    [Header("Collision Parameters")]
    [SerializeField] private LayerMask layers;

    [Header("Movement Parameters")]
    [SerializeField] private float gravity;
    [SerializeField] private float sensitivity;
    [SerializeField] private float smoothingTime;
    [SerializeField] private float moveSmoothingTime;
    [SerializeField] private float cameraDistance;
    [SerializeField] private Vector3 pivotOffset;
    [SerializeField] private Vector2 rotationLimits;
    private Vector3 pivotVelocity;
    private Vector3 camVelocity;
    private float fieldOfViewVelocity;

    private float targetYRotation, currentYRotation, currentYRotationVelocity, yAngleOffset;
    private float targetXRotation, currentXRotation, currentXRotationVelocity;
    private float targetControllerYRotation, currentControllerYRotation, currentControllerYRotationVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        View();

        cameraPivot.position = Vector3.SmoothDamp(cameraPivot.position, pivotOffset + controller.transform.position, ref camVelocity, 0.069f);
    }

    private void View()
    {
        Vector2 inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (inputs.x * inputs.x + inputs.y * inputs.y > 0)
        {
            targetControllerYRotation = Mathf.Atan2(inputs.x, inputs.y) * Mathf.Rad2Deg;
            targetControllerYRotation -= yAngleOffset;

            currentControllerYRotation = Mathf.SmoothDampAngle(currentControllerYRotation, targetControllerYRotation, ref currentControllerYRotationVelocity, moveSmoothingTime);
        }
        controller.transform.eulerAngles = new Vector3(0f, targetControllerYRotation, 0);
    }

    private void FixedUpdate()
    {
        viewModel.transform.eulerAngles = new Vector3(viewModel.transform.eulerAngles.x, currentControllerYRotation, viewModel.transform.eulerAngles.z);
    }

    private void MoveCamera()
    {
        Vector2 inputs = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        targetYRotation += inputs.x * sensitivity;
        targetXRotation -= inputs.y * sensitivity;
        targetXRotation = Mathf.Clamp(targetXRotation, rotationLimits.x, rotationLimits.y);

        currentXRotation = Mathf.SmoothDampAngle(currentXRotation, targetXRotation, ref currentXRotationVelocity, smoothingTime);
        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, targetYRotation, ref currentYRotationVelocity, smoothingTime);
        cameraPivot.eulerAngles = new Vector3(currentXRotation, currentYRotation, 0f);

        Ray cameraCollision = new Ray(cameraPivot.position, -cameraPivot.forward);
        float maxDist = cameraDistance;

        if (Physics.SphereCast(cameraCollision, 0.25f, out RaycastHit obj, cameraDistance, layers))
        {
            maxDist = (obj.point - cameraPivot.position).magnitude - 0.25f;
        }

        cameraTransform.localPosition = Vector3.forward * -(maxDist - 0.1f);
        yAngleOffset = Mathf.Atan2(cameraPivot.forward.z, cameraPivot.forward.x) * Mathf.Rad2Deg - 90f;
    }
}
