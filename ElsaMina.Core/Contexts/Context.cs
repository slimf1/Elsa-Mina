﻿using System.Globalization;
using ElsaMina.Core.Bot;
using ElsaMina.Core.Models;
using ElsaMina.Core.Services.Config;
using ElsaMina.Core.Services.Resources;

namespace ElsaMina.Core.Contexts;

public abstract class Context : IContext
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IResourcesService _resourcesService;
    
    public IBot Bot { get; }
    public string Target { get; }
    public IUser Sender { get; }
    public string Command { get; }

    protected Context(IConfigurationManager configurationManager,
        IResourcesService resourcesService,
        IBot bot,
        string target,
        IUser sender,
        string command)
    {
        _configurationManager = configurationManager;
        _resourcesService = resourcesService;
        
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
    
    public string GetString(string key)
    {
        return _resourcesService.GetString(key, Locale);
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