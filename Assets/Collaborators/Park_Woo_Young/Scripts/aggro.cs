using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : MonoBehaviour
{
    // ��׷� �����ϱ� 
    // ���͸� �����ų� ���Ͻ� ��׷μ�ġ�� �ö󰡰� �� �� ��׷μ�ġ�� �Ѱų� ���������� ���Ͱ� �ش� �÷��̾� �����ϰ� �ϱ�
    [SerializeField] float maxAggro; // ���ݹ��� �� �ִ� ��׷�
    public float currentAggro;       // ���� ��׷� ��ġ(�����ֱ�)

    private void Start()
    {
        currentAggro = 0;
    }
    private void Aggroo()
    {
        if (currentAggro > maxAggro)
        {

        }
    }


}
