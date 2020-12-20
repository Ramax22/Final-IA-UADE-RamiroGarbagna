using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : ISteeringBehaviour
{
    GameObject _entityGO;
    List<GameObject> _aliveSoldiers; // Todos mis compañeros vivos
    LayerMask _unavoidableObstacles;
    Transform _objective;


    float partnerDistance;

    public Flock(GameObject entityGO, List<GameObject> aliveSoldiers, LayerMask unavoidableObstacles, Transform objective)
    {
        _entityGO = entityGO;
        _aliveSoldiers = aliveSoldiers;
        _unavoidableObstacles = unavoidableObstacles;
        _objective = objective;

        partnerDistance = 5f;
    }

    public void SetObjective(Transform newObjective) { _objective = newObjective; }

    public Vector3 GetDir()
    {
        // NEW VERSION

        /*Vector3 vAlignment = new Vector3();
        Vector3 vCohesion = new Vector3();
        Vector3 vSeparation = new Vector3();
        int neighborCount = 0;

        foreach (var partner in _aliveSoldiers)
        {
            if (partner != _entityGO)
            {
                if (Vector3.Distance(_entityGO.transform.position, partner.transform.position) < partnerDistance)
                {
                    // alignment

                    // separation
                    vSeparation.x += partner.transform.position.x - _entityGO.transform.position.x;
                    vSeparation.z += partner.transform.position.z - _entityGO.transform.position.z;

                    neighborCount++;
                }
            }
        }

        if (neighborCount != 0)
        {
            // alignment

            // separation
            vSeparation.x /= neighborCount;
            vSeparation.z /= neighborCount;

            vSeparation.x *= -1;
            vSeparation.z *= -1;
        }

        Vector3 dir = new Vector3();
        dir.x += _objective.position.normalized.x + _objective.forward.normalized.x + vSeparation.normalized.x;
        dir.z += _objective.position.normalized.z + _objective.forward.normalized.z + vSeparation.normalized.z;
        return dir;*/





        // OLD VERSION
        
        //Si no tengo objetivo, devuelvo Vector3.up que es una señal de que hay un error o de que salio mal
        if (_objective == null) return Vector3.up;

        int companions = 0; // el contador de compañeros en el grupo

        //Vectores de centro y de avoid
        Vector3 center = Vector3.zero;
        Vector3 avoid = Vector3.zero;

        foreach (var partner in _aliveSoldiers)
        {
            // Si soy yo, salto al siguiente
            if (partner == _entityGO) continue;

            // agarro mi distancia entre el compañero y mi mismo
            Vector3 diff = partner.transform.position - _entityGO.transform.position;
            float distance = diff.magnitude;

            bool areObstacles = Physics.Raycast(_entityGO.transform.position, diff.normalized, distance, _unavoidableObstacles);

            if (distance <= partnerDistance && !areObstacles)
            {
                // cohesion
                center += partner.transform.position;

                // sumo el contador de compañeros
                companions++;

                // avoidance
                if (distance < 2f)
                {
                    avoid = avoid + (_entityGO.transform.position - partner.transform.position); // al vector de avoid le sumo un vector para que vaya hacia el lugar contrario
                }
            }
        }

        // basicamente, si tiene compañeros, moverse en la direccion, sino, retorno Vector3.up para tener una referencia de que no tiene grupo este NPC
        if (companions > 0) 
        {
            //center = (center / companions) + _objective.position;

            center = center / companions + (_objective.position - _entityGO.transform.position);

            // alignment lo hace el mismo codigo que ejecute este tipo de movimiento
            Vector3 dir = (center + avoid) - _entityGO.transform.position;
            return dir.normalized;
        }
        else
        {
            return Vector3.up;
        }
        
    }
}