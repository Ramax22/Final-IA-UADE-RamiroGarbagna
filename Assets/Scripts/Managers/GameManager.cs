using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Quantity of Soldiers")]
    [SerializeField] int _blueTeamSoldiersQuantity;
    [SerializeField] int _redTeamSoldiersQuantity;

    [Header("Blue team configs")]
    [SerializeField] EntityContainer _blueLeader;
    [SerializeField] GameObject _blueSoldierPrefab;
    [SerializeField] List<Transform> _blueTeamSpawnPoints;

    [Header("Red team configs")]
    [SerializeField] EntityContainer _redLeader;
    [SerializeField] GameObject _redSoldierPrefab;
    [SerializeField] List<Transform> _redTeamSpawnPoints;

    //La lista de GO en cada equipo
    List<GameObject> _blueTeamSoldiers = new List<GameObject>();
    List<GameObject> _redTeamSoldiers = new List<GameObject>();

    private void Awake()
    {
        //Clampeo al maximo la cantidad de soldados spawneados por la cantidad de spawn points en el mapa
        _blueTeamSoldiersQuantity = Mathf.Clamp(_blueTeamSoldiersQuantity, 0, _blueTeamSpawnPoints.Count - 1);
        _redTeamSoldiersQuantity = Mathf.Clamp(_redTeamSoldiersQuantity, 0, _redTeamSpawnPoints.Count - 1);

        // Instancio los soldados de cada equipo
        InstantiateTeams();
    }

    #region ~~~ GAME FUNCTIONS ~~~
    // Instancia ambos equipos
    void InstantiateTeams()
    {
        // ~~~ blue team ~~~

        // los instancio
        _blueTeamSoldiers.Add(_blueLeader.transform.gameObject);
        for (int i = 0; i < _blueTeamSoldiersQuantity; i++)
        {
            Transform position = _blueTeamSpawnPoints[i];
            GameObject soldier = GameObject.Instantiate(_blueSoldierPrefab, position.position, position.rotation);
            _blueTeamSoldiers.Add(soldier);
        }

        // les mando la lista de soldados de su equipo
        foreach (var soldier in _blueTeamSoldiers)
        {
            EntityContainer soldierContainer = soldier.GetComponent<EntityContainer>();
            soldierContainer.Allies = _blueTeamSoldiers;
            soldierContainer._blueLeader = _blueLeader;
            soldierContainer.Manager = this;
            Sight s = soldier.GetComponent<Sight>();
            s.Enemies = _redTeamSoldiers;
        }


        // ~~~ red team ~~~

        // los instancio
        _redTeamSoldiers.Add(_redLeader.transform.gameObject);
        for (int i = 0; i < _redTeamSoldiersQuantity; i++)
        {
            Transform position = _redTeamSpawnPoints[i];
            GameObject soldier = GameObject.Instantiate(_redSoldierPrefab, position.position, position.rotation);
            _redTeamSoldiers.Add(soldier);
        }

        // les mando la lista de soldados de su equipo
        foreach (var soldier in _redTeamSoldiers)
        {
            EntityContainer soldierContainer = soldier.GetComponent<EntityContainer>();
            soldierContainer.Allies = _redTeamSoldiers;
            soldierContainer._redLeader = _redLeader;
            soldierContainer.Manager = this;
            Sight s = soldier.GetComponent<Sight>();
            s.Enemies = _blueTeamSoldiers;
        }

        // ahora les digo a todas las unidades cuales son sus enemigo
    }
    #endregion

    #region ~~~ ENCAPSULADOS ~~~
    public EntityContainer BlueLeader { get { return _blueLeader; } }
    public List<GameObject> BlueTeamSoldiers { get { return _blueTeamSoldiers; } }
    public List<GameObject> RedTeamSoldiers { get { return _redTeamSoldiers; } }
    #endregion
}