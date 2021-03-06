﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vmr.Common;
using Vmr.Runtime.Vm;
using Xunit;

namespace Vmr.Runtime.Tests.Internal.Vm
{
    public class VirtualMachineTests
    {
        [Fact]
        public void Pop_remove_data_from_stack()
        {
            // Assert
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(1);
            builder.Pop();
            var instructions = builder.GetBinaryProgram();

            var vm = new VirtualMachine();

            // Act
            vm.Execute(instructions);
            var actualResult = vm.GetStack();

            // Assert
            Assert.Empty(actualResult);
        }

        [Fact]
        public void Ldc_i4_load_number_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(1);
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, 0x01);
        }

        [Fact]
        public void Ldstr_load_string_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldstr("test");
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, "test");
        }

        [Fact]
        public void Ldcstr_load_string_with_spaces_from_arg()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldstr("test test");
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, "test test");
        }

        [Fact]
        public void Add_load_two_numbers_then_sum()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(1);
            builder.Ldc_i4(2);
            builder.Add();
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, 3);
        }

        [Fact]
        public void Br_branch_with_one_jump()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Br("start");
            builder.Pop();
            builder.Label("start");
            builder.Ldc_i4(1);
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, 1);
        }

        [Fact]
        public void Br_branch_with_two_jump()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Br("start");
            builder.Label("pop");
            builder.Pop();
            builder.Br("end");
            builder.Label("start");
            builder.Ldc_i4(0);
            builder.Ldc_i4(1);
            builder.Ldc_i4(2);
            builder.Add();
            builder.Br("pop");
            builder.Label("end");
            builder.Nop();
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, 0);
        }

        [Fact]
        public void Ceq_compare_two_numbers()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(2);
            builder.Ldc_i4(2);
            builder.Ceq();
            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, 1);
        }

        [Fact]
        public void Brfalse_compare_and_write_equality()
        {
            var builder = new CodeBuilder();
            builder.Method("main", isEntryPoint: true);
            builder.Ldc_i4(0);
            builder.Ldc_i4(1);
            builder.Ceq();
            builder.Brfalse("false");
            builder.Ldstr("Equals");
            builder.Br("halt");
            builder.Label("false");
            builder.Ldstr("Not equals");
            builder.Label("halt");
            builder.Nop();

            var instructions = builder.GetBinaryProgram();

            ExecNoDataSingleResultTest(instructions, "Not equals");
        }

        private void ExecNoDataSingleResultTest<TResult>(byte[] instructions, TResult expected)
        {
            // Assert
            var vm = new VirtualMachine();

            // Act
            vm.Execute(instructions);
            var actualResult = vm.GetStack();

            // Assert
            Assert.NotEmpty(actualResult);
            var result = Assert.Single(actualResult);
            var number = Assert.IsType<TResult>(result);
            Assert.Equal(expected, number);
        }
    }
}
