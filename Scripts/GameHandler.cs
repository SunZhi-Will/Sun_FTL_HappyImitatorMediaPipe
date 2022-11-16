using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
  const int UP = 1, RIGHT = 2, DOWN = 3, LEFT = 4, EMPTY = 0;

  public enum ActionState
  {
    EMPTY,
    UP,
    RIGHT,
    DOWN,
    LEFT
  }
  public enum ControlMode
  {
    Mouse,
    Keyboard,
    ImageRecognition
  }
  public ControlMode controlMode;
  public Beatmap beatmap;
  List<int> beats = new List<int>();
  List<HitStatus> hasHits = new List<HitStatus>();
  [Range(0f, 10f)] public float minMouseAccel;
  [Range(0f, 1f)] public float judgementMultiply; // 寬鬆程度，你好鬆
  float judgementTime = 0;
  public static float beatTime;
  [SerializeField] int beatmapProgress = 0;
  [SerializeField] float measureTime = 0;
  float timeLastFrame;
  bool hasInput = false;  // 避免多次回傳
  public Text text;
  int maxScore = 0;
  int score = 0;
  int dancerCount = 1;

  public GameObject[] arrows;
  public Transform beatlineUI;
  List<GameObject> arrowBeatObjects = new List<GameObject>();

  public AudioSource audioSource;

  public Animator aniMaster;
  public Transform characterParent;
  List<Animator> animators = new List<Animator>();

  public Text debugText;
  List<string> inputDebugString = new List<string>();
  public int inputDebugLength = 8;

  [SerializeField] private GameObject m_homeScreen;
  public delegate ActionState ControlDetection();
  public ControlDetection imageRecognition;

  public delegate bool StartTriggering();
  public delegate float StartBox();
  public StartTriggering startBox;

  public StartTriggering isPlayerRange;

  [SerializeField] private CharacterControl characterControl;
  private bool m_gameStart = false;
  private bool m_gameEnd = false;
  private float m_startCountdown = 3;
  private int m_emptyCount = 0;
  [SerializeField] private Text m_startCountdownText;
  [SerializeField] private Text m_EndCountdownText;
  [SerializeField] private GameObject m_endUI;
  [SerializeField] private Animator m_fail;
  private void Awake()
  {
    imageRecognition = characterControl.HandJudgment;
    startBox = characterControl.OnPlayerRange;
    isPlayerRange = characterControl.OnIsPlayerRange;
    Screen.sleepTimeout = SleepTimeout.NeverSleep;
  }
  private void Init()
  {
    for (int i = 0; i < beatlineUI.childCount; i++)
    {
      Destroy(beatlineUI.GetChild(i).gameObject);
    }
    for (int i = 0; i < characterParent.childCount; i++)
    {
      if (characterParent.GetChild(i) && characterParent.GetChild(i).GetComponent<Dancer>())
      {
        characterParent.GetChild(i).GetComponent<Dancer>().Start();
      }

    }
    m_homeScreen.SetActive(true);

    beatlineUI.localPosition = new Vector3(0, beatlineUI.localPosition.y, beatlineUI.localPosition.z);
    beatmapProgress = 0;
    beats = new List<int>();
    hasHits = new List<HitStatus>();
    arrowBeatObjects = new List<GameObject>();
    dancerCount = 1;
    score = 0;
    m_emptyCount = 0;
    m_startCountdown = 3;
    m_gameStart = false;
    m_gameEnd = false;
  }
  private void Start()
  {
    beatTime = 60f / beatmap.bpm;
    judgementTime = judgementMultiply * beatTime;
    audioSource.clip = beatmap.clip;
    foreach (string s in beatmap.beatlines)
    {
      // char[] nums = s.ToCharArray();
      for (int i = 0; i < 2; ++i)
      {
        foreach (char c in s.ToCharArray())
        {
          int temp = (int)System.Char.GetNumericValue(c);
          beats.Add(temp);
          hasHits.Add(HitStatus.NotHit);
          if (temp != EMPTY)
            maxScore++;
          GameObject arrow = default;
          switch (temp)
          {
            case UP:
              arrow = Instantiate(arrows[UP], beatlineUI);
              break;

            case DOWN:
              arrow = Instantiate(arrows[DOWN], beatlineUI);
              break;

            case LEFT:
              arrow = Instantiate(arrows[LEFT], beatlineUI);
              break;

            case RIGHT:
              arrow = Instantiate(arrows[RIGHT], beatlineUI);
              break;

            case EMPTY:
              arrowBeatObjects.Add(Instantiate(arrows[EMPTY], beatlineUI));
              m_emptyCount++;
              continue;

          }

          arrow.GetComponent<Image>().color = i == 0 ? Color.white : new Color(1, 1, 1, .2f);
          arrowBeatObjects.Add(arrow);
        }

      }
    }
    // StartCoroutine(beatlineUI.GetComponent<BeatmapScroller>().Movement(60f / beatmap.bpm));

    foreach (Transform t in characterParent)
      animators.Add(t.GetComponent<Animator>());
  }

  public void GameStart()
  {
    m_homeScreen.SetActive(false);
    m_gameStart = true;
    audioSource.Play();
    StartCoroutine(GameUpdate());
  }

  IEnumerator GameUpdate()
  {
    if (beats[beatmapProgress] != 0)
      aniMaster.SetTrigger(IntToAniString(beats[beatmapProgress]));
    while (beatmapProgress < beats.Count)
    {
      if (measureTime >= beatTime)
      {
        measureTime -= beatTime;
        beatmapProgress++;

        if (beatmapProgress >= beats.Count)
          break;

        if (beats[beatmapProgress] != 0)
          aniMaster.SetTrigger(IntToAniString(beats[beatmapProgress]));
        beatlineUI.GetComponent<BeatmapScroller>().Move(beatTime);
      }

      if (beatmapProgress / beatmap.measure % 2 != 0)
      {
        JudgeBeat();
      }



      if (!audioSource.isPlaying)
      {
        audioSource.Play();
        timeLastFrame = 0;
      }

      float deltaTime = audioSource.time - timeLastFrame;
      timeLastFrame = audioSource.time;
      measureTime += deltaTime;
      text.text = score.ToString();

      DebugTextUpdate();

      yield return null;
    }
    timeLastFrame = 0;
    m_gameEnd = true;
    audioSource.Stop();
    m_endUI.SetActive(true);
  }

  public int InputCheck()
  {
    float x, y; // for mouse and touch mode

#if UNITY_EDITOR
    if (controlMode == ControlMode.Mouse)
    {
      if (hasInput)
      {
        if (Input.GetMouseButtonUp(0))
        {
          hasInput = false;
        }
        // return -1;
      }
      if (!Input.GetMouseButton(0))
      {
        // print("沒有按壓屏幕");
        return -1;
      }


      x = Input.GetAxis("Mouse X");
      y = Input.GetAxis("Mouse Y");
      print($"{x} , {y}");
      if (Mathf.Abs(x) > Mathf.Abs(y))
      {
        if (Mathf.Abs(x) < minMouseAccel)
        {
          return 0;
        }
        else
        {
          hasInput = true;
          AddInputString(IntToAniString(x > 0 ? RIGHT : LEFT));
          return x > 0 ? RIGHT : LEFT;
        }
      }
      else
      {
        if (Mathf.Abs(y) < minMouseAccel)
        {
          return 0;
        }
        else
        {
          hasInput = true;
          AddInputString(IntToAniString(y > 0 ? UP : DOWN));
          return y > 0 ? UP : DOWN;
        }
      }
    }
    else if (controlMode == ControlMode.Keyboard)
    {
      if (Input.GetKeyDown(KeyCode.UpArrow))
        return UP;
      if (Input.GetKeyDown(KeyCode.DownArrow))
        return DOWN;
      if (Input.GetKeyDown(KeyCode.LeftArrow))
        return LEFT;
      if (Input.GetKeyDown(KeyCode.RightArrow))
        return RIGHT;
      if (Input.GetKeyDown(KeyCode.Space))
        return EMPTY;
      return -1;
    }
    else
    {
      return (int)imageRecognition();
      //return -1;
    }
#else
        // Input.multiTouchEnabled = false;
        // AddInputString($"Total Touch{Input.touchCount}");
        //print(Input.touchCount);
        
        return (int)imageRecognition();
        
        // if (Input.touchCount == 0)
        // {
        //     // print("沒有按壓屏幕");
        //     return -1;
        // }

        // if (hasInput)
        // {
        //     if (Input.GetTouch(0).phase == TouchPhase.Ended)
        //     {
        //         hasInput = false;
        //     }
        // }

        // x = Input.GetTouch(0).deltaPosition.x;
        // y = Input.GetTouch(0).deltaPosition.y;
        // // print($"{x} , {y}");
        // if (Mathf.Abs(x) > Mathf.Abs(y))
        // {
        //     if (Mathf.Abs(x) < minMouseAccel)
        //     {
        //         return 0;
        //     }
        //     else
        //     {
        //         hasInput = true;
        //         AddInputString(IntToAniString(x > 0 ? RIGHT : LEFT));
        //         return x > 0 ? RIGHT : LEFT;
        //     }
        // }
        // else
        // {
        //     if (Mathf.Abs(y) < minMouseAccel)
        //     {
        //         return 0;
        //     }
        //     else
        //     {
        //         hasInput = true;
        //         AddInputString(IntToAniString(y > 0 ? UP : DOWN));
        //         return y > 0 ? UP : DOWN;
        //     }
        // }
#endif
  }

  public string IntToAniString(int i)
  {
    //////////////////////////////////////////////////
    switch (i)
    {
      case UP:
        return "UP";

      case DOWN:
        return "DOWN";

      case LEFT:
        return "LEFT";

      case RIGHT:
        return "RIGHT";
    }
    //////////////////////////////////////////////////
    return null;
  }

  public void AddInputString(string s)
  {
    if (inputDebugString.Count != 0 && s == inputDebugString[inputDebugString.Count - 1])
      return;
    if (inputDebugString.Count > inputDebugLength)
    {
      inputDebugString.RemoveAt(0);
    }

    inputDebugString.Add(s);
  }
  public void DebugTextUpdate()
  {
    string temp = "";
    foreach (string s in inputDebugString)
    {
      temp += s + "\n";
    }

    debugText.text = temp;
  }

  public void JudgeBeat()
  {
    int input = 0;
    input = InputCheck();
    debugText.text = input.ToString();
    string aniString = null;

    if (measureTime <= judgementTime / 2 || measureTime >= (beatTime - judgementTime / 2))
    {
      int currentProgress = beatmapProgress;
      if (currentProgress >= hasHits.Count)
        return;

      if (hasHits[currentProgress] == HitStatus.NotHit)
      {
        int beat = beats[currentProgress];
        if (beat == 0)
        {
          // Debug.Log("空節拍");
        }
        else if (input == -1 || input == 0)
        {
          // Debug.Log("無輸入");
        }
        else if (input == beat)
        {
          hasHits[currentProgress] = HitStatus.Perfect;
          Debug.Log($"得分 {beat}");
          aniString = IntToAniString(beat);
          arrowBeatObjects[currentProgress].GetComponent<Image>().color = Color.white;
          if (arrowBeatObjects[currentProgress].GetComponentInChildren<UIParticle>() != null)
            arrowBeatObjects[currentProgress].GetComponentInChildren<UIParticle>().Play();
          score++;
          for (int i = dancerCount; i < (float)score / (((float)beats.Count - m_emptyCount) / 2f) * (float)characterParent.childCount; i++)
          {
            if (characterParent.GetChild(i) && characterParent.GetChild(i).GetComponent<Dancer>())
            {
              characterParent.GetChild(i).GetComponent<Dancer>().IntoCamera();
              dancerCount++;
            }
            else
            {
              break;
            }
          }
        }
        else if (input != beat)
        {
          // Debug.Log($"{beat} 打錯成 {input}");
          aniString = "MISS";
          hasHits[currentProgress] = HitStatus.Miss;
        }
      }
    }

    if (measureTime > judgementTime / 2 && hasHits[beatmapProgress] == HitStatus.NotHit && beats[beatmapProgress] != 0)
    {
      Debug.Log("錯過");
      hasHits[beatmapProgress] = HitStatus.Miss;
      /////////////////////////////////////////////////
      aniString = "MISS";
      ////////////////////////////////////////////////
    }

    if (aniString != null)
    {
      foreach (Animator ani in animators)
      {
        ani.SetTrigger(aniString);
      }
      if (aniString == "MISS")
      {
        m_fail.SetTrigger("Fail");
      }

    }
  }
  public void GameEnd()
  {
    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    Init();
    Start();
  }
  private void Update()
  {
    if (m_gameEnd)
    {
      HumanoidInduction(m_EndCountdownText);
    }
    else if (!m_gameStart)
    {
      HumanoidInduction(m_startCountdownText);
    }


  }
  private void HumanoidInduction(Text CountdownText)
  {
    if (isPlayerRange())
    {
      debugText.text = startBox() + "";

      if (startBox() && m_startCountdown > 0)
      {
        CountdownText.GetComponent<Animator>().SetBool("Scanning", false);
        m_startCountdown -= Time.deltaTime;
        CountdownText.text = "Start Scanning...\n" + m_startCountdown.ToString("0");
      }
      else if (startBox() && m_startCountdown > -1)
      {
        m_startCountdown = -1f;
        if (m_gameEnd)
        {
          m_endUI.SetActive(false);
          GameEnd();
        }
        else if (!m_gameStart)
        {
          GameStart();
        }

      }
      else
      {
        CountdownText.text = "Start Scanning...\nStep Back";
        CountdownText.GetComponent<Animator>().SetBool("Scanning", true);
        m_startCountdown = 3;
      }
    }
  }
}
