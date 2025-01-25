using Itmo.Dev.Platform.Events;

namespace SubmissionService.Application.Contracts.Applications.Events;

public record OrderApprovalResultReceivedEvent(
    long OrderId,
    bool IsApproved,
    string CreatedBy,
    DateTimeOffset CreatedAt) : IEvent;