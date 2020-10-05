﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vmr.Instructions
{
    public static class Linker
    {
        public static void LinkLabels(List<object> code, LableInfo lableInfo)
        {
            for (int i = 0; i < code.Count; i++)
            {
                if(lableInfo.TryGetCallSite(i, out var label))
                {
                    code[i] = lableInfo.GetTarget(label).Value;
                }
            }
        }
    }
}
