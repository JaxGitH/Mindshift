﻿using UnityEngine;

namespace Mindshift.CharacterControllerPro.Implementation
{
    public interface IUIVector2Action : IUIAction
    { 
        Vector2 Vector2Value { get; }
    }
}

