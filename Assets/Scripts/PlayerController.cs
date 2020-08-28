using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{

    public float SpeedForce;
    //public float DriftFactor = 0.8f;
    public GameObject PoleInZone;
    public GameObject ConnectedPole;
    
    private Rigidbody _rigidbody;

    private Tweener currentTween;
    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }

    
    private void Update()
    {
        if(GameManager.Instance.GameState != GameConstants.GameState.Playable) return;

        if ( InputManager.Instance.InputState == GameConstants.InputState.Hold || InputManager.Instance.InputState == GameConstants.InputState.FirstTouch )
        {
            if ( PoleInZone != null )
            {
                if ( PoleInZone.GetComponent<FixedJoint>().connectedBody != _rigidbody )
                {
                    ConnectToPole();
                }
            }
        }
        else if( InputManager.Instance.InputState == GameConstants.InputState.Released )
        {
            ReleasePole();
        }
    }

    
    void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameConstants.GameState.Playable)
        {
            Stop();
            return;
        }

        Move();
    }

    /// <summary>
    /// Movement handler.
    /// </summary>
    private void Move()
    {
        _rigidbody.velocity = new Vector3(transform.forward.x * SpeedForce, _rigidbody.velocity.y, transform.forward.z * SpeedForce);
    }

    /// <summary>
    /// Set velocity and angular velocity to zero.
    /// </summary>
    public void Stop()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void ResetAngularVelocity()
    {
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void ConnectToPole()
    {
        currentTween.Kill();
        ConnectedPole = PoleInZone;
        
        PoleInZone.GetComponent<FixedJoint>().connectedBody = _rigidbody;
    }
    
    
    private void ReleasePole()
    {
        ConnectedPole.GetComponent<FixedJoint>().connectedBody = null;
        ResetAngularVelocity();
        
        if (currentTween.IsActive())
            currentTween.Pause();

        var angle = Vector3.Angle(transform.forward,
            ConnectedPole.transform.parent.parent.GetComponent<Road>().EndPosition.forward);

        Debug.Log(angle);
        
        if (angle < 60)
        {
            currentTween = transform.DOLookAt(transform.position + ConnectedPole.transform.parent.parent.GetComponent<Road>().EndPosition.forward * 1000, angle/75).SetEase(Ease.InOutBack, 4);
        }

        ConnectedPole = null;
    }

    public IEnumerator FinishAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0, transform.DORotate(new Vector3(0, 135, -10), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine));
            
        sequence.Append(transform.DORotate(new Vector3(0, 0, 2.5f), 0.10f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutSine));
            
        sequence.Append(transform.DORotate(new Vector3(0, 0, 7.5f), 1.9f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutElastic));
            
        sequence.Insert(0,transform.DOMoveZ(transform.position.z + 15f, 1.25f).SetEase(Ease.OutSine));
            
        while (sequence.active)
        {
            yield break;
        }
    }

    private Vector3 CalculateClosestDirection()
    {
        var closestAngle = float.MaxValue;
        var closestVector = Vector3.zero;

        Debug.Log(Vector3.Angle(transform.forward, Vector3.forward) + " | " +
                                                   Vector3.Angle(-transform.forward, Vector3.back) + " | " + 
                                                                                     Vector3.Angle(transform.right, Vector3.right) + " | " + 
                                                                                                                    Vector3.Angle(-transform.right, Vector3.left));
        if (Vector3.Angle(transform.forward, Vector3.forward) < closestAngle)
        {
            closestAngle = Vector3.Angle(transform.forward, Vector3.forward);
            closestVector = Vector3.forward;
        }
        
        if (Vector3.Angle(transform.forward, Vector3.back) < closestAngle)
        {
            closestAngle = Vector3.Angle(transform.forward, Vector3.back);
            closestVector = Vector3.back;
        }
        
        if (Vector3.Angle(transform.forward, Vector3.right) < closestAngle)
        {
            closestAngle = Vector3.Angle(transform.forward, Vector3.right);
            closestVector = Vector3.right;
        }
        
        if (Vector3.Angle(transform.forward, Vector3.left) < closestAngle)
        {
            closestAngle = Vector3.Angle(transform.forward, Vector3.left);
            closestVector = Vector3.left;
        }

        return closestVector;
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.Instance.GameState != GameConstants.GameState.Playable) return;
        
        if (other.CompareTag("Pole"))
        {
            PoleInZone = RoadManager.Instance.GetClosestPole(transform);
            //CurrentPole = other.transform.parent.GetComponent<Road>().PolePosition.GetChild(0).gameObject;
        }
        else if (other.CompareTag("Obstacle"))
        {
            GameManager.Instance.Fail();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(GameManager.Instance.GameState != GameConstants.GameState.Playable) return;

        if (other.CompareTag("Pole"))
        {
            PoleInZone = null;
            
        }
    }
}
