using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public GameManager gameManager;
    private Rigidbody2D rigid;
    private Animator animator;
    private bool isGrounded;
    private Key nearbyKey; // 근처에 있는 키
    public Key key;       // 현재 소유한 키
    public bool getFinish; // Finish와 충돌 여부
    public bool onStair = false; // 계단과 충돌 여부

    // hasKey 속성
    public bool hasKey
    {
        get { return key != null; }
    }

    // KeyObject 속성
    public GameObject KeyObject
    {
        get { return key != null ? key.gameObject : null; }
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 플레이어 크기 1.5배로 키우기 (가로, 세로 모두 1.5배)
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    } // Start 메서드 닫힘 추가

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

        //Key를 가지고 있을 때 이동 속도 감소
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
                // 키 소유 해제
                key.Drop();
                key = null;
            }
            else if (nearbyKey != null && !nearbyKey.IsOwned())
            {
                // 키 소유
                key = nearbyKey;
                key.SetOwner(transform);
            }
        }
    }

    // 계단에 올라갔을 때 속도 조절
    private void OnCollisionStay2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Stair") && hasKey)
        {
            // 경사로의 법선 벡터를 가져옴
            Vector2 slopeNormal = other.contacts[0].normal;

            // 경사로의 각도를 계산
            float slopeAngle = Vector2.Angle(Vector2.up, slopeNormal);

            // 경사로의 각도가 45도 이상이면 이동을 막음
            if (slopeAngle > 44f)
            {
                Vector2 pushDirection = -slopeNormal.normalized;
                rigid.velocity = pushDirection * moveSpeed * 1f; // 약간 밀리는 속도 조절
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
            getFinish = true; // Finish와 충돌했음을 저장
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
            getFinish = false; // Finish에서 벗어났으면 초기화
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    // DropKey 메서드 추가
    public void DropKey()
    {
        if (key != null)
        {
            key.Drop(); // 키 소유 해제
            key = null;
        }
    }
}
