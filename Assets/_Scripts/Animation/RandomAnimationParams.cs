using UnityEngine;

public class RandomAnimationParams : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private string _animationOffsetParamName = "AnimationOffset";
    [SerializeField] private string _animationSpeedParamName = "AnimationSpeed";

    [Header("Speed")]
    [SerializeField] private float _maxSpeedMultiplier = 1.5f;
    [SerializeField] private float _minSpeedMultiplier = 0.8f;

    private void Start()
    {
        float offset = Random.Range(0f, 1f);
        float speed = Random.Range(_minSpeedMultiplier, _maxSpeedMultiplier);

        _anim.SetFloat(_animationOffsetParamName, offset);
        _anim.SetFloat(_animationSpeedParamName, speed);
    }
}

