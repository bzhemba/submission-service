using Itmo.Dev.Platform.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubmissionService.Kafka.Contracts;

namespace SubmissionService.Presentation.Kafka.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection collection,
        IConfiguration configuration)
    {
        const string producerKey = "Presentation:Kafka:Producers";
        collection.AddPlatformKafka(
            kafka => kafka
                .ConfigureOptions(configuration.GetSection("Presentation:Kafka"))
                .AddProducer(b => b
                    .WithKey<DraftNotificationKey>()
                    .WithValue<DraftNotificationValue>()
                    .WithConfiguration(configuration.GetSection($"{producerKey}:DraftNotification"))
                    .SerializeKeyWithProto()
                    .SerializeValueWithProto()
                    .WithOutbox()));

        return collection;
    }
}