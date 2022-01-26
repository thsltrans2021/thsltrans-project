using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RainController : MonoBehaviour
{

    public bool changeSentence = false;
    public bool changeCtrl = false;
    public string animCtrlNameToBeDeleted;
    public string newAnimCtrlName = "newController";
    
    Animator animator;
    UnityEditor.Animations.AnimatorController newAnimatorCtrl;
    private AnimationManager animManager;
    private string defaultAnimCtrlPath = "RainAnimCtrl";
   
    const string IDLE_ANIM = "Rain@idle";
    const string MYSELF_ANIM = "MYSELF";
    const string WALK_ANIM = "PERSONbody-WALK";
    const string YOU_ANIM = "YOU";

    string[] ANIM_NAMES = { IDLE_ANIM, MYSELF_ANIM, WALK_ANIM, YOU_ANIM };
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animManager = new AnimationManager(animator);
        animManager.LoadAnimatorControllerToAnimator(defaultAnimCtrlPath);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            animManager.StartAnimation();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            changeSentence = !changeSentence;
            if (defaultAnimCtrlPath == "RainAnimCtrl")
            {
                animator.SetBool("Change", changeSentence);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Change animator controller back");
            changeCtrl = !changeCtrl;
            animManager.LoadAnimatorControllerToAnimator(defaultAnimCtrlPath);
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Create new animator controller: " + newAnimCtrlName);
            newAnimatorCtrl = animManager.CreateAnimatorController(newAnimCtrlName);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Assign new anim ctrl");

            if (newAnimatorCtrl != null)
            {
                // TODO: retrieve from DB or load if exist
                var idleState = animManager.AddAnimationClip(newAnimatorCtrl, IDLE_ANIM);
                var myselfState = animManager.AddAnimationClip(newAnimatorCtrl, MYSELF_ANIM);
                var youState = animManager.AddAnimationClip(newAnimatorCtrl, YOU_ANIM);
                var walkState = animManager.AddAnimationClip(newAnimatorCtrl, WALK_ANIM);

                UnityEditor.Animations.AnimatorState[] sentence1 = { idleState, myselfState, walkState };
                UnityEditor.Animations.AnimatorState[] sentence2 = { idleState, youState, walkState };

                animManager.CreateAnimatorStateTransitions(1, sentence1);
                animManager.CreateAnimatorStateTransitions(2, sentence2);

                animManager.AssignAnimatorControllerToAnimator(newAnimatorCtrl);
            }
            else
            {
                Debug.Log("Cannot find " + newAnimCtrlName);
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            animManager.DeleteAnimatorController(animManager.LoadAnimatorController(animCtrlNameToBeDeleted));
        }
    }
}
