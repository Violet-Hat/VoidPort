namespace VoidPort.Common
{
    public class Timer
    {
        public static int SecondsToFrames(int second)
        {
            //1 second = 60 frames
            return second * 60;
        }
    }
}