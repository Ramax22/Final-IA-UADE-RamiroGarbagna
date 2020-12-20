using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AproachState<T> : FSMState<T>
{
    // variables
    EntityContainer _entity;
    GameObject _target;
    float _minDistance;
    Sight _sight;
    Pursit pursit;
    Avoid _avoid;

    public AproachState(EntityContainer entity, Sight sight, LayerMask avoidableObstacles)
    {
        _entity = entity;
        _sight = sight;
        _minDistance = 1.5f;
        _avoid = new Avoid(entity.transform, 1, 1, avoidableObstacles);
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        _target = _entity.Target;
        _avoid.SetTarget(_target.transform);
        pursit = new Pursit(_entity.transform, _target.transform, _target.GetComponent<Rigidbody>(), 0.5f);
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        // si no llego a tener target, ejecuto el desicion tree
        if (_target == null)
        {
            _entity.ExecuteDesicionTree();
            return;
        }

        //Calculo la distancia
        Vector3 diff = _target.transform.position - _entity.transform.position;
        float distance = diff.magnitude;

        if (distance <= _minDistance)
        {
            //significa que puedo atacar, por lo que cambio de estado
            _entity.MoveEntity(Vector3.zero);
            _entity.ExecuteDesicionTree();
        }
        else
        {
            //Si llego a perder la linea de vision directa con mi objetivo, hago otra accion
            if (_sight.CheckSpecificUnitWithObstacles(_target))
            {
                _entity.MoveEntity(Vector3.zero);
                _entity.ExecuteDesicionTree();
                return;
            }

            //En caso de no estar en la distancia correcta, me muevo hacia el enemigo
            Vector3 dir;

            if (distance < 4)
            {
                dir = _avoid.GetDir();
            }
            else
            {
                dir = pursit.GetDir();
            }
 
            _entity.MoveEntity(dir);
            _entity.LookAtPoint(_target.transform.position);
        }
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {
    }
}