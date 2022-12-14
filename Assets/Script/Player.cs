using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 5.0f;
    public float jumpSpeed = 6.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rot = Vector3.zero;

    //获取枪械
    public GameObject gun;
    //最大射程
    public float raydis=400;
    //射击对象所在层
    public LayerMask firelayer;
    //武器射速
    float FileTime=0.1f;
    //弹夹剩余子弹
    int Bullet=30;
    //后备弹药
    public int Ammunition=300;
    //分数
    public int Score;
    //分数文本
    public Text ScoreText;
    //弹药显示
    public Text Bullettext;
    //宝箱数量
    public int BoxNumber;
    //宝箱显示
    public Text Boxtext;
    //生命值
    public int Health=100;
    //生命文本
    public Text HealthText;
    //游戏结束文本
    public Text GameoverText;
    //获取敌人的预制体
    public GameObject enemy;
    //重新开始按钮
    public Button Regame;
    //退出游戏
    public Button Exitgame;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //每隔三秒生成一个敌人
        InvokeRepeating("Spawn",1,1);
        //隐藏并固定鼠标
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //esc退出游戏
        if (Input.GetKey("escape")) Exit();
        //获取鼠标移动
        float mouseX=Input.GetAxis("Mouse X");
        float mouseY=Input.GetAxis("Mouse Y");
        rot.x-=1*mouseY;
        rot.y+=1.5f*mouseX;
        //控制相机旋转
        Camera.main.transform.eulerAngles=rot;
        //非子物体时保持相机在头顶
        //Camera.main.transform.position=transform.position+new Vector3(0,0.6f,0);
        //控制自身旋转
        transform.eulerAngles=new Vector3(0,rot.y,0);
        //加速及下蹲
        if(Input.GetKey(KeyCode.LeftShift))
            {
            speed=2;
            transform.localScale=new Vector3(1,0.5f,1);
            GetComponent<CapsuleCollider>().height=1;
            }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speed=9;
            transform.localScale=new Vector3(1,1,1);
            GetComponent<CapsuleCollider>().height=2;
        }
        else
        {
            speed=6;
            transform.localScale=new Vector3(1,1,1);
            GetComponent<CapsuleCollider>().height=2;
        }
        //获取屏幕中心
        Vector3 pos=new Vector3(Screen.width/2,Screen.height/2,0);
        //从屏幕中心发出射线
        Ray fireray=Camera.main.ScreenPointToRay(pos);
        //判断是否开枪并击中物体
        if(Input.GetMouseButton(0)&&(FileTime-=Time.deltaTime)<0&&Bullet>0)
        {
            GameObject.Find("ak74m").GetComponent<AudioSource>().Play();
            RaycastHit hitinfo;
            gun.GetComponent<Animator>().SetBool("Fire",true);
            if(Physics.Raycast(fireray,out hitinfo,raydis,firelayer))
            {
                hitinfo.transform.gameObject.GetComponent<Enemy>().EnemyDead();
                if (hitinfo.transform.gameObject.tag=="Enemy")
                Score += 1;
                ScoreText.text = "Score:" + Score.ToString();
            }
            if(--Bullet==0)
            {
                Invoke("ChangeBullets",3);
                FileTime=4;
                gun.GetComponent<Animator>().SetTrigger("Change");
            }
            //武器射速
            FileTime=0.1f;
            //更新弹药显示
            Bullettext.text=Bullet.ToString()+'/'+Ammunition.ToString();
        }
        else
        {
            //停止枪械抖动
            gun.GetComponent<Animator>().SetBool("Fire",false);
            //点射功能
            if(FileTime>0)FileTime-=Time.deltaTime;
        }
        //按R换弹
        if(Input.GetKey(KeyCode.R))
        {
            Invoke("ChangeBullets",3);
            FileTime=6;
            gun.GetComponent<Animator>().SetTrigger("Change");
        }
        if(Input.GetMouseButtonDown(1))
            gun.GetComponent<Animator>().SetTrigger("aim");
        if(Input.GetMouseButtonUp(1))
            gun.GetComponent<Animator>().SetTrigger("unaim");

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
            moveDirection *= speed;
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
    //生成敌人
    public void Spawn()
    {
        Instantiate(enemy,new Vector3(Random.Range(-12f,10f),0,Random.Range(-13f,10f)),Quaternion.identity);
    }
    //受伤掉血并判断是否嗝屁
    public void Hurt(int h)
    {
        Health-=h;
        HealthText.text=Health.ToString();
        if(Health<=0)
        {
            GameoverText.gameObject.SetActive(true);
            //释放鼠标
            Cursor.lockState=CursorLockMode.None;
            Regame.gameObject.SetActive(true);
            Exitgame.gameObject.SetActive(true);
            Time.timeScale=0;
        }
    }
    //捡宝箱
    public void GetBox()
    {
        if(++BoxNumber==10)
        {
            Boxtext.text="宝箱:"+BoxNumber.ToString();
            GameoverText.text="YouWin~\n召唤烟花！";
            GameoverText.color=new Color32(255,255,255,255);
            GameoverText.gameObject.SetActive(true);
            //释放鼠标
            Cursor.lockState=CursorLockMode.None;
            Regame.gameObject.SetActive(true);
            Exitgame.gameObject.SetActive(true);
            Time.timeScale=0;
        }
        Boxtext.text="宝箱:"+BoxNumber.ToString()+"/10";
    }
    //换弹功能
    void ChangeBullets()
    {
        if(Bullet<=0)
            Bullet=0;
        Ammunition+=Bullet;
        if(Ammunition>30)
        {
            Bullet=30;
            Ammunition-=30;
        }
        else
        {
            Bullet=Ammunition;
            Ammunition=0;
        }
        //更新弹药显示
        Bullettext.text=Bullet.ToString()+'/'+Ammunition.ToString();
    }
    //退出游戏
    public void Exit()
    {
        Application.Quit();
    }
    //重新开始
    public void Rega()
    {
        SceneManager.LoadScene(0);
        Time.timeScale=1;
    }
}
