using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

public class EnemyAttackSystem
{
    private GameEvent gameEvent;
    private GameObject playerObject = null;
    private ObjectPool objectPool;
    private GameObject enemyTurnManagerObject;
    private List<CharacterBaseComponent> characterBaseLis = new List<CharacterBaseComponent>();
    private List<EnemyTurnComponent> turnList = new List<EnemyTurnComponent>();
    private List<EnemyAttackComponent> enemyAttackList = new List<EnemyAttackComponent>();

    public EnemyAttackSystem(GameEvent gameEvent, ObjectPool objectPool, GameObject player, GameObject enemyTurnManager)
    {
        this.gameEvent = gameEvent;
        this.objectPool = objectPool;
        playerObject = player;
        enemyTurnManagerObject = enemyTurnManager;
        gameEvent.AddComponentList += AddComponentList;
        gameEvent.RemoveComponentList += RemoveComponentList;
    }

    public void OnUpdate()
    {
        for (int i = 0; i < turnList.Count; i++)
        {
            EnemyTurnComponent turn = turnList[i];
            CharacterBaseComponent characterBase = characterBaseLis[i];

            if (enemyAttackList[i].ShieldEffectList.Count > 0)
            {
                List<int> removeIndex = new List<int>();
                enemyAttackList[i].ShieldEffectList.ForEach(x =>
                {
                    if (x.MoveNext()) return;
                    removeIndex.Add(enemyAttackList[i].ShieldEffectList.IndexOf(x));
                    return;
                });

                if (removeIndex.Count < 0) break;
                foreach (var x in removeIndex)
                {
                    enemyAttackList[i].ShieldEffectList.RemoveAt(x);
                }
            }


            if (!turn.gameObject.activeSelf) continue;

            TurnComponent turnComponent = enemyTurnManagerObject.gameObject.GetComponent<TurnComponent>();

            if (!turnComponent.IsMyTurn || turnComponent.TurnState != TurnState.Play) continue;

            if (!turn.IsPhaseStart || turn.IsPhaseEnd) continue;

            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                DamageComponent damage = playerObject.gameObject.GetComponent<DamageComponent>();
                damage.DamagePoint = characterBase.AttackPoint;
                Debug.Log(turn.gameObject.name + " attack " + playerObject.name);
            }
            else
            {
                characterBase.DefensePoint += 1;
                Debug.Log(turn.gameObject.name + " defense " + playerObject.name);
                GenerateShieldEffect(enemyAttackList[i]);
            }

            turn.IsPhaseEnd = true;
        }
    }

    private void GenerateShieldEffect(EnemyAttackComponent enemyAttack)
    {
        GameObject shieldEffect = objectPool.GetGameObject(enemyAttack.ShieldEffectPrefab);
        shieldEffect.transform.position = enemyAttack.transform.position + new Vector3(-1, 1, -1);
        shieldEffect.transform.localScale = new Vector3(1, 1, 1);
        shieldEffect.SetActive(true);
        shieldEffect.transform.parent = enemyAttack.transform;
        ShieldEffectComponent shieldEffectComponent = shieldEffect.GetComponent<ShieldEffectComponent>();
        shieldEffectComponent.ShieldMaterial = shieldEffect.GetComponent<Renderer>().material;
        shieldEffectComponent.ShieldMaterial.SetColor("_Color", new Color(0, 1, 1, 1));
        enemyAttack.ShieldEffectList.Add(ShieldEffect(shieldEffectComponent));
    }

    private IEnumerator ShieldEffect(ShieldEffectComponent shieldEffect)
    {
        while (true)
        {
            shieldEffect.Timer += Time.deltaTime;
            shieldEffect.transform.position += new Vector3(0, 0.05f, 0);
            shieldEffect.ShieldMaterial.SetColor("_Color", new Color(0, 1, 1, 1 - shieldEffect.Timer / shieldEffect.TimerLimit));

            if (shieldEffect.Timer >= shieldEffect.TimerLimit)
            {
                shieldEffect.Timer = 0.0f;
                gameEvent.ReleaseObject?.Invoke(shieldEffect.gameObject);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void AddComponentList(GameObject gameObject)
    {
        CharacterBaseComponent characterBase = gameObject.GetComponent<CharacterBaseComponent>();
        EnemyTurnComponent turn = gameObject.GetComponent<EnemyTurnComponent>();
        EnemyAttackComponent enemyAttack = gameObject.GetComponent<EnemyAttackComponent>();

        if (characterBase == null || turn == null || enemyAttack == null) return;

        characterBaseLis.Add(characterBase);
        turnList.Add(turn);
        enemyAttackList.Add(enemyAttack);
    }

    private void RemoveComponentList(GameObject gameObject)
    {
        CharacterBaseComponent characterBase = gameObject.GetComponent<CharacterBaseComponent>();
        EnemyTurnComponent turn = gameObject.GetComponent<EnemyTurnComponent>();
        EnemyAttackComponent enemyAttack = gameObject.GetComponent<EnemyAttackComponent>();

        if (characterBase == null || turn == null || enemyAttack == null) return;

        characterBaseLis.Remove(characterBase);
        turnList.Remove(turn);
        enemyAttackList.Remove(enemyAttack);
    }
}
