using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dancer : MonoBehaviour
{
  public Transform startPoint;
  public Transform endPoint;
  Vector3 startV3;
  Vector3 endV3;

  public void Start()
  {
    startV3 = startPoint.position;
    endV3 = endPoint.position;

    transform.position = startV3;

  }

  public void IntoCamera()
  {
    transform.DOMove(endV3, GameHandler.beatTime);
  }
}
