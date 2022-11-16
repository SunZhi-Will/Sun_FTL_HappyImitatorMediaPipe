using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGeneration : MonoBehaviour
{
  [SerializeField] private GameObject _sphere;
  [SerializeField] private RectTransform _mainCanvas;
  //private float _sphereShootTime = 3;
  [SerializeField] private Vector2 _spherePoint;
  [SerializeField] private Vector2 _sphereShootSize;
  private GameObject _shootSphere = null;
  private void Start()
  {
    _sphereShootSize = new Vector2(_mainCanvas.rect.width / 2, _mainCanvas.rect.height / 2);
    _shootSphere = Instantiate(_sphere,
        new Vector3(0, 0, 0),
        Quaternion.identity);
    _shootSphere.transform.SetParent(_mainCanvas.transform);
    _shootSphere.transform.localPosition = new Vector3(Random.Range(-_sphereShootSize.x + _spherePoint.x, _sphereShootSize.x - _spherePoint.x),
            Random.Range(-_sphereShootSize.y + _spherePoint.y, _sphereShootSize.y - _spherePoint.y),
            transform.localPosition.z);
  }
  private void Update()
  {
    if (_shootSphere == null)
    {
      _sphereShootSize = new Vector2(_mainCanvas.rect.width / 2, _mainCanvas.rect.height / 2);
      _shootSphere = Instantiate(_sphere,
        new Vector3(0, 0, 0),
        Quaternion.identity);
      _shootSphere.transform.SetParent(_mainCanvas.transform);
      _shootSphere.transform.localPosition = new Vector3(Random.Range(-_sphereShootSize.x + _spherePoint.x, _sphereShootSize.x - _spherePoint.x),
          Random.Range(-_sphereShootSize.y + _spherePoint.y, _sphereShootSize.y - _spherePoint.y),
          transform.localPosition.z);
    }
  }

  public Vector3 UIToWorld(Vector3 uiPos)
  {
    float width = _mainCanvas.rect.width / 2; //UI一半的寬
    float height = _mainCanvas.rect.height / 2; //UI一半的高
    Vector3 screenPos = new Vector3(((uiPos.x / width) + 1f) / 2, ((uiPos.y / height) + 1f) / 2, 20); //須小心Z座標的位置
    return Camera.main.ViewportToWorldPoint(screenPos);
  }


}
