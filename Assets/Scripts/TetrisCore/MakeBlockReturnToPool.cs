using System;
using UnityEngine;

namespace TetrisCore
{
    public class MakeBlockReturnToPool : MonoBehaviour
    {
        [SerializeField] private Board _board;
        private int targetY = 0;

        private void FixedUpdate()
        {
            if (transform.position.y > targetY)
            {
                _board.ClearRowsBelowHeight(targetY);
                targetY++;
            }
        }
    }
}
