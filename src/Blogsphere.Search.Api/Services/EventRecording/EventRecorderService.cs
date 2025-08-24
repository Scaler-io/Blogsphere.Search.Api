namespace Blogsphere.Search.Api.Services.EventRecording;

public class EventRecorderService(
    IDataTableRepository<EventPublishHistory> eventPublishHistoryRepository,
    ILogger logger) : IEventRecorderService
{
    private readonly IDataTableRepository<EventPublishHistory> _eventPublishHistoryRepository = eventPublishHistoryRepository;
    private readonly ILogger _logger = logger;

    public async Task<Result<bool>> CreateEvent(EventPublishHistory history)
    {
        _logger.Here().MethodEntered();
        try
        {
            await _eventPublishHistoryRepository.AddAsync(history);
            _logger.Here().Information("History record created");
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }
        catch
        {
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }
    }

    public async Task<Result<EventPublishHistory>> GetEvent(string correlationId)
    {
        _logger.Here().MethodEntered();
        var result = await _eventPublishHistoryRepository.GetAsync(correlationId, correlationId);
        if(result == null) return Result<EventPublishHistory>.Failure(ErrorCodes.NotFound, ErrorMessages.NotFound);

        _logger.Here().WithCorrelationId(correlationId).Information("History record found with {correlationId}", correlationId);
        _logger.Here().MethodExited();
        return Result<EventPublishHistory>.Success(result);
    }

    public async Task<Result<bool>> UpdateGenericEvent(EventPublishHistory history)
    {
        _logger.Here().MethodEntered();
        try
        {
            await _eventPublishHistoryRepository.UpdateAsync(history);
            _logger.Here().Information("History record updated");
            _logger.Here().MethodExited();
            return Result<bool>.Success(true);
        }
        catch
        {
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }
    }
}
