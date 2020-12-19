using System.Collections.Generic;

public class FSMState<T> //Creo un parametro generico
{
    //Diccionario que contega todos los estados de la FSM con el parametro génerico declarado en la clase
    Dictionary<T, FSMState<T>> _states = new Dictionary<T, FSMState<T>>();

    //Funciones que inician, ejecutan y apagan el estado. Serán reemplazadas por el mismo estado
    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    //Funcion para agregar transiciones entre los distintos estados
    public void AddTransition(T input, FSMState<T> state)
    {
        //Lo agrego al diccionario, si este no lo tiene
        if (!_states.ContainsKey(input)) _states.Add(input, state);
    }

    //Funcion para eliminar transiciones entre estados
    public void RemoveTransition(T input)
    {
        //Si el diccicionario contiene este elemento, lo elimina
        if (_states.ContainsKey(input)) _states.Remove(input);
    }

    //Funcion para obtener un estado mediante input
    public FSMState<T> GetTransition(T input)
    {
        //Si el estado pasado por parametro esta en el diccionario, lo retorno,
        //Caso contrario retorno null
        if (_states.ContainsKey(input)) return _states[input];
        else return null;
    }
}