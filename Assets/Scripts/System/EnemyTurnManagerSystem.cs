using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurnManagerSystem
{
    private GameEvent gameEvent;
    private List<TurnComponent> TurnList = new List<TurnComponent>();
    private List<EnemyTurnManagerComponent> enemyTurnManagerList = new List<EnemyTurnManagerComponent>();
    public EnemyTurnManagerSystem(GameEvent gameEvent)
    {
        this.gameEvent = gameEvent;
        gameEvent.AddComponentList += AddComponentList;
        gameEvent.RemoveComponentList += RemoveComponentList;
    }

    public void OnUpdate()
    {
        for (int i = 0; i < enemyTurnManagerList.Count; i++)
        {
            TurnComponent turn = TurnList[i];
            EnemyTurnManagerComponent enemyTurnManager = enemyTurnManagerList[i];
            if (!enemyTurnManager.gameObject.activeSelf) continue;

            if (!turn.IsMyTurn) continue;

            if (enemyTurnManager.EnemyTurnComponentList.Count <= 0) continue;

            for (int j = 0; j < enemyTurnManager.EnemyTurnComponentList.Count; j++)
            {
                EnemyTurnComponent enemyTurn = enemyTurnManager.EnemyTurnComponentList[j];

                if (!enemyTurn.IsPhaseStart)
                {
                    if (j == 0) enemyTurn.IsPhaseStart = true;
                    continue;
                }
                if (!enemyTurn.IsPhaseEnd) continue;

                enemyTurnManager.TurnCount = j + 1;

                if (enemyTurnManager.TurnCount >= enemyTurnManager.EnemyTurnComponentList.Count) break;
                if (enemyTurnManager.EnemyTurnComponentList[enemyTurnManager.TurnCount].IsPhaseStart) continue;
                enemyTurnManager.EnemyTurnComponentList[enemyTurnManager.TurnCount].IsPhaseStart = true;
            }

            if (enemyTurnManager.TurnCount >= enemyTurnManager.EnemyTurnComponentList.Count)
            {

                foreach (var x in enemyTurnManager.EnemyTurnComponentList)
                {
                    x.IsPhaseStart = false;
                    x.IsPhaseEnd = false;
                }

                enemyTurnManager.TurnCount = 0;
                switch (TurnList[i].TurnState)
                {
                    case TurnState.Start:
                        TurnList[i].TurnState = TurnState.Play;
                        enemyTurnManager.EnemyTurnComponentList[enemyTurnManager.TurnCount].IsPhaseStart = true;
                        Debug.Log("Enemy Turn Start");
                        break;
                    case TurnState.Play:
                        TurnList[i].TurnState = TurnState.End;
                        enemyTurnManager.EnemyTurnComponentList[enemyTurnManager.TurnCount].IsPhaseEnd = true;
                        Debug.Log("Enemy Turn End");
                        break;
                    case TurnState.End:
                        TurnList[i].TurnState = TurnState.None;
                        gameEvent.TurnEnd(enemyTurnManager.gameObject);
                        Debug.Log("Enemy Turn None");
                        break;
                }
            }
        }
    }

    private void AddComponentList(GameObject gameObject)
    {
        TurnComponent turn = gameObject.GetComponent<TurnComponent>();
        EnemyTurnManagerComponent enemyTurnManager = gameObject.GetComponent<EnemyTurnManagerComponent>();

        if (turn == null) return;
        if (enemyTurnManager == null) return;

        TurnList.Add(turn);
        enemyTurnManagerList.Add(enemyTurnManager);
    }

    private void RemoveComponentList(GameObject gameObject)
    {
        TurnComponent turn = gameObject.GetComponent<TurnComponent>();
        EnemyTurnManagerComponent enemyTurnManager = gameObject.GetComponent<EnemyTurnManagerComponent>();

        if (turn == null) return;
        if (enemyTurnManager == null) return;

        TurnList.Remove(turn);
        enemyTurnManagerList.Remove(enemyTurnManager);
    }
}
