﻿using System;
using System.Collections.Generic;
using System.IO;

namespace I18NPortable.Parsers
{
    public class I18NLocaleParser : ILocaleParser
    {
        public void Parse(Stream stream, Dictionary<string, string> translations)
        {
            using (var streamReader = new StreamReader(stream))
            {
                string key = null;
                string value = null;

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var isEmpty = string.IsNullOrWhiteSpace(line);
                    var isComment = !isEmpty && line.Trim().StartsWith("#");
                    var isKeyValuePair = !isEmpty && !isComment && line.Contains("=");

                    if ((isEmpty || isComment || isKeyValuePair) && key != null && value != null)
                    {
                        translations.Add(key, value);

                        key = null;
                        value = null;
                    }

                    if (isEmpty || isComment)
                        continue;

                    if (isKeyValuePair)
                    {
                        var kvp = line.Split(new[] { '=' }, 2);

                        key = kvp[0].Trim();
                        value = kvp[1].Trim().UnescapeLineBreaks();
                    }
                    else if (key != null && value != null)
                    {
                        value = value + Environment.NewLine + line.Trim().UnescapeLineBreaks();
                    }
                }

                if (key != null && value != null)
                    translations.Add(key, value);
            }
        }
    }
}
