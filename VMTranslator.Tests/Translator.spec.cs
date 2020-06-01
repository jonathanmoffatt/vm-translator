using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VMTranslator.Tests
{
    #region WhenTranslatingPushOperations

    [TestClass]
    public class WhenTranslatingPushOperations
    {
        private Translator classUnderTest;
        private readonly LineOfCode pushFromLocal = new LineOfCode
        {
            VmCode = "push local 3",
            Command = Command.Push,
            Segment = Segment.Local,
            Value = 3
        };
        private readonly LineOfCode pushFromArgument = new LineOfCode
        {
            VmCode = "push argument 2",
            Command = Command.Push,
            Segment = Segment.Argument,
            Value = 2
        };
        private readonly LineOfCode pushFromThis = new LineOfCode
        {
            VmCode = "push this 5",
            Command = Command.Push,
            Segment = Segment.This,
            Value = 5
        };
        private readonly LineOfCode pushFromThat = new LineOfCode
        {
            VmCode = "push that 6",
            Command = Command.Push,
            Segment = Segment.That,
            Value = 6
        };
        private readonly LineOfCode pushConstant = new LineOfCode
        {
            VmCode = "push constant 17",
            Command = Command.Push,
            Segment = Segment.Constant,
            Value = 17
        };
        private readonly LineOfCode pushFromTemp = new LineOfCode
        {
            VmCode = "push temp 4",
            Command = Command.Push,
            Segment = Segment.Temp,
            Value = 4
        };
        private readonly LineOfCode pushFromPointer0 = new LineOfCode
        {
            VmCode = "push pointer 0",
            Command = Command.Push,
            Segment = Segment.Pointer,
            Value = 0
        };
        private readonly LineOfCode pushFromPointer1 = new LineOfCode
        {
            VmCode = "push pointer 1",
            Command = Command.Push,
            Segment = Segment.Pointer,
            Value = 1
        };
        private readonly LineOfCode pushFromStatic = new LineOfCode
        {
            VmCode = "push static 5",
            Command = Command.Push,
            Segment = Segment.Static,
            Value = 5
        };

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator("Foo");
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

    #region WhenTranslatingPopOperations

    [TestClass]
    public class WhenTranslatingPopOperations
    {
        private Translator classUnderTest;
        private readonly LineOfCode popToLocal = new LineOfCode
        {
            VmCode = "pop local 3",
            Command = Command.Pop,
            Segment = Segment.Local,
            Value = 3
        };
        private readonly LineOfCode popToArgument = new LineOfCode
        {
            VmCode = "pop argument 2",
            Command = Command.Pop,
            Segment = Segment.Argument,
            Value = 2
        };
        private readonly LineOfCode popToThis = new LineOfCode
        {
            VmCode = "pop this 5",
            Command = Command.Pop,
            Segment = Segment.This,
            Value = 5
        };
        private readonly LineOfCode popToThat = new LineOfCode
        {
            VmCode = "pop that 6",
            Command = Command.Pop,
            Segment = Segment.That,
            Value = 6
        };
        private readonly LineOfCode popToTemp = new LineOfCode
        {
            VmCode = "pop temp 4",
            Command = Command.Pop,
            Segment = Segment.Temp,
            Value = 4
        };
        private readonly LineOfCode popToPointer0 = new LineOfCode
        {
            VmCode = "pop pointer 0",
            Command = Command.Pop,
            Segment = Segment.Pointer,
            Value = 0
        };
        private readonly LineOfCode popToPointer1 = new LineOfCode
        {
            VmCode = "pop pointer 1",
            Command = Command.Pop,
            Segment = Segment.Pointer,
            Value = 1
        };
        private readonly LineOfCode popToStatic = new LineOfCode
        {
            VmCode = "pop static 5",
            Command = Command.Pop,
            Segment = Segment.Static,
            Value = 5
        };

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator("Foo");
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

    #region WhenTranslatingArithmeticOperations

    [TestClass]
    public class WhenTranslatingArithmeticOperations
    {
        private Translator classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Translator("Foo");
        }

        [TestMethod]
        public void ShouldTranslateAdd()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Add })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D+M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateSub()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Sub })
                .Should().Be("// sub\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=M-D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateAnd()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.And })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D&M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateOr()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Or })
                .Should().Be("// add\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nM=D|M\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateNeg()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Neg })
                .Should().Be("// neg\n@SP\nM=M-1\nA=M\nD=M\nM=-D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateNot()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Not })
                .Should().Be("// not\n@SP\nM=M-1\nA=M\nD=M\nM=!D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateEq()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Eq, LineNumber = 11 })
                .Should().Be("// eq\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@EQ_11\nD;JEQ\nD=-1\n(EQ_11)\nD=!D\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateLt()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Lt, LineNumber = 12 })
                .Should().Be("// lt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_12\nD;JLT\nD=0\n@DONE_12\n0;JMP\n(YES_12)\nD=-1\n(DONE_12)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }

        [TestMethod]
        public void ShouldTranslateGt()
        {
            classUnderTest.Translate(new LineOfCode { Command = Command.Gt, LineNumber = 13 })
                .Should().Be("// gt\n@SP\nM=M-1\nA=M\nD=M\n@SP\nM=M-1\nA=M\nD=M-D\n@YES_13\nD;JGT\nD=0\n@DONE_13\n0;JMP\n(YES_13)\nD=-1\n(DONE_13)\n@SP\nA=M\nM=D\n@SP\nM=M+1\n");
        }
    }

    #endregion
}
