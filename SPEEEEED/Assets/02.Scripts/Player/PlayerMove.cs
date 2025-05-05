using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public GameManager gameManager;
    private Rigidbody2D rigid;
    private Animator animator;
    private bool isGrounded;
    private Key nearbyKey; // ��ó�� �ִ� Ű
    public Key key;       // ���� ������ Ű
    public bool getFinish; // Finish�� �浹 ����
    public bool onStair = false; // ��ܰ� �浹 ����

    // hasKey �Ӽ�
    public bool hasKey
    {
        get { return key != null; }
    }

    // KeyObject �Ӽ�
    public GameObject KeyObject
    {
        get { return key != null ? key.gameObject : null; }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // �÷��̾� ũ�� 1.5��� Ű��� (����, ���� ��� 1.5��)
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    } // Start �޼��� ���� �߰�

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleKeyInteraction();
        if (getFinish && Input.GetKeyDown(KeyCode.Space) && hasKey)
        {
            gameManager.NextStage();
            getFinish = false;
        }
    }



    private void HandleMovement()
    {

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1f;

        //Key�� ������ ���� �� �̵� �ӵ� ����
        if (hasKey)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -0.6f;
            if (Input.GetKey(KeyCode.RightArrow)) moveInput = 0.6f;
        }


        rigid.velocity = new Vector2(moveInput * moveSpeed, rigid.velocity.y);

        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * 1.5f, 1.5f, 1f);
        }

        animator.SetBool("IsWalking", moveInput != 0);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && hasKey != true)
        {
            rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("IsJumping", true);
        }
    }

    private void HandleKeyInteraction()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (key != null)
            {
                // Ű ���� ����
                key.Drop();
                key = null;
            }
            else if (nearbyKey != null && !nearbyKey.IsOwned())
            {
                // Ű ����
                key = nearbyKey;
                key.SetOwner(transform);
            }
        }
    }

    // ��ܿ� �ö��� �� �ӵ� ����
    private void OnCollisionStay2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Stair") && hasKey)
        {
            // ������ ���� ���͸� ������
            Vector2 slopeNormal = other.contacts[0].normal;

            // ������ ������ ���
            float slopeAngle = Vector2.Angle(Vector2.up, slopeNormal);

            // ������ ������ 45�� �̻��̸� �̵��� ����
            if (slopeAngle > 44f)
            {
                Vector2 pushDirection = -slopeNormal.normalized;
                rigid.velocity = pushDirection * moveSpeed * 1f; // �ణ �и��� �ӵ� ����
                return;
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            nearbyKey = other.GetComponent<Key>();
        }
        if (other.CompareTag("Finish"))
        {
            getFinish = true; // Finish�� �浹������ ����
        }
        if (other.CompareTag("Stair"))
        {
            onStair = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Key"))
        {
            nearbyKey = null;
        }
        if (other.CompareTag("Finish"))
        {
            getFinish = false; // Finish���� ������� �ʱ�ȭ
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    // DropKey �޼��� �߰�
    public void DropKey()
    {
        if (key != null)
        {
            key.Drop(); // Ű ���� ����
            key = null;
        }
    }
}
