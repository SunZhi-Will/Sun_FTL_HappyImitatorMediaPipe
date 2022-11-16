using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The UI for displaying result
/// </summary>
public class ResultUI : MonoBehaviour
{
    private Animator _animator;
    private readonly int _successAnimParam = Animator.StringToHash("success");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ShowSuccessUI(float delaySecond = 0)
    {
        if (delaySecond > 0)
            StartCoroutine(
                DelayCoroutine(delaySecond, ActivateSuccessUI));
        else
            ActivateSuccessUI();
    }

    public void ShowFailedUI(float delaySecond = 0)
    {
        if (delaySecond > 0)
            StartCoroutine(
                DelayCoroutine(delaySecond, ActivateFailedUI));
        else
            ActivateFailedUI();
    }

    private IEnumerator DelayCoroutine(float delaySecond, Action onDelayEnded)
    {
        yield return new WaitForSeconds(delaySecond);
        onDelayEnded.Invoke();
    }

    private void ActivateSuccessUI()
    {
        _animator.enabled = true;
        _animator.SetBool(_successAnimParam, true);
    }

    private void ActivateFailedUI()
    {
        _animator.enabled = true;
        _animator.SetBool(_successAnimParam, false);
    }
}
