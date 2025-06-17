using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;
using Terraria;

namespace UICustomizer.Helpers
{
    public static class ChatUtils
    {
        public static void ClearMessages()
        {
            // Clear stored messages
            var messages = GetMessages();
            messages.Clear();
        }
        public static int GetMessagesCount()
        {
            // Grab stored message count
            var messages = GetMessages();
            int totalMessages = messages.Count;
            return totalMessages;
        }

        public static List<ChatMessageContainer> GetMessages()
        {
            var monitor = (RemadeChatMonitor)Main.chatMonitor;
            var field = typeof(RemadeChatMonitor).GetField("_messages", BindingFlags.Instance | BindingFlags.NonPublic);
            var messages = (List<ChatMessageContainer>)field.GetValue(monitor);
            return messages;
        }
    }
}
