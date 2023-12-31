using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject deck;
    [SerializeField] GameObject hands;
    [SerializeField] GameObject trash;
    [SerializeField] GameObject effectRoot;
    [SerializeField] GameObject enemyTurnManager;

    private GameEvent gameEvent;
    private ObjectPool objectPool;
    private Movement movement;

    private DamageSystem damageSystem;
    // カード
    private CardSelectSystem cardSelectSystem;
    private DeckSystem deckSystem;
    private DrawSystem drawSystem;
    private HandsSystem handsSystem;
    // UI
    private HitPointUISystem hitPointUISystem;
    private ManaUISystem manaUISystem;
    private TurnEndButtonSystem turnEndButtonSystem;
    //　　turn
    private TurnSystem turnSystem;
    private StartTurnSystem startTurnSystem;
    private EnemyAttackSystem enemyAttackSystem;
    private TurnEndSystem turnEndSystem;
    private EnemyTurnManagerSystem enemyTurnManagerSystem;

    void Start()
    {
        gameEvent = new GameEvent();
        objectPool = new ObjectPool(gameEvent);
        movement = new Movement();

        damageSystem = new DamageSystem(gameEvent, objectPool, movement);
        // カード
        cardSelectSystem = new CardSelectSystem(gameEvent, movement, objectPool, player, enemy, trash.transform, effectRoot, enemyTurnManager);
        deckSystem = new DeckSystem(gameEvent);
        drawSystem = new DrawSystem(gameEvent);
        handsSystem = new HandsSystem(gameEvent, objectPool, player, deck.transform);
        // UI
        hitPointUISystem = new HitPointUISystem(gameEvent);
        manaUISystem = new ManaUISystem(gameEvent);
        turnEndButtonSystem = new TurnEndButtonSystem(gameEvent);
        // turn
        turnSystem = new TurnSystem(gameEvent, player);
        startTurnSystem = new StartTurnSystem(gameEvent, player);
        enemyAttackSystem = new EnemyAttackSystem(gameEvent, objectPool, player, enemyTurnManager);
        turnEndSystem = new TurnEndSystem(gameEvent, enemyTurnManager);
        enemyTurnManagerSystem = new EnemyTurnManagerSystem(gameEvent);

        gameEvent.AddComponentList?.Invoke(player);
        gameEvent.AddComponentList?.Invoke(enemy);
        gameEvent.AddComponentList?.Invoke(enemy2);
        gameEvent.AddComponentList?.Invoke(deck);
        gameEvent.AddComponentList?.Invoke(hands);
        gameEvent.AddComponentList?.Invoke(enemyTurnManager);
    }

    void Update()
    {
        damageSystem.OnUpdate();
        // カード
        cardSelectSystem.OnUpdate();
        deckSystem.OnUpdate();
        handsSystem.OnUpdate();
        // UI
        hitPointUISystem.OnUpdate();
        manaUISystem.OnUpdate();
        // turn
        turnSystem.OnUpdate();
        startTurnSystem.OnUpdate();
        enemyAttackSystem.OnUpdate();
        enemyTurnManagerSystem.OnUpdate();
        turnEndSystem.OnUpdate();
    }
}
