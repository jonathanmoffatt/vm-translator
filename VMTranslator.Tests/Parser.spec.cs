using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VMTranslator.Tests
{
    [TestClass]
    public class WhenParsing
    {
        private Parser classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Parser();
        }

        [TestMethod]
        public void ShouldReturnNullIfTheLineIsEmpty()
        {
            classUnderTest.Parse("   ").Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnNullIfLineStartsWithAComment()
        {
            classUnderTest.Parse("// does stuff n things").Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnNullIfLineIsAComment()
        {
            classUnderTest.Parse("   \t\t// comments etc").Should().BeNull();
        }

        [TestMethod]
        public void ShouldReturnStackArithmeticCommands()
        {
            classUnderTest.Parse("add").Command.Should().Be(Command.Add);
            classUnderTest.Parse("neg").Command.Should().Be(Command.Neg);
            classUnderTest.Parse("eq").Command.Should().Be(Command.Eq);
            classUnderTest.Parse("or").Command.Should().Be(Command.Or);
            classUnderTest.Parse("sub").Command.Should().Be(Command.Sub);
            classUnderTest.Parse("gt").Command.Should().Be(Command.Gt);
            classUnderTest.Parse("lt").Command.Should().Be(Command.Lt);
            classUnderTest.Parse("and").Command.Should().Be(Command.And);
            classUnderTest.Parse("not").Command.Should().Be(Command.Not);
        }

        [TestMethod]
        public void ShouldLeaveSegmentAndValueEmptyForArithmeticCommands()
        {
            LineOfCode lineOfCode = classUnderTest.Parse("add");
            lineOfCode.Segment.Should().BeNull();
            lineOfCode.Value.Should().BeNull();
        }

        [TestMethod]
        public void ShouldRecognisePushCommands()
        {
            classUnderTest.Parse("push constant 17").Command.Should().Be(Command.Push);
        }

        [TestMethod]
        public void ShouldIncludeTheOriginalVmCode()
        {
            classUnderTest.Parse("push constant 17").VmCode.Should().Be("push constant 17");
        }

        [TestMethod]
        public void ShouldRecognisePopCommands()
        {
            classUnderTest.Parse("pop local 2").Command.Should().Be(Command.Pop);
        }

        [TestMethod]
        public void ShouldIdentifyTheSegment()
        {
            classUnderTest.Parse("push argument 3").Segment.Should().Be(Segment.Argument);
        }

        [TestMethod]
        public void ShouldIdentifyTheValue()
        {
            classUnderTest.Parse("push argument 3").Value.Should().Be(3);
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfArithmeticOperationIncludesSegment()
        {
            classUnderTest.Parse("add argument 3").Error.Should().Be("Arithmetic operation cannot include any additional arguments");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfStackOperationDoesNotIncludeAValue()
        {
            classUnderTest.Parse("pop local").Error.Should().Be("Stack operations must include a value");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfStackOperationDoesNotIncludeASegment()
        {
            classUnderTest.Parse("push").Error.Should().Be("Stack operations must include a segment");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfCommandCouldNotBeRecognised()
        {
            classUnderTest.Parse("mult").Error.Should().Be("Command 'mult' not recognised");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfSegmentCouldNotBeRecognised()
        {
            classUnderTest.Parse("push somewhere 3").Error.Should().Be("Segment 'somewhere' not recognised");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfValueNotAValidInteger()
        {
            classUnderTest.Parse("push local blah").Error.Should().Be("Value 'blah' is not a valid integer");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfTryingToPopAConstant()
        {
            classUnderTest.Parse("pop constant 17").Error.Should().Be("pop is not a valid operation on a constant");
        }
    }
}
