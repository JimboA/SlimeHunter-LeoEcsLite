using UnityEngine;

namespace Client.Battle.View
{
    public class RandomizeIdleAnimation : StateMachineBehaviour
    {
        private static readonly int IdleValue = Animator.StringToHash("IdleValue");
    
        public float DelayMin;
        public float DelayMax;
        public int   IdleAnimationsCount;

        private float _delay;
        private int   _currentValue;
        private int   _nextValue;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            _delay = Random.Range(DelayMin, DelayMax);
            _currentValue = 0;
            _nextValue = 0;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_delay <= 0)
            {
                _delay = Random.Range(DelayMin, DelayMax);
                animator.SetInteger(IdleValue, 0);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_delay <= 0)
                _nextValue = Random.Range(1, IdleAnimationsCount + 1);
            else
                _delay -= Time.deltaTime;

            if (_currentValue != _nextValue)
            {
                animator.SetInteger(IdleValue, _nextValue);
                _currentValue = _nextValue;
            }
        }
    }
}
