using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
  private void OnCollisionEnter(Collision other)
  {
    Destroy(this.gameObject);
  }
}
