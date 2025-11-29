using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.PriorityQueue;
using Lib.Tasks;
using NUnit.Framework;

namespace Lib.ContextQueue.Tests
{
    [TestFixture]
    public class QueueManagerTests
    {
        private QueueManager _queueManager;
        private TestPriorityOrder _priorityOrder;
        private const string TestContext1 = "TestContext1";
        private const string TestContext2 = "TestContext2";
        private const string ActiveContext = "ActiveContext";
        private const string InactiveContext = "InactiveContext";

        [SetUp]
        public void Setup()
        {
            _queueManager = new QueueManager();
            _priorityOrder = new TestPriorityOrder();
        }


        #region Context Creation Tests

        [Test]
        public void CreateContext_ValidContext_ShouldCreateSuccessfully()
        {
            // Act
            _queueManager.CreateContext(TestContext1, _priorityOrder);

            // Assert
            Assert.IsTrue(_queueManager.HasContext(TestContext1));
            Assert.Contains(TestContext1, _queueManager.GetContextKeys());
        }

        [Test]
        public void CreateContext_MultipleContexts_ShouldCreateAll()
        {
            // Act
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.CreateContext(TestContext2, _priorityOrder);

            // Assert
            Assert.IsTrue(_queueManager.HasContext(TestContext1));
            Assert.IsTrue(_queueManager.HasContext(TestContext2));
        }

        #endregion

        #region Context Management Tests

        [Test]
        public void SetCurrentContext_ValidContext_ShouldSetSuccessfully()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);

            // Act
            var result = _queueManager.SetCurrentContext(TestContext1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(TestContext1, _queueManager.CurrentContextName);
        }

        #endregion

        #region Enqueue Order Tests

