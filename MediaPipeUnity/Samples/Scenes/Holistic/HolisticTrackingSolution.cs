// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using UnityEngine;

namespace Mediapipe.Unity.Holistic
{
  public class HolisticTrackingSolution : ImageSourceSolution<HolisticTrackingGraph>
  {

    [Flags]
    public enum ShowStatus
    {
      Nome = 0,
      Pose = 1,
      Hand = 2,
      Face = 4,
      PoseWorld = 8
    }
    public ShowStatus showStatus = 0;
    [SerializeField] private RectTransform _worldAnnotationArea;
    // [SerializeField] private DetectionAnnotationController _poseDetectionAnnotationController;
    [SerializeField] private HolisticLandmarkListAnnotationController _holisticAnnotationController;
    // [SerializeField] private PoseWorldLandmarkListAnnotationController _poseWorldLandmarksAnnotationController;
    // [SerializeField] private MaskAnnotationController _segmentationMaskAnnotationController;
    [SerializeField] private NormalizedRectAnnotationController _poseRoiAnnotationController;
    [SerializeField] private NormalizedRectAnnotationController _poseRoiAScopeController;

    [SerializeField] private CharacterControl _characterControl;

    public HolisticTrackingGraph.ModelComplexity modelComplexity
    {
      get => graphRunner.modelComplexity;
      set => graphRunner.modelComplexity = value;
    }

    public bool smoothLandmarks
    {
      get => graphRunner.smoothLandmarks;
      set => graphRunner.smoothLandmarks = value;
    }

    public bool refineFaceLandmarks
    {
      get => graphRunner.refineFaceLandmarks;
      set => graphRunner.refineFaceLandmarks = value;
    }

    public bool enableSegmentation
    {
      get => graphRunner.enableSegmentation;
      set => graphRunner.enableSegmentation = value;
    }

    public bool smoothSegmentation
    {
      get => graphRunner.smoothSegmentation;
      set => graphRunner.smoothSegmentation = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

    protected override void SetupScreen(ImageSource imageSource)
    {
      base.SetupScreen(imageSource);
      _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
    }

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {


        if ((showStatus & ShowStatus.Face) == ShowStatus.Face)
          graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
        if ((showStatus & ShowStatus.Pose) == ShowStatus.Pose)
        {
          graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
          graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
        }
        if ((showStatus & ShowStatus.Hand) == ShowStatus.Hand)
        {
          graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
          graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
        }
        if ((showStatus & ShowStatus.PoseWorld) == ShowStatus.PoseWorld)
        {
          //   graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
          graphRunner.OnPoseWorldLandmarksOutput += OnObject3DFitOutput;
        }
        // graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
        graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;
        graphRunner.OnPoseRoiOutput += OnPoseRoiScopeOutput;
        graphRunner.OnPoseRoiOutput += OnPoseRoiCharacterControlOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      // SetupAnnotationController(_poseDetectionAnnotationController, imageSource);
      SetupAnnotationController(_holisticAnnotationController, imageSource);
      // SetupAnnotationController(_poseWorldLandmarksAnnotationController, imageSource);
      //SetupAnnotationController(_characterControl, imageSource);
      // SetupAnnotationController(_segmentationMaskAnnotationController, imageSource);
      // _segmentationMaskAnnotationController.InitScreen(imageSource.textureWidth, imageSource.textureHeight);
      SetupAnnotationController(_poseRoiAnnotationController, imageSource);
      SetupAnnotationController(_poseRoiAScopeController, imageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      Detection poseDetection = null;
      NormalizedLandmarkList faceLandmarks = null;
      NormalizedLandmarkList poseLandmarks = null;
      NormalizedLandmarkList leftHandLandmarks = null;
      NormalizedLandmarkList rightHandLandmarks = null;
      LandmarkList poseWorldLandmarks = null;
      ImageFrame segmentationMask = null;
      NormalizedRect poseRoi = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() =>
          graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, false));
      }

      // _poseDetectionAnnotationController.DrawNow(poseDetection);
      _holisticAnnotationController.DrawNow(faceLandmarks, poseLandmarks, leftHandLandmarks, rightHandLandmarks);
      _characterControl.DrawNow(poseWorldLandmarks, poseRoi);
      // _poseWorldLandmarksAnnotationController.DrawNow(poseWorldLandmarks);

      // _segmentationMaskAnnotationController.DrawNow(segmentationMask);
      _poseRoiAnnotationController.DrawNow(poseRoi);
      _poseRoiAScopeController.DrawNow(poseRoi);
    }

    private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
    {
      // _poseDetectionAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawFaceLandmarkListLater(eventArgs.value);
    }

    private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawPoseLandmarkListLater(eventArgs.value);
    }

    private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawLeftHandLandmarkListLater(eventArgs.value);
    }

    private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _holisticAnnotationController.DrawRightHandLandmarkListLater(eventArgs.value);
    }

    // private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    // {
    //    _poseWorldLandmarksAnnotationController.DrawLater(eventArgs.value);
    // }

    // private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
    // {
    //    _segmentationMaskAnnotationController.DrawLater(eventArgs.value);
    // }

    private void OnPoseRoiOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {
      //eventArgs.value
      NormalizedRect eventA = new NormalizedRect();
      eventA.XCenter = 0.5f;
      eventA.YCenter = 0.5f;
      eventA.Width = 1.5f;
      eventA.Height = 1;
      _poseRoiAnnotationController.DrawLater(eventA);
    }
    private void OnPoseRoiScopeOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {
      //eventArgs.value

      _poseRoiAScopeController.DrawLater(eventArgs.value);
    }
    private void OnPoseRoiCharacterControlOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {

      _characterControl.DrawLater(eventArgs.value);
    }
    private void OnObject3DFitOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    {

      _characterControl.DrawLater(eventArgs.value);
    }
  }
}
