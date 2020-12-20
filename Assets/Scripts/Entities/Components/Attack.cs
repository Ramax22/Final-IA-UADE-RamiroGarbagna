using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] float _force;

    public void DeliverDamage(EntityContainer entity)
    {
        entity.LastHittedBy = gameObject;
        entity.GetDamaged(_force);
    }
}