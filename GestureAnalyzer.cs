using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GestureAnalyzer
{
    public enum MeaningfulGesture {
        None,
        Hold,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Rock,
        Spiderman,
        FuckQ
    }
    public static MeaningfulGesture Analyze (this GestureTrackingDirector.FingerState state) {
        if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen |
        GestureTrackingDirector.FingerState.RingOpen)) {
        return MeaningfulGesture.Nine;
        } else if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen)) {
        return MeaningfulGesture.Eight;
        } else if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.IndexOpen)) {
        return MeaningfulGesture.Seven;
        } else if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.PinkyOpen)) {
        return MeaningfulGesture.Six;
        } else if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen |
        GestureTrackingDirector.FingerState.RingOpen |
        GestureTrackingDirector.FingerState.PinkyOpen)) {
        return MeaningfulGesture.Five;
        } else if (state == (
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen |
        GestureTrackingDirector.FingerState.RingOpen |
        GestureTrackingDirector.FingerState.PinkyOpen)) {
        return MeaningfulGesture.Four;
        } else if (state == (
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen |
        GestureTrackingDirector.FingerState.RingOpen)) {
        return MeaningfulGesture.Three;
        } else if (state == (
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.MiddleOpen)) {
        return MeaningfulGesture.Two;
        } else if (state == (
        GestureTrackingDirector.FingerState.IndexOpen)) {
        return MeaningfulGesture.One;
        } else if (state == (
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.PinkyOpen)) {
        return MeaningfulGesture.Rock;
        } else if (state == (
        GestureTrackingDirector.FingerState.ThumbOpen |
        GestureTrackingDirector.FingerState.IndexOpen |
        GestureTrackingDirector.FingerState.PinkyOpen)) {
        return MeaningfulGesture.Spiderman;
        } else if (state == (
        GestureTrackingDirector.FingerState.Closed)) {
        return MeaningfulGesture.Hold;
        } else if (state == (
        GestureTrackingDirector.FingerState.MiddleOpen)) {
        return MeaningfulGesture.FuckQ;
        } else {
        return MeaningfulGesture.None;
        }
    }
}
