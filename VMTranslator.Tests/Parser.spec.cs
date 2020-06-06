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
        public void ShouldReturnStackArithmeticInstructions()
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
        public void ShouldLeaveSegmentAndValueEmptyForArithmeticInstructions()
        {
            LineOfCode lineOfCode = classUnderTest.Parse("add");
            lineOfCode.Segment.Should().BeNull();
            lineOfCode.Value.Should().BeNull();
        }

        [TestMethod]
        public void ShouldRecognisePushInstructions()
        {
            classUnderTest.Parse("push constant 17").Instruction.Should().Be(InstructionType.Push);
        }

        [TestMethod]
        public void ShouldIncludeTheOriginalVmCode()
        {
            classUnderTest.Parse("push constant 17").VmCode.Should().Be("push constant 17");
        }

        [TestMethod]
        public void ShouldRecognisePopInstructions()
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
            classUnderTest.Parse("add argument 3").Error.Should().Be("Arithmetic instructions cannot include any additional arguments");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfStackOperationDoesNotIncludeAValue()
        {
            classUnderTest.Parse("pop local").Error.Should().Be("Stack instructions must include a value");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfStackOperationDoesNotIncludeASegment()
        {
            classUnderTest.Parse("push").Error.Should().Be("Stack instructions must include a segment");
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
            classUnderTest.Parse("pop constant 17").Error.Should().Be("pop cannot be performed on a constant");
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
        public void ShouldRecogniseBranchingInstructions()
        {
            classUnderTest.Parse("goto myLabel").Instruction.Should().Be(InstructionType.Goto);
            classUnderTest.Parse("if-goto myLabel").Instruction.Should().Be(InstructionType.IfGoto);
            classUnderTest.Parse("label myLabel").Instruction.Should().Be(InstructionType.Label);
        }

        [TestMethod]
        public void ShouldSetTheLabelForBranchingInstructions()
        {
            classUnderTest.Parse("goto myLabel").Label.Should().Be("myLabel");
            classUnderTest.Parse("if-goto myLabel").Label.Should().Be("myLabel");
            classUnderTest.Parse("label myLabel").Label.Should().Be("myLabel");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfABranchingCommandDoesNotHaveALabel()
        {
            classUnderTest.Parse("goto").Error.Should().Be("Branching instructions must have a label");
            classUnderTest.Parse("if-goto").Error.Should().Be("Branching instructions must have a label");
            classUnderTest.Parse("label").Error.Should().Be("Branching instructions must have a label");
        }

        [TestMethod]
        public void ShouldRecogniseFunctionInstructions()
        {
            classUnderTest.Parse("function myFunction 5").Instruction.Should().Be(InstructionType.Function);
            classUnderTest.Parse("call myFunction 2").Instruction.Should().Be(InstructionType.Call);
            classUnderTest.Parse("return").Instruction.Should().Be(InstructionType.Return);
        }

        [TestMethod]
        public void ShouldSetFunctionNameForFunctionsAndCalls()
        {
            classUnderTest.Parse("function myFunction 5").FunctionName.Should().Be("myFunction");
            classUnderTest.Parse("call myFunction 2").FunctionName.Should().Be("myFunction");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfFunctionOrCallIsMissingAName()
        {
            classUnderTest.Parse("function").Error.Should().Be("Function must have a name");
            classUnderTest.Parse("call").Error.Should().Be("Function must have a name");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfReturnHasAName()
        {
            classUnderTest.Parse("return myFunction").Error.Should().Be("Return cannot have a name");
        }

        [TestMethod]
        public void ShouldPutNumberOfLocalsIntoValueForFunctions()
        {
            classUnderTest.Parse("function myFunction 5").Value.Should().Be(5);
        }

        [TestMethod]
        public void ShouldPutNumberOfArgumentsIntoValueForCalls()
        {
            classUnderTest.Parse("call myFunction 2").Value.Should().Be(2);
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfFunctionIsMissingTheNumberOfLocalVariables()
        {
            classUnderTest.Parse("function myFunction").Error.Should().Be("Function must specify the number of local variables");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfCallIsMissingTheNumberOfArguments()
        {
            classUnderTest.Parse("call myFunction").Error.Should().Be("Call must specify the number of arguments");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfFunctionHasInvalidNumberOfLocalVariables()
        {
            classUnderTest.Parse("function myFunction blah").Error.Should().Be("Value 'blah' is not a valid integer");
        }

        [TestMethod]
        public void ShouldReturnAnErrorIfCallHasInvalidNumberOfArguments()
        {
            classUnderTest.Parse("call myFunction blah").Error.Should().Be("Value 'blah' is not a valid integer");
        }
    }
}
