
namespace Player
{
    public abstract class PlayerState
    {
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}