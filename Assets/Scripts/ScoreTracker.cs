using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReachBeyond.VariableObjects;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public IntConstReference score;
    public TextMeshProUGUI textObj;
    public string prefix = "Score\n";

    void Update()
    {
        textObj.text = prefix + score.ConstValue;
    }
}
