using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_1000 : MonoBehaviour
{
    protected Dictionary<State, IState> states = new Dictionary<State, IState>();
    protected IState currentState;
    protected Transform bodySpriteTransform => transform.GetChild(0);
    protected float walkBobTimer = 0f;
    protected float bodySpriteOriginY = 0f; // 角色原始位置的y坐标

    public virtual void Start()
    {
        states.Add(State.Idle, new IdleState_1000(this));

        currentState = states[State.Idle];
        currentState.OnEnter();
    }

    public virtual void Update()
    {
        currentState.OnUpdate();
    }
    public virtual void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[newState];
        currentState.OnEnter();
    }
    // 旋转角色朝向目标
    public virtual void RotateTowardsTarget(Vector2 direction, bool leftRight = true)
    {
        if (direction.x > 0)
        {
            bodySpriteTransform.GetComponent<SpriteRenderer>().flipX = leftRight;
        }
        else if (direction.x < 0)
        {
            bodySpriteTransform.GetComponent<SpriteRenderer>().flipX = !leftRight;
        }
    }
    // 新增：上下跳动方法
    public virtual void PlayWalkBob(float walkBobAmplitude = 0.05f, float walkBobFrequency = 30f)
    {
        walkBobTimer += Time.deltaTime * walkBobFrequency;
        float offsetY = Mathf.Sin(walkBobTimer) * walkBobAmplitude;
        Vector3 pos = bodySpriteTransform.localPosition;
        pos.y = bodySpriteOriginY + offsetY;
        bodySpriteTransform.localPosition = pos;
    }
    public void PlayHitFlash(float flashTime = 0.5f)
    {
        StartCoroutine(HitFlashCoroutine(flashTime));
    }

    public virtual IEnumerator HitFlashCoroutine(float flashTime)
    {
        SpriteRenderer sr = bodySpriteTransform.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;
        sr.color = new Color(1f, 0f, 0f, 1f); // 红色
        yield return new WaitForSeconds(flashTime);
        sr.color = new Color(1f, 1f, 1f, 1f); // 恢复原色
    }
}
