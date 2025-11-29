using System;
using Lib.Debugger;
using Lib.PriorityQueue;

namespace Lib.Tasks
{
    public interface ITaskPriorityQueue
    {
        void Enqueue(BaseTask task, IComparable priority);
        BaseTask Dequeue();
        int Count { get; }
        void Clear();
        BaseTask ActiveTask { get; }
        bool IsExecuting { get; }
        void StartExecution();
        void StopExecution();
    }

    public class TaskPriorityQueue : ITaskPriorityQueue
    {
        private readonly SimplePriorityQueue<BaseTask, int> _queue = new();
        private BaseTask _activeTask;
        private bool _isExecuting;
        private readonly IPriorityMap _priorityMap;
        public int Count => _queue.Count;
        public BaseTask ActiveTask => _activeTask;
        public bool IsExecuting => _isExecuting;

        public TaskPriorityQueue(IPriorityOrder priorityOrder)
        {
            _priorityMap = new PriorityMap(priorityOrder);
        }
        
        public void Enqueue(BaseTask task, IComparable priority)
        {
            if (task is null)
            {
                LogErr("Cannot enqueue a null task.");
                return;
            }
            var priorityInt = _priorityMap.GetPriority(priority);
            if (!_queue.Contains(task))
            {
                Log($"Enqueuing task: {task.Name} with priority: {priority}");
                _queue.Enqueue(task, priorityInt);
            }
            else
            {
                Log($"Task {task.Name} is already in the queue, updating priority to: {priority}");
                _queue.UpdatePriority(task, priorityInt);
            }
        }

        public void StartExecution()
        {
            if (_isExecuting)
            {
                Log(" Execution is already in progress.");
                return;
            }

            Execute();
        }

        private void Execute()
        {
            _isExecuting = true;  
            ExecuteNextTask();
        }

        public void StopExecution()
        {
            if (!_isExecuting)
            {
                LogHelper.LogWarning(" No execution in progress to stop.");
                return;
            }
            
            _isExecuting = false;
            if (_activeTask is not null)
            {
                if (!_activeTask.IsCompleted)
                {
                    _activeTask.Cancel();
                }
                _activeTask = null;
            }
            Log(" Stopped task execution.");
        }

        private void ExecuteNextTask()
        {
            if (!_isExecuting || _queue.Count == 0)
            {
                if (_queue.Count == 0)
                {
                    Log(" No more tasks to execute.");
                    _isExecuting = false;
                }
                return;
            }

            _activeTask = _queue.Dequeue();
            Log($"Executing task: {_activeTask.Name}");
            
            _activeTask.OnComplete += OnTaskComplete;
            _activeTask.Run();
        }

        private void OnTaskComplete()
        {
            if (_activeTask != null)
            {
                Log($" Task completed: {_activeTask.Name}");
                
                _activeTask.OnComplete -= OnTaskComplete;
                _activeTask = null;
            }
            
            ExecuteNextTask();
        }

        public BaseTask Dequeue()
        {
            if (_queue.Count == 0)
            {
                Log("empty, cannot dequeue.");
                return null;
            }
            return _queue.Dequeue();
        }

        public void Clear()
        {
            if (_isExecuting)
            {
                StopExecution();
            }

            if (_queue.Count != 0)
            {
                Log($"Clearing TaskPriorityQueue with {_queue.Count} items.");
                _queue.Clear();
            }
        }

        public bool IsTaskInQueue(BaseTask task)
        {
            return _queue.Contains(task);
        }

        public bool ContainsTaskWithName(string taskName)
        {
            foreach (var queuedTask in _queue)
            {
                if (queuedTask is not null && queuedTask.Name == taskName)
                {
                    return true;
                }
            }
            return false;
        }
        
        public BaseTask GetTaskWithName(string taskName)
        {
            foreach (var queuedTask in _queue)
            {
                if (queuedTask is not null && queuedTask.Name == taskName)
                {
                    return queuedTask;
                }
            }
            if (_activeTask is not null && _activeTask.Name == taskName)
            {
                return _activeTask;
            }
            return null;
        }

        private static void Log(string msg)
        {
            LogHelper.Log(msg, "TaskPriorityQueue");
        }

        private static void LogErr(string msg)
        {
            LogHelper.LogError(msg, "TaskPriorityQueue");
        }

    }
}