using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float _speed;
    public GameObject _rowOne;
    public GameObject _rowTwo;

    public float _YRecycyclePoint = -24f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // update the rows
        _rowOne.transform.position = _rowOne.transform.position + Vector3.up * Time.deltaTime * _speed;
        _rowTwo.transform.position = _rowTwo.transform.position + Vector3.up * Time.deltaTime * _speed;

        if (_rowOne.transform.position.y <= _YRecycyclePoint)
        {
            _rowOne.transform.position = _rowTwo.transform.position + (Vector3.up * 24f);
        }

        if (_rowTwo.transform.position.y <= _YRecycyclePoint)
        {
            _rowTwo.transform.position = _rowOne.transform.position + (Vector3.up * 24f);
        }
    }
}
