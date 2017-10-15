using System;
using System.Text.RegularExpressions;
using Xunit;

namespace StringUnindent
{
    public static class Extensions
    {
        public static string Unindent(this String str)
        {
            int? minLevel = null;
            var lineDelimiters = new string[] { "\r\n", "\n" };
            var lines = str.Split(lineDelimiters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                var line  = lines[i];
                var match = Regex.Match(line, @"[^\s]");

                if (match.Success)
                {
                  var index = match.Index;
                  if ((minLevel is null) || (index < minLevel))
                      minLevel = index;
                }
            }
            if (minLevel is null) minLevel = 0;

            return Regex.Replace(str, "(?m:^[ \t]{"+ minLevel +"})", "");
        }
    }

    public class SimpleIndentation
    {

        [Fact]
        public void RemovesSpaceIndentation()
        {
            Assert.Equal("abc", "  abc".Unindent());
        }

        [Fact]
        public void RemovesTabIndentation()
        {
            Assert.Equal("abc", "\tabc".Unindent());
        }

        [Fact]
        public void RemovesSpaceAndTabIndentation()
        {
            Assert.Equal("abc", "\t abc".Unindent());
        }

        [Fact]
        public void HandlesEmptyStrings()
        {
            Assert.Equal("", "".Unindent());
        }
    }

    public class MultiLineIndentation
    {
        [Fact]
        public void RemovesSpaceIndentation()
        {
            Assert.Equal("abc\nabc", "  abc\n  abc".Unindent());
        }

        [Fact]
        public void RemovesTabIndentation()
        {
            Assert.Equal("abc\nabc", "\tabc\n\tabc".Unindent());
        }

        [Fact]
        public void RemovesSpaceAndTabIndentation()
        {
            Assert.Equal("abc\nabc", "\t abc\n\t abc".Unindent());
        }

        [Fact]
        public void KeepsRelativeIndentation()
        {
            Assert.Equal("abc\n\tabc", "\tabc\n\t\tabc".Unindent());
        }

        [Fact]
        public void IgnoresBlankLinesForIndentCalculation()
        {
            Assert.Equal("\nabc\n\n\tabc\n", "\n\tabc\n\n\t\tabc\n".Unindent());
        }

        [Fact]
        public void WorksWithVerbatimStrings()
        {
            var s = @"
              abc
                abc
            ";
            Assert.Equal("abc\r\n  abc", s.Unindent().Trim());
        }

        [Theory]
        [InlineData("\r\n")]
        [InlineData("\n")]
        public void HandlesDifferentNewlineCharacters(string nl)
        {
            Assert.Equal($"abc{nl}abc", $"  abc{nl}  abc".Unindent());
        }
    }
}
