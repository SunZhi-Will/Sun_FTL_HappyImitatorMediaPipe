using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
  // [SerializeField] private float _hipHeightMeter = 0.9f;
  private IList<Landmark> _currentTarget;
  private NormalizedRect _poseRoi;

  // public PointAnnotation pointAnnotation;
  // public Vector3 scale;
  // public bool visualizeZ = false;

  public enum FootStatus
  {
    Stand,
    RightFoot,
    LeftFoot,
    Feet
  }
  private Vector3 _cubeMove;

  protected void Start()
  {
    //base.Start();
    //transform.localPosition = new Vector3(0, _hipHeightMeter * scale.y, 0);
  }
  public void DrawLater(LandmarkList target)
  {
    DrawLater(target?.Landmark);

  }
  public void DrawLater(IList<Landmark> target)
  {
    if (target != null || _currentTarget != null)
    {
      _currentTarget = target;
    }
  }


  public void DrawLater(NormalizedRect target)
  {
    _poseRoi = target;
    Debug.Log(_poseRoi);
  }
  public void DrawNow(LandmarkList target, NormalizedRect poseRoi)
  {
    DrawNow(target?.Landmark, poseRoi);
    SyncNow();
  }
  public void DrawNow(IList<Landmark> target, NormalizedRect poseRoi)
  {

    _currentTarget = target;
    _poseRoi = poseRoi;
    SyncNow();

  }

  protected void SyncNow()
  {
    try
    {
      //HandJudgment();
    }
    catch (System.Exception)
    {
    }
  }

  public GameHandler.ActionState actionState;
  /// <summary>
  /// 舉起左右手
  /// </summary>
  public GameHandler.ActionState HandJudgment()
  {
    try
    {
      float rightHand = _currentTarget[12].Y - _currentTarget[14].Y;
      float leftHand = _currentTarget[11].Y - _currentTarget[13].Y;

      if (rightHand < 0.1f && rightHand > -0.1f)
      {
        Debug.Log("右手");
        return GameHandler.ActionState.RIGHT;
      }
      else if (leftHand < 0.1f && leftHand > -0.1f)
      {
        Debug.Log("左手");
        return GameHandler.ActionState.LEFT;
      }
      else if (rightHand > 0 && leftHand > 0)
      {
        Debug.Log("雙手舉高");
        return GameHandler.ActionState.UP;
      }
      else if (rightHand < 0 && leftHand < 0)
      {
        Debug.Log("雙手朝地");
        return GameHandler.ActionState.DOWN;
      }
    }
    catch (NullReferenceException)
    {
      //return GameHandler.ActionState.EMPTY;
    }
    return GameHandler.ActionState.EMPTY;
  }
  public bool OnIsPlayerRange()
  {
    return _poseRoi != null;
  }
  public bool OnPlayerRange()
  {
    return _poseRoi.Width < 1.5f && _poseRoi.Height < 1f && _poseRoi.XCenter < 0.6f && _poseRoi.XCenter > 0.4f;
  }


  private void Update()
  {
    SyncNow();
  }


}
