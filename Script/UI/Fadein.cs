using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fadein : MonoBehaviour
{
    public Image panel;

    float time = 0f;
    float alpha = 1f;

    bool fadein = true;

    public void Fade(bool FadeIn)
    {
        if (true == FadeIn)
            StartCoroutine(SetFadeIn());
        else
            StartCoroutine(SetFadeOut());
    }

    IEnumerator SetFadeIn()
    {
        Color color = panel.color;

        while (color.a < alpha)
        {
            time += Time.deltaTime / 1f;
            color.a = Mathf.Lerp(0, 1, time);

            panel.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator SetFadeOut()
    {
        Color color = panel.color;

        while (alpha < color.a)
        {
            time += Time.deltaTime / 1f;
            color.a = Mathf.Lerp(1, 0, time);

            panel.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }
}