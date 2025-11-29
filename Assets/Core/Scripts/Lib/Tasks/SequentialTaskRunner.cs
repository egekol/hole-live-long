using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Lib.Tasks
{
    /// <summary>
    /// A task that executes multiple BaseTask instances sequentially
    /// </summary>
    public class SequentialTaskRunner : BaseTask
    {
        private readonly List<BaseTask> _tasks;

        /// <summary>
        /// Creates a new SequentialTaskRunner with the specified tasks
        /// </summary>
        /// <param name="tasks">The tasks to execute in sequence</param>
        public SequentialTaskRunner(IEnumerable<BaseTask> tasks)
        {
            _tasks = tasks?.ToList() ?? throw new ArgumentNullException(nameof(tasks));
        }

        /// <summary>
        /// Creates a new SequentialTaskRunner with the specified tasks
        /// </summary>
        /// <param name="tasks">The tasks to execute in sequence</param>
        public SequentialTaskRunner(params BaseTask[] tasks) : this((IEnumerable<BaseTask>)tasks)
        {
        }

        public override async UniTask<bool> ExecuteAsync()
        {
            try
            {
                foreach (var task in _tasks)
                {
                    if (task == null)
                        continue;

                    // Execute task and wait for completion
                    var result = await task.ExecuteAsync();

                    // If task failed, return false immediately
                    if (!result)
                    {
                        Complete();
                        return false;
                    }
                }

                // All tasks completed successfully
                Complete();
                return true;
            }
            catch (Exception e)
            {
                // Log error and complete with failure
                UnityEngine.Debug.LogError($"Error in SequentialTaskRunner: {e}");
                Complete();
                return false;
            }
        }
    }
}
