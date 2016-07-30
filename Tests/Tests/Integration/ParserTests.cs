using System.IO;
using NUnit.Framework;

namespace Tests.Integration
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void GivenTypicalFolderCanParse()
        {
            // arrange
            var inputPath = @"C:\work\LPv39\Source\Sites\LampsPlus";
            var outputPath = @"C:\work";
            var mask = "*.as?x,*.cshtml";
            var parser = new ParserLib.Parser();
            var expectedFile = Path.Combine(outputPath, "id-catalog.json");
            File.Delete(expectedFile);

            // act
            parser.OutputIdCatalogByFolder(inputPath, outputPath, mask);

            // assert
            Assert.That(File.Exists(expectedFile), Is.True, "File Missing");
            Assert.That(new FileInfo(expectedFile).Length, Is.GreaterThan(0), "File empty");
            var fileContents = File.ReadAllText(expectedFile).ToLower();
            Assert.That(fileContents.Contains(".aspx") && fileContents.Contains(".cshtml"), Is.True, "Missing expected extensions");
            Assert.That(fileContents.Contains(@"Source\\Sites\\LampsPlus\\style-guide\\default.aspx".ToLower()), Is.Not.True, "File not ignored");
        }
    }
}
