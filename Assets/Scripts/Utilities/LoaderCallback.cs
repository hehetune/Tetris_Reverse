using System;
using UnityEngine;

namespace Utilities
{
    public class LoaderCallback : MonoBehaviour
    {
        private void Awake()
        {
            Loader.LoaderCallback();
        }
    }
}