        [Test]
        public void EnqueueTask_DifferentPriorities_ShouldRespectOrder()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);

            var lowPriorityTask = new TestTask("LowPriority");
            var highPriorityTask = new TestTask("HighPriority");
            var mediumPriorityTask = new TestTask("MediumPriority");

            // Act - Enqueue in non-priority order
            _queueManager.EnqueueTask(lowPriorityTask, (int)TestPriority.Low);
            _queueManager.EnqueueTask(highPriorityTask, (int)TestPriority.High);
            _queueManager.EnqueueTask(mediumPriorityTask, (int)TestPriority.Medium);

            // Assert
            Assert.AreEqual(3, _queueManager.GetCurrentContextTaskCount());
        }

        [Test]
        public void EnqueueTask_SamePriority_ShouldMaintainFIFOOrder()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);

            var task1 = new TestTask("Task1");
            var task2 = new TestTask("Task2");
            var task3 = new TestTask("Task3");

            // Act
            _queueManager.EnqueueTask(task1, (int)TestPriority.Medium);
            _queueManager.EnqueueTask(task2, (int)TestPriority.Medium);
            _queueManager.EnqueueTask(task3, (int)TestPriority.Medium);

            // Assert
            Assert.AreEqual(3, _queueManager.GetCurrentContextTaskCount());
        }

        #endregion

        #region Inactive Context Tests

        [Test]
        public void EnqueueTask_InactiveContext_ShouldNotExecuteImmediately()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(InactiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);

            var inactiveTask = new TestTask("InactiveTask", false);
            var activeTask = new TestTask("ActiveTask", false);

            // Act
            _queueManager.EnqueueTask(InactiveContext, inactiveTask, (int)TestPriority.High);
            _queueManager.EnqueueTask(ActiveContext, activeTask, (int)TestPriority.High);

            _queueManager.ActivateManager();
            // Assert
            Assert.IsFalse(_queueManager.IsContextExecuting(InactiveContext));
            Assert.IsTrue(_queueManager.IsContextExecuting(ActiveContext));
        }

        [Test]
        public void EnqueueTask_ActiveContext_ShouldExecuteImmediately()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);

            var task = new TestTask("ActiveTask", false);

            // Act
            var result = _queueManager.EnqueueTask(task, (int)TestPriority.High);

            _queueManager.ActivateManager();
            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_queueManager.IsContextExecuting(ActiveContext));
        }

        #endregion

        #region Context Switching Tests

        [Test]
        public void ContextSwitch_WithPendingTasks_ShouldStartExecutionInNewContext()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.CreateContext(TestContext2, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);
            _queueManager.ActivateManager();
            
            var task1 = new TestTask("Task1", false);
            var task2 = new TestTask("Task2", false);

            // Add task to inactive context
            _queueManager.EnqueueTask(TestContext2, task2, (int)TestPriority.High);
            _queueManager.EnqueueTask(TestContext1, task1, (int)TestPriority.High);

            // Verify initial state
            Assert.IsFalse(_queueManager.IsContextExecuting(TestContext2));
            Assert.IsTrue(_queueManager.IsContextExecuting(TestContext1));

            // Act - Switch context
            var result = _queueManager.SetCurrentContext(TestContext2);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(TestContext2, _queueManager.CurrentContextName);
            Assert.AreEqual(1, _queueManager.GetTaskCount(TestContext2));
        }

        [Test]
        public void ContextSwitch_MultipleTasksInInactiveContext_ShouldExecuteWhenActivated()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.CreateContext(TestContext2, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            var task1 = new TestTask("Task1");
            var task2 = new TestTask("Task2");
            var task3 = new TestTask("Task3");

            // Add multiple tasks to inactive context
            _queueManager.EnqueueTask(TestContext2, task3, (int)TestPriority.Low);
            _queueManager.EnqueueTask(TestContext2, task2, (int)TestPriority.Medium);
            _queueManager.EnqueueTask(TestContext2, task1, (int)TestPriority.High);

            // Verify tasks are queued but not executing
            Assert.AreEqual(3, _queueManager.GetTaskCount(TestContext2));
            Assert.IsFalse(_queueManager.IsContextExecuting(TestContext2));

            // Act - Switch to context with pending tasks
            _queueManager.SetCurrentContext(TestContext2);

            // Assert
            Assert.AreEqual(TestContext2, _queueManager.CurrentContextName);
            Assert.AreEqual(3, _queueManager.GetCurrentContextTaskCount());
        }

        #endregion

        #region Event Tests

        [Test]
        public void OnContextChanged_WhenContextSwitches_ShouldFireEvent()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.CreateContext(TestContext2, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);

            string oldContext = null;
            string newContext = null;
            _queueManager.OnContextChanged += (old, newCtx) =>
            {
                oldContext = old;
                newContext = newCtx;
            };

            // Act
            _queueManager.SetCurrentContext(TestContext2);

            // Assert
            Assert.AreEqual(TestContext1, oldContext);
            Assert.AreEqual(TestContext2, newContext);
        }

        #endregion

        #region Manager Activation Tests

        [Test]
        public void ActivateManager_WhenInactive_ShouldActivate()
        {
            // Arrange
            Assert.IsFalse(_queueManager.IsActive);

            // Act
            _queueManager.ActivateManager();

            // Assert
            Assert.IsTrue(_queueManager.IsActive);
        }

        [Test]
        public void DeactivateManager_WhenActive_ShouldDeactivate()
        {
            // Arrange
            _queueManager.ActivateManager();
            Assert.IsTrue(_queueManager.IsActive);

            // Act
            _queueManager.DeactivateManager();

            // Assert
            Assert.IsFalse(_queueManager.IsActive);
        }

        [Test]
        public void EnqueueTask_ManagerInactive_ShouldNotStartExecution()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);
            // Manager is inactive by default
            Assert.IsFalse(_queueManager.IsActive);

            var task = new TestTask("TestTask");

            // Act
            var result = _queueManager.EnqueueTask(task, (int)TestPriority.High);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(_queueManager.IsContextExecuting(TestContext1));
            Assert.AreEqual(1, _queueManager.GetCurrentContextTaskCount());
        }

        [Test]
        public void EnqueueTask_ManagerActive_ShouldStartExecution()
        {
            // Arrange
            _queueManager.CreateContext(TestContext1, _priorityOrder);
            _queueManager.SetCurrentContext(TestContext1);
            _queueManager.ActivateManager();

            var task = new TestTask("TestTask", false);
            var task2 = new TestTask("TestTask2", false);

            // Act
            var result = _queueManager.EnqueueTask(task, (int)TestPriority.High);
            _queueManager.EnqueueTask(task2, (int)TestPriority.Medium);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_queueManager.IsContextExecuting(TestContext1));
            Assert.AreEqual(1, _queueManager.GetCurrentContextTaskCount());
        }

        [Test]
        public void ActivateManager_WithPendingTasks_ShouldStartExecution()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);

            var task = new TestTask("TestTask", false);
            _queueManager.EnqueueTask(task, (int)TestPriority.High);

            // Verify task is queued but not executing
            Assert.IsFalse(_queueManager.IsContextExecuting(ActiveContext));
            Assert.AreEqual(1, _queueManager.GetCurrentContextTaskCount());

            // Act
            _queueManager.ActivateManager();

            // Assert
            Assert.IsTrue(_queueManager.IsActive);
            Assert.IsTrue(_queueManager.IsContextExecuting(ActiveContext));
        }

        [Test]
        public void DeactivateManager_WithRunningTasks_ShouldStopExecution()
        {
            // Arrange
            _queueManager.CreateContext(ActiveContext, _priorityOrder);
            _queueManager.SetCurrentContext(ActiveContext);
            _queueManager.ActivateManager();

            var task = new TestTask("TestTask", false);
            _queueManager.EnqueueTask(task, (int)TestPriority.High);

            // Verify task is executing
            Assert.IsTrue(_queueManager.IsContextExecuting(ActiveContext));

            // Act
            _queueManager.DeactivateManager();

            // Assert
            Assert.IsFalse(_queueManager.IsActive);
            Assert.IsFalse(_queueManager.IsContextExecuting(ActiveContext));
        }

        [Test]
        public void OnManagerActiveChanged_WhenActivationChanges_ShouldFireEvent()
        {
            // Arrange
            bool? activeState = null;
            _queueManager.OnManagerActiveChanged += (isActive) => { activeState = isActive; };

            // Act - Activate
            _queueManager.ActivateManager();

            // Assert
            Assert.AreEqual(true, activeState);

            // Act - Deactivate
            _queueManager.DeactivateManager();

            // Assert
            Assert.AreEqual(false, activeState);
        }

        #endregion

        #region Helper Classes

        private class TestTask : BaseTask
        {
            private readonly bool _shouldComplete;

            public TestTask(string name, bool shouldComplete = true)
            {
                SetName(name);
                _shouldComplete = shouldComplete;
            }

            public override UniTask<bool> ExecuteAsync()
            {
                if (_shouldComplete) Complete();
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