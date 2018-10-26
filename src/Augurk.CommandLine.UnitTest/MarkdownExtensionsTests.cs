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
using Augurk.CommandLine.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Augurk.CommandLine.UnitTest
{
    [TestClass]
    public class MarkdownExtenionsTests
    {
        #region EmbedImages

        [TestMethod]
        public void EmbedSingleImage()
        {
            // Arrange
            string inputMarkdown = "text ![alt for image1.png](image1.png \"title for image1.png\") text, bla [link](image1.png)";
            string expectedMarkdown = "text ![alt for image1.png](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1Pe" +
                                      "AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xM" +
                                      "kMEa+wAAAAMSURBVBhXY/jPwAAAAwEBAGMkVdMAAAAASUVORK5CYII= \"title for image1.png\") te" +
                                      "xt, bla [link](image1.png)";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            actualMarkdown.ShouldBe(expectedMarkdown);
        }

        [TestMethod]
        public void EmbedMultipleImages()
        {
            // Arrange
            string inputMarkdown = "![alt](image1.png \"title\") ![alt](image2.png) ![alt](image3.jpg \"title\")";
            string expectedMarkdown = "![alt](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAABGdBTUEAALGPC/x" +
                                      "hBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4xMkMEa+wAAAAMSURBVBh" +
                                      "XY/jPwAAAAwEBAGMkVdMAAAAASUVORK5CYII= \"title\") ![alt](data:image/png;base64,iVBORw0KGgoAAA" +
                                      "ANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwgAADsIBFShKgAAAABl0RVh0U2" +
                                      "9mdHdhcmUAcGFpbnQubmV0IDQuMC4xMkMEa+wAAAAMSURBVBhXY2BQ+w8AAU8BJr37vWkAAAAASUVORK5CYII=) ![al" +
                                      "t](data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/4QBoRXhpZgAATU0AKgAAAAgABAEaAAUAAAABAA" +
                                      "AAPgEbAAUAAAABAAAARgEoAAMAAAABAAIAAAExAAIAAAARAAAATgAAAAAAAABgAAAAAQAAAGAAAAABcGFpbnQubmV0ID" +
                                      "QuMC4xMgAA/9sAQwABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ" +
                                      "EBAQEBAQEB/9sAQwEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQ" +
                                      "EBAQEBAQEB/8AAEQgAAQABAwEiAAIRAQMRAf/EAB8AAAEFAQEBAQEBAAAAAAAAAAABAgMEBQYHCAkKC//EALUQAAIBAw" +
                                      "MCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5Ok" +
                                      "NERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8" +
                                      "TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+v/EAB8BAAMBAQEBAQEBAQEAAAAAAAABAgMEBQYHCAkKC/" +
                                      "/EALURAAIBAgQEAwQHBQQEAAECdwABAgMRBAUhMQYSQVEHYXETIjKBCBRCkaGxwQkjM1LwFWJy0QoWJDThJfEXGBkaJi" +
                                      "coKSo1Njc4OTpDREVGR0hJSlNUVVZXWFlaY2RlZmdoaWpzdHV2d3h5eoKDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqr" +
                                      "KztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uLj5OXm5+jp6vLz9PX29/j5+v/aAAwDAQACEQMRAD8A/WCiiiv+Pc/51z" +
                                      "//2Q== \"title\")";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            actualMarkdown.ShouldBe(expectedMarkdown);
        }

        [TestMethod]
        public void EmbedImageThatDoesNotExist()
        {
            // Arrange
            string inputMarkdown = "text ![alt for image4.png](image4.png \"title for image4.png\") text, bla [link](image4.png)";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            // As nothing should happen, compare it against the input
            actualMarkdown.ShouldBe(inputMarkdown);
        }

        [TestMethod]
        public void EmbedImageFromTheWeb()
        {
            // Arrange
            string inputMarkdown = "![alt](https://augurk.github.io/Augurk/img/icon16.png)";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            // As nothing should happen, compare it against the input
            actualMarkdown.ShouldBe(inputMarkdown);
        }

        [TestMethod]
        public void EmbedImageWithRandomText()
        {
            // Arrange
            string inputMarkdown = "![alt](There should probably have been a filename here. \"Title which will not be shown.\")";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            // As nothing should happen, compare it against the input
            actualMarkdown.ShouldBe(inputMarkdown);
        }

        #endregion

        #region TrimLineStart

        [TestMethod]
        public void TrimLineStart()
        {
            // Arrange
            string inputMarkdown = "\tThis feature describes how trimming works" + Environment.NewLine + "\tFor every line in this piece of markdown the start should be trimmed.  ";
            string expectedMarkdown = "This feature describes how trimming works" + Environment.NewLine + "For every line in this piece of markdown the start should be trimmed.  ";

            // Act
            string actualMarkdown = inputMarkdown.TrimLineStart();

            // Assert
            actualMarkdown.ShouldBe(expectedMarkdown);
        }

        [TestMethod]
        public void TrimLineStartWithFinalNewLine()
        {
            // Arrange
            string inputMarkdown = "\tThis feature describes how trimming works" + Environment.NewLine + "\tFor every line in this piece of markdown the start should be trimmed.  " + Environment.NewLine;
            string expectedMarkdown = "This feature describes how trimming works" + Environment.NewLine + "For every line in this piece of markdown the start should be trimmed.  " + Environment.NewLine;

            // Act
            string actualMarkdown = inputMarkdown.TrimLineStart();

            // Assert
            actualMarkdown.ShouldBe(expectedMarkdown);
        }

        #endregion
    }
}

