namespace MySpot.Api.Exception
{
    public sealed class EmptyLicensePlateException : CustomException
    {
        public EmptyLicensePlateException() : base("License plate is empty")
        { 
        }

    }
}