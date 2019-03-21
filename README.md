# ClrAnalyzer
.NET library for hooking and dumping Clr

Article with the descripntion can be found [here](https://github.com/GeorgePlotnikov/georgeplotnikov.github.io/blob/master/articles/just-in-time-hooking.md)

## Example of output

```
native size of code: 8
IL code: 0000012D20C61E30
method attribs: 4800000
	CORINFO_FLG_CONSTRUCTOR: This method is an instance or type initializer
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
native size of code: 12
IL code: 0000012D20C61E30
method attribs: 14000050
	CORINFO_FLG_FINAL
	CORINFO_FLG_VIRTUAL
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
	CORINFO_FLG_DONT_INLINE: The method should not be inlined
native size of code: 14
IL code: 0000012D20C61570
method attribs: 4000060
	CORINFO_FLG_SYNCH
	CORINFO_FLG_VIRTUAL
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
native size of code: 20
IL code: 0000012D20C62140
method attribs: 4010008
	CORINFO_FLG_STATIC
	CORINFO_FLG_FORCEINLINE: The method should be inlined if possible
	CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks
```
