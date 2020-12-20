using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState<T> : FSMState<T>
{
    EntityContainer _entity;
    GameObject _targetGO;
    EntityContainer _target;
    Attack _attack;
    float _attackCooldown;

    public AttackState(EntityContainer entity, Attack attack)
    {
        _entity = entity;
        _attack = attack;
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        _targetGO = _entity.Target;
        _target = _targetGO.GetComponent<EntityContainer>();
        _attackCooldown = .5f;
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        // si el target no exite mas, o es nulo, ejecutamos el arbol
        if (_target == null)
        {
            _entity.ExecuteDesicionTree();
            return;
        }

        _attackCooldown -= Time.deltaTime;

        //Calculo la distancia
        Vector3 diff = _target.transform.position - _entity.transform.position;
        float distance = diff.magnitude;

        if (distance <= 1.5f && _attackCooldown <= 0f)
        {
            _entity.LookAtPoint(_target.transform.position);
            _entity.MoveEntity(Vector3.zero);
            _attack.DeliverDamage(_target);
            _attackCooldown = .5f;
        }
        else
        {
            _entity.ExecuteDesicionTree();
        }
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {

    }
}