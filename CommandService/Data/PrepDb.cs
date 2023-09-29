using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{

    public static class PrepDb
    {

        public static void PrepPopulation(IApplicationBuilder applicationBuilder) {

            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope()) {

                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient?.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }

        }

        private static void SeedData(ICommandRepo? repo, IEnumerable<Platform>? platforms) {

            if (repo == null || platforms == null) {
                Console.WriteLine("--> CommandRepo Or Platforms is null");
                return;
            }

            Console.WriteLine("--> Seeding new platforms...");

            foreach(var platform in platforms) {

                if(!repo.ExternalPlatformExists(platform.ExternalId)) {

                    repo.CreatePlatform(platform);
                }

                repo.SaveChanges();
            }

        }

    }

}