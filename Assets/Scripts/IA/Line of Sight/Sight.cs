using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    //Variables
    [SerializeField] float _range;
    [SerializeField] float _angle;
    [SerializeField] LayerMask _mask;
    [SerializeField] EntityContainer _owner;

    List<GameObject> _enemies;
    List<GameObject> _toInsvestigate = new List<GameObject>(); // Aca se van guardando todos los posibles target
    List<GameObject> _toInsvestigateAux = new List<GameObject>();
    GameObject _target; // este es el target que el codigo eligió para atacar

    #region ~~~ ENCAPSULADO ~~~
    public List<GameObject> Enemies { set { _enemies = value; } }
    #endregion

    #region ~~~ CHEQUEOS ~~~
    //Funcion que chequea si el target esta a la vista
    public void IsInSight()
    {
        //Si la lista de enemigos esta vacía o es nula, que no haga ningún chequeo
        if (_enemies == null || _enemies.Count == 0) return;

        // Vacio las listas
        _toInsvestigate.Clear();
        _toInsvestigateAux.Clear();

        // vacio el GO del target
        _target = null;

        //Chequeo distancia
        if (!CheckDistance()) return;

        //Chequeo angulo
        if (!CheckAngle()) return;

        //Chequeo si hay obstaculos o no
        if (!CheckObstacles()) return;

        // Asignar el target a el entity controller
        _owner.SetTarget(_target);
    }

    // Chequea si hay al menos 1 enemigo en la distancia de vision 
    bool CheckDistance()
    {
        // Reviso la lista de enemigos hasta encontrar 1 que este en la distancia, una vez hecho, dejo de buscar
        foreach (var target in _enemies)
        {
            // Calculo la distancia entre el objetivo y mi soldado
            Vector3 diff = target.transform.position - transform.position;
            float distance = diff.magnitude;

            // si la distancia es menor a mi rango de alcanze, agrego el target en una lista que pasará a mas investigación 
            if (distance < _range) _toInsvestigate.Add(target);
        }

        // aca lo que se hace, es revisar si la lista de investigate quedo algún target, en caso de que haya quedado vacía, significa
        // que no hay ningun enemigo que este en la distancia de vision del soldado
        if (_toInsvestigate.Count == 0) return false;
        else return true;
    }

    // Calcula si alguno de los enemigos esta en angulo de visión
    bool CheckAngle()
    {
        // recorro la lista que arme en la funcion CheckDistance y veo si alguno de sus target esta en el angulo de vision de mi soldado
        foreach (var target in _toInsvestigate)
        {
            // Calculamos el angulo
            Vector3 diff = target.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(transform.forward, diff.normalized);

            // Si este target esta dentro de mi ángulo de vision, lo agrego a una lista secundaria (para que no se pise con la primer lista)
            if (angleToTarget < _angle / 2) _toInsvestigateAux.Add(target);
        }

        // si hay enemigos, paso al siguiente chequeo, sino, corto el chequeo aca
        if (_toInsvestigateAux.Count == 0) return false;
        else return true;
    }

    // Chequea si no hay obstaculos entre el NPC y algun target
    bool CheckObstacles()
    {
        // Recorro todos los enemigos en búsqueda de alguno que 
        foreach (var target in _toInsvestigateAux)
        {
            //Calculamos que no haya un osbtaculo entre la entidad y el objetivo
            Vector3 diff = target.transform.position - transform.position;
            float distance = diff.magnitude;

            //Si no hay nada entre medio, ataco al enemigo que vi
            if (!Physics.Raycast(transform.position, diff.normalized, distance, _mask))
            {
                // asigno al primer target que encontré que haya cumplido con la condicion y salgo
                _target = target;
                return true; 
            }
        }

        // Si llega hasta aca, significa que todos los enemigos estan bloqueados por algun obstaculo
        return false;
    }

    public bool CheckSpecificUnitWithObstacles(GameObject target)
    {
        //Calculamos que no haya un osbtaculo entre la entidad y el objetivo
        Vector3 diff = target.transform.position - transform.position;
        float distance = diff.magnitude;

        //Si hay un obstaculo, retorno True, en caso contrario retorno False
        return Physics.Raycast(transform.position, diff.normalized, distance, _mask);
    }
    #endregion
}