using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        start:
        Console.WriteLine("请输入tg导出文件地址");

        // tg 文件地址
        string? path = Console.ReadLine();
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            Console.WriteLine("未找到地址文件");
            goto start; // 重新跳转到开始位置
        }

        var pathFiles = Directory.GetFiles(path);
        var files = pathFiles.Length < 2 ? 0 : pathFiles.Length + 1; 
        // 0 - 0 
        // 1 - 0 
        // 2 - 3
        // 20 - 21

        // 读取HTML文件
        string htmlContent = String.Empty;
        // 没有 0 文件, 所以给个初始化
        htmlContent += File.ReadAllText(Path.Combine(path, $"messages.html"));
        for (int i = 2; i < files; i++)
        {
            htmlContent +=
                File.ReadAllText(Path.Combine(@"C:\Users\豪\Downloads\Telegram Desktop\ChatExport_2024-09-01\",
                    $"messages{i}.html"));
        }

        // string htmlContent = File.ReadAllText(Path.Combine(@"C:\Users\豪\Downloads\Telegram Desktop\ChatExport_2024-09-01\", "messages19.html"));

        // 匹配所有聊天记录
        Regex messageRegex =
            new Regex(
                @"<div class=""message(?:\s+default)?(?:\s+clearfix)?"" id=""(message\d+)"">.*?<div class=""text"">(.*?)<\/div>.*?<\/div>",
                RegexOptions.Singleline);
        MatchCollection messages = messageRegex.Matches(htmlContent);

        // 保存所有聊天记录的ID和内容
        Dictionary<string, string> messageDict = new Dictionary<string, string>();

        foreach (Match match in messages)
        {
            string id = match.Groups[1].Value;
            string text = match.Groups[2].Value.Trim();
            messageDict[id] = text;
        }

        // 匹配所有引用的聊天记录
        Regex replyRegex = new Regex(@"<div class=""reply_to details"">.*?href=""#go_to_(message\d+)"".*?<\/div>",
            RegexOptions.Singleline);
        MatchCollection replies = replyRegex.Matches(htmlContent);

        Console.WriteLine(replies.Count);
        // 输出所有被引用的消息
        
        var file = $"{Path.Combine(path, "remark.txt")}";
        // 如果存在, 则删除此文件内容
        if(File.Exists(file))
            File.Delete(file);
        foreach (Match reply in replies)
        {
            string referencedId = reply.Groups[1].Value;
            if (messageDict.TryGetValue(referencedId, out var value))
            {
                var content = $"\r\n被引用的消息 (ID: {referencedId}): {value}";
                File.AppendAllText(file, content);
            }
        }
    }
}