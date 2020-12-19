using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundState<T> : FSMState<T>
{
    // Vars
    float _timer;
    EntityContainer _entity;
    float _timerSight;
    Sight _sight;

    // constructor
    public LookAroundState(EntityContainer entity)
    {
        _entity = entity;
        _sight = entity.Sight;
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        _timer = Random.Range(1, 5);
        _timerSight = 0.3f;
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        _timer -= Time.deltaTime;
        _timerSight -= Time.deltaTime;

        if (_timerSight <= 0)
        {
            _sight.IsInSight();
            _timerSight = 0.3f;
        }

        if (_timer >= 0)
        {
            _entity.RotateEntity(new Vector3(0f, 100f, 0f));
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