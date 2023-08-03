using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggro : MonoBehaviour
{
    // 어그로 구현하기 
    // 몬스터를 때리거나 죽일시 어그로수치가 올라가게 한 뒤 어그로수치를 넘거나 동일해지면 몬스터가 해당 플레이어 공격하게 하기
    [SerializeField] float maxAggro; // 공격받을 수 있는 어그로
    public float currentAggro;       // 현재 어그로 수치(보여주기)

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
