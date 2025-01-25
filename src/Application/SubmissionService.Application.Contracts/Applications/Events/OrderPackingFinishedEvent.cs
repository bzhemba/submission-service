using Itmo.Dev.Platform.Events;

namespace SubmissionService.Application.Contracts.Applications.Events;

public record OrderPackingFinishedEvent(
    long OrderId,
    DateTimeOffset FinishedAt,
    bool IsSuccessful,
    string? FailureReason) : IEvent;