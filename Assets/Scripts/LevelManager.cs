using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private bool canPlay = true;
    public static LevelManager Instance;
    public int KnifesCount;
    [SerializeField]
    private DishController Target;

    [SerializeField]
    private GameObject KnifesGO;
    [SerializeField]
    private Transform KnifesSpwanPoint;
    private KnifeController CurrKnife;
    private int CurrKnifesCount;
    private int hits;
    [SerializeField]
    private List<Image> knifeicon;

    private List<Rigidbody2D> CurrHitKnifes = new List<Rigidbody2D>();
    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GenereteLevel();
    }
    public void GenereteLevel() 
    {
        KnifesCount = Random.Range(1,8);
        for (int i =0; i < knifeicon.Count; i++) 
        {
            if (i >= KnifesCount) 
            {
                knifeicon[i].color = new Color(knifeicon[i].color.r,knifeicon[i].color.g,knifeicon[i].color.b,0);   
            }
        }
        int stickingKnife = Random.Range(1, 5);
        float MaxAngle = 360 / (float)stickingKnife;
        float LastAngle = 0;

        for (int i = 0; i < stickingKnife; i++) 
        {
            float angle = LastAngle + Random.Range(20,MaxAngle) * Mathf.Deg2Rad;
            LastAngle = angle;
            Vector3 pos = Target.transform.position + new Vector3(Mathf.Sin(angle),Mathf.Cos(angle),0) * 1.25f;
            GameObject knife = Instantiate(KnifesGO,pos,Quaternion.identity);
            knife.transform.up = Target.transform.position - knife.transform.position;
            knife.transform.parent = Target.transform;
            KnifeController knifeBehaviour = knife.GetComponent<KnifeController>();
            knifeBehaviour.myCollider.enabled = true;
            CurrHitKnifes.Add(knifeBehaviour.myRigidbody);
        }

        CurrKnifesCount = KnifesCount;
        SpwanKnifes();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canPlay)
            return;
        if (Input.GetMouseButtonDown(0) &&  CurrKnife != null && CurrKnifesCount>-1)
        {
            ShootKnife();

        }

    }
    void ShootKnife() 
    {
        CurrKnife.Shoot();
        CurrKnifesCount--;
        knifeicon[CurrKnifesCount].color = new Color(0,0,0,0.5f);
        if (CurrKnifesCount > 0)
        {
            SpwanKnifes();
        }
        else
            CurrKnife = null; 
    }
    void SpwanKnifes() 
    {
        GameObject Knifes = Instantiate(KnifesGO, KnifesSpwanPoint.position, Quaternion.identity);
        CurrKnife = Knifes.GetComponent<KnifeController>();
        CurrKnife.ShowAnimation();
    }

    public void SuccessHit(Rigidbody2D knife) 
    {
        Target.GotHit();
        hits++;
        CurrHitKnifes.Add(knife);
        GameManager.Instance.AddScore();

        if (hits == KnifesCount) 
        {
            Win();
        }
    }
    public void WrongHit() 
    {

        Lose();
    }
    void Win() 
    {
        
        for (int i = 0; i< CurrHitKnifes.Count; i++) 
        {
            Rigidbody2D currKnife = CurrHitKnifes[i];
            currKnife.transform.parent = null;
            currKnife.bodyType = RigidbodyType2D.Dynamic;
            Vector3 forceDirection = (currKnife.transform.position - Target.transform.position).normalized * 4;

            if (i == CurrHitKnifes.Count - 1)
                forceDirection.y = 10;

            currKnife.AddForceAtPosition(forceDirection, Target.transform.position,ForceMode2D.Impulse);
            currKnife.AddTorque(4,ForceMode2D.Impulse);


            Destroy(currKnife,3);
        }

        Target.DestroyMe();
        GameManager.Instance.ReloadScene();
    }
    void Lose() 
    {
        canPlay = false;
        GameManager.Instance.ReloadScene();
        GameManager.Instance.ResetScore();
    }
}
