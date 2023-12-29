using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnManagerComponent : MonoBehaviour
{
    private List<GameObject> enemyTurnList = new List<GameObject>();
    [SerializeField] private List<EnemyTurnComponent> enemyTurnComponentList = new List<EnemyTurnComponent>();
    private TurnState turnStatus = TurnState.None;
    private int turnCount = 0;

    public List<GameObject> EnemyTurnList { get => enemyTurnList; set => enemyTurnList = value; }
    public List<EnemyTurnComponent> EnemyTurnComponentList { get => enemyTurnComponentList; set => enemyTurnComponentList = value; }
    public TurnState TurnStatus { get => turnStatus; set => turnStatus = value; }
    public int TurnCount { get => turnCount; set => turnCount = value; }
}
