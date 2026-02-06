using System.Collections.Generic;
using ElsaMina.Commands.Ai.LanguageModel;
using ElsaMina.Core.Services.Rooms;

namespace ElsaMina.Commands.Ai.Chat;

public interface IConversationHistoryService
{
    List<LanguageModelMessage> BuildConversation(IRoom room, IUser sender, string latestMessage);
    void StoreAssistantReply(IRoom room, string response);
}
