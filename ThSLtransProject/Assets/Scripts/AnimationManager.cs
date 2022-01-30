using System.Collections.Generic;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.ApplyOverrides.html
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}

public class AnimationManager
{
    private Animator _avatarAnimator;
    private RuntimeAnimatorController _defaultAvatarAnimatorCtrl;
    private bool _isSentencePosition1Started = false;
    private bool _isSentencePosition2Started = false;
    private int _playCount = 0;
    private string[] AnimStates1 = { "state1-1", "state2-1", "state3-1", "state4-1", "state5-1", "state6-1", "state7-1", "state8-1", "state9-1", "state10-1" };
    private string[] AnimStates2 = { "state1-2", "state2-2", "state3-2", "state4-2", "state5-2", "state6-2", "state7-2", "state8-2", "state9-2", "state10-2" };

    private const string AnimCtrlParamPlay = "Play";
    private const string AnimCtrlParamEnd = "End";
    private const string AnimCtrlParamEnd2 = "End2";
    private const string AnimCtrlParamIsP1Started = "IsP1Started";
    private const string AnimCtrlParamIsP2Started = "IsP2Started";
    private const string AnimationPath = "Animations/";
    private const string AnimCtrlPath = "AnimatorControllers/";

    private bool _isP1StartedPreviousState = false;
    private bool _isP1StartedCurrentState = false;
    private bool _isP2StartedPreviousState = false;
    private bool _isP2StartedCurrentState = false;

    public Animator AvatarAnimator => _avatarAnimator;
    public int SentencePlayCount => _playCount;

    public AnimationManager(Animator animator, string animCtrlName = null)
    {
        _avatarAnimator = animator;
        if (animCtrlName != null)
        {
            LoadAnimatorControllerToAnimator(animCtrlName);
        }
        _defaultAvatarAnimatorCtrl = _avatarAnimator.runtimeAnimatorController;
    }

    public void PlayAnimation(int sentencePosition)
    {
        _avatarAnimator.SetInteger(AnimCtrlParamPlay, sentencePosition);
    }

    public void StopAnimation()
    {
        _avatarAnimator.SetInteger(AnimCtrlParamPlay, 0);
    }

    public void ForceStopAnimation()
    {
        StopAnimation();
        ChangeAnimationState("idle");
    }

    public void ForcePlayAnimation(string animClipName)
    {
        //ChangeAnimationState(ANIM_STATES[0]);
    }

