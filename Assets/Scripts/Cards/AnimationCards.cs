using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCards : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    Cards card;
    Color SetColor;

    private void Start()
    {
        GameManager.game.UpdateAnimationCards += AnimationCard;

        card = GetComponent<Cards>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();//когда будет переделаны карты, поменять
    }

    private void OnDestroy()
    {
        GameManager.game.UpdateAnimationCards -= AnimationCard;
    }

    void AnimationCard()
    {
        if (GameManager.game.Stage == Stages.GreenCardsProperties || GameManager.game.Stage == Stages.PurpleCardsProperties || GameManager.game.Stage == Stages.Building)
        {
            if (card != null)
                if (card.availability)
                {
                    SetColor = new Color(1f, 1f, 1f, 1f);
                }
                else SetColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            SetColor = new Color(1f, 1f, 1f, 1f);
        }

        spriteRenderer.color = new Color(
            animParameter(spriteRenderer.color.r, SetColor.r, 1f),
            animParameter(spriteRenderer.color.g, SetColor.g, 1f),
            animParameter(spriteRenderer.color.b, SetColor.b, 1f),
            animParameter(spriteRenderer.color.a, SetColor.a, 1f));

    }

    float animParameter(float nowPar, float setPar, float speed = 3f)
    {
        if (nowPar > setPar)
        {
            nowPar -= Mathf.Max(speed, 3 * Mathf.Abs(nowPar - setPar)) * Time.deltaTime;
            if (nowPar < setPar)
                nowPar = setPar;
        }

        if (nowPar < setPar)
        {
            nowPar += Mathf.Max(speed, 3 * Mathf.Abs(nowPar - setPar)) * Time.deltaTime;
            if (nowPar > setPar)
                nowPar = setPar;
        }

        return nowPar;
    }
}
