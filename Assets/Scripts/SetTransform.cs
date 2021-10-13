using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransform : MonoBehaviour
{
    public Vector3 SetPosition
    {
        get => _setPosition;
        set
        {
            _setPosition = value;
        }
    }

    public Vector3 SetPositionNow
    {
        get => transform.localPosition;
        set
        {
            transform.localPosition = value;
            _setPosition = value;
            _setEndAnimation = false;
        }
    }

    public Vector3 SetPositionLast
    {
        get => _setPositionLast;
        set
        {
            _setPositionLast = value;
            _setEndAnimation = true;
        }
    }

    Vector3 _setPosition;
    Vector3 _setPositionLast;
    bool _setEndAnimation = false;

    private void Start()
    {
        GameManager.game.UpdatePositionCards += Position;
    }

    private void OnDestroy()
    {
        GameManager.game.UpdatePositionCards -= Position;
    }

    void Position()
    {
        float x = animParameter(transform.localPosition.x, _setPosition.x);
        float y = animParameter(transform.localPosition.y, _setPosition.y);
        float z = animParameter(transform.localPosition.z, _setPosition.z);
        transform.localPosition = new Vector3(x, y, z);

        if (_setEndAnimation && transform.localPosition == _setPosition)
            SetPositionNow = SetPositionLast;
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
