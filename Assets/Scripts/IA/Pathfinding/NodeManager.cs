using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    [SerializeField] List<Node> _mapNodeList; // Lista de todos los nodos que hay en el nivel

    public List<Node> MapNodeList { get { return _mapNodeList; } }
}