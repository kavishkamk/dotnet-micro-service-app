using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{

    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform>? ReturnAllPlatforms()
        {

            var grpcPlatform = _configuration["GrpcPlatform"];

            if (grpcPlatform == null)
            {
                Console.WriteLine("--> gRPC Platform is null");
            } else {
                 Console.WriteLine($"--> Calling GRPC Service {grpcPlatform}");

                var channel = GrpcChannel.ForAddress(grpcPlatform);
                var client = new GrpcPlatform.GrpcPlatformClient(channel);
                var request = new GetAllRequest();

                try {
                    
                    var reply = client.GetAllPlatforms(request);
                    return _mapper.Map<IEnumerable<Platform>>(reply.Platform);

                } catch(Exception ex) {
                    Console.WriteLine($"--> Could not call GRPC Server {ex.Message}");
                }
            }

             return null;

           
        }
    }

}