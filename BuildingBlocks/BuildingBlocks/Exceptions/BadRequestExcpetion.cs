namespace BuildingBlocks.Exceptions
{
    public class BadRequestExcpetion : Exception
    {
        public BadRequestExcpetion(string message) : base(message)
        {
        }
        public BadRequestExcpetion(string message, string details) : base(message)
        {
            Details = details;
        }
        public string? Details { get; set; }
    }
}
