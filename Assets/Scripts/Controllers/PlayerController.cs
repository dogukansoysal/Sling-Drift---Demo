using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Player controller script which contains the mechanics of the game, the user's actions and the fundamentals of Gameplay.
/// </summary>
public class PlayerController : MonoBehaviour
{

    public float SpeedForce;
    //public float DriftFactor = 0.8f;     //TODO: Implement drift factor depending on speed.
    public GameObject PoleInZone;
    public GameObject ConnectedPole;
    
    
    /* Component definition */
    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    private Tweener currentTween;
    
    
    
    private void Awake()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _lineRenderer = transform.GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
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
    /// Reset angular velocity to avoid the drag to the outside of the road.
    /// </summary>
    private void ResetAngularVelocity()
    {
        _rigidbody.angularVelocity = Vector3.zero;
    }
    
    
    
    /// <summary>
    /// Create a connection between pole
    /// </summary>
    private void ConnectToPole()
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();
        
        ConnectedPole = PoleInZone;
        
        PoleInZone.GetComponent<FixedJoint>().connectedBody = _rigidbody;

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, transform.InverseTransformPoint(ConnectedPole.transform.position) + Vector3.up);
    }
    
    
    
    /// <summary>
    /// Release the connection between pole
    /// </summary>
    private void ReleasePole()
    {
        if(!ConnectedPole) return;
        
        ConnectedPole.GetComponent<FixedJoint>().connectedBody = null;
        ResetAngularVelocity();
        


        var angle = Vector3.Angle(transform.forward,
            ConnectedPole.transform.parent.parent.GetComponent<Road>().EndPosition.forward);

        //Debug.Log(angle);
        //transform.forward = ConnectedPole.transform.parent.parent.GetComponent<Road>().EndPosition.forward;     //TEST
        
        if (angle < 60)
        {
            if (currentTween != null && currentTween.IsActive())
                currentTween.Kill();
            
            currentTween = transform.DOLookAt(transform.position + ConnectedPole.transform.parent.parent.GetComponent<Road>().EndPosition.forward * 1000, angle/75).SetEase(Ease.InOutBack, 4);
        }
        
        ConnectedPole = null;
        
        _lineRenderer.positionCount = 0;
    }

    
    
    public IEnumerator FinishAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0, transform.DORotate(new Vector3(0, 135, -10), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine));
            
        sequence.Append(transform.DORotate(new Vector3(0, 0, 2.5f), 0.10f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutSine));
            
        sequence.Append(transform.DORotate(new Vector3(0, 0, 7.5f), 1.9f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutElastic));
            
        sequence.Insert(0,transform.DOMove(transform.position + (transform.forward * 5f), 1.25f).SetEase(Ease.OutSine));
            
        while (sequence.active)
        {
            yield break;
        }
    }

    
    
    /// <summary>
    /// Obsolete Calculation of direction function.
    /// DIRECTIONS: LEFT, RIGHT, FORWARD, BACK
    /// </summary>
    /// <returns>Vector of closest rotation direction.</returns>
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
            ReleasePole();
            GameManager.Instance.Fail();
        }
        else if (other.CompareTag("Finish"))
        {
            ReleasePole();
            StartCoroutine(FinishAnimation());
            GameManager.Instance.Success();
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
