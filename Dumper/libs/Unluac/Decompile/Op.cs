using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnluacNET
{
    public sealed class Opcode
    {
        // TODO: Optimize method
        public static string CodePointToString(Op opcode, LInstruction code)
        {
            var name = opcode.GetType().Name;

            switch (opcode)
            {
            // A
            case Op.CLOSE:
            case Op.LOADKX:
                    return String.Format("{0} {1}",
                        name, code.A);

            // A_B
            case Op.MOVE:
            case Op.LOADNIL:
            case Op.GETUPVAL:
            case Op.SETUPVAL:
            case Op.UNM:
            case Op.NOT:
            case Op.LEN:
            case Op.RETURN:
            case Op.VARARG:
                    return String.Format("{0} {1} {2}",
                        name, code.A, code.B);

            // A_C
            case Op.TEST:
            case Op.TFORLOOP:
            case Op.TFORCALL:
                    return String.Format("{0} {1} {2}",
                        name, code.A, code.C);
            
            // A_B_C
            case Op.LOADBOOL:
            case Op.GETTABLE:
            case Op.SETTABLE:
            case Op.NEWTABLE:
            case Op.SELF:
            case Op.ADD:
            case Op.SUB:
            case Op.MUL:
            case Op.DIV:
            case Op.MOD:
            case Op.POW:
            case Op.CONCAT:
            case Op.EQ:
            case Op.LT:
            case Op.LE:
            case Op.TESTSET:
            case Op.CALL:
            case Op.TAILCALL:
            case Op.SETLIST:
            case Op.GETTABUP:
            case Op.SETTABUP:
                    return String.Format("{0} {1} {2} {3}",
                        name,
                        code.A,
                        code.B,
                        code.C);

            // A_Bx
            case Op.LOADK:
            case Op.GETGLOBAL:
            case Op.SETGLOBAL:
            case Op.CLOSURE:
                    return String.Format("{0} {1} {2}",
                        name, code.A, code.Bx);

            // A_sBx
            case Op.FORLOOP:
            case Op.FORPREP:
                    return String.Format("{0} {1} {2}",
                        name, code.A, code.sBx);

            // Ax
            case Op.EXTRAARG:
                return String.Format("{0} <Ax>", name);

            // sBx
            case Op.JMP:
                return String.Format("{0} {1}",
                    name, code.sBx);

            default:
                return name;
            }
        }
    }

    public enum Op
    {
        LOADNIL,
        MUL,
        SETTABLE,
        LE,
        CLOSE,
        NEWTABLE,
        LOADK,
        POW,
        GETTABLE,
        TESTSET,
        LT,
        EQ,
        MOVE,
        SETGLOBAL,
        GETUPVAL,
        LOADBOOL,
        DIV,
        RETURN,
        ADD,
        GETGLOBAL,
        CONCAT,
        CALL,
        TFORLOOP,
        CLOSURE,
        FORPREP,
        SETLIST,
        TAILCALL,
        FORLOOP,
        SETUPVAL,
        JMP,
        MOD,
        NOT,
        SELF,
        UNM,
        TEST,
        LEN,
        SUB,
        VARARG,
        // Lua 5.2 Opcodes
        LOADKX,
        GETTABUP,
        SETTABUP,
        TFORCALL,
        EXTRAARG
    }

    /*===========================================================================
      Notes:
      (*) In OP_CALL, if (B == 0) then B = top. C is the number of returns - 1,
          and can be 0: OP_CALL then sets `top' to last_result+1, so
          next open instruction (OP_CALL, OP_RETURN, OP_SETLIST) may use `top'.

      (*) In OP_VARARG, if (B == 0) then use actual number of varargs and
          set top (like in OP_CALL with C == 0).

      (*) In OP_RETURN, if (B == 0) then return up to `top'

      (*) In OP_SETLIST, if (B == 0) then B = `top';
          if (C == 0) then next `instruction' is real C

      (*) For comparisons, A specifies what condition the test should accept
          (true or false).

      (*) All `skips' (pc++) assume that next instruction is a jump
    ===========================================================================*/
}
