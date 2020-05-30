using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    
    public float Speed;
    public SpriteRenderer MyRend;
    [HideInInspector]
    public bool Shot;
    public Collider2D myCollider;
    public Rigidbody2D myRigidbody;

    private Vector3 InitialPos;
    private Vector3 LastPos;
    // Start is called before the first frame update
    
    void Start()
    {
        InitialPos = transform.position;
     //   myCollider = GetComponent<Collider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
       // ShowAnimation();
    }

    // Update is called once per frame
    void Update()
    {
     

     if (Shot) 
     {
            LastPos = transform.position;
            transform.position += Vector3.up * Speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.Linecast(LastPos, transform.position);
            if (hit.collider != null) 
            {
                if (hit.transform.tag == "Knife")
                {
                    LevelManager.Instance.WrongHit();
                    myRigidbody.bodyType = RigidbodyType2D.Dynamic;
                    myRigidbody.AddTorque(10, ForceMode2D.Impulse);

                }
                else
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                    transform.parent = hit.transform;
                    myCollider.enabled = true;
                    LevelManager.Instance.SuccessHit(myRigidbody);
                    Shot = false;
                }
            }

     }   
           
    }
    public void Shoot() 
    {
        Shot = true;
    }
    public void ShowAnimation() 
    {
        StartCoroutine("ShowKnife");
    }
    IEnumerator ShowKnife() 
    {
        yield return new WaitForEndOfFrame();
        float startTime = Time.time;
        float duration = 0.3f;
        Vector3 downPos = InitialPos - new Vector3(0, 0.5f, 0);
        while (Time.time < startTime + duration) 
        {
            float t = (Time.time - startTime) / duration;
            MyRend.color = new Color(MyRend.color.a,MyRend.color.g,MyRend.color.b,Mathf.Lerp(0,1,t));
            transform.position = Vector3.Lerp(downPos, InitialPos, t);
            yield return null;

        }
        MyRend.color = new Color(MyRend.color.a, MyRend.color.g, MyRend.color.b, 1);
        transform.position = InitialPos;
    }
}
