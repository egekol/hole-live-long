using System;
using System.Collections.Generic;

namespace Lib.PriorityQueue
{
    public class PriorityMap : IPriorityMap
    {
        private readonly Dictionary<IComparable, int> _map = new();

        public PriorityMap(IPriorityOrder priorityList)
        {
            var priorityOrder = priorityList.PriorityOrderList;
            for (int i = 0; i < priorityOrder.Count; i++)
            {
                var priority = priorityOrder[i];
                if (!_map.ContainsKey(priority))
                {
                    _map[priority] = PriorityOrderCalculator.Calculate(i);
                }
            }
        }
        
        public int GetPriority(IComparable priority)
        {
            if (priority is null)
            {
                return int.MaxValue;
            }

            return _map.GetValueOrDefault(priority, int.MaxValue);
        }
    }
    
    public interface IPriorityOrder
    {
        IList<IComparable> PriorityOrderList { get; }
    }
    
    public static class PriorityOrderCalculator
    {
        public static int Calculate(int basePriority)
        {
            return (basePriority + 1) * 100; // Multiply by 100 to allow for finer adjustments later
        }
    }
}