public class FSM<T>
{
    //Referencia al estado actual
    FSMState<T> _current;

    //Función que se llamará todos los frames para ejecutar el Execute del estado correspondiente
    public void OnUpdate() { _current.Execute(); }

    //Funcion que se encargará de manejar las transiciones
    public void Transition(T input)
    {
        //Agarro el estado que deseo ejecutar
        FSMState<T> newState = _current.GetTransition(input);

        //En caso de que el estado exista en el diccionario, lo ejecuto y apago el actual
        if (newState != null)
        {
            _current.Sleep();
            _current = newState;
            _current.Awake();
        }
    }

    public FSMState<T> GetState()
    {
        return _current;
    }

    //Funcion donde declaro el initialState como _current y ejecuto su awake
    public void SetInitialState(FSMState<T> initialState)
    {
        _current = initialState;
        _current.Awake();
    }
}