using Lightbug.Utilities;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{
    public class CharacterCollisions2D : CharacterCollisions
    {
        protected override void Awake()
        {
            base.Awake();

            PhysicsComponent = gameObject.AddComponent<PhysicsComponent2D>();
        }

        public override float ContactOffset => Physics2D.defaultContactOffset;
        public override float CollisionRadius => CharacterActor.BodySize.x / 2f;
    }
}
