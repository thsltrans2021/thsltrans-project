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
    private string[] AnimStates = { "state1", "state2", "state3", "state4", "state5", "state6", "state7", "state8", "state9", "state10" };

    private const string AnimCtrlParamPlay = "Play";
    private const string AnimCtrlParamEnd = "End";
    private const string AnimationPath = "Animations/";
    private const string AnimCtrlPath = "AnimatorControllers/";

    public Animator AvatarAnimator => _avatarAnimator;

    public AnimationManager(Animator animator, string animCtrlName = null)
    {
        _avatarAnimator = animator;
        if (animCtrlName != null)
        {
            LoadAnimatorControllerToAnimator(animCtrlName);
        }
        _defaultAvatarAnimatorCtrl = _avatarAnimator.runtimeAnimatorController;
    }

    public void PlayAnimation()
    {
        _avatarAnimator.SetInteger(AnimCtrlParamPlay, 1);
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

    public void ResetAnimatorControllerStates()
    {
        Object.Destroy(_avatarAnimator.runtimeAnimatorController);
        _avatarAnimator.runtimeAnimatorController = _defaultAvatarAnimatorCtrl;
        SetAnimationEndingPosition(AnimStates.Length);
        ForceStopAnimation();
    }

    // Position starts at 1, unlike the index which starts at 0
    public void SetAnimationEndingPosition(int targetStatePosition)
    {
        _avatarAnimator.SetInteger(AnimCtrlParamEnd, targetStatePosition);
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
        } else
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

   public void OverrideAnimationClips(AnimatorOverrideController controller, string[] animClipNames)
    {
        var currentOverrides = new AnimationClipOverrides(controller.overridesCount);
        controller.GetOverrides(currentOverrides);

        for (int i = 0; i < animClipNames.Length; i++)
        {
            AnimationClip newClip = LoadAnimationClip(animClipNames[i]);
            string defaultClipName = AnimStates[i];
            //Debug.Log($"{i} Default: {defaultClipName}");
            currentOverrides[defaultClipName] = newClip;
        }

        controller.ApplyOverrides(currentOverrides);
    }

    public void OverrideSingleAnimationClip(AnimatorOverrideController controller, string animClipName, int targetStateIndex)
    {
        AnimationClip newClip = LoadAnimationClip(animClipName);
        controller[AnimStates[targetStateIndex]] = newClip;
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
