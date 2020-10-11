# Vmr project

Minimalistic and simplified abstract low level language with tooling and runtime.

## Features

- MSIL based low level language.
- Compilation to portable, binary file format.
- Stack based virtual machine that can execute binary file format.

## Sample

```msil
IL_0000:  ldc.i4 0
IL_0005:  stloc 0

IL_000A:  ldstr "Looped"
IL_0012:  ldloc 0
IL_0017:  ldc.i4 9
IL_001C:  ceq
IL_001D:  brtrue IL_0037
IL_0022:  ldloc 0
IL_0027:  ldc.i4 1
IL_002C:  add
IL_002D:  stloc 0
IL_0032:  br IL_000A

IL_0037:  nop
```