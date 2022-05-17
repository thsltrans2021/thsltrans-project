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
    private bool _isInitialOverrideCtrlCreated = false;

    const string IdleAnim = "Rain@idle";
    const string MyselfAnim = "Pron1";
    const string WalkAnim = "PERSONbody-WALK";
    const string YouAnim = "Pron2";
    const string MoneyAnim = "MONEY";
    const string IdleState = "idle";

    private AnimatorOverrideController _overrideController;
    private int _paragraphIndex = -1;
    private int _sentenceIndex = 0;
    /*private List<List<List<string>>> _paragraphs = new List<List<List<string>>>()
    {
        new List<List<string>>()
        {
            new List<string>() { "WORK-FROM-HOME", "1", "2" },
            new List<string>() { YouAnim, MoneyAnim },
            new List<string>() { MoneyAnim, WalkAnim },
            new List<string>() { YouAnim, WalkAnim, WalkAnim },
            new List<string>() { MoneyAnim, MoneyAnim, YouAnim },
        },
        new List<List<string>>()
        {
            new List<string>() { WalkAnim, YouAnim },
        },
    };*/
    private List<List<List<string>>> _paragraphs;
    private int _paragraphsLength = 0;

    private void ResetAnimationHelperParams()
    {
        _switchSentence = false;
        _waitToUpdateAnimation = false;
        _reachEndOfParagraph = false;
        _sentenceIndex = 0;
        _nextSentencePosition = 0;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _animManager = new AnimationManager(_animator, DefaultAnimCtrlPath);
    }

    void Update()
    {
        _animManager.UpdateSentencePositionStatus();
        _animManager.UpdateSentencePlayCount();

        //PlayGround();
        //PlayAnimationsOfOneParagraph(_paragraphs[0]);

        if (Input.GetKeyDown(KeyCode.P))
        {
            //_startTranslation = true;
            if (_paragraphs != null)
            {
                PlayParagraphs(_paragraphs);
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log("Print current _paragraphs");
            foreach (List<List<string>> paragraph in _paragraphs)
            {
                for (int i = 0; i < paragraph.Count; i++)
                {
                    for (int j = 0; j < paragraph[i].Count; j++)
                    {
                        Debug.Log($"s {i}, w {j}, {paragraph[i][j]}");
                    }
                }
            }
        }

        if (_paragraphs != null)
        {
            _paragraphsLength = _paragraphs.Count;
        }
        else
        {
            _paragraphsLength = 0;
        }

        // TODO: still weird when changing paragraph -> no separation at all
     /*   Debug.Log($"paragraphIndex: {_paragraphIndex}");*/
        if (_paragraphIndex >= 0 && _paragraphIndex < _paragraphsLength)
        {
            if (_paragraphIndex == 0 && !_isInitialOverrideCtrlCreated)
            {
                Debug.Log($"Create override controller {_paragraphIndex}");
                _overrideController = _animManager.CreateAnimatorOverrideController();
                _isInitialOverrideCtrlCreated = true;
            }

            PlayAnimationsOfOneParagraph(_paragraphs[_paragraphIndex]);

            // check if all anim are played and now is playing idle before changing paragraphIndex
            if (_reachEndOfParagraph && _animManager.IsAnimationStatePlayed(IdleState))
            {
                Debug.Log("Switch to next paragraph");
                _animManager.ResetAnimatorControllerStates();
                _animManager.ResetSentencePlayCount();
                ResetAnimationHelperParams();

                _paragraphIndex++;

                if (_paragraphIndex < _paragraphsLength)
                {
                    Debug.Log($"Create override controller {_paragraphIndex}");
                    _overrideController = _animManager.CreateAnimatorOverrideController();
                }
            }
        }
        else if (_paragraphIndex >= _paragraphsLength)
        {
            Debug.Log("All paragraphs are played");
            _reachEndOfParagraph = false;
            _paragraphIndex = -1;
            _isInitialOverrideCtrlCreated = false;
        }

    }

    public void PlayParagraphs(List<List<List<string>>> paragraphs)
    {
        _paragraphs = paragraphs;
        Debug.Log("Play paragraph");
        _paragraphIndex = 0;
    }

    private void PlayGround()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!_switchSentence)
            {
                _animManager.PlayAnimation(1);
            }
            else
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
            var overrideController = _animManager.CreateAnimatorOverrideController();
            var animNames = new List<string>() { YouAnim, WalkAnim };
            _animManager.OverrideAnimationClips(overrideController, animNames, 1);
            _animManager.OverrideSingleAnimationClip(overrideController, MoneyAnim, 3, 2);

            _animManager.PrintOverrideAnimationClips(overrideController);
            _animManager.AssignOverrideControllerToAvatarAnimatorController(overrideController);

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _animManager.ResetAnimatorControllerStates();
        }
    }

    // TODO: refactor
    private void PlayAnimationsOfOneParagraph(List<List<string>> paragraph)
    {
        // update (or assign?) when reaching idle agian after 2 sentences are played
        if (_waitToUpdateAnimation && _animManager.IsAnimationStatePlayed(IdleState))
        {
            _animator.Update(0.0f);
            _animManager.PlayAnimation(_nextSentencePosition);
            _waitToUpdateAnimation = false;
            Debug.Log($"Update animator. Set next position {_nextSentencePosition}");
            _animManager.PrintOverrideAnimationClips();
        }

        if (_sentenceIndex == 0)
        {
            // take care of the 1st sentence and the 2nd sentence
            for (int i = 0; i < paragraph.Count; i++)
            {
                Debug.Log($"i: {i}, {i + 1}");
                if (i > 1)
                {
                    break;
                }
                List<string> sentence = paragraph[i];
                _animManager.OverrideAnimationClips(_overrideController, sentence, i + 1);
                _animManager.SetAnimationEndingPosition(sentence.Count, i + 1);
                _sentenceIndex++;
            }
            _animManager.AssignOverrideControllerToAvatarAnimatorController(_overrideController);
            _animManager.PlayAnimation(1);
            _animManager.ChangeAnimationState("state1-1");
            _waitToUpdateAnimation = true;
            _nextSentencePosition = 2;
            Debug.Log($"Initialize override controller. Current sentenceIndex: {_sentenceIndex}, p length: {paragraph.Count}");
            _animManager.PrintOverrideAnimationClips();
        }
        // take care of the 3rd sentence until the last sentence
        else if (_sentenceIndex < paragraph.Count)
        {
            List<string> sentence = paragraph[_sentenceIndex];
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
            if (_animManager.SentencePlayCount == paragraph.Count && !_reachEndOfParagraph)
            {
                Debug.Log("All sentences are played");
                _reachEndOfParagraph = true;
                _waitToUpdateAnimation = true;
                _nextSentencePosition = 0;
            }
        }
    }
}
