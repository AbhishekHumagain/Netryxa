namespace Static.Application.Common.Interfaces
{
    public interface ISQSService
    {
        Task<bool> SendMessage(string queueUrl, string messageBody, string queueType);
    }
}
