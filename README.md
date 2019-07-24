# ClrAnalyzer
.NET library for hooking and dumping Clr

The article with the descripntion can be found [here](https://github.com/GeorgePlotnikov/georgeplotnikov.github.io/blob/master/articles/just-in-time-hooking.md)

The video with the demo of usage can be found [here](https://www.youtube.com/watch?v=NqHGiBimD7I)

The :ru: video from the DotNet meetup can be found [here](https://youtu.be/39fOc4Jr8lE) with the :uk: [slides](https://speakerdeck.com/dotnetru/gieorghii-plotnikov-just-in-time-hooking)

## Example of output

```
native size of code: 8
IL code: 000001C923D7B380
===
IL_0000: ldarg.0
IL_0001: call <0x06000002>
IL_0006: nop
IL_0007: ret
===
method attribs: 4800000
===
	CORINFO_FLG_CONSTRUCTOR: This method is an instance or type initializer
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
===
native size of code: 12
IL code: 000001C923D7A5F0
===
IL_0000: nop
IL_0001: ldarg.1
IL_0002: conv.r8
IL_0003: call <0x0a00000e>
IL_0008: stloc.0
IL_0009: ldloc.0
IL_000a: conv.i4
IL_000b: ldarg.2
IL_000c: mul
IL_000d: stloc.1
IL_000e: br.s IL_0010
IL_0010: ldloc.1
IL_0011: ret
===
method attribs: 14000050
===
	CORINFO_FLG_FINAL
	CORINFO_FLG_VIRTUAL
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
	CORINFO_FLG_DONT_INLINE: The method should not be inlined
===
native size of code: 14
IL code: 000001C923D7AAC0
===
IL_0000: nop
IL_0001: ldarg.1
IL_0002: conv.r8
IL_0003: call <0x0a00000e>
IL_0008: stloc.0
IL_0009: ldloc.0
IL_000a: conv.i4
IL_000b: ldarg.2
IL_000c: mul
IL_000d: ldc.i4.2
IL_000e: mul
IL_000f: stloc.1
IL_0010: br.s IL_0012
IL_0012: ldloc.1
IL_0013: ret
===
method attribs: 4000060
===
	CORINFO_FLG_SYNCH
	CORINFO_FLG_VIRTUAL
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
===
native size of code: 20
IL code: 000001C923D7B070
===
IL_0000: nop
IL_0001: ldarg.0
IL_0002: conv.r8
IL_0003: call <0x0a00000e>
IL_0008: stloc.0
IL_0009: ldloc.0
IL_000a: conv.i4
IL_000b: ldarga.s 0x01
IL_000d: ldarg.0
IL_000e: call <0x0a00000f>
IL_0013: call <0x0a000010>
IL_0018: mul
IL_0019: ldc.i4.3
IL_001a: mul
IL_001b: stloc.1
IL_001c: br.s IL_001e
IL_001e: ldloc.1
IL_001f: ret
===
method attribs: 4010008
===
	CORINFO_FLG_STATIC
	CORINFO_FLG_FORCEINLINE: The method should be inlined if possible
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
===
```
