
using System;
using System.Collections.Generic;

namespace Game.StateMachineHandling
{
    public class StateMachine
    {
        private StateNode current;
        private Dictionary<Type, StateNode> nodes = new();
        private HashSet<ITransition> anyTransitions = new();


        public void OnUpdate()
        {
            ITransition transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }

            current.State?.OnUpdate();
        }

        public void OnFixedUpdate()
        {
            current.State?.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            current.State?.OnLateUpdate();
        }

        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(to, condition));
        }

        private StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if(node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }

        private void ChangeState(IState state)
        {
            if(state == current.State)
            {
                return;
            }

            var previousState = current.State;
            var nextState = nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState?.OnEnter();

            current = nodes[state.GetType()];
        }

        private ITransition GetTransition()
        {
            foreach(var transition in anyTransitions)
            {
                if(transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            foreach(var transition in current.Transitions)
            {
                if(transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            return null;
        }

        class StateNode
        {
            public IState State { get => state; }
            public HashSet<ITransition> Transitions { get => transitions; }

            private IState state;
            private HashSet<ITransition> transitions;

            public StateNode(IState state)
            {
                this.state = state;
                this.transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                transitions.Add(new Transition(to, condition));
            }
        }
    }
}
