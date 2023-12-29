using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnComponent : MonoBehaviour
{
    private bool isPhaseStart = false;
    private bool isPhaseEnd = false;

    public bool IsPhaseStart { get => isPhaseStart; set => isPhaseStart = value; }
    public bool IsPhaseEnd { get => isPhaseEnd; set => isPhaseEnd = value; }
}
