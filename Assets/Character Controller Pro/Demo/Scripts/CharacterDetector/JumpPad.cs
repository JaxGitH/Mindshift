using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class JumpPad : CharacterDetector
    {
        public bool useLocalSpace = true;
        public Vector3 direction = Vector3.up;
        public float jumpPadVelocity = 10f;

        protected override void ProcessEnterAction(CharacterActor characterActor)
        {
            if (characterActor.GroundObject != gameObject)
                return;

            characterActor.ForceNotGrounded();

            Vector3 direction = useLocalSpace ? transform.TransformDirection(this.direction) : this.direction;
            characterActor.Velocity += direction * jumpPadVelocity;
        }

        protected override void ProcessStayAction(CharacterActor characterActor)
        {
            ProcessEnterAction(characterActor);
        }

        private void OnDrawGizmos()
        {
            Vector3 direction = useLocalSpace ? transform.TransformDirection(this.direction) : this.direction;
            //Gizmos.DrawRay(transform.position, direction * 2f);
            CustomUtilities.DrawArrowGizmo(transform.position, transform.position + direction * 2f, Color.red);
        }

    }

}
