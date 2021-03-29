using Microsoft.AspNetCore.Http;
using ToDoApp.Resources;
using System;
using System.IO;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class FileSizeAttributeTests
    {
        private FileSizeAttribute attribute;

        public FileSizeAttributeTests()
        {
            attribute = new FileSizeAttribute(12.25);
        }

        [Fact]
        public void FileSizeAttribute_SetsMaximumMB()
        {
            Decimal expected = 12.25M;
            Decimal actual = new FileSizeAttribute(12.25).MaximumMB;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForName()
        {
            attribute = new FileSizeAttribute(12.25);

            String expected = Validation.For("FileSize", "File", attribute.MaximumMB);
            String actual = attribute.FormatErrorMessage("File");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Fact]
        public void IsValid_NotIFormFileValueIsValid()
        {
            Assert.True(attribute.IsValid("100"));
        }

        [Theory]
        [InlineData(240546)]
        [InlineData(12845056)]
        public void IsValid_LowerOrEqualFileSize(Int64 length)
        {
            using MemoryStream stream = new();
            IFormFile file = new FormFile(stream, 0, length, "File", "file.txt");

            Assert.True(attribute.IsValid(file));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumIsNotValid()
        {
            using MemoryStream stream = new();
            IFormFile file = new FormFile(stream, 0, 12845057, "File", "file.txt");

            Assert.False(attribute.IsValid(file));
        }

        [Theory]
        [InlineData(240546, 4574)]
        [InlineData(12840000, 5056)]
        public void IsValid_LowerOrEqualFileSizes(Int64 firstLength, Int64 secondLength)
        {
            using MemoryStream stream = new();
            IFormFile[] files =
            {
                new FormFile(stream, 0, firstLength, "FirstFile", "first.txt"),
                new FormFile(stream, 0, secondLength, "SecondFile", "second.txt")
            };

            Assert.True(attribute.IsValid(files));
        }

        [Fact]
        public void IsValid_GreaterThanMaximumSizesAreNotValid()
        {
            using MemoryStream stream = new();
            IFormFile[] files =
            {
                new FormFile(stream, 0, 5057, "FirstFile", "first.txt"),
                new FormFile(stream, 0, 12840000, "SecondFile", "second.txt")
            };

            Assert.False(attribute.IsValid(files));
        }
    }
}
