using UnityEngine;

namespace Game.StateMachineHandling
{
    public class Transition : ITransition
    {
        public IState To { get => to; }
        public IPredicate Condition { get => condition; }

        private readonly IState to;
        private readonly IPredicate condition;

        public Transition(IState to, IPredicate condition)
        {
            this.to = to;
            this.condition = condition;
        }
    }
}
