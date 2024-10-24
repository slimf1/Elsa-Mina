﻿namespace ElsaMina.Core.Services.Formats;

public interface IFormatsManager
{
    IEnumerable<string> Formats { get; }
    void ParseFormatsFromReceivedLine(string message);
    string GetCleanFormat(string format);
}