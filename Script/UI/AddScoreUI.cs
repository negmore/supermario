using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScoreUI : MonoBehaviour
{
    public GameObject[] gameobject;

    public float delTime = 2f;

    void Awake()
    {
        Destroy(gameObject, delTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, 0.05f * Time.deltaTime);
    }

    public void SetScore(int Score)
    {
        if (Score < 1000)
            gameobject[0].SetActive(false);
        else
            gameobject[0].SetActive(true);

        for (int i = 0; i < 4; ++i)
        {
            int value = Score % 10;

            SpriteRenderer renderer = gameobject[3 - i].GetComponent<SpriteRenderer>();
            renderer.sprite = GameManager.instance.GetNumberSprite(value);

            Score /= 10;
        }
    }
}
