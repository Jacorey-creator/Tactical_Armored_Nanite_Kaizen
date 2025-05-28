using Assets.Scripts.Enviornment.MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class UnityPlayer : IPlayer
    {
        private Transform playerTransform;
        public Vector3 Position => playerTransform != null ? playerTransform.position : Vector3.zero;
        public bool IsValid => playerTransform != null;

        public event Action<Vector3> OnPositionChanged;

        private Vector3 lastPosition;

        public UnityPlayer(Transform transform)
        {
            playerTransform = transform;
            lastPosition = Position;
        }

        public void Update()
        {
            if (IsValid && Vector3.Distance(lastPosition, Position) > 0.1f)
            {
                lastPosition = Position;
                OnPositionChanged?.Invoke(Position);
            }
        }
    }
}
