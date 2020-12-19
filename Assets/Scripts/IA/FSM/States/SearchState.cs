using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState<T> : FSMState<T>
{
    // Variables
    EntityContainer _entity;
    AStar<Node> _aStar;
    List<Node> _nodeList;
    LayerMask _obstacleMask;

    Node _origin;
    Node _destiny;

    Avoid _avoid;
    List<Node> _destinyWay;

    int _pointer;

    float _sightTimer; // El tiempo que tarda en volver a ejecutar el codigo que revisa la visión del soldado
    Sight _sight; // el componente sight del EnitityContainer

    // Constructor
    public SearchState(EntityContainer entity, List<Node> mapNodes, LayerMask obstacleMask, LayerMask avoidableObstacles)
    {
        _entity = entity;
        _obstacleMask = obstacleMask;
        _sight = entity.Sight;

        _aStar = new AStar<Node>();
        _avoid = new Avoid(entity.transform, 1.5f, 1.5f, avoidableObstacles);
        _nodeList = mapNodes;

        _destinyWay = new List<Node>();
    }

    //Sobreescribo la función Awake de la clase FSMState
    public override void Awake()
    {
        SetDestination();
        _destinyWay = _aStar.Run(_origin, Satisfies, GetNeighbours, GetCost, Heuristic);
        _pointer = 0;
        _sightTimer = 0.3f;
    }

    //Sobreescribo la funcion de Execute de la clase FSMState
    public override void Execute()
    {
        // chequeo la vision 
        _sightTimer -= Time.deltaTime;
        if (_sightTimer <= 0)
        {
            _sight.IsInSight();
            _sightTimer = 0.3f;
        }

        if (_pointer < _destinyWay.Count)
        {
            _avoid.SetTarget(_destinyWay[_pointer].transform);
            _entity.MoveEntity(_avoid.GetDir());
            _entity.LookDirEntity(_destinyWay[_pointer].transform.position);

            Vector3 target = _destinyWay[_pointer].transform.position;

            Vector3 diff = target - _entity.transform.position;
            float distance = diff.magnitude;

            if (distance <= 1.5f) _pointer++;
        }
        else
        {
            _entity.MoveEntity(new Vector3(0f, 0f, 0f));
            _entity.ExecuteDesicionTree();
        }
    }

    //Sobreescribo la funcion de Sleep de la clase FSMState
    public override void Sleep()
    {
    }

    #region ~~~ NODE SELECTOR ~~~
    
    // Asigna un valor a los nodos _origin y _destiny
    void SetDestination()
    {
        // Agarro un nodo de origen
        _origin = FindNearestNode();

        // Agarro un nodo destino
        _destiny = GetRandomNode();
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

    // Agarra un nodo random de una lista
    Node GetRandomNode()
    {
        Node n = _nodeList[Random.Range(0, _nodeList.Count - 1)];

        while (n == _origin) n = _nodeList[Random.Range(0, _nodeList.Count - 1)];
        return n;
    }
    #endregion

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