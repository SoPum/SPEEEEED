using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    private int jumpCount;           // 현재 점프 횟수를 추적
    public int maxJumpCount;     // 최대 점프 횟수 설정
    public bool getFinish;
    private bool isDamaged = false;
    public int Stage9JumpCount = 0; // Stage 9 JumpCount

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }
    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            if (gameManager.stageIndex == 8)
            {
                if (Input.GetButtonDown("Jump"))
                    Stage9JumpCount += 1;
                if (Stage9JumpCount == 20)
                    gameManager.NextStage();
            }
            rigid.velocity = new Vector2(rigid.velocity.x, 0); // 이전 점프 속도 초기화
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            jumpCount++; // 점프 횟수 증가
            
        }


        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0, rigid.velocity.y);
        }

        //Direction of Sprite
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput == -1;
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) == 0)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

        //Finish에 닿으면 다음 스테이지로 이동
        if (getFinish)
        {
            gameManager.NextStage();
            getFinish = false;
        }

        // 데미지를 입었을 때 이동 제한
        if (isDamaged)
        {
            rigid.velocity = Vector2.zero;
            return;
        }

        if (gameManager.stageIndex == 15)
        {
            gameManager.GetCoinMission();

        }
        else if (gameManager.stageIndex == 5)
        {
            maxSpeed = 30;

        }
        else if (gameManager.stageIndex != 5)
            maxSpeed = 7;

        //JumpCounts
        if (gameManager.stageIndex == 14)
            maxJumpCount = 2;
        else if (gameManager.stageIndex == 16)
            maxJumpCount = 99999;
        else if (gameManager.stageIndex == 10)
            maxJumpCount = 0;
        else
            maxJumpCount = 1;
        


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");
        if (gameManager.stageIndex != 13)
        {
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        else if (gameManager.stageIndex == 13)
        {
            rigid.AddForce(Vector2.right * -h, ForceMode2D.Impulse);
        }

        //Max Speed
        if (rigid.velocity.x > maxSpeed) //Right Max Spped
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Spped
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        if (gameManager.stageIndex == 3)
        {
            CheckTileUnderPlayer();
        }

    }
    //Landing Ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            anim.SetBool("isJumping", false);
            jumpCount = 0;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack((collision.transform));
            }
            //Damaged
            else
            {
                OnDamaged(collision.transform.position);
            }
        }

        if (collision.gameObject.tag == "Spike")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint += 10;
        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
    void OnDamaged(Vector2 targetPos)
    {
        isDamaged = true;
        rigid.velocity = Vector2.zero;

        //Change Layer (Immortal Active)
        gameObject.layer = 6;

        // 투명하게 보임
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*5, ForceMode2D.Impulse);
        

        Invoke("OffDamaged", 1);

        Invoke("Respawn", 1);
    }

    void OffDamaged()
    {
        isDamaged = false;
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        
    }

    // 리스폰
    void Respawn()
    {
        gameManager.PlayerReposition();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            getFinish = true; // Finish에 닿았음을 저장
        }

        // Coin 획득
        if (collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 1;
            else if(isSilver)
                gameManager.stagePoint += 5;
            else if(isGold)
                gameManager.stagePoint += 10;

            //Deactive Item
            collision.gameObject.SetActive(false);
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void CheckTileUnderPlayer()
    {
        // 플레이어의 발 아래로 Raycast 발사
        Vector2 rayOrigin = rigid.position;
        Vector2 rayDirection = Vector2.down;
        float rayLength = 1.0f; // 플레이어 크기에 따라 조정

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, LayerMask.GetMask("Tile"));

        if (hit.collider != null)
        {
            // 충돌한 오브젝트의 태그 확인
            string tileTag = hit.collider.tag;

            // targetTile과 태그가 일치하면 스테이지 이동
            if (tileTag == gameManager.targetTile.tag)
            {
                gameManager.NextStage();
            }
        }
    }
}
