using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField]private Transform normalKeyPrefab;
    [SerializeField] private Transform counterTopPoint;
    public void Interact()
    {
        Transform normalKeyTransform = Instantiate(normalKeyPrefab, counterTopPoint);
        normalKeyTransform.localPosition = Vector3.zero;
    }
}
