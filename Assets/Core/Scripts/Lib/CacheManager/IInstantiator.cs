using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Lib.CacheManager
{
    public interface IInstantiator
    {
        UniTask<bool> IsValid(IList<string> keys);
        UniTask<bool> DownloadKeys(IList<string> keys);
        UniTask<T> InstantiateAsync<T>(string key, Transform parent);
    }
}