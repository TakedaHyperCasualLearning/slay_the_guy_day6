using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsSystem
{
    private GameObject playerObject;
    private GameEvent gameEvent = null;
    private ObjectPool objectPool = null;
    private Transform deckTransform = null;
    private List<HandsComponent> handsList = new List<HandsComponent>();

    public HandsSystem(GameEvent gameEvent, ObjectPool objectPool, GameObject player, Transform deckTransform)
    {
        this.gameEvent = gameEvent;
        this.objectPool = objectPool;
        playerObject = player;
        this.deckTransform = deckTransform;
        gameEvent.AddComponentList += AddComponentList;
        gameEvent.RemoveComponentList += RemoveComponentList;
    }

    private void Initialize(HandsComponent hands)
    {
        hands.NowHandsCardCount = 5;
        // for (int i = 0; i < hands.CardList.Count; i++)
        // {
        //     CardBaseComponent card = hands.CardList[i];
        //     gameEvent.AddComponentList?.Invoke(card.gameObject);
        //     DrawEffectComponent drawEffect = card.gameObject.GetComponent<DrawEffectComponent>();
        //     drawEffect.StartPosition = deckTransform.gameObject.GetComponent<RectTransform>().anchoredPosition3D;
        //     card.transform.position = drawEffect.StartPosition;
        //     card.transform.rotation = hands.transform.rotation;
        //     card.transform.localScale = new Vector3(0, 0, 0);
        // }
    }

    public void OnUpdate()
    {
        for (int i = 0; i < handsList.Count; i++)
        {
            HandsComponent hands = handsList[i];
            if (!hands.gameObject.activeSelf) continue;

            foreach (var drawEffect in hands.DrawEffectList)
            {
                if (drawEffect.MoveNext()) continue;
                hands.DrawEffectList.Remove(drawEffect);
                break;
            }

            TurnComponent turn = playerObject.GetComponent<TurnComponent>();
            if (!turn.IsMyTurn || turn.TurnState != TurnState.Draw) continue;

            hands.DrawEffectTimer += Time.deltaTime;
            if (hands.DrawEffectTimer < hands.DrawEffectLimitTime) continue;
            if (hands.CardList.Count > 0 && hands.CardList.Count >= hands.NowHandsCardCount)
            {

                Debug.Log(hands.CardList[hands.DrawCount].gameObject.name);
                gameEvent.ReleaseObject(hands.CardList[hands.DrawCount].gameObject);
            }
            DrawCard(hands);
            hands.DrawCount++;
            hands.DrawEffectTimer = 0.0f;

            if (hands.DrawCount < hands.NowHandsCardCount) continue;
            hands.DrawCount = 0;
            turn.TurnState = TurnState.Play;
            Debug.Log(turn.gameObject.name + "Battle phase");
        }
    }

    private void DrawCard(HandsComponent hands)
    {
        List<CardBaseComponent> cardList = gameEvent.Draw();
        for (int i = 0; i < cardList.Count; i++)
        {
            GameObject cardObject = objectPool.GetGameObject(hands.CardPrefab);
            cardObject.transform.SetParent(hands.CardRoot.transform);
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = deckTransform.gameObject.GetComponent<RectTransform>().anchoredPosition3D;
            rectTransform.rotation = hands.transform.rotation;
            rectTransform.localScale = new Vector3(0, 0, 0);
            CardBaseComponent card = cardObject.GetComponent<CardBaseComponent>();
            if (objectPool.IsNewCreate)
            {
                hands.CardList.Add(card);
                gameEvent.AddComponentList?.Invoke(card.gameObject);
                objectPool.IsNewCreate = false;
            }

            card.CostPoint = cardList[i].CostPoint;
            card.AttackPoint = cardList[i].AttackPoint;
            card.Title = cardList[i].Title;
            card.Description = cardList[i].Description;

            card.CostPointText.text = cardList[i].CostPoint.ToString();
            card.TitleText.text = cardList[i].Title;
            card.DescriptionText.text = cardList[i].Description;

            DrawEffectComponent drawEffect = card.gameObject.GetComponent<DrawEffectComponent>();
            drawEffect.StartPosition = deckTransform.gameObject.GetComponent<RectTransform>().anchoredPosition3D;
            Vector3 tempPosition = drawEffect.EndPosition;
            float mag_y = Mathf.Abs(hands.DrawCount - Mathf.Floor(hands.NowHandsCardCount / 2.0f));
            tempPosition.x = hands.Distance * hands.DrawCount - Mathf.Floor(hands.NowHandsCardCount / 2.0f) * hands.Distance;
            tempPosition.y = hands.BaseHeight - hands.Height * mag_y - hands.Height * Mathf.Max(0, mag_y + mag_y - 1.0f);
            tempPosition.z = 0.0f;
            drawEffect.EndPosition = tempPosition;

            Quaternion tempRotation = Quaternion.identity;
            tempRotation.eulerAngles = new Vector3(0, 0, -hands.Angle * hands.DrawCount + Mathf.Floor(hands.NowHandsCardCount / 2.0f) * hands.Angle);
            drawEffect.EndRotation = tempRotation;

            CardSelectComponent cardSelect = card.GetComponent<CardSelectComponent>();
            cardSelect.BasePosition = drawEffect.EndPosition;
            cardSelect.BaseRotation = drawEffect.EndRotation;

            hands.DrawEffectList.Add(DrawEffect(drawEffect));
        }
    }

    private IEnumerator DrawEffect(DrawEffectComponent drawEffect)
    {
        while (true)
        {
            drawEffect.Timer += Time.deltaTime;
            float ratio = drawEffect.Timer / drawEffect.LimitTime;
            RectTransform rectTransform = drawEffect.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.Lerp(drawEffect.StartPosition, drawEffect.EndPosition, ratio);
            rectTransform.rotation = Quaternion.Lerp(drawEffect.transform.rotation, drawEffect.EndRotation, ratio);
            rectTransform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(3, 3, 3), ratio);

            if (drawEffect.Timer >= drawEffect.LimitTime)
            {
                rectTransform.anchoredPosition3D = drawEffect.EndPosition;
                drawEffect.transform.rotation = drawEffect.EndRotation;
                drawEffect.transform.localScale = new Vector3(3, 3, 3);
                drawEffect.Timer = 0;
                yield break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void AddComponentList(GameObject gameObject)
    {
        HandsComponent hands = gameObject.GetComponent<HandsComponent>();

        if (hands == null) return;

        handsList.Add(hands);

        Initialize(hands);
    }

    private void RemoveComponentList(GameObject gameObject)
    {
        HandsComponent hands = gameObject.GetComponent<HandsComponent>();

        if (hands == null) return;

        handsList.Remove(hands);
    }
}
