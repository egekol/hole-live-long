using Cysharp.Threading.Tasks;

namespace Lib.Tasks
{
    public abstract class BaseTask : ATask<bool>
    {
        public abstract override UniTask<bool> ExecuteAsync();
        public string Name { get; private set; }

        protected BaseTask()
        {
            Name = GetType().Name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public override void Cancel()
        {
            base.Cancel();
            ClearActions();
        }

    }
}