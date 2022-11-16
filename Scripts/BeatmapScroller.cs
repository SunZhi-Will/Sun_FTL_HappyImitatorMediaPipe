using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatmapScroller : MonoBehaviour
{
  public float scrollOffset;
  [SerializeField] AnimationCurve beatSpeedCurve;
  public IEnumerator Movement(float time)
  {
    float speed = scrollOffset / time;
    while (true)
    {
      // transform.Translate(Vector3.left * speed * Time.deltaTime);
      transform.position += Vector3.left * speed * Time.deltaTime;
      yield return null;
    }
  }

  public void Move(float time)
  {
    //Debug.Log(time);
    transform.DOLocalMoveX(transform.localPosition.x - scrollOffset, 1).SetEase(Ease.OutExpo);
  }
}
