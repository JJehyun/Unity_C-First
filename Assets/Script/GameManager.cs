using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health ;
    public PlayerMove player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIStage;
    public GameObject RestartBtn;
    // Start is called before the first frame update
    void Awake()
    {   
        health = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //스테이지 이동
    public void NextStage(){
        //change Stage
        //stage 넘어가기

        Stages[stageIndex].SetActive(false); //현재 stage는 비활성화
        stageIndex++;
        Stages[stageIndex].SetActive(true); //다음 stage는 활성화
        PlayerReposition();

        //점수 계산기
        totalPoint = totalPoint + stagePoint;
        stagePoint = 0;

        //UI stage 문구 변경하기
        UIStage.text = "STAGE "+ (stageIndex + 1);

    }
    void OnTriggerEnter2D(Collider2D other) {
        //Player 떨어지면 점수 Down, Player 원래 위치로 복귀 시키기
        if(other.gameObject.tag == "Player"){
            //Player health down
            healthDown();
            //Player 리스폰
            if(health > 1){
            PlayerReposition(); }
        }
        
    }

    public void healthDown(){
        if(health > 1){
            UIhealth[(health-1)].color = new Color(1,0,0,0.4f);
            health--;}
        else{
            //Player 죽음 효과(데미지 횩허)
            player.Die();
            UIhealth[(health-1)].color = new Color(1,0,0,0.4f);

        }
    }

    void PlayerReposition(){
            player.transform.position = new Vector3(1.5f,10);
            //낙하속도를 0으로 만듦 별로 필요없음
            player.VelocityZero();
    }
}
