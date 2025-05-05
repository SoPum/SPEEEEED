using UnityEngine;

public class Key : MonoBehaviour
{
    private Transform owner = null; // 현재 키를 소유한 플레이어
    private Vector3 offset = new Vector3(1f, 0.6f, 0f); // 키가 따라다니는 위치 (플레이어 머리 옆)
    private Vector3 originalScale; // 키의 원래 크기 저장
    private Vector3 lastKnownPosition; // 키가 따라다니던 마지막 위치 저장

    private void Start()
    {
        // 시작 시 키의 원래 크기를 저장
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (owner != null)
        {
            // 소유자 옆에서 따라다니도록 위치 설정
            float direction = Mathf.Sign(owner.localScale.x); // 플레이어 방향 확인
            Vector3 adjustedOffset = new Vector3(offset.x * direction, offset.y, offset.z); // 방향에 따른 오프셋
            lastKnownPosition = owner.position + adjustedOffset; // 마지막 위치를 업데이트

            transform.position = lastKnownPosition;

            // 키는 항상 원래 크기를 유지
            transform.localScale = new Vector3(originalScale.x * direction, originalScale.y, originalScale.z); // 방향에 따라 반전
        }
    }

    public void SetOwner(Transform newOwner)
    {
        owner = newOwner;
    }

    public bool IsOwned()
    {
        return owner != null;
    }

    public void Drop()
    {
        if (owner != null)
        {
            // 소유 해제 시 현재 위치 유지
            transform.position = lastKnownPosition; // 마지막 알려진 위치를 사용
            owner = null; // 소유자 초기화
        }

        // 키 크기를 원래대로 유지
        transform.localScale = originalScale;
    }
}
