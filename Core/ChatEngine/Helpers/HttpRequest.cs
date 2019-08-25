using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace ChatClient.Helpers
{
    public class HttpRequest
    {
        private readonly Dictionary<string, string> Attributes;
        private readonly Dictionary<string, CustomFile> Files;
        public string UrlPath;
        public HttpMethod HttpMethod;

        public HttpRequest(string path)
        {
            Attributes = new Dictionary<string, string>();
            Files = new Dictionary<string, CustomFile>();
            UrlPath = path;
            HttpMethod = HttpMethod.Post;
        }

        public HttpRequest(string path, HttpMethod http)
        {
            Attributes = new Dictionary<string, string>();
            Files = new Dictionary<string, CustomFile>();
            UrlPath = path;
            HttpMethod = http;
        }

        public void AddFile(string key, CustomFile filePath)
        {
            Files[key] = filePath;
        }

        public void AddParameter(string name, object value)
        {
            Attributes.Add(name, value.ToString());
        }

        public HttpContent GenrateRequest()
        {
            if (Files.Count == 0)
                return new FormUrlEncodedContent(Attributes);
            else
            {
                var requestContent = new MultipartFormDataContent();
                foreach (var item in Files)
                {

                    if (item.Value.FilePath != null)
                    {
                        var streamContent = new StreamContent(File.OpenRead(item.Value.FilePath));
                        requestContent.Add(streamContent, item.Key, item.Value.FileName);
                    }

                    if (item.Value.FileStream != null)
                    {
                        var streamContent = new StreamContent(item.Value.FileStream);
                        //streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        requestContent.Add(streamContent, item.Key, item.Value.FileName);
                    }

                    if (item.Value.FileByte != null)
                    {
                        var streamContent = new ByteArrayContent(item.Value.FileByte);
                        //streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        requestContent.Add(streamContent, item.Key, item.Value.FileName);
                    }

                }


                foreach (var item in Attributes)
                    requestContent.Add(new StringContent(item.Value), item.Key);

                return requestContent;
            }
        }
    }

    public class CustomFile
    {
        public string FileName { get; internal set; }
        public string FilePath { get; internal set; }
        public byte[] FileByte { get; set; }
        public Stream FileStream { get; set; }
        public bool IsAvailable => !string.IsNullOrWhiteSpace(FileName);

        public CustomFile(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            FilePath = filePath;
        }

        public CustomFile(string fileName, byte[] fileBytes)
        {
            FileByte = fileBytes;
            FileName = fileName;
        }

        public CustomFile(string fileName, Stream fileStream)
        {
            FileStream = fileStream;
            if (FileStream != null) FileStream.Position = 0;
            FileName = fileName;
        }
    }
}