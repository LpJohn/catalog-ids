using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ParserLib;

namespace Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase(@"<span id=""testId"">Test Span</span>","testId")]
        [TestCase(@"<span id=testId>Test Span</span>","testId")]
        [TestCase(@"<span id = ""testId"">Test Span</span>","testId")]
        public void GivenSimpleHtmlTagCanParseId(string html, string expectedId)
        {
           // arrange
            var parser = new ParserLib.Parser();

            // act
            var result = parser.GetIds(html);

            // assert
            Assert.That(result.Contains(expectedId), Is.True, "Wrong HTML element ID: " + expectedId);
        }

        [TestCase(@"<div id=someDiv><span id = ""testId"">Test Span</span>&nbsp;<br /></div><div id=""nextDiv"">", new[] { "someDiv", "testId", "nextDiv", })]
        public void GivenComplexHtmlTagCanParseId(string html, string[] expectedIds)
        {
           // arrange
            var parser = new ParserLib.Parser();

            // act
            var result = parser.GetIds(html);

            // assert
            foreach (var expectedId in expectedIds)
            {
                Assert.That(result.Contains(expectedId), Is.True, "Wrong HTML element ID: " + expectedId);
            }
        }

    }
}
