using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boundary;
    [SerializeField] private float speed;
    [SerializeField] private float timer;
    [SerializeField] private float restingTime;
    [SerializeField] private string idleAnimationTag, movingAnimationTag, randomBehaviourTag;
    private Animator anim;
    private Vector2 wayPoint;
    private WaitForSeconds _restDelay;
    private float range = 0.5f;
    private float timeRemaining;
    private bool isResting, stopWander;


    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play(movingAnimationTag, -1, 0f);

        timeRemaining = timer;
        _restDelay = new WaitForSeconds(restingTime);
    }

    void OnEnable()
    {
        isResting = stopWander = false;
        SetNewDestination();
    }

    void OnDisable()
    {
        ExitWander();
    }

    void Update()
    {
        if (!isResting && !stopWander)
        {
            transform.position = Vector2.MoveTowards(transform.position, wayPoint, speed * Time.deltaTime);

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }

            if ((Vector2.Distance(transform.position, wayPoint) < range) || timeRemaining <= 0)
            {
                timeRemaining = timer;
                SetNewDestination();
                
                StartCoroutine(Rest());
            }
        }
    }

    void SetNewDestination()
    {
        wayPoint = new Vector2(Random.Range(boundary.bounds.min.x, boundary.bounds.max.x), Random.Range(boundary.bounds.min.y, boundary.bounds.max.y));
    }

    IEnumerator Rest()
    {
        isResting = true;

        anim.Play(idleAnimationTag, -1, 0f);
        yield return _restDelay;
        yield return RandomBehaviour();

        isResting = false;

        if (transform.position.x < wayPoint.x)
            transform.localScale = new Vector2(-1, transform.localScale.y);
        else
            transform.localScale = new Vector2(1, transform.localScale.y);

        anim.Play(movingAnimationTag, -1, 0f);
    }
    
    IEnumerator RandomBehaviour()
    {
        if(Random.value > 0.6)
        {
            anim.Play(randomBehaviourTag, -1, 0f);
            yield return WaitForCache.GetWaitForSecond(1f);
            
            anim.Play(idleAnimationTag, -1, 0f);
            yield return WaitForCache.GetWaitForSecond(1f);
        }
        yield break;
    }

    public void ExitWander()
    {
        stopWander = true;
        StopAllCoroutines();
        transform.localScale = new Vector2(1, transform.localScale.y);
    }

    public void PlayAnimation(string animTag)
    {
        anim.Play(animTag, -1, 0f);
    }
}
