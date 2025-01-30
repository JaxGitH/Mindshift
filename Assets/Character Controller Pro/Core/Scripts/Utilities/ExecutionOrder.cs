namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This class contains constant values related to execution order values. These values are used by the Core components to define its own execution order inside Unity.
    /// </summary>
    public static class ExecutionOrder
    {
        public const int CharacterActorOrder = 10;
        public const int CharacterGraphicsOrder = CharacterActorOrder + 10;
    }

}
