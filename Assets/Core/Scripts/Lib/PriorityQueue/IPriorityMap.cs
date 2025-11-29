using System;

namespace Lib.PriorityQueue
{
    public interface IPriorityMap
    {
        int GetPriority(IComparable priority);
    }
}