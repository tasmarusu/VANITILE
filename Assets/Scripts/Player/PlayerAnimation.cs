using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }


    /// <summary>
    /// アニメーションパラメーターのBool値を入れる
    /// </summary>
    /// <param name="name"> セットするアニメーション名 </param>
    /// <param name="value"> 真か偽か </param>
    public void SetBoolParameter(ParameterNames name, bool value)
        => animator.SetBool(name.ToString(), value);


    /// <summary>
    /// アニメーションパラメーターのTriggerを発生させる
    /// </summary>
    /// <param name="name"> 発生するアニメーション名 </param>
    public void SetTriggerParameter(ParameterNames name)
        => animator.SetTrigger(name.ToString());



    public enum ParameterNames
    {
        wall,
        run,
        ground,
        kick,
        move,
    }
}
