using UnityEngine;

namespace Lib.CustomAttributes.Scripts
{

    public interface IButtonAttribute
    {
        string Error { get; }
        bool PerformCheck(Object obj);
    }
}
