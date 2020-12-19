using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] bool _isBlueZone;

    private void OnTriggerEnter(Collider other)
    {
        EntityContainer container = other.GetComponent<EntityContainer>();

        if (container) { container.ChangeRouletteValues(_isBlueZone); }
    }
}