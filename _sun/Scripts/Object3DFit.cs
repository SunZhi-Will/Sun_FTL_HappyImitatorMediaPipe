using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

public class Object3DFit : MonoBehaviour
{
  [SerializeField] private float _hipHeightMeter = 0.9f;
  private IList<Landmark> _currentTarget;
  public PointAnnotation pointAnnotation;
  public Vector3 scale;
  public bool visualizeZ = false;
  protected void Start()
  {
    //base.Start();
    transform.localPosition = new Vector3(0, _hipHeightMeter * scale.y, 0);

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
  public void DrawNow(LandmarkList target)
  {
    DrawNow(target?.Landmark);
  }
  public void DrawNow(IList<Landmark> target)
  {

    _currentTarget = target;
    SyncNow();

  }
  protected void SyncNow()
  {
    Landmark landmark = new Landmark(_currentTarget[16]);
    landmark.X *= -1;
    Debug.Log("QQ");
    pointAnnotation.Draw(landmark, scale, visualizeZ);
    AnglePoint();
  }
  //16 20

  private void AnglePoint()
  {
    float xDiff = _currentTarget[16].X - _currentTarget[14].X;
    float yDiff = _currentTarget[16].Y - _currentTarget[14].Y;
    float ZDiff = _currentTarget[16].Z - _currentTarget[14].Z;

    float angle = (float)(Mathf.Atan2(yDiff, xDiff) * 180.0 / Mathf.PI);
    float angleX = (float)(Mathf.Atan2(ZDiff, yDiff) * 180.0 / Mathf.PI);
    float angleY = (float)(Mathf.Atan2(xDiff, ZDiff) * 180.0 / Mathf.PI);
    pointAnnotation.transform.eulerAngles = new Vector3(-angleX - 90, 0, 0);
  }
  private void Update()
  {
    //Debug.Log(_currentTarget[16]);
    SyncNow();
    //AnglePoint();
  }


}
