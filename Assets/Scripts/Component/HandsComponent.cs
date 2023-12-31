using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsComponent : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab = null;
    [SerializeField] private GameObject cardRoot = null;
    private List<CardBaseComponent> cardList = new List<CardBaseComponent>();
    private List<IEnumerator> drawEffectList = new List<IEnumerator>();
    private float drawEffectTimer = 0.0f;
    [SerializeField] private float drawEffectLimitTime = 0.0f;
    private int drawCount = 0;
    [SerializeField] private int distance = 0;
    [SerializeField] private int angle = 0;
    [SerializeField] private int height = 0;
    [SerializeField] private int baseHeight = 0;
    private int nowHandsCardCount = 0;

    public GameObject CardPrefab { get => cardPrefab; set => cardPrefab = value; }
    public GameObject CardRoot { get => cardRoot; set => cardRoot = value; }
    public List<CardBaseComponent> CardList { get => cardList; set => cardList = value; }
    public List<IEnumerator> DrawEffectList { get => drawEffectList; set => drawEffectList = value; }
    public float DrawEffectTimer { get => drawEffectTimer; set => drawEffectTimer = value; }
    public float DrawEffectLimitTime { get => drawEffectLimitTime; set => drawEffectLimitTime = value; }
    public int DrawCount { get => drawCount; set => drawCount = value; }
    public int Distance { get => distance; set => distance = value; }
    public int Angle { get => angle; set => angle = value; }
    public int Height { get => height; set => height = value; }
    public int BaseHeight { get => baseHeight; set => baseHeight = value; }
    public int NowHandsCardCount { get => nowHandsCardCount; set => nowHandsCardCount = value; }
}
