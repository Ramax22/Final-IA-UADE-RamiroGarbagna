using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeState<T> : FSMState<T>
{
    EntityContainer _entity;
    AStar<Node> _aStar;
    List<Node> _nodeList;
    LayerMask _obstacleMask;
    Avoid _avoid;
    Evade _escape;
    List<Node> _destinyWay;
    GameObject _hittedBy;

    Node _origin;
    Node _destiny;

    int _pointer;

    public EscapeState(EntityContainer entity, LayerMask obstacleMask, LayerMask avoidableObstacles, List<Node> mapNodes)
    {
        _entity = entity;
        _obstacleMask = obstacleMask;
        _nodeList = mapNodes;

        _aStar = new AStar<Node>();
        _avoid = new Avoid(entity.transform, 1.5f, 1.5f, avoidableObstacles);
        _destinyWay = new List<Node>();

        string tag = _entity.transform.tag;
        if (tag == "Blue") _destiny = GameObject.Find("Node (0)").GetComponent<Node>();
        else _destiny = GameObject.Find("Node (8)").GetComponent<Node>();
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        _hittedBy = _entity.LastHittedBy;
        _origin = FindNearestNode();
        _destinyWay = _aStar.Run(_origin, Satisfies, GetNeighbours, GetCost, Heuristic);
        _pointer = 0;
        _escape = new Evade(_entity.transform, _hittedBy.transform, _hittedBy.GetComponent<Rigidbody>(), 1f);
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        if (_pointer < _destinyWay.Count)
        {
            _avoid.SetTarget(_destinyWay[_pointer].transform);
            _entity.MoveEntity(_avoid.GetDir());
            _entity.LookDirEntity(_destinyWay[_pointer].transform.position);

            Vector3 target = _destinyWay[_pointer].transform.position;

            Vector3 diff = target - _entity.transform.position;
            float distance = diff.magnitude;

            if (distance <= 1) _pointer++;
        }
        else
        {
            if (_hittedBy == null)
            {
                _hittedBy = _entity.LastHittedBy;
                if (_hittedBy != null)
                {
                    _escape = new Evade(_entity.transform, _hittedBy.transform, _hittedBy.GetComponent<Rigidbody>(), 1f);
                }
                else
                {
                    _entity.MoveEntity(Vector3.zero);
                }
            }
            else
            {
                var dir = _escape.GetDir();
                _entity.MoveEntity(dir);
            }



            //_entity.MoveEntity(new Vector3(0f, 0f, 0f));
            //_entity.ExecuteDesicionTree();
        }
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {
    }

    //Busca el nodo mas cercano donde tengamos vision directa
    Node FindNearestNode()
    {
        //Declaro variables
        float currentDistance = float.PositiveInfinity;
        Node nearestNode = null;

        //Recorro todos los nodos
        foreach (var item in _nodeList)
        {
            //Calculo la distancia
            Vector3 diff = item.transform.position - _entity.transform.position;
            float dist = diff.magnitude;
            //Verifico si no hay obstaculos entre medio y si la distancia es menor a la distancia del nodo anterior
            bool isFree = Physics.Raycast(_entity.transform.position, diff.normalized, dist, _obstacleMask);
            if (dist < currentDistance && !isFree)
            {
                currentDistance = dist;
                nearestNode = item;
            }
        }
        return nearestNode;
    }

    #region ~~~ PATHFINDING CONDITIONS ~~~
    bool Satisfies(Node curr) { return curr == _destiny; } // Chequeo para ver si el nodo es el que buscamos
    List<Node> GetNeighbours(Node curr) { return curr.neightbourds; } // Funcion para agarrar a los vecinos del nodo
    float GetCost(Node p, Node c) { return Vector3.Distance(p.transform.position, c.transform.position); } // El costo de viajar hacia el nodo vecino
    // Calcular la heuristica 
    float Heuristic(Node curr)
    {
        float cost = 0;
        cost += Vector3.Distance(curr.transform.position, _destiny.transform.position);
        return cost;
    }
    #endregion
}