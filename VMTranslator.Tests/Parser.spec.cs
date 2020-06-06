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
        public void ShouldIncrementLineNumberWithEachCall()
        {
            classUnderTest.Parse("add").LineNumber.Should().Be(1);
            classUnderTest.Parse("// something");
            classUnderTest.Parse("blah");
            classUnderTest.Parse("push constant 17").LineNumber.Should().Be(4);
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
            classUnderTest.Parse("add").Instruction.Should().Be(InstructionType.Add);
            classUnderTest.Parse("neg").Instruction.Should().Be(InstructionType.Neg);
            classUnderTest.Parse("eq").Instruction.Should().Be(InstructionType.Eq);
            classUnderTest.Parse("or").Instruction.Should().Be(InstructionType.Or);
            classUnderTest.Parse("sub").Instruction.Should().Be(InstructionType.Sub);
            classUnderTest.Parse("gt").Instruction.Should().Be(InstructionType.Gt);
            classUnderTest.Parse("lt").Instruction.Should().Be(InstructionType.Lt);
            classUnderTest.Parse("and").Instruction.Should().Be(InstructionType.And);
            classUnderTest.Parse("not").Instruction.Should().Be(InstructionType.Not);
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
            classUnderTest.Parse("push constant 17").Instruction.Should().Be(InstructionType.Push);
        }

        [TestMethod]
        public void ShouldIncludeTheOriginalVmCode()
        {
            classUnderTest.Parse("push constant 17").VmCode.Should().Be("push constant 17");
        }

        [TestMethod]
        public void ShouldRecognisePopCommands()
        {
            classUnderTest.Parse("pop local 2").Instruction.Should().Be(InstructionType.Pop);
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

        [TestMethod]
        public void ShouldReturnAnErrorIfTryingToPushAPointerGreaterThan1()
        {
            classUnderTest.Parse("push pointer 1").Error.Should().BeNull();
            classUnderTest.Parse("push pointer 0").Error.Should().BeNull();
            classUnderTest.Parse("push pointer 2").Error.Should().Be("pointer value can only be 0 or 1");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfTryingToPopAPointerGreaterThan1()
        {
            classUnderTest.Parse("pop pointer 1").Error.Should().BeNull();
            classUnderTest.Parse("pop pointer 0").Error.Should().BeNull();
            classUnderTest.Parse("pop pointer 2").Error.Should().Be("pointer value can only be 0 or 1");
        }

        [TestMethod]
        public void ShouldRecogniseBranchingCommands()
        {
            classUnderTest.Parse("goto myLabel").Instruction.Should().Be(InstructionType.Goto);
            classUnderTest.Parse("if-goto myLabel").Instruction.Should().Be(InstructionType.IfGoto);
            classUnderTest.Parse("label myLabel").Instruction.Should().Be(InstructionType.Label);
        }

        [TestMethod]
        public void ShouldSetTheLabelForBranchingCommands()
        {
            classUnderTest.Parse("goto myLabel").Label.Should().Be("myLabel");
            classUnderTest.Parse("if-goto myLabel").Label.Should().Be("myLabel");
            classUnderTest.Parse("label myLabel").Label.Should().Be("myLabel");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfABranchingCommandDoesNotHaveALabel()
        {
            classUnderTest.Parse("goto").Error.Should().Be("Branching commands must have a label");
            classUnderTest.Parse("if-goto").Error.Should().Be("Branching commands must have a label");
            classUnderTest.Parse("label").Error.Should().Be("Branching commands must have a label");
        }
    }
}
