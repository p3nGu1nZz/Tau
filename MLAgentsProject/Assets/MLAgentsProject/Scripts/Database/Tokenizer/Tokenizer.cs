using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class Tokenizer
{
    private static HashSet<string> _vocabulary = new();

    public static List<string> ExtractVocabulary(List<Message> messages)
    {
        _vocabulary.Clear();

        Log.Message($"Adding '{Constants.ReservedWords.Length}' reserved words to 'vocabulary'...");
        TokenizerUtilities.AddToVocabulary(Constants.ReservedWords, _vocabulary);

        foreach (var message in messages)
        {
            Log.Message($"Processing message: {message.domain}");
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.domain), _vocabulary);
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.context), _vocabulary);
            TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(message.system), _vocabulary);

            foreach (var turn in message.turns)
            {
                TokenizerUtilities.AddToVocabulary(TokenizerUtilities.Normalize(turn.message), _vocabulary);
            }
        }

        List<string> sortedVocabulary = _vocabulary.ToList();
        sortedVocabulary.Sort();
        Log.Message($"Extracted vocabulary size: {sortedVocabulary.Count}");

        return sortedVocabulary;
    }

    public static async Task<string> Export(List<string> words)
    {
        Log.Message("Tokenizer exporting vocabulary...");

        string vocabFileName = DataUtilities.GetFilePath(TableNames.Vocabulary + ".json");
        string tokenFileName = DataUtilities.GetFilePath(TableNames.Tokens + ".json");

        var vocabList = new VocabularyList
        {
            version = Constants.Version,
            model_name = Constants.ModelName,
            organization = Constants.Organization,
            total_words = words.Count,
            words = words
        };

        var vocabulary = Database.Instance.GetTable(TableNames.Vocabulary);
        var tokens = TokenizerUtilities.GetTokensFromVocabularyWords(vocabulary, words);
        var tokenList = new TokenList
        {
            version = Constants.Version,
            model_name = Constants.ModelName,
            organization = Constants.Organization,
            total_tokens = vocabulary.Count,
            tokens = tokens
        };

        await TokenizerUtilities.SaveToFileAsync(vocabFileName, vocabList);
        await TokenizerUtilities.SaveToFileAsync(tokenFileName, tokenList);

        Log.Message("Export process completed.");

        return tokenFileName;
    }
}
