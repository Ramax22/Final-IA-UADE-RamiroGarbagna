using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    // Variables
    EntityContainer _entity; // La entidad
    float _idleTimer; // El tiempo que esta en idle
    float _sightTimer; // El tiempo que tarda en volver a ejecutar el codigo que revisa la visión del soldado
    Sight _sight; // el componente sight del EnitityContainer

    // Constructor
    public IdleState(EntityContainer entity)
    {
        _entity = entity;
        _sight = entity.Sight;
    }

    // Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        _idleTimer = Random.Range(5, 10); //Doy un tiempo random al idle
        _sightTimer = 0.3f; // Seteo para que cada 0.3 segundos ejecute el codigo de vision
    }

    // Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        // resto de ambos timers
        _sightTimer -= Time.deltaTime;
        _idleTimer -= Time.deltaTime;

        // chequeo la vision del soldado si corresponde
        if (_sightTimer <= 0)
        {
            _sight.IsInSight();
            _sightTimer = 0;
        }

        // si se cumplió el tiempo del timer de idle, pasa a la siguiente acción
        if (_idleTimer <= 0) _entity.ExecuteDesicionTree();
    }

    // Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {
    }
}