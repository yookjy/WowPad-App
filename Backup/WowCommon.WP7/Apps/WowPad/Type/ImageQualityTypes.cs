
namespace Apps.WowPad.Type
{
    public enum ImageQualityTypes
    {
#if WP7
        Low = 5,
        Medium = 7,
        High = 9
#else
        Low = 5,
        Medium = 7,
        High = 10
#endif
    }
}
