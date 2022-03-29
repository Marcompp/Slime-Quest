using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lives : MonoBehaviour
{
    Text textComp;
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        gm = GameManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        textComp.text = $"Lives: {gm.vidas}";
    }
}
