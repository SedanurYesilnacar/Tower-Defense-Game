using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject _towerObject;
    [SerializeField]
    private int _towerPrice;

    // Property
    public GameObject TowerObject
    {
        get
        {
            return _towerObject;
        }
    }

    public int TowerPrice {
        get
        {
            return _towerPrice;
        }
    }
}
