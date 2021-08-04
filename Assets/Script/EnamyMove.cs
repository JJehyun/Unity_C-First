using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnamyMove : MonoBehaviour
{       Animator anim;
        Rigidbody2D rigid;
        SpriteRenderer spriteRenderer;
        BoxCollider2D boxccollider2D;
        public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxccollider2D = GetComponent<BoxCollider2D>();
        Invoke("Think",2);
    }


    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove,rigid.velocity.y);  //y축은 항상 지금 오브젝트의 y축의 속력을 넣어준다.
                                                            //왼쪽으로 움직이게 설정 자동으로
    
    
        //지형체크 레이저 빔으로 지형이 있는지 확인하기!
        Vector2 frontvec = new Vector2(rigid.position.x+ nextMove, rigid.position.y);
        Debug.DrawRay(frontvec, Vector3.down, new Color(0,1,0));  //레이저 확인하는 용도
        RaycastHit2D rayHit = Physics2D.Raycast(frontvec ,Vector3.down,1,LayerMask.GetMask("platform"));
        if(rayHit.collider == null){ //rayHit.collider 앞에 낭 떨어지 일때
            Turn();
        }

    }
    void Think(){
        nextMove = Random.Range(-1 ,2); //최소 ~ 최대 범위의 랜덤 수 생성 -1, 0 ,1 3중 하나가 Random 들어가게됨
                                        //-1,0,1은 방향값이다-1왼쪽 , 0멈춤 ,1오른쪽
        
        Invoke("Think",3);  //함수에 딜레이를 주는 함수 Invoke 3초 후에 Think라는 함수를 호출함

        anim.SetInteger("WalkSpeed", nextMove);  //int 형 애니메이션 추가

        if(nextMove != 0)// 오브젝트가 움직이고 있을 때 앞, 뒤로 돌기 code
        spriteRenderer.flipX = nextMove == 1;    //오른쪽 
        
    }

    void Turn(){
            nextMove = nextMove * -1;  //낭 떨어지일 때 방향을 전환 하기
            spriteRenderer.flipX = nextMove == 1;
            CancelInvoke();          //Invoke 초기화
            Invoke("Think",3);
    }


    //외부에서 접근해야하기 때문에 public이다. Enamy가 데미지 받은 것을 구현
    public void OnDamaged(){
        //공격을 받았을 때 희리게 구현
        spriteRenderer.color = new Color(1,1,1,0.4f);
        //공격을 받았을 때 Y축으로 방향 전환(적이 y축으로 뒤집어짐)(filp는 앞뒤 바꿀때 사용)
        spriteRenderer.flipY = true;
        //바닥과 충돌을 없앰
        boxccollider2D.enabled = false;   //(boxCollider2D충돌)바닥과 충돌을 없애서 추락하게 시킴
        //적이 죽었을 때 점프 모션 추가
        rigid.AddForce(Vector2.up*5, ForceMode2D.Impulse); //위로 점프하게 됨
        //3초 뒤 Enamy오브젝트 비활성화 하기
        Invoke("DeActive",3);
    }
    void DeActive(){
        gameObject.SetActive(false);
    }
}
