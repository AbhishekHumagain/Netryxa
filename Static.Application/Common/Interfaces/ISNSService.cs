using Static.Application.Common.Dto;

namespace Static.Application.Common.Interfaces;

public interface ISnsService
{
    Task<bool> IvysnsReporting(SnsNotificationRequest request);

}