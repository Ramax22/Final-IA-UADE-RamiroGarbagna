using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContainer : MonoBehaviour
{
    // Team enum
    public enum Team { Blue, Red}

    // Generic Vars
    [SerializeField] LayerMask _mapObstacles;
    [SerializeField] LayerMask _avoidableObstacles;
    [SerializeField] LayerMask _teamSoldier;
    [SerializeField] Movement _entityMovement;
    [SerializeField] bool _isLeader;
    [SerializeField] Team _myTeam;
    [SerializeField] Transform _flockingAim; // variable que solo utilizará el lider, y es para tener un punto a donde sus soldados puedan seguir para hacer flocking
    [SerializeField] Sight _sight;
    [SerializeField] Health _health;
    [SerializeField] Attack _attackComponent;

    // FSM
    FSM<string> _fsm;

    // Keys de los estados
    const string _search = "search";
    const string _lookAround = "look around";
    const string _idle = "idle";
    const string _follow = "follow";
    const string _escape = "escape";
    const string _attack = "attack";
    const string _aproach = "aproach";

    // La lista de todos los nodos del mapa
    List<Node> _mapNodes;

    // Node Manager que tiene referencia a todos los nodos
    NodeManager _nodeManager;

    // La lista de todos sus soldados aliados
    List<GameObject> _allies = new List<GameObject>();

    // Variables para tener de referencia a los lideres
    public EntityContainer _blueLeader;
    public EntityContainer _redLeader;

    // Referencia a mi GO target
    GameObject _target;

    // Referencia al GO que me golpeo ultimo
    GameObject _lastHittedBy;

    // Question Nodes
    QuestionNode _lowHp;
    QuestionNode _seeingTarget;
    QuestionNode _inAttackRange;
    QuestionNode _followLeader;

    //Roulette 
    Roulette<string> _actionRoulette;
    Dictionary<string, int> _statesRoulette;

    //referencia al Game Manager
    GameManager _manager;

    //El estado de follow y escape
    FollowState<string> followState = null;
    EscapeState<string> escapeState = null;

    #region ~~~ ENCAPSULADO ~~~
    public List<GameObject> Allies { set { _allies = value; } }
    public Transform FlockingAim { get { return _flockingAim; } }
    public GameObject Target { get { return _target; } }
    public Sight Sight { get { return _sight; } }
    public GameManager Manager { set { _manager = value; } }
    public GameObject LastHittedBy { set { _lastHittedBy = value; } get { return _lastHittedBy; } }
    #endregion

    private void Start()
    {
        InitInternalBlackboard(); // Init del blackboard
        InitFSM(); // Inicializo la FSM
        InitDesicionTree(); // Inicializo el arbol de desiciones
        InitRoulette(); // inicializo la ruleta
    }

    private void Update()
    {
        // Ejecuto el estado de la FSM
        _fsm.OnUpdate();
    }

    #region ~~~ FSM ~~~
    // Init de todo lo relacionado con la FSM
    void InitFSM()
    {
        // Inicializo la FSM
        _fsm = new FSM<string>();

        // Creo los distintos estados
        IdleState<string> idleState = new IdleState<string>(this);
        SearchState<string> searchState = new SearchState<string>(this, _mapNodes, _mapObstacles, _avoidableObstacles);
        LookAroundState<string> lookAroundState = new LookAroundState<string>(this);
        escapeState = new EscapeState<string>(this, _mapObstacles, _avoidableObstacles, _mapNodes);
        AproachState<string> aproachState = new AproachState<string>(this, _sight, _avoidableObstacles);
        AttackState<string> attackState = new AttackState<string>(this, _attackComponent);

        if (!_isLeader)
        {
            EntityContainer myLeader;

            if (_myTeam == Team.Blue)
            {
                myLeader = _blueLeader;
            }
            else
            {
                myLeader = _redLeader;
            }
            followState = new FollowState<string>(this, _allies, _mapObstacles, myLeader.FlockingAim);
        }
        //GoToLastPointState<string> goToLastPointState = new GoToLastPointState<string>();

        // Creo las transiciones
        idleState.AddTransition(_search, searchState);
        idleState.AddTransition(_lookAround, lookAroundState);
        idleState.AddTransition(_escape, escapeState);
        if (!_isLeader) idleState.AddTransition(_follow, followState);
        idleState.AddTransition(_aproach, aproachState);
        idleState.AddTransition(_attack, attackState);

        searchState.AddTransition(_idle, idleState);
        searchState.AddTransition(_lookAround, lookAroundState);
        searchState.AddTransition(_escape, escapeState);
        if (!_isLeader) searchState.AddTransition(_follow, followState);
        searchState.AddTransition(_aproach, aproachState);
        searchState.AddTransition(_attack, attackState);

        lookAroundState.AddTransition(_idle, idleState);
        lookAroundState.AddTransition(_search, searchState);
        lookAroundState.AddTransition(_escape, escapeState);
        if (!_isLeader) lookAroundState.AddTransition(_follow, followState);
        lookAroundState.AddTransition(_aproach, aproachState);
        lookAroundState.AddTransition(_attack, attackState);

        escapeState.AddTransition(_idle, idleState);
        escapeState.AddTransition(_search, searchState);
        escapeState.AddTransition(_lookAround, lookAroundState);
        if (!_isLeader) escapeState.AddTransition(_follow, followState);
        escapeState.AddTransition(_aproach, aproachState);
        escapeState.AddTransition(_attack, attackState);

        if (!_isLeader)
        {
            followState.AddTransition(_idle, idleState);
            followState.AddTransition(_search, searchState);
            followState.AddTransition(_lookAround, lookAroundState);
            followState.AddTransition(_escape, escapeState);
            followState.AddTransition(_aproach, aproachState);
            followState.AddTransition(_attack, attackState);
        }

        aproachState.AddTransition(_idle, idleState);
        aproachState.AddTransition(_search, searchState);
        aproachState.AddTransition(_lookAround, lookAroundState);
        if (!_isLeader) aproachState.AddTransition(_follow, followState);
        aproachState.AddTransition(_escape, escapeState);
        aproachState.AddTransition(_attack, attackState);

        attackState.AddTransition(_idle, idleState);
        attackState.AddTransition(_search, searchState);
        attackState.AddTransition(_lookAround, lookAroundState);
        if (!_isLeader) attackState.AddTransition(_follow, followState);
        attackState.AddTransition(_escape, escapeState);
        attackState.AddTransition(_aproach, aproachState);

        // Inicializo la fsm
        if (_isLeader) _fsm.SetInitialState(idleState); // Líder
        else _fsm.SetInitialState(idleState); // Soldier
    }

    void GoToIdleState() { _fsm.Transition(_idle); }
    void GoToSearchState() { _fsm.Transition(_search); }
    void GoToLookAroundState() { _fsm.Transition(_lookAround); }
    void GoToEscapeState() { _fsm.Transition(_escape); }
    void GoToFollowState() { _fsm.Transition(_follow); }
    void GoToAproachState() { _fsm.Transition(_aproach); }
    void GoToAttackState() { _fsm.Transition(_attack); }
    void GoToRandomState() { _fsm.Transition(ExecuteRoulette()); }

    // Ejecutar el Desicion Tree
    public void ExecuteDesicionTree()
    {
        if (_fsm.GetState() == escapeState) return;
        _lowHp.Execute();
    }
    #endregion

    #region ~~~ DESICION TREE ~~~
    void InitDesicionTree()
    {
        if (_isLeader)
        {
            // Creo los action node para el lider
            ActionNode _escapeNode = new ActionNode(GoToEscapeState);
            ActionNode _aproachState = new ActionNode(GoToAproachState);
            ActionNode _attackState = new ActionNode(GoToAttackState);
            ActionNode _randomState = new ActionNode(GoToRandomState);

            //Creo los question nodes
            _inAttackRange = new QuestionNode(IsInRange, _attackState, _aproachState);
            _seeingTarget = new QuestionNode(SeeingTarget, _inAttackRange, _randomState);
            _lowHp = new QuestionNode(HaveLowHp, _escapeNode, _seeingTarget);
        }
        else
        {
            // Creo los action node para el lider
            ActionNode _escapeNode = new ActionNode(GoToEscapeState);
            ActionNode _aproachState = new ActionNode(GoToAproachState);
            ActionNode _attackState = new ActionNode(GoToAttackState);
            ActionNode _randomState = new ActionNode(GoToRandomState);
            ActionNode _followState = new ActionNode(GoToFollowState);

            //Creo los question nodes
            _followLeader = new QuestionNode(CanFollowLeader, _followState, _randomState);
            _inAttackRange = new QuestionNode(IsInRange, _attackState, _aproachState);
            _seeingTarget = new QuestionNode(SeeingTarget, _inAttackRange, _followLeader);
            _lowHp = new QuestionNode(HaveLowHp, _escapeNode, _seeingTarget);
        }
    }

    bool HaveLowHp() { return _health.ActualHp < 20; }
    bool SeeingTarget()
    {
        if (_target == null) return false;
        else return true;
    }
    bool IsInRange()
    {
        float distance = Vector3.Distance(transform.position, _target.transform.position);
        if (distance <= 1.5f) return true;
        else return false;
    }
    bool CanFollowLeader()
    {
        EntityContainer leader;
        if (_myTeam == Team.Blue) leader = _blueLeader;
        else leader = _redLeader;

        if (leader == null) return false;

        Vector3 diff = leader.transform.position - transform.position;
        float distance = diff.magnitude;
        if (distance < 10f)
        {
            bool areObstacles = Physics.Raycast(transform.position, diff.normalized, distance, _mapObstacles);
            if (areObstacles || _fsm.GetState() == followState) return false;
            else return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region ~~~ ROULETTE DESICION WHEEL ~~~
    void InitRoulette()
    {
        // inicializo la roulette
        _actionRoulette = new Roulette<string>();

        //Agrego el dicionario con todos los elementos de la ruleta
        _statesRoulette = new Dictionary<string, int>();
        _statesRoulette.Add(_search, 30);
        _statesRoulette.Add(_lookAround, 50);
        _statesRoulette.Add(_idle, 20);
    }

    string ExecuteRoulette() { return _actionRoulette.Run(_statesRoulette); }

    public void ChangeRouletteValues(bool isBlueZone)
    {
        if (isBlueZone)
        {
            if (_myTeam == Team.Blue)
            {
                _statesRoulette[_search] = 30;
                _statesRoulette[_lookAround] = 50;
                _statesRoulette[_idle] = 20;
            }
            else
            {
                _statesRoulette[_search] = 70;
                _statesRoulette[_lookAround] = 20;
                _statesRoulette[_idle] = 10;
            }
        }
        else
        {
            if (_myTeam == Team.Red)
            {
                _statesRoulette[_search] = 30;
                _statesRoulette[_lookAround] = 50;
                _statesRoulette[_idle] = 20;
            }
            else
            {
                _statesRoulette[_search] = 70;
                _statesRoulette[_lookAround] = 20;
                _statesRoulette[_idle] = 10;
            }
        }
    }
    #endregion

    #region ~~~ INTERNAL BLACKBOARD ~~~
    void InitInternalBlackboard()
    {
        _nodeManager = GameObject.Find("Nodes").GetComponent<NodeManager>();

        _mapNodes = _nodeManager.MapNodeList;
    }

    public void SetTarget(GameObject target) 
    { 
        _target = target;
        ExecuteDesicionTree();
    }
    #endregion

    #region ~~~ COMPONENTS FUNCTIONS ~~~
    public void MoveEntity(Vector3 dir) { _entityMovement.Move(dir); }
    public void LookDirEntity(Vector3 point) { _entityMovement.Look(point); }
    public void RotateTo(Quaternion rotation) { _entityMovement.RotateTo(rotation); }
    public void LookAtPoint(Vector3 lookAt) { _entityMovement.LookAtPoint(lookAt); }
    public void RotateEntity(Vector3 eulers) { _entityMovement.Rotate(eulers); }
    public void GetDamaged(float damage) 
    { 

        _health.ChangeLife(damage); 
    }
    
    public void Die()
    {
        if (_myTeam == Team.Blue)
        {
            _manager.BlueTeamSoldiers.Remove(gameObject);

            if (_isLeader)
            {
                int soldiersAlive = _manager.BlueTeamSoldiers.Count - 1;

                for (int i = 0; i <= soldiersAlive; i++)
                {
                    if (_manager.BlueTeamSoldiers.Count < 1) continue;
                    var soldier = _manager.BlueTeamSoldiers[0];
                    soldier.GetComponent<EntityContainer>().Die();
                }
                _manager.EndGame(false);
            }
            Destroy(gameObject);
        }
        else
        {
            _manager.RedTeamSoldiers.Remove(gameObject);

            if (_isLeader)
            {
                foreach (var soldier in _manager.RedTeamSoldiers)
                {
                    soldier.GetComponent<EntityContainer>().Die();
                }
                _manager.EndGame(true);
            }
            Destroy(gameObject);
        }
    }
    #endregion
}