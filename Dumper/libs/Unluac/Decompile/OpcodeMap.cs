using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnluacNET
{
    public class OpcodeMap
    {
        /*
        ** masks for instruction properties. The format is:
        ** bits 0-1: op mode
        ** bits 2-3: C arg mode
        ** bits 4-5: B arg mode
        ** bit 6: instruction set register A
        ** bit 7: operator is a test
        */
        private static int opmode(byte T, byte A, OpArgMask B, OpArgMask C, OpMode M)
        {
            return (((T) << 7) | ((A) << 6) | (((byte)B) << 4) | (((byte)C) << 2) | ((byte)M));
        }

        public static readonly int[] luaP_opmodes = { 0x8C, 0xA8, 0x28, 0x68, 0x00, 0x94, 0x89, 0xA8, 0xAC, 0xDC, 0x68, 0x68, 0x8C, 0x09, 0x84, 0x94, 0xA8, 0x04, 0xA8, 0x89, 0xBC, 0x94, 0x50, 0x85, 0x8E, 0x14, 0x94, 0x8E, 0x04, 0x0E, 0xA8, 0x8C, 0xAC, 0x8C, 0xDC, 0x8C, 0xA8, 0x84 };
        public static readonly int[] luaP_opmodes2 = {
          /*       T  A    B                 C                 mode             opcode       */
            opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC)    /* OP_LOADNIL */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_MUL */
           ,opmode(0, 0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_SETTABLE */
           ,opmode(1, 0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_LE */
           ,opmode(0, 0, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABC)    /* OP_CLOSE */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC)    /* OP_NEWTABLE */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx)    /* OP_LOADK */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_POW */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC)    /* OP_GETTABLE */
           ,opmode(1, 1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC)    /* OP_TESTSET */
           ,opmode(1, 0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_LT */
           ,opmode(1, 0, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_EQ */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC)    /* OP_MOVE */
           ,opmode(0, 0, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx)    /* OP_SETGLOBAL */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)    /* OP_GETUPVAL */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC)    /* OP_LOADBOOL */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_DIV */
           ,opmode(0, 0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)    /* OP_RETURN */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_ADD */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx)    /* OP_GETGLOBAL */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC)    /* OP_CONCAT */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC)    /* OP_CALL */
           ,opmode(1, 0, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC)    /* OP_TFORLOOP */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx)    /* OP_CLOSURE */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx)   /* OP_FORPREP */
           ,opmode(0, 0, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC)    /* OP_SETLIST */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC)    /* OP_TAILCALL */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx)   /* OP_FORLOOP */
           ,opmode(0, 0, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)    /* OP_SETUPVAL */
           ,opmode(0, 0, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx)   /* OP_JMP */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_MOD */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC)    /* OP_NOT */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC)    /* OP_SELF */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC)    /* OP_UNM */
           ,opmode(1, 1, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC)    /* OP_TEST */
           ,opmode(0, 1, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC)    /* OP_LEN */
           ,opmode(0, 1, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC)    /* OP_SUB */
           ,opmode(0, 1, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC)    /* OP_VARARG */
        };
        
        private Op[] m_map;

        public Op this[int opcode]
        {
            get { return GetOp(opcode); }
        }

        public Op GetOp(int opcode)
        {
            if (opcode >= 0 && opcode < m_map.Length)
            {
                return m_map[opcode];
            }
            else
            {
                throw new ArgumentOutOfRangeException("opcode", opcode,
                    "The specified opcode exceeds the boundaries of valid opcodes.");
            }
        }

        public OpMode GetOpMode(int m)
        {
            return (OpMode)(luaP_opmodes[m] & 3);
        }

        public OpArgMask GetBMode(int m)
        {
            return (OpArgMask)((luaP_opmodes[m] >> 4) & 3);
        }

        public OpArgMask GetCMode(int m)
        {
            return (OpArgMask)((luaP_opmodes[m] >> 2) & 3);
        }

        public bool TestAMode(int m)
        {
            return (luaP_opmodes[m] & (1 << 6)) == 1;
        }

        public bool TestTMode(int m)
        {
            return (luaP_opmodes[m] & (1 << 7)) == 1;
        }

        public OpcodeMap(int version)
        {
            if (version == 0x51)
            {
                m_map = new Op[38] {
                    Op.LOADNIL,
                    Op.MUL,
                    Op.SETTABLE,
                    Op.LE,
                    Op.CLOSE,
                    Op.NEWTABLE,
                    Op.LOADK,
                    Op.POW,
                    Op.GETTABLE,
                    Op.TESTSET,
                    Op.LT,
                    Op.EQ,
                    Op.MOVE,
                    Op.SETGLOBAL,
                    Op.GETUPVAL,
                    Op.LOADBOOL,
                    Op.DIV,
                    Op.RETURN,
                    Op.ADD,
                    Op.GETGLOBAL,
                    Op.CONCAT,
                    Op.CALL,
                    Op.TFORLOOP,
                    Op.CLOSURE,
                    Op.FORPREP,
                    Op.SETLIST,
                    Op.TAILCALL,
                    Op.FORLOOP,
                    Op.SETUPVAL,
                    Op.JMP,
                    Op.MOD,
                    Op.NOT,
                    Op.SELF,
                    Op.UNM,
                    Op.TEST,
                    Op.LEN,
                    Op.SUB,
                    Op.VARARG,
                };
            }
            else
            {
                m_map = new Op[40] {
                    Op.MOVE,
                    Op.LOADK,
                    Op.LOADKX,
                    Op.LOADBOOL,
                    Op.LOADNIL,
                    Op.GETUPVAL,
                    Op.GETTABUP,
                    Op.GETTABLE,
                    Op.SETTABUP,
                    Op.SETUPVAL,
                    Op.SETTABLE,
                    Op.NEWTABLE,
                    Op.SELF,
                    Op.ADD,
                    Op.SUB,
                    Op.MUL,
                    Op.DIV,
                    Op.MOD,
                    Op.POW,
                    Op.UNM,
                    Op.NOT,
                    Op.LEN,
                    Op.CONCAT,
                    Op.JMP,
                    Op.EQ,
                    Op.LT,
                    Op.LE,
                    Op.TEST,
                    Op.TESTSET,
                    Op.CALL,
                    Op.TAILCALL,
                    Op.RETURN,
                    Op.FORLOOP,
                    Op.FORPREP,
                    Op.TFORCALL,
                    Op.TFORLOOP,
                    Op.SETLIST,
                    Op.CLOSURE,
                    Op.VARARG,
                    Op.EXTRAARG
                };
            }
        }
    }
}
