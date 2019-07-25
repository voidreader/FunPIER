namespace BallBlaster
{

    public interface IPlayerInput
    {

        bool IsPointerDown();
        bool IsPointerUp();

        float GetTouchPointX();
        bool IsShootPressed();

    }

}