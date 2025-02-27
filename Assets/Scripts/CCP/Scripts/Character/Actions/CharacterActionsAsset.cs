﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mindshift.CharacterControllerPro.Implementation
{

    /// <summary>
    /// This ScriptableObject contains all the names used as input actions by the human brain. The name of the action will matters depending on the input handler used.
    /// </summary>
    [CreateAssetMenu(menuName = "Scripts/CCP/Scripts/Character/Actions/Character actions asset")]
    public class CharacterActionsAsset : ScriptableObject
    {

        [SerializeField]
        string[] boolActions;

        [SerializeField]
        string[] floatActions;

        [SerializeField]
        string[] vector2Actions;


    }

}