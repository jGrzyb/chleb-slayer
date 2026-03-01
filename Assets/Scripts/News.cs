using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class News : MonoBehaviour
{
    public TextMeshProUGUI info;
    void Start()
    {
        GameManager.EndStats stats = GameManager.I.endStats;
        if (stats.win)
        {
            info.text = "Win";
        }
        else
        {
            info.text = "Lose";
        }
    }
}
