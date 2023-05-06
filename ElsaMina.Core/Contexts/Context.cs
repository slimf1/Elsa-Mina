﻿using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;

namespace ElsaMina.Core.Contexts;

public abstract class Context
{
    private readonly IConfigurationManager _configurationManager;
    
    public IBot Bot { get; }
    public string Target { get; }
    public IUser Sender { get; }
    public string Command { get; }

    protected Context(IConfigurationManager configurationManager,
        IBot bot,
        string target,
        IUser sender,
        string command)
    {
        _configurationManager = configurationManager;
        
        Bot = bot;
        Target = target;
        Sender = sender;
        Command = command;
    }

    public bool IsSenderWhitelisted => _configurationManager
        .Configuration?
        .Whitelist?
        .Contains(Sender.UserId) == true;
    
    public void SendHtmlPage(string pageName, string html)
    {
        Bot.Say(RoomId, $"/sendhtmlpage {Sender.UserId}, {pageName}, {html}");
    }

    public abstract string RoomId { get; }
    public abstract ContextType Type { get; }
    public abstract bool IsPm { get; }
    public abstract CultureInfo Locale { get; set; }
    
    public abstract bool HasSufficientRank(char requiredRank);
    public abstract void Reply(string message);
    public abstract void SendHtml(string html, string roomId = null);
    public abstract void SendUpdatableHtml(string htmlId, string html, bool isChanging);
}