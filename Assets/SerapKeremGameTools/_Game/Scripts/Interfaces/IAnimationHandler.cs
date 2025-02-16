namespace SerapKeremGameTools._Game._TopDownCharacterSystem
{
    /// <summary>
    /// Interface for managing animations in a modular and extensible way.
    /// </summary>
    public interface IAnimationHandler
    {
        void UpdateMovementAnimation(float speed);
        void PlayTriggerAnimation(string parameterName);
    }
}
