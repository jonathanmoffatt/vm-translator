using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VMTranslator.Tests
{
    #region WhenTranslatingPushToAssembly

    [TestClass]
    public class WhenTranslatingPushToAssembly
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
            classUnderTest.Translate(pushFromLocal).Should().Be(
@"// push local 3
@LCL
D=M
@3
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushFromArgument()
        {
            classUnderTest.Translate(pushFromArgument).Should().Be(
@"// push argument 2
@ARG
D=M
@2
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushFromThis()
        {
            classUnderTest.Translate(pushFromThis).Should().Be(
@"// push this 5
@THIS
D=M
@5
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushFromThat()
        {
            classUnderTest.Translate(pushFromThat).Should().Be(
@"// push that 6
@THAT
D=M
@6
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushConstant()
        {
            classUnderTest.Translate(pushConstant).Should().Be(
@"// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushTemp()
        {
            classUnderTest.Translate(pushFromTemp).Should().Be(
@"// push temp 4
@5
D=M
@4
A=D+A
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushPointer0()
        {
            classUnderTest.Translate(pushFromPointer0).Should().Be(
@"// push pointer 0
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

        [TestMethod]
        public void ShouldTranslatePushPointer1()
        {
            classUnderTest.Translate(pushFromPointer1).Should().Be(
@"// push pointer 1
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1
");
        }

    }

    #endregion

    #region WhenTranslatingPopToAssembly

    [TestClass]
    public class WhenTranslatingPopToAssembly
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
            classUnderTest.Translate(popToLocal).Should().Be(
@"// pop local 3
@LCL
D=M
@3
D=D+A
@SP
M=M-1
A=M
A=M
D=D+A
A=D-A
D=D-A
M=D
");
        }

        [TestMethod]
        public void ShouldTranslatePopToArgument()
        {
            classUnderTest.Translate(popToArgument).Should().Be(
@"// pop argument 2
@ARG
D=M
@2
D=D+A
@SP
M=M-1
A=M
A=M
D=D+A
A=D-A
D=D-A
M=D
"
);
        }

        [TestMethod]
        public void ShouldTranslatePopToThis()
        {
            classUnderTest.Translate(popToThis).Should().Be(
@"// pop this 5
@THIS
D=M
@5
D=D+A
@SP
M=M-1
A=M
A=M
D=D+A
A=D-A
D=D-A
M=D
"
);
        }

        [TestMethod]
        public void ShouldTranslatePopToThat()
        {
            classUnderTest.Translate(popToThat).Should().Be(
@"// pop that 6
@THAT
D=M
@6
D=D+A
@SP
M=M-1
A=M
A=M
D=D+A
A=D-A
D=D-A
M=D
"
);
        }

        [TestMethod]
        public void ShouldTranslatePopToTemp()
        {
            classUnderTest.Translate(popToTemp).Should().Be(
@"// pop temp 4
@5
D=M
@4
D=D+A
@SP
M=M-1
A=M
A=M
D=D+A
A=D-A
D=D-A
M=D
"
);
        }

        [TestMethod]
        public void ShouldTranslatePopToPointer0()
        {
            classUnderTest.Translate(popToPointer0).Should().Be(
@"// pop pointer 0
@SP
M=M-1
A=M
D=M
@THIS
M=D
");
        }

        [TestMethod]
        public void ShouldTranslatePopToPointer1()
        {
            classUnderTest.Translate(popToPointer1).Should().Be(
@"// pop pointer 1
@SP
M=M-1
A=M
D=M
@THAT
M=D
");
        }
    }

    #endregion
}
