using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{

    public bool ChangeSentence = false;
    public bool ChangeCtrl = false;
    public string DefaultAnimCtrlPath = "02RainAnimCtrl";

    private Animator _animator;
    private AnimationManager _animManager;
    
    const string IdleAnim = "Rain@idle";
    const string MyselfAnim = "I";
    const string WalkAnim = "PERSONbody-WALK";
    const string YouAnim = "YOU";
    const string MoneyAnim = "MONEY";
        
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animManager = new AnimationManager(_animator, DefaultAnimCtrlPath);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            _animManager.PlayAnimation();
            //animManager.ChangeAnimationState("walk01");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _animManager.ForceStopAnimation();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeSentence = !ChangeSentence;
            /*
            if (defaultAnimCtrlPath == "RainAnimCtrl")
            {
                animator.SetBool("Change", changeSentence);
            }*/
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Change animator controller back");
            ChangeCtrl = !ChangeCtrl;
            _animManager.LoadAnimatorControllerToAnimator(DefaultAnimCtrlPath);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Assign new override anim ctrl");

            var overrideController = _animManager.CreateAnimatorOverrideController();
            string[] animNames = { YouAnim, WalkAnim };
            _animManager.OverrideAnimationClips(overrideController, animNames);
            _animManager.OverrideSingleAnimationClip(overrideController, WalkAnim, 3);

            _animManager.PrintOverrideAnimationClips(overrideController);
            _animManager.AssignOverrideControllerToAvatarAnimatorController(overrideController);
            _animManager.SetAnimationEndingPosition(4);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _animManager.ResetAnimatorControllerStates();
        }
    }
}
