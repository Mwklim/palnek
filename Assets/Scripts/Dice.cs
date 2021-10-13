using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] int _dice = 0;//заданное значение

    float _rollSpeed;//начальная скорость кручения
    float _angle;//заданный угол движения

    int isMoving;//отслеживание количества Coroutine
    int turnover = 0;//количество оборотов
    int turnoverMin = 9;//минимальное количество оборотов
    float timeExpectation = 3.6f;//время ожидания для удаления

    int type = 0;//тип выкатывания кубиков
    public int dice
    {
        set
        {
            _dice = value;
            _rollSpeed = Random.Range(30f, 50f);

            type = Random.Range(0, 2);

            _angle = Random.Range(70f, 85f);
            transform.rotation = Quaternion.Euler(0, 0, _angle - 45f * type);

            turnoverMin = Random.Range(7, 11);
            //4,2 (-45f)
        }
    }

    private void Update()
    {
        timeExpectation -= Time.deltaTime;

        if (timeExpectation <= 0)
        {
            Destroy(gameObject);
        }
        else if (timeExpectation <= 1)
        {
            //запустить анимацию исчезания
        }

        if (isMoving > 0) return;

        if (CheckDice() != _dice || turnover < 9)
        {
            _rollSpeed -= 3f; if (_rollSpeed < 6f) _rollSpeed = 6f;

            if (type == 0)
            {
                if (turnover == 0)
                {
                    if (_dice == 3 || _dice == 1 || ((_dice == 6 || _dice == 5) && Random.Range(0, 2) > 0))
                    {
                        if (Random.Range(0, 2) > 0)
                        {
                            Assemble(new Vector3(1f * Mathf.Sin(_angle * Mathf.Deg2Rad), -1f * Mathf.Cos(_angle * Mathf.Deg2Rad), 0));// Vector3.down);
                        }
                        else
                        {
                            Assemble(new Vector3(-1f * Mathf.Sin(_angle * Mathf.Deg2Rad), 1f * Mathf.Cos(_angle * Mathf.Deg2Rad), 0));//Vector3.up);
                        }
                    }
                }
                else
                {
                    Assemble(new Vector3(1f * Mathf.Cos(_angle * Mathf.Deg2Rad), 1f * Mathf.Sin(_angle * Mathf.Deg2Rad), 0));//Vector3.right);
                }
            }

            if (type == 1)
                if (turnover == 0)
                {         
                    if (_dice == 6 || _dice == 5)
                    {
                        if (Random.Range(0, 2) > 0)
                            Assemble(new Vector3(1f * Mathf.Cos((_angle - 45f) * Mathf.Deg2Rad), 1f * Mathf.Sin((_angle - 45f) * Mathf.Deg2Rad), 0));//Vector3.right);
                        else
                            Assemble(new Vector3(-1f * Mathf.Cos((_angle - 45f) * Mathf.Deg2Rad), -1f * Mathf.Sin((_angle - 45f) * Mathf.Deg2Rad), 0));//Vector3.right);
                    }

                    if (_dice == 3 || _dice == 1)
                    {
                        if (Random.Range(0, 2) > 0)
                            Assemble(new Vector3(-1f * Mathf.Sin((_angle - 45f) * Mathf.Deg2Rad), 1f * Mathf.Cos((_angle - 45f) * Mathf.Deg2Rad), 0));//Vector3.right);
                        else
                            Assemble(new Vector3(1f * Mathf.Sin((_angle - 45f) * Mathf.Deg2Rad), -1f * Mathf.Cos((_angle - 45f) * Mathf.Deg2Rad), 0));//Vector3.right);
                    }
                }
                else
                {
                    Assemble(new Vector3(1f * Mathf.Cos(_angle * Mathf.Deg2Rad), 1f * Mathf.Sin(_angle * Mathf.Deg2Rad), 0));//Vector3.right);
                }


            turnover++;
        }

    }

    int CheckDice()
    {
        int diceCount = 0;
        float minDot = 0.7f;

        if (Vector3.Dot(transform.forward, Vector3.back) > minDot)
            diceCount = 2;
        if (Vector3.Dot(-transform.forward, Vector3.back) > minDot)
            diceCount = 4;
        if (Vector3.Dot(transform.up, Vector3.back) > minDot)
            diceCount = 3;
        if (Vector3.Dot(-transform.up, Vector3.back) > minDot)
            diceCount = 1;
        if (Vector3.Dot(transform.right, Vector3.back) > minDot)
            diceCount = 5;
        if (Vector3.Dot(-transform.right, Vector3.back) > minDot)
            diceCount = 6;

        return diceCount;
    }


    void Assemble(Vector3 dir, float angle = 90f)
    {
        Vector3 anchor = transform.position + (Vector3.forward + dir) * 0.5f;
        Vector3 axis = Vector3.Cross(Vector3.back, dir);
        StartCoroutine(Roll(anchor, axis, angle));
    }

    IEnumerator Roll(Vector3 anchor, Vector3 axis, float angle = 90f)
    {
        isMoving++;
        for (int i = 0; i < Mathf.Round(angle / _rollSpeed); i++)
        {
            transform.RotateAround(anchor, axis, angle / Mathf.Round(angle / _rollSpeed));
            yield return new WaitForSeconds(0.01f);
        }

        isMoving--;
    }

    // 1 - (), -90, -90     or (), 90, 90
    // 2 - 0, 180, ()       or 180, 0, ()
    // 3 - (), -90, 90      or (), 90, -90
    // 4 - 0, 0, ()         or 180, 180, ()
    // 5 - (), -90, 180     or (), 90, 0
    // 6 - (), -90, 0       or (), 90, 180
}
