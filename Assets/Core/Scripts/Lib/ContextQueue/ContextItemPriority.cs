using System;
using System.Collections.Generic;
using System.Linq;
using Lib.PriorityQueue;
using Unity.Collections;
using UnityEngine;

namespace Lib.ContextQueue
{

    [CreateAssetMenu(fileName = "ContextItemPriority", menuName = "GameSettings/PrioritySettings", order = 0)]
    public class ContextItemPriority : ScriptableObject, IPriorityOrder
    {
        [SerializeField] private List<ContextItemPriorityData> _priorityOrder = new();
        public IList<IComparable> PriorityOrderList => _priorityOrder.Cast<IComparable>().ToList();

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = 0; i < _priorityOrder.Count; i++)
            {
                var data = _priorityOrder[i];
                data.Priority = PriorityOrderCalculator.Calculate(i);
            }
        }
#endif
    }

    [Serializable]
    public class ContextItemPriorityData : IComparable, IComparable<ContextItemPriorityData>
    {
        public string ContextKey;
        [ReadOnly] public int Priority;

        public int CompareTo(object obj)
        {
            if (obj is ContextItemPriorityData other)
                return CompareTo(other);
            throw new ArgumentException("Object is not a ContextItemPriorityData");
        }

        public int CompareTo(ContextItemPriorityData other)
        {
            if (other == null) return 1;
            return string.Compare(ContextKey, other.ContextKey, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}