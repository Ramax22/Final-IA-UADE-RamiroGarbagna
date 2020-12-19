using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState<T> : FSMState<T>
{
    EntityContainer _entity;
    Transform _objetive;
    Flock _flocking;
    float _rotationSpeed;
    Vector3 dir;
    float _timerSight;
    Sight _sight;

    public FollowState(EntityContainer entity, List<GameObject> alliedSoldiers, LayerMask unavoidableObstacles, Transform objectiveTarget)
    {
        _entity = entity;
        _sight = entity.Sight;
        _flocking = new Flock(entity.transform.gameObject, alliedSoldiers, unavoidableObstacles, objectiveTarget);
        _objetive = objectiveTarget;

        _rotationSpeed = 4f;
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        //Actualizo la posicion de mi objetivo
        _flocking.SetObjective(_objetive);
        _timerSight = 0.3f;
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        _timerSight -= Time.deltaTime;
        if(_timerSight <= 0)
        {
            _sight.IsInSight();
            _timerSight = 0.3f;
        }

        // Actualizo la posicion de mi objetivo
        _flocking.SetObjective(_objetive);

        // Agarro el vector de movimiento
        dir = _flocking.GetDir();
            
        // hago el alignment y el movimiento solo si no devolvio Vector3.Up
        if (dir != Vector3.up)
        {
            if (dir == Vector3.zero) 
            {
                _entity.MoveEntity(Vector3.zero);
            }
            else
            {
                // ahora hago el alignment
                Quaternion rotation = Quaternion.Slerp(_entity.transform.rotation, Quaternion.LookRotation(dir), _rotationSpeed * Time.deltaTime);
                _entity.RotateTo(rotation);

                //Me muevo en esa direción
                _entity.MoveEntity(dir);
            }
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