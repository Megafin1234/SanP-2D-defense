using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; // 탐지 범위
    public RaycastHit2D[] targets; // 탐지된 타겟
    public Transform nearestTarget; // 가장 가까운 타겟

    void FixedUpdate()
    {
        // 모든 Collider2D를 탐지
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero);
        nearestTarget = GetNearest(); // 가장 가까운 적 탐지
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = Mathf.Infinity; // 무한으로 초기화

        foreach (RaycastHit2D target in targets)
        {
            if (target.transform.CompareTag("Enemy")) // 태그를 통해 적 확인
            {
                Vector3 myPos = transform.position;
                Vector3 targetPos = target.transform.position;
                float curDiff = Vector3.Distance(myPos, targetPos);

                if (curDiff < diff)
                {
                    diff = curDiff;
                    result = target.transform; // 가장 가까운 적 저장
                }
            }
        }
        return result;
    }
}
