using CnControls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractButtonPressed;
    public static GameInput Instance { get; private set; }

    private const string ForwardAxis = "Horizontal";
    private const string StrafeAxis = "Vertical";
    private const string PlayerRotationXAxis = "Camera X";
    private const string PlayerRotationYAxis = "Camera Y";
    private const string InteractButton = "Interact";
    private Vector2 rotation = Vector2.zero;
    private void Awake()
    {
        Instance = this;
        
    }

    private void Update()
    {
            if (CnInputManager.GetButtonDown(InteractButton))
            {
                OnInteractButtonPressed?.Invoke(this, EventArgs.Empty);
            };
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = new Vector2(CnInputManager.GetAxis(ForwardAxis), CnInputManager.GetAxis(StrafeAxis));
        return inputVector.normalized;
    }

    public Vector2 GetRotationVector()
    {
        rotation.x += CnInputManager.GetAxis(PlayerRotationXAxis);
        rotation.y += CnInputManager.GetAxis(PlayerRotationYAxis);
        return rotation;
    }

}
