using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Debugger;
using Lib.PriorityQueue;
using Lib.Tasks;

namespace Lib.ContextQueue
{
    public interface IQueueManager
    {
        void CreateContext(string contextType, IPriorityOrder priorityOrder);
        void RemoveContext(string contextType);
        bool SetCurrentContext(string contextType);
        bool EnqueueTask(string contextType, BaseTask task, IComparable priority);
        bool EnqueueTask(BaseTask task, IComparable priority);
        bool ClearContext(string contextType);
        void ActivateManager();
        void DeactivateManager();
        bool IsActive { get; }
        bool IsTaskInContext(string contextKey, BaseTask task);
        event Action<string, string> OnContextChanged;
    }

    public class QueueManager : IQueueManager
    {
        private readonly Dictionary<string, TaskPriorityQueue> _contextQueues = new();
        private string _currentContextName = string.Empty;
        private TaskPriorityQueue _currentContext = null;
        private bool _isActive = false;
        
        public event Action<string, string> OnContextChanged;
        public event Action<bool> OnManagerActiveChanged;
        
        public string CurrentContextName => _currentContextName;
        public bool IsActive => _isActive;
        public bool IsExecuting => _currentContext?.IsExecuting ?? false;

        public void CreateContext(string contextKey, IPriorityOrder priorityOrder)
        {
            if (HasContext(contextKey))
            {
                LogErr($" Context '{contextKey}' already exists");
                return;
            }

            _contextQueues[contextKey] = new TaskPriorityQueue(priorityOrder);
            Log($" Created context: {contextKey}");
        }

        public void RemoveContext(string contextKey)
        {
            if (!HasContext(contextKey))
            {
                return;
            }

            var queue = _contextQueues[contextKey];
            queue.StopExecution();
            queue.Clear();
            _contextQueues.Remove(contextKey);

            if (_currentContextName == contextKey)
            {
                _currentContextName = string.Empty;
                _currentContext = null;
            }

            Log($" Removed context: {contextKey}");
        }

        public bool SetCurrentContext(string contextKey)
        {
            if (string.IsNullOrEmpty(contextKey))
            {
                LogErr(" Cannot set null or empty context key");
                return false;
            }

            if (!_contextQueues.ContainsKey(contextKey))
            {
                LogErr(
                    $" Context '{contextKey}' does not exist. Create it first using CreateContext()");
                return false;
            }

            string oldContext = _currentContextName;
            _currentContextName = contextKey;
            _currentContext = _contextQueues[contextKey];
            if (_isActive && !_currentContext.IsExecuting)
            {
                _currentContext.StartExecution();
            }

            OnContextChanged?.Invoke(oldContext, _currentContextName);
            Log($" Context changed: {oldContext} --> {_currentContextName}");
            return true;
        }

        public bool EnqueueTask(string contextType, BaseTask task, IComparable priority)
        {
            if (string.IsNullOrEmpty(contextType))
            {
                LogErr(" Cannot enqueue task to null or empty context key");
                return false;
            }

            if (task == null)
            {
                LogErr("Cannot enqueue null task");
                return false;
            }

            if (!_contextQueues.ContainsKey(contextType))
            {
                LogErr($" Context '{contextType}' does not exist. Create it first using CreateContext()");
                return false;
            }

            var contextItem = _contextQueues[contextType];
            contextItem.Enqueue(task, priority);

            Log($" Enqueued '{task.Name}' --> '{contextType}'. Priority: {priority}");

            if (contextType == _currentContextName && _isActive && !contextItem.IsExecuting)
            {
                contextItem.StartExecution();
                Log($" Started execution in current context '{_currentContextName}'");
            }

            return true;
        }

        public bool EnqueueTask(BaseTask task, IComparable priority)
        {
            if (string.IsNullOrEmpty(_currentContextName))
            {
                LogErr(" No current context set. Set a context first using SetCurrentContext()");
                return false;
            }

            return EnqueueTask(_currentContextName, task, priority);
        }

        public int GetTaskCount(string contextKey)
        {
            if (!HasContext(contextKey))
                return 0;

            return _contextQueues[contextKey].Count;
        }

        public int GetCurrentContextTaskCount()
        {
            return GetTaskCount(_currentContextName);
        }

