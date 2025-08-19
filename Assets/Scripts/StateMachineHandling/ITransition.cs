
namespace Game.StateMachineHandling
{
    public interface ITransition
    {
        IState To {  get; }
        IPredicate Condition { get; }
    }
}
