using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Lib.PriorityQueue;
using Lib.Tasks;
using NUnit.Framework;

namespace Lib.ContextQueue.Tests
{
    [TestFixture]
    public class QueueManagerIntegrationTests
    {
        private QueueManager _queueManager;
        private TestPriorityOrder _priorityOrder;
        private List<string> _executionOrder;
        private const string ActiveContext = "ActiveContext";
        private const string InactiveContext = "InactiveContext";

        [SetUp]
        public void Setup()
        {
            _queueManager = new QueueManager();
            _priorityOrder = new TestPriorityOrder();
            _executionOrder = new List<string>();
        }



        [Test]
        public Task EnqueueOrder_WithDifferentPriorities_ShouldExecuteInCorrectOrder()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);

            var lowTask = new TrackingTask("Low", _executionOrder);
            var highTask = new TrackingTask("High", _executionOrder);
            var mediumTask = new TrackingTask("Medium", _executionOrder);

            // Act - Enqueue in reverse priority order
            _queueManager.EnqueueTask(lowTask, (int)TestPriority.Low);
            _queueManager.EnqueueTask(mediumTask, (int)TestPriority.Medium);
            _queueManager.EnqueueTask(highTask, (int)TestPriority.High);

            _queueManager.ActivateManager();

            // Assert - Should execute in High, Medium, Low order
            Assert.AreEqual(3, _executionOrder.Count);
            Assert.AreEqual("High", _executionOrder[0]);
            Assert.AreEqual("Medium", _executionOrder[1]);
            Assert.AreEqual("Low", _executionOrder[2]);
            return Task.CompletedTask;
        }

        [Test]
        public Task InactiveContextTasks_ShouldNotExecuteUntilContextActivated()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(InactiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            var activeTask = new TrackingTask("ActiveTask", _executionOrder);
            var inactiveTask1 = new TrackingTask("InactiveTask1", _executionOrder);
            var inactiveTask2 = new TrackingTask("InactiveTask2", _executionOrder);

            // Act - Add tasks to both contexts
            _queueManager.EnqueueTask(ActiveContext, activeTask, (int)TestPriority.High);
            _queueManager.EnqueueTask(InactiveContext, inactiveTask1, (int)TestPriority.High);
            _queueManager.EnqueueTask(InactiveContext, inactiveTask2, (int)TestPriority.Medium);


            // Assert - Only active task should have executed
            Assert.AreEqual(1, _executionOrder.Count);
            Assert.AreEqual("ActiveTask", _executionOrder[0]);
            Assert.IsFalse(_queueManager.IsContextExecuting(InactiveContext));
            Assert.AreEqual(2, _queueManager.GetTaskCount(InactiveContext));
            return Task.CompletedTask;
        }

        [Test]
        public Task ContextSwitch_ShouldStartExecutingPreviouslyInactiveTasks()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(InactiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);

            // Clear execution order to track only new executions
            _executionOrder.Clear();

            var activeTask = new TrackingTask("ActiveTask", _executionOrder);
            var inactiveTask1 = new TrackingTask("InactiveTask1", _executionOrder);
            var inactiveTask2 = new TrackingTask("InactiveTask2", _executionOrder);


            // Add tasks to both contexts
            _queueManager.EnqueueTask(ActiveContext, activeTask, (int)TestPriority.High);
            _queueManager.EnqueueTask(InactiveContext, inactiveTask1, (int)TestPriority.High);
            _queueManager.EnqueueTask(InactiveContext, inactiveTask2, (int)TestPriority.Medium);

            // Act - Switch to inactive context
            _queueManager.SetCurrentContext(InactiveContext);
            _queueManager.ActivateManager();

            // Assert - Previously inactive tasks should now execute
            Assert.AreEqual(2, _executionOrder.Count);
            Assert.AreEqual("InactiveTask1", _executionOrder[0]); // High priority first
            Assert.AreEqual("InactiveTask2", _executionOrder[1]); // Medium priority second
            
            return Task.CompletedTask;
        }

        [Test]
        public Task MixedPriorityContextSwitch_ShouldMaintainPriorityOrder()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(InactiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            // Add tasks with mixed priorities to inactive context
            var lowTask = new TrackingTask("Low", _executionOrder);
            var highTask = new TrackingTask("High", _executionOrder);
            var mediumTask = new TrackingTask("Medium", _executionOrder);

            _queueManager.EnqueueTask(InactiveContext, lowTask, (int)TestPriority.Low);
            _queueManager.EnqueueTask(InactiveContext, highTask, (int)TestPriority.High);
            _queueManager.EnqueueTask(InactiveContext, mediumTask, (int)TestPriority.Medium);

            // Verify tasks are queued but not executing
            Assert.AreEqual(3, _queueManager.GetTaskCount(InactiveContext));
            Assert.IsFalse(_queueManager.IsContextExecuting(InactiveContext));

            // Act - Switch to inactive context
            _queueManager.SetCurrentContext(InactiveContext);

            // Assert - Should execute in priority order
            Assert.AreEqual(3, _executionOrder.Count);
            Assert.AreEqual("High", _executionOrder[0]);
            Assert.AreEqual("Medium", _executionOrder[1]);
            Assert.AreEqual("Low", _executionOrder[2]);
            return Task.CompletedTask;
        }

        [Test]
        public Task MultipleContextSwitches_ShouldMaintainTaskIntegrity()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(InactiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            var context1Task = new TrackingTask("Context1Task", _executionOrder);
            var context2Task = new TrackingTask("Context2Task", _executionOrder);

            // Act - Add tasks and switch contexts multiple times
            _queueManager.EnqueueTask(ActiveContext, context1Task, (int)TestPriority.High);

            _queueManager.SetCurrentContext(InactiveContext);
            _queueManager.EnqueueTask(InactiveContext, context2Task, (int)TestPriority.High);

            _queueManager.SetCurrentContext(ActiveContext);
            
            // Add another task to first context
            var context1Task2 = new TrackingTask("Context1Task2", _executionOrder);
            _queueManager.EnqueueTask(ActiveContext, context1Task2, (int)TestPriority.High);

            // Assert
            Assert.AreEqual(3, _executionOrder.Count);
            Assert.AreEqual("Context1Task", _executionOrder[0]);
            Assert.AreEqual("Context2Task", _executionOrder[1]);
            Assert.AreEqual("Context1Task2", _executionOrder[2]);
            return Task.CompletedTask;
        }

        [Test]
        public void ContextCreation_MultipleContexts_ShouldCreateAsExpected()
        {
            // Act
            _queueManager.CreateContext("Context1", _priorityOrder);
            _queueManager.CreateContext("Context2", _priorityOrder);
            _queueManager.CreateContext("Context3", _priorityOrder);

            // Assert
            Assert.IsTrue(_queueManager.HasContext("Context1"));
            Assert.IsTrue(_queueManager.HasContext("Context2"));
            Assert.IsTrue(_queueManager.HasContext("Context3"));

            var contextKeys = new List<string>(_queueManager.GetContextKeys());
            Assert.AreEqual(3, contextKeys.Count);
            Assert.Contains("Context1", contextKeys);
            Assert.Contains("Context2", contextKeys);
            Assert.Contains("Context3", contextKeys);
        }

        [Test]
        public void ManagerActivation_WithTasksInCurrentContext_ShouldStartExecution()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            var task1 = new TrackingTask("Task1", _executionOrder);
            var task2 = new TrackingTask("Task2", _executionOrder);

            // Add tasks while manager is inactive
            _queueManager.EnqueueTask(task2, (int)TestPriority.Medium);
            _queueManager.EnqueueTask(task1, (int)TestPriority.High);

            // Verify tasks are queued but not executing
            Assert.IsFalse(_queueManager.IsActive);
            Assert.IsFalse(_queueManager.IsContextExecuting(ActiveContext));
            Assert.AreEqual(2, _queueManager.GetCurrentContextTaskCount());
            Assert.AreEqual(0, _executionOrder.Count);


            // Act - Activate manager
            _queueManager.ActivateManager();

            // Assert - Tasks should start executing
            Assert.AreEqual("Task1", _executionOrder[0]); // High priority first
            Assert.AreEqual("Task2", _executionOrder[1]); // Medium priority second
            Assert.IsTrue(_queueManager.IsActive);
            Assert.AreEqual(2, _executionOrder.Count);
        }

        [Test]
        public void ManagerDeactivation_WithRunningTasks_ShouldStopExecution()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            var task1 = new TrackingTask("Task1", _executionOrder);
            var task2 = new TrackingTask("Task2", _executionOrder);

            _queueManager.EnqueueTask(task1, (int)TestPriority.High);
            _queueManager.EnqueueTask(task2, (int)TestPriority.Medium);

            // Verify tasks are executing
            Assert.AreEqual(2, _executionOrder.Count);

            // Act - Deactivate manager
            _queueManager.DeactivateManager();

            // Assert - Execution should stop
            Assert.IsFalse(_queueManager.IsActive);
            Assert.IsFalse(_queueManager.IsContextExecuting(ActiveContext));

            // Add another task - should not execute
            _executionOrder.Clear();
            var task3 = new TrackingTask("Task3", _executionOrder);
            _queueManager.EnqueueTask(task3, (int)TestPriority.High);

            Assert.AreEqual(0, _executionOrder.Count);
            Assert.IsFalse(_queueManager.IsContextExecuting(ActiveContext));
            Assert.AreEqual(1, _queueManager.GetCurrentContextTaskCount());
        }

        #region Helper Classes

        private class TrackingTask : BaseTask
        {
            private readonly List<string> _executionOrder;

            public TrackingTask(string name, List<string> executionOrder)
            {
                SetName(name);
                _executionOrder = executionOrder;
            }

            public override UniTask<bool> ExecuteAsync()
            {
                _executionOrder.Add(Name);
                UnityEngine.Debug.Log($"[TrackingTask] '{Name}' executed, added to execution order. Count: {_executionOrder.Count}");
                Complete();
                return UniTask.FromResult(true);
            }
        }

        private class TestPriorityOrder : IPriorityOrder
        {
            public IList<IComparable> PriorityOrderList { get; } = new List<IComparable>
            {
                (int)TestPriority.High,
                (int)TestPriority.Medium,
                (int)TestPriority.Low
            };
        }

        private enum TestPriority
        {
            High = 0,
            Medium = 1,
            Low = 2
        }

        #endregion
    }
}
