using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEndSystem
{
    private GameObject enemyTurnManagerObject;
    private List<TurnComponent> turnList = new List<TurnComponent>();

    public TurnEndSystem(GameEvent gameEvent, GameObject enemyTurnManager)
    {
        this.enemyTurnManagerObject = enemyTurnManager;
        gameEvent.AddComponentList += AddComponentList;
        gameEvent.RemoveComponentList += RemoveComponentList;
        gameEvent.TurnEnd += TurnEnd;
    }

    private void TurnEnd(GameObject gameObject)
    {
        for (int i = 0; i < turnList.Count; i++)
        {
            TurnComponent turn = turnList[i];
            if (!turn.gameObject.activeSelf) continue;

            if (turn.gameObject == gameObject)
            {
                turn.IsMyTurn = false;
                turn.TurnState = TurnState.None;
                Debug.Log(gameObject.name + " turn end");
                continue;
            }

            turn.IsMyTurn = true;
            turn.TurnState = TurnState.Start;
            Debug.Log(turn.gameObject.name + " turn start");
        }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < turnList.Count; i++)
        {
            TurnComponent turn = turnList[i];
            if (!turn.gameObject.activeSelf) continue;

            if (!turn.IsMyTurn || turn.TurnState != TurnState.End) continue;

            if (turn.gameObject == enemyTurnManagerObject)
            {
                List<EnemyTurnComponent> enemyTurnList = turn.gameObject.GetComponent<EnemyTurnManagerComponent>().EnemyTurnComponentList;
                for (int j = 0; j < enemyTurnList.Count; j++)
                {
                    EnemyTurnComponent enemyTurn = enemyTurnList[j];
                    if (!enemyTurn.IsPhaseStart) continue;

                    enemyTurn.IsPhaseEnd = true;
                }
                continue;
            }
        }
    }


    private void AddComponentList(GameObject gameObject)
    {
        TurnComponent turn = gameObject.GetComponent<TurnComponent>();

        if (turn == null) return;

        turnList.Add(turn);
    }

    private void RemoveComponentList(GameObject gameObject)
    {
        TurnComponent turn = gameObject.GetComponent<TurnComponent>();

        if (turn == null) return;

        turnList.Remove(turn);
    }
}
