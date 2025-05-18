namespace ElsaMina.Commands.Ai.TextToSpeech;

public interface IAiTextToSpeechProvider
{
    Task<string> GetTextToSpeechAudioUrlAsync(string text, VoiceType voiceType, CancellationToken cancellationToken = default);
}