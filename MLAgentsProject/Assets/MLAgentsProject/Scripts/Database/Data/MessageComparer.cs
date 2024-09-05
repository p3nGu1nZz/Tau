using System.Collections.Generic;
using System.Linq;

public class MessageComparer
{
    public static List<Message> FindDuplicates(List<Message> messages)
    {
        var seenMessages = new HashSet<string>();
        var duplicates = new List<Message>();

        foreach (var message in messages)
        {
            var userTurn = message.turns.FirstOrDefault(t => t.role == "User");
            if (userTurn != null)
            {
                if (seenMessages.Contains(userTurn.message))
                {
                    duplicates.Add(message);
                }
                else
                {
                    seenMessages.Add(userTurn.message);
                }
            }
        }

        return duplicates;
    }
}
