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
    private bool _switchSentence = false;
    private bool _waitToUpdateAnimation = false;
    private int _nextSentencePosition = 0;
    private bool _reachEndOfParagraph = false;

    const string IdleAnim = "Rain@idle";
    const string MyselfAnim = "I";
    const string WalkAnim = "PERSONbody-WALK";
    const string YouAnim = "YOU";
    const string MoneyAnim = "MONEY";
    const string IdleState = "idle";

    private AnimatorOverrideController _overrideController;
    private int _sentenceIndex = 0;
    private List<List<string>> _paragraph = new List<List<string>>()
    {
        new List<string>() { YouAnim, WalkAnim },
        new List<string>() { MyselfAnim, MoneyAnim, WalkAnim, YouAnim },
        new List<string>() { MoneyAnim, WalkAnim },
        new List<string>() { YouAnim, MyselfAnim, WalkAnim, WalkAnim },
        new List<string>() { MyselfAnim, MoneyAnim, YouAnim, MoneyAnim },
    };

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animManager = new AnimationManager(_animator, DefaultAnimCtrlPath);
        _overrideController = _animManager.CreateAnimatorOverrideController();
    }

    // Update is called once per frame
    void Update()
    {
        _animManager.UpdateSentencePositionStatus();
        _animManager.UpdateSentencePlayCount();
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!_switchSentence)
            {
                _animManager.PlayAnimation(1);
            } else
            {
                _animManager.PlayAnimation(2);
            }
            _switchSentence = !_switchSentence;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(_animManager.GetCurrentPlayingSentencePosition());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _animManager.ForceStopAnimation();
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
            //var overrideController = _animManager.CreateAnimatorOverrideController();
            //var animNames = new List<string>() { YouAnim, WalkAnim };
            //_animManager.OverrideAnimationClips(overrideController, animNames, 1);
            //_animManager.OverrideSingleAnimationClip(overrideController, MoneyAnim, 3, 2);

            //_animManager.PrintOverrideAnimationClips(overrideController);
            //_animManager.AssignOverrideControllerToAvatarAnimatorController(overrideController);

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _animManager.ResetAnimatorControllerStates();
        }

        /* -------------------------------------- */

        // update (or assign?) when reaching idle agian after 2 sentences are played
        // use some param to check
        if (_waitToUpdateAnimation && _animManager.IsAnimationStatePlayed(IdleState))
        {
            _animator.Update(0.0f);
            _animManager.PlayAnimation(_nextSentencePosition);
            _waitToUpdateAnimation = false;
            Debug.Log($"Update animator. Set next position {_nextSentencePosition}");
        }
        
        if (_sentenceIndex == 0)
        {
            // take care 1st sentence and 2nd sentence
            for (int i = 0; i < _paragraph.Count; i++)
            {
                Debug.Log($"i: {i}, {i + 1}");
                if (i > 1)
                {
                    break;
                }
                List<string> sentence = _paragraph[i];
                _animManager.OverrideAnimationClips(_overrideController, sentence, i + 1);
                _animManager.SetAnimationEndingPosition(sentence.Count, i + 1);
                _sentenceIndex++;
            }
            _animManager.AssignOverrideControllerToAvatarAnimatorController(_overrideController);
            _animManager.PlayAnimation(1);
            _animManager.ChangeAnimationState("state1-1");
            _waitToUpdateAnimation = true;
            _nextSentencePosition = 2;
            Debug.Log($"Initialize override controller. Current sentenceIndex: {_sentenceIndex}, p count: {_paragraph.Count}");
        }
        // take care 3rd sentence until the last sentence
        else if (_sentenceIndex < _paragraph.Count)
        {
            //Debug.Log("index < count");
            List<string> sentence = _paragraph[_sentenceIndex];
            int sentenceLength = sentence.Count;
            //int targetSentencePosition;

            if (_sentenceIndex % 2 == 0)
            {
                // even index = odd position
                if (_animManager.GetCurrentPlayingSentencePosition() == 2 && !_waitToUpdateAnimation)
                {
                    // safe to override anim
                    Debug.Log("Current state: " + _animManager.GetCurrentPlayingSentencePosition());
                    _animManager.OverrideAnimationClips(_overrideController, sentence, 1);
                    _animManager.SetAnimationEndingPosition(sentenceLength, 1);
                    _waitToUpdateAnimation = true;
                    _nextSentencePosition = 1;
                    Debug.Log($"Override position 1 with sentenceIndex {_sentenceIndex}");
                    _sentenceIndex++;
                }
            }
            else
            {
                // odd index = even position
                if (_animManager.GetCurrentPlayingSentencePosition() == 1 && !_waitToUpdateAnimation)
                {
                    // safe to override anim
                    _animManager.OverrideAnimationClips(_overrideController, sentence, 2);
                    _animManager.SetAnimationEndingPosition(sentenceLength, 2);
                    _waitToUpdateAnimation = true;
                    _nextSentencePosition = 2;
                    Debug.Log($"Override position 2 with sentenceIndex {_sentenceIndex}");
                    _sentenceIndex++;
                }
            }
        }
        else
        {
            if (_animManager.SentencePlayCount == _paragraph.Count && !_reachEndOfParagraph)
            {
                Debug.Log("All sentences are played");
                _reachEndOfParagraph = true;
                _waitToUpdateAnimation = true;
                _nextSentencePosition = 0;
            }
        }
    }
}
