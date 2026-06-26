using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.AiringStates;

public partial record AiringState
{
    public static class Errors
    {
        public static Error PremiereTimeRequiredForStatus(TitleStatus status) => Error.Validation(
            "AiringState.PremiereTimeRequiredForStatus",
            $"A premiere time is required when creating a title with '{status}' status.",
            new Dictionary<string, object> { { "status", status.ToString() } });

        public static Error FuturePremiereNotAllowedForAiredTitle(
            TitleStatus currentStatus,
            DateTimeOffset premiereTimeUtc) =>
            Error.Validation(
                "Title.FuturePremiereNotAllowedForAiredTitle",
                $"Cannot set a future premiere time for a title that is already '{currentStatus}'. Premiere time: {premiereTimeUtc}.",
                new Dictionary<string, object>
                {
                    { "currentStatus", currentStatus.ToString() },
                    { "premiereTimeUtc", premiereTimeUtc }
                });

        public static Error PastPremiereNotAllowedForAnnouncedTitle(DateTimeOffset premiereTimeUtc) =>
            Error.Validation(
                "AiringState.PastPremiereNotAllowedForAnnouncedTitle",
                $"An 'Announced' title cannot have a premiere time in the past. Premiere time: {premiereTimeUtc}.",
                new Dictionary<string, object> { { "premiereTimeUtc", premiereTimeUtc } });

        public static Error InvalidStatusTransition(TitleStatus current, TitleStatus target) =>
            Error.Conflict(
                "Title.InvalidStatusTransition",
                $"Cannot transition title from status '{current}' to '{target}'.",
                new Dictionary<string, object>
                {
                    { "currentStatus", current.ToString() },
                    { "targetStatus", target.ToString() }
                });
    }
}
