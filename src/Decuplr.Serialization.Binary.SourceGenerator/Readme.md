# Decuplr.Serialization.Binary.SourceGenerator

This library is responsible for generating code that binds with the BinarySerializer.

## Limitations
This paragh shows a list of limitations that this library cannot current do.

- ~~Nested Partial Class is not supported.~~ It's freaking done bich! but we still need to verify it
  - It's just too much hashle to deal with it
  - First you need to check if it's parent class is also partial, which much invoke make to it's syntax and lookup
- Nested Class using private or protected marked as BinaryFormat.
  - Sorry pal, we can't create a formatter if we can't see it
  - Also, should we simply ignore this on go on our merry way?
  - Or should we dump a error
  - I'm thinking an warning on compilation level
- Private fields that are not declared publicly writtable, or readonly properties.
  - We will kindly require you to use "partial" keyword to help you create a constructor for you
  - No, we won't accept yourself writting one. Just add a partial to your class, not that hard.
- Not field backed property marked as [Index]
  - For example : `public int Example => somewhereValue;`
  - How are we going to write to that?!
- Const field marked as [Index]
  - Emmm...., we should allow this, since it's.... constant, wouldn't change
- Help us create a exact constructor that takes the same parameter....
  - Not cool! We will issue a compiler error

### Mapping

There are few ways to go about mapping data.

#### 1. Compile Time Mapping

Compile time mapping is when the source generator is figuring it how