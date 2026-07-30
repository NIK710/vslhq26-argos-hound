namespace ArgosHound.Api.Services;

public abstract class LlmAnalysisException(
    string message,
    Exception? innerException = null) : Exception(message, innerException);

public sealed class LlmAnalysisTimeoutException(
    string message,
    Exception? innerException = null) : LlmAnalysisException(message, innerException);

public sealed class LlmAnalysisUnavailableException(
    string message,
    Exception? innerException = null) : LlmAnalysisException(message, innerException);

public sealed class LlmAnalysisOutputException(
    string message,
    Exception? innerException = null) : LlmAnalysisException(message, innerException);
