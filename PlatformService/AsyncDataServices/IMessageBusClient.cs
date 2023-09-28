using PlatformService.Dtos;

namespace PlatformService.AsycnDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishDto platformPublishDto);
    }
}