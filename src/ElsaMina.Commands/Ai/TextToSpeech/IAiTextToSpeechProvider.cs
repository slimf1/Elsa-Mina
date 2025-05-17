namespace ElsaMina.Commands.Ai.TextToSpeech;

public interface IAiTextToSpeechProvider
{
    Task<Stream> GetTextToSpeechAudioStreamAsync(string text, CancellationToken cancellationToken = default);
}