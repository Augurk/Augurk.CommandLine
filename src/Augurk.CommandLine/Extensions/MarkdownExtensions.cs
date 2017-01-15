/*
 Copyright 2017, Augurk
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Augurk.CommandLine.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="System.String">strings</see> containing markdown syntax.
    /// </summary>
    public static class MarkdownExtensions
    {
        /// <summary>
        /// Embeds the local images that reference from the markdown into the markdown.
        /// </summary>
        /// <param name="sourceMarkdown"></param>
        /// <returns>The provided markdown with embeded images.</returns>
        public static string EmbedImages(this string sourceMarkdown)
        {
            const string imageRegex = "!\\[.*?\\]\\((?<file>.+?)(?: \\\".+?\\\")?\\)";

            // Put the source in the result variable
            string resultMarkdown = sourceMarkdown;

            // Find the first match
            var match = Regex.Match(sourceMarkdown, imageRegex);

            // Set up some general variables
            int offset = 0;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            while (match.Success)
            {
                Group fileGroup = match.Groups["file"];

                if (File.Exists(fileGroup.Value))
                {
                    Image image = Image.FromFile(fileGroup.Value);

                    // Determine the mimetype
                    string mimeType = codecs.First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;

                    using (var ms = new MemoryStream())
                    {
                        // Save the image in a stream so we can convert it to a string
                        image.Save(ms, image.RawFormat);

                        // Use the data URI syntax (RFC 2397)
                        string encodedFile = $"data:{mimeType};base64,{Convert.ToBase64String(ms.ToArray())}";

                        // Replace the original filename with the encoded file
                        resultMarkdown = $"{resultMarkdown.Substring(0, fileGroup.Index + offset)}{encodedFile}{resultMarkdown.Substring(fileGroup.Index + fileGroup.Length + offset)}";

                        // Determine the change in length of the file, to use as offset for following matches
                        offset += encodedFile.Length - fileGroup.Length;
                    }
                }
                // Move to the next match
                match = match.NextMatch();
            }

            return resultMarkdown;
        }

        /// <summary>
        /// Trims the start of each line in the provided <paramref name="sourceMarkdown"/>.
        /// </summary>
        /// <param name="sourceMarkdown">Markdown to trim the start of each line for.</param>
        /// <returns>Returns the source markdown where the start of each line is trimmed.</returns>
        public static string TrimLineStart(this string sourceMarkdown)
        {
            string[] lines = sourceMarkdown.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (i != lines.Length - 1)
                {
                    result.AppendLine(line.TrimStart());
                }
                else
                {
                    result.Append(line.TrimStart());
                }
            }

            return result.ToString();
        }
    }
}
