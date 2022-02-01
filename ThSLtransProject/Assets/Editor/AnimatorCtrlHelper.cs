using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimatorCtrlHelper
{

    private Animator avatarAnimator;
    private string animCtrlParamName = "playSentence";

    private const string ANIMATION_PATH = "Animations/";
    private const string ANIMCTRL_PATH = "AnimatorControllers/";

    public AnimatorCtrlHelper(Animator animator)
    {
        avatarAnimator = animator;
    }

    public void LoadAnimatorControllerToAnimator(string animCtrlFilename)
    {
        avatarAnimator.runtimeAnimatorController = LoadAnimatorController(animCtrlFilename);
    }

    public void AssignAnimatorControllerToAnimator(UnityEditor.Animations.AnimatorController newAnimatorCtrl)
    {
        avatarAnimator.runtimeAnimatorController = newAnimatorCtrl;
    }

    // TODO: automatically play animation from first sentence to last
    public void StartAnimation()
    {
        avatarAnimator.SetInteger(animCtrlParamName, 1);
    }

    public UnityEditor.Animations.AnimatorController CreateAnimatorController(string name)
    {
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/" + ANIMCTRL_PATH + name + ".controller");
        controller.AddParameter(animCtrlParamName, AnimatorControllerParameterType.Int);
        return controller;
    }

    public RuntimeAnimatorController LoadAnimatorController(string animCtrlFilename)
    {
        return Resources.Load(ANIMCTRL_PATH + animCtrlFilename) as RuntimeAnimatorController;
    }

    public void DeleteAnimatorController(RuntimeAnimatorController controller)
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(controller);
        Debug.Log("Delete: " + path);
        UnityEditor.AssetDatabase.DeleteAsset(path);
    }

    public UnityEditor.Animations.AnimatorState AddAnimationClip(UnityEditor.Animations.AnimatorController controller, string animClipName)
    {
        // create a new state with the motion (animation clip) at root state machine
        AnimationClip clip = Resources.Load(ANIMATION_PATH + animClipName) as AnimationClip;
        if (clip == null)
        {
            Debug.Log("Cannot find an animation clip called " + animClipName);
        }
        else
        {
            Debug.Log(clip.ToString());
        }

        if (Resources.Load("Temp/" + animClipName + ".fbx"))
        {
            Debug.Log("Found fbx: " + animClipName);
        }
        else
        {
            Debug.Log("Cannot find fbx");
        }

        return controller.AddMotion(clip);
    }

    // Sentence level
    public void CreateAnimatorStateTransitions(int sentenceNumber, UnityEditor.Animations.AnimatorState[] states)
    {
        UnityEditor.Animations.AnimatorState idleState = states[0];

        for (int i = 0; i < states.Length; i++)
        {
            UnityEditor.Animations.AnimatorStateTransition transition;
            if (i + 1 == states.Length)
            {
                // last state -> idle
                transition = states[i].AddTransition(idleState);
                transition.hasExitTime = true;
            }
            else if (i + 1 < states.Length)
            {
                transition = states[i].AddTransition(states[i + 1]);
                transition.hasExitTime = true;

                // add condition for changing the sentence
                if (i == 0)
                {
                    transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Equals, sentenceNumber, animCtrlParamName);
                }
            }
        }

    }
}
