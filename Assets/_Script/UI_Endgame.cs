using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Endgame : MonoBehaviour
{
    public Text message;
    GameManager gm;

    private void OnEnable()
    {
        gm = GameManager.GetInstance();

        if(gm.vidas > 0)
        {
            message.color = Color.cyan;
            message.text = "Victory!!!";
        }
        else
        {
            message.color = Color.red;
            message.text = "GAME OVER";
        }
    }

    public void Voltar()
    {
        gm.ChangeState(GameManager.GameState.GAME);
    }
    public void Menu()
    {
        gm.ChangeState(GameManager.GameState.MENU);
    }
}
