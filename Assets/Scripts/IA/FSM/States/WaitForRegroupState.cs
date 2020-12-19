using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForRegroupState<T> : FSMState<T>
{
    EntityContainer _entity;
    int _aliveSoldiers;
    float _timer;
    float _radius = 8f;
    LayerMask _alliedLayerMask;

    public WaitForRegroupState(EntityContainer entity, LayerMask soldierLayermask)
    {
        _entity = entity;
        _alliedLayerMask = soldierLayermask;
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        //_aliveSoldiers = _entity.AliveSoldiers;
        _timer = 10f;
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0) Debug.Log("Time's out, go to next state");

        /*Collider[] hitColliders = Physics.OverlapSphere(_entity.transform.position, _radius, _alliedLayerMask);

        if (hitColliders.Length >= _aliveSoldiers) Debug.Log("Go to next state");

        _aliveSoldiers = _entity.AliveSoldiers;*/
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {
        Debug.Log("Cambio");
    }
}