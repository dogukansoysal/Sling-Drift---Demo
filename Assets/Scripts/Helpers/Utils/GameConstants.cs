using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class GameConstants
{
    #region State Machines
    
    /// <summary>
    /// Game state keeps the current game state if it's playing, paused or in menu.
    /// </summary>
    public enum GameState
    {
        Playable,
        Unplayable,
        Paused,
        Menu
    }

    /// <summary>
    /// Input state keeps the current input type.
    /// </summary>
    public enum InputState
    {
        FirstTouch,
        Hold,
        Released,
        None
    }

    #endregion
    
    #region Enums
    
    public enum RoadType
    {
        Straight,
        RightTurn,
        LeftTurn,
        UTurnToRight,
        UTurnToLeft
    }
    
    public enum ColorName
    {
        Yellow,
        Green,
        Red,
        Blue
    }
    
    
    #endregion
    
    

    #region Variables
    
    
    
    
    
    /* Color Hex Codes Collected from Color Palette. */
    public static string GreenHexValue = "#45FF5F";
    public static string RedHexValue = "#FF4545";
    public static string YellowHexValue = "#F5FF3B";
    public static string GrayHexValue = "#B4B4B4";

    
    
    
    
    
    #endregion

    
    
    
    #region UI_Tweens
    
    public static void HandleScaleTween(Component scaleTween)
    {
        if (scaleTween)
        {
            scaleTween.GetComponent<RectTransform>().DOScale(scaleTween.GetComponent<RectTransform>().localScale * 1.2f, 0.75f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
        }
        //CompleteImage.GetComponent<RectTransform>().DOSizeDelta(CompleteImage.GetComponent<RectTransform>().sizeDelta * 1.05f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
    
    #endregion
    
    
    
    // Common static functions.
    #region Functions

    /// <summary>
    /// Returns a random value with inside an 
    /// </summary>
    /// <returns></returns>
    public static int GetRandomValueWithException(int minInterval, int maxInterval, int exceptionValue)
    {
        var randomValue = exceptionValue;
        if (minInterval >= maxInterval)
        {
            Debug.Log("Min interval cannot be greater then max interval.");
            return -1;
        } 
        
        while (randomValue == exceptionValue)
        {
            randomValue = Random.Range(minInterval, maxInterval);
        }

        return randomValue;
    }
    
    
    
    /// <summary>
    /// Exception handling for NaN (Not a Number)
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static bool Vector3IsNan(Vector3 vector)
    {
        // Not a Number check
        if(float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
            return true;
        else
        {
            return false;
        }
    }
    
    
    /// <summary>
    /// Returns the direction from first point to last point.
    /// </summary>
    /// <param name="firstPos"></param>
    /// <param name="lastPos"></param>
    /// <returns></returns>
    public static Vector3 CalculateDirection(Vector3 firstPos, Vector3 lastPos)
    {
        var heading = lastPos - firstPos;
        return heading / heading.magnitude; // Normalized direction
    }
    
    /// <summary>
    /// Wrap angle for 180+/- as signed.
    /// EX: 260degree = -100 degree
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float WrapAngle(float angle)
    {
        angle%=360;
        if(angle >180)
            return angle - 360;
 
        return angle;
    }
    #endregion
    
   
}
