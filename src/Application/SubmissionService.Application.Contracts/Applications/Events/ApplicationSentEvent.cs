using Itmo.Dev.Platform.Events;

namespace SubmissionService.Application.Contracts.Applications.Events;

public record ApplicationSentEvent(long ApplicationId, DateTimeOffset SendingDate) : IEvent;