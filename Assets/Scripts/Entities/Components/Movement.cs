using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] float _speed;

    private void Awake()
    {
        if (!_rigidbody) Debug.LogError("Not RB applied");
    }

    public void Move(Vector3 dir) { _rigidbody.velocity = dir * _speed; }
    public void Look(Vector3 point) { transform.LookAt(new Vector3(point.x, transform.position.y, point.z)); }
    public void LookAtPoint(Vector3 point) { transform.LookAt(point); }
    public void Rotate(Vector3 eulers) { transform.Rotate(eulers * Time.deltaTime); }
    public void RotateTo(Quaternion rotation) { transform.rotation = rotation; }
}