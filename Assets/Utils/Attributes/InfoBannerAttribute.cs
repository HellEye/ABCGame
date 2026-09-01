public class InfoBannerAttribute : System.Attribute
{
    public string Message { get; }

    public InfoBannerAttribute(string message)
    {
        Message = message;
    }
}