        public bool ClearContext(string contextType)
        {
            if (!HasContext(contextType))
            {
                LogErr($" Context '{contextType}' does not exist");
                return false;
            }

            var queue = _contextQueues[contextType];
            queue.StopExecution();
            queue.Clear();
            Log($" Cleared all tasks from context '{contextType}'");
            return true;
        }

        public bool ClearCurrentContext()
        {
            return ClearContext(_currentContextName);
        }

        public void ClearAllContexts()
        {
            foreach (var kvp in _contextQueues)
            {
                kvp.Value.StopExecution();
                kvp.Value.Clear();
            }

            Log(" Cleared all tasks from all contexts");
        }

        public bool StartContextExecution(string contextKey)
        {
            if (!HasContext(contextKey))
            {
                LogErr($" Context '{contextKey}' does not exist");
                return false;
            }

            if (contextKey == _currentContextName)
            {
                _contextQueues[contextKey].StartExecution();
                Log($" Started execution in context '{contextKey}'");
                return true;
            }

            LogErr(
                $" Cannot start execution in non-current context '{contextKey}'. Current context is '{_currentContextName}'");
            return false;
        }


        private bool StopContextExecution(string contextKey)
        {
            if (string.IsNullOrEmpty(contextKey) || !_contextQueues.ContainsKey(contextKey))
            {
                LogErr($" Context '{contextKey}' does not exist");
                return false;
            }

            _contextQueues[contextKey].StopExecution();
            Log($" Stopped execution in context '{contextKey}'");
            return true;
        }
        public List<string> GetContextKeys()
        {
            return _contextQueues.Keys.ToList();
        }

        public bool HasContext(string contextKey)
        {
            return !string.IsNullOrEmpty(contextKey) && _contextQueues.ContainsKey(contextKey);
        }

        public bool IsContextExecuting(string contextKey)
        {
            if (string.IsNullOrEmpty(contextKey) || !_contextQueues.ContainsKey(contextKey))
                return false;

            return _contextQueues[contextKey].IsExecuting;
        }

        public BaseTask GetContextActiveTask(string contextKey)
        {
            if (!HasContext(contextKey))
                return null;

            return _contextQueues[contextKey].ActiveTask;
        }

        public bool IsTaskInContext(string contextKey, BaseTask task)
        {
            if (!HasContext(contextKey) || task == null)
                return false;

            var queue = _contextQueues[contextKey];
            return queue.IsTaskInQueue(task);
        }

        public BaseTask GetTaskByName(string contextKey, string taskName)
        {
            if (!HasContext(contextKey) ||
                string.IsNullOrEmpty(taskName))
                return null;

            var queue = _contextQueues[contextKey];
            return queue.GetTaskWithName(taskName);
        }

        public bool RemoveTask(string contextKey, BaseTask task)
        {
            if (!HasContext(contextKey) || task == null)
                return false;

            LogErr(
                $" RemoveTask not supported by TaskPriorityQueue. Cannot remove task '{task.Name}' from context '{contextKey}'");
            return false;
        }

        public bool RemoveTask(BaseTask task)
        {
            return RemoveTask(_currentContextName, task);
        }

        public void ActivateManager()
        {
            if (_isActive)
            {
                Log(" Manager is already active");
                return;
            }

            _isActive = true;
            OnManagerActiveChanged?.Invoke(_isActive);
            Log(" Manager activated");

            if (!string.IsNullOrEmpty(_currentContextName) && _currentContext != null && 
                _currentContext.Count > 0 && !_currentContext.IsExecuting)
            {
                _currentContext.StartExecution();
            }
        }

        public void DeactivateManager()
        {
            if (!_isActive)
            {
                Log(" Manager is already inactive");
                return;
            }

            _isActive = false;
            OnManagerActiveChanged?.Invoke(_isActive);
            Log(" Manager deactivated");

            // Stop execution in current context if it's executing
            if (!string.IsNullOrEmpty(_currentContextName) && _currentContext != null && _currentContext.IsExecuting)
            {
                _currentContext.StopExecution();
                Log($" Stopped execution in current context '{_currentContextName}' after deactivation");
            }
        }

        private static void Log(string msg)
        {
            LogHelper.Log(msg, "QueueManager");
        }

        private static void LogErr(string msg)
        {
            LogHelper.LogError(msg, "QueueManager");
        }
    }
}