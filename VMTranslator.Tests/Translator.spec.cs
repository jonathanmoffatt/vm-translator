using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VMTranslator.Tests
{
    #region WhenTranslatingPushInstructions

    [TestClass]
    public class WhenTranslatingPushInstructions
    {
        private Translator classUnderTest;
        private readonly LineOfCode pushFromLocal = new LineOfCode
        {
            VmCode = "push local 3",
            Instruction = InstructionType.Push,
            Segment = Segment.Local,
            Value = 3
        };
        private readonly LineOfCode pushFromArgument = new LineOfCode
        {
            VmCode = "push argument 2",
            Instruction = InstructionType.Push,
            Segment = Segment.Argument,
            Value = 2
        };
        private readonly LineOfCode pushFromThis = new LineOfCode
        {
            VmCode = "push this 5",
            Instruction = InstructionType.Push,
            Segment = Segment.This,
            Value = 5
        };
        private readonly LineOfCode pushFromThat = new LineOfCode
        {
            VmCode = "push that 6",
            Instruction = InstructionType.Push,
            Segment = Segment.That,
            Value = 6
        };
        private readonly LineOfCode pushConstant = new LineOfCode
        {
            VmCode = "push constant 17",
            Instruction = InstructionType.Push,
            Segment = Segment.Constant,
            Value = 17
        };
        private readonly LineOfCode pushFromTemp = new LineOfCode
        {
            VmCode = "push temp 4",
            Instruction = InstructionType.Push,
            Segment = Segment.Temp,
            Value = 4
        };
        private readonly LineOfCode pushFromPointer0 = new LineOfCode
        {
            VmCode = "push pointer 0",
            Instruction = InstructionType.Push,
            Segment = Segment.Pointer,
            Value = 0
        };
        private readonly LineOfCode pushFromPointer1 = new LineOfCode
        {
            VmCode = "push pointer 1",
            Instruction = InstructionType.Push,
            Segment = Segment.Pointer,
            Value = 1
        };
        private readonly LineOfCode pushFromStatic = new LineOfCode
        {
            VmCode = "push static 5",
            Instruction = InstructionType.Push,
            Segment = Segment.Static,
            FileName = "Foo",
            Value = 5
        };

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator();
        }

        [TestMethod]
        public void ShouldIncludeTheOriginalVmCodeAsAComment()
        {
            classUnderTest.Translate(pushFromLocal)
                .Should().StartWith("// push local 3");
        }

        [TestMethod]
        public void ShouldTranslatePushFromLocal()
        {
            classUnderTest.Translate(pushFromLocal)
                .Should().Be("// push local 3\n@LCL\nD=M\n@3\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromArgument()
        {
            classUnderTest.Translate(pushFromArgument)
                .Should().Be("// push argument 2\n@ARG\nD=M\n@2\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromThis()
        {
            classUnderTest.Translate(pushFromThis)
                .Should().Be("// push this 5\n@THIS\nD=M\n@5\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromThat()
        {
            classUnderTest.Translate(pushFromThat)
                .Should().Be("// push that 6\n@THAT\nD=M\n@6\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushConstant()
        {
            classUnderTest.Translate(pushConstant)
                .Should().Be("// push constant 17\n@17\nD=A\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromTemp()
        {
            classUnderTest.Translate(pushFromTemp)
                .Should().Be("// push temp 4\n@5\nD=A\n@4\nA=D+A\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromPointer0()
        {
            classUnderTest.Translate(pushFromPointer0)
                .Should().Be("// push pointer 0\n@THIS\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromPointer1()
        {
            classUnderTest.Translate(pushFromPointer1)
                .Should().Be("// push pointer 1\n@THAT\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslatePushFromStatic()
        {
            classUnderTest.Translate(pushFromStatic)
                .Should().Be("// push static 5\n@Foo.5\nD=M\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }
    }

    #endregion

    #region WhenTranslatingPopInstructions

    [TestClass]
    public class WhenTranslatingPopInstructions
    {
        private Translator classUnderTest;
        private readonly LineOfCode popToLocal = new LineOfCode
        {
            VmCode = "pop local 3",
            Instruction = InstructionType.Pop,
            Segment = Segment.Local,
            Value = 3
        };
        private readonly LineOfCode popToArgument = new LineOfCode
        {
            VmCode = "pop argument 2",
            Instruction = InstructionType.Pop,
            Segment = Segment.Argument,
            Value = 2
        };
        private readonly LineOfCode popToThis = new LineOfCode
        {
            VmCode = "pop this 5",
            Instruction = InstructionType.Pop,
            Segment = Segment.This,
            Value = 5
        };
        private readonly LineOfCode popToThat = new LineOfCode
        {
            VmCode = "pop that 6",
            Instruction = InstructionType.Pop,
            Segment = Segment.That,
            Value = 6
        };
        private readonly LineOfCode popToTemp = new LineOfCode
        {
            VmCode = "pop temp 4",
            Instruction = InstructionType.Pop,
            Segment = Segment.Temp,
            Value = 4
        };
        private readonly LineOfCode popToPointer0 = new LineOfCode
        {
            VmCode = "pop pointer 0",
            Instruction = InstructionType.Pop,
            Segment = Segment.Pointer,
            Value = 0
        };
        private readonly LineOfCode popToPointer1 = new LineOfCode
        {
            VmCode = "pop pointer 1",
            Instruction = InstructionType.Pop,
            Segment = Segment.Pointer,
            Value = 1
        };
        private readonly LineOfCode popToStatic = new LineOfCode
        {
            VmCode = "pop static 5",
            Instruction = InstructionType.Pop,
            Segment = Segment.Static,
            FileName = "Foo",
            Value = 5
        };

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator();
        }

        [TestMethod]
        public void ShouldIncludeTheOriginalVmCodeAsAComment()
        {
            classUnderTest.Translate(popToLocal).Should().StartWith("// pop local 3");
        }

        [TestMethod]
        public void ShouldTranslatePopToLocal()
        {
            classUnderTest.Translate(popToLocal)
                .Should().Be("// pop local 3\n@LCL\nD=M\n@3\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToArgument()
        {
            classUnderTest.Translate(popToArgument)
                .Should().Be("// pop argument 2\n@ARG\nD=M\n@2\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToThis()
        {
            classUnderTest.Translate(popToThis)
                .Should().Be("// pop this 5\n@THIS\nD=M\n@5\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToThat()
        {
            classUnderTest.Translate(popToThat)
                .Should().Be("// pop that 6\n@THAT\nD=M\n@6\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToTemp()
        {
            classUnderTest.Translate(popToTemp)
                .Should().Be("// pop temp 4\n@5\nD=A\n@4\nD=D+A\n@SP\nM=M-1\nA=M\nA=M\nD=D+A\nA=D-A\nD=D-A\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToPointer0()
        {
            classUnderTest.Translate(popToPointer0)
                .Should().Be("// pop pointer 0\n@SP\nM=M-1\nA=M\nD=M\n@THIS\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToPointer1()
        {
            classUnderTest.Translate(popToPointer1)
                .Should().Be("// pop pointer 1\n@SP\nM=M-1\nA=M\nD=M\n@THAT\nM=D\n");
        }

        [TestMethod]
        public void ShouldTranslatePopToStatic()
        {
            classUnderTest.Translate(popToStatic)
                .Should().Be("// pop static 5\n@SP\nM=M-1\nA=M\nD=M\n@Foo.5\nM=D\n");
        }
    }

    #endregion

    #region WhenTranslatingArithmeticInstructions

    [TestClass]
    public class WhenTranslatingArithmeticInstructions
    {
        private Translator classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator();
        }

        [TestMethod]
        public void ShouldTranslateAdd()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Add })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D+M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateSub()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Sub })
                .Should().Be("// sub\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=M-D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateAnd()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.And })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D&M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateOr()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Or })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D|M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateNeg()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Neg })
                .Should().Be("// neg\n@SP\nM=M-1\nA=M\nD=M\nM=-D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateNot()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Not })
                .Should().Be("// not\n@SP\nM=M-1\nA=M\nD=M\nM=!D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateEq()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Eq, LineNumber = 11 })
                .Should().Be("// eq\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@EQ_11\nD;JEQ\nD=-1\n(EQ_11)\nD=!D\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateLt()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Lt, LineNumber = 12 })
                .Should().Be("// lt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_12\nD;JLT\nD=0\n@DONE_12\n0;JMP\n(YES_12)\nD=-1\n(DONE_12)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateGt()
        {
            classUnderTest.Translate(new LineOfCode { Instruction = InstructionType.Gt, LineNumber = 13 })
                .Should().Be("// gt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_13\nD;JGT\nD=0\n@DONE_13\n0;JMP\n(YES_13)\nD=-1\n(DONE_13)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }
    }

    #endregion

    #region WhenTranslatingBranchingInstructions

    [TestClass]
    public class WhenTranslatingBranchingInstructions
    {
        private Translator classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator();
        }

        [TestMethod]
        public void ShouldTranslateLabel()
        {
            classUnderTest.Translate(new LineOfCode { FileName = "Foo", Instruction = InstructionType.Label, Label = "START", VmCode = "label START" })
                .Should().Be("// label START\n(Foo$START)\n");
        }

        [TestMethod]
        public void ShouldTranslateGoTo()
        {
            classUnderTest.Translate(new LineOfCode { FileName = "Foo", Instruction = InstructionType.Goto, Label = "somewhere", VmCode = "goto somewhere" })
                .Should().Be("// goto somewhere\n@Foo$somewhere\n0;JMP\n");
        }

        [TestMethod]
        public void ShouldTranslateIfGoto()
        {
            classUnderTest.Translate(new LineOfCode { FileName = "Foo", Instruction = InstructionType.IfGoto, Label = "start", VmCode = "if-goto start" })
                .Should().Be("// if-goto start\n@SP\nM=M-1\nA=M\nD=M\n@Foo$start\nD;JNE\n");
        }
    }

    #endregion

    #region WhenTranslatingFunctionInstructions

    [TestClass]
    public class WhenTranslatingFunctionInstructions
    {
        private Translator classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator();
        }

        [TestMethod]
        public void ShouldTranslateFunctionWithNoLocalVariables()
        {
            LineOfCode loc = new LineOfCode
            {
                VmCode = "function myFunction 0",
                Instruction = InstructionType.Function,
                FunctionName = "myFunction",
                FileName = "Foo",
                Value = 0
            };
            classUnderTest.Translate(loc)
                .Should().Be("// function myFunction 0\n(Foo.myFunction)\n");
        }

        [TestMethod]
        public void ShouldTranslateFunctionWithASingleLocalVariable()
        {
            LineOfCode loc = new LineOfCode {
                VmCode = "function myFunction 1",
                Instruction = InstructionType.Function,
                FunctionName = "myFunction",
                FileName = "Foo",
                Value = 1
            };
            classUnderTest.Translate(loc)
                .Should().Be("// function myFunction 1\n(Foo.myFunction)\n@SP\nA=M\nM=0\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateFunctionWithMoreThanOneLocalVariable()
        {
            LineOfCode loc = new LineOfCode
            {
                VmCode = "function myFunction 3",
                Instruction = InstructionType.Function,
                FunctionName = "myFunction",
                FileName = "Foo",
                Value = 3
            };
            classUnderTest.Translate(loc)
                .Should().Be("// function myFunction 3\n(Foo.myFunction)\n@3\nD=A\n(Foo.myFunction.init)\n@SP\nA=M\nM=0\n@SP\nM=M+1\nD=D-1\n@Foo.myFunction.init\nD;JNE\n");
        }

        [TestMethod]
        public void ShouldTranslateFunctionReturn()
        {
            LineOfCode loc = new LineOfCode
            {
                VmCode = "return",
                Instruction = InstructionType.Return,
                FunctionName = "myFunction"
            };
            classUnderTest.Translate(loc)
                .Should().Contain("endFrame");
        }

        [TestMethod]
        public void ShouldTranslateFunctionCall()
        {
            LineOfCode loc = new LineOfCode
            {
                VmCode = "call myFunction 2",
                Instruction = InstructionType.Call,
                FunctionName = "myFunction",
                FileName = "Foo",
                Value = 2,
                LineNumber = 123
            };
            classUnderTest.Translate(loc)
                .Should().Contain("@Foo.myFunction.return.123");
        }
    }

    #endregion
}