    // maybe use param to track if the first state of either sentence is played
    public int GetCurrentPlayingSentencePosition()
    {
        if (_avatarAnimator.GetBool(AnimCtrlParamIsP1Started))
        {
            return 1;
        }
        else if (_avatarAnimator.GetBool(AnimCtrlParamIsP2Started))
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public bool IsAnimationStatePlayed(string stateName)
    {
        var layerNumber = 0;
        return _avatarAnimator.GetCurrentAnimatorStateInfo(layerNumber).IsName(stateName);
    }

    public void UpdateSentencePositionStatus()
    {
        if (_avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("state1-1"))
        {
            _isSentencePosition1Started = true;
        }
        else if (_avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("state1-2"))
        {
            _isSentencePosition2Started = true;
        }
        else if (_avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            _isSentencePosition1Started = false;
            _isSentencePosition2Started = false;
        }

        _avatarAnimator.SetBool(AnimCtrlParamIsP1Started, _isSentencePosition1Started);
        _avatarAnimator.SetBool(AnimCtrlParamIsP2Started, _isSentencePosition2Started);
        _isP1StartedCurrentState = _isSentencePosition1Started;
        _isP2StartedCurrentState = _isSentencePosition2Started;
    }

    public void UpdateSentencePlayCount()
    {
        if (_isP1StartedPreviousState != _isP1StartedCurrentState)
        {
            if (!_isP1StartedPreviousState)
            {
                _playCount++;
                Debug.Log("[0] Play count: " + SentencePlayCount);
            }
            _isP1StartedPreviousState = _isP1StartedCurrentState;
            
        }
        else if (_isP2StartedPreviousState != _isP2StartedCurrentState)
        {
            if (!_isP2StartedPreviousState)
            {
                _playCount++;
                Debug.Log("[1] Play count: " + SentencePlayCount);
            }
            _isP2StartedPreviousState = _isP2StartedCurrentState;
        }
    }

    public void ResetAnimatorControllerStates()
    {
        Object.Destroy(_avatarAnimator.runtimeAnimatorController);
        _avatarAnimator.runtimeAnimatorController = _defaultAvatarAnimatorCtrl;
        SetAnimationEndingPosition(AnimStates1.Length, 1);
        SetAnimationEndingPosition(AnimStates2.Length, 2);
        ForceStopAnimation();
    }

    // Position starts at 1, unlike the index which starts at 0
    public void SetAnimationEndingPosition(int targetStatePosition, int sentencePosition)
    {
        if (sentencePosition == 1)
        {
            _avatarAnimator.SetInteger(AnimCtrlParamEnd, targetStatePosition);
        }
        else
        {
            _avatarAnimator.SetInteger(AnimCtrlParamEnd2, targetStatePosition);
        }
    }

    public void LoadAnimatorControllerToAnimator(string animCtrlFilename)
    {
        _avatarAnimator.runtimeAnimatorController = LoadAnimatorController(animCtrlFilename);
    }

    public void AssignOverrideControllerToAvatarAnimatorController(AnimatorOverrideController overrideController)
    {
        _avatarAnimator.runtimeAnimatorController = overrideController;
        _avatarAnimator.Update(0.0f);
    }

    public AnimatorOverrideController CreateAnimatorOverrideController(Animator animator = null)
    {
        if (animator == null)
        {
            return new AnimatorOverrideController(_avatarAnimator.runtimeAnimatorController);
        }
        else
        {
            return new AnimatorOverrideController(animator.runtimeAnimatorController);
        }
        
    }

    public RuntimeAnimatorController LoadAnimatorController(string animCtrlFilename)
    {
        return Resources.Load(AnimCtrlPath + animCtrlFilename) as RuntimeAnimatorController;
    }

    public AnimationClip LoadAnimationClip(string animClipName)
    {
        AnimationClip clip = Resources.Load(AnimationPath + animClipName) as AnimationClip;
        if (clip == null)
        {
            Debug.Log($"Cannot find an animation clip called {animClipName}");
        }
        else
        {
            Debug.Log($"Load: {clip}");
        }

        /*if (Resources.Load("Temp/" + animClipName + ".fbx"))
        {
            Debug.Log("Found fbx: " + animClipName);
        }
        else
        {
            Debug.Log("Cannot find fbx");
        }*/

        return clip;
    }

   public void OverrideAnimationClips(AnimatorOverrideController controller, List<string> animClipNames, int sentencePosition)
    {
        var currentOverrides = new AnimationClipOverrides(controller.overridesCount);
        controller.GetOverrides(currentOverrides);

        for (int i = 0; i < animClipNames.Count; i++)
        {
            AnimationClip newClip = LoadAnimationClip(animClipNames[i]);
            string defaultClipName = AnimStates1[i];
            if (sentencePosition == 2)
            {
                defaultClipName = AnimStates2[i];
            }
            //Debug.Log($"{i} Default: {defaultClipName}");
            currentOverrides[defaultClipName] = newClip;
        }
        controller.ApplyOverrides(currentOverrides);
    }

    public void OverrideSingleAnimationClip(AnimatorOverrideController controller, string animClipName, int targetStateIndex, int sentencePosition)
    {
        AnimationClip newClip = LoadAnimationClip(animClipName);
        if (sentencePosition == 1)
        {
            controller[AnimStates1[targetStateIndex]] = newClip;
        }
        else
        {
            controller[AnimStates2[targetStateIndex]] = newClip;
        }
    }

    public void ChangeAnimationState(string stateName)
    {
        _avatarAnimator.Play(stateName);
    }

    public void PrintOverrideAnimationClips(AnimatorOverrideController controller)
    {
        var overrides = new AnimationClipOverrides(controller.overridesCount);
        controller.GetOverrides(overrides);
        for (int i = 0; i < overrides.Capacity; i++)
        {
            Debug.Log($"{i} {overrides[i]}");
        }
    }
}
