using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Flash;

    [SerializeField]
    private GameObject Destroted;
    [SerializeField]
    private List<Rigidbody2D> DestroyedPieces;

    private Vector3 InitialPos;
    private float roundRotaionSpeed;
    private float roundStartTime;
    private float roundDuration;
    // Start is called before the first frame update
    void Start()
    {
        InitialPos = transform.position;
        NewRound();
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time - roundStartTime) / roundDuration;
        t = 1 - t;
        float CurrRoundRotationSpeed = roundRotaionSpeed * t;
        transform.Rotate(new Vector3(0, 0, CurrRoundRotationSpeed) * Time.deltaTime);
        if (t < 0.05f)
            NewRound();
    }
    void NewRound() 
    {
        roundStartTime = Time.time;
        float roundpower = Random.Range(0 , 1f);
        roundRotaionSpeed = -150 - 150 * roundpower;
        roundDuration = 5 + 5 * roundpower;
    }
    public void GotHit() 
    {
        StopCoroutine("Pushing");
        StopCoroutine("Flashing");

        StartCoroutine("Pushing");
        StartCoroutine("Flashing");
    }
    IEnumerator Pushing() 
    {
        float startTime = Time.time;
        float duration = 0.025f;
        Vector3 upPos = InitialPos + new Vector3(0, 0.1f, 0);
        while (Time.time < startTime + duration) 
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(InitialPos, upPos, t);
            yield return null; 
        }
        startTime = Time.time;
        duration = 0.2f;
        while (Time.time < startTime + duration) 
        {
            float t = (Time.time - startTime) / duration;
            t = 1 - Mathf.Abs(Mathf.Pow(t - 1, 2));
            transform.position = Vector3.Lerp(upPos, InitialPos, t);
            yield return null;
        }
        transform.position = InitialPos;
    }
    IEnumerator Flashing()
    {
        float startTime = Time.time;
        float duration = 0.025f;
        Vector3 upPos = InitialPos + new Vector3(0, 0.1f, 0);
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            Flash.color = new Color(Flash.color.r, Flash.color.g, Flash.color.b, Mathf.Lerp(0, 0.5f, t));
            yield return null;
        }
        startTime = Time.time;
        duration = 0.2f;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            t = 1 - Mathf.Abs(Mathf.Pow(t - 1, 2));
            Flash.color = new Color(Flash.color.r, Flash.color.g, Flash.color.b, Mathf.Lerp(0.5f, 0, t));

            yield return null;
        }
        Flash.color = new Color(Flash.color.r, Flash.color.g, Flash.color.b, 0);

    }
    public void DestroyMe()
    {
        Destroted.transform.parent = null;
        Destroted.SetActive(true);
        for (int i = 0; i<DestroyedPieces.Count;i++) 
        {
            Vector3 forcedirection = (DestroyedPieces[i].transform.position - transform.position).normalized*4;
            forcedirection.y = forcedirection.y < 0 ? forcedirection.y * 1 : forcedirection.y;
            DestroyedPieces[i].AddForceAtPosition(forcedirection,transform.position,ForceMode2D.Impulse);
            DestroyedPieces[i].AddTorque(4,ForceMode2D.Impulse);
            Destroy(DestroyedPieces[i].gameObject,3);
;        }
        Destroy(gameObject);
    }
}
