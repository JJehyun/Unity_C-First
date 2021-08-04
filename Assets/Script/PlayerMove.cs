using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{   public GameManager gameManager;
    public float jumpPower;
    public float maxSpeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator ani;
    CapsuleCollider2D ccapsuleCollider2D;
    // Start is called before the first frame update

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();   
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani =  GetComponent<Animator>();
        ccapsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    //STOP SPEED
    void Update(){
        //키보드에서 손을 땠을 때 속력을 줄여주는 code
        if(Input.GetButtonUp("Horizontal")){
           //백터의 단위를 구할 때 사용하는 함수 normalized, 방향을 구할 때 사용함!
            rigid.velocity = new Vector2(rigid.velocity.normalized.x*0.5f, rigid.velocity.y);
        }
        


        if(Input.GetButtonDown("Horizontal"))
        spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;


        //애니메이션 변환 walk , Adle
        if(Mathf.Abs(rigid.velocity.x) < 0.3)// 횡이동 단위값이 0이다 == 가만히 있는 상태
            ani.SetBool("isWalking", false);
        else
            ani.SetBool("isWalking", true);

        //jump 구현  
        if(Input.GetButtonDown("Jump") && !ani.GetBool("isJumping")){
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            ani.SetBool("isJumping",true);}
    }


    void FixedUpdate()  //물리 현상은FixedUpdate 사용
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed)//현재 속도가 maxSpeed 빠르면 오른쪽
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y );//velocity 현재 속도를 나타내는 함수
        else if(rigid.velocity.x < maxSpeed*(-1))//현재 속도가 maxSpeed 빠르면 왼쪽
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y );

        //Landing Platform 빔 쏘기 사용자가 빔을 확인하기 위함(위치, 아래 방향, 초록색)
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0));
        //빔을 아래 방향으로 Vector3.down , 1만큼의 크기로 쏜다. LayerMask.GetMask("platform") 해당하는 애들만 스캔한다.
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position ,Vector3.down,1,LayerMask.GetMask("platform"));
        //빔을 쏴서 충돌체에 맞았다면
        if(rayHit.collider != null){
            if(rayHit.distance < 1.2f){
                ani.SetBool("isJumping",false);}
        }
    }
        
        
    //Player 와 Enamy 충돌시 Player 일정 시간 무적 모드!
    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Enamy"){
                //Attack 적 죽이기 event 떨어지는 속력 = -1 , 적보다 위에 있을 때
                if(rigid.velocity.y < 0 && transform.position.y > other.transform.position.y){
                        OnAttack(other.transform);
                }else{
                OnDamaged(other.transform.position); // 충돌한 오브젝트의 위치
                Invoke("OffDamaged", 3);
                ani.SetTrigger("doDamaged");
                }
        }
    }

    // 아이템 먹기 event
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Item"){
            //Point 증가하기
            if(other.gameObject.layer == 10)//브론즈 코인 습득시 100점
                gameManager.stagePoint = gameManager.stagePoint + 100;
            else if(other.gameObject.layer == 11)//실버 코인 습득시 100점
                gameManager.stagePoint = gameManager.stagePoint + 150;
            else if(other.gameObject.layer == 12)//골드 코인 습득시 100점
                gameManager.stagePoint = gameManager.stagePoint + 200;
            //아이템 먹고 사라지기
            other.gameObject.SetActive(false);
        }
        else if(other.gameObject.tag == "Finish"){
            gameManager.NextStage();
        }
            
    }

    //공격하기
    void OnAttack(Transform enemy){
        rigid.AddForce(Vector2.up * 5,ForceMode2D.Impulse);
        //Point 증가하기
        gameManager.stagePoint = gameManager.stagePoint + 100;

        EnamyMove enemyMove = enemy.GetComponent<EnamyMove>(); //적 오브젝트에 적용된 script파일(EnamyMove)'
        enemyMove.OnDamaged();
    }


    //적에게 닿았을 때 PLAYER Layer 변경
    void OnDamaged(Vector2 targetPos){
        //(layer변경, 무적모드로 변경) 변경 할 layer 번호를 적어줌 , gameObject = 자신 게임오브젝트를 의미한다.
        gameObject.layer = 9; 

        //적에게 맞았을 때 player 색상 변경
        spriteRenderer.color = new Color(1,1,1,0.4f); 
        
        //적에게 충돌 시 Player 팅겨나가게 힘주기
        int dirt = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirt,1)*20, ForceMode2D.Impulse);

        //health Down
        gameManager.healthDown();
    }

    //적에게 닿고 무적모드 풀기
    void OffDamaged(){
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,1); 
    }



        //Player 죽었을 때
    public void Die(){
        //공격을 받았을 때 희리게 구현
        spriteRenderer.color = new Color(1,1,1,0.4f);
        //공격을 받았을 때 Y축으로 방향 전환(적이 y축으로 뒤집어짐)(filp는 앞뒤 바꿀때 사용)
        spriteRenderer.flipY = true;
        //바닥과 충돌을 없앰
        ccapsuleCollider2D.enabled = false;   //(boxCollider2D충돌)바닥과 충돌을 없애서 추락하게 시킴
        //적이 죽었을 때 점프 모션 추가
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse); //위로 점프하게 됨
    }


    public void VelocityZero(){
        rigid.velocity = Vector2.zero;
    }
}
