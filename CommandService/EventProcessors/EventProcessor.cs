using System.Text.Json;
using System.Windows.Input;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.EventProcessing;
using CommandService.Models;

namespace CommandService.EventProcessors
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            EventType eventType = DetermineEventType(message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEventType(string message)
        {
            Console.WriteLine("--> Determining Event");

            GenericEventDto? genericEventDto = JsonSerializer.Deserialize<GenericEventDto>(message);

            if(genericEventDto == null) {
                return EventType.Undetermined;
            }

            switch(genericEventDto.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                ICommandRepo repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                PlatformPublishDto? platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);
                
                try {
                    Platform? platform = _mapper.Map<Platform>(platformPublishDto);
                    if(!repo.ExternalPlatformExists(platform.ExternalId))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added!");
                    } else {
                        Console.WriteLine("--> Platform already exists...");
                    }
                } catch(Exception ex) {
                    Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                }
            }
        }

    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}