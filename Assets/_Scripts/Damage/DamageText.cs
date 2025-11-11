using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _disappearTime;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _xOffsetRange;

    public string Text { get => _text.text; set => _text.text = value; }

    public void StartTextPopUp()
        => RpcStartTextPopUp();

    private void RpcStartTextPopUp()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        StartCoroutine(DamageTextPopUp());
    }

    private IEnumerator DamageTextPopUp()
    {
        float elapsedTime = _disappearTime;
        Vector3 moveDirection = Vector3.up + new Vector3(Random.Range(-_xOffsetRange, _xOffsetRange), 0, 0);
        float speed = Random.Range(_minSpeed, _maxSpeed);

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;

            transform.Translate(moveDirection * speed * Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}
