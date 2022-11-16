using System;
using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GestureTrackingDirector : MonoBehaviour
{

  public TextMeshProUGUI m_GestureText;
  public TextMeshProUGUI m_NpcGestureText;
  public TextMeshProUGUI m_WinOrLoseText;

  [Flags]
  public enum FingerState
  {
    Closed = 0,
    ThumbOpen = 1,
    IndexOpen = 2,
    MiddleOpen = 4,
    RingOpen = 8,
    PinkyOpen = 16,
  }
  public enum Mora
  {
    Nome,
    Scissors,
    Stone,
    Cloth
  }
  public Mora _moras = 0;
  public Mora _npcMoras = 0;


  public HandTrackingGraph handTrackingGraph;
  private void Awake()
  {
    OnStateChanged += HandleOnStateChanged;

  }
  private void Start()
  {
    StartCoroutine(Guessing());
  }
  public void Gool()
  {
    Debug.Log("Gool");
    handTrackingGraph.OnHandLandmarksOutput += OnHandLandmarksOutputs;

  }

  public void OnHandLandmarksOutputs(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
  {
    if (eventArgs.value != null)
    {
      landmarkLists = eventArgs.value;
      Process(landmarkLists[0]);
    }

  }

  public delegate void HandStateEvent(FingerState previousState, FingerState currentState);
  public event HandStateEvent OnStateChanged = (p, c) => { };
  GestureAnalyzer.MeaningfulGesture gesture = 0;
  private void HandleOnStateChanged(FingerState previousState, FingerState currentState)
  {

    gesture = currentState.Analyze();

    Debug.Log(gesture);
  }

  private void Update()
  {
    switch (gesture)
    {
      case GestureAnalyzer.MeaningfulGesture.Five:
        _moras = Mora.Cloth;

        break;
      case GestureAnalyzer.MeaningfulGesture.Two:
        _moras = Mora.Scissors;

        break;
      case GestureAnalyzer.MeaningfulGesture.Hold:
        _moras = Mora.Stone;

        break;
      default:
        break;
    }
    m_GestureText.text = _moras.ToString();
  }

  public IEnumerator Guessing()
  {
    int i = 3;
    while (true)
    {
      yield return new WaitForSeconds(1f);
      i--;
      m_WinOrLoseText.text = i + "";

      if (i <= 0)
      {
        i = 3;
        _npcMoras = (Mora)UnityEngine.Random.Range(1, 4);
        m_NpcGestureText.text = "Player " + _moras.ToString() + " : Enemy " + _npcMoras.ToString();
        if (_moras > _npcMoras)
        {
          if ((byte)_moras == 3 && (byte)_npcMoras == 1)
          {
            m_WinOrLoseText.text = "Lose";
            m_WinOrLoseText.color = UnityEngine.Color.red;
          }
          else
          {
            m_WinOrLoseText.text = "Win";
            m_WinOrLoseText.color = UnityEngine.Color.yellow;
          }
        }
        else if (_moras < _npcMoras)
        {
          if ((byte)_moras == 1 && (byte)_npcMoras == 3)
          {
            m_WinOrLoseText.text = "Win";
            m_WinOrLoseText.color = UnityEngine.Color.yellow;
          }
          else
          {
            m_WinOrLoseText.text = "Lose";
            m_WinOrLoseText.color = UnityEngine.Color.red;
          }
        }
        else
        {
          m_WinOrLoseText.text = "A Draw";
          m_WinOrLoseText.color = UnityEngine.Color.white;
        }
      }
    }


  }




  public IList<NormalizedLandmarkList> landmarkLists { get; set; }

  FingerState m_FingerState;

  public void Process(NormalizedLandmarkList normalizedLandmarkList)
  {
    //NormalizedLandmarkList normalizedLandmarkList = landmarkList[0];



    FingerState fingerState = FingerState.Closed;

    float pseudoFixKeyPoint = normalizedLandmarkList.Landmark[2].X;
    if ((normalizedLandmarkList.Landmark[0].X > normalizedLandmarkList.Landmark[1].X && normalizedLandmarkList.Landmark[3].X < pseudoFixKeyPoint && normalizedLandmarkList.Landmark[4].X < pseudoFixKeyPoint) ||
    (normalizedLandmarkList.Landmark[0].X < normalizedLandmarkList.Landmark[1].X && normalizedLandmarkList.Landmark[3].X > pseudoFixKeyPoint && normalizedLandmarkList.Landmark[4].X > pseudoFixKeyPoint))
    {
      fingerState |= FingerState.ThumbOpen;
    }
    pseudoFixKeyPoint = normalizedLandmarkList.Landmark[6].Y;
    if (normalizedLandmarkList.Landmark[7].Y < pseudoFixKeyPoint && normalizedLandmarkList.Landmark[8].Y < pseudoFixKeyPoint)
    {
      fingerState |= FingerState.IndexOpen;
    }
    pseudoFixKeyPoint = normalizedLandmarkList.Landmark[10].Y;
    if (normalizedLandmarkList.Landmark[11].Y < pseudoFixKeyPoint && normalizedLandmarkList.Landmark[12].Y < pseudoFixKeyPoint)
    {
      fingerState |= FingerState.MiddleOpen;
    }
    pseudoFixKeyPoint = normalizedLandmarkList.Landmark[14].Y;
    if (normalizedLandmarkList.Landmark[15].Y < pseudoFixKeyPoint && normalizedLandmarkList.Landmark[16].Y < pseudoFixKeyPoint)
    {
      fingerState |= FingerState.RingOpen;
    }
    pseudoFixKeyPoint = normalizedLandmarkList.Landmark[18].Y;
    if (normalizedLandmarkList.Landmark[19].Y < pseudoFixKeyPoint && normalizedLandmarkList.Landmark[20].Y < pseudoFixKeyPoint)
    {
      fingerState |= FingerState.PinkyOpen;
    }


    if (m_FingerState != fingerState)
    {
      OnStateChanged(m_FingerState, fingerState);
      m_FingerState = fingerState;
    }
  }
}
