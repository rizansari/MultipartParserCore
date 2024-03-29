﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MultipartParserCore
{
    public class MultipartParser
    {
        protected Stream _stream;
        protected string _boundary;
        protected string _boundaryWithDashes;
        protected ParseMessageType _messageType;
        protected Encoding _encoding = Encoding.UTF8;

        public event EventHandler<ParsedMessage> OnMessage;

        public MultipartParser(Stream stream, string boundary, ParseMessageType messageType = ParseMessageType.Json)
        {
            _stream = stream;
            _boundary = boundary;
            _boundaryWithDashes = "--" + _boundary;
            _messageType = messageType;
        }

        public void StartParsing(CancellationToken cts)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string result = "";

                var sr = new StreamReader(_stream, encoding: _encoding);
                string data = sr.ReadLine();
                while (data != null && !cts.IsCancellationRequested)
                {
                    data = data.Trim();

                    if (data == string.Empty)
                    {
                        // pass
                    }
                    else if (data.Trim() == _boundaryWithDashes)
                    {
                        if (sb.Length > 0)
                        {
                            result = sb.ToString();
                            sb = new StringBuilder();
                            sb.Append(data.Trim());
                        }
                    }
                    else if (data.StartsWith("Content-Type:") || data.StartsWith("Content-Length:"))
                    {
                        // pass
                    }
                    else if (data.Contains(_boundaryWithDashes))
                    {
                        int index = data.IndexOf(_boundaryWithDashes);

                        string temp = data.Substring(0, index);
                        sb.Append(temp);
                        if (temp.EndsWith("}"))
                        {
                            // try to parse json
                            string temp2 = sb.ToString();
                            temp2 = temp2.Substring(temp2.IndexOf("{"));
                            if (ValidateJSON(temp2))
                            {
                                result = sb.ToString();
                                sb = new StringBuilder();
                            }
                        }
                    }
                    else
                    {
                        string temp = data;
                        sb.Append(temp);
                        if (temp.EndsWith("}"))
                        {
                            // try to parse json
                            string temp2 = sb.ToString();
                            temp2 = temp2.Substring(temp2.IndexOf("{"));
                            if (ValidateJSON(temp2))
                            {
                                result = sb.ToString();
                                sb = new StringBuilder();
                            }
                        }
                    }
                    data = sr.ReadLine();

                    if (result != "")
                    {
                        OnMessage?.Invoke(null, new ParsedMessage() { Body = result });
                        result = "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateJSON(string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                return false;
            }
        }
    }
}
