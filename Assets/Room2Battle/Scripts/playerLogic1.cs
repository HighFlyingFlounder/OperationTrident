using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerLogic1 : MonoBehaviour
{
    protected Camera _camera;
    protected CharacterController _charController;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    private float _rotationX = 0;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    public float speed = 5.0f;

    [SerializeField]
    private GameObject fireballPrefab;

    void Start()
    {
        //获取相机句柄
        _camera = Camera.main;
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
        _charController = GetComponent<CharacterController>();
    }

    void handleInput()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
        _camera.transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);

        _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
        _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
        float _rotationY = _camera.transform.localEulerAngles.y;

        _camera.transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0.0f);

        //玩家移动     
        float h = Input.GetAxis("Horizontal") * speed;
        float v = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(h, 0, v);
        float y = _camera.transform.rotation.eulerAngles.y;
        movement = Quaternion.Euler(0, y, 0) * movement;

        _charController.transform.Translate(movement * Time.deltaTime * speed, Space.World);
    }

    public void Update()
    {
        handleInput();
        if (Input.GetMouseButtonDown(0))
        {
            GameObject _fireBall;
            _fireBall = Instantiate(fireballPrefab) as GameObject;
            _fireBall.transform.position = _camera.transform.TransformPoint(Vector3.forward * 1.5f);
            _fireBall.transform.rotation = _camera.transform.rotation;
            StartCoroutine(BulletFly(_fireBall));
        }
    }

    private IEnumerator BulletFly(GameObject bullet)
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(bullet);
    }
}
