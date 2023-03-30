using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private LayerMask countersLayerMask;
    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    [Range(0f, 90f)][SerializeField] private float yRotationLimit = 70f;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }

    private Vector2 rotation = Vector2.zero;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractButtonPressed += GameInput_OnInteractButtonPressed;
    }

    private void GameInput_OnInteractButtonPressed(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
        
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerSizeRadius = .2f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDir, moveDistance);
        if (!canMove)
        {
            //Cannot move in this direction but attemp x movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);

            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            } else
            {
                //Cannot move in X direction, attempt Z movement
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);

                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                } else
                {
                    //Cannnot move in any direction 
                }

            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }


        rotation = GameInput.Instance.GetRotationVector() * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        transform.localRotation = xQuat;
        playerCamera.transform.localRotation = yQuat;

    }
    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;

        float interactDistance = 2f;
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit rayCastHit, interactDistance, countersLayerMask)){
            if (rayCastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            { 
                //Has counter
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            } else
            {
                SetSelectedCounter(null);
            }
        } else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter,
        });
    }
}

