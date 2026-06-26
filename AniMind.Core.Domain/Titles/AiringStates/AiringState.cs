using AniMind.SharedKernel.Results;

namespace AniMind.Core.Domain.Titles.AiringStates;

public partial record AiringState
{
    public TitleStatus Status { get; }
    public DateTimeOffset? PremiereTimeUtc { get; }

    private AiringState(TitleStatus status, DateTimeOffset? premiereTimeUtc)
    {
        Status = status;
        PremiereTimeUtc = premiereTimeUtc;
    }

    public static Result<AiringState> Create(
        TitleStatus status,
        DateTimeOffset? premiereTimeUtc,
        DateTimeOffset currentTimeUtc)
    {
        return status switch
        {
            TitleStatus.Ongoing or TitleStatus.Released when !premiereTimeUtc.HasValue =>
                Result<AiringState>.Failure(Errors.PremiereTimeRequiredForStatus(status)),

            TitleStatus.Ongoing or TitleStatus.Released when premiereTimeUtc.Value > currentTimeUtc =>
                Result<AiringState>.Failure(
                    Errors.FuturePremiereNotAllowedForAiredTitle(status, premiereTimeUtc.Value)),

            TitleStatus.Announced when premiereTimeUtc.HasValue && premiereTimeUtc.Value < currentTimeUtc =>
                Result<AiringState>.Failure(
                    Errors.PastPremiereNotAllowedForAnnouncedTitle(premiereTimeUtc.Value)),

            _ => new AiringState(status, premiereTimeUtc)
        };
    }

    public Result<AiringState> StartAiring(DateTimeOffset currentTimeUtc) => Status != TitleStatus.Announced
        ? Errors.InvalidStatusTransition(Status, TitleStatus.Ongoing)
        : Create(TitleStatus.Ongoing, PremiereTimeUtc, currentTimeUtc);

    public Result<AiringState> FinishAiring(DateTimeOffset currentTimeUtc) => Status != TitleStatus.Ongoing
        ? Errors.InvalidStatusTransition(Status, TitleStatus.Released)
        : Create(TitleStatus.Released, PremiereTimeUtc, currentTimeUtc);
}
