using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float initialSpeedCoefficient;
    [SerializeField] private float verticalSpeedCoefficient;
    [SerializeField] private float timeToSpeedUp;

    private Camera _playerCamera;
    private float _timeMoving;

    private void Start()
    {
        _playerCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Move(GetMovementDirection());
    }
    //Interp directions instead of Speed
    private void Move(Vector3 direction)
    {
        if (direction.magnitude > 0 && _timeMoving < timeToSpeedUp)
            _timeMoving += Time.deltaTime;
        else if (direction.magnitude == 0)
            _timeMoving -= Time.deltaTime;
        if(_timeMoving < 0)
            _timeMoving = 0;

        transform.position += Mathf.Lerp(initialSpeedCoefficient, 1, InterpolationFunction(_timeMoving / timeToSpeedUp)) * cameraSpeed * Time.deltaTime * direction;

    }

    private Vector3 GetMovementDirection()
    {
        var localMovementDirection = new Vector3(Input.GetAxis("HorizontalFront"), Input.GetAxis("Vertical"), verticalSpeedCoefficient * Input.GetAxis("HorizontalSide")).normalized;
        var lookRotation = transform.forward;
        lookRotation.y = 0;
        return Quaternion.LookRotation(lookRotation) * localMovementDirection;
    }

    private float InterpolationFunction(float t)
    {
        //return Mathf.Exp(-6 * Mathf.Pow(t - 1, 2));
        return t * t;
    }
}
