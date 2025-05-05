using UnityEngine;

public class Key : MonoBehaviour
{
    private Transform owner = null; // ���� Ű�� ������ �÷��̾�
    private Vector3 offset = new Vector3(1f, 0.6f, 0f); // Ű�� ����ٴϴ� ��ġ (�÷��̾� �Ӹ� ��)
    private Vector3 originalScale; // Ű�� ���� ũ�� ����
    private Vector3 lastKnownPosition; // Ű�� ����ٴϴ� ������ ��ġ ����

    private void Start()
    {
        // ���� �� Ű�� ���� ũ�⸦ ����
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (owner != null)
        {
            // ������ ������ ����ٴϵ��� ��ġ ����
            float direction = Mathf.Sign(owner.localScale.x); // �÷��̾� ���� Ȯ��
            Vector3 adjustedOffset = new Vector3(offset.x * direction, offset.y, offset.z); // ���⿡ ���� ������
            lastKnownPosition = owner.position + adjustedOffset; // ������ ��ġ�� ������Ʈ

            transform.position = lastKnownPosition;

            // Ű�� �׻� ���� ũ�⸦ ����
            transform.localScale = new Vector3(originalScale.x * direction, originalScale.y, originalScale.z); // ���⿡ ���� ����
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
            // ���� ���� �� ���� ��ġ ����
            transform.position = lastKnownPosition; // ������ �˷��� ��ġ�� ���
            owner = null; // ������ �ʱ�ȭ
        }

        // Ű ũ�⸦ ������� ����
        transform.localScale = originalScale;
    }
}
