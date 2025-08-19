using System;
using UnityEngine;

namespace Game.StateMachineHandling
{
    public class FuncPredicate : IPredicate
    {
        private Func<bool> condition;

        public FuncPredicate(Func<bool> condition)
        {
            this.condition = condition;
        }

        public bool Evaluate()
        {
            return condition.Invoke();
        }
    }
}
