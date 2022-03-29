using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject GreenSlime;
    public Vector3[] GreenPositions = new Vector3[] { 
        new Vector3(90.55f,1.82f,0), 
        new Vector3(98.5f,1.25f,0), 
        new Vector3(102.5f,1.25f,0)
    };
    public GameObject PurpleSlime;
    public Vector3[] PurplePositions = new Vector3[] { 
        new Vector3(114.35f,-1.2f,0), 
        new Vector3(115.35f,-2.9f,0), 
        new Vector3(117.35f,-2.4f,0), 
        new Vector3(118.35f,-0.1f,0)
    };
    public GameObject RedSlime;
    public Vector3[] RedPositions = new Vector3[] { 
        new Vector3(78.5f,-3.3f,0)
    };
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.GetInstance();
        GameManager.changeStateDelegate += Construir;
        Construir();
    }

     void Construir()
    {
        if (gm.gameState == GameManager.GameState.GAME)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Vector3 posicao in GreenPositions)
            {
                Instantiate(GreenSlime, posicao, Quaternion.identity, transform);
            }
            foreach (Vector3 posicao in PurplePositions)
            {
                Instantiate(PurpleSlime, posicao, Quaternion.identity, transform);
            }
            foreach (Vector3 posicao in RedPositions)
            {
                Instantiate(RedSlime, posicao, Quaternion.identity, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
