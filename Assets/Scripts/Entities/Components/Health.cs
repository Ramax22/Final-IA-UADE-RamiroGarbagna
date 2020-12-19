using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _maxHp;
    [SerializeField] EntityContainer _owner;

    [SerializeField] float _actualHp;

    private void Awake()
    {
        _actualHp = _maxHp;
    }

    #region ~~~ ENCAPSULADO ~~~
    public float ActualHp { get { return _actualHp; } }
    #endregion

    #region ~~~ FUNCTIONS ~~~
    public void ChangeLife(float modifier) 
    {
        _actualHp -= modifier;
        _actualHp = Mathf.Clamp(_actualHp, 0, _maxHp);

        if (_actualHp == 0)
        {
            _owner.Die();
        }
    }
    #endregion
}