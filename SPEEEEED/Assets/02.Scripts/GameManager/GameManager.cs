using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player player;
    public GameObject[] Stages;
    public TextMeshProUGUI UIStage;
    public GameObject[] tiles; // 모든 타일을 배열로 관리
    public GameObject targetTile; // 현재 미션 타일
    public GameObject missionText; // 미션 텍스트 UI (예: "Red Tile 위로 올라가세요")
    public GameObject jumpCounts; // Stage 9 현재 점프 수
    public GameObject coinCounts; // Stage 16 코인 수
    public TextMeshProUGUI tmp; // Mission UI
    public GameObject UIRestartBtn; // 재시작 버튼
    public AudioClip audioNextStage;
    AudioSource audioSource;


    public int stageIndex;
    private int previousStageIndex = -1; // 이전 stageIndex를 저장
    public int stagePoint;
    private float gameTimer = 0f; // 게임 시작 후 경과 시간
    private bool isGameOver = false; // 게임 종료 상태

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (stageIndex != previousStageIndex) // stageIndex가 변경되었을 때만 실행
        {
            if (stageIndex == 3)
            {
                AssignRandomTile();
            }
            
            if (stageIndex == 15)
            {
                GetCoinMission();
            }
            previousStageIndex = stageIndex; // 이전 stageIndex 업데이트
            MissionUI(); // UI 업데이트
            audioSource.clip = audioNextStage;
            audioSource.Play();
        }

        // 게임 오버가 되었을 때 Update()의 다른 로직이 실행되지 않도록 함
        if (isGameOver)
            return;

        // 타이머 증가
        gameTimer += Time.deltaTime;

        // 노래가 끝나면 게임 종료
        if (gameTimer >= 69f)
        {
            GameOver();
        }

        JumpCounts();
        CoinCounts();
    }
    public void NextStage()
    {
        // Change stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            // Game Clear
            // Player Control Lock
            Time.timeScale = 0;

            //Restart Button UI
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = "Game Clear!";
            ViewBtn();

        }

        stagePoint = 0;


    }

    // Player가 리스폰 존에 들어왔을 때 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Player Reposition
            PlayerReposition();
        }

    }

    public void PlayerReposition()
    {
        if (stageIndex == 10)
            player.transform.position = new Vector3(0, 10, 0);
        else
        {
            player.transform.position = new Vector3(0, 0, 0);
            player.VelocityZero();
        }
        
    }

    void AssignRandomTile()
    {
        // 랜덤으로 타일 선택
        int randomIndex = Random.Range(0, tiles.Length);
        targetTile = tiles[randomIndex];

        // 미션 텍스트 업데이트
        TextMeshProUGUI textComponent = missionText.GetComponent<TextMeshProUGUI>();
        textComponent.text = targetTile.name + "위로 올라가세요!";
    }

    void JumpCounts()
    {
        if (stageIndex == 8)
        {
            jumpCounts.SetActive(true);
            TextMeshProUGUI textComponent = jumpCounts.GetComponent<TextMeshProUGUI>();
            textComponent.text = $"{player.Stage9JumpCount}/20";
        }
        

        if (stageIndex != 8)
            jumpCounts.SetActive(false); // Stage 8이 아닐 때 UI 비활성화
    }

    public void GetCoinMission()
    {
        if (stagePoint >= 100)
            NextStage();
    }

    void CoinCounts()
    {
        if (stageIndex == 15)
        {
            coinCounts.SetActive(true);
            TextMeshProUGUI textComponent = coinCounts.GetComponent<TextMeshProUGUI>();
            textComponent.text = $"{stagePoint}/100";
        }


        if (stageIndex != 15)
            coinCounts.SetActive(false); // Stage 15이 아닐 때 UI 비활성화
    }


    void MissionUI()
    {
        if (stageIndex == 0)
        {
            tmp.text = "문까지 도달하세요!";
        }
        else if (stageIndex == 1)
        {
            tmp.text = "가시를 피해서 넘어가세요!";
        }
        else if (stageIndex == 2)
        {
            tmp.text = "낭떠러지를 피해 넘어가세요!";
        }
        else if (stageIndex == 4)
        {
            tmp.text = "위로 올라가세요!";
        }
        else if (stageIndex == 5)
        {
            tmp.text = "구멍 속으로 들어가세요!";
        }
        else if (stageIndex == 6)
        {
            tmp.text = "공룡을 피해 넘어가세요!";
        }
        else if (stageIndex == 7)
        {
            tmp.text = "구름 위로 올라가세요!";
        }
        else if (stageIndex == 8)
        {
            tmp.text = "점프를 20번 하세요!";
        }
        else if (stageIndex == 9)
        {
            tmp.text = "계단 위로 올라가세요!";
        }
        else if (stageIndex == 10)
        {
            tmp.text = "장애물을 피해 떨어지세요!";
        }
        else if (stageIndex == 11)
        {
            tmp.text = "내려가세요!";
        }
        else if (stageIndex == 12)
        {
            tmp.text = "넘어가세요!";
        }
        else if (stageIndex == 13)
        {
            tmp.text = "문까지 도달하세요!";
        }
        else if (stageIndex == 14)
        {
            tmp.text = "더블 점프로 넘어가세요!";
        }
        else if (stageIndex == 15)
        {
            tmp.text = "100 골드 이상 획득하세요!";
        }
        else if (stageIndex == 16)
        {
            tmp.text = "계속 점프하세요!";
        }
        else if (stageIndex == 17)
        {
            tmp.text = "꼭대기까지 올라가세요!";
        }
        else if (stageIndex == 18)
        {
            tmp.text = "조심히 점프해서 넘어가세요!";
        }
        else if (stageIndex == 19)
        {
            tmp.text = "Last!";
        }
    }

    void ViewBtn()
    {
        UIRestartBtn.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    private void GameOver()
    {
        isGameOver = true; // 게임 종료 상태
        Time.timeScale = 0; // 게임 멈춤
        ViewBtn();
    }
}
