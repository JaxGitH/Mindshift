using UnityEngine;
using System.Collections.Generic;

namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// This input handler implements the input detection following the Unity's Input Manager convention. This scheme is used for desktop games.
    /// </summary>
    public class UnityInputHandler : InputHandler
    {
        struct Vector2Action
        {
            public string x;
            public string y;

            public Vector2Action(string x, string y)
            {
                this.x = x;
                this.y = y;
            }
        }

        Dictionary<string, Vector2Action> vector2Actions = new Dictionary<string, Vector2Action>();

        public override bool GetBool(string actionName)
        {
            bool output = false;
            try
            {
                output = Input.GetButton(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }

        public override float GetFloat(string actionName)
        {
            float output = default(float);
            try
            {
                output = Input.GetAxis(actionName);
            }
            catch (System.Exception)
            {
                PrintInputWarning(actionName);
            }

            return output;
        }

        public override Vector2 GetVector2(string actionName)
        {
            // Not officially supported	by Unity's input manager.
            // Example : "Movement"  splits into "Movement X" and "Movement Y"

            Vector2Action vector2Action;

            bool found = vector2Actions.TryGetValue(actionName, out vector2Action);

            if (!found)
            {
                vector2Action = new Vector2Action(
                    string.Concat(actionName, " X"),
                    string.Concat(actionName, " Y")
                );

                vector2Actions.Add(actionName, vector2Action);
            }

            Vector2 output = default(Vector2);

            try
            {
                output = new Vector2(Input.GetAxis(vector2Action.x), Input.GetAxis(vector2Action.y));
            }
            catch (System.Exception)
            {
                PrintInputWarning(vector2Action.x, vector2Action.y);
            }

            return output;
        }

        void PrintInputWarning(string actionName)
        {
            Debug.LogWarning($"{actionName} action not found! Please make sure this action is included in your input settings (axis). If you're only testing the demo scenes from " +
            "Character Controller Pro please load the input preset included at \"Character Controller Pro/OPEN ME/Presets/.");
        }

        void PrintInputWarning(string actionXName, string actionYName)
        {
            Debug.LogWarning($"{actionXName} and/or {actionYName} actions not found! Please make sure both of these actions are included in your input settings (axis). If you're only testing the demo scenes from " +
            "Character Controller Pro please load the input preset included at \"Character Controller Pro/OPEN ME/Presets/.");
        }
    }

}
