using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity
{
  public class HandWorldLandmarkListAnnotationController : AnnotationController<HandLandmarkListAnnotation>
  {
    [SerializeField] private float _hipHeightMeter = 0.9f;
    [SerializeField] private Vector3 _scale = new Vector3(100, 100, 100);
    [SerializeField] private bool _visualizeZ = true;

    private IList<Landmark> _currentTarget;

    protected override void Start()
    {
      base.Start();
      transform.localPosition = new Vector3(0, _hipHeightMeter * _scale.y, 0);

    }

    public void DrawNow(IList<Landmark> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawNow(IList<LandmarkList> target)
    {

      DrawNow(target);
    }

    public void DrawLater(IList<Landmark> target)
    {
      Debug.Log(target);
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    public void DrawLater(IList<LandmarkList> target)
    {

      if (target != null)
        DrawLater(target[0].Landmark);
    }

    protected override void SyncNow()
    {
      isStale = false;
      //annotation.Draw(_currentTarget, _scale, _visualizeZ);
    }
  }
}
