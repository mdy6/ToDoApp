using Microsoft.AspNetCore.Http;
using ToDoApp.Resources;
using System;
using System.IO;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class AcceptFilesAttributeTests
    {
        private AcceptFilesAttribute attribute;

        public AcceptFilesAttributeTests()
        {
            attribute = new AcceptFilesAttribute(".docx,.xlsx");
        }

        [Fact]
        public void AcceptFilesAttribute_SetsExtensions()
        {
            String actual = new AcceptFilesAttribute(".docx,.xlsx").Extensions;
            String expected = ".docx,.xlsx";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForProperty()
        {
            attribute = new AcceptFilesAttribute(".docx,.xlsx");

            String expected = Validation.For("AcceptFiles", "File", attribute.Extensions);
            String actual = attribute.FormatErrorMessage("File");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_Null()
        {
            Assert.True(attribute.IsValid(null));
        }

        [Fact]
        public void IsValid_NotFileReturnsFalse()
        {
            Assert.False(attribute.IsValid("100"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData(".doc")]
        [InlineData("docx")]
        [InlineData(".docx.doc")]
        public void IsValid_DifferentExtensionReturnsFalse(String fileName)
        {
            using MemoryStream stream = new();
            IFormFile file = new FormFile(stream, 0, 1, "File", fileName);

            Assert.False(attribute.IsValid(file));
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData(".doc")]
        [InlineData("docx")]
        [InlineData(".docx.doc")]
        public void IsValid_DifferentExtensionsReturnsFalse(String fileName)
        {
            using MemoryStream stream = new();
            IFormFile[] files =
            {
                new FormFile(stream, 0, 1, "FirstFile", fileName),
                new FormFile(stream, 0, 1, "SecondFile", "File.docx")
            };

            Assert.False(attribute.IsValid(files));
        }

        [Theory]
        [InlineData(".docx")]
        [InlineData(".xlsx")]
        [InlineData("docx.docx")]
        [InlineData("docx..docx")]
        [InlineData("xlsx.doc.xlsx")]
        public void IsValid_Extension(String fileName)
        {
            using MemoryStream stream = new();
            IFormFile file = new FormFile(stream, 0, 1, "File", fileName);

            Assert.True(attribute.IsValid(file));
        }

        [Theory]
        [InlineData("docx.docx", ".docx")]
        [InlineData("docx..docx", ".xlsx")]
        [InlineData(".xlsx", "docx..docx")]
        [InlineData(".docx", "xlsx.doc.xlsx")]
        [InlineData("xlsx.doc.xlsx", ".docx.docx")]
        public void IsValid_Extensions(String firstFileName, String secondFileName)
        {
            using MemoryStream stream = new();
            IFormFile[] files =
            {
                new FormFile(stream, 0, 1, "FirstFile", firstFileName),
                new FormFile(stream, 0, 1, "SecondFile", secondFileName)
            };

            Assert.True(attribute.IsValid(files));
        }
    }
}
