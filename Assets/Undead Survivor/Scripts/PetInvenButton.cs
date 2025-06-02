using UnityEngine;

public class PetInvenButton : MonoBehaviour
{
    public GameObject thisPet;
    public GameObject detailPanel;
    public bool isSummon = false;

    public void openDetailOptions()//상세옵션이 hirearchy뷰에서 더 밑에있는 다른 버튼들에 가려지지 않게 하기 위해
    {
        if (thisPet == null) return;
        transform.SetAsLastSibling();//나 자신을 가장 밑으로 내린다.
        detailPanel.SetActive(true);
    }

    public void SummonThisPet()
    {
        isSummon = true;
        //플레이어의 반경 3 이내의 랜덤한 위치를 지정
        Vector2 randomOffset = Random.insideUnitCircle.normalized * 3f;
        Vector3 spawnPos = GameManager.instance.player.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        thisPet.transform.position = spawnPos;
        //위에서 지정한 랜덤 위치에 소환
        thisPet.SetActive(true);
    }

    public void ReturnThisPet()
    {
        isSummon = false;
        thisPet.SetActive(false);
    }

}
