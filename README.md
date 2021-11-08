# Multipart Parser library for .NET Core

Library Version: v1.0.0

## Installation

```powershell
Install-Package MultipartParserCore
```

## Usage
Create HttpClient and pass result stream to MultipartParser along with boundary, start parsing and get results
```

using (HttpClient httpClient = new HttpClient())
{
    httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
    var requestUri = "http://10.1.1.182:8080/";
    var stream = httpClient.GetStreamAsync(requestUri).Result;

    MultipartParserCore.MultipartParser parser = new MultipartParserCore.MultipartParser(stream, "Boundary");
    parser.OnMessage += Parser_OnMessage;
    parser.StartParsing();
}

private static void Parser_OnMessage(object sender, MultipartParserCore.ParsedMessage e)
{
    Console.WriteLine("----start----");
    Console.WriteLine(e.Body);
    Console.WriteLine("-----end-----");
}

```

## License

This library licensed under the MIT license.
