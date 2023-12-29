using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class StartTurnSystem
{
    private GameObject playerObject;
    private List<TurnComponent> turnList = new List<TurnComponent>();

    public StartTurnSystem(GameEvent gameEvent, GameObject player)
    {
        playerObject = player;
        gameEvent.AddComponentList += AddComponentList;
        gameEvent.RemoveComponentList += RemoveComponentList;
    }


    public void OnUpdate()
    {
        for (int i = 0; i < turnList.Count; i++)
        {
            TurnComponent turn = turnList[i];
            if (!turn.gameObject.activeSelf) continue;

            if (!turn.IsMyTurn || turn.TurnState != TurnState.Start) continue;

            if (turn.gameObject == playerObject)
            {
                turn.TurnState = TurnState.Draw;
                CharacterBaseComponent characterBase = playerObject.GetComponent<CharacterBaseComponent>();
                characterBase.ManaPoint = characterBase.ManaPointMax;
                Debug.Log(turn.gameObject.name + "Draw phase");
                continue;
            }

            List<EnemyTurnComponent> enemyTurnList = turn.gameObject.GetComponent<EnemyTurnManagerComponent>().EnemyTurnComponentList;
            for (int j = 0; j < enemyTurnList.Count; j++)
            {
                EnemyTurnComponent enemyTurn = enemyTurnList[j];
                if (!enemyTurn.IsPhaseStart) continue;

                enemyTurn.IsPhaseEnd = true;
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
