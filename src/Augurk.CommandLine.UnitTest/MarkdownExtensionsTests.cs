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

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            actualMarkdown.ShouldContain("data:image/png;base64,");
        }

        [TestMethod]
        public void EmbedMultipleImages()
        {
            // Arrange
            string inputMarkdown = "![alt](image1.png \"title\") ![alt](image2.png) ![alt](image3.jpg \"title\")";

            // Act
            string actualMarkdown = inputMarkdown.EmbedImages();

            // Assert
            actualMarkdown.ShouldContain("data:image/png;base64");
            actualMarkdown.ShouldContain("data:image/jpeg;base64");
